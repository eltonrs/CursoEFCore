using System;

namespace AppEFCore.DomainSigma
{
    public class Regional
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public Guid EmpresaId { get; set; }
        public virtual Empresa Empresa { get; set; }
    }
}