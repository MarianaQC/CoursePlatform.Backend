using CoursePlatform.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CoursePlatform.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    private IDbContextTransaction _currentTransaction;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }
    
    public DbSet<Course> Courses { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(e => e.Status)
                .IsRequired();
            
            entity.Property(e => e.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);
            
            entity.Property(e => e.CreatedAt)
                .IsRequired();
            
            entity.Property(e => e.UpdatedAt)
                .IsRequired();
            
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.IsDeleted);
            
            entity.HasQueryFilter(e => !e.IsDeleted);
            
            entity.HasMany(e => e.Lessons)
                .WithOne(l => l.Course)
                .HasForeignKey(l => l.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(e => e.CourseId)
                .IsRequired();
            
            entity.Property(e => e.Order)
                .IsRequired();
            
            entity.Property(e => e.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);
            
            entity.Property(e => e.CreatedAt)
                .IsRequired();
            
            entity.Property(e => e.UpdatedAt)
                .IsRequired();
            
            entity.HasIndex(e => new { e.CourseId, e.Order })
                .IsUnique()
                .HasFilter("IsDeleted = 0");
            
            entity.HasIndex(e => e.IsDeleted);
            
            entity.HasQueryFilter(e => !e.IsDeleted);
        });
        
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(50);
            
            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(50);
            
            entity.Property(e => e.CreatedAt)
                .IsRequired();
            
            entity.Property(e => e.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);
        });
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
        
        return await base.SaveChangesAsync(cancellationToken);
    }
    
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            return;
        }

        _currentTransaction = await Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            await _currentTransaction?.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _currentTransaction?.RollbackAsync(cancellationToken);
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }
}