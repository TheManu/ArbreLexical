using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArbreLexicalService.Arbre.Cheminement;
using ArbreLexicalService.Arbre.Construction.Elements;
using ArbreLexicalService.Arbre.Dto;
using ArbreLexicalService.Exceptions;
using Common.Collections;
using Common.Exceptions;
using Common.Ioc;
using Common.Locks;
using Common.Reflexion;
using Common.Services;

namespace ArbreLexicalService.Arbre.Construction
{
    internal class ArbreConstruction : ServiceBase, IArbreConstruction
    {

        #region Private Fields

        private readonly Dictionary<Etat, EtatInfos> dicoEtatsInfos =
            new Dictionary<Etat, EtatInfos>();

        private readonly Etat etatEntree;

        private readonly Etat etatSortie;

        private readonly ILockLectureEtEcriture lockeur =
            Fabrique.Instance.RecupererInstance<ILockLectureEtEcriture>();

        #endregion Private Fields

        #region Public Constructors

        public ArbreConstruction()
        {
            etatEntree = AjouterEtatDansConstructeur();
            etatSortie = AjouterEtatDansConstructeur();
        }

        #endregion Public Constructors

        #region Public Properties

        public Etat EtatEntree
        {
            get
            {
                return etatEntree;
            }
        }

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

