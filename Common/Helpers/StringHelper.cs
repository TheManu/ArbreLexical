using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helpers
{
    public static class StringHelper
    {
        public static bool EstNonNullEtNonVideEtNonEspaces(
            string texte)
        {
            return (!string
                .IsNullOrWhiteSpace(texte));
        }

        public static bool EstNonNullEtNonVide(
            string texte)
        {
            return (!string
                .IsNullOrEmpty(texte));
        }

        public static string SiNullOuVideOuEspacesAlorsDefaut(
            string texte,
            string defaut)
        {
            return SiNullOuVideOuEspacesAlorsSuivant(
                texte,
                defaut) ?? defaut;
        }

        public static string SiNullOuVideAlorsDefaut(
            string texte,
            string defaut)
        {
            return SiNullOuVideAlorsSuivant(
                texte,
                defaut) ?? defaut;
        }

        public static string SiNullOuVideOuEspacesAlorsSuivant(
            params string[] textes)
        {
            return textes
                ?.FirstOrDefault(t =>
                    !string.IsNullOrWhiteSpace(t));
        }

        public static string SiNullOuVideAlorsSuivant(
            params string[] textes)
        {
            return textes
                ?.FirstOrDefault(t =>
                    !string.IsNullOrEmpty(t));
        }
    }
}
