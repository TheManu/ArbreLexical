using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Exceptions;

namespace ArbreLexicalService.Exceptions
{
    public class ExceptionArbreConstruction : ExceptionTechnique
    {
        public ExceptionArbreConstruction() : base()
        { }

        public ExceptionArbreConstruction(
            string message) : base(message ?? string.Empty)
        { }

        public ExceptionArbreConstruction(
            string message,
            Exception innerException) : base(message ?? string.Empty, innerException)
        { }
    }
}
