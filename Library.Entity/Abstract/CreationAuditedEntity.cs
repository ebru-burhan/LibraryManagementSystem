namespace Library.Entity.Abstract;

public abstract class CreationAuditedEntity : BaseEntity
{
    //ımmutbale olanlar meesela loglar için 
    //sunucu saati veya yerel saat farklılıkları engelledik utc.now
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedByUserId { get; set; }
}