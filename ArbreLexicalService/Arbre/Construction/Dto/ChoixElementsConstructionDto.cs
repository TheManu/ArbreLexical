using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArbreLexicalService.Arbre.Construction.Dto
{
    internal class ChoixElementsConstructionDto : SequenceElementsConstructionDto
    {
        public ChoixElementsConstructionDto(
            IEnumerable<ElementConstructionDto> elements) : base(elements)
        {
        }

        public override EnumTypeElement TypeElement
        {
            get
            {
                return EnumTypeElement.ChoixMultiple;
            }
        }
    }
}
