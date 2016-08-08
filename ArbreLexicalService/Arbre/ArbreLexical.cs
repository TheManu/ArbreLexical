using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArbreLexicalService.Arbre.Dto;

namespace ArbreLexicalService.Arbre
{
    internal class ArbreLexical : IArbreLexical
    {
        private readonly Etat[] etats;

        public ArbreLexical(
            IEnumerable<Etat> etats)
        {
            this.etats = etats.ToArray();
        }

        public Etat[] Etats
        {
            get
            {
                return etats;
            }
        }
    }
}
