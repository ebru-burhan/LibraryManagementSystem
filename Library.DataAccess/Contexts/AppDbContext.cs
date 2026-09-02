using Microsoft.EntityFrameworkCore;
using Library.Entity.Concrete.Auth;
using Library.Entity.Concrete.Catalog;
using Library.Entity.Concrete.Interactions;
using Library.Entity.Concrete.Lookups;
using Library.Entity.Concrete.Membership;
using Library.Entity.Concrete.Operations;
using Library.Entity.Concrete.System;
using Library.Entity.Abstract;
using System.Reflection;

namespace Library.DataAccess.Contexts;

// AppDbContext classı, tüm Entity sınıflarını EF Core'a " Hanım hanım bunlar benim veritabanı tablolarım" diye tanıtacak
// ve yazacağımız ayar (Configuration) dosyalarını otomatik bulmasını sağlayacak
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // 1. Auth Modülü
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

    // 2. Membership Modülü
    public DbSet<Member> Members { get; set; }
    public DbSet<MembershipApplication> MembershipApplications { get; set; }

    // 3. Catalog Modülü
    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<BookAuthor> BookAuthors { get; set; }
    public DbSet<BookCategory> BookCategories { get; set; }
    public DbSet<BookCopy> BookCopies { get; set; }

    // 4. Operations Modülü
    public DbSet<Loan> Loans { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Penalty> Penalties { get; set; }
    public DbSet<RenewalRequest> RenewalRequests { get; set; }
    public DbSet<LostBook> LostBooks { get; set; }

    // 5. Interactions Modülü
    public DbSet<MemberFavorite> MemberFavorites { get; set; }
    public DbSet<BookReview> BookReviews { get; set; }
    public DbSet<MemberReadingStatus> MemberReadingStatuses { get; set; }

    // 6. Lookups Modülü
    public DbSet<LoanStatus> LoanStatuses { get; set; }
    public DbSet<ReservationStatus> ReservationStatuses { get; set; }
    public DbSet<PenaltyType> PenaltyTypes { get; set; }
    public DbSet<BookStatus> BookStatuses { get; set; }
    public DbSet<MembershipType> MembershipTypes { get; set; }
    public DbSet<MembershipApplicationStatus> MembershipApplicationStatuses { get; set; }

    // 7. System Modülü
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Setting> Settings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurations klasörüne yazacağımız tüm ayar dosyalarını otomatik bul ve uygula!
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    //TARİHLERİ OTOMATİK YÖNETME (Zaman/UTC Sihri)
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        
        var creationEntries = ChangeTracker.Entries<CreationAuditedEntity>();
        foreach (var entry in creationEntries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
        }

        var auditableEntries = ChangeTracker.Entries<AuditableEntity>();
        foreach (var entry in auditableEntries)
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}