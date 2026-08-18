using Library.Entity.Abstract;
using Library.Entity.Concrete.Membership;

namespace Library.Entity.Concrete.Lookups;

public class MembershipApplicationStatus : LookupEntity
{

    //Üyelik başvuru durumları (PENDING, APPROVED, REJECTED) static enum yerine. böylece admin durumlar ekleyebilir
    //lookup entity deki str code için constanttaki static classları kullancaz


    // Bir başvuru durumuna ait birden fazla başvuru olabilir
    public ICollection<MembershipApplication> Applications { get; set; } = new List<MembershipApplication>();
}