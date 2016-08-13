using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ArbreLexicalService.Arbre.Construction;
using System.Threading.Tasks;

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
        public async Task Construire()
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
        }
    }
}
