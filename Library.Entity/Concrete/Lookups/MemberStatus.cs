using Library.Entity.Abstract;

namespace Library.Entity.Concrete.Lookups;

public class MemberStatus : LookupEntity
{
    // Üye durumları (ACTIVE, PASSIVE, SUSPENDED). Enum yerine lookup; admin kod değiştirmeden durum ekleyebilir.
}
