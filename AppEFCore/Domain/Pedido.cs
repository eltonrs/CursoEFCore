using AppEFCore.ValueObjects;
using System;
using System.Collections.Generic;

namespace AppEFCore.Domain
{
    public class Pedido
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public virtual Cliente Cliente { get; set; } // Propriedade de navegação (EF)
        public DateTime IniciadoEm { get; set; }
        public DateTime FinalizadoEM { get; set; }
        public TipoFrete TipoFrete { get; set; }
        public bool Status { get; set; }
        public string Observacao { get; set; }
        public virtual ICollection<PedidoItem> Itens { get; set; }
    }
}