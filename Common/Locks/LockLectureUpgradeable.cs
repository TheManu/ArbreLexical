using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Exceptions;
using Common.Ioc;

namespace Common.Locks
{
    internal class LockLectureUpgradeable : LockBase, ILockDisposableUpgradeable
    {
        public LockLectureUpgradeable(
            ReaderWriterLockSlim lockeur) : base(lockeur)
        {
            lockeur.EnterUpgradeableReadLock();
        }

        public override void LibererLock()
        {
            try
            {
                lockeur.ExitUpgradeableReadLock();
            }
            catch (Exception ex)
            {
                throw EncapsulerEtGererException<ExceptionTechnique>(
                    ex);
            }
        }

        public ILockDisposable RecupererLockEcriture()
        {
            return new LockEcriture(
                lockeur);
        }
    }
}
