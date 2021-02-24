using AppEFCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppEFCore.Data.Configurations
{
    public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
    {
        public void Configure(EntityTypeBuilder<Produto> builder)
        {
            builder.ToTable("Produtos");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Descricao)
                //.HasColumnType("VARCHAR(100)") Como o tipo da propriedade é string, o EF cria com o tipo correspondente no MSSQLS
                .HasMaxLength(100) // Como o tipo da propriedade é string, o EF cria com o tipo correspondente no MSSQLS. Basta indicar o tamanho máximo (nesse caso será NVARCHAR(100)).
                .IsRequired();

            builder.Property(p => p.CodigoBarras)
                .HasColumnType("CHAR(100)");

            builder.Property(p => p.Valor)
                //.HasColumnType("DECIMAL(10, 2)")
                .HasPrecision(10, 2)
                .IsRequired();

            builder.Property(p => p.TipoProduto)
                //.HasConversion(
                //    v => v.ToString(),
                //    v => (TipoProduto)Enum.Parse(typeof(TipoProduto), v)
                //)
                .HasConversion<string>() // Simplesmente faz um ToString no enum
                .IsRequired();

            builder.Property(p => p.Ativo)
                .HasDefaultValue(true);
        }
    }
}