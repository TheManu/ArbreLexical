using System;
using System.Threading;
using Common.Exceptions;
using Common.Ioc;

namespace Common.Locks
{
    internal class LockEcriture : LockBase
    {
        public LockEcriture(
            ReaderWriterLockSlim lockeur) : base(lockeur)
        {
            lockeur.EnterWriteLock();
        }

        public override void LibererLock()
        {
            try
            {
                lockeur.ExitWriteLock();
            }
            catch (Exception ex)
            {
                throw EncapsulerEtGererException<ExceptionTechnique>(
                    ex);
            }
        }
    }
}