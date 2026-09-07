using Library.Entity.Abstract;

namespace Library.Entity.Concrete.Lookups;

public class MembershipType : LookupEntity
{
    // Öğrenci, akademik, halk vb. — enum değil lookup; admin kod değiştirmeden tür ekleyebilir.

    // tipe göre izin verilen gün şimdilik herkese 15
    public int MaxLoanDays { get; set; } = 15; 

    // TODO: teslim tarihi geçince ödenecek ama dökümanda 3 gün ücretsiz !!!! dikkat
    public decimal DailyPenaltyRate { get; set; }

    // 3 gün ücretsiz gerçi bu herekse 3 gün de değişebilir sonra aama ben sistem çalışıyor mu diye farklı günler yapcam seed de
    public int GracePeriodDays { get; set; } = 0;
}