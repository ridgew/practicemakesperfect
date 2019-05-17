using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HostView
{
    /// <summary>
    /// Abstract base class that should be inherited by the Host view
    /// </summary>
    public abstract class NumberProcessorHostView
    {
        #region Abstract Methods
        public abstract List<int> ProcessNumbers(int fromNumber, int toNumber);

        public abstract void Initialize(HostObject host);
        #endregion
    }

    /// <summary>
    /// Abstract base class that should be inherited by a class within the host
    /// application that can make use of the reported progress
    /// </summary>
    public abstract class HostObject
    {
        #region Abstract Methods
        public abstract void ReportProgress(int progressPercent);
        #endregion
    }
}
