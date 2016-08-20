using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Exceptions;
using Common.Helpers;
using Common.Ioc;

namespace Common.Services
{
    public abstract class ServiceBase
    {
        #region Protected Fields

        protected bool relancerException = true;

        #endregion Protected Fields

        #region Protected Properties

        protected IFabrique FabriqueInstance
        {
            get
            {
                return Fabrique.Instance;
            }
        }

        #endregion Protected Properties

        #region Protected Methods

        protected Exception EncapsulerEtGererException<T>(
            string message,
            Exception ex,
            bool forcerEncapsulation = false)
            where T : ExceptionBase
        {
            try
            {
                if (null == ex)
                {
                    return EncapsulerEtGererException<ExceptionTechnique>(
                        new ArgumentNullException(nameof(ex)));
                }

                var exEncapsulee = (ex as ExceptionBase);

                if (null != exEncapsulee)
                { // Exception déjà encapsulée => trace le message (si existe) + retourne l'exception

                    // Traces du message
                    if (StringHelper.EstNonNullEtNonVideEtNonEspaces(message))
                    {
                        FabriqueInstance
                            ?.RecupererGestionnaireTraces()
                            ?.PublierErreur(
                                ExceptionBase.RecupererLibelleMessage(message, -1));
                    }

                    // Relancer exception encapsulée
                    if (forcerEncapsulation)
                    {
                        return (T)Activator.CreateInstance(
                            typeof(T),
                            ExceptionBase.RecupererLibelleMessage(message, -1),
                            exEncapsulee);
                    }
                    else
                    {
                        return exEncapsulee;
                    }
                }
                else
                { // Exception non encapsulée => encapsulation => trace l'exception encapsulée + la retourne

                    // Encapsulation
                    exEncapsulee = (T)Activator.CreateInstance(
                        typeof(T),
                        ExceptionBase.RecupererLibelleMessage(message, -1),
                        ex);

                    // Traces
                    FabriqueInstance
                        ?.RecupererGestionnaireTraces()
                        ?.PublierException(
                            exEncapsulee);

                    // Retourne exception encapsulée
                    return exEncapsulee;
                }
            }
#if DEBUG
            catch (Exception ex_)
            {
                Debug
                    .WriteLine(
                        ex_.Message);

                Debugger
                    .Break();

                throw;
            }
#else
            catch (Exception)
            { 
                throw;
            }
#endif
        }

        protected Exception EncapsulerEtGererException<T>(
            Exception ex,
            bool forcerEncapsulation = false)
            where T : ExceptionBase
        {
            return EncapsulerEtGererException<T>(
                null,
                ex,
                forcerEncapsulation);
        }

        protected Exception EncapsulerEtGererExceptionTechnique(
            Exception ex,
            bool forcerEncapsulation = false)
        {
            return EncapsulerEtGererException<ExceptionTechnique>(
                null,
                ex,
                forcerEncapsulation);
        }

        protected Exception EncapsulerEtGererExceptionTechnique(
            string message,
            Exception ex,
            bool forcerEncapsulation = false)
        {
            return EncapsulerEtGererException<ExceptionTechnique>(
                message,
                ex,
                forcerEncapsulation);
        }

        protected Exception EncapsulerEtGererExceptionMetier(
            Exception ex,
            bool forcerEncapsulation = false)
        {
            return EncapsulerEtGererException<ExceptionMetier>(
                null,
                ex,
                forcerEncapsulation);
        }

        protected Exception EncapsulerEtGererExceptionMetier(
            string message,
            Exception ex,
            bool forcerEncapsulation = false)
        {
            return EncapsulerEtGererException<ExceptionMetier>(
                message,
                ex,
                forcerEncapsulation);
        }

        #endregion Protected Methods
    }
}
