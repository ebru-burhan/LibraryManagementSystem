using Library.Entity.Abstract;

namespace Library.Entity.Concrete.Lookups;

public class MembershipType : LookupEntity
{
    // Öğrenci, akademik, halk vb. — enum değil lookup; admin kod değiştirmeden tür ekleyebilir.
}
