using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Common.Traces;
using Jwc.Funz;

namespace Common.Ioc
{
    public class Fabrique : IFabrique
    {
        #region Private Fields

        /// <summary>
        /// Funz (tiré de Funq) IoC. Ce framework est plus performant que Ninject mais il est moins puissant.
        /// </summary>
        private static Container funzIoc =
            new Container();

        #endregion Private Fields

        #region Public Constructors

        static Fabrique()
        {
            try
            {
                funzIoc
                    .Register<IFabrique>(c =>
                        new Fabrique());

                var fabrique = funzIoc
                    .TryResolve<IFabrique>();

                fabrique
                    .ChargerIocDepuisAssembly();
            }
            catch (Exception ex)
            {
                Debug
                    .WriteLine(
                        ex.Message);

#if DEBUG
                Debugger.Break();
#endif
            }
        }

        #endregion Public Constructors

        #region Public Properties

        public static IFabrique Instance
        {
            get
            {
                return funzIoc
                    .TryResolve<IFabrique>();
            }
        }

        #endregion Public Properties

        #region Public Methods

        public void ChargerIocDepuisAssembly()
        {
            try
            {                
                var typesIoc = Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
                    .Where(o =>
                        o.GetCustomAttributes().Any(a => a is IocAttribute));

                foreach (var typeIoc in typesIoc)
                {
                    var instanceIoc = Activator
                        .CreateInstance(typeIoc) as IChargementIoc;

                    instanceIoc
                        ?.Enregistrer(
                            this);
                }
            }
            catch (Exception ex)
            {
                Debug
                    .WriteLine(
                        ex.Message);

#if DEBUG
                Debugger.Break();
#endif
                funzIoc
                    .TryResolve<ITraces>()
                    ?.PublierException(
                        ex);
            }
        }
        public void Enregistrer<TCible>(
            Func<TCible> ftFactory)
        {
            funzIoc
                .Register<TCible>(
                    c =>
                        ftFactory())
                .ReusedWithinNone();
        }

        public void Enregistrer<TCible, TArg>(
            Func<TArg, TCible> ftFactory)
        {
            funzIoc
                .Register<TCible, TArg>(
                    (c, a) =>
                        ftFactory(a))
                .ReusedWithinNone();
        }

        public void Enregistrer<TCible, TArg1, TArg2>(
            Func<TArg1, TArg2, TCible> ftFactory)
        {
            funzIoc
                .Register<TCible, TArg1, TArg2>(
                    (c, a1, a2) =>
                        ftFactory(a1, a2))
                .ReusedWithinNone();
        }

        public void Enregistrer<TCible, TArg1, TArg2, TArg3>(
            Func<TArg1, TArg2, TArg3, TCible> ftFactory)
        {
            funzIoc
                .Register<TCible, TArg1, TArg2, TArg3>(
                    (c, a1, a2, a3) =>
                        ftFactory(a1, a2, a3))
                .ReusedWithinNone();
        }

        public void Enregistrer<TCible, TArg1, TArg2, TArg3, TArg4>(
            Func<TArg1, TArg2, TArg3, TArg4, TCible> ftFactory)
        {
            funzIoc
                .Register<TCible, TArg1, TArg2, TArg3, TArg4>(
                    (c, a1, a2, a3, a4) =>
                        ftFactory(a1, a2, a3, a4))
                .ReusedWithinNone();
        }

        public void Enregistrer<TCible, TArg1, TArg2, TArg3, TArg4, TArg5>(
            Func<TArg1, TArg2, TArg3, TArg4, TArg5, TCible> ftFactory)
        {
            funzIoc
                .Register<TCible, TArg1, TArg2, TArg3, TArg4, TArg5>(
                    (c, a1, a2, a3, a4, a5) =>
                        ftFactory(a1, a2, a3, a4, a5))
                .ReusedWithinNone();
        }

        public void EnregistrerSingleton<TCible>(
            Func<TCible> ftFactory)
        {
            funzIoc
                .Register<TCible>(
                    c => ftFactory())
                .ReusedWithinContainer();
        }

        public void EnregistrerSingleton<TCible, TArg>(
            Func<TArg, TCible> ftFactory)
        {
            funzIoc
                .Register<TCible, TArg>(
                    (c, a) => ftFactory(a))
                .ReusedWithinContainer();
        }

        public void EnregistrerSingleton<TCible, TArg1, TArg2>(
            Func<TArg1, TArg2, TCible> ftFactory)
        {
            funzIoc
                .Register<TCible, TArg1, TArg2>(
                    (c, a1, a2) => ftFactory(a1, a2))
                .ReusedWithinContainer();
        }

        public void EnregistrerSingleton<TCible, TArg1, TArg2, TArg3>(
            Func<TArg1, TArg2, TArg3, TCible> ftFactory)
        {
            funzIoc
                .Register<TCible, TArg1, TArg2, TArg3>(
                    (c, a1, a2, a3) => ftFactory(a1, a2, a3))
                .ReusedWithinContainer();
        }

        public void EnregistrerSingleton<TCible, TArg1, TArg2, TArg3, TArg4>(
            Func<TArg1, TArg2, TArg3, TArg4, TCible> ftFactory)
        {
            funzIoc
                .Register<TCible, TArg1, TArg2, TArg3, TArg4>(
                    (c, a1, a2, a3, a4) => ftFactory(a1, a2, a3, a4))
                .ReusedWithinContainer();
        }

        public void EnregistrerSingleton<TCible, TArg1, TArg2, TArg3, TArg4, TArg5>(
            Func<TArg1, TArg2, TArg3, TArg4, TArg5, TCible> ftFactory)
        {
            funzIoc
                .Register<TCible, TArg1, TArg2, TArg3, TArg4, TArg5>(
                    (c, a1, a2, a3, a4, a5) => ftFactory(a1, a2, a3, a4, a5))
                .ReusedWithinContainer();
        }

        public TCible RecupererInstance<TCible>()
        {
            return funzIoc.TryResolve<TCible>();
        }

        public TCible RecupererInstance<TCible, TArg>(
            TArg a1)
        {
            return funzIoc.TryResolve<TCible, TArg>(
                a1);
        }

        public TCible RecupererInstance<TCible, TArg1, TArg2>(
            TArg1 a1, TArg2 a2)
        {
            return funzIoc.TryResolve<TCible, TArg1, TArg2>(
                a1, a2);
        }

        public TCible RecupererInstance<TCible, TArg1, TArg2, TArg3>(
            TArg1 a1, TArg2 a2, TArg3 a3)
        {
            return funzIoc.TryResolve<TCible, TArg1, TArg2, TArg3>(
                a1, a2, a3);
        }

        public TCible RecupererInstance<TCible, TArg1, TArg2, TArg3, TArg4>(
            TArg1 a1, TArg2 a2, TArg3 a3, TArg4 a4)
        {
            return funzIoc.TryResolve<TCible, TArg1, TArg2, TArg3, TArg4>(
                a1, a2, a3, a4);
        }

        public TCible RecupererInstance<TCible, TArg1, TArg2, TArg3, TArg4, TArg5>(
            TArg1 a1, TArg2 a2, TArg3 a3, TArg4 a4, TArg5 a5)
        {
            return funzIoc.TryResolve<TCible, TArg1, TArg2, TArg3, TArg4, TArg5>(
                a1, a2, a3, a4, a5);
        }

        #endregion Public Methods
    }
}
