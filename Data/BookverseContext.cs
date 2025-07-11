using Microsoft.EntityFrameworkCore;
using Bookverse.Models;

namespace Bookverse.Data
{
    public class BookverseContext : DbContext
    {
        public BookverseContext(DbContextOptions<BookverseContext> options) : base(options) { }
        public DbSet<Item> Items { get; set; }
    }
}
