using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArbreLexicalService.Arbre.Construction.Dto
{
    internal abstract class ElementConstructionDto
    {
        public abstract EnumTypeElement TypeElement
        {
            get;
        }
    }
}
