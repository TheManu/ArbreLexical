using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Locks
{
    internal class LockLectureEtEcriture : ILockLectureEtEcriture
    {
        private readonly ReaderWriterLockSlim lockeur;

        public LockLectureEtEcriture()
        {
            lockeur = new ReaderWriterLockSlim();
        }

        public LockLectureEtEcriture(
            LockRecursionPolicy recursionStatut)
        {
            lockeur = new ReaderWriterLockSlim(
                recursionStatut);
        }

        public ILockDisposable RecupererLockEcriture()
        {
            return new LockEcriture(
                lockeur);
        }

        public ILockDisposable RecupererLockLecture()
        {
            return new LockLecture(
                lockeur);
        }
    }
}
