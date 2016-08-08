using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArbreLexicalService.Arbre.Dto;

namespace ArbreLexicalService.Arbre.Cheminement
{
    public interface INavigation
    {
        Transition[] Transitions
        {
            get;
        }

        Etat[] EtatsCourants
        {
            get;
        }

        Etat[] EtatsOrigine
        {
            get;
        }

        void DefinirEtatsOrigine(
            IEnumerable<Etat> etatsOrigine);

        void TransitionsSansSymbole();

        void TransitionPar(
            char symbole);

        void TransitionPar(
            char? symbole);
    }
}
