using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArbreLexicalService.Arbre.Construction.Dto
{
    internal class ElementEtiquetteConstructionDto : ElementConstructionDto
    {
        private readonly ElementConstructionDto element;

        private readonly string id;

        private readonly EnumTypeBlock typeBloc;

        public ElementEtiquetteConstructionDto(
            EnumTypeBlock typeBloc, 
            string id,
            ElementConstructionDto element)
        {
            this.typeBloc = typeBloc;
            this.id = id;
            this.element = element;
        }

        public string Id
        {
            get
            {
                return id;
            }
        }

        public override EnumTypeElement TypeElement
        {
            get
            {
                return EnumTypeElement.Etiquette;
            }
        }

        internal ElementConstructionDto Element
        {
            get
            {
                return element;
            }
        }

        internal EnumTypeBlock TypeBloc
        {
            get
            {
                return typeBloc;
            }
        }
    }
}
