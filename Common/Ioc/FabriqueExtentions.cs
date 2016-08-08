using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Traces;

namespace Common.Ioc
{
    public static class FabriqueExtentions
    {
        public static ITraces RecupererGestionnaireTraces(
            this IFabrique fabrique)
        {
            return fabrique
                ?.RecupererInstance<ITraces>();
        }
    }
}
