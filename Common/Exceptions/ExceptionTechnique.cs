using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exceptions
{
    public class ExceptionTechnique : ExceptionBase
    {
        public ExceptionTechnique() : base()
        { }

        public ExceptionTechnique(
            string message) : base(message ?? string.Empty)
        { }

        public ExceptionTechnique(
            string message,
            Exception innerException) : base(message ?? string.Empty, innerException)
        { }
    }
}
