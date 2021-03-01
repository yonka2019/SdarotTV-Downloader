using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SdarotTV_Downloader
{
    public class SeriesWebDriver
    {
        public readonly IWebDriver webDriver;

        public SeriesWebDriver(IWebDriver webDriver)
        {
            this.webDriver = webDriver;
            Task.Run(() =>
            {
                webDriver.Navigate().GoToUrl(Consts.SITE_URL);
            });
        }

        public void Quit()
        {
            webDriver.Quit();
        }

        public SearchResult SearchSeries(string seriesName)
        {
            string seriesUrl = Consts.SEARCH_URL + seriesName;
            bool doubleBack = false;
            webDriver.Navigate().GoToUrl(seriesUrl);
            if (webDriver.Url.StartsWith(Consts.SEARCH_URL))
            {
                var results = webDriver.FindElements(By.CssSelector("div.col-lg-2.col-md-2.col-sm-4.col-xs-6"));
                if (results.Count > 0)
                {
                    SearchResultsForm resultsForm = new SearchResultsForm(results);
                    DialogResult dialogResult = resultsForm.ShowDialog();
                    if (dialogResult == DialogResult.OK)
                    {
                        int resultIndex = resultsForm.resultIndex;
                        webDriver.Navigate().GoToUrl(results[resultIndex].FindElement(By.TagName("a")).GetAttribute("href"));
                        doubleBack = true;
                    }
                    else
                    {
                        webDriver.Navigate().Back();
                        return SearchResult.SearchCanceled;
                    }
                }
                else
                {
                    webDriver.Navigate().Back();
                    return SearchResult.NotFound;
                }
            }
            if (webDriver.Url.StartsWith(Consts.SERIES_URL))
            {
                if (GetSeasonsAmount() > 0)
                {
                    return SearchResult.Found;
                }
                else
                {
                    webDriver.Navigate().Back();
                    if (doubleBack)
                    {
                        webDriver.Navigate().Back();
                    }
                    return SearchResult.NoEpisodes;
                }
            }
            webDriver.Navigate().Back();
            if (doubleBack)
            {
                webDriver.Navigate().Back();
            }
            return SearchResult.NotFound;
        }

        public string GetSeriesName()
        {
            return webDriver.FindElement(By.XPath("//*[@id=\"watchEpisode\"]/div[1]/div/h1/strong/span")).Text;
        }

        public string[] GetSeasonsNames()
        {
            List<string> names = new List<string>();
            IWebElement seasonsList = webDriver.FindElement(By.Id("season"));
            int i = 0;
            foreach (var season in seasonsList.FindElements(By.TagName("a")))
            {
                if (GetSeasonEpisodesAmount(i) > 0)
                {
                    names.Add(season.Text);
                }
                i++;
            }
            return names.ToArray();
        }

        private void NavigateToSeason(int seasonIndex)
        {
            webDriver.FindElement(By.Id("season")).FindElements(By.TagName("li"))[seasonIndex].Click();
        }

        public string[] GetSeasonEpisodesNames(int seasonIndex)
        {
            List<string> names = new List<string>();
            NavigateToSeason(seasonIndex);
            IWebElement episodesList = webDriver.FindElement(By.Id("episode"));
            foreach (var episode in episodesList.FindElements(By.TagName("a")))
            {
                names.Add(episode.Text);
            }
            return names.ToArray();
        }

        private int GetSeasonsAmount()
        {
            return GetSeasonsNames().Length;
        }

        public int GetSeasonEpisodesAmount(int seasonIndex)
        {
            return GetSeasonEpisodesNames(seasonIndex).Length;
        }

        public void NavigateToEpisode(Episode episode)
        {
            NavigateToSeason(episode.seasonIndex);
            webDriver.FindElement(By.Id("episode")).FindElements(By.TagName("li"))[episode.episodeIndex].Click();
        }

        public DialogResult DownloadEpisodes(int seasonIndex, int episodeIndex, int episodeAmount, string downloadLocation, string seasonName="")
        {
            DownloadForm downloadForm = new DownloadForm(this, seasonIndex, episodeIndex, episodeAmount, downloadLocation, seasonName);
            return downloadForm.ShowDialog();
        }

        public DialogResult DownloadSeason(int seasonIndex, string downloadLocation, string seasonName)
        {
            return DownloadEpisodes(seasonIndex, 0, GetSeasonEpisodesAmount(seasonIndex), downloadLocation, seasonName);
        }

        public void DownloadSeries(string downloadLocation)
        {
            string[] seasonNames = GetSeasonsNames();
            for (int i = 0; i < seasonNames.Length; i++)
            {
                DialogResult dr = DownloadSeason(i, downloadLocation, seasonNames[i]);
                if (dr == DialogResult.Cancel)
                {
                    break;
                }
            }
        }
    }
}
