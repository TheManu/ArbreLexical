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
    public abstract class ExceptionBase : Exception
    {
        #region Public Constructors

        public ExceptionBase() : base()
        { }

        public ExceptionBase(
            string message) : base(message ?? string.Empty)
        { }

        public ExceptionBase(
            string message, 
            Exception innerException) : base(message ?? string.Empty, innerException)
        { }
        #endregion Public Constructors

        #region Public Properties

        public abstract EnumTypeException TypeException
        {
            get;
        }

        #endregion Public Properties

        #region Public Methods

        public static string RecupererLibelleMessage()
        {
            return RecupererLibelleMessage(
                null,
                -1);
        }

        public static string RecupererLibelleMessage(
            int niveauPile)
        {
            return RecupererLibelleMessage(
                null,
                Math.Abs(niveauPile) + 1);
        }

        public static string RecupererLibelleMessage(
            string message)
        {
            return RecupererLibelleMessage(
                message,
                -1);
        }

        public static string RecupererLibelleMessage(
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

                // L'exception n'est pas relancée !
            }

            return message ?? string.Empty;
        }

        #endregion Public Methods
    }
}
