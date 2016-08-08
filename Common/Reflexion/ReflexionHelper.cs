using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Common.Exceptions;
using Common.Ioc;

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

        public static ClasseAttibut<T>[] RecupererClassesAvecAttribut<T>(
            this IEnumerable<Assembly> composants)
            where T : Attribute
        {
            return composants
                .SelectMany(a =>
                    a.GetTypes())
                .Select(t =>
                    new ClasseAttibut<T>
                    {
                        Classe = t,
                        Attribut = t
                            .GetCustomAttributes()
                            .FirstOrDefault(a => a is T) as T
                    })
                .Where(o =>
                    null != o.Attribut)
                .ToArray();
        }
    }

    public class ClasseAttibut<T>
    {
        public T Attribut { get; internal set; }

        public Type Classe { get; internal set; }
    }
}
