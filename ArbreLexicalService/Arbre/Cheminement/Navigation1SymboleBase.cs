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
    internal abstract class Navigation1SymboleBase : INavigation1Symbole
    {

        #region Protected Fields

        protected readonly List<Transition> transitions =
            new List<Transition>();

        protected Etat[] etatsCourants;

        protected Etat[] etatsOrigine;

        private Transition[] transitionsParSymbole;

        #endregion Protected Fields

        #region Public Constructors

        public Navigation1SymboleBase(
            IEnumerable<Etat> etatsOrigine)
        {
            this.etatsOrigine = etatsOrigine?.ToArray();
        }

        public Navigation1SymboleBase()
        {
        }

        #endregion Public Constructors

        #region Public Properties

        public Etat[] EtatsCourants
        {
            get
            {
                return etatsCourants ?? EtatsOrigine;
            }
        }

        public Etat[] EtatsOrigine
        {
            get
            {
                return etatsOrigine ?? new Etat[0];
            }
        }

        public Transition[] Transitions
        {
            get
            {
                return transitions
                    .ToArray();
            }
        }

        public Transition[] TransitionsParSymbole
        {
            get
            {
                return transitionsParSymbole 
                    ?? new Transition[0];
            }

            set
            {
                transitionsParSymbole = value;
            }
        }

        #endregion Public Properties

        #region Public Methods

        public void DefinirEtatsOrigine(
            IEnumerable<Etat> etatsOrigine)
        {
            try
            {
                transitions.Clear();
                this.etatsOrigine = etatsOrigine?.ToArray();
                etatsCourants = this.etatsOrigine;
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
                        symbole.Value);
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
                etatsCourants = EtatsOrigine;

                TransitionSansSymboleMultiNiveaux();

                var transitionsAvecSymbole = TransitionPar1Niveau(
                    symbole);
                NettoyerTransitionsApresTransitionAvecSymbole(
                    transitionsAvecSymbole);

                TransitionSansSymboleMultiNiveaux();
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

        private void NettoyerTransitionsApresTransitionAvecSymbole(
            IEnumerable<Transition> transitionsAvecSymbole)
        {
            try
            {
                if (transitions.Any())
                {
                    if (transitionsAvecSymbole.Any())
                    { // On valide les transitions en remontant des états courants vers les états en origine (càd de la fin vers le début)

                        var transitionsAGarder = new List<Transition>(
                            transitionsAvecSymbole);
                        var transitionsAAnalyser = transitionsAvecSymbole
                            .ToArray();

                        do
                        {
                            var etatsSource = transitionsAAnalyser
                                .Select(t =>
                                    t.EtatSource)
                                .ToArray();

                            transitionsAAnalyser = transitions
                                .Except(transitionsAGarder)
                                .Where(t =>
                                    etatsSource.Contains(t.EtatCible))
                                .ToArray();

                            transitionsAGarder
                                .AddRange(
                                    transitionsAAnalyser);
                        }
                        while (transitionsAAnalyser.Any());

                        transitionsParSymbole = transitionsAGarder
                            .Distinct()
                            .ToArray();
                        transitions
                            .Clear();
                        transitions
                            .AddRange(transitionsParSymbole);
                    }
                    else
                    {
                        transitions.Clear();
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
                etatsCourants = EtatsOrigine;
                TransitionSansSymboleMultiNiveaux();
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

        protected abstract Transition RecupererTransitionAvecSymbole(
            Etat etatSource,
            char symbole);

        protected abstract Transition[] RecupererTransitionsSansSymbole(
            Etat etatSource);

        protected Transition[] TransitionPar1Niveau(
            char symbole)
        {
            try
            {
                var transitionsAvecSymbole = new List<Transition>();

                foreach (var etatCourant in etatsCourants)
                {
                    var transitionParSymbole = RecupererTransitionAvecSymbole(
                        etatCourant,
                        symbole);

                    if (null != transitionParSymbole)
                    {
                        transitionsAvecSymbole
                            .AjouterSaufSiStocke(
                                transitionParSymbole);
                    }
                }

                transitions
                    .AjouterSaufSiStocke(
                        transitionsAvecSymbole);

                etatsCourants = transitionsAvecSymbole
                    .Select(t =>
                        t.EtatCible)
                    .Distinct()
                    .ToArray();

                return transitionsAvecSymbole
                    .ToArray();
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

        protected void TransitionSansSymboleMultiNiveaux()
        {
            try
            {
                Etat[] etatsNiveauPrecedent = null;
                var etatsDejaAnalyses = new Dictionary<Etat, Transition[]>();
                var transitionsAAjouter = new List<Transition>();
                var etatsAAnalyser = etatsCourants;

                if (etatsAAnalyser.Any())
                {
                    do
                    {
                        var transitionsSansSymbole = new List<Transition>();

                        foreach (var etatAAnalyser in etatsAAnalyser)
                        {
                            Transition[] etatTransitionsSansSymbole;
                            if (!etatsDejaAnalyses.TryGetValue(etatAAnalyser, out etatTransitionsSansSymbole))
                            {
                                etatTransitionsSansSymbole = RecupererTransitionsSansSymbole(
                                    etatAAnalyser);

                                etatsDejaAnalyses
                                    .Add(
                                        etatAAnalyser,
                                        etatTransitionsSansSymbole);
                            }

                            transitionsSansSymbole
                                .AddRange(
                                    etatTransitionsSansSymbole);
                        }

                        if (transitionsSansSymbole.Any())
                        {
                            etatsNiveauPrecedent = etatsAAnalyser;
                            etatsAAnalyser = transitionsSansSymbole
                                .Select(t =>
                                    t.EtatCible)
                                .Distinct()
                                .ToArray();

                            transitionsAAjouter
                                .AjouterSaufSiStocke(
                                    transitionsSansSymbole.Distinct());

                            if (etatsAAnalyser.EstIdentique(etatsNiveauPrecedent))
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    while (etatsAAnalyser.Any());

                    if (transitionsAAjouter.Any())
                    {
                        var etatsCiblesParTransitions = transitionsAAjouter
                            .Select(t =>
                                t.EtatCible)
                            .Distinct();

                        etatsCourants = etatsCourants
                            .ToList()
                            .AjouterSaufSiStocke(
                                etatsCiblesParTransitions)
                            .ToArray();

                        transitions
                            .AjouterSaufSiStocke(
                                transitionsAAjouter);
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

        #endregion Protected Methods

    }
}
