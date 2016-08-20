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
    internal class ConstructionMultiChoixSurArbre : ConstructionElementArbre
    {
        #region Public Constructors

        public ConstructionMultiChoixSurArbre(
            IArbreConstruction arbre) : base(arbre)
        {
        }

        #endregion Public Constructors

        protected Task<IConstructionElementArbre> AjouterBrancheAsync()
        {
            try
            {
                var task = Task.Run<IConstructionElementArbre>(
                    (Func<IConstructionElementArbre>) AjouterBranche);

                return task;
            }
            catch (Exception ex)
            {
                throw EncapsulerEtGererException<ExceptionTechnique>(
                    ex);
            }
        }

        private IConstructionElementArbre AjouterBranche()
        {
            try
            {
                var element = arbre
                    .CreerElement(
                        new Etat[] { EtatEntree },
                        new Etat[] { EtatSortie });

                return element;
            }
            catch (Exception ex)
            {
                throw EncapsulerEtGererException<ExceptionTechnique>(
                    ex);
            }
        }
    }
}
