//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

namespace AppEFCore.Domain
{
    //[Table("Clientes")] // Utilizado qdo o nome da propriedade da classe é diferente do nome da tabela no banco de dados.
    public class Cliente
    {
        /* Há duas formas de definir o modelo de dados:
         * 1- Fluent API (geralmente melhor)
         * 2- Data Annotations
         */


        // Com Fluent API, as definições do modelo de dados ficam em outras classes/arquivos. Tornando o acoplamento de código, extremamente baixo.
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Telefone { get; set; }

        public string CEP { get; set; }

        public string Cidade { get; set; }

        public string Estado { get; set; }

        public string Email { get; set; }

        // Com Data Annotations a classe que presenta a entida fica "suja"

        //[Key]
        //public int Id { get; set; }

        //[Required]
        //public string Nome { get; set; }

        //[Column("Phone")] // Utilizado qdo o nome da propriedade da classe é diferente do nome da coluna no banco de dados.
        //public string Telefone { get; set; }

        //public string CEP { get; set; }

        //public string Cidade { get; set; }

        //public string Estado { get; set; }
    }


}