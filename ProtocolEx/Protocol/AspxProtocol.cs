using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;

namespace Protocol
{
    /// <summary>
    /// Summary description for AspxProtocol.
    /// </summary>
    [Guid("E00957BE-D0E1-4eb9-A025-7743FDC8B27B"), ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    [AsyncProtocol(Name = "aspx", Description = "Loads the aspx page using a local host and returns the HTML output.")]
    [ContextHandler(Name = "Execute", Key = "aspxfile")]
    [ContextHandler(Name = "Execute", Key = "asmxfile")]
    public class AspxProtocol : ProtocolBase, IInternetProtocol, IInternetProtocolRoot,
        IInternetProtocolInfo, IShellExtInit, IContextMenu
    {
        #region IInternetProtocol Members

        public void Start(string szURL, IInternetProtocolSink Sink, IInternetBindInfo pOIBindInfo, uint grfPI, uint dwReserved)
        {
            //System.Diagnostics.Debugger.Launch();
            AspNetRequestData RequestData = new AspNetRequestData(Stream);
            try
            {
                BINDINFO BindInfo = GetBindInfo(pOIBindInfo);
                RequestData.Verb = GetVerb(BindInfo);
                RequestData.PostData = GetPostData(BindInfo);

                IHttpNegotiate Negotiate = GetHttpNegotiate(Sink);
                string strRequestHeaders;
                Negotiate.BeginningTransaction(szURL, string.Empty, 0, out strRequestHeaders);
                RequestData.SetRequestHeaders(strRequestHeaders);

                ProcessRequest(RequestData, szURL);

                string strNewResponseHeaders;
                Negotiate.OnResponse(200, RequestData.GetResponseHeaders(), strRequestHeaders, out strNewResponseHeaders);

                //This must be the size of the buffer.
                Sink.ReportData(BSCF.BSCF_LASTDATANOTIFICATION, (uint)Stream.Length, (uint)Stream.Length);

                Sink.ReportResult(0, RequestData.Status, RequestData.StatusDescription);
            }
            catch (Exception e)
            {
                WriteBasicMessage(e.Message + "<hr>" + e.StackTrace, "Error");
            }
        }

        public void ProcessRequest(AspNetRequestData RequestData, string szURL)
        {
            int loc = szURL.IndexOf(':');

            if (loc < 0)
            {
                WriteBasicMessage("Invalid protocol format.  Must contain a colon (:)", "Error Page");
                return;
            }

            string Url = szURL.Remove(0, loc + 1).Replace("/", "\\");
            if (Directory.Exists(Url))
            {
                DirectoryBrowser(Url);
                return;
            }

            RequestData.Url = Url;
            AspNetHost Host = new AspNetHost();
            Host.process(RequestData);
        }

        #endregion
        #region IInternetProtocolInfo Members

        public UInt32 ParseUrl(string pwzUrl, Protocol.PARSEACTION ParseAction, UInt32 dwParseFlags, IntPtr pwzBuffer, UInt32 cchResult, out UInt32 pcchResult, UInt32 dwReserved)
        {
            Debug.WriteLine("ParseUrl:" + pwzUrl);
            pcchResult = 0;
            return HRESULT.INET_E_DEFAULT_ACTION;
        }

        public UInt32 CombineUrl(string pwzBaseUrl, string pwzRelativeUrl, UInt32 dwCombineFlags, IntPtr pwzBuffer, UInt32 cchResult, out UInt32 pcchResult, UInt32 dwReserved)
        {
            Debug.WriteLine("CombineUrl:" + pwzBaseUrl + "-" + pwzRelativeUrl);
            string temp = null;
            if (pwzRelativeUrl.Substring(0, 1) != "/")
                temp = DirectoryFromBaseUrl(pwzBaseUrl) + pwzRelativeUrl;
            else
                temp = VirtualDirctoryFromBaseUrl(pwzBaseUrl) + pwzRelativeUrl;
            if (pwzRelativeUrl.IndexOf(":") > 0)
                temp = pwzRelativeUrl;
            if (temp.Length > cchResult)
            {
                pcchResult = 0;
                return HRESULT.S_FALSE;
            }
            Marshal.Copy(temp.ToCharArray(), 0, pwzBuffer, temp.Length);
            Marshal.WriteInt32(pwzBuffer, temp.Length * 2, 0);
            pcchResult = (UInt32)temp.Length + 1;
            return HRESULT.S_OK;
        }

        public UInt32 CompareUrl(string pwzUrl1, string pwzUrl2, UInt32 dwCompareFlags)
        {
            return (UInt32)pwzUrl1.CompareTo(pwzUrl2);
        }

        public UInt32 QueryInfo(string pwzUrl, Protocol.QUERYOPTION OueryOption, UInt32 dwQueryFlags, IntPtr pBuffer, UInt32 cbBuffer, ref UInt32 pcbBuf, UInt32 dwReserved)
        {
            return HRESULT.INET_E_DEFAULT_ACTION;
        }

        #endregion
        #region IShellExtInit Members

        IDataObject m_DataObject;
        uint m_hDrop = 0;

        [DllImport("shell32")]
        static extern uint DragQueryFile(uint hDrop, uint iFile, StringBuilder buffer, int cch);

        [DllImport("user32")]
        static extern int InsertMenuItem(uint hmenu, uint uposition, uint uflags, ref MENUITEMINFO mii);

        [DllImport("user32")]
        static extern int MessageBox();

        [DllImport("user32")]
        static extern int MessageBox(int hWnd, string text, string caption, int type);

        public static void MessageBox(string str)
        {
            MessageBox(0, str, "Error", 0);
        }

        public int Initialize(IntPtr pidlFolder, IntPtr lpdobj, uint hKeyProgID)
        {
            try
            {
                m_DataObject = null;
                if (lpdobj == IntPtr.Zero)
                    throw new Exception("Data not found.");

                m_DataObject = (IDataObject)Marshal.GetObjectForIUnknown(lpdobj);
                FORMATETC fmt = new FORMATETC();
                fmt.cfFormat = CLIPFORMAT.CF_HDROP;
                fmt.ptd = 0;
                fmt.dwAspect = DVASPECT.DVASPECT_CONTENT;
                fmt.lindex = -1;
                fmt.tymed = TYMED.TYMED_HGLOBAL;
                STGMEDIUM medium = new STGMEDIUM();
                m_DataObject.GetData(ref fmt, ref medium);
                m_hDrop = (uint)medium.u;

                return 0;
            }
            catch (Exception e)
            {
                MessageBox(e.Message);
                return 0;
            }
        }

        #endregion
        #region IContextMenu Members

        public int QueryContextMenu(uint hmenu, uint iMenu, int idCmdFirst, int idCmdLast, uint uFlags)
        {
            try
            {
                int id = 1;
                if ((uFlags & 0xf) == 0 || (uFlags & (uint)CMF.CMF_EXPLORE) != 0)
                {
                    MENUITEMINFO mii = new MENUITEMINFO();
                    mii.cbSize = 48;
                    mii.fMask = (uint)MIIM.ID | (uint)MIIM.TYPE | (uint)MIIM.STATE;

                    mii.wID = idCmdFirst + id;
                    mii.fType = (uint)MF.STRING;
                    mii.dwTypeData = "Execute";
                    mii.fState = (uint)MF.ENABLED;
                    InsertMenuItem(hmenu, iMenu, 1, ref mii);
                    ++id;
                }
                return id;
            }
            catch (Exception e)
            {
                MessageBox(e.Message);
                return 0;
            }
        }

        public void InvokeCommand(IntPtr pici)
        {
            try
            {
                StringBuilder sb = new StringBuilder(1024);
                DragQueryFile(m_hDrop, 0, sb, sb.Capacity + 1);
                System.Diagnostics.Process.Start("aspx:" + sb.ToString().Replace(@"\", "/"));
            }
            catch (Exception e) { MessageBox(e.Message); }
        }

        public void GetCommandString(int idcmd, uint uFlags, int reserved, StringBuilder commandString, int cch)
        {
            try
            {
                switch (uFlags)
                {
                    case (uint)GCS.VERB:
                        commandString = new StringBuilder("Execute");
                        break;
                    case (uint)GCS.HELPTEXT:
                        commandString = new StringBuilder("Execute");
                        break;
                    case (uint)GCS.VALIDATE:
                        break;
                }
            }
            catch (Exception e) { MessageBox(e.Message); }
        }

        #endregion

        #region static members
        public static string DirectoryFromBaseUrl(string Url)
        {
            int loc = Url.LastIndexOf('/');
            if (loc < 0)
                return String.Empty;
            return Url.Substring(0, loc + 1);
        }

        public static string VirtualDirctoryFromBaseUrl(string Url)
        {
            int loc = Url.LastIndexOf('/');
            if (loc < 0)
                return String.Empty;
            return Url.Substring(0, loc);
        }

        public static string BaseDirectory(string Url)
        {
            string result = GlobalAsaxDir(Url);
            if (result != null)
                return result;
            result = WebConfigDir(Url);
            if (result != null)
                return result;
            return DirectoryFromBaseUrl(Url);
        }

        public static string[] DirectoryList(String Directory)
        {
            return Directory.Split('/');
        }

        public static string GlobalAsaxDir(String Dir)
        {
            string[] DirList = DirectoryList(Dir);
            for (int i = DirList.Length; i >= 0; --i)
            {
                string CurrentDir = String.Join("/", DirList, 0, i);
                if (File.Exists(CurrentDir + "/Global.asax"))
                    return CurrentDir;
            }
            return null;
        }

        public static string WebConfigDir(String Dir)
        {
            string[] DirList = DirectoryList(Dir);
            for (int i = 0; i < DirList.Length; ++i)
            {
                string CurrentDir = String.Join("/", DirList, 0, i);
                if (File.Exists(CurrentDir + "/web.config"))
                    return CurrentDir;
            }
            return null;
        }

        public void WriteBasicMessage(string Message, string Title)
        {
            StreamWriter Writer = new StreamWriter(Stream);
            Writer.Write("<HTML><HEAD><TITLE>{1}</TITLE></HEAD><BODY>{0}</BODY></HTML>", Message, Title);
            Writer.Flush();
            Stream.Position = 0;
        }

        public void DirectoryBrowser(string Dir)
        {
            if (!Dir.EndsWith("\\"))
                Dir += "\\";

            StringBuilder Builder = new StringBuilder();

            if (Directory.GetParent(Dir) != null && Directory.GetParent(Dir).Parent != null)
                Builder.Append(string.Format("<a href='aspx:{0}'>{0}</a><br>", Directory.GetParent(Dir).Parent.FullName.Replace("\\", "/")));

            foreach (string directory in Directory.GetDirectories(Dir))
                Builder.Append(string.Format("<a href='aspx:{0}'>{0}</a><br>", directory.Replace("\\", "/")));

            foreach (string File in Directory.GetFiles(Dir))
                Builder.Append(string.Format("<a href='aspx:{0}'>{0}</a><br>", File.Replace("\\", "/")));

            WriteBasicMessage(Builder.ToString(), Dir);
        }

        public string GetVerb(BINDINFO BindInfo)
        {
            if (BindInfo.dwBindVerb == BINDVERB.BINDVERB_GET)
                return "GET";
            if (BindInfo.dwBindVerb == BINDVERB.BINDVERB_POST)
                return "POST";
            throw new Exception("the Aspx protocol currently only accepts get and post verbs.");
        }

        public byte[] GetPostData(BINDINFO BindInfo)
        {
            if (BindInfo.dwBindVerb != BINDVERB.BINDVERB_POST)
                return new byte[0];
            byte[] result = new byte[0];
            if (BindInfo.stgmedData.enumType == TYMED.TYMED_HGLOBAL)
            {
                UInt32 length = BindInfo.cbStgmedData;
                result = new byte[length];

                Marshal.Copy(BindInfo.stgmedData.u, result, 0, (int)length);
                if (BindInfo.stgmedData.pUnkForRelease == null)
                    Marshal.FreeHGlobal(BindInfo.stgmedData.u);
            }
            return result;
        }

        #endregion

    }
}
