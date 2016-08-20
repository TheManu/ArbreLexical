using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Ioc;

namespace ArbreLexicalService.Arbre.Dto
{
    public class Transition
    {

        #region Private Fields

        private readonly Etat etatCible;

        private readonly Etat etatSource;

        private readonly char? symbole;

        #endregion Private Fields

        #region Public Constructors

        public Transition(
            Etat etatSource, 
            Etat etatCible, 
            char? symbole)
        {
            this.etatSource = etatSource;
            this.etatCible = etatCible;
            this.symbole = symbole;
        }

        public Transition(
            Etat etatSource, 
            Etat etatCible,
            char symbole) : this(etatSource, etatCible, new char?(symbole))
        {
        }

        public Transition(
            Etat etatSource, 
            Etat etatCible) : this(etatSource, etatCible, null)
        {
        }

        public Transition(
            Etat etatRecursif,
            char? symbole) : this(etatRecursif, etatRecursif, symbole)
        {
        }

        public Transition(
            Etat etatRecursif,
            char symbole) : this(etatRecursif, etatRecursif, symbole)
        {
        }

        public Transition(
            Etat etatRecursif) : this(etatRecursif, etatRecursif, null)
        {
        }

        #endregion Public Constructors

        #region Public Properties

        public bool EstRecursif
        {
            get
            {
                return (etatSource == etatCible);
            }
        }

        public Etat EtatCible
        {
            get
            {
                return etatCible;
            }
        }

        public Etat EtatSource
        {
            get
            {
                return etatSource;
            }
        }

        public char? Symbole
        {
            get
            {
                return symbole;
            }
        }

        #endregion Public Properties

        #region Public Methods

        public bool EstEquivalentA(
            Transition transition)
        {
            return (etatSource == transition.etatSource &&
                etatCible == transition.etatCible &&
                symbole == transition.symbole);
        }

        public override string ToString()
        {
            try
            {
                var symboleAffichage = (symbole.HasValue ?
                    symbole.Value.ToString().Replace("\n", "A la ligne") :
                    string.Empty);

                return $"Transition {etatSource.Identifiant}=='{symboleAffichage}'==>{etatCible.Identifiant}";
            }
            catch (Exception ex)
            {
                Fabrique.Instance
                    ?.RecupererGestionnaireTraces()
                    ?.PublierException(
                        ex);

                // L'exception n'est pas relancée !
            }

            return base.ToString();
        }

        #endregion Public Methods

    }
}
