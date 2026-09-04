using Library.Business.Security.Hashing;
using Library.DataAccess.Contexts;
using Library.Entity.Concrete.Auth;
using Library.Entity.Concrete.Catalog;
using Library.Entity.Concrete.Lookups;
using Library.Entity.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Business.SeedData
{
    public static class DatabaseSeed
    {
        public static void Seed(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var hashingHelper = scope.ServiceProvider.GetRequiredService<IHashingHelper>();

                context.Database.Migrate();

                // 1. ROLLERİ SEED ET
                if (!context.Roles.Any())
                {
                    context.Roles.AddRange(
                        new Role { Name = "Admin", Description = "Sistem Yöneticisi", Permissions = "view_dashboard,manage_members,view_loans,create_book,view_catalog", CreatedAt = DateTime.Now, ExternalId = Guid.NewGuid() },
                        new Role { Name = "Librarian", Description = "Kütüphane Personeli", Permissions = "view_catalog", CreatedAt = DateTime.Now, ExternalId = Guid.NewGuid() },
                        new Role { Name = "Member", Description = "Kütüphane Üyesi", Permissions = "view_loans,view_catalog", CreatedAt = DateTime.Now, ExternalId = Guid.NewGuid() }
                    );
                    context.SaveChanges();
                }

                // 2. ADMIN KULLANICISINI SEED ET
                if (!context.Users.Any())
                {
                    hashingHelper.CreatePasswordHash("123456", out byte[] passwordHash, out byte[] passwordSalt);

                    var adminUser = new User
                    {
                        FirstName = "Sistem",
                        LastName = "Admin",
                        Username = "admin",
                        Email = "admin@lumina.com",
                        IdentityNumber = "11111111111",
                        DateOfBirth = new DateOnly(1990, 1, 1),
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
                        IsKvkkApproved = true,
                        IsTermsAccepted = true,
                        CreatedAt = DateTime.Now,
                        ExternalId = Guid.NewGuid()
                    };
                    context.Users.Add(adminUser);
                    context.SaveChanges();

                    var adminRole = context.Roles.FirstOrDefault(r => r.Name == "Admin");
                    if (adminRole != null)
                    {
                        context.UserRoles.Add(new UserRole { UserId = adminUser.Id, RoleId = adminRole.Id });
                        context.SaveChanges();
                    }
                }

                // 3. LOOKUP TABLOLARI: ÜYELİK TİPLERİ VE DURUMLARI
                if (!context.Set<MembershipType>().Any())
                {
                    context.Set<MembershipType>().AddRange(
                        new MembershipType { Name = "Öğrenci", Code = MembershipTypes.Student },
                        new MembershipType { Name = "Akademik Personel", Code = MembershipTypes.Academic },
                        new MembershipType { Name = "Sivil/Halk", Code = MembershipTypes.Public }
                    );
                    context.SaveChanges();
                }

                if (!context.Set<MembershipApplicationStatus>().Any())
                {
                    context.Set<MembershipApplicationStatus>().AddRange(
                        new MembershipApplicationStatus { Code = Statuses.MembershipApplication.Pending, Name = "Beklemede", CreatedAt = DateTime.Now, ExternalId = Guid.NewGuid() },
                        new MembershipApplicationStatus { Code = Statuses.MembershipApplication.Approved, Name = "Onaylandı", CreatedAt = DateTime.Now, ExternalId = Guid.NewGuid() },
                        new MembershipApplicationStatus { Code = Statuses.MembershipApplication.Rejected, Name = "Reddedildi", CreatedAt = DateTime.Now, ExternalId = Guid.NewGuid() },
                        new MembershipApplicationStatus { Code = Statuses.MembershipApplication.Incomplete, Name = "Eksik Bilgi", CreatedAt = DateTime.Now, ExternalId = Guid.NewGuid() }
                    );
                    context.SaveChanges();
                }

                if (!context.Set<MemberStatus>().Any())
                {
                    context.Set<MemberStatus>().AddRange(
                        new MemberStatus { Code = Statuses.Member.Active, Name = "Aktif", Description = "Kütüphane hizmetlerini kullanabilir.", CreatedAt = DateTime.Now, ExternalId = Guid.NewGuid() },
                        new MemberStatus { Code = Statuses.Member.Passive, Name = "Pasif", Description = "Üyelik geçici olarak durdurulmuştur.", CreatedAt = DateTime.Now, ExternalId = Guid.NewGuid() },
                        new MemberStatus { Code = Statuses.Member.Suspended, Name = "Askıya Alınmış", Description = "Üyelik askıya alınmıştır.", CreatedAt = DateTime.Now, ExternalId = Guid.NewGuid() }
                    );
                    context.SaveChanges();
                }
                // 4. LOOKUP TABLOLARI: KİTAP KOPYASI, ÖDÜNÇ VE REZERVASYON DURUMLARI
                if (!context.Set<BookStatus>().Any())
                {
                    context.Set<BookStatus>().AddRange(
                        new BookStatus { Code = Statuses.BookCopy.Available, Name = "Rafta", CreatedAt = DateTime.Now, ExternalId = Guid.NewGuid() },
                        new BookStatus { Code = Statuses.BookCopy.OnLoan, Name = "Ödünç Verildi", CreatedAt = DateTime.Now, ExternalId = Guid.NewGuid() },
                        new BookStatus { Code = Statuses.BookCopy.InRepair, Name = "Tamirde", CreatedAt = DateTime.Now, ExternalId = Guid.NewGuid() },
                        new BookStatus { Code = Statuses.BookCopy.Lost, Name = "Kayıp", CreatedAt = DateTime.Now, ExternalId = Guid.NewGuid() }
                    );
                    context.SaveChanges();
                }

                if (!context.Set<LoanStatus>().Any())
                {
                    context.Set<LoanStatus>().AddRange(
                        new LoanStatus { Code = Statuses.Loan.Borrowed, Name = "Ödünç Alındı", CreatedAt = DateTime.Now, ExternalId = Guid.NewGuid() },
                        new LoanStatus { Code = Statuses.Loan.Approaching, Name = "Yaklaşıyor", CreatedAt = DateTime.Now, ExternalId = Guid.NewGuid() },
                        new LoanStatus { Code = Statuses.Loan.Overdue, Name = "Gecikti", CreatedAt = DateTime.Now, ExternalId = Guid.NewGuid() },
                        new LoanStatus { Code = Statuses.Loan.Critical, Name = "Kritik", CreatedAt = DateTime.Now, ExternalId = Guid.NewGuid() },
                        new LoanStatus { Code = Statuses.Loan.Returned, Name = "İade Edildi", CreatedAt = DateTime.Now, ExternalId = Guid.NewGuid() }
                    );
                    context.SaveChanges();
                }

                if (!context.Set<ReservationStatus>().Any())
                {
                    context.Set<ReservationStatus>().AddRange(
                        new ReservationStatus { Code = Statuses.Reservation.Waiting, Name = "Bekliyor", CreatedAt = DateTime.Now, ExternalId = Guid.NewGuid() },
                        new ReservationStatus { Code = Statuses.Reservation.Completed, Name = "Tamamlandı", CreatedAt = DateTime.Now, ExternalId = Guid.NewGuid() },
                        new ReservationStatus { Code = Statuses.Reservation.Cancelled, Name = "İptal Edildi", CreatedAt = DateTime.Now, ExternalId = Guid.NewGuid() },
                        new ReservationStatus { Code = Statuses.Reservation.Expired, Name = "Süresi Doldu", CreatedAt = DateTime.Now, ExternalId = Guid.NewGuid() }
                    );
                    context.SaveChanges();
                }

                // 5. TEST VERİSİ (Arayüz gelişene kadar süreci denemek için sadece birkaç tane)
                if (!context.Books.Any())
                {
                    context.Books.AddRange(
                        new Book { Title = "Clean Code", ISBN = "9780132350884", Publisher = "Prentice Hall", PublicationYear = 2008, PageCount = 464, CreatedAt = DateTime.Now, ExternalId = Guid.NewGuid() },
                        new Book { Title = "Design Patterns", ISBN = "9780201633610", Publisher = "Addison-Wesley", PublicationYear = 1994, PageCount = 395, CreatedAt = DateTime.Now, ExternalId = Guid.NewGuid() },
                        new Book { Title = "Pragmatic Programmer", ISBN = "9780201616224", Publisher = "Addison-Wesley", PublicationYear = 1999, PageCount = 352, CreatedAt = DateTime.Now, ExternalId = Guid.NewGuid() }
                    );
                    context.SaveChanges();
                }
            }
        }
    }
}