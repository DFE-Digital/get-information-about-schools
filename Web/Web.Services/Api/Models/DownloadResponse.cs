namespace Web.Services.Api.Models
{
    public class DownloadResponse
    {
        public byte[] ResponseContents { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public bool NotFound { get; set; }
    }
}