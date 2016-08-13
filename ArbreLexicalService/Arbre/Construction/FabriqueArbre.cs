﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArbreLexicalService.Arbre.Construction.Dto;
using ArbreLexicalService.Arbre.Construction.Elements;
using ArbreLexicalService.Arbre.Dto;
using ArbreLexicalService.Exceptions;
using Common.Exceptions;
using Common.Ioc;

namespace ArbreLexicalService.Arbre.Construction
{
    internal class FabriqueArbre : IFabriqueArbre
    {
        #region Protected Fields

        protected ElementConstructionDto elementsConstruction;

        #endregion Protected Fields

        #region Private Fields

        private readonly IArbreConstruction arbre;

        #endregion Private Fields

        #region Public Constructors

        public FabriqueArbre(
            IArbreConstruction arbre)
        {
            this.arbre = arbre;
        }

        #endregion Public Constructors

        #region Internal Properties

        internal ElementConstructionDto ElementsConstruction
        {
            get
            {
                return elementsConstruction;
            }
        }

        #endregion Internal Properties

        #region Public Methods

        public async Task ConstruireAsync()
        {
            try
            {
                if (null != elementsConstruction)
                {
                    await ConstruireAsync(
                        new Etat[] { arbre.EtatEntree },
                        new Etat[] { arbre.EtatSortie },
                        elementsConstruction);
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

        #endregion Public Methods

        #region Private Methods

        private void BrancherElement(
            IEnumerable<Etat> etatsDebut,
            IEnumerable<Etat> etatsFin,
            Etat etatEntreeElement,
            Etat etatSortieElement)
        {
            try
            {
                if (null != etatsDebut)
                {
                    foreach (var etatDebut in etatsDebut)
                    {
                        arbre
                            .AjouterTransition(
                                etatDebut,
                                etatEntreeElement);
                    }
                }

                if (null != etatsFin)
                {
                    foreach (var etatFin in etatsFin)
                    {
                        arbre
                            .AjouterTransition(
                                etatSortieElement,
                                etatFin);
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

        private Task BrancherElementAsync(
            IEnumerable<Etat> etatsDebut,
            IEnumerable<Etat> etatsFin,
            Etat etatEntreeElement,
            Etat etatSortieElement)
        {
            try
            {
                return Task.Run(() =>
                    BrancherElement(
                        etatsDebut,
                        etatsFin,
                        etatEntreeElement,
                        etatSortieElement));
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

        private async Task ConstruireAsync(
                            IEnumerable<Etat> etatsDebut,
            IEnumerable<Etat> etatsFin,
            ElementConstructionDto elementConstruction)
        {
            try
            {
                switch (elementConstruction.TypeElement)
                {
                    case EnumTypeElement.Chemin:
                        await ConstruireCheminAsync(
                            etatsDebut,
                            etatsFin,
                            elementConstruction as ElementCheminConstructionDto);
                        break;

                    case EnumTypeElement.Repetition:
                        await ConstruireRepetitionAsync(
                            etatsDebut,
                            etatsFin,
                            elementConstruction as ElementRepetitionConstructionDto);
                        break;

                    case EnumTypeElement.Sequence:
                        await ConstruireSequenceAsync(
                            etatsDebut,
                            etatsFin,
                            elementConstruction as SequenceElementsConstructionDto);
                        break;

                    case EnumTypeElement.ChoixMultiple:
                        await ConstruireChoixAsync(
                            etatsDebut,
                            etatsFin,
                            elementConstruction as ChoixElementsConstructionDto);
                        break;

                    default:
                        throw new ExceptionArbreConstruction();
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

        private Transition[] ConstruireChemin(
            Etat etatEntreeChemin,
            string chemin)
        {
            try
            {
                return arbre
                    .AjouterChemin(
                        etatEntreeChemin,
                        chemin);
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

        private async Task ConstruireCheminAsync(
            IEnumerable<Etat> etatsDebut,
            IEnumerable<Etat> etatsFin,
            ElementCheminConstructionDto donnees)
        {
            try
            {
                if (null != donnees &&
                    !string.IsNullOrEmpty(donnees.Chemin))
                {
                    var element = await CreerElementAsync(
                        etatsDebut,
                        etatsFin);

                    var transitions = await ConstruireCheminAsync(
                        element.EtatEntree,
                        donnees.Chemin);

                    var derniereTransition = transitions
                        .LastOrDefault();

                    if (null != derniereTransition)
                    {
                        arbre
                            .AjouterTransition(
                                derniereTransition.EtatCible,
                                element.EtatSortie);
                    }
                    else
                    {
                        arbre
                            .AjouterTransition(
                                element.EtatEntree,
                                element.EtatSortie);
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

        private Task<Transition[]> ConstruireCheminAsync(
            Etat etatEntreeChemin,
            string chemin)
        {
            try
            {
                return Task.Run<Transition[]>(() =>
                    ConstruireChemin(
                        etatEntreeChemin,
                        chemin));
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

        private async Task ConstruireChoixAsync(
                                    IEnumerable<Etat> etatsDebut, 
            IEnumerable<Etat> etatsFin,
            ChoixElementsConstructionDto donnees)
        {
            try
            {
                if ((donnees?.Elements?.Any()).GetValueOrDefault())
                {
                    var tasks = new List<Task>();
                    var elementParent = await CreerElementAsync(
                        etatsDebut,
                        etatsFin);

                    foreach (var donneesElement in donnees.Elements)
                    {
                        var element = await CreerElementAsync(
                            new Etat[] { elementParent.EtatEntree },
                            new Etat[] { elementParent.EtatSortie });

                        var task = ConstruireAsync(
                            new Etat[] { element.EtatEntree },
                            new Etat[] { element.EtatSortie },
                            donneesElement);
                        tasks
                            .Add(task);
                    }

                    Task
                        .WaitAll(
                            tasks.ToArray());
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

        private async Task ConstruireRepetitionAsync(
            IEnumerable<Etat> etatsDebut,
            IEnumerable<Etat> etatsFin,
            ElementRepetitionConstructionDto donnees)
        {
            try
            {
                if (null != donnees?.Element)
                {
                    var nbreMin = donnees.NombreMin;
                    var nbreMax = donnees.NombreMax
                        .GetValueOrDefault(int.MaxValue);

                    if (nbreMin >= 0 &&
                        nbreMax >= 1 &&
                        nbreMin <= nbreMax)
                    {
                        var tasks = new List<Task>();
                        int nbreElements = Math.Max(
                            nbreMin,
                            nbreMax == int.MaxValue ? 1 : nbreMax);

                        var elementParent = await CreerElementAsync(
                            etatsDebut,
                            etatsFin);
                        var etatsDebutOuFin = new Etat[] { elementParent.EtatEntree };
                        var etatsFinElementParent = new Etat[] { elementParent.EtatSortie };

                        if (nbreMin == 0)
                        {
                            arbre
                                .AjouterTransition(
                                    elementParent.EtatEntree,
                                    elementParent.EtatSortie);
                        }

                        for (int i = 0; i < nbreElements; i++)
                        {
                            var element = await CreerElementAsync(
                                etatsDebutOuFin,
                                i + 1 == nbreElements ?
                                    etatsFinElementParent :
                                    null);
                            etatsDebutOuFin = new Etat[] { element.EtatSortie };

                            if (nbreMin < nbreElements &&
                                i + 1 == nbreMin)
                            {
                                arbre
                                    .AjouterTransition(
                                        element.EtatSortie,
                                        elementParent.EtatSortie);
                            }

                            var task = ConstruireAsync(
                                new Etat[] { element.EtatEntree },
                                etatsDebutOuFin,
                                donnees.Element);
                            tasks
                                .Add(task);
                        }

                        Task
                            .WaitAll(
                                tasks.ToArray());
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

        private async Task ConstruireSequenceAsync(
                    IEnumerable<Etat> etatsDebut, 
            IEnumerable<Etat> etatsFin,
            SequenceElementsConstructionDto donnees)
        {
            try
            {
                if ((donnees?.Elements?.Any()).GetValueOrDefault())
                {
                    var dernieresDonneesElement = donnees.Elements
                        .LastOrDefault();
                    var etatsDebutOuFin = etatsDebut;

                    foreach (var donneesElement in donnees.Elements)
                    {
                        var element = await CreerElementAsync(
                            etatsDebutOuFin,
                            donneesElement == dernieresDonneesElement ?
                                etatsFin :
                                null);
                        etatsDebutOuFin = new Etat[] { element.EtatSortie };

                        await ConstruireAsync(
                            new Etat[] { element.EtatEntree },
                            etatsDebutOuFin,
                            donneesElement);
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
        private async Task<IConstructionElementArbre> CreerElementAsync(
            IEnumerable<Etat> etatsDebut, 
            IEnumerable<Etat> etatsFin)
        {
            try
            {
                var element = Fabrique.Instance
                    .RecupererInstance<IConstructionElementArbre, IArbreConstruction>(
                        arbre);
                await element
                    .CreerAsync();

                var etatEntreeChemin = element.EtatEntree;
                var etatSortieChemin = element.EtatSortie;

                await BrancherElementAsync(
                    etatsDebut, 
                    etatsFin, 
                    etatEntreeChemin, 
                    etatSortieChemin);

                return element;
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
    }
}