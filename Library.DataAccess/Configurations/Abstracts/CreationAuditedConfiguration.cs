using Library.Entity.Abstract;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.DataAccess.Configurations.Abstracts;

// T, CreationAuditedEntity kısıtlamasına sahip ve BaseConfiguration'dan miras alıyor!
public abstract class CreationAuditedConfiguration<T> : BaseConfiguration<T> where T : CreationAuditedEntity
{
    public override void Configure(EntityTypeBuilder<T> builder)
    {
        base.Configure(builder); // Id kuralını getir

        builder.Property(x => x.CreatedAt).IsRequired();
    }
}