using DigitalLibrary.Models.Entities;
using IdentityModel;
using Microsoft.EntityFrameworkCore;

namespace DigitalLibrary.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }

        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Accounting> Accountings { get; set; }
        public DbSet<Library> Libraries { get; set; }
        public DbSet<BookItem> Storage { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
    }
}
