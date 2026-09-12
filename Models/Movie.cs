// [Required], [MaxLength], [Key] and other general-purpose validation
// attributes. Part of .NET itself, not EF; used by MVC model binding,
// Blazor forms, and EF alike.
using System.ComponentModel.DataAnnotations;

// [Table], [Column], [NotMapped] — attributes that describe how a class
// maps to a database schema. Also in .NET itself; EF Core reads them.
using System.ComponentModel.DataAnnotations.Schema;

// [Unicode], [Precision], [Index] and other EF Core-specific attributes.
// This one only exists once the EF Core package is installed, which is
// why it errored before Step 2 actually landed.
using Microsoft.EntityFrameworkCore;

namespace MoviesBooks.Api.Models;

// An ENTITY: a C# class that EF Core maps to one table row.
// Every property below maps to one column. EF matches by name unless told
// otherwise, so property names that already equal column names need nothing.
// [Table] pins the class to the real table name. Convention would guess
// "Movies" from the DbSet name in Step 6, so this is belt-and-suspenders.
[Table("Movie_Name")]
public class Movie
{
    // Convention: a property named <ClassName>Id is the primary key.
    // "MovieId" on class "Movie" qualifies, so no [Key] attribute needed.
    // is_identity = 1 in SQL, and EF's default for an int key is
    // "database generates it," so inserts will leave it out automatically.
    public int MovieId { get; set; }

    // varchar(100), nullable. string? = may be null.
    // EF assumes nvarchar (Unicode) for strings; [Unicode(false)] tells it
    // this column is varchar so query parameters match the column's type and
    // SQL Server doesn't do an implicit conversion on every WHERE.
    [MaxLength(100)]
    [Unicode(false)]
    public string? Name { get; set; }

    // int, nullable -> int? (nullable value type). Same story for the rest.
    public int? Year { get; set; }

    // Column name has an underscore; C# convention is PascalCase without it.
    // [Column] maps the C# name to the real column name. You could name the
    // property Created_On and skip the attribute, but the attribute keeps
    // your C# idiomatic while the database stays untouched.
    // NOT NULL in SQL -> plain DateTime, no ?.
    [Column("Created_On")]
    public DateTime CreatedOn { get; set; }

    [Column("Modified_On")]
    public DateTime? ModifiedOn { get; set; }

    [Column("Last_Watched_Date")]
    public DateTime? LastWatchedDate { get; set; }

    public int? Rating { get; set; }

    // bit -> bool. Nullable bit -> bool?.
    public bool? Lost { get; set; }
}