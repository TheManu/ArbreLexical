using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArbreLexicalService.Arbre.Dto;
using Common.Collections;
using Common.Exceptions;
using Common.Ioc;
using Common.Services;

namespace ArbreLexicalService.Arbre.Cheminement
{
    internal abstract class Navigation1SymboleBase : ServiceBase, INavigation1Symbole
    {

        #region Protected Fields

        protected readonly List<Transition> transitions =
            new List<Transition>();

        protected Etat[] etatsCourants;

        protected Etat[] etatsOrigine;

        #endregion Protected Fields

        #region Private Fields

        private bool forcerNettoyage = true;

        private bool forcerTransitionsSansSymboleEnEntree = true;

        private bool forcerTransitionsSansSymboleEnSortie = true;

        #endregion Private Fields

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

        public bool ForcerNettoyage
        {
            get
            {
                return forcerNettoyage;
            }

            set
            {
                forcerNettoyage = value;
            }
        }

        public bool ForcerTransitionsSansSymboleEnEntree
        {
            get
            {
                return forcerTransitionsSansSymboleEnEntree;
            }

            set
            {
                forcerTransitionsSansSymboleEnEntree = value;
            }
        }

        public bool ForcerTransitionsSansSymboleEnSortie
        {
            get
            {
                return forcerTransitionsSansSymboleEnSortie;
            }

            set
            {
                forcerTransitionsSansSymboleEnSortie = value;
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
                throw EncapsulerEtGererException<ExceptionTechnique>(
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
                throw EncapsulerEtGererException<ExceptionTechnique>(
                    ex);
            }
        }

        public void TransitionPar(
            char symbole)
        {
            try
            {
                etatsCourants = EtatsCourants;

                if (forcerTransitionsSansSymboleEnEntree)
                {
                    TransitionSansSymboleMultiNiveaux(); 
                }

                var transitionsAvecSymbole = TransitionPar1Niveau(
                    symbole);
                if (forcerNettoyage)
                {
                    NettoyerTransitionsEtEtatsOrigine(
                        transitionsAvecSymbole); 
                }

                if (forcerTransitionsSansSymboleEnSortie)
                {
                    TransitionSansSymboleMultiNiveaux(); 
                }
            }
            catch (Exception ex)
            {
                throw EncapsulerEtGererException<ExceptionTechnique>(
                    ex);
            }
        }

        public void TransitionsSansSymbole()
        {
            try
            {
                etatsCourants = EtatsCourants;
                TransitionSansSymboleMultiNiveaux();
            }
            catch (Exception ex)
            {
                throw EncapsulerEtGererException<ExceptionTechnique>(
                    ex);
            }
        }

        #endregion Public Methods

        #region Protected Methods

        protected void NettoyerTransitionsEtEtatsOrigine(
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

                        // Nettoyage des transitions
                        var transitionsParSymbole = transitionsAGarder
                            .Distinct()
                            .ToArray();
                        transitions
                            .Clear();
                        transitions
                            .AddRange(transitionsParSymbole);

                        // Nettoyage des états en origine
                        etatsOrigine = EtatsOrigine
                            .Intersect(
                                transitionsParSymbole.Select(t => t.EtatSource))
                            .ToArray();
                    }
                    else
                    {
                        transitions.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                throw EncapsulerEtGererException<ExceptionTechnique>(
                    ex);
            }
        }

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
                throw EncapsulerEtGererException<ExceptionTechnique>(
                    ex);
            }
        }

        protected void TransitionSansSymboleMultiNiveaux()
        {
            try
            {
                var etatsArriveePrecedents = new List<Etat[]>();
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
                            if (!etatsArriveePrecedents.Any() || 
                                etatsArriveePrecedents.All(tab => !tab.EstIdentique(etatsAAnalyser)))
                            { // On sauvegarde les états qui viennent d'être analysés

                                etatsArriveePrecedents
                                    .Add(etatsAAnalyser);
                            }

                            // Calcul des états d'arrivée par les transitions récupérées
                            etatsAAnalyser = transitionsSansSymbole
                                .Select(t =>
                                    t.EtatCible)
                                .Distinct()
                                .ToArray();

                            transitionsAAjouter
                                .AjouterSaufSiStocke(
                                    transitionsSansSymbole.Distinct());

                            if (etatsArriveePrecedents.Any(tab => tab.EstIdentique(etatsAAnalyser)))
                            { // Les états d'arrivée ont déjà été analysés

                                break;
                            }
                        }
                        else
                        { // Il n'y a pas de transition récupérée ni d'état en arrivée
                            
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
                throw EncapsulerEtGererException<ExceptionTechnique>(
                    ex);
            }
        }

        #endregion Protected Methods
    }
}
