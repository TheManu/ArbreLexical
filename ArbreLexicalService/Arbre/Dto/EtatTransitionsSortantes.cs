using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Exceptions;
using Common.Ioc;

namespace ArbreLexicalService.Arbre.Dto
{
    internal class EtatTransitionsSortantes : IEtatTransitionsSortantes
    {
        private readonly Etat etat;

        private readonly List<Transition> transitions =
            new List<Transition>();

        public Transition[] Transitions
        {
            get
            {
                return transitions
                    .ToArray();
            }
        }

        public EtatTransitionsSortantes(
            Etat etat)
        {
            this.etat = etat;
        }

        public IEtatTransitionsSortantes Ajouter(
            Transition transition)
        {
            try
            {
                if (!transitions.Contains(transition))
                {
                    transitions.Add(
                        transition);
                }

                return this;
            }
            catch (Exception ex)
            {
                Fabrique.Instance
                    ?.RecupererGestionnaireTraces()
                    ?.PublierException(
                        ex);

                throw new ExceptionTechnique(
                    ExceptionBase.RecupererLibelleErreur(),
                    ex);
            }
        }

        public IEtatTransitionsSortantes Supprimer(
            Transition transition)
        {
            try
            {
                if (transitions.Contains(transition))
                {
                    transitions.Remove(
                        transition);
                }
                else
                {
                    throw new ExceptionTechnique(
                        ExceptionBase.RecupererLibelleErreur(
                            $"Impossible de supprimer la transition {transition}"));
                }

                return this;
            }
            catch (Exception ex)
            {
                Fabrique.Instance
                    ?.RecupererGestionnaireTraces()
                    ?.PublierException(
                        ex);

                throw new ExceptionTechnique(
                    ExceptionBase.RecupererLibelleErreur(),
                    ex);
            }
        }

        public Transition RecupererTransitionEquivalente(
            Transition transition)
        {
            try
            {
                return transitions
                    .FirstOrDefault(t =>
                        t.EstEquivalentA(transition));
            }
            catch (Exception ex)
            {
                Fabrique.Instance
                    ?.RecupererGestionnaireTraces()
                    ?.PublierException(
                        ex);

                throw new ExceptionTechnique(
                    ExceptionBase.RecupererLibelleErreur(),
                    ex);
            }
        }

        public Transition RecupererTransitionAvecSymbole(
            char symbole)
        {
            try
            {
                return transitions
                    .FirstOrDefault(t =>
                        t.Symbole.HasValue &&
                        t.Symbole.Value == symbole);
            }
            catch (Exception ex)
            {
                Fabrique.Instance
                    ?.RecupererGestionnaireTraces()
                    ?.PublierException(
                        ex);

                throw new ExceptionTechnique(
                    ExceptionBase.RecupererLibelleErreur(),
                    ex);
            }
        }

        public Transition[] RecupererTransitionsSansSymbole()
        {
            try
            {
                return transitions
                    .Where(t =>
                        !t.Symbole.HasValue)
                    .ToArray();
            }
            catch (Exception ex)
            {
                Fabrique.Instance
                    ?.RecupererGestionnaireTraces()
                    ?.PublierException(
                        ex);

                throw new ExceptionTechnique(
                    ExceptionBase.RecupererLibelleErreur(),
                    ex);
            }
        }
    }
}
