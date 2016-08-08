using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Ioc
{
    public interface IFabrique
    {
        void Enregistrer<TCible>(
            Func<TCible> ftFactory);

        void Enregistrer<TCible, TArg>(
            Func<TArg, TCible> ftFactory);

        void Enregistrer<TCible, TArg1, TArg2>(
            Func<TArg1, TArg2, TCible> ftFactory);

        void Enregistrer<TCible, TArg1, TArg2, TArg3>(
            Func<TArg1, TArg2, TArg3, TCible> ftFactory);

        void Enregistrer<TCible, TArg1, TArg2, TArg3, TArg4>(
            Func<TArg1, TArg2, TArg3, TArg4, TCible> ftFactory);

        void Enregistrer<TCible, TArg1, TArg2, TArg3, TArg4, TArg5>(
            Func<TArg1, TArg2, TArg3, TArg4, TArg5, TCible> ftFactory);

        void ChargerIocDepuisAssembly();

        void EnregistrerSingleton<TCible>(
            Func<TCible> ftFactory);

        void EnregistrerSingleton<TCible, TArg>(
            Func<TArg, TCible> ftFactory);

        void EnregistrerSingleton<TCible, TArg1, TArg2>(
            Func<TArg1, TArg2, TCible> ftFactory);

        void EnregistrerSingleton<TCible, TArg1, TArg2, TArg3>(
            Func<TArg1, TArg2, TArg3, TCible> ftFactory);

        void EnregistrerSingleton<TCible, TArg1, TArg2, TArg3, TArg4>(
            Func<TArg1, TArg2, TArg3, TArg4, TCible> ftFactory);

        void EnregistrerSingleton<TCible, TArg1, TArg2, TArg3, TArg4, TArg5>(
            Func<TArg1, TArg2, TArg3, TArg4, TArg5, TCible> ftFactory);

        TCible RecupererInstance<TCible>();

        TCible RecupererInstance<TCible, TArg>(
            TArg a1);

        TCible RecupererInstance<TCible, TArg1, TArg2>(
            TArg1 a1, TArg2 a2);

        TCible RecupererInstance<TCible, TArg1, TArg2, TArg3>(
            TArg1 a1, TArg2 a2, TArg3 a3);

        TCible RecupererInstance<TCible, TArg1, TArg2, TArg3, TArg4>(
            TArg1 a1, TArg2 a2, TArg3 a3, TArg4 a4);

        TCible RecupererInstance<TCible, TArg1, TArg2, TArg3, TArg4, TArg5>(
            TArg1 a1, TArg2 a2, TArg3 a3, TArg4 a4, TArg5 a5);
    }
}
