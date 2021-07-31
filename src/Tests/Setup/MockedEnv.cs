using System;
using puchealth.Services;

namespace Tests.Setup
{
    public class MockedEnv : IEnv
    {
        private int _currentGuid;

        public MockedEnv()
        {
            _currentGuid = 0;
        }

        // GUIDs are sequential when testing to be deterministic and predictable.
        public Guid NewGuid()
        {
            _currentGuid++;
            return CreateGuid(_currentGuid);
        }

        public static Guid CreateGuid(int id)
        {
            return new(id, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        }
    }
}