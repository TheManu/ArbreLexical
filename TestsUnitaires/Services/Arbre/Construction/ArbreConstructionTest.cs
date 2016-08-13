using System.Linq;
using ArbreLexicalService.Arbre.Construction;
using ArbreLexicalService.Arbre.Dto;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestsUnitaires.Services.Arbre.Construction
{
    /// <summary>
    /// Description résumée pour UnitTest1
    /// </summary>
    [TestClass]
    public class ArbreConstructionTest
    {
        public ArbreConstructionTest()
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
        public void AjouterEtat()
        {
            // Préparations
            IArbreConstruction arbre = new ArbreConstruction();

            // Action à vérifier
            var etat = arbre.AjouterEtat();

            // Tests
            Assert
                .IsNotNull(etat);
            Assert
                .IsTrue(
                    (arbre as ArbreConstruction).Etats.Contains(etat));
        }

        [TestMethod]
        public void AjouterTransitionAvecSymbole()
        {
            // Préparations
            IArbreConstruction arbre = new ArbreConstruction();
            var etatSource = arbre.AjouterEtat();
            var etatCible = arbre.AjouterEtat();
            const char symbole = 'a';

            // Action à vérifier
            var transition = arbre.AjouterTransition(
                etatSource,
                etatCible,
                symbole);

            // Tests
            Assert
                .IsNotNull(transition);
            Assert
                .IsTrue(
                    (arbre as ArbreConstruction).Transitions.Contains(transition));
        }

        [TestMethod]
        public void AjouterMemeTransitionAvecSymbole()
        {
            // Préparations
            IArbreConstruction arbre = new ArbreConstruction();
            var etatSource = arbre.AjouterEtat();
            var etatCible = arbre.AjouterEtat();
            const char symbole = 'a';
            var transition1 = arbre.AjouterTransition(
                etatSource,
                etatCible,
                symbole);

            // Action à vérifier
            var transition2 = arbre.AjouterTransition(
                etatSource,
                etatCible,
                symbole);

            // Tests
            Assert
                .AreEqual(
                    transition1,
                    transition2);
        }

        [TestMethod]
        public void Ajouter2TransitionsAvecMemeSymbole()
        {
            // Préparations
            IArbreConstruction arbre = new ArbreConstruction();
            var etatSource = arbre.AjouterEtat();
            var etatCible1 = arbre.AjouterEtat();
            var etatCible2 = arbre.AjouterEtat();
            const char symbole = 'a';
            var transition1 = arbre.AjouterTransition(
                etatSource,
                etatCible1,
                symbole);

            // Action à vérifier
            var transition2 = arbre.AjouterTransition(
                etatSource,
                etatCible2,
                symbole);

            // Tests
            var etatSourceInfos = (arbre as ArbreConstruction)
                .RecupererInfos(etatSource);
            var etatSourceTransitionsSortantes = etatSourceInfos
                .TransitionsSortantes;
            var transitionIntermediaire = etatSourceTransitionsSortantes
                .RecupererTransitionAvecSymbole(symbole);
            var etatIntermediaire = transitionIntermediaire
                .EtatCible;
            var etatIntermediaireInfos = (arbre as ArbreConstruction)
                .RecupererInfos(etatIntermediaire);
            var etatIntermediaireTransitionsSortantes = etatIntermediaireInfos
                .TransitionsSortantes
                .Transitions;

            Assert
                .IsFalse(
                    new[] { etatSource, etatCible1, etatCible2 }.Contains(etatIntermediaire));
            Assert
                .IsTrue(
                    etatSourceTransitionsSortantes.Transitions.All(t => t != transition1));
            Assert
                .AreEqual(
                    symbole,
                    transitionIntermediaire.Symbole);
            Assert
                .AreEqual(
                    etatSource,
                    transitionIntermediaire.EtatSource);
            Assert
                .AreEqual(
                    2,
                    etatIntermediaireTransitionsSortantes.Count());
            Assert
                .IsTrue(
                    etatIntermediaireTransitionsSortantes.All(t => !t.Symbole.HasValue));
            Assert
                .IsNull(
                    transition2.Symbole);
            Assert
                .IsTrue(
                    etatIntermediaireTransitionsSortantes.Any(t => t.EtatCible == etatCible1));
            Assert
                .IsTrue(
                    etatIntermediaireTransitionsSortantes.Any(t => t.EtatCible == etatCible2));
        }

        [TestMethod]
        public void AjouterMemeTransitionSansSymbole()
        {
            // Préparations
            IArbreConstruction arbre = new ArbreConstruction();
            var etatSource = arbre.AjouterEtat();
            var etatCible = arbre.AjouterEtat();
            var transition1 = arbre.AjouterTransition(
                etatSource,
                etatCible);

            // Action à vérifier
            var transition2 = arbre.AjouterTransition(
                etatSource,
                etatCible);

            // Tests
            Assert
                .AreEqual(
                    transition1,
                    transition2);
        }

        [TestMethod]
        public void AjouterTransitionAvecSymboleRecursif()
        {
            // Préparations
            IArbreConstruction arbre = new ArbreConstruction();
            var etatRecursif = arbre.AjouterEtat();
            const char symbole = 'a';

            // Action à vérifier
            var transition = arbre.AjouterTransition(
                etatRecursif,
                symbole);

            // Tests
            Assert
                .IsNotNull(transition);
            Assert
                .IsTrue(
                    transition.EstEquivalentA(new Transition(etatRecursif, symbole)));
            Assert
                .IsTrue(
                    (arbre as ArbreConstruction).Transitions.Contains(transition));
        }

        [TestMethod]
        public void AjouterTransitionSansSymbole()
        {
            // Préparations
            IArbreConstruction arbre = new ArbreConstruction();
            var etatSource = arbre.AjouterEtat();
            var etatCible = arbre.AjouterEtat();

            // Action à vérifier
            var transition = arbre.AjouterTransition(
                etatSource,
                etatCible);

            // Tests
            Assert
                .IsNotNull(transition);
            Assert
                .IsTrue(
                    (arbre as ArbreConstruction).Transitions.Contains(transition));
        }

        [TestMethod]
        public void FinaliserArbre()
        {
            // Préparations
            IArbreConstruction constructeur = new ArbreConstruction();
            var etatSource = constructeur.AjouterEtat();
            var etatCible = constructeur.AjouterEtat();
            var transition = constructeur.AjouterTransition(
                etatSource,
                etatCible);

            // Action à vérifier
            var arbre = constructeur.FinaliserArbre();

            // Tests
            Assert
                .IsNotNull(arbre);
            Assert
                .IsTrue(
                    arbre.Etats.Any(e => e == etatSource && e.TransitionsSortantes.Count() == 1 && e.TransitionsSortantes.First() == transition));
            Assert
                .IsTrue(
                    arbre.Etats.Any(e => e == etatCible && !e.TransitionsSortantes.Any()));
        }
    }
}
