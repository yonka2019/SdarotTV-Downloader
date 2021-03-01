using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace SdarotTV_Downloader
{
    public partial class DownloadForm : Form
    {
        private readonly SeriesWebDriver webDriver;
        private CookieAwareWebClient client;
        private readonly int seasonIndex;
        private int episodeIndex;
        private readonly int episodeAmount;
        private readonly string downloadLocation;
        public static bool fileIsDownloading;
        private Thread downloadThread;
        private string episodePath;

        public DownloadForm(SeriesWebDriver webDriver, int seasonIndex, int episodeIndex, int episodeAmount, string downloadLocation, string seasonName="")
        {
            InitializeComponent();
            if (seasonName != "")
            {
                Text += " (Season " + seasonName + ")";
            }
            EpisodeLoad_ProgressBar.Maximum = Consts.PB_DURATION * Consts.PB_FPS;
            Overall_ProgressBar.Maximum = episodeAmount;
            OverallProgress_Label.Text = Utils.GetProgressString(0, episodeAmount);
            this.webDriver = webDriver;
            this.seasonIndex = seasonIndex;
            this.episodeIndex = episodeIndex;
            this.episodeAmount = episodeAmount;
            this.downloadLocation = downloadLocation;
        }

        private void DownloadEpisode(Episode episode, string downloadLocation)
        {
            webDriver.NavigateToEpisode(episode);
            int triesLeft = 3;
            while (true)
            {
                triesLeft -= 1;
                for (int i = 1; i <= Consts.PB_DURATION * Consts.PB_FPS; i++)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        EpisodeLoad_ProgressBar.Value = i;
                        EpisodeLoad_Label.Text = Utils.GetProgressString(i / Consts.PB_FPS, Consts.PB_DURATION, "s");
                    });
                    Thread.Sleep(TimeSpan.FromMilliseconds(1000 / Consts.PB_FPS));
                }
                try
                {
                    new WebDriverWait(webDriver.webDriver, TimeSpan.FromSeconds(10)).Until(ExpectedConditions.ElementToBeClickable(By.Id("proceed"))).Click();
                    break;
                }
                catch (WebDriverTimeoutException)
                {
                    if (triesLeft == 0)
                    {
                        return;
                    }
                    webDriver.webDriver.Navigate().Refresh();
                }
            }
            string episodeUrl = webDriver.webDriver.FindElement(By.Id(Consts.VIDEO_HTML_ID)).GetAttribute("src");
            CookieContainer cc = new CookieContainer();
            foreach (var cookie in webDriver.webDriver.Manage().Cookies.AllCookies)
            {
                cc.Add(new System.Net.Cookie(cookie.Name, cookie.Value) { Domain = cookie.Domain });
            }
            DownloadEpisodeFromWeb(episodeUrl, cc, downloadLocation);
        }

        private void DownloadEpisodeFromWeb(string episodeUrl, CookieContainer cookieContainer, string downloadLocation)
        {
            using (client = new CookieAwareWebClient())
            {
                client.CookieContainer = cookieContainer;
                client.Headers.Add("user-agent", "User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.190 Safari/537.36");
                // Assign the event to capture the progress percentage
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Client_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(Client_DownloadFileCompleted);

                fileIsDownloading = true;
                Invoke((MethodInvoker)delegate
                {
                    client.DownloadFileAsync(new Uri(episodeUrl), downloadLocation);
                });

                while (fileIsDownloading)
                {
                    Thread.Sleep(200);
                }
            }
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                File.Delete(episodePath);
            }
            else
            {
                fileIsDownloading = false;
            }
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            int megaBytesIn = Convert.ToInt32(e.BytesReceived / Consts.MB);
            int totalMegaBytes = Convert.ToInt32(e.TotalBytesToReceive / Consts.MB);
            int percentage = Convert.ToInt32(100 * megaBytesIn / totalMegaBytes);
            try
            {
                Invoke((MethodInvoker)delegate
                {
                    EpisodeDownload_ProgressBar.Value = percentage;
                    EpisodeDonwload_Label.Text = percentage.ToString() + "% (" + Utils.GetProgressString(megaBytesIn, totalMegaBytes, "MB") + ")";
                });
            }
            catch { }
        }

        private void DownloadEpisodes()
        {
            int downloaded = 0;
            string[] seasons = webDriver.GetSeasonsNames();
            string seriesDir = Path.Combine(downloadLocation, webDriver.GetSeriesName());
            for (int season = seasonIndex; season < seasons.Length && downloaded < episodeAmount; season++)
            {
                string seasonNumber = seasons[season].PadLeft(2, '0');
                string seasonDir = Path.Combine(seriesDir, "Season " + seasonNumber);
                string[] currSeasonEpisodesNames = webDriver.GetSeasonEpisodesNames(season);
                for (int episode = episodeIndex; episode < currSeasonEpisodesNames.Length && downloaded < episodeAmount; episode++)
                {
                    string episodeNumber = currSeasonEpisodesNames[episode].PadLeft(2, '0');
                    episodePath = Path.Combine(seasonDir, "Episode S" + seasonNumber + "E" + episodeNumber + ".mp4");
                    Invoke((MethodInvoker)delegate
                    {
                        EpisodeNumber_Label.Text = seasonNumber + "." + episodeNumber;
                        DownloadLocation_Label.Text = Utils.TruncateString(episodePath, Consts.MAX_PATH_CHARS);
                        EpisodeLoad_ProgressBar.Value = 0;
                        EpisodeDownload_ProgressBar.Value = 0;
                        EpisodeLoad_Label.Text = "";
                        EpisodeDonwload_Label.Text = "";
                    });
                    Directory.CreateDirectory(Path.GetDirectoryName(episodePath));
                    DownloadEpisode(new Episode(season, episode, currSeasonEpisodesNames[episode]), episodePath);
                    downloaded++;
                    Invoke((MethodInvoker)delegate
                    {
                        Overall_ProgressBar.Value = downloaded;
                        OverallProgress_Label.Text = Utils.GetProgressString(downloaded, episodeAmount);
                    });
                    
                }
                episodeIndex = 0;
            }
            Invoke((MethodInvoker)delegate
            {
                Close();
            });
        }

        private void DownloadForm_Load(object sender, EventArgs e)
        {
            downloadThread = new Thread(new ThreadStart(DownloadEpisodes));
            downloadThread.Start();
        }

        private void DownloadForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                client.CancelAsync();
            }
            catch { }
            downloadThread.Abort();
        }
    }

    public class CookieAwareWebClient : WebClient
    {
        public CookieContainer CookieContainer { get; set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);

            if (request is HttpWebRequest webRequest)
            {
                webRequest.CookieContainer = CookieContainer;
            }

            return request;
        }
    }
}
