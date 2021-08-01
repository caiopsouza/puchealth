using System;
using AutoMapper;
using puchealth.Models;

namespace puchealth.Views
{
    [AutoMap(typeof(Estabelecimento))]
    public class EstabelecimentoView
    {
        public Guid Id { get; set; }

        public string Nome { get; set; }

        public string RazaoSocial { get; set; }

        public EstabelecimentoTipo Tipo { get; set; }

        public string Endereco { get; set; }
    }
}