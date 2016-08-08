using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArbreLexicalService.Arbre.Dto;

namespace ArbreLexicalService.Arbre.Construction
{
    public interface IArbreConstruction
    {
        Etat AjouterEtat();

        Transition AjouterTransition(
            Etat etatSource,
            Etat etatCible);

        Transition AjouterTransition(
            Etat etatSource,
            Etat etatCible,
            char symbole);

        Transition AjouterTransition(
            Etat etatSource,
            Etat etatCible,
            char? symbole);

        Transition AjouterTransition(
            Etat etatAvecReflexion);

        Transition AjouterTransition(
            Etat etatAvecReflexion,
            char symbole);

        Transition AjouterTransition(
            Etat etatAvecReflexion,
            char? symbole);
    }
}
