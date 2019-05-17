using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.AddIn.Pipeline;

namespace AddInSideAdapter
{

    /// <summary>
    /// Adapter use to talk to AddIn <see cref="Contract.INumberProcessorContract">AddIn Contract</see>
    /// </summary>
    [AddInAdapter]
    public class NumberProcessorViewToContractAdapter : ContractBase, Contract.INumberProcessorContract
    {
        #region Data
        private AddInView.NumberProcessorAddInView view;
        #endregion

        #region Ctor
        public NumberProcessorViewToContractAdapter(AddInView.NumberProcessorAddInView view)
        {
            this.view = view;
        }
        #endregion

        #region Public Methods
        public List<int> ProcessNumbers(int fromNumber, int toNumber)
        {
            return view.ProcessNumbers(fromNumber, toNumber);
        }

        public void Initialize(Contract.IHostObjectContract hostObj)
        {            
            view.Initialize(new HostObjectContractToViewAddInAdapter(hostObj));
        }
        #endregion
    }

    /// <summary>
    /// Allows AddIn adapter to talk back to HostView
    /// </summary>
    public class HostObjectContractToViewAddInAdapter : AddInView.HostObject
    {
        #region Data
        private Contract.IHostObjectContract contract;
        private ContractHandle handle;
        #endregion
        
        #region Ctor
        public HostObjectContractToViewAddInAdapter(Contract.IHostObjectContract contract)
        {
            this.contract = contract;
            this.handle = new ContractHandle(contract);
        }
        #endregion

        #region Public Methods
        public override void ReportProgress(int progressPercent)
        {
            contract.ReportProgress(progressPercent);
        }
        #endregion
    }
    
}

