using Microsoft.EntityFrameworkCore;
using PFD_Extractor.Server.Models;

namespace PFD_Extractor.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
        }

        public DbSet<PdfMetadata> PdfMetadata { get; set; }
    }
}
