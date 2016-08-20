using AnalyseSyntaxiqueService.Analyse.Dto;

namespace AnalyseSyntaxiqueService.Analyse
{
    public interface IAnalyseurSyntaxique
    {
        void Ajouter(
            EntreeAnalyseurSyntaxiqueDto entree);
    }
}