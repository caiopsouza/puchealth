using System.Collections.Generic;

namespace puchealth.Services
{
    public interface IProductSeed
    {
        IEnumerable<string> SqlSeed();
    }
}