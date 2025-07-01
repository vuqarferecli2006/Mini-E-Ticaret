// ProductConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using E_Biznes.Domain.Entities;

namespace E_Biznes.Persistance.Configurations;

// ImageConfiguration.cs
public class ImageConfiguration : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Image_Url).IsRequired();

        builder.HasOne(i => i.Product)
               .WithMany(p => p.ProductImages)
               .HasForeignKey(i => i.ProductId);
    }
}
