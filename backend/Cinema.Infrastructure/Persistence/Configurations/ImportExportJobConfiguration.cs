using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema.Infrastructure.Persistence.Configurations;

public class ImportExportJobConfiguration : IEntityTypeConfiguration<ImportExportJob>
{
    public void Configure(EntityTypeBuilder<ImportExportJob> builder)
    {
        builder.ToTable(
            "ImportExportJobs",
            t =>
            {
                t.HasCheckConstraint("CK_ImportExportJobs_Type", "\"Type\" IN ('Import','Export')");
                t.HasCheckConstraint(
                    "CK_ImportExportJobs_Status",
                    "\"Status\" IN ('Queued','Running','Completed','Failed')"
                );
                t.HasCheckConstraint(
                    "CK_ImportExportJobs_Progress",
                    "\"Progress\" >= 0 AND \"Progress\" <= 100"
                );
                t.HasCheckConstraint(
                    "CK_ImportExportJobs_Rows",
                    "\"TotalRows\" >= 0 AND \"SuccessRows\" >= 0 AND \"FailedRows\" >= 0"
                );
            }
        );
        builder.HasKey(x => x.JobId);
        builder.Property(x => x.JobId).ValueGeneratedNever();
        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.Module).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Format).HasMaxLength(10).IsRequired();
        builder
            .Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(ImportExportJobStatus.Queued);
        builder.Property(x => x.Progress).HasDefaultValue(0);
        builder.Property(x => x.TotalRows).HasDefaultValue(0);
        builder.Property(x => x.SuccessRows).HasDefaultValue(0);
        builder.Property(x => x.FailedRows).HasDefaultValue(0);
        builder.Property(x => x.FileName).HasMaxLength(255);
        builder.Property(x => x.StoragePath).HasMaxLength(500);
        builder.Property(x => x.FileUrl).HasMaxLength(1000);
        builder.Property(x => x.ErrorMessage).HasMaxLength(2000);
        builder
            .Property(x => x.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasDefaultValueSql("NOW()");
        builder.Property(x => x.StartedAt).HasColumnType("timestamp without time zone");
        builder.Property(x => x.CompletedAt).HasColumnType("timestamp without time zone");
        builder.Property(x => x.ExpiresAt).HasColumnType("timestamp without time zone");

        builder
            .HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(x => new { x.CreatedByUserId, x.CreatedAt })
            .HasDatabaseName("IX_ImportExportJobs_User_CreatedAt");
        builder
            .HasIndex(x => new
            {
                x.Type,
                x.Module,
                x.Status,
            })
            .HasDatabaseName("IX_ImportExportJobs_Type_Module_Status");
        builder
            .HasIndex(x => x.ExpiresAt)
            .HasFilter("\"StoragePath\" IS NOT NULL")
            .HasDatabaseName("IX_ImportExportJobs_ExpiresAt_HasFile");
    }
}
