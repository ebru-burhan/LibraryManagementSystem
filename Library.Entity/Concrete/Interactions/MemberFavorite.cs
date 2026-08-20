using Library.Entity.Abstract;
using Library.Entity.Concrete.Membership;
using Library.Entity.Concrete.Catalog;

namespace Library.Entity.Concrete.Interactions;

    public class MemberFavorite : AuditableEntity
    {
    public int MemberId { get; set; }
    public Member Member { get; set; } = null!;

    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
}

