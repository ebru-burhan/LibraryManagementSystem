namespace Library.Entity.Abstract;

public interface ISoftDelete
{
    // veri kaybı olmasın da yapay zeka önerileri için 
    //Ceza hesaplamaları veya geçmiş raporlar asla bozulmaz
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
}