using DataBase.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataBase.Context;

public class MfrDbContext : DbContext
{
    public MfrDbContext(DbContextOptions<MfrDbContext> options) : base(options)
    {
    }

    public virtual required DbSet<User> User { get; set; }
    public virtual required DbSet<Role> Role { get; set; }
    public virtual required DbSet<UserRole> UserRole { get; set; }
    public virtual required DbSet<Campus> Campus { get; set; }
    public virtual required DbSet<UserCampus> UserCampus { get; set; }
    public virtual required DbSet<Category> Category { get; set; }
    public virtual required DbSet<CategoryDetails> CategoryDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        /* Thuộc tính chính */

        /* Khóa ngoại */
        modelBuilder.Entity<UserRole>().HasOne(d => d.User)
            .WithMany(p => p.UserRole)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserRole>().HasOne(d => d.Role)
            .WithMany(p => p.UserRole)
            .HasForeignKey(d => d.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserCampus>().HasOne(d => d.User)
            .WithMany(p => p.UserCampus)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserCampus>().HasOne(d => d.Campus)
            .WithMany(p => p.UserCampus)
            .HasForeignKey(d => d.CampusId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CategoryDetails>().HasOne(d => d.Category)
            .WithMany(p => p.CategoryDetails)
            .HasForeignKey(d => d.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}