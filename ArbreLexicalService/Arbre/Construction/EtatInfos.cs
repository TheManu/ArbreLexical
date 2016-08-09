using ArbreLexicalService.Arbre.Dto;
using Common.Ioc;
using Common.Locks;

namespace ArbreLexicalService.Arbre.Construction
{
    internal class EtatInfos
    {
        #region Private Fields

        private readonly Etat etatOrigine;

        private readonly IEtatTransitionsSortantes transitionsSortantes;

        #endregion Private Fields

        #region Public Constructors

        public EtatInfos(
            Etat etatOrigine)
        {
            this.etatOrigine = etatOrigine;

            transitionsSortantes = Fabrique.Instance
                .RecupererInstance<IEtatTransitionsSortantes, Etat>(
                    etatOrigine);
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