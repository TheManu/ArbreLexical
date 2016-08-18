using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArbreLexicalService.Arbre.Construction;

namespace ArbreLexicalService.Arbre.Dto
{
    public class EtiquetteDto
    {
        private readonly EnumExtremiteEtiquette extremite;
        private readonly string id;
        private readonly EnumTypeBlock typeBlock;

        public EtiquetteDto(
            string id, 
            EnumTypeBlock typeBlock, 
            EnumExtremiteEtiquette extremite)
        {
            this.id = id;
            this.typeBlock = typeBlock;
            this.extremite = extremite;
        }

        public EnumExtremiteEtiquette Extremite
        {
            get
            {
                return extremite;
            }
        }

        public string Id
        {
            get
            {
                return id;
            }
        }

        public EnumTypeBlock TypeBlock
        {
            get
            {
                return typeBlock;
            }
        }
    }
}
