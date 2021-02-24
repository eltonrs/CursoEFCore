using AppEFCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppEFCore.Data.Configurations
{
    public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("Clientes");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Nome)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(c => c.Telefone)
                .HasMaxLength(15);

            builder.Property(c => c.CEP)
                .HasColumnType("CHAR(8)")
                .IsRequired();

            builder.Property(c => c.Estado)
                .HasColumnType("CHAR(2)")
                .IsRequired();

            builder.Property(c => c.Cidade)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(c => c.Telefone)
                .HasDatabaseName("IDX_Cliente_Telefone");
        }
    }
}