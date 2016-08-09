using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArbreLexicalService.Arbre.Dto;
using Common.Collections;
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
                return navigations
                    .SelectMany(n =>
                        n.Transitions)
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

                if (navigations.Any())
                {
                    NavigationCourante
                        .ForcerTransitionsSansSymboleEnEntree = false;
                }
                else
                { // 1ère navigation : il n'y a pas encore de navigateur => création

                    FabriquerEtEnregistrerNavigation();
                }

                NavigationCourante
                    .ForcerTransitionsSansSymboleEnSortie = false;

                NavigationCourante
                    .TransitionPar(symbole);
                NettoyerTransitionsEtEtatsOrigine();

                FabriquerEtEnregistrerNavigation();
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

        private class Navigation1Symbole : Navigation1SymboleBase
        {
            protected override Transition RecupererTransitionAvecSymbole(
                Etat etatSource, 
                char symbole)
            {
                throw new NotImplementedException();
            }

            protected override Transition[] RecupererTransitionsSansSymbole(
                Etat etatSource)
            {
                throw new NotImplementedException();
            }

            public Navigation1Symbole(
                IEnumerable<Etat> etatsOrigine,
                IEnumerable<Etat> etatsCourants,
                IEnumerable<Transition> transitions)
            {
                this.etatsOrigine = etatsOrigine.ToArray();
                this.etatsCourants = etatsCourants.ToArray();

                this.transitions
                    .AddRange(transitions);
            }

            public Navigation1Symbole Nettoyer(
                IEnumerable<Transition> transitionsAAnalyser)
            {
                NettoyerTransitionsEtEtatsOrigine(
                    transitionsAAnalyser);

                return this;
            }

            public Navigation1Symbole Ajouter(
                IEnumerable<Transition> transitionsAAjouter)
            {
                transitions
                    .AjouterSaufSiStocke(
                        transitionsAAjouter);

                return this;
            }
        }

        private void NettoyerTransitionsEtEtatsOrigine()
        {
            try
            {
                var nbreNavigations = navigations.Count;

                if (nbreNavigations > 1)
                {
                    var avantDerniereNavigation = navigations
                        .ElementAt(
                            nbreNavigations - 2);
                    var derniereNavigation = NavigationCourante;

                    if (avantDerniereNavigation.EtatsCourants.Count() > derniereNavigation.EtatsOrigine.Count())
                    {
                        var transitionsPrecedentes = navigations
                            .SelectMany((n, i) =>
                                i + 1 < nbreNavigations ?
                                    n.Transitions :
                                    Enumerable.Empty<Transition>())
                            .Distinct();
                        var transitionsAAnalyser = avantDerniereNavigation
                            .Transitions
                            .Where(t =>
                                t.Symbole.HasValue);

                        // Note : on aurait pu englober la dernière navigation mais on aurait dû mettre ne place une récupération des transactions avec symbole sur la dernière navigation pour une utilisation sur le prochain nettoyage
                        var navigationOptimisee = new Navigation1Symbole(
                            navigations.First().EtatsOrigine,
                            NavigationCourante.EtatsOrigine,
                            transitionsPrecedentes);
                        navigationOptimisee
                            .Nettoyer(
                                transitionsAAnalyser);

                        navigations
                            .Clear();
                        navigations
                            .Add(navigationOptimisee);
                        // Ajout de la dernière navigation, cela permettra d'avoir les dernières transitions avec symbole sur le prochain nettoyage
                        navigations
                            .Add(derniereNavigation);
                    }
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
