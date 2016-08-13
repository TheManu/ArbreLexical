using System.Collections.Generic;
using System.Threading.Tasks;
using ArbreLexicalService.Arbre.Dto;

namespace ArbreLexicalService.Arbre.Construction.Elements
{
    public interface IConstructionElementArbre
    {
        Task CreerAsync();

        Etat EtatEntree
        {
            get;
        }

        Etat EtatSortie
        {
            get;
        }

        Task RelierADebutAsync(
            IEnumerable<Etat> etatsARelier);

        Task RelierAFinAsync(
            IEnumerable<Etat> etatsARelier);
    }
}