namespace SdarotTV_Downloader
{
    public class Episode
    {
        public int seasonIndex;
        public int episodeIndex;
        public string name;

        public Episode(int season, int episode, string name)
        {
            seasonIndex = season;
            episodeIndex = episode;
            this.name = name;
        }
    }
}
