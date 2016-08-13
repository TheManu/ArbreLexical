using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArbreLexicalService.Arbre.Construction.Dto
{
    internal class ElementCheminConstructionDto : ElementConstructionDto
    {
        private readonly string chemin;

        public ElementCheminConstructionDto(
            string chemin)
        {
            this.chemin = chemin;
        }

        public string Chemin
        {
            get
            {
                return chemin;
            }
        }

        public override EnumTypeElement TypeElement
        {
            get
            {
                return EnumTypeElement.Chemin;
            }
        }
    }
}
