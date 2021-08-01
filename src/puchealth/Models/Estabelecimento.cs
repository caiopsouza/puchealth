using System;

namespace puchealth.Models
{
    public class Estabelecimento
    {
        public Guid Id { get; set; }

        public string Nome { get; set; }

        public string RazaoSocial { get; set; }

        public EstabelecimentoTipo Tipo { get; set; }

        public Guid EnderecoId { get; set; }
        public Endereco Endereco { get; set; }
    }
}