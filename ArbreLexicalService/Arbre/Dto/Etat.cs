using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Exceptions;
using Common.Ioc;

namespace ArbreLexicalService.Arbre.Dto
{
    public class Etat
    {
        #region Private Fields

        private static int identifiantMax = 0;

        private readonly int identifiant;

        #endregion Private Fields

        #region Public Constructors

        public Etat()
        {
            identifiant = Interlocked
                .Increment(
                    ref identifiantMax);
        }

        #endregion Public Constructors

        #region Public Properties

        public int Identifiant
        {
            get
            {
                return identifiant;
            }
        }

        public Transition[] TransitionsSortantes
        {
            get;
            internal set;
        }

        #endregion Public Properties

        #region Public Methods

        public override string ToString()
        {
            try
            {
                return $"Etat {identifiant}";
            }
            catch (Exception ex)
            {
                Fabrique.Instance
                    ?.RecupererGestionnaireTraces()
                    ?.PublierException(
                        ex);
            }

            return base.ToString();
        }

        #endregion Public Methods
    }
}
