using System;

namespace Common.Locks
{
    public interface ILockLectureEtEcriture
    {
        ILockDisposable RecupererLockLecture();

        ILockDisposable RecupererLockEcriture();
    }

    public interface ILockDisposable : IDisposable
    {
        void LibererLock();
    }

    public interface ILockDisposableUpgradeable : ILockDisposable
    {
        ILockDisposable RecupererLockEcriture();
    }
}