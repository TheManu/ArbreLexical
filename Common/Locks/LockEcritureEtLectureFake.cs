using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Locks
{
    public class LockEcritureEtLectureFake : ILockLectureEtEcriture
    {
        public LockEcritureEtLectureFake()
        {
        }

        public LockEcritureEtLectureFake(
            LockRecursionPolicy recursionStatut)
        {
        }

        public ILockDisposable RecupererLockEcriture()
        {
            return new LockFake();
        }

        public ILockDisposable RecupererLockLecture()
        {
            return new LockFake();
        }

        private class LockFake : ILockDisposable
        {
            public void Dispose()
            {
            }

            public void LibererLock()
            {
            }
        }
    }
}
