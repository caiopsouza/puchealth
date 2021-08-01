using System;
using AutoMapper;
using puchealth.Models;

namespace puchealth.Views
{
    [AutoMap(typeof(Endereco))]
    public class EnderecoView
    {
        public Guid Id { get; set; }

        public string Rua { get; set; }

        public string Numero { get; set; }

        public string Bairro { get; set; }

        public string Cidade { get; set; }

        public string Estado { get; set; }

        public string CEP { get; set; }
    }
}