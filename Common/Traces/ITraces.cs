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

        void PublierErreur(
            string message);

        void PublierInformation(
            string message);

        void PublierAvertissement(
            string message);
    }
}