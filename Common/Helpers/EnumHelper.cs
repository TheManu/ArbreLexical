using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helpers
{
    public static class EnumHelper
    {
        public static T RecupererEnum<T>(
            int valeur,
            T defaut)
        {
            return Enum.IsDefined(typeof(T), valeur) ?
                (T)Enum.ToObject(typeof(T), valeur) :
                defaut;
        }
    }
}
