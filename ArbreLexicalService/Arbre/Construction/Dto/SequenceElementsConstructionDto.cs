using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArbreLexicalService.Arbre.Construction.Dto
{
    internal class SequenceElementsConstructionDto : ElementConstructionDto
    {
        private readonly ElementConstructionDto[] elements;

        public SequenceElementsConstructionDto(
            IEnumerable<ElementConstructionDto> elements)
        {
            this.elements = elements
                .ToArray();
        }

        internal ElementConstructionDto[] Elements
        {
            get
            {
                return elements;
            }
        }

        public override EnumTypeElement TypeElement
        {
            get
            {
                return EnumTypeElement.Sequence;
            }
        }
    }
}
