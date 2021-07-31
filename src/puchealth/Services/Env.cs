using System;

namespace puchealth.Services
{
    public class Env : IEnv
    {
        public Guid NewGuid()
        {
            return Guid.NewGuid();
        }
    }
}