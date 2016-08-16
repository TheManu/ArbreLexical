using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ArbreLexicalService.Arbre.Construction;
using System.Threading.Tasks;
using System.Linq;
using ArbreLexicalService.Arbre.Construction.Dto;

namespace TestsUnitaires.Services.Arbre.Construction
{
    /// <summary>
    /// Description résumée pour FabriqueArbreDepuisJsonTest
    /// </summary>
    [TestClass]
    public class FabriqueArbreDepuisJsonTest
    {
        public FabriqueArbreDepuisJsonTest()
        {
            //
            // TODO: ajoutez ici la logique du constructeur
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Obtient ou définit le contexte de test qui fournit
        ///des informations sur la série de tests active, ainsi que ses fonctionnalités.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Attributs de tests supplémentaires
        //
        // Vous pouvez utiliser les attributs supplémentaires suivants lorsque vous écrivez vos tests :
        //
        // Utilisez ClassInitialize pour exécuter du code avant d'exécuter le premier test de la classe
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Utilisez ClassCleanup pour exécuter du code une fois que tous les tests d'une classe ont été exécutés
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Utilisez TestInitialize pour exécuter du code avant d'exécuter chaque test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Utilisez TestCleanup pour exécuter du code après que chaque test a été exécuté
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public async Task ChargerFichier()
        {
            // Préparations
            IArbreConstruction arbre = new ArbreConstruction();
            IFabriqueArbreDepuisJson fabrique = new FabriqueArbreDepuisJson(
                arbre,
                @"Services\Arbre\Construction\Donnees\Json\ConstructionArbre.json");

            // Action à vérifier
            await fabrique
                .ChargerFichierAsync();

            // Tests
            Assert
                .IsNotNull(
                    (fabrique as FabriqueArbre).ElementsConstruction);
        }

        [TestMethod]
        public async Task ConstruireDepuisFichier()
        {
            // Préparations
            IArbreConstruction arbre = new ArbreConstruction();
            IFabriqueArbreDepuisJson fabrique = new FabriqueArbreDepuisJson(
                arbre,
                @"Services\Arbre\Construction\Donnees\Json\ConstructionArbre.json");
            await fabrique
                .ChargerFichierAsync();

            // Action à vérifier
            await fabrique
                .ConstruireAsync();

            // Tests
            Assert
                .IsNotNull(
                    (fabrique as FabriqueArbre).ElementsConstruction);

            var navigateur = (arbre as ArbreConstruction)
                .RecupererNavigateurMultiSymbolesSur(
                    arbre.EtatEntree);
            navigateur
                .TransitionPar(
                    "long nom_Variable = 5;");
            Assert
                .IsTrue(
                    navigateur.EtatsCourants.Contains(arbre.EtatSortie));
        }

        [TestMethod]
        public async Task ConstruireCheminReussite()
        {
            // Préparations
            IArbreConstruction arbre = new ArbreConstruction();
            const string chemin = "123456";
            var elementRacine = new ElementCheminConstructionDto(
                chemin);
            IFabriqueArbre fabrique = new FabriqueArbre(
                arbre,
                elementRacine);

            // Action à vérifier
            await fabrique
                .ConstruireAsync();

            // Tests
            Assert
                .IsNotNull(
                    (fabrique as FabriqueArbre).ElementsConstruction);

            var navigateur = (arbre as ArbreConstruction)
                .RecupererNavigateurMultiSymbolesSur(
                    arbre.EtatEntree);
            navigateur
                .TransitionPar(
                    chemin);
            Assert
                .IsTrue(
                    navigateur.EtatsCourants.Contains(arbre.EtatSortie));
        }

        [TestMethod]
        public async Task ConstruireSequenceReussite()
        {
            // Préparations
            IArbreConstruction arbre = new ArbreConstruction();
            const string chemin1 = "123456";
            const string chemin2 = "abcdef";
            var elementRacine = new SequenceElementsConstructionDto(
                new[] 
                {
                    new ElementCheminConstructionDto(chemin1),
                    new ElementCheminConstructionDto(chemin2)
                });
            IFabriqueArbre fabrique = new FabriqueArbre(
                arbre,
                elementRacine);

            // Action à vérifier
            await fabrique
                .ConstruireAsync();

            // Tests
            Assert
                .IsNotNull(
                    (fabrique as FabriqueArbre).ElementsConstruction);

            var navigateur = (arbre as ArbreConstruction)
                .RecupererNavigateurMultiSymbolesSur(
                    arbre.EtatEntree);
            navigateur
                .TransitionPar(
                    chemin1);
            navigateur
                .TransitionPar(
                    chemin2);
            Assert
                .IsTrue(
                    navigateur.EtatsCourants.Contains(arbre.EtatSortie));
        }

        [TestMethod]
        public async Task ConstruireChoixReussite()
        {
            // Préparations
            IArbreConstruction arbre = new ArbreConstruction();
            const string chemin1 = "123456";
            const string chemin2 = "abcdef";
            var elementRacine = new ChoixElementsConstructionDto(
                new[] 
                {
                    new ElementCheminConstructionDto(chemin1),
                    new ElementCheminConstructionDto(chemin2)
                });
            IFabriqueArbre fabrique = new FabriqueArbre(
                arbre,
                elementRacine);

            // Action à vérifier
            await fabrique
                .ConstruireAsync();

            // Tests
            Assert
                .IsNotNull(
                    (fabrique as FabriqueArbre).ElementsConstruction);

            // Choix 1
            var navigateur = (arbre as ArbreConstruction)
                .RecupererNavigateurMultiSymbolesSur(
                    arbre.EtatEntree);
            navigateur
                .TransitionPar(
                    chemin1);
            Assert
                .IsTrue(
                    navigateur.EtatsCourants.Contains(arbre.EtatSortie));

            // Choix 2
            navigateur = (arbre as ArbreConstruction)
                .RecupererNavigateurMultiSymbolesSur(
                    arbre.EtatEntree);
            navigateur
                .TransitionPar(
                    chemin2);
            Assert
                .IsTrue(
                    navigateur.EtatsCourants.Contains(arbre.EtatSortie));
        }

        [TestMethod]
        public async Task ConstruireChoixEchec()
        {
            // Préparations
            IArbreConstruction arbre = new ArbreConstruction();
            const string chemin1 = "123456";
            const string chemin2 = "abcdef";
            const string cheminSuivi = "_";
            var elementRacine = new ChoixElementsConstructionDto(
                new[] 
                {
                    new ElementCheminConstructionDto(chemin1),
                    new ElementCheminConstructionDto(chemin2)
                });
            IFabriqueArbre fabrique = new FabriqueArbre(
                arbre,
                elementRacine);

            // Action à vérifier
            await fabrique
                .ConstruireAsync();

            // Tests
            Assert
                .IsNotNull(
                    (fabrique as FabriqueArbre).ElementsConstruction);

            // Sans respecter un choix => échec
            var navigateur = (arbre as ArbreConstruction)
                .RecupererNavigateurMultiSymbolesSur(
                    arbre.EtatEntree);
            navigateur
                .TransitionsSansSymbole();
            Assert
                .IsFalse(
                    navigateur.EtatsCourants.Contains(arbre.EtatSortie));

            // Choix 2
            navigateur = (arbre as ArbreConstruction)
                .RecupererNavigateurMultiSymbolesSur(
                    arbre.EtatEntree);
            navigateur
                .TransitionPar(
                    cheminSuivi);
            Assert
                .AreEqual(
                    0,
                    navigateur.EtatsCourants.Count());
        }

        [TestMethod]
        public async Task ConstruireEtiquette()
        {
            // Préparations
            IArbreConstruction arbre = new ArbreConstruction();
            const string chemin = "123456";
            const string idEtiquette = "Etiq";
            var typeBlock = EnumTypeBlock.Reference;
            var elementEnfant = new ElementCheminConstructionDto(chemin);
            var elementRacine = new ElementEtiquetteConstructionDto(
                typeBlock,
                idEtiquette,
                elementEnfant);
            IFabriqueArbre fabrique = new FabriqueArbre(
                arbre,
                elementRacine);

            // Action à vérifier
            await fabrique
                .ConstruireAsync();

            // Tests
            Assert
                .IsNotNull(
                    (fabrique as FabriqueArbre).ElementsConstruction);
            Assert
                .AreEqual(
                    1,
                    ((fabrique as FabriqueArbre).BlocksInfos?.Count).GetValueOrDefault(0));
            var block = (fabrique as FabriqueArbre).BlocksInfos.First();
            Assert
                .AreEqual(
                    idEtiquette,
                    block.Id);
            Assert
                .AreEqual(
                    elementEnfant,
                    block.Donnees);
       }

        [TestMethod]
        public async Task ConstruireReferenceDoubleNiveau()
        {
            // Préparations
            // Construction de 2 étiquette, l'une appelant l'autre
            IArbreConstruction arbre = new ArbreConstruction();
            const string chemin = "123456";
            var typeBlock = EnumTypeBlock.Reference;
            const string idEtiquetteEnfant = "EtiqEnfant";
            var elementEtiquetteEnfant = new ElementEtiquetteConstructionDto(
                typeBlock,
                idEtiquetteEnfant,
                new ElementCheminConstructionDto(chemin));
            const string idEtiquetteParent = "EtiqParent";
            var elementEtiquetteParent = new ElementEtiquetteConstructionDto(
                typeBlock,
                idEtiquetteParent,
                new ElementReferenceConstructionDto(idEtiquetteEnfant));
            var elementReference = new ElementReferenceConstructionDto(
                idEtiquetteParent);
            var elementSequence = new SequenceElementsConstructionDto(
                new[]
                {
                    elementReference,
                    elementReference
                });
            var elementRacine = new ChoixElementsConstructionDto(
                new ElementConstructionDto[] {
                    elementEtiquetteEnfant,
                    elementEtiquetteParent,
                    elementSequence // = séquence contenant 2 références à l'étiquette de plus haut niveau
                });
            IFabriqueArbre fabrique = new FabriqueArbre(
                arbre,
                elementRacine);

            // Action à vérifier
            await fabrique
                .ConstruireAsync();

            // Tests
            Assert
                .IsNotNull(
                    (fabrique as FabriqueArbre).ElementsConstruction);

            var navigateur = (arbre as ArbreConstruction)
                .RecupererNavigateurMultiSymbolesSur(
                    arbre.EtatEntree);

            // 1 fois => pas suffisant
            navigateur
                .TransitionPar(
                    chemin);
            Assert
                .IsFalse(
                    navigateur.EtatsCourants.Contains(arbre.EtatSortie));

            // 2 fois => ok
            navigateur
                .TransitionPar(
                    chemin);
            Assert
                .IsTrue(
                    navigateur.EtatsCourants.Contains(arbre.EtatSortie));
       }

        [TestMethod]
        public async Task ConstruireReferenceReussite()
        {
            // Préparations
            IArbreConstruction arbre = new ArbreConstruction();
            const string chemin = "123456";
            const string idEtiquette = "Etiq";
            var typeBlock = EnumTypeBlock.Reference;
            var elementEtiquette = new ElementEtiquetteConstructionDto(
                typeBlock,
                idEtiquette,
                new ElementCheminConstructionDto(chemin));
            var elementReference = new ElementReferenceConstructionDto(
                idEtiquette);
            var elementRacine = new ChoixElementsConstructionDto(
                new ElementConstructionDto[] {
                    elementEtiquette,
                    elementReference
                });
            IFabriqueArbre fabrique = new FabriqueArbre(
                arbre,
                elementRacine);

            // Action à vérifier
            await fabrique
                .ConstruireAsync();

            // Tests
            Assert
                .IsNotNull(
                    (fabrique as FabriqueArbre).ElementsConstruction);

            var navigateur = (arbre as ArbreConstruction)
                .RecupererNavigateurMultiSymbolesSur(
                    arbre.EtatEntree);
            navigateur
                .TransitionPar(
                    chemin);
            Assert
                .IsTrue(
                    navigateur.EtatsCourants.Contains(arbre.EtatSortie));
       }

        [TestMethod]
        public async Task ConstruireReferenceEchec()
        {
            // Préparations
            IArbreConstruction arbre = new ArbreConstruction();
            const string chemin = "123456";
            const string cheminSuivi = "ab";
            const string idEtiquette = "Etiq";
            var typeBlock = EnumTypeBlock.Reference;
            var elementEtiquette = new ElementEtiquetteConstructionDto(
                typeBlock,
                idEtiquette,
                new ElementCheminConstructionDto(chemin));
            var elementReference = new ElementReferenceConstructionDto(
                idEtiquette);
            var elementRacine = new ChoixElementsConstructionDto(
                new ElementConstructionDto[] {
                    elementEtiquette,
                    elementReference
                });
            IFabriqueArbre fabrique = new FabriqueArbre(
                arbre,
                elementRacine);

            // Action à vérifier
            await fabrique
                .ConstruireAsync();

            // Tests
            Assert
                .IsNotNull(
                    (fabrique as FabriqueArbre).ElementsConstruction);

            var navigateur = (arbre as ArbreConstruction)
                .RecupererNavigateurMultiSymbolesSur(
                    arbre.EtatEntree);
            navigateur
                .TransitionPar(
                    cheminSuivi);
            Assert
                .IsFalse(
                    navigateur.EtatsCourants.Contains(arbre.EtatSortie));
       }

        [TestMethod]
        public async Task ConstruireEtiquetteSansEnregistrement()
        {
            // Préparations
            IArbreConstruction arbre = new ArbreConstruction();
            const string chemin = "123456";
            const string idEtiquette = "Etiq";
            var typeBlock = EnumTypeBlock.Autre;
            var elementEnfant = new ElementCheminConstructionDto(chemin);
            var elementRacine = new ElementEtiquetteConstructionDto(
                typeBlock,
                idEtiquette,
                elementEnfant);
            IFabriqueArbre fabrique = new FabriqueArbre(
                arbre,
                elementRacine);

            // Action à vérifier
            await fabrique
                .ConstruireAsync();

            // Tests
            Assert
                .IsNotNull(
                    (fabrique as FabriqueArbre).ElementsConstruction);
            Assert
                .AreEqual(
                    0,
                    ((fabrique as FabriqueArbre).BlocksInfos?.Count).GetValueOrDefault(0));
       }

        [TestMethod]
        public async Task ConstruireRepetitionMin1Max2Reussite()
        {
            // Préparations
            IArbreConstruction arbre = new ArbreConstruction();
            const string chemin = "123456";
            var elementRacine = new ElementRepetitionConstructionDto(
                new ElementCheminConstructionDto(chemin),
                1,
                2);
            IFabriqueArbre fabrique = new FabriqueArbre(
                arbre,
                elementRacine);

            // Action à vérifier
            await fabrique
                .ConstruireAsync();

            // Tests
            Assert
                .IsNotNull(
                    (fabrique as FabriqueArbre).ElementsConstruction);

            var navigateur = (arbre as ArbreConstruction)
                .RecupererNavigateurMultiSymbolesSur(
                    arbre.EtatEntree);
            navigateur
                .TransitionPar(
                    chemin);
            Assert
                .IsTrue(
                    navigateur.EtatsCourants.Contains(arbre.EtatSortie));

            navigateur
                .TransitionPar(
                    chemin);
            Assert
                .IsTrue(
                    navigateur.EtatsCourants.Contains(arbre.EtatSortie));
        }

        [TestMethod]
        public async Task ConstruireRepetitionMin2Max3Reussite()
        {
            // Préparations
            IArbreConstruction arbre = new ArbreConstruction();
            const string chemin = "123456";
            var elementRacine = new ElementRepetitionConstructionDto(
                new ElementCheminConstructionDto(chemin),
                2,
                3);
            IFabriqueArbre fabrique = new FabriqueArbre(
                arbre,
                elementRacine);

            // Action à vérifier
            await fabrique
                .ConstruireAsync();

            // Tests
            Assert
                .IsNotNull(
                    (fabrique as FabriqueArbre).ElementsConstruction);

            var navigateur = (arbre as ArbreConstruction)
                .RecupererNavigateurMultiSymbolesSur(
                    arbre.EtatEntree);

            // 1 fois => pas suffisant
            navigateur
                .TransitionPar(
                    chemin);
            Assert
                .IsFalse(
                    navigateur.EtatsCourants.Contains(arbre.EtatSortie));

            // 2 fois => ok
            navigateur
                .TransitionPar(
                    chemin);
            Assert
                .IsTrue(
                    navigateur.EtatsCourants.Contains(arbre.EtatSortie));

            // 3 fois => ok
            navigateur
                .TransitionPar(
                    chemin);
            Assert
                .IsTrue(
                    navigateur.EtatsCourants.Contains(arbre.EtatSortie));
        }

        [TestMethod]
        public async Task ConstruireRepetitionMin2Max3Echec()
        {
            // Préparations
            IArbreConstruction arbre = new ArbreConstruction();
            const string chemin = "123456";
            var elementRacine = new ElementRepetitionConstructionDto(
                new ElementCheminConstructionDto(chemin),
                2,
                3);
            IFabriqueArbre fabrique = new FabriqueArbre(
                arbre,
                elementRacine);

            // Action à vérifier
            await fabrique
                .ConstruireAsync();

            // Tests
            Assert
                .IsNotNull(
                    (fabrique as FabriqueArbre).ElementsConstruction);

            var navigateur = (arbre as ArbreConstruction)
                .RecupererNavigateurMultiSymbolesSur(
                    arbre.EtatEntree);

            // 0 fois => pas suffisant
            navigateur
                .TransitionsSansSymbole();
            Assert
                .IsFalse(
                    navigateur.EtatsCourants.Contains(arbre.EtatSortie));

            // 1 fois => pas suffisant
            navigateur
                .TransitionPar(
                    chemin);
            Assert
                .IsFalse(
                    navigateur.EtatsCourants.Contains(arbre.EtatSortie));

            // 2 fois => ok
            navigateur
                .TransitionPar(
                    chemin);
            Assert
                .IsTrue(
                    navigateur.EtatsCourants.Contains(arbre.EtatSortie));

            // 3 fois => ok
            navigateur
                .TransitionPar(
                    chemin);
            Assert
                .IsTrue(
                    navigateur.EtatsCourants.Contains(arbre.EtatSortie));

            // 4 fois => non
            navigateur
                .TransitionPar(
                    chemin);
            Assert
                .IsFalse(
                    navigateur.EtatsCourants.Contains(arbre.EtatSortie));
        }

        [TestMethod]
        public async Task ConstruireRepetitionMin0MaxInfiniReussite()
        {
            // Préparations
            IArbreConstruction arbre = new ArbreConstruction();
            const string chemin = "123456";
            var elementRacine = new ElementRepetitionConstructionDto(
                new ElementCheminConstructionDto(chemin),
                0,
                int.MaxValue);
            IFabriqueArbre fabrique = new FabriqueArbre(
                arbre,
                elementRacine);

            // Action à vérifier
            await fabrique
                .ConstruireAsync();

            // Tests
            Assert
                .IsNotNull(
                    (fabrique as FabriqueArbre).ElementsConstruction);

            var navigateur = (arbre as ArbreConstruction)
                .RecupererNavigateurMultiSymbolesSur(
                    arbre.EtatEntree);

            // 0 fois => ok
            navigateur
                .TransitionsSansSymbole();
            Assert
                .IsTrue(
                    navigateur.EtatsCourants.Contains(arbre.EtatSortie));

            // 1 fois => ok
            navigateur
                .TransitionPar(
                    chemin);
            Assert
                .IsTrue(
                    navigateur.EtatsCourants.Contains(arbre.EtatSortie));

            // 2 fois => ok
            navigateur
                .TransitionPar(
                    chemin);
            Assert
                .IsTrue(
                    navigateur.EtatsCourants.Contains(arbre.EtatSortie));
        }

        [TestMethod]
        public async Task ConstruireSequenceEchec()
        {
            // Préparations
            IArbreConstruction arbre = new ArbreConstruction();
            const string chemin1 = "123456";
            const string chemin2 = "abcdef";
            const string cheminSuivi = "6";
            var elementRacine = new SequenceElementsConstructionDto(
                new[] 
                {
                    new ElementCheminConstructionDto(chemin1),
                    new ElementCheminConstructionDto(chemin2)
                });
            IFabriqueArbre fabrique = new FabriqueArbre(
                arbre,
                elementRacine);

            // Action à vérifier
            await fabrique
                .ConstruireAsync();

            // Tests
            Assert
                .IsNotNull(
                    (fabrique as FabriqueArbre).ElementsConstruction);

            // Sans cheminer => échec
            var navigateur = (arbre as ArbreConstruction)
                .RecupererNavigateurMultiSymbolesSur(
                    arbre.EtatEntree);
            navigateur
                .TransitionsSansSymbole();
            Assert
                .IsFalse(
                    navigateur.EtatsCourants.Contains(arbre.EtatSortie));

            // Autre chemin => pas de cible
            navigateur = (arbre as ArbreConstruction)
                .RecupererNavigateurMultiSymbolesSur(
                    arbre.EtatEntree);
            navigateur
                .TransitionPar(
                    cheminSuivi);
            Assert
                .AreEqual(
                    0,
                    navigateur.EtatsCourants.Count());
        }

        [TestMethod]
        public async Task ConstruireCheminEchec()
        {
            // Préparations
            IArbreConstruction arbre = new ArbreConstruction();
            const string chemin = "123456";
            const string cheminSuivi = "6";
            var elementRacine = new ElementCheminConstructionDto(
                chemin);
            IFabriqueArbre fabrique = new FabriqueArbre(
                arbre,
                elementRacine);

            // Action à vérifier
            await fabrique
                .ConstruireAsync();

            // Tests
            Assert
                .IsNotNull(
                    (fabrique as FabriqueArbre).ElementsConstruction);

            // Sans cheminer => échec
            var navigateur = (arbre as ArbreConstruction)
                .RecupererNavigateurMultiSymbolesSur(
                    arbre.EtatEntree);
            navigateur
                .TransitionsSansSymbole();
            Assert
                .IsFalse(
                    navigateur.EtatsCourants.Contains(arbre.EtatSortie));

            // Mauvais chemin => pas de cible
            navigateur = (arbre as ArbreConstruction)
                .RecupererNavigateurMultiSymbolesSur(
                    arbre.EtatEntree);
            navigateur
                .TransitionPar(
                    cheminSuivi);
            Assert
                .AreEqual(
                    0,
                    navigateur.EtatsCourants.Count());
        }
    }
}
