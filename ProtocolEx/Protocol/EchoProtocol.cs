using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Protocol
{
	/// <summary>
	/// Summary description for EchoProtocol.
	/// </summary>
	[ Guid("E00957BD-D0E1-4eb9-A025-7743FDC8B27B"), ComVisible(true), ClassInterface(ClassInterfaceType.None)]
	[ AsyncProtocol(Name="echo", Description="Returns the URL of the protocol as HTML content.") ]
	public class EchoProtocol : ProtocolBase, IInternetProtocol, IInternetProtocolRoot
	{
		#region IInternetProtocol Members

		public void Start(string szURL, IInternetProtocolSink Sink, IInternetBindInfo pOIBindInfo, uint grfPI, uint dwReserved)
		{
			Debug.WriteLine("Start:" + szURL, "Info");
			try 
			{
				if (Sink is IServiceProvider)
				{
					Debug.WriteLine("ServiceProvider");
					IServiceProvider Provider = (IServiceProvider)Sink;
					object obj_Negotiate = new object();
					Provider.QueryService(ref Guids.IID_IHttpNegotiate, ref Guids.IID_IHttpNegotiate, out obj_Negotiate);
					IHttpNegotiate Negotiate = (IHttpNegotiate)obj_Negotiate;

					string strNewHeaders;
					Negotiate.BeginningTransaction(szURL, string.Empty, 0, out strNewHeaders);
					Debug.WriteLine(strNewHeaders);

					StreamWriter Writer = new StreamWriter(Stream); 
					int loc = szURL.IndexOf(':');
					if (loc >= 0)
						Writer.Write(string.Format("<HTML><BODY>{0}</BODY></HTML>", System.Web.HttpUtility.UrlDecode(szURL.Remove(0, loc+1))));
					else
						Writer.Write(string.Format("<HTML><BODY>{0}</BODY></HTML>", System.Web.HttpUtility.UrlDecode(szURL)));
					Writer.Flush();
					Stream.Position = 0;

					string StrResponseHeaders = string.Format("HTTP/1.1 200 OK\r\nContent-Type: text/html; charset=utf-8\r\nContent-Length:{0}\r\n\r\n", Stream.Length);
					string strNewResponseHeaders;
					Negotiate.OnResponse(200, StrResponseHeaders, strNewHeaders, out strNewResponseHeaders);
					Debug.WriteLine(strNewResponseHeaders);
				}

				Sink.ReportData(BSCF.BSCF_LASTDATANOTIFICATION, (uint)Stream.Length, (uint)Stream.Length);
				Sink.ReportResult(0, 200, null);
			} 
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
			}
		}

		#endregion
	}
}
