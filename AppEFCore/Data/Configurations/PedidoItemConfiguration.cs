using AppEFCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace AppEFCore.Data.Configurations
{
    public class PedidoItemConfiguration : IEntityTypeConfiguration<PedidoItem>
    {
        public void Configure(EntityTypeBuilder<PedidoItem> builder)
        {
            builder.ToTable("PedidoItens");
            builder.HasKey(pi => pi.Id);
            builder.Property(pi => pi.Quantidade)
                .HasDefaultValue(1);
            builder.HasOne(pi => pi.Pedido)
                .WithMany(p => p.Itens)
                .HasForeignKey(pi => pi.PedidoId);


            // 1 (Pedido) : N (PedidoItem)
            //p.HasMany(p => p.Itens)
            //    .WithOne(p => p.Pedido)
            //    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}