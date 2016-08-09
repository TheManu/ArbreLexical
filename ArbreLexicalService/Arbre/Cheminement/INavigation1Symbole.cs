using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArbreLexicalService.Arbre.Dto;

namespace ArbreLexicalService.Arbre.Cheminement
{
    public interface INavigation1Symbole : INavigation
    {
        bool ForcerNettoyage
        {
            get;
            set;
        }

        bool ForcerTransitionsSansSymboleEnEntree
        {
            get;
            set;
        }

        bool ForcerTransitionsSansSymboleEnSortie
        {
            get;
            set;
        }
    }
}
