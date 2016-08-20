using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Services;

namespace Common.Locks
{
    internal abstract class LockBase : ServiceBase, ILockDisposable
    {
        protected readonly ReaderWriterLockSlim lockeur;

        public LockBase(
            ReaderWriterLockSlim lockeur)
        {
            this.lockeur = lockeur;
        }

        public void Dispose()
        {
            LibererLock();
        }

        public abstract void LibererLock();
    }
}
