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

            var metadata = await _pdfService.ExtractPdfMetadataAsync(formFile);
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

        [HttpGet("search")]
        public async Task<ActionResult<List<PdfMetadata>>> SearchPdfs(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search query cannot be empty.");
            }

            var result = await _pdfService.SearchPdfsAsync(query);

            if (!result.Any())
            {
                return NotFound("No PDFs found.");
            }

            return Ok(result);
        }
    }
}
