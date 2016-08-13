using ArbreLexicalService.Arbre.Construction.Dto;

namespace ArbreLexicalService.Arbre.Construction
{
    internal interface IDeserialisateurElement
    {
        ElementConstructionDto Deserialiser();
    }
}