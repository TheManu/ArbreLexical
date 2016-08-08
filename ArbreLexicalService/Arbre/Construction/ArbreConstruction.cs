using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArbreLexicalService.Arbre.Cheminement;
using ArbreLexicalService.Arbre.Dto;
using ArbreLexicalService.Exceptions;
using Common.Collections;
using Common.Exceptions;
using Common.Ioc;
using Common.Locks;
using Common.Reflexion;

namespace ArbreLexicalService.Arbre.Construction
{
    internal class ArbreConstruction : IArbreConstruction
    {

        #region Private Fields

        private readonly Dictionary<Etat, EtatInfos> dicoEtatsInfos =
            new Dictionary<Etat, EtatInfos>();

        private readonly ILockLectureEtEcriture lockeur =
            Fabrique.Instance.RecupererInstance<ILockLectureEtEcriture>();

        #endregion Private Fields

        #region Public Properties

        public Etat[] Etats
        {
            get
            {
                using (lockeur.RecupererLockLecture())
                {
                    return dicoEtatsInfos
                        .Keys
                        .ToArray();
                }
            }
        }

        public Transition[] Transitions
        {
            get
            {
                using (lockeur.RecupererLockLecture())
                {
                    return dicoEtatsInfos
                        .Values
                        .SelectMany(ei =>
                            ei.TransitionsSortantes.Transitions)
                        .Distinct()
                        .ToArray();
                }
            }
        }

        #endregion Public Properties

        #region Public Methods

        public IArbreLexical FinaliserArbre()
        {
            try
            {
                using (lockeur.RecupererLockLecture())
                {
                    //todo nettoyer les états
                    //todo gérer tags

                    // Rempli les états avec ses transitions
                    foreach (var kv in dicoEtatsInfos)
                    {
                        var etat = kv.Key;
                        var infos = kv.Value;

                        etat.TransitionsSortantes = infos.TransitionsSortantes.Transitions;
                    }

                    return Fabrique.Instance
                        ?.RecupererInstance<IArbreLexical, IEnumerable<Etat>>(
                            dicoEtatsInfos.Keys); 
                }
            }
            catch (Exception ex)
            {
                Fabrique.Instance
                    ?.RecupererGestionnaireTraces()
                    ?.PublierException(
                        ex);

                throw new ExceptionArbreConstruction(
                    ExceptionBase.RecupererLibelleErreur(),
                    ex);
            }
        }

        public Etat AjouterEtat()
        {
            try
            {
                return AjouterEtatEtSesInfos()
                    ?.EtatOrigine;
            }
            catch (Exception ex)
            {
                Fabrique.Instance
                    ?.RecupererGestionnaireTraces()
                    ?.PublierException(
                        ex);

                throw new ExceptionArbreConstruction(
                    ExceptionBase.RecupererLibelleErreur(),
                    ex);
            }
        }

        public Transition AjouterTransition(
            Etat etatSource,
            Etat etatCible)
        {
            return AjouterTransition(
                etatSource,
                etatCible,
                null);
        }

        public Transition AjouterTransition(
            Etat etatSource,
            Etat etatCible,
            char symbole)
        {
            return AjouterTransition(
                etatSource,
                etatCible,
                new char?(symbole));
        }

