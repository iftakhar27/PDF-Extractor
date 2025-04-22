using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PFD_Extractor.Server.Data;
using PFD_Extractor.Server.Models;
using PFD_Extractor.Server.Services;
using System.Reflection.Metadata;

namespace PFD_Extractor.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PDFController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PdfService _pdfService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;

        public PDFController(ApplicationDbContext context, PdfService pdfService, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _pdfService = pdfService;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> uploadPdf(IFormFile formFile)
        {
            if (formFile == null || formFile.ContentType != "application/pdf")
                return BadRequest("Invalid file type.");

            var metadata = await _pdfService.ExtractMetadataAsync(formFile);
            _context.PdfMetadata.Add(metadata);
            await _context.SaveChangesAsync();

            return Ok(metadata);
        }

        [HttpGet("metadata")]
        public async Task<IActionResult> GetMetadata()
        {
            var metadataList = await _context.PdfMetadata.ToListAsync();            
            return Ok(metadataList);
        }


        [HttpGet("metadata1")]
        public IActionResult GetMataData()
        {
            var documents = new List<PdfMetadata>();
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var query = "SELECT Id, Title, Author FROM PdfMetadata";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        documents.Add(new PdfMetadata
                        {
                            //Id = reader.GetInt32(0),
                            //Title = reader.GetString(1),
                            //Author = reader.GetString(2)
                        });
                    }
                }
            }

            return Ok(documents);
        }
    }
}
