using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArbreLexicalService.Arbre.Construction
{
    public interface IFabriqueArbreDepuisJson : IFabriqueArbre
    {
        Task ChargerFichierAsync();

        Task DeserialiserAsync(
            string jsonStr);
    }
}