        public Transition AjouterTransition(
            Etat etatSource,
            Etat etatCible,
            char? symbole)
        {
            try
            {
                // Récupération des infos sur les états source et cible
                EtatInfos etatInfosSource = null,
                    etatInfosCible = null;

                using (lockeur.RecupererLockLecture())
                {
                    bool etatSourceDansArbre = dicoEtatsInfos
                        .TryGetValue(
                            etatSource,
                            out etatInfosSource);
                    bool etatCibleDansArbre = dicoEtatsInfos
                        .TryGetValue(
                            etatCible,
                            out etatInfosCible); ;

                    if (!etatSourceDansArbre ||
                        !etatCibleDansArbre)
                    {
                        throw new ExceptionArbreConstruction(
                            ExceptionBase.RecupererLibelleErreur());
                    }
                } // Note : on lève le verrou sur le dico pour maintenir les performances. On vérifie plus loin que les états existent encore (après les avoir lockés)...

                // Lock sur les états source et cible en évitant les inter-locks : lock toujours dans le même ordre
                var lockeurs = RecupererLockeurs(
                    etatInfosSource,
                    etatInfosCible);

                using (lockeurs[0].RecupererLockEcriture())
                {
                    using ((lockeurs.Length == 2 ? lockeurs[1] : new LockEcritureEtLectureFake()) // Si la transition est récursive, alors Fake
                        .RecupererLockEcriture())
                    {
                        // Les états source et cible sont-ils encore dans l'arbre ? (vérification pour se prémunir dans un environnement multi-threads)
                        if (!etatInfosSource.EstActif ||
                            !etatInfosCible.EstActif)
                        { // L'état source et/ou l'état cible n'existe(nt) plus dans l'arbre => erreur

                            throw new ExceptionArbreConstruction(
                                ExceptionBase.RecupererLibelleErreur());
                        }

                        // La transition existe t'elle déjà ?
                        var transition = new Transition(
                            etatSource,
                            etatCible,
                            symbole);

                        var etatSourceTransitionsSortantes = etatInfosSource.TransitionsSortantes;
                        var transitionEquivalente = etatSourceTransitionsSortantes
                            .RecupererTransitionEquivalente(transition);

                        if (null != transitionEquivalente)
                        { // La transition existe déjà => pas d'ajout
                            // RDG 1 : Il ne peut y avoir qu'une seule transition pour un état cible et un symbole (même vide) donné, pour un état source.

                            return transitionEquivalente;
                        }
                        else
                        { // La transition n'existe pas encore => ajout

                            if (symbole.HasValue)
                            { // Ajout d'une transition avec symbole

                                // Le symbole existe t'il déjà sur une transition sortante ?
                                // RDG 2 : Un état ne doit avoir qu'une transition sortante par symbole.
                                // RDG 3 : Il peut y avoir plusieurs transitions sans symbole vers des états différents.
                                transitionEquivalente = etatSourceTransitionsSortantes
                                    .RecupererTransitionAvecSymbole(
                                        symbole.Value);

                                if (null != transitionEquivalente)
                                { // Une transition existe déjà avec le même symbole (vers un autre état) => reconstruction pour respecter les RDG
                                    // Création d'un état intermédiaire qui deviendra la cible du symbole.
                                    // Il sera également la source de transitions sans symbole :
                                    //      - vers l'état ciblé par la transition équivalente 
                                    //      - et l'état ciblé en paramètre.

                                    var etatCibleSurEquivalence = transitionEquivalente.EtatCible;
                                    var etatInfosIntermediaire = CreerEtatEtSesInfos(); // Création de l'état intermédiaire sans l'ajouter dans l'arbre, ce qui permet de le locker
                                    var etatIntermediaire = etatInfosIntermediaire.EtatOrigine;
                                    var etatIntermediaireTransitionsSortantes = etatInfosIntermediaire.TransitionsSortantes;

                                    // Préparation des transitions
                                    var transitionIntermediaire = new Transition(
                                        etatSource,
                                        etatIntermediaire,
                                        symbole.Value);
                                    var transitionVersEtatCibleOrigine = new Transition(
                                        etatIntermediaire,
                                        etatCibleSurEquivalence);
                                    var transitionVersEtatCible = new Transition(
                                        etatIntermediaire,
                                        etatCible);

                                    // Ajout de l'état intermédiaire et des transitions dans l'arbre
                                    using (etatInfosIntermediaire.Lockeur.RecupererLockEcriture())
                                    {
                                        Ajouter(
                                            etatInfosIntermediaire);

                                        etatIntermediaireTransitionsSortantes
                                            .Ajouter(transitionVersEtatCibleOrigine)
                                            .Ajouter(transitionVersEtatCible);
                                        etatSourceTransitionsSortantes
                                            .Supprimer(transitionEquivalente)
                                            .Ajouter(transitionIntermediaire);
                                    }

                                    return transitionVersEtatCible;
                                }
                                else
                                { // Transition avec symbole, le symbole n'est pas encore déclaré => ajout
                                }
                            }
                            else
                            { // Transition sans symbole vers un état pas encore ciblé => ajout                                
                            }

                            // Ajout de la transition
                            etatSourceTransitionsSortantes
                                .Ajouter(transition);

                            return transition;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Fabrique.Instance
                    ?.RecupererGestionnaireTraces()
                    ?.PublierException(
                        ex);

                throw new ExceptionArbreConstruction(
                    ExceptionBase.RecupererLibelleErreur(),
                    ex);
            }
        }

        public Transition AjouterTransition(Etat etatAvecReflexion)
        {
            return AjouterTransition(
                etatAvecReflexion,
                etatAvecReflexion,
                null);
        }

        public Transition AjouterTransition(
            Etat etatAvecReflexion,
            char symbole)
        {
            return AjouterTransition(
                etatAvecReflexion,
                etatAvecReflexion,
                new char?(symbole));
        }

        public Transition AjouterTransition(
            Etat etatAvecReflexion,
            char? symbole)
        {
            return AjouterTransition(
                etatAvecReflexion,
                etatAvecReflexion,
                symbole);
        }

        #endregion Public Methods

        #region Internal Methods

        internal EtatInfos RecupererInfos(
            Etat etat)
        {
            try
            {
                EtatInfos etatInfos = null;

                using (lockeur.RecupererLockLecture())
                {
                    dicoEtatsInfos
                        .TryGetValue(
                            etat,
                            out etatInfos);
                }

                return etatInfos;
            }
            catch (Exception ex)
            {
                Fabrique.Instance
                    ?.RecupererGestionnaireTraces()
                    ?.PublierException(
                        ex);

                throw new ExceptionArbreConstruction(
                    ExceptionBase.RecupererLibelleErreur(),
                    ex);
            }
        }

        internal INavigation1Symbole RecupererNavigateurSur(
            params Etat[] etats)
        {
            try
            {
                return RecupererNavigateurSur(
                    etats.AsEnumerable());
            }
            catch (Exception ex)
            {
                Fabrique.Instance
                    ?.RecupererGestionnaireTraces()
                    ?.PublierException(
                        ex);

                throw new ExceptionArbreConstruction(
                    ExceptionBase.RecupererLibelleErreur(),
                    ex);
            }
        }

        internal INavigation1Symbole RecupererNavigateurSur(
            IEnumerable<Etat> etatsOrigine)
        {
            try
            {
                var navigateur = new Navigation1Symbole(
                    this);
                navigateur
                    .DefinirEtatsOrigine(etatsOrigine);

                return navigateur;
            }
            catch (Exception ex)
            {
                Fabrique.Instance
                    ?.RecupererGestionnaireTraces()
                    ?.PublierException(
                        ex);

                throw new ExceptionArbreConstruction(
                    ExceptionBase.RecupererLibelleErreur(),
                    ex);
            }
        }

        internal INavigationSurSymboles RecupererNavigateurMultiSymbolesSur(
            params Etat[] etats)
        {
            try
            {
                return RecupererNavigateurMultiSymbolesSur(
                    etats.AsEnumerable());
            }
            catch (Exception ex)
            {
                Fabrique.Instance
                    ?.RecupererGestionnaireTraces()
                    ?.PublierException(
                        ex);

                throw new ExceptionArbreConstruction(
                    ExceptionBase.RecupererLibelleErreur(),
                    ex);
            }
        }

        internal INavigationSurSymboles RecupererNavigateurMultiSymbolesSur(
            IEnumerable<Etat> etatsOrigine)
        {
            try
            {
                var navigateur = new NavigationSurSymboles(
                    this);
                navigateur
                    .DefinirEtatsOrigine(etatsOrigine);

                return navigateur;
            }
            catch (Exception ex)
            {
                Fabrique.Instance
                    ?.RecupererGestionnaireTraces()
                    ?.PublierException(
                        ex);

                throw new ExceptionArbreConstruction(
                    ExceptionBase.RecupererLibelleErreur(),
                    ex);
            }
        }

        #endregion Internal Methods

        #region Private Methods

        private static EtatInfos CreerEtatEtSesInfos()
        {
            try
            {
                var etat = new Etat();
                var etatInfos = new EtatInfos(
                    etat);

                return etatInfos;
            }
            catch (Exception ex)
            {
                Fabrique.Instance
                    ?.RecupererGestionnaireTraces()
                    ?.PublierException(
                        ex);

                throw new ExceptionArbreConstruction(
                    ExceptionBase.RecupererLibelleErreur(),
                    ex);
            }
        }

        private static ILockLectureEtEcriture[] RecupererLockeurs(
                            params EtatInfos[] etatsInfos)
        {
            return etatsInfos
                .Distinct()
                .OrderBy(ei =>
                    ei.EtatOrigine.Identifiant)
                .Select(ei => ei.Lockeur)
                .ToArray();
        }
        private EtatInfos Ajouter(
            EtatInfos etatInfos)
        {
            try
            {
                etatInfos.EstActif = true;

                using (lockeur.RecupererLockEcriture())
                {
#if DEBUG
                    if (dicoEtatsInfos.ContainsKey(etatInfos.EtatOrigine))
                    { // L'état est déjà dans l'arbre => erreur

                        throw new ExceptionArbreConstruction(
                            ExceptionBase.RecupererLibelleErreur());
                    }
#endif

                    dicoEtatsInfos
                        .Add(
                            etatInfos.EtatOrigine,
                            etatInfos);

                    return etatInfos;
                }
            }
            catch (Exception ex)
            {
                Fabrique.Instance
                    ?.RecupererGestionnaireTraces()
                    ?.PublierException(
                        ex);

                throw new ExceptionArbreConstruction(
                    ExceptionBase.RecupererLibelleErreur(),
                    ex);
            }
        }

        private EtatInfos AjouterEtatEtSesInfos()
        {
            try
            {
                EtatInfos etatInfos = CreerEtatEtSesInfos();

                return Ajouter(
                    etatInfos);
            }
            catch (Exception ex)
            {
                Fabrique.Instance
                    ?.RecupererGestionnaireTraces()
                    ?.PublierException(
                        ex);

                throw new ExceptionArbreConstruction(
                    ExceptionBase.RecupererLibelleErreur(),
                    ex);
            }
        }

        #endregion Private Methods

        #region Private Classes

        private class Navigation1Symbole : Navigation1SymboleBase, INavigation1Symbole
        {

            #region Private Fields

            private readonly ArbreConstruction arbre;

            #endregion Private Fields

            #region Public Constructors

            public Navigation1Symbole(
                ArbreConstruction arbre,
                IEnumerable<Etat> etatsOrigine) : base(etatsOrigine)
            {
                this.arbre = arbre;
            }

            public Navigation1Symbole(
                ArbreConstruction arbre) : this(arbre, null)
            {
            }

            #endregion Public Constructors

            #region Protected Methods

            protected override Transition RecupererTransitionAvecSymbole(
                Etat etatSource,
                char symbole)
            {
                try
                {
                    var etatInfos = arbre
                        .RecupererInfos(etatSource);

                    if (null != etatInfos)
                    {
                        using (etatInfos.Lockeur.RecupererLockLecture())
                        {
                            if (etatInfos.EstActif)
                            {
                                var transitionsSortantes = etatInfos
                                    .TransitionsSortantes;

                                return transitionsSortantes
                                    .RecupererTransitionAvecSymbole(symbole);
                            }
                        }
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    Fabrique.Instance
                        ?.RecupererGestionnaireTraces()
                        ?.PublierException(
                            ex);

                    throw new ExceptionArbreConstruction(
                        ExceptionBase.RecupererLibelleErreur(),
                        ex);
                }
            }

            protected override Transition[] RecupererTransitionsSansSymbole(
                Etat etatSource)
            {
                try
                {
                    var etatInfos = arbre
                        .RecupererInfos(etatSource);

                    if (null != etatInfos)
                    {
                        using (etatInfos.Lockeur.RecupererLockLecture())
                        {
                            if (etatInfos.EstActif)
                            {
                                var transitionsSortantes = etatInfos
                                    .TransitionsSortantes;

                                return transitionsSortantes
                                    .RecupererTransitionsSansSymbole();
                            }
                        }
                    }

                    return new Transition[0];
                }
                catch (Exception ex)
                {
                    Fabrique.Instance
                        ?.RecupererGestionnaireTraces()
                        ?.PublierException(
                            ex);

                    throw new ExceptionArbreConstruction(
                        ExceptionBase.RecupererLibelleErreur(),
                        ex);
                }
            }

            #endregion Protected Methods

        }

        private class NavigationSurSymboles : NavigationSurSymbolesBase, INavigationSurSymboles
        {

            #region Private Fields

            private readonly ArbreConstruction arbre;

            #endregion Private Fields

            #region Public Constructors

            public NavigationSurSymboles(
                ArbreConstruction arbre,
                IEnumerable<Etat> etatsOrigine) : base(etatsOrigine)
            {
                this.arbre = arbre;
            }

            public NavigationSurSymboles(
                ArbreConstruction arbre) : this(arbre, null)
            {
            }

            #endregion Public Constructors

            #region Protected Methods

            protected override INavigation1Symbole FabriquerNavigation()
            {
                try
                {
                    return new Navigation1Symbole(
                        arbre,
                        NavigationCourante?.EtatsCourants ?? etatsOrigine);
                }
                catch (Exception ex)
                {
                    Fabrique.Instance
                        ?.RecupererGestionnaireTraces()
                        ?.PublierException(
                            ex);

                    throw new ExceptionArbreConstruction(
                        ExceptionBase.RecupererLibelleErreur(),
                        ex);
                }
            }

            #endregion Protected Methods

        }

        #endregion Private Classes

    }
}
