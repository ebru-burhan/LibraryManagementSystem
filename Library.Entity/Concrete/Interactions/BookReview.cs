using Library.Entity.Abstract;
using Library.Entity.Concrete.Membership;
using Library.Entity.Concrete.Catalog;


namespace Library.Entity.Concrete.Interactions;

    public class BookReview : AuditableEntity
    {
    public int MemberId { get; set; }
    public Member Member { get; set; } = null!;

    public int BookId { get; set; }
    public Book Book { get; set; } = null!;

    public int Rating { get; set; } // 1-5 arası puan
    //birçok yorum yapsa daha iyi sanki
    public string? Comment { get; set; } // Kullanıcı yorumu

}

