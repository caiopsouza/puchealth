using System.Collections.Generic;
using puchealth.Services;

namespace Tests.Setup
{
    public class ProductSeed : IProductSeed
    {
        public IEnumerable<string> SqlSeed() => new[]
        {
            @"insert into ""Product"" (""Id"", ""Title"", ""Image"", ""Price"", ""ReviewScore"") values ('00000000-0001-0000-0000-000000000000', 'Cadeira para Auto Burigotto Matrix p/ Crianças', 'http://challenge-api.luizalabs.com/images/ddeb989e-53c4-e68b-aa93-6e43afddb797.jpg', '704.8', null);",
            @"insert into ""Product"" (""Id"", ""Title"", ""Image"", ""Price"", ""ReviewScore"") values ('00000000-0002-0000-0000-000000000000', 'Película Protetora para Samsung Galaxy S6', 'http://challenge-api.luizalabs.com/images/de2911eb-ce5c-e783-1ca5-82d0ccd4e3d8.jpg', '39.9', null);",
            @"insert into ""Product"" (""Id"", ""Title"", ""Image"", ""Price"", ""ReviewScore"") values ('00000000-0003-0000-0000-000000000000', 'Assento Sanitário Cristal Translúcido Century', 'http://challenge-api.luizalabs.com/images/1cc8ece1-895e-5d2a-de69-ad2d7884e722.jpg', '556.9', null);",
            @"insert into ""Product"" (""Id"", ""Title"", ""Image"", ""Price"", ""ReviewScore"") values ('00000000-0004-0000-0000-000000000000', 'Colcha/Cobre-Leito Patchwork Casal Camesa Curação', 'http://challenge-api.luizalabs.com/images/b5e7410b-cd4f-bb9d-3c95-49010fbee801.jpg', '159.75', '1.0')",
        };
    }
}