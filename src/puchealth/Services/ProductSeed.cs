using System.Collections.Generic;
using System.Linq;

namespace puchealth.Services
{
    public class ProductSeed : IProductSeed
    {
        public IEnumerable<string> SqlSeed()
        {
            return Enumerable.Empty<string>();
        }
    }
}