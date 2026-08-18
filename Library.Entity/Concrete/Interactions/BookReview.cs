using Library.Entity.Abstract;
using Library.Entity.Concrete.Auth;
// book için using
namespace Library.Entity.Concrete.Interactions;

    public class BookReview : CreationAuditedEntity
    {
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    // book gelecek

    public int Rating { get; set; } // 1-5 arası puan
    //birçok yorum yapsa daha iyi sanki
    public string? Comment { get; set; } // Kullanıcı yorumu

}

