using iText.Forms;
using iText.Kernel.Pdf;
using Microsoft.EntityFrameworkCore;
using PFD_Extractor.Server.Data;
using PFD_Extractor.Server.Models;
using System.Text;


namespace PFD_Extractor.Server.Services
{
    public class PdfService
    {
        private readonly ApplicationDbContext _context;

        public PdfService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<PdfMetadata> ExtractMetadataAsync(IFormFile file)
        {
            var result = new Dictionary<string, string>();

            using var stream = file.OpenReadStream();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            using var reader = new PdfReader(memoryStream);
            using var pdfDoc = new PdfDocument(reader);

            var textBuilder = new StringBuilder();
            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
            {
                textBuilder.Append(iText.Kernel.Pdf.Canvas.Parser.PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i)));
            }

            string content = textBuilder.ToString();
            var pdfMetadata = new PdfMetadata()
            {
                Author = ExtractValue(content, "Author:"),
                CreatedDate = Convert.ToDateTime(ExtractValue(content, "Date:")),
                Title = ExtractValue(content, "Title:"),
                PageCount = Convert.ToInt16(ExtractValue(content, "Count:")),
                FileName = file.FileName                
            };
            return pdfMetadata;
        }

        public async Task<List<PdfMetadata>> SearchPdfsAsync(string query)
        {
            var searchQuery = query.ToLower();
            return await _context.PdfMetadata
                .Where(p => p.Title.ToLower().Contains(searchQuery) || p.Author.ToLower().Contains(searchQuery))
                .ToListAsync();
        }
        private string ExtractValue(string content, string label)
        {
            var start = content.IndexOf(label);
            if (start == -1) return null;
            start += label.Length;
            var end = content.IndexOf('\n', start);
            if (end == -1) end = content.Length;
            return content.Substring(start, end - start).Trim();
        }

    }
}
