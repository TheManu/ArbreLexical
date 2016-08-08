using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Common.Exceptions;

namespace Common.Reflexion
{
    public static class ReflexionHelper
    {
        public static MethodBase RecupererMethode(
            int niveauPile)
        {
            var stackTrace = new StackTrace();
            var niveau = Math.Abs(niveauPile) + 1;

            if (stackTrace.FrameCount >= niveau + 1)
            {
                var stackFrame = stackTrace
                    .GetFrame(niveau);

                return stackFrame
                    .GetMethod();
            }
            else
            {
                throw new ExceptionTechnique(
                    $"{nameof(ReflexionHelper)}.{nameof(ReflexionHelper.RecupererMethode)}() : Dépassement de la pile");
            }
        }

        public static string RecupererNomMethode(
            int niveauPile)
        {
            var niveau = Math.Abs(niveauPile) + 1;

            return RecupererMethode(niveau)
                .Name;
        }

        public static string RecupererNomMethodeCourante()
        {
            return RecupererNomMethode(-1);
        }
    }
}
