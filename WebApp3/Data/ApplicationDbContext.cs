using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApp3.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder) // permet rel many to many
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<FilmUtilisateur>()
        .HasKey(t => new { t.IdUtilisateur, t.IdFilm});
    }
    public DbSet<Film> Films { get; set; }
    public DbSet<FilmUtilisateur> FilmsUtilisateur { get; set; }
}
