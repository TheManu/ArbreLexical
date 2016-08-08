using System;

namespace Common.Traces
{
    public interface ITraces
    {
        void PublierException(
            string message,
            Exception ex);

        void PublierException(
            Exception ex);
    }
}