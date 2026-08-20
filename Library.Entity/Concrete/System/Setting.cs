using Library.Entity.Abstract;

namespace Library.Entity.Concrete.System;

public class Setting : AuditableEntity
{
    //"DailyPenaltyFee", "MaxLoanDays", "MaxExtensionCount"
    public string Key { get; set; } = null!;

    // "15.00", "15", "2" (Her şeyi string tutup kullanırken dönüştüreceğiz)
    public string Value { get; set; } = null!;

    public string? Description { get; set; }
}