using Library.Entity.Abstract;
using Library.Entity.Concrete.Membership;
namespace Library.Entity.Concrete.Interactions;

    public class MemberFavorite : BaseEntity
    {
    public int MemberId { get; set; }
    public Member Member { get; set; } = null!;

    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
}

