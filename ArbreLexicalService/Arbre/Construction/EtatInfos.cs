using System;
using ArbreLexicalService.Arbre.Dto;
using ArbreLexicalService.Exceptions;
using Common.Exceptions;
using Common.Ioc;
using Common.Locks;
using Common.Services;

namespace ArbreLexicalService.Arbre.Construction
{
    internal class EtatInfos : ServiceBase
    {
        #region Private Fields

        private readonly Etat etatOrigine;

        private readonly IEtatTransitionsSortantes transitionsSortantes;

        #endregion Private Fields

        public bool AjoutTransitionEntrantePossible
        {
            get;
            private set;
        }

        public bool AjoutTransitionSortantePossible
        {
            get;
            private set;
        }

        internal void InterdireTransitionEntrante()
        {
            try
            {
                AjoutTransitionEntrantePossible = false;
            }
            catch (Exception ex)
            {
                throw EncapsulerEtGererException<ExceptionTechnique>(
                    ex);
            }
        }

        internal void InterdireTransitionSortante()
        {
            try
            {
                AjoutTransitionSortantePossible = false;
            }
            catch (Exception ex)
            {
                throw EncapsulerEtGererException<ExceptionTechnique>(
                    ex);
            }
        }

        #region Public Constructors

        public EtatInfos(
            Etat etatOrigine)
        {
            this.etatOrigine = etatOrigine;

            transitionsSortantes = Fabrique.Instance
                .RecupererInstance<IEtatTransitionsSortantes, Etat>(
                    etatOrigine);

            AjoutTransitionEntrantePossible = true;
            AjoutTransitionSortantePossible = true;
        }

        #endregion Public Constructors

        #region Public Properties

        public bool EstActif { get; internal set; }

        public Etat EtatOrigine
        {
            get
            {
                return etatOrigine;
            }
        }

        public ILockLectureEtEcriture Lockeur { get; private set; } =
                            Fabrique.Instance.RecupererInstance<ILockLectureEtEcriture>();

        #endregion Public Properties

        #region Internal Properties

        internal IEtatTransitionsSortantes TransitionsSortantes
        {
            get
            {
                return transitionsSortantes;
            }
        }

        #endregion Internal Properties
    }
}