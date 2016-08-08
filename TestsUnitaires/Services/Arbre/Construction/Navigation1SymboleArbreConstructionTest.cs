using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ArbreLexicalService.Arbre.Construction;
using System.Linq;

namespace TestsUnitaires.Services.Arbre.Construction
{
    /// <summary>
    /// Description résumée pour EtatTransitionsSortantesMultiNiveauxArbreConstructionTest
    /// </summary>
    [TestClass]
    public class Navigation1SymboleArbreConstructionTest
    {
        #region Private Fields

        private TestContext testContextInstance;

        #endregion Private Fields

        #region Public Constructors

        public Navigation1SymboleArbreConstructionTest()
        {
            //
            // TODO: ajoutez ici la logique du constructeur
            //
        }

        #endregion Public Constructors

        #region Public Properties

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

        #endregion Public Properties

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

        #region Public Methods

        [TestMethod]
        public void DefinirEtatsOrigine()
        {
            // Préparations
            IArbreConstruction arbre = new ArbreConstruction();
            var etat1 = arbre.AjouterEtat();
            var etat2 = arbre.AjouterEtat();

            // Action à vérifier
            var navigateur = (arbre as ArbreConstruction)
                .RecupererNavigateurSur(
                    etat1,
                    etat2);

            // Tests

            // Tests sur l'origine
            Assert
                .AreEqual(
                    2,
                    navigateur.EtatsOrigine.Count());
            Assert
                .IsTrue(
                    navigateur.EtatsOrigine.Contains(etat1));
            Assert
                .IsTrue(
                    navigateur.EtatsOrigine.Contains(etat2));

            // Tests sur les états courants
            Assert
                .AreEqual(
                    2,
                    navigateur.EtatsCourants.Count());
            Assert
                .IsTrue(
                    navigateur.EtatsCourants.Contains(etat1));
            Assert
                .IsTrue(
                    navigateur.EtatsCourants.Contains(etat2));

            // Tests sur les transitions
            Assert
                .AreEqual(
                    0,
                    navigateur.Transitions.Count());
        }

        [TestMethod]
        public void TransitionsAvecSymbole()
        {
            // Préparations
            IArbreConstruction arbre = new ArbreConstruction();
            const char symbole = 'a';
            var etat1 = arbre.AjouterEtat();
            var etat2 = arbre.AjouterEtat();
            var etat3 = arbre.AjouterEtat();
            var etat4 = arbre.AjouterEtat();
            var etat5 = arbre.AjouterEtat();

            arbre.AjouterTransition(
                etat1,
                etat2);
            arbre.AjouterTransition(
                etat1,
                etat4,
                symbole);
            arbre.AjouterTransition(
                etat2,
                etat3);
            arbre.AjouterTransition(
                etat4,
                etat5);

            var navigateur = (arbre as ArbreConstruction)
                .RecupererNavigateurSur(
                    etat1);

            // Action à vérifier
            navigateur
                .TransitionPar(symbole);

            // Tests

            // Tests sur l'origine
            Assert
                .AreEqual(
                    1,
                    navigateur.EtatsOrigine.Count());
            Assert
                .IsTrue(
                    navigateur.EtatsOrigine.Contains(etat1));

            // Tests sur les états courants
            Assert
                .AreEqual(
                    2,
                    navigateur.EtatsCourants.Count());
            Assert
                .IsTrue(
                    navigateur.EtatsCourants.Contains(etat4));
            Assert
                .IsTrue(
                    navigateur.EtatsCourants.Contains(etat5));

            // Tests sur les transitions
            Assert
                .AreEqual(
                    2,
                    navigateur.Transitions.Count());
            Assert
                .IsTrue(
                    navigateur.Transitions.Any(t => t.EtatSource == etat1 && t.EtatCible == etat4));
            Assert
                .IsTrue(
                    navigateur.Transitions.Any(t => t.EtatSource == etat4 && t.EtatCible == etat5));
            Assert
                .AreEqual(
                    1,
                    navigateur.TransitionsParSymbole.Count());
            Assert
                .IsTrue(
                    navigateur.TransitionsParSymbole.Any(t => t.EtatSource == etat1 && t.EtatCible == etat4));
        }

        [TestMethod]
        public void TransitionsAvecSymboleArbreAvec1Etat()
        {
            // Préparations
            IArbreConstruction arbre = new ArbreConstruction();
            const char symbole = 'a';
            var etat1 = arbre.AjouterEtat();

            var navigateur = (arbre as ArbreConstruction)
                .RecupererNavigateurSur(
                    etat1);

            // Action à vérifier
            navigateur
                .TransitionPar(symbole);

            // Tests

            // Tests sur l'origine
            Assert
                .AreEqual(
                    1,
                    navigateur.EtatsOrigine.Count());

            // Tests sur les états courants
            Assert
                .AreEqual(
                    0,
                    navigateur.EtatsCourants.Count());

            // Tests sur les transitions
            Assert
                .AreEqual(
                    0,
                    navigateur.Transitions.Count());
            Assert
                .AreEqual(
                    0,
                    navigateur.TransitionsParSymbole.Count());
        }

