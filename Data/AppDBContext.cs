// DbContext, DbSet<T>, DbContextOptions all live here.
using Microsoft.EntityFrameworkCore;

// The entity class from Step 5.
using MoviesBooks.Api.Models;

namespace MoviesBooks.Api.Data;

// A DbContext is the unit of work: it opens the connection, tracks which
// objects you've loaded or changed, translates LINQ into SQL, and writes
// changes back on SaveChanges(). One instance per HTTP request (Step 6
// registers it that way); never share one across requests.
public class AppDbContext : DbContext
{
    // The options carry the provider (SQL Server) and connection string.
    // They come from AddDbContext in Program.cs, not from this file, which
    // keeps the context ignorant of *where* the database is. The
    // ": base(options)" passes them up to DbContext, which does the work.
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // One DbSet per table you want to query. This property is what you'll
    // write "db.Movies.Where(...)" against. Its name is also what EF uses to
    // guess the table name ("Movies"), which [Table] on the entity confirms.
    // "=> Set<Movie>()" is the modern shape that keeps the nullable
    // compiler happy; the older "{ get; set; } = null!" does the same job.
    public DbSet<Movie> Movies => Set<Movie>();
}