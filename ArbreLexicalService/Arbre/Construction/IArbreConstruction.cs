using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArbreLexicalService.Arbre.Construction.Elements;
using ArbreLexicalService.Arbre.Dto;

namespace ArbreLexicalService.Arbre.Construction
{
    public interface IArbreConstruction
    {
        Etat EtatEntree
        {
            get;
        }

        Etat EtatSortie
        {
            get;
        }

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

        IArbreLexical FinaliserArbre();

        IConstructionElementArbre CreerElement(
            IEnumerable<Etat> etatsConnexionDebut,
            IEnumerable<Etat> etatsConnexionFin);

        IConstructionElementArbre CreerElement(
            IEnumerable<Etat> etatsConnexionDebut);

        Transition[] AjouterChemin(
            Etat etatDebut, 
            string chemin);

        void Etiquetter(
            string id,
            EnumTypeBlock typeBlock,
            Etat etatEntree,
            Etat etatSortie);
    }
}
