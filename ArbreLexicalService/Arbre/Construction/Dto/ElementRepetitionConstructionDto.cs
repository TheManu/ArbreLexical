using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArbreLexicalService.Arbre.Construction.Dto
{
    [JsonObject]
    internal class ElementRepetitionConstructionDto : ElementConstructionDto
    {
        public ElementRepetitionConstructionDto()
        {
        }

        public ElementRepetitionConstructionDto(
            ElementConstructionDto enfant, 
            int min, 
            int max)
        {
            Element = enfant;
            NombreMin = min;
            NombreMax = max;
        }

        [JsonProperty(PropertyName = "min")]
        public int NombreMin
        {
            get;
            set;
        } = 0;

        [JsonProperty(PropertyName = "max")]
        public int? NombreMax
        {
            get;
            set;
        } = 0;

        public override EnumTypeElement TypeElement
        {
            get
            {
                return EnumTypeElement.Repetition;
            }
        }

        [JsonProperty(PropertyName = "item")]
        public ElementConstructionDto Element { get; set; }
    }
}
