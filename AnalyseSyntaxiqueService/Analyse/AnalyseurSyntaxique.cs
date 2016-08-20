using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnalyseSyntaxiqueService.Analyse.Dto;
using ArbreLexicalService.Arbre.Construction;

namespace AnalyseSyntaxiqueService.Analyse
{
    internal class AnalyseurSyntaxique : IAnalyseurSyntaxique
    {
        private readonly IArbreConstruction arbre;

        public AnalyseurSyntaxique(IArbreConstruction arbre)
        {
            this.arbre = arbre;
        }

        public void Ajouter(
            EntreeAnalyseurSyntaxiqueDto entree)
        {

        }
    }
}
