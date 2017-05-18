namespace Edubase.Services.Domain
{
    public enum DownloadType
    {
        xlsx,
        csv
    }

    public class DownloadDto
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int FileSizeInBytes { get; set; }
        public string Tag { get; set; }
    }
}
