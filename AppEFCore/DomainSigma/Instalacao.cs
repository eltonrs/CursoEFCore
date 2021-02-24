using System;

namespace AppEFCore.DomainSigma
{
    public class Instalacao
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public Guid? RegionalId { get; set; }
        public virtual Regional Regional { get; set; }
    }
}