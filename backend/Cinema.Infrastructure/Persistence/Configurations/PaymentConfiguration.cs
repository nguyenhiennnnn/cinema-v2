using Cinema.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable(
            "Payments",
            t =>
            {
                t.HasCheckConstraint(
                    "CK_Payments_Method",
                    "\"PaymentMethod\" IN ('VNPay','Momo','Cash','Stripe')"
                );
                t.HasCheckConstraint("CK_Payments_Amount", "\"Amount\" >= 0");
                t.HasCheckConstraint(
                    "CK_Payments_Status",
                    "\"Status\" IN ('Pending','Success','Failed','Refunded')"
                );
            }
        );
        builder.HasKey(x => x.PaymentId);
        builder
            .Property(x => x.PaymentMethod)
            .HasConversion<string>()
            .HasColumnName("PaymentMethod")
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(x => x.TransactionId).HasMaxLength(150);
        builder.Property(x => x.GatewayTransactionId).HasMaxLength(150);
        builder.Property(x => x.GatewayResponseCode).HasMaxLength(20);
        builder.Property(x => x.Amount).HasColumnType("decimal(18,2)");
        builder
            .Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(Domain.Enums.PaymentStatus.Pending);
        builder.Property(x => x.PaidAt).HasColumnType("timestamp without time zone");
        builder
            .Property(x => x.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");
        builder.Property(x => x.RefundTransactionId).HasMaxLength(150);
        builder.Property(x => x.RefundedAmount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.RefundedAt).HasColumnType("timestamp without time zone");
        builder.Property(x => x.AmountReceived).HasColumnType("decimal(18,2)");
        builder.Property(x => x.ChangeGiven).HasColumnType("decimal(18,2)");

        builder
            .HasOne(x => x.Booking)
            .WithMany(bk => bk.Payments)
            .HasForeignKey(x => x.BookingId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.BookingId).HasDatabaseName("IX_Payments_Booking");
        builder.HasIndex(x => x.Status).HasDatabaseName("IX_Payments_Status");
    }
}
