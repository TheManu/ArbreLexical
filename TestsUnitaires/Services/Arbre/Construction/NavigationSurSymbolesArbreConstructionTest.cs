using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ArbreLexicalService.Arbre.Construction;
using System.Linq;

namespace TestsUnitaires.Services.Arbre.Construction
{
    /// <summary>
    /// Description résumée pour NavigationSurSymbolesArbreConstructionTest
    /// </summary>
    [TestClass]
    public class NavigationSurSymbolesArbreConstructionTest
    {
        public NavigationSurSymbolesArbreConstructionTest()
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
        public void TransitionsAvecSymboleX2()
        {
            // Préparations
            IArbreConstruction arbre = new ArbreConstruction();
            const char symbole1 = 'a';
            const char symbole2 = 'b';
            var etat1 = arbre.AjouterEtat();
            var etat2 = arbre.AjouterEtat();
            var etat3 = arbre.AjouterEtat();
            var etat4 = arbre.AjouterEtat();
            var etat5 = arbre.AjouterEtat();
            var etat6 = arbre.AjouterEtat();
            var etat7 = arbre.AjouterEtat();

            arbre.AjouterTransition(
                etat1,
                etat2);
            arbre.AjouterTransition(
                etat1,
                etat4,
                symbole1);
            arbre.AjouterTransition(
                etat2,
                etat3);
            arbre.AjouterTransition(
                etat4,
                etat5);
            arbre.AjouterTransition(
                etat5,
                etat6,
                symbole2);
            arbre.AjouterTransition(
                etat6,
                etat7);

            var navigateur = (arbre as ArbreConstruction)
                .RecupererNavigateurMultiSymbolesSur(
                    etat1);

            // Action à vérifier
            navigateur
                .TransitionPar(symbole1);
            navigateur
                .TransitionPar(symbole2);

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
                    navigateur.EtatsCourants.Contains(etat6));
            Assert
                .IsTrue(
                    navigateur.EtatsCourants.Contains(etat7));

            // Tests sur les transitions
            Assert
                .AreEqual(
                    4,
                    navigateur.Transitions.Count());
            Assert
                .IsTrue(
                    navigateur.Transitions.Any(t => t.EtatSource == etat1 && t.EtatCible == etat4));
            Assert
                .IsTrue(
                    navigateur.Transitions.Any(t => t.EtatSource == etat4 && t.EtatCible == etat5));
            Assert
                .IsTrue(
                    navigateur.Transitions.Any(t => t.EtatSource == etat5 && t.EtatCible == etat6));
            Assert
                .IsTrue(
                    navigateur.Transitions.Any(t => t.EtatSource == etat6 && t.EtatCible == etat7));
        }

        [TestMethod]
        public void TransitionsAvecSymboleX2AvecRecursion()
        {
            // Préparations
            IArbreConstruction arbre = new ArbreConstruction();
            const char symbole1 = 'a';
            const char symbole2 = 'b';
            var etat1 = arbre.AjouterEtat();
            var etat2 = arbre.AjouterEtat();
            var etat3 = arbre.AjouterEtat();
            var etat4 = arbre.AjouterEtat();
            var etat5 = arbre.AjouterEtat();
            var etat6 = arbre.AjouterEtat();
            var etat7 = arbre.AjouterEtat();

            arbre.AjouterTransition(
                etat1,
                etat2);
            arbre.AjouterTransition(
                etat1,
                etat1);
            arbre.AjouterTransition(
                etat1,
                etat4,
                symbole1);
            arbre.AjouterTransition(
                etat4,
                etat4);
            arbre.AjouterTransition(
                etat4,
                etat1);
            arbre.AjouterTransition(
                etat2,
                etat3);
            arbre.AjouterTransition(
                etat4,
                etat5);
            arbre.AjouterTransition(
                etat5,
                etat6,
                symbole2);
            arbre.AjouterTransition(
                etat6,
                etat7);

            var navigateur = (arbre as ArbreConstruction)
                .RecupererNavigateurMultiSymbolesSur(
                    etat1);

            // Action à vérifier
            navigateur
                .TransitionPar(symbole1);
            navigateur
                .TransitionPar(symbole2);

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
                    navigateur.EtatsCourants.Contains(etat6));
            Assert
                .IsTrue(
                    navigateur.EtatsCourants.Contains(etat7));

            // Tests sur les transitions
            Assert
                .AreEqual(
                    5,
                    navigateur.TransitionsParSymboles.Count());
            Assert
                .IsTrue(
                    navigateur.TransitionsParSymboles.Any(t => t.EtatSource == etat1 && t.EtatCible == etat1));
            Assert
                .IsTrue(
                    navigateur.TransitionsParSymboles.Any(t => t.EtatSource == etat1 && t.EtatCible == etat4 && t.Symbole == symbole1));
            Assert
                .IsTrue(
                    navigateur.TransitionsParSymboles.Any(t => t.EtatSource == etat4 && t.EtatCible == etat4));
            Assert
                .IsTrue(
                    navigateur.TransitionsParSymboles.Any(t => t.EtatSource == etat4 && t.EtatCible == etat5));
            Assert
                .IsTrue(
                    navigateur.TransitionsParSymboles.Any(t => t.EtatSource == etat5 && t.EtatCible == etat6 && t.Symbole == symbole2));
            Assert
                .AreEqual(
                    6,
                    navigateur.Transitions.Count());
            Assert
                .AreEqual(
                    1,
                    navigateur.Transitions.Except(navigateur.TransitionsParSymboles).Count());
            Assert
                .IsTrue(
                    navigateur.Transitions.Any(t => t.EtatSource == etat6 && t.EtatCible == etat7));
        }
    }
}
