using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArbreLexicalService.Arbre.Dto;
using Common.Exceptions;
using Common.Ioc;

namespace ArbreLexicalService.Arbre.Cheminement
{
    internal abstract class NavigationSurSymbolesBase : INavigationSurSymboles
    {

        #region Protected Fields

        protected Etat[] etatsOrigine;

        #endregion Protected Fields

        #region Private Fields

        private readonly List<INavigation1Symbole> navigations =
            new List<INavigation1Symbole>();

        private readonly List<char> symboles =
            new List<char>();

        #endregion Private Fields

        #region Public Constructors

        public NavigationSurSymbolesBase(
            IEnumerable<Etat> etatsOrigine)
        {
            this.etatsOrigine = etatsOrigine?.ToArray();
        }

        public NavigationSurSymbolesBase()
        {
        }

        #endregion Public Constructors

        #region Public Properties

        public Etat[] EtatsCourants
        {
            get
            {
                return NavigationCourante
                    .EtatsCourants
                    ?? EtatsOrigine;
            }
        }

        public Etat[] EtatsOrigine
        {
            get
            {
                return etatsOrigine
                    ?? new Etat[0];
            }
        }

        public Transition[] Transitions
        {
            get
            {
                var nbre = navigations.Count;

                return navigations
                    .SelectMany((n, i) =>
                        i + 1 < nbre ? n.TransitionsParSymbole : n.Transitions)
                    .Distinct()
                    .ToArray();
            }
        }

        public Transition[] TransitionsParSymboles
        {
            get
            {
                return navigations
                    .SelectMany(n =>
                        n.TransitionsParSymbole)
                    .Distinct()
                    .ToArray();
            }
        }

        #endregion Public Properties

        #region Protected Properties

        protected INavigation1Symbole NavigationCourante
        {
            get
            {
                return navigations
                    .LastOrDefault();
            }
        }

        #endregion Protected Properties

        #region Public Methods

        public void DefinirEtatsOrigine(
            IEnumerable<Etat> etatsOrigine)
        {
            try
            {
                navigations
                    .Clear();

                this.etatsOrigine = etatsOrigine?.ToArray();
            }
            catch (Exception ex)
            {
                Fabrique.Instance
                    ?.RecupererGestionnaireTraces()
                    ?.PublierException(
                        ex);

                throw new ExceptionTechnique(
                    ExceptionBase.RecupererLibelleErreur(),
                    ex);
            }
        }

        public void TransitionPar(
            char? symbole)
        {
            try
            {
                if (symbole.HasValue)
                {
                    TransitionPar(
                        symbole);
                }
                else
                {
                    TransitionsSansSymbole();
                }
            }
            catch (Exception ex)
            {
                Fabrique.Instance
                    ?.RecupererGestionnaireTraces()
                    ?.PublierException(
                        ex);

                throw new ExceptionTechnique(
                    ExceptionBase.RecupererLibelleErreur(),
                    ex);
            }
        }

        public void TransitionPar(
            char symbole)
        {
            try
            {
                symboles
                    .Add(symbole);
                FabriquerEtEnregistrerNavigation();

                NavigationCourante
                    .TransitionPar(symbole);
            }
            catch (Exception ex)
            {
                Fabrique.Instance
                    ?.RecupererGestionnaireTraces()
                    ?.PublierException(
                        ex);

                throw new ExceptionTechnique(
                    ExceptionBase.RecupererLibelleErreur(),
                    ex);
            }
        }

        public void TransitionsSansSymbole()
        {
            try
            {
                if (!navigations.Any())
                {
                    FabriquerEtEnregistrerNavigation();
                }

                NavigationCourante
                    .TransitionsSansSymbole();
            }
            catch (Exception ex)
            {
                Fabrique.Instance
                    ?.RecupererGestionnaireTraces()
                    ?.PublierException(
                        ex);

                throw new ExceptionTechnique(
                    ExceptionBase.RecupererLibelleErreur(),
                    ex);
            }
        }

        #endregion Public Methods

        #region Protected Methods

        protected abstract INavigation1Symbole FabriquerNavigation();

        #endregion Protected Methods

        #region Private Methods

        private void FabriquerEtEnregistrerNavigation()
        {
            try
            {
                var navigation = FabriquerNavigation();
                navigations
                    .Add(navigation);
            }
            catch (Exception ex)
            {
                Fabrique.Instance
                    ?.RecupererGestionnaireTraces()
                    ?.PublierException(
                        ex);

                throw new ExceptionTechnique(
                    ExceptionBase.RecupererLibelleErreur(),
                    ex);
            }
        }

        #endregion Private Methods

    }
}
