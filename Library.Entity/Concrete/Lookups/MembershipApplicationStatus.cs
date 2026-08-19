using Library.Entity.Abstract;

namespace Library.Entity.Concrete.Lookups;

public class MembershipApplicationStatus : LookupEntity
{

    //Üyelik başvuru durumları (PENDING, APPROVED, REJECTED) static enum yerine. böylece admin durumlar ekleyebilir
    //lookup entity deki str code için constanttaki static classları kullancaz


    // Bir başvuru durumuna ait birden fazla başvuru olabilir
    // TODO: bunu sonra araştır. devasa veri çekimi olurmuş
    //public ICollection<MembershipApplication> Applications { get; set; } = new List<MembershipApplication>();
}