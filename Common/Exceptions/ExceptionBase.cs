using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Helpers;
using Common.Ioc;
using Common.Reflexion;

namespace Common.Exceptions
{
    public class ExceptionBase : Exception
    {
        public ExceptionBase() : base()
        { }

        public ExceptionBase(
            string message) : base(message ?? string.Empty)
        { }

        public ExceptionBase(
            string message, 
            Exception innerException) : base(message ?? string.Empty, innerException)
        { }

        public static string RecupererLibelleErreur()
        {
            return RecupererLibelleErreur(
                null,
                -1);
        }

        public static string RecupererLibelleErreur(
            string message)
        {
            return RecupererLibelleErreur(
                message,
                -1);
        }

        public static string RecupererLibelleErreur(
            string message,
            int niveauPile)
        {
            try
            {
                var niveau = Math.Abs(niveauPile) + 1;
                var methodeAppelant = ReflexionHelper
                    .RecupererMethode(niveau);
                var classeAppelant = methodeAppelant.DeclaringType;
                string messageAjout = (StringHelper.EstNonNullEtNonVideEtNonEspaces(message) ?
                    $" : {message.Trim()}" :
                    string.Empty);

                return $"{classeAppelant.FullName}.{methodeAppelant.Name}(){messageAjout}";
            }
            catch (Exception ex)
            {
                Fabrique.Instance
                    ?.RecupererGestionnaireTraces()
                    ?.PublierException(
                        ex);
            }

            return message ?? string.Empty;
        }
    }
}
