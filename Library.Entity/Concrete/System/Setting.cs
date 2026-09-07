using Library.Entity.Abstract;

namespace Library.Entity.Concrete.System;

public class Setting : AuditableEntity
{
    //"DailyPenaltyFee", "MaxLoanDays", "MaxExtensionCount"
    //ama membertype geldi ordan yaptık da gene bu kullanılır ya 
    public string Key { get; set; } = null!;

    // "15.00", "15", "2" (Her şeyi string tutup kullanırken dönüştüreceğiz)
    public string Value { get; set; } = null!;

    public string? Description { get; set; }

    // TODO: genel sistem ayarları için falan kullanılabilir gene kalsın. kütüphane adı, çalışma saatleri, global kısıtlar
}