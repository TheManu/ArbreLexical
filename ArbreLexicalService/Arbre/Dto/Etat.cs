using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Ioc;

namespace ArbreLexicalService.Arbre.Dto
{
    public class Etat
    {
        private static int identifiantMax = 0;

        private readonly int identifiant;

        public Etat()
        {
            identifiant = Interlocked
                .Increment(
                    ref identifiantMax);
        }

        public int Identifiant
        {
            get
            {
                return identifiant;
            }
        }

        public override string ToString()
        {
            try
            {
                return $"Etat {identifiant}";
            }
            catch (Exception ex)
            {
                Fabrique.Instance
                    ?.RecupererGestionnaireTraces()
                    ?.PublierException(
                        ex);
            }

            return base.ToString();
        }
    }
}
