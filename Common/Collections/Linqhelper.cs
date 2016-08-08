using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Exceptions;
using Common.Ioc;

namespace Common.Collections
{
    public static class Linqhelper
    {
        public static ICollection<T> AjouterSaufSiStocke<T>(
            this ICollection<T> collection,
            IEnumerable<T> elementsAAjouter)
        {
            try
            {
                elementsAAjouter = (elementsAAjouter as ICollection<T>) ?? elementsAAjouter.ToArray(); // Permet de ne pas recalculer en permanence le contenu si c'est un Enumerable 'pur'

                if (elementsAAjouter.Any())
                {
                    if (elementsAAjouter.Count() == 1)
                    {
                        var item = elementsAAjouter.First();

                        if (!collection.Contains(item))
                        {
                            collection
                                .Add(
                                    item);
                        }
                    }
                    else
                    {
                        var items = elementsAAjouter
                            .Except(collection);

                        collection
                            .Ajouter(items); 
                    }
                }

                return collection;
            }
            catch (Exception ex)
            {
                Fabrique.Instance
                    ?.RecupererGestionnaireTraces()
                    ?.PublierException(
                        ex);

                throw new ExceptionTechnique(
                    ExceptionBase.RecupererLibelleErreur(),
                    ex);
            }
        }

        public static ICollection<T> AjouterSaufSiStocke<T>(
            this ICollection<T> collection,
            params T[] elementsAAjouter)
        {
            try
            {
                return collection
                    .AjouterSaufSiStocke(
                        elementsAAjouter.AsEnumerable());
            }
            catch (Exception ex)
            {
                Fabrique.Instance
                    ?.RecupererGestionnaireTraces()
                    ?.PublierException(
                        ex);

                throw new ExceptionTechnique(
                    ExceptionBase.RecupererLibelleErreur(),
                    ex);
            }
        }

        public static ICollection<T> Ajouter<T>(
            this ICollection<T> collection,
            IEnumerable<T> elementsAAjouter)
        {
            try
            {
                var liste = collection as List<T>;

                if (null != liste)
                {
                    liste
                        .AddRange(elementsAAjouter);
                }
                else
                {
                    foreach (var item in elementsAAjouter)
                    {
                        collection
                            .Add(item);
                    }
                }

                return collection;
            }
            catch (Exception ex)
            {
                Fabrique.Instance
                    ?.RecupererGestionnaireTraces()
                    ?.PublierException(
                        ex);

                throw new ExceptionTechnique(
                    ExceptionBase.RecupererLibelleErreur(),
                    ex);
            }
        }

        public static bool EstIdentique<T>(
            this IEnumerable<T> elements1,
            IEnumerable<T> elements2)
        {
            try
            {
                elements1 = (elements1 as ICollection<T>) ?? elements1.ToArray(); // Permet de ne pas recalculer en permanence le contenu si c'est un Enumerable 'pur'
                elements2 = (elements2 as ICollection<T>) ?? elements2.ToArray();

                if (elements1.Count() != elements2.Count())
                {
                    return false;
                }
                else
                {
                    return (!elements1
                        .Except(elements2)
                        .Any());
                }
            }
            catch (Exception ex)
            {
                Fabrique.Instance
                    ?.RecupererGestionnaireTraces()
                    ?.PublierException(
                        ex);

                throw new ExceptionTechnique(
                    ExceptionBase.RecupererLibelleErreur(),
                    ex);
            }
        }
    }
}
