using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private readonly List<BlockInfos> blocksInfos =
            new List<BlockInfos>();

        #endregion Private Fields

        #region Public Constructors

        public FabriqueArbre(
            IArbreConstruction arbre) : this(arbre, null)
        {
        }

        public FabriqueArbre(
            IArbreConstruction arbre,
            ElementConstructionDto elementsConstruction)
        {
            this.arbre = arbre;
            this.elementsConstruction = elementsConstruction;
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

        internal List<BlockInfos> BlocksInfos
        {
            get
            {
                return blocksInfos;
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

                    if (blocksInfos.Any(b => b.ElementsEnAttente.Any()))
                    {
                        throw new ExceptionArbreConstruction();
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
            ElementConstructionDto elementConstruction,
            Dictionary<string, IConstructionElementArbre> dicoReferences = null)
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
                            elementConstruction as ElementRepetitionConstructionDto,
                            dicoReferences);
                        break;

                    case EnumTypeElement.Sequence:
                        await ConstruireSequenceAsync(
                            etatsDebut,
                            etatsFin,
                            elementConstruction as SequenceElementsConstructionDto,
                            dicoReferences);
                        break;

                    case EnumTypeElement.ChoixMultiple:
                        await ConstruireChoixAsync(
                            etatsDebut,
                            etatsFin,
                            elementConstruction as ChoixElementsConstructionDto,
                            dicoReferences);
                        break;

                    case EnumTypeElement.Etiquette:
                        await ConstruireEtiquetteAsync(
                            etatsDebut,
                            etatsFin,
                            elementConstruction as ElementEtiquetteConstructionDto,
                            dicoReferences);
                        break;

                    case EnumTypeElement.Reference:
                        await ConstruireReferenceAsync(
                            etatsDebut,
                            etatsFin,
                            elementConstruction as ElementReferenceConstructionDto,
                            dicoReferences);
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
            ChoixElementsConstructionDto donnees,
            Dictionary<string, IConstructionElementArbre> dicoReferences = null)
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
                        if (donneesElement.TypeElement != EnumTypeElement.Etiquette)
                        {
                            var element = await CreerElementAsync(
                                new Etat[] { elementParent.EtatEntree },
                                new Etat[] { elementParent.EtatSortie });

                            var task = ConstruireAsync(
                                new Etat[] { element.EtatEntree },
                                new Etat[] { element.EtatSortie },
                                donneesElement,
                                dicoReferences);
                            tasks
                                .Add(task);
                        }
                        else
                        {
                            await ConstruireEtiquetteAsync(
                                null,
                                null,
                                donneesElement as ElementEtiquetteConstructionDto);
                        }
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

        private async Task ConstruireEtiquetteAsync(
            IEnumerable<Etat> etatsDebut,
            IEnumerable<Etat> etatsFin,
            ElementEtiquetteConstructionDto donnees,
            Dictionary<string, IConstructionElementArbre> dicoReferences = null)
        {//todo injecter les étiquette (en début ou fin) dans les états et interdire (après mise en place) les ajouts de transitions sortantes (si début) ou entrantes (si fin)
            try
            {
                if (null != donnees?.Element)
                {
                    if (donnees.TypeBlock != EnumTypeBlock.Autre &&
                        !string.IsNullOrWhiteSpace(donnees.Id))
                    {
                        // Enregistrement du block pour permettre les références dessus
                        lock (blocksInfos)
                        {
                            var blockInfosEnregistre = blocksInfos
                                .FirstOrDefault(b => b.Id == donnees.Id);

                            if (null != blockInfosEnregistre)
                            {
                                if (null != blockInfosEnregistre.Donnees)
                                {
                                    throw new ExceptionArbreConstruction();
                                }
                                else
                                { // Le block a été enregistré par une référence en attente => on garnit la/les référence(s) en attente

                                    blockInfosEnregistre.Donnees = donnees.Element;
                                    blockInfosEnregistre.TypeBlock = donnees.TypeBlock;
                                    var elementsEnAttente = blockInfosEnregistre.ElementsEnAttente;

                                    if (elementsEnAttente.Any())
                                    {
                                        var tasks = new List<Task>();

                                        foreach (var elementEnAttente in elementsEnAttente)
                                        {
                                            var task = ConstruireAsync(
                                                new Etat[] { elementEnAttente.EtatEntree },
                                                new Etat[] { elementEnAttente.EtatSortie },
                                                donnees.Element,
                                                dicoReferences);
                                            tasks
                                                .Add(task);
                                        }

                                        Task
                                            .WaitAll(
                                                tasks.ToArray());

                                        foreach (var elementEnAttente in elementsEnAttente)
                                        {
                                            arbre
                                                .Etiquetter(
                                                    donnees.Id,
                                                    donnees.TypeBlock,
                                                    elementEnAttente.EtatEntree,
                                                    elementEnAttente.EtatSortie);
                                        }
                                    }
                                }
                            }
                            else
                            { // Le block n'existe pas encore => ajout

                                var infos = new BlockInfos(
                                    donnees.Id,
                                    donnees.TypeBlock,
                                    donnees.Element);

                                blocksInfos
                                    .Add(infos);
                            }
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

        private async Task ConstruireReferenceAsync(
            IEnumerable<Etat> etatsDebut, 
            IEnumerable<Etat> etatsFin,
            ElementReferenceConstructionDto donnees,
            Dictionary<string, IConstructionElementArbre> dicoReferences = null)
        {
            try
            {
                if (null != donnees?.Id &&
                    !string.IsNullOrWhiteSpace(donnees.Id))
                {
                    dicoReferences = dicoReferences ?? new Dictionary<string, IConstructionElementArbre>();
                    var elementParent = await CreerElementAsync(
                        etatsDebut,
                        etatsFin);

                    IConstructionElementArbre elementRefCirculaire;
                    if (dicoReferences.TryGetValue(donnees.Id, out elementRefCirculaire))
                    { // C'est une référence circulaire => ajout d'une transition vers le début de la construction de la référence

                        arbre
                            .AjouterTransition(
                                elementParent.EtatEntree,
                                elementRefCirculaire.EtatEntree);
                        arbre
                            .AjouterTransition(
                                elementRefCirculaire.EtatSortie,
                                elementParent.EtatSortie);

                        arbre
                            .Etiquetter(
                                $"RefCirculaire: {donnees.Id}-{elementParent.EtatEntree}/{elementParent.EtatSortie}",
                                EnumTypeBlock.Autre,
                                elementParent.EtatEntree,
                                elementParent.EtatSortie);
                    }
                    else
                    { // Pas de référence circulaire => construction de la référence (reportée si l'étiquette n'est pas encore définie)

                        BlockInfos blockInfosEnregistre = null;
                        lock (blocksInfos)
                        {
                            blockInfosEnregistre = blocksInfos
                                .FirstOrDefault(b => b.Id == donnees.Id);

                            if (null == blockInfosEnregistre)
                            {
                                blockInfosEnregistre = new BlockInfos(
                                    donnees.Id);

                                blocksInfos
                                    .Add(
                                        blockInfosEnregistre);
                            }

                            if (null == blockInfosEnregistre.Donnees)
                            { // Le block n'existe pas encore => ajout pour référencer cette cible

                                blockInfosEnregistre
                                    .AjouterElementEnAttente(
                                        elementParent);
                            }
                        }

                        if (null != blockInfosEnregistre?.Donnees)
                        { // Création de la référence

                            var elementSource = blockInfosEnregistre.Donnees;
                            var dicoReferencesPourEnfants = new Dictionary<string, IConstructionElementArbre>(
                                dicoReferences);
                            dicoReferencesPourEnfants
                                .Add(
                                    donnees.Id,
                                    elementParent);

                            await ConstruireAsync(
                                new Etat[] { elementParent.EtatEntree },
                                new Etat[] { elementParent.EtatSortie },
                                elementSource,
                                dicoReferencesPourEnfants);

                            arbre
                                .Etiquetter(
                                    donnees.Id,
                                    blockInfosEnregistre.TypeBlock,
                                    elementParent.EtatEntree,
                                    elementParent.EtatSortie);
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

        private async Task ConstruireRepetitionAsync(
            IEnumerable<Etat> etatsDebut,
            IEnumerable<Etat> etatsFin,
            ElementRepetitionConstructionDto donnees,
            Dictionary<string, IConstructionElementArbre> dicoReferences = null)
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

                            if (i == 0 &&
                                nbreMax == int.MaxValue)
                            { // Max = infini => boucle sur le 1er élément

                                arbre
                                    .AjouterTransition(
                                        element.EtatSortie,
                                        element.EtatEntree);
                            }

                            if (nbreMin < nbreElements &&
                                i + 1 == nbreMin)
                            { // min < max (< infini) => sortie possible

                                arbre
                                    .AjouterTransition(
                                        element.EtatSortie,
                                        elementParent.EtatSortie);
                            }

                            var task = ConstruireAsync(
                                new Etat[] { element.EtatEntree },
                                etatsDebutOuFin,
                                donnees.Element,
                                dicoReferences);
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
            SequenceElementsConstructionDto donnees,
            Dictionary<string, IConstructionElementArbre> dicoReferences = null)
        {
            try
            {
                if ((donnees?.Elements?.Any()).GetValueOrDefault())
                {
                    var etatsDebutOuFin = etatsDebut;

                    for (int i = 0, nbre = donnees.Elements.Count(); i < nbre; i++)
                    {
                        var donneesElement = donnees.Elements[i];
                        var element = await CreerElementAsync(
                            etatsDebutOuFin,
                            i + 1 == nbre ?
                                etatsFin :
                                null);
                        etatsDebutOuFin = new Etat[] { element.EtatSortie };

                        await ConstruireAsync(
                            new Etat[] { element.EtatEntree },
                            etatsDebutOuFin,
                            donneesElement,
                            dicoReferences);
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

        #region Private Classes

        internal class BlockInfos
        {
            #region Private Fields

            private readonly List<IConstructionElementArbre> elementsEnAttente =
                new List<IConstructionElementArbre>();

            private readonly string id;
            private ElementConstructionDto donnees;
            private EnumTypeBlock typeBlock;

            #endregion Private Fields

            #region Public Constructors

            public BlockInfos(
                string id, 
                EnumTypeBlock typeBlock,
                ElementConstructionDto donnees)
            {
                this.id = id;
                this.typeBlock = typeBlock;
                this.donnees = donnees;
            }

            public BlockInfos(
                string id) : this(id, EnumTypeBlock.Autre, null)
            {
                this.id = id;
            }

            #endregion Public Constructors

            #region Public Properties

            public IConstructionElementArbre[] ElementsEnAttente
            {
                get
                {
                    return elementsEnAttente
                        .ToArray();
                }
            }

            public string Id
            {
                get
                {
                    return id;
                }
            }

            #endregion Public Properties

            #region Internal Properties

            internal ElementConstructionDto Donnees
            {
                get
                {
                    return donnees;
                }
                set
                {
                    donnees = value;
                }
            }

            public EnumTypeBlock TypeBlock
            {
                get
                {
                    return typeBlock;
                }
                set
                {
                    typeBlock = value;
                }
            }

            #endregion Internal Properties

            #region Internal Methods

            internal void AjouterElementEnAttente(
                IConstructionElementArbre elementEnAttente)
            {
                elementsEnAttente
                    .Add(
                        elementEnAttente);
            }

            #endregion Internal Methods
        }

        #endregion Private Classes
    }
}
