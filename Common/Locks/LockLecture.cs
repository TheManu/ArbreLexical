﻿using System;
using System.Threading;
using Common.Exceptions;
using Common.Ioc;

namespace Common.Locks
{
    internal class LockLecture : LockBase
    {
        public LockLecture(
            ReaderWriterLockSlim lockeur) : base(lockeur)
        {
            lockeur.EnterReadLock();
        }

        public override void LibererLock()
        {
            try
            {
                lockeur.ExitReadLock();
            }
            catch (Exception ex)
            {
                throw EncapsulerEtGererException<ExceptionTechnique>(
                    ex);
            }
        }
    }
}