using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Exceptions;
using Common.Ioc;
using Common.Services;

namespace ArbreLexicalService.Arbre.Dto
{
    internal class EtatTransitionsSortantes : ServiceBase, IEtatTransitionsSortantes
    {
        #region Private Fields

        private readonly Etat etat;

        private readonly List<Transition> transitions =
            new List<Transition>();

        #endregion Private Fields

        #region Public Constructors

        public EtatTransitionsSortantes(
            Etat etat)
        {
            this.etat = etat;
        }

        #endregion Public Constructors

        #region Public Properties

        public Transition[] Transitions
        {
            get
            {
                return transitions
                    .ToArray();
            }
        }

        #endregion Public Properties

        #region Public Methods

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
                throw EncapsulerEtGererException<ExceptionTechnique>(
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
                throw EncapsulerEtGererException<ExceptionTechnique>(
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
                throw EncapsulerEtGererException<ExceptionTechnique>(
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
                throw EncapsulerEtGererException<ExceptionTechnique>(
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
                        ExceptionBase.RecupererLibelleMessage(
                            $"Impossible de supprimer la transition {transition}"));
                }

                return this;
            }
            catch (Exception ex)
            {
                throw EncapsulerEtGererException<ExceptionTechnique>(
                    ex);
            }
        }

        #endregion Public Methods
    }
}
