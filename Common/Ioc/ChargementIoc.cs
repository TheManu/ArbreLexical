using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Locks;
using Common.Traces;
using Moq;

namespace Common.Ioc
{
    [Ioc]
    internal class ChargementIoc : IChargementIoc
    {
        public void Enregistrer(
            IFabrique fabrique)
        {
            try
            {
                fabrique.EnregistrerSingleton<ITraces>(() =>
                    new Mock<ITraces>().Object);

                fabrique.Enregistrer<ILockLectureEtEcriture>(() =>
                    new LockLectureEtEcriture());
                fabrique.Enregistrer<ILockLectureEtEcriture, LockRecursionPolicy>(o =>
                    new LockLectureEtEcriture(o));
            }
            catch (Exception ex)
            {
                Debug
                    .WriteLine(
                        ex.Message);

                fabrique
                    ?.RecupererInstance<ITraces>()
                    ?.PublierException(
                        ex);
            }
        }
    }
}
