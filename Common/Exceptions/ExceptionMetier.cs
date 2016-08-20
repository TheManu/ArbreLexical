using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exceptions
{
    public class ExceptionMetier : ExceptionBase
    {
        public ExceptionMetier() : base()
        { }

        public ExceptionMetier(
            string message) : base(message ?? string.Empty)
        { }

        public ExceptionMetier(
            string message,
            Exception innerException) : base(message ?? string.Empty, innerException)
        { }

        public override EnumTypeException TypeException
        {
            get
            {
                return EnumTypeException.Metier;
            }
        }
    }
}
