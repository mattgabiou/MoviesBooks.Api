/*
 * DbContext, DbSet<T>, DbContextOptions all live here.
 */
using Microsoft.EntityFrameworkCore;

/*
 * The entity model from Models/Movie.cs.
 */
using MoviesBooks.Api.Models;

namespace MoviesBooks.Api.Data;

/*
 * A DbContext is the gateway between C# and the database. Four jobs:
 *   1. Connection  — opens/closes the SQL connection; you never write
 *                    SqlConnection code.
 *   2. Translation — turns LINQ into T-SQL and rows back into objects.
 *   3. Change tracking — remembers every entity it hands you; on
 *                    SaveChanges() it emits INSERT/UPDATE/DELETE for what
 *                    actually changed.
 *   4. Unit of work — all changes since the context was created go out
 *                    together in one transaction.
 * One instance per HTTP request (registered as scoped in Program.cs);
 * never share one across requests.
 */
public class AppDbContext : DbContext
{
    /*
     * The options carry the provider (SQL Server) and connection string.
     * They come from AddDbContext in Program.cs, not from this file, which
     * keeps the context ignorant of *where* the database is. The
     * ": base(options)" passes them up to DbContext, which does the work.
     */
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /*
     * One DbSet per table you want to query. This property is what you'll
     * write "db.Movies.Where(...)" against. A DbSet is a queryable, not a
     * loaded list — nothing hits SQL until ToList/First/Count etc.
     *
     * "=> Set<Movie>()" is an expression-bodied getter that asks the base
     * class for the set it already knows about. The older
     * "{ get; set; } = null!" does the same job; this form avoids the
     * nullable-reference warning cleanly.
     */
    public DbSet<Movie> Movies => Set<Movie>();
}