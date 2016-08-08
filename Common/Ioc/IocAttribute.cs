namespace Common.Ioc
{
    public class IocAttribute : System.Attribute
    {
        #region Private Fields

        private readonly int niveauCouche;

        private readonly int prioriteDansCouche;

        #endregion Private Fields

        #region Public Constructors

        public IocAttribute(
            int niveauCouche,
            int prioriteDansCouche)
        {
            this.niveauCouche = niveauCouche;
            this.prioriteDansCouche = prioriteDansCouche;
       }

        public IocAttribute(
            int niveauCouche) : this(niveauCouche, 0)
        {
        }

        #endregion Public Constructors

        #region Public Properties

        public int NiveauCouche
        {
            get
            {
                return niveauCouche;
            }
        }

        public int PrioriteDansCouche
        {
            get
            {
                return prioriteDansCouche;
            }
        }

        #endregion Public Properties
    }
}