using Microsoft.WindowsAPICodePack.Dialogs;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SdarotTV_Downloader
{
    public partial class MainForm : Form
    {
        readonly SeriesWebDriver seriesDriver;
        private static string downloadLocation;

        public MainForm()
        {
            InitializeComponent();
            //FindSuitableDriver();
            seriesDriver = CreateChromeDriver();
            downloadLocation = Consts.DEFAULT_DOWNLOAD_LOCATION;
            DownloadLocation_Label.Text = Consts.DEFAULT_DOWNLOAD_LOCATION;
        }

        private static void FindSuitableDriver()
        {
            string chromeVersion = Utils.GetExecutableBaseVersion(Utils.GetChromePath());
            if (!File.Exists(Consts.CHROME_DRIVER_FILE))
            {
                string srcDriver = Consts.CHROME_DRIVERS_FOLDER + chromeVersion + ".exe";
                File.Copy(srcDriver, Consts.CHROME_DRIVER_FILE, true);
            }
        }

        private static SeriesWebDriver CreateChromeDriver()
        {
            ChromeDriverService driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            ChromeOptions chromeOptions = new ChromeOptions();
            if (Consts.HEADLESS_DRIVER)
            {
                chromeOptions.AddArgument("headless");
            }
            IWebDriver chromeDriver = new ChromeDriver(driverService, chromeOptions);
            return new SeriesWebDriver(chromeDriver);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            seriesDriver.Quit();
        }

        private void Search_Button_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                SearchForSeries();
            });
        }

        private void SearchForSeries()
        {
            Invoke((MethodInvoker)delegate
            {
                Search_Button.Enabled = false;
                Info("Searching series...");
            });
            SearchResult sr = seriesDriver.SearchSeries(Search_TextBox.Text);
            Invoke((MethodInvoker)delegate
            {
                switch (sr)
                {
                    case SearchResult.NotFound:
                        Error(Consts.SERIES_NOT_FOUND);
                        break;
                    case SearchResult.Found:
                        Info("Loading series...");
                        break;
                    case SearchResult.NoEpisodes:
                        Error(Consts.SERIES_NO_EPISODES);
                        break;
                    case SearchResult.SearchCanceled:
                        Error(Consts.SEARCH_CANCELED);
                        break;
                    default:
                        break;
                }
            });
            if (sr == SearchResult.Found)
            {
                SeriesFound();
            }
            Invoke((MethodInvoker)delegate
            {
                Search_Button.Enabled = true;
            });
        }

        private void SeriesFound()
        {
            string seriesName = seriesDriver.GetSeriesName();
            string[] seasonsNames = seriesDriver.GetSeasonsNames();
            Invoke((MethodInvoker)delegate
            {
                SeriesName_Label.Text = seriesName;
                FirstEpisodeSeason_ComboBox.Items.Clear();
                FirstEpisodeSeason_ComboBox.Items.AddRange(seasonsNames);
                FirstEpisodeSeason_ComboBox.SelectedIndex = 0;
                DownloadEpisodes_RadioButton.Checked = true;
                EpisodesAmount_NumericUpDown.Value = 1;
                Download_Panel.Enabled = true;
                Info("");
            });
        }

        private void FirstEpisodeSeason_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DownloadMethod_GroupBox.Enabled = false;
            FirstEpisode_GroupBox.Enabled = false;
            Task.Run(() =>
            {
                ReloadEpisodesList();
                ReloadEpisodesAmount();
                Invoke((MethodInvoker)delegate
                {
                    FirstEpisode_GroupBox.Enabled = true;
                    DownloadMethod_GroupBox.Enabled = true;
                });
            });
        }

        private void ReloadEpisodesAmount()
        {
            if (DownloadSeason_RadioButton.Checked)
            {
                int seasonIndex = 0;
                Invoke((MethodInvoker)delegate
                {
                    seasonIndex = FirstEpisodeSeason_ComboBox.SelectedIndex;
                });
                int episodesAmount = seriesDriver.GetSeasonEpisodesAmount(seasonIndex);
                Invoke((MethodInvoker)delegate
                {
                    EpisodesAmount_NumericUpDown.Maximum = episodesAmount;
                    EpisodesAmount_NumericUpDown.Value = episodesAmount;
                });
            }
            else if (DownloadEpisodes_RadioButton.Checked)
            {
                Invoke((MethodInvoker)delegate
                {
                    EpisodesAmount_NumericUpDown.Maximum = 100;
                    EpisodesAmount_NumericUpDown.Value = 1;
                });
            }
            else if (DownloadSeries_RadioButton.Checked)
            {
                Invoke((MethodInvoker)delegate
                {
                    EpisodesAmount_NumericUpDown.Maximum = 100;
                    EpisodesAmount_NumericUpDown.Value = 1;
                });
            }
        }

        private void ReloadEpisodesList()
        {
            int seasonIndex = 0;
            Invoke((MethodInvoker)delegate
            {
                seasonIndex = FirstEpisodeSeason_ComboBox.SelectedIndex;
            });
            string[] episodesNames = seriesDriver.GetSeasonEpisodesNames(seasonIndex);
            Invoke((MethodInvoker)delegate
            {
                FirstEpisodeEpisode_ComboBox.Items.Clear();
                FirstEpisodeEpisode_ComboBox.Items.AddRange(episodesNames);
                FirstEpisodeEpisode_ComboBox.SelectedIndex = 0;
            });
        }

        private void DownloadEpisodes_RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (DownloadEpisodes_RadioButton.Checked)
            {
                DownloadMethod_GroupBox.Enabled = false;
                FirstEpisode_GroupBox.Enabled = false;
                Task.Run(() =>
                {
                    ReloadEpisodesList();
                    ReloadEpisodesAmount();
                    Invoke((MethodInvoker)delegate
                    {
                        FirstEpisodeSeason_ComboBox.Enabled = true;
                        FirstEpisodeEpisode_ComboBox.Enabled = true;
                        EpisodesAmount_NumericUpDown.Enabled = true;
                        FirstEpisode_GroupBox.Enabled = true;
                        DownloadMethod_GroupBox.Enabled = true;
                    });
                });
            }
        }

        private void DownloadSeason_RadioNutton_CheckedChanged(object sender, EventArgs e)
        {
            if (DownloadSeason_RadioButton.Checked)
            {
                DownloadMethod_GroupBox.Enabled = false;
                FirstEpisode_GroupBox.Enabled = false;
                EpisodesAmount_NumericUpDown.Enabled = false;
                Task.Run(() =>
                {
                    ReloadEpisodesList();
                    ReloadEpisodesAmount();
                    Invoke((MethodInvoker)delegate
                    {
                        FirstEpisodeSeason_ComboBox.Enabled = true;
                        FirstEpisodeEpisode_ComboBox.Enabled = false;
                        FirstEpisode_GroupBox.Enabled = true;
                        DownloadMethod_GroupBox.Enabled = true;
                    });
                });
            }
        }

        private void DownloadSeries_RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (DownloadSeries_RadioButton.Checked)
            {
                DownloadMethod_GroupBox.Enabled = false;
                FirstEpisode_GroupBox.Enabled = false;
                EpisodesAmount_NumericUpDown.Enabled = false;
                Task.Run(() =>
                {
                    ReloadEpisodesList();
                    ReloadEpisodesAmount();
                    Invoke((MethodInvoker)delegate
                    {
                        FirstEpisodeSeason_ComboBox.SelectedIndex = 0;
                        FirstEpisodeSeason_ComboBox.Enabled = false;
                        FirstEpisodeEpisode_ComboBox.Enabled = false;
                        FirstEpisode_GroupBox.Enabled = true;
                        DownloadMethod_GroupBox.Enabled = true;
                    });
                });
            }
        }

        private void Download_Button_Click(object sender, EventArgs e)
        {
            if (DownloadEpisodes_RadioButton.Checked)
            {
                seriesDriver.DownloadEpisodes(FirstEpisodeSeason_ComboBox.SelectedIndex, FirstEpisodeEpisode_ComboBox.SelectedIndex, Convert.ToInt32(EpisodesAmount_NumericUpDown.Value), downloadLocation);
            }
            else if (DownloadSeason_RadioButton.Checked)
            {
                seriesDriver.DownloadSeason(FirstEpisodeSeason_ComboBox.SelectedIndex, downloadLocation, seriesDriver.GetSeasonsNames()[FirstEpisodeSeason_ComboBox.SelectedIndex]);
            }
            else if (DownloadSeries_RadioButton.Checked)
            {
                seriesDriver.DownloadSeries(downloadLocation);
            }
        }

        private void ChangeDirectory_Button_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                downloadLocation = dialog.FileName;
                DownloadLocation_Label.Text = Utils.TruncateString(downloadLocation, Consts.MAX_PATH_CHARS);
            }
        }

        private void Search_TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)System.Windows.Forms.Keys.Enter)
            {
                Task.Run(() =>
                {
                    SearchForSeries();
                });
            }
        }

        private void Info(string info)
        {
            InfoMessage_Label.ForeColor = Consts.INFO_COLOR;
            InfoMessage_Label.Text = info;
        }

        private void Error(string error)
        {
            InfoMessage_Label.ForeColor = Consts.ERROR_COLOR;
            InfoMessage_Label.Text = error;
        }
    }
}