        public Etat EtatSortie
        {
            get
            {
                return etatSortie;
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

        public void Etiquetter(
            string id,
            EnumTypeBlock typeBlock,
            Etat etatEntree,
            Etat etatSortie)
        {
            try
            {
                // Récupération des infos sur les états en entrée et en sortie
                EtatInfos etatInfosEntree = null,
                    etatInfosSortie = null;

                using (lockeur.RecupererLockLecture())
                {
                    bool etatEntreeDansArbre = dicoEtatsInfos
                        .TryGetValue(
                            etatEntree,
                            out etatInfosEntree);
                    bool etatSortieDansArbre = dicoEtatsInfos
                        .TryGetValue(
                            etatSortie,
                            out etatInfosSortie);

                    if (!etatEntreeDansArbre ||
                        !etatSortieDansArbre)
                    {
                        throw new ExceptionTechniqueArbreConstruction(
                            ExceptionBase.RecupererLibelleMessage());
                    }
                } // Note : on lève le verrou sur le dico pour maintenir les performances. On vérifie plus loin que les états existent encore (après les avoir lockés)...

                // Lock sur les états source et cible en évitant les inter-locks : lock toujours dans le même ordre
                var lockeurs = RecupererLockeurs(
                    etatInfosEntree,
                    etatInfosSortie);

                using (lockeurs[0].RecupererLockEcriture())
                {
                    if (!etatInfosEntree.EstActif)
                    {
                        throw new ExceptionTechniqueArbreConstruction(
                            ExceptionBase.RecupererLibelleMessage());
                    }

                    etatEntree.Etiquette = new EtiquetteDto(
                        id,
                        typeBlock,
                        EnumExtremiteEtiquette.Entree);

                    etatInfosEntree
                        .InterdireTransitionSortante();
                }

                if (lockeurs.Length == 2)
                {
                    using (lockeurs[1].RecupererLockEcriture())
                    {
                        if (!etatInfosSortie.EstActif)
                        {
                            throw new ExceptionTechniqueArbreConstruction(
                                ExceptionBase.RecupererLibelleMessage());
                        }

                        etatSortie.Etiquette = new EtiquetteDto(
                            id,
                            typeBlock,
                            EnumExtremiteEtiquette.Sortie);

                        etatInfosSortie
                            .InterdireTransitionEntrante();
                    }
                }
            }
            catch (Exception ex)
            {
                throw EncapsulerEtGererException<ExceptionTechniqueArbreConstruction>(
                    ex);
            }
        }

        public Transition[] AjouterChemin(
            Etat etatDebut,
            string chemin)
        {
            try
            {
                var transitions = new List<Transition>();

                if (!string.IsNullOrEmpty(chemin))
                {
                    foreach (var symbole in chemin)
                    {
                        var etat = AjouterEtat();

                        var transition = AjouterTransition(
                            etatDebut,
                            etat,
                            symbole);
                        transitions
                            .Add(transition);

                        etatDebut = etat;
                    }
                }

                return transitions
                    .ToArray();
            }
            catch (Exception ex)
            {
                throw EncapsulerEtGererException<ExceptionTechniqueArbreConstruction>(
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
                throw EncapsulerEtGererException<ExceptionTechniqueArbreConstruction>(
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
                            out etatInfosCible);

                    if (!etatSourceDansArbre ||
                        !etatCibleDansArbre)
                    {
                        throw new ExceptionTechniqueArbreConstruction(
                            ExceptionBase.RecupererLibelleMessage());
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

                            throw new ExceptionTechniqueArbreConstruction(
                                ExceptionBase.RecupererLibelleMessage());
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

                            if (!etatInfosSource.AjoutTransitionSortantePossible)
                            {
                                throw new ExceptionTechniqueArbreConstruction(
                                    ExceptionBase.RecupererLibelleMessage(
                                        $"L'ajout d'une transition sortante sur l'état {etatInfosCible.EtatOrigine.Identifiant} est interdit"));
                            }

                            if (!etatInfosCible.AjoutTransitionEntrantePossible)
                            {
                                throw new ExceptionTechniqueArbreConstruction(
                                    ExceptionBase.RecupererLibelleMessage(
                                        $"L'ajout d'une transition entrante sur l'état {etatInfosCible.EtatOrigine.Identifiant} est interdit"));
                            }

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
                throw EncapsulerEtGererException<ExceptionTechniqueArbreConstruction>(
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

        public IConstructionElementArbre CreerElement(
            IEnumerable<Etat> etatsConnexionDebut,
            IEnumerable<Etat> etatsConnexionFin)
        {
            try
            {
                var element = CreerElement();

                if (null != etatsConnexionDebut)
                {
                    foreach (var etatSource in etatsConnexionDebut)
                    {
                        AjouterTransition(
                            etatSource,
                            element.EtatEntree);
                    }
                }

                if (null != etatsConnexionFin)
                {
                    foreach (var etatCible in etatsConnexionFin)
                    {
                        AjouterTransition(
                            element.EtatSortie,
                            etatCible);
                    }
                }

                return element;
            }
            catch (Exception ex)
            {
                throw EncapsulerEtGererException<ExceptionTechniqueArbreConstruction>(
                    ex);
            }
        }

        public IConstructionElementArbre CreerElement(
            IEnumerable<Etat> etatsConnexionDebut)
        {
            try
            {
                return CreerElement(
                    etatsConnexionDebut,
                    null);
            }
            catch (Exception ex)
            {
                throw EncapsulerEtGererException<ExceptionTechniqueArbreConstruction>(
                    ex);
            }
        }

        public IArbreLexical FinaliserArbre()
        {
            try
            {
                using (lockeur.RecupererLockLecture())
                {
                    //todo nettoyer les états

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
                throw EncapsulerEtGererException<ExceptionTechniqueArbreConstruction>(
                    ex);
            }
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
                throw EncapsulerEtGererException<ExceptionTechniqueArbreConstruction>(
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
                throw EncapsulerEtGererException<ExceptionTechniqueArbreConstruction>(
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
                throw EncapsulerEtGererException<ExceptionTechniqueArbreConstruction>(
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
                throw EncapsulerEtGererException<ExceptionTechniqueArbreConstruction>(
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
                throw EncapsulerEtGererException<ExceptionTechniqueArbreConstruction>(
                    ex);
            }
        }

        #endregion Internal Methods

        #region Private Methods

        private EtatInfos CreerEtatEtSesInfos()
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
                throw EncapsulerEtGererException<ExceptionTechniqueArbreConstruction>(
                    ex);
            }
        }

        private ILockLectureEtEcriture[] RecupererLockeurs(
            params EtatInfos[] etatsInfos)
        {
            try
            {
                return etatsInfos
                    .Distinct()
                    .OrderBy(ei =>
                        ei.EtatOrigine.Identifiant)
                    .Select(ei => ei.Lockeur)
                    .ToArray();
            }
            catch (Exception ex)
            {
                throw EncapsulerEtGererException<ExceptionTechniqueArbreConstruction>(
                    ex);
            }
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

                        throw new ExceptionTechniqueArbreConstruction(
                            ExceptionBase.RecupererLibelleMessage());
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
                throw EncapsulerEtGererException<ExceptionTechniqueArbreConstruction>(
                    ex);
            }
        }

        private Etat AjouterEtatDansConstructeur()
        {
            try
            {
                var etat = new Etat();
                var etatInfos = new EtatInfos(
                    etat);
                etatInfos.EstActif = true;

                dicoEtatsInfos
                    .Add(
                        etat,
                        etatInfos);

                return etat;
            }
            catch (Exception ex)
            {
                throw EncapsulerEtGererException<ExceptionTechniqueArbreConstruction>(
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
                throw EncapsulerEtGererException<ExceptionTechniqueArbreConstruction>(
                    ex);
            }
        }

        private IConstructionElementArbre CreerElement()
        {
            try
            {
                var element = Fabrique.Instance
                    .RecupererInstance<IConstructionElementArbre, IArbreConstruction>(
                        this);
                element
                    .CreerAsync();

                return element;
            }
            catch (Exception ex)
            {
                throw EncapsulerEtGererException<ExceptionTechniqueArbreConstruction>(
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
                    throw EncapsulerEtGererException<ExceptionTechniqueArbreConstruction>(
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
                    throw EncapsulerEtGererException<ExceptionTechniqueArbreConstruction>(
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
                    throw EncapsulerEtGererException<ExceptionTechniqueArbreConstruction>(
                        ex);
                }
            }

            #endregion Protected Methods

        }

        #endregion Private Classes

    }
}
