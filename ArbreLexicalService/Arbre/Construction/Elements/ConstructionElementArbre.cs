using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArbreLexicalService.Arbre.Dto;
using ArbreLexicalService.Exceptions;
using Common.Exceptions;
using Common.Ioc;

namespace ArbreLexicalService.Arbre.Construction.Elements
{
    internal class ConstructionElementArbre : IConstructionElementArbre
    {

        #region Protected Fields

        protected readonly IArbreConstruction arbre;

        #endregion Protected Fields

        #region Public Constructors

        public ConstructionElementArbre(
            IArbreConstruction arbre)
        {
            this.arbre = arbre;
        }

        #endregion Public Constructors

        #region Public Properties

        public AggregateException Erreur
        {
            get;
            private set;
        }

        public bool EstTraitementTermine
        {
            get;
            private set;
        }

        public Etat EtatEntree
        {
            get;
            protected set;
        }

        public Etat EtatSortie
        {
            get;
            protected set;
        }

        #endregion Public Properties

        #region Public Methods

        public Task CreerAsync()
        {
            try
            {
                var task = Task.Run(
                    (Action)Creer);
                task
                    .ContinueWith(
                        TraitementTermine);

                return task;
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

        public Task RelierADebutAsync(
            IEnumerable<Etat> etatsARelier)
        {
            try
            {
                var task = Task.Run(() =>
                    RelierADebut(etatsARelier));

                return task;
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

        public Task RelierAFinAsync(
            IEnumerable<Etat> etatsARelier)
        {
            try
            {
                var task = Task.Run(() =>
                    RelierAFin(etatsARelier));

                return task;
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

        public virtual void RelierADebut(
            IEnumerable<Etat> etatsARelier)
        {
            try
            {
                foreach (var etat in etatsARelier)
                {
                    arbre
                        .AjouterTransition(
                            EtatEntree,
                            etat);
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

        public virtual void RelierAFin(
            IEnumerable<Etat> etatsARelier)
        {
            try
            {
                foreach (var etat in etatsARelier)
                {
                    arbre
                        .AjouterTransition(
                            etat,
                            EtatSortie);
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

        #region Protected Methods

        protected virtual void Creer()
        {
            try
            {
                if (null != EtatEntree)
                {
                    throw new ExceptionArbreConstruction();
                }

                EtatEntree = arbre
                    .AjouterEtat();
                EtatSortie = arbre
                    .AjouterEtat();
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

        #region Private Methods

        private void TraitementTermine(
            Task taskParent)
        {
            try
            {
                EstTraitementTermine = true;
                Erreur = taskParent.Exception;
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
