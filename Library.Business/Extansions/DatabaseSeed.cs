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

                //  Otomatik migration uygula
                context.Database.Migrate();

                //  ROLLERİ SEED ET
                if (!context.Roles.Any())
                {
                    context.Roles.AddRange(
                        new Role
                        {
                            Name = "Admin",
                            Description = "Sistem Yöneticisi",
                            Permissions = "view_dashboard,manage_members,view_loans,create_book",
                            CreatedAt = DateTime.Now,
                            ExternalId = Guid.NewGuid()
                        },
                        new Role
                        {
                            Name = "Librarian",
                            Description = "Kütüphane Personeli",
                            Permissions = "view_catalog",
                            CreatedAt = DateTime.Now,
                            ExternalId = Guid.NewGuid()
                        },
                        new Role
                        {
                            Name = "Member",
                            Description = "Kütüphane Üyesi",
                            Permissions = "view_loans,view_catalog",
                            CreatedAt = DateTime.Now,
                            ExternalId = Guid.NewGuid()
                        }
                    );
                    context.SaveChanges();
                }

                //  ADMIN KULLANICISINI VE ROLÜNÜ SEED ET (Gerçek Hashing ile)
                if (!context.Users.Any())
                {
                    hashingHelper.CreatePasswordHash("123456", out byte[] passwordHash, out byte[] passwordSalt);

                    var adminUser = new User
                    {
                        FirstName = "Sistem",
                        LastName = "Admin",
                        Email = "admin@lumina.com",
                        IdentityNumber = "11111111111",
                        DateOfBirth = new DateOnly(1990, 1, 1),
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
                        CreatedAt = DateTime.Now,
                        ExternalId = Guid.NewGuid()
                    };

                    context.Users.Add(adminUser);
                    context.SaveChanges();

                    var adminRole = context.Roles.FirstOrDefault(r => r.Name == "Admin");
                    if (adminRole != null)
                    {
                        context.UserRoles.Add(new UserRole
                        {
                            UserId = adminUser.Id,
                            RoleId = adminRole.Id
                        });
                        context.SaveChanges();
                    }
                }


                // ÜYELİK BAŞVURU DURUMLARINI SEED ET
                if (!context.Set<MembershipApplicationStatus>().Any())
                {
                    context.Set<MembershipApplicationStatus>().AddRange(
                        new MembershipApplicationStatus
                        {
                            Code = Statuses.MembershipApplication.Pending,
                            Name = "Beklemede",
                            CreatedAt = DateTime.Now,
                            ExternalId = Guid.NewGuid()
                        },
                        new MembershipApplicationStatus
                        {
                            Code = Statuses.MembershipApplication.Approved,
                            Name = "Onaylandı",
                            CreatedAt = DateTime.Now,
                            ExternalId = Guid.NewGuid()
                        },
                        new MembershipApplicationStatus
                        {
                            Code = Statuses.MembershipApplication.Rejected,
                            Name = "Reddedildi",
                            CreatedAt = DateTime.Now,
                            ExternalId = Guid.NewGuid()
                        },
                        new MembershipApplicationStatus
                        {
                            Code = Statuses.MembershipApplication.Incomplete,
                            Name = "Eksik Bilgi",
                            CreatedAt = DateTime.Now,
                            ExternalId = Guid.NewGuid()
                        }
                    );
                    context.SaveChanges();
                }







                //  ÖRNEK KİTAPLARI SEED ET
                if (!context.Books.Any())
                {
                    context.Books.AddRange(
                        new Book
                        {
                            Title = "Clean Code",
                            ISBN = "9780132350884",
                            Publisher = "Prentice Hall",
                            PublicationYear = 2008,
                            PageCount = 464,
                            CreatedAt = DateTime.Now,
                            ExternalId = Guid.NewGuid()
                        },
                        new Book
                        {
                            Title = "Design Patterns",
                            ISBN = "9780201633610",
                            Publisher = "Addison-Wesley",
                            PublicationYear = 1994,
                            PageCount = 395,
                            CreatedAt = DateTime.Now,
                            ExternalId = Guid.NewGuid()
                        },
                        new Book
                        {
                            Title = "Pragmatic Programmer",
                            ISBN = "9780201616224",
                            Publisher = "Addison-Wesley",
                            PublicationYear = 1999,
                            PageCount = 352,
                            CreatedAt = DateTime.Now,
                            ExternalId = Guid.NewGuid()
                        }
                    );
                    context.SaveChanges();
                }
            }
        }
    }
}