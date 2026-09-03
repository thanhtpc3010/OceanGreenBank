using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectService.Domain.Entity;

namespace ProjectService.Infrastructure.Persistence.Configurations;

/// <summary>Cấu hình bảng Translations — khóa (Key, Language) là duy nhất.</summary>
public class TranslationConfiguration : IEntityTypeConfiguration<Translation>
{
    public void Configure(EntityTypeBuilder<Translation> builder)
    {
        builder.Property(t => t.Key).HasMaxLength(200).IsRequired();
        builder.Property(t => t.Language).HasMaxLength(10).IsRequired();
        builder.Property(t => t.Value).IsRequired();

        builder.HasIndex(t => new { t.Key, t.Language }).IsUnique();
    }
}
