using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Exceptions;

namespace ArbreLexicalService.Exceptions
{
    public class ExceptionTechniqueArbreConstruction : ExceptionTechnique
    {
        public ExceptionTechniqueArbreConstruction() : base()
        { }

        public ExceptionTechniqueArbreConstruction(
            string message) : base(message ?? string.Empty)
        { }

        public ExceptionTechniqueArbreConstruction(
            string message,
            Exception innerException) : base(message ?? string.Empty, innerException)
        { }
    }
}
