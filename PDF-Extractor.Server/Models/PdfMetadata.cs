namespace PFD_Extractor.Server.Models
{
    public class PdfMetadata
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int PageCount { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

}
