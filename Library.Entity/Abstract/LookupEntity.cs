namespace Library.Entity.Abstract;

public abstract class LookupEntity : AuditableEntity
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    // static enumdan kaçmak için 
}