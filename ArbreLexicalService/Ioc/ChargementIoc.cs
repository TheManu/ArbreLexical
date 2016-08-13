using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArbreLexicalService.Arbre;
using ArbreLexicalService.Arbre.Construction;
using ArbreLexicalService.Arbre.Construction.Elements;
using ArbreLexicalService.Arbre.Dto;
using Common.Ioc;
using Common.Traces;

namespace ArbreLexicalService.Ioc
{
    [Ioc(2)]
    internal class ChargementIoc : IChargementIoc
    {
        public void Enregistrer(
            IFabrique fabrique)
        {
            try
            {
                fabrique
                    .Enregistrer<IArbreLexical, IEnumerable<Etat>>(etats =>
                        new ArbreLexical(etats));

                fabrique
                    .Enregistrer<IArbreConstruction>(() =>
                        new ArbreConstruction());

                fabrique
                    .Enregistrer<IEtatTransitionsSortantes, Etat>(e =>
                        new EtatTransitionsSortantes(e));

                fabrique
                    .Enregistrer<IConstructionElementArbre, IArbreConstruction>(a =>
                        new ConstructionElementArbre(a));
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
