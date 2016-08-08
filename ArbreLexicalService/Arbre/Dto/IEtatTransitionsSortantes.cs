using System.Collections.Generic;

namespace ArbreLexicalService.Arbre.Dto
{
    internal interface IEtatTransitionsSortantes
    {
        Transition[] Transitions { get; }

        IEtatTransitionsSortantes Ajouter(
            Transition transition);

        IEtatTransitionsSortantes Supprimer(
            Transition transition);

        Transition RecupererTransitionEquivalente(
            Transition transition);

        Transition RecupererTransitionAvecSymbole(
            char symbole);

        Transition[] RecupererTransitionsSansSymbole();
   }
}