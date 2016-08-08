using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArbreLexicalService.Arbre;
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
                fabrique.Enregistrer<IArbreLexical, IEnumerable<Etat>>(etats =>
                    new ArbreLexical(etats));
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
