using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Library.Entity.Abstract;

namespace Library.DataAccess.Configurations.Abstracts;

public abstract class BaseConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        // Tüm tablolarda ortak olan Primary Key kuralı
        builder.HasKey(x => x.Id);
    }
}