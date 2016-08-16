using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArbreLexicalService.Arbre.Construction.Dto
{
    internal class ElementReferenceConstructionDto : ElementConstructionDto
    {
        private readonly string id;

        public ElementReferenceConstructionDto(
            string id)
        {
            this.id = id;
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
                return EnumTypeElement.Reference;
            }
        }
    }
}
