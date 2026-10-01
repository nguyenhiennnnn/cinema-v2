using Cinema.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema.Infrastructure.Persistence.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");
        builder.HasKey(x => x.EmployeeId);
        builder.Property(x => x.HireDate).HasColumnType("date");
        builder.Property(x => x.IsActive).HasDefaultValue(true);

        builder
            .HasOne(x => x.User)
            .WithOne(u => u.Employee)
            .HasForeignKey<Employee>(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.UserId).IsUnique();

        builder
            .HasOne(x => x.Position)
            .WithMany(p => p.Employees)
            .HasForeignKey(x => x.PositionId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.PositionId).HasDatabaseName("IX_Employees_Position");
    }
}
