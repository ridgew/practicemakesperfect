using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DBHR.ClientModule
{
    public partial class BaseEditForm : Form
    {
        public BaseEditForm()
        {
            InitializeComponent();
        }

        private List<Type> MainTypeList = new List<Type>();

        private List<Type> DetailTypeList = new List<Type>();

        private Dictionary<string, string> DetailMainObj = new Dictionary<string, string>();


        public void AddMainType(Type aType)
        {
            MainTypeList.Add(aType);
        }

        public void AddDetailType(Type aType)
        {
            DetailTypeList.Add(aType);
        }

        private List<string> CustomMainModuleIdList = new List<string>();

        public void AddCustomMainModuleType(Type aType, string customModuleId)
        {
            MainTypeList.Add(aType);
            CustomMainModuleIdList.Add(customModuleId);
        }

        public void AddDetailType(Type aType, Type bType)
        {
            ClientModuleDetailObj objDetail = (ClientModuleDetailObj)aType.InvokeMember(
                null, System.Reflection.BindingFlags.CreateInstance,
                null, null, null
                );

            ClientModuleObj objMain = (ClientModuleObj)bType.InvokeMember(null,
                 System.Reflection.BindingFlags.CreateInstance,
                 null,
                 null, null);

            DetailMainObj.Add(objDetail.FunID, objMain.FunID);
            DetailTypeList.Add(aType);
        }

        public void DoCreateFormControls()
        {

        }

    }

    public class ClientModuleObj : Panel
    {
        public string FunID { get; set; }
    }

    public class ClientModuleDetailObj : ClientModuleObj
    {

    }

}
