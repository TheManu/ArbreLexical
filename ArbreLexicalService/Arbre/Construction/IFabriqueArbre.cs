using System.Threading.Tasks;

namespace ArbreLexicalService.Arbre.Construction
{
    public interface IFabriqueArbre
    {
        Task ConstruireAsync();
    }
}