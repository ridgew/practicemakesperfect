using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.AddIn.Pipeline;

namespace HostSideAdapter
{

    /// <summary>
    /// Adapter use to talk to <see cref="HostView.NumberProcessorHostView">Host View</see>
    /// </summary>
    [HostAdapter]
    public class NumberProcessorContractToViewHostAdapter : HostView.NumberProcessorHostView
    {
        #region Data
        private Contract.INumberProcessorContract contract;
        private ContractHandle contractHandle;
        #endregion

        #region Ctor
        public NumberProcessorContractToViewHostAdapter(Contract.INumberProcessorContract contract)
        {            
            this.contract = contract;
            contractHandle = new ContractHandle(contract);
        }
        #endregion

        #region Public Methods
        public override List<int> ProcessNumbers(int fromNumber, int toNumber)
        {
            return contract.ProcessNumbers(fromNumber, toNumber);
        }

        public override void Initialize(HostView.HostObject host)
        {            
            HostObjectViewToContractHostAdapter hostAdapter = new HostObjectViewToContractHostAdapter(host);
            contract.Initialize(hostAdapter);
        }
        #endregion
    }


    /// <summary>
    /// Allows Host side adapter to talk back to HostView
    /// </summary>
    public class HostObjectViewToContractHostAdapter : ContractBase, Contract.IHostObjectContract
    {
        #region Data
        private HostView.HostObject view;
        #endregion

        #region Public Methods
        public HostObjectViewToContractHostAdapter(HostView.HostObject view)
        {
            this.view = view;
        }

        public void ReportProgress(int progressPercent)
        {
            view.ReportProgress(progressPercent);
        }
        #endregion
    }
}
