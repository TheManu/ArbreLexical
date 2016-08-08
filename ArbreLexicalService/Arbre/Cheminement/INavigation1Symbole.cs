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
        Transition[] TransitionsParSymbole
        {
            get;
        }
    }
}
