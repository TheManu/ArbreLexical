using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnalyseSyntaxiqueService.Analyse;
using ArbreLexicalService.Arbre.Construction;
using Common.Ioc;
using Common.Traces;

namespace AnalyseSyntaxiqueService.Ioc
{
    [Ioc(2, 1)]
    internal class ChargementIoc : IChargementIoc
    {
        public void Enregistrer(
            IFabrique fabrique)
        {
            try
            {
                fabrique
                    .Enregistrer<IAnalyseurSyntaxique, IArbreConstruction>(arbre =>
                        new AnalyseurSyntaxique(arbre));
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
