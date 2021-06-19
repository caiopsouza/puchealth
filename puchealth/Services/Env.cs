using System;

namespace puchealth.Services
{
    public class Env : IEnv
    {
        public Guid NewGuid() => Guid.NewGuid();
    }
}