        [TestMethod]
        public void TransitionsAvecSymboleArbreVide()
        {
            // Préparations
            IArbreConstruction arbre = new ArbreConstruction();
            const char symbole = 'a';

            var navigateur = (arbre as ArbreConstruction)
                .RecupererNavigateurSur();

            // Action à vérifier
            navigateur
                .TransitionPar(symbole);

            // Tests

            // Tests sur l'origine
            Assert
                .AreEqual(
                    0,
                    navigateur.EtatsOrigine.Count());

            // Tests sur les états courants
            Assert
                .AreEqual(
                    0,
                    navigateur.EtatsCourants.Count());

            // Tests sur les transitions
            Assert
                .AreEqual(
                    0,
                    navigateur.Transitions.Count());
            Assert
                .AreEqual(
                    0,
                    navigateur.TransitionsParSymbole.Count());
        }

        [TestMethod]
        public void TransitionsAvecSymboleAvecAucunResultat()
        {
            // Préparations
            IArbreConstruction arbre = new ArbreConstruction();
            const char symbole = 'a';
            var etat1 = arbre.AjouterEtat();
            var etat2 = arbre.AjouterEtat();
            var etat3 = arbre.AjouterEtat();

            arbre.AjouterTransition(
                etat1,
                etat2);
            arbre.AjouterTransition(
                etat2,
                etat3);

            var navigateur = (arbre as ArbreConstruction)
                .RecupererNavigateurSur(
                    etat1);

            // Action à vérifier
            navigateur
                .TransitionPar(symbole);

            // Tests

            // Tests sur l'origine
            Assert
                .AreEqual(
                    1,
                    navigateur.EtatsOrigine.Count());
            Assert
                .IsTrue(
                    navigateur.EtatsOrigine.Contains(etat1));

            // Tests sur les états courants
            Assert
                .AreEqual(
                    0,
                    navigateur.EtatsCourants.Count());

            // Tests sur les transitions
            Assert
                .AreEqual(
                    0,
                    navigateur.Transitions.Count());
        }

        [TestMethod]
        public void TransitionsSansSymbole()
        {
            // Préparations
            IArbreConstruction arbre = new ArbreConstruction();
            var etat1 = arbre.AjouterEtat();
            var etat2 = arbre.AjouterEtat();
            var etat3 = arbre.AjouterEtat();
            var etat4 = arbre.AjouterEtat();

            arbre.AjouterTransition(
                etat1,
                etat2);
            arbre.AjouterTransition(
                etat1,
                etat4,
                'a');
            arbre.AjouterTransition(
                etat2,
                etat3);

            var navigateur = (arbre as ArbreConstruction)
                .RecupererNavigateurSur(
                    etat1);

            // Action à vérifier
            navigateur
                .TransitionsSansSymbole();

            // Tests

            // Tests sur l'origine
            Assert
                .AreEqual(
                    1,
                    navigateur.EtatsOrigine.Count());
            Assert
                .IsTrue(
                    navigateur.EtatsOrigine.Contains(etat1));

            // Tests sur les états courants
            Assert
                .AreEqual(
                    3,
                    navigateur.EtatsCourants.Count());
            Assert
                .IsTrue(
                    navigateur.EtatsCourants.Contains(etat1));
            Assert
                .IsTrue(
                    navigateur.EtatsCourants.Contains(etat2));
            Assert
                .IsTrue(
                    navigateur.EtatsCourants.Contains(etat3));

            // Tests sur les transitions
            Assert
                .AreEqual(
                    2,
                    navigateur.Transitions.Count());
            Assert
                .IsTrue(
                    navigateur.Transitions.Any(t => t.EtatSource == etat1 && t.EtatCible == etat2));
            Assert
                .IsTrue(
                    navigateur.Transitions.Any(t => t.EtatSource == etat2 && t.EtatCible == etat3));
        }

        [TestMethod]
        public void TransitionsSansSymboleArbreAvec1Etat()
        {
            // Préparations
            IArbreConstruction arbre = new ArbreConstruction();
            var etat1 = arbre.AjouterEtat();

            var navigateur = (arbre as ArbreConstruction)
                .RecupererNavigateurSur(
                    etat1);

            // Action à vérifier
            navigateur
                .TransitionsSansSymbole();

            // Tests

            // Tests sur l'origine
            Assert
                .AreEqual(
                    1,
                    navigateur.EtatsOrigine.Count());

            // Tests sur les états courants
            Assert
                .AreEqual(
                    1,
                    navigateur.EtatsCourants.Count());

            // Tests sur les transitions
            Assert
                .AreEqual(
                    0,
                    navigateur.Transitions.Count());
            Assert
                .AreEqual(
                    0,
                    navigateur.TransitionsParSymbole.Count());
        }

        [TestMethod]
        public void TransitionsSansSymboleArbreVide()
        {
            // Préparations
            IArbreConstruction arbre = new ArbreConstruction();

            var navigateur = (arbre as ArbreConstruction)
                .RecupererNavigateurSur();

            // Action à vérifier
            navigateur
                .TransitionsSansSymbole();

            // Tests

            // Tests sur l'origine
            Assert
                .AreEqual(
                    0,
                    navigateur.EtatsOrigine.Count());

            // Tests sur les états courants
            Assert
                .AreEqual(
                    0,
                    navigateur.EtatsCourants.Count());

            // Tests sur les transitions
            Assert
                .AreEqual(
                    0,
                    navigateur.Transitions.Count());
            Assert
                .AreEqual(
                    0,
                    navigateur.TransitionsParSymbole.Count());
        }

        #endregion Public Methods
    }
}
