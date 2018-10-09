using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.Hosting;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Protocol
{
	/// <summary>
	/// Summary description for AspNetHost.
	/// </summary>
	[System.Runtime.InteropServices.ComVisible(false)]
    public class AspNetHost
	{
		public static Hashtable HostMap = new Hashtable();
		public static AspNetBridge GetBridge(string Url)
		{
			string Directory = DirectoryFromUrl(Url);

			object host = HostMap[Directory];
			if (host == null)
			{
				host = ApplicationHost.CreateApplicationHost(typeof(AspNetBridge), "/", Directory);
				HostMap[Directory] = host;
			}
			return (AspNetBridge)host;
		}

		public static string DirectoryFromUrl(string Url)
		{
			string[] DirList = DirectoryList(Url);
			string result = GlobalAsaxDir(DirList);
			if (result != null)
				return result;
			result = WebConfigDir(DirList);
			if (result != null)
				return result;
			return string.Join("\\", DirList, 0, DirList.Length-1);
		}

		public static string[] DirectoryList(String Directory)
		{
			return Directory.Split('\\');
		}

		public static string GlobalAsaxDir(string[] DirList)
		{
			for(int i = DirList.Length; i>=0; --i)
			{
				string CurrentDir = String.Join("\\", DirList, 0, i);
				if (File.Exists(CurrentDir + "\\Global.asax"))
					return CurrentDir;
			}
			return null;
		}

		public static string WebConfigDir(string[] DirList)
		{
			for(int i = 0; i<DirList.Length; ++i)
			{
				string CurrentDir = String.Join("\\", DirList, 0, i);
				if (File.Exists(CurrentDir + "\\web.config"))
					return CurrentDir;
			}
			return null;
		}

		public void process(AspNetRequestData RequestData)
		{
			GetBridge(RequestData.Url).SendRequest(RequestData);
		}

	}

	[System.Runtime.InteropServices.ComVisible(false)]
    public class AspNetBridge : System.MarshalByRefObject
	{
		public void SendRequest(AspNetRequestData RequestData)
		{
			Debug.WriteLine("Bin:" + HttpRuntime.BinDirectory);
			Debug.WriteLine("AppPath:" + HttpRuntime.AppDomainAppPath);
			HttpRuntime.ProcessRequest(new AspNetRequest(RequestData));
		}
	}

	[System.Runtime.InteropServices.ComVisible(false)] 
	public class AspNetRequest : SimpleWorkerRequest
	{
		public AspNetRequest(AspNetRequestData requestData) :
			base(requestData.Page, requestData.QueryString, TextWriter.Null)
		{	
			_RequestData = requestData;						  
		}

		protected AspNetRequestData _RequestData;

		public override string GetKnownRequestHeader(int index)
		{
			string HeaderName = HttpWorkerRequest.GetKnownRequestHeaderName(index);
			return GetUnknownRequestHeader(HeaderName);
		}

		public override string GetUnknownRequestHeader(string HeaderName)
		{
			string Result = _RequestData.ReadRequestHeader(HeaderName);
			Debug.WriteLine("Header Requested:" + HeaderName + " result:" + Result);
			return Result;
		}

		public override string[][] GetUnknownRequestHeaders()
		{
			return base.GetUnknownRequestHeaders ();
		}

		public override void SendKnownResponseHeader(int index, string value)
		{
			SendUnknownResponseHeader(HttpWorkerRequest.GetKnownRequestHeaderName(index), value);
		}
		
		public override void SendUnknownResponseHeader(string name, string value)
		{
			Debug.WriteLine("Send Header:" + name + "=" + value);
			_RequestData.SendResponseHeader(name, value);
		}

		public override void SendStatus(int statusCode, string statusDescription)
		{
			Debug.WriteLine("Status:" + statusDescription);
			_RequestData.Status = (uint)statusCode;
			_RequestData.StatusDescription = statusDescription;
		}

		public override string GetHttpVerbName()
		{
			string Verb = _RequestData.Verb;
			Debug.WriteLine("Get Verb:" + Verb);
			return Verb;
		}

		public override byte[] GetPreloadedEntityBody()
		{
			Debug.WriteLine("Post data requested.");
			byte[] data = _RequestData.PostData;
			if (data != null)
				return data;
			return base.GetPreloadedEntityBody();
		}

		public override void EndOfRequest()
		{
			_RequestData.EndOfRequest();
			base.EndOfRequest();
		}

		public override void SendCalculatedContentLength(int contentLength)
		{
			_RequestData.SendResponseHeader("Content-Length:", contentLength.ToString());
			base.SendCalculatedContentLength (contentLength);
		}

		public override void SendResponseFromMemory(byte[] data, int length)
		{
			Debug.WriteLine("SendResponsefromMemory");
			_RequestData.WriteData(data, length);
		}

		public override void SendResponseFromMemory(System.IntPtr data, int length)
		{
			if (buffer.Length < length)
				buffer = new byte[length];
			Marshal.Copy(data, buffer, 0, length);
			SendResponseFromMemory(buffer, length);
		}

		public override void SendResponseFromFile(string filename, long offset, long length)
		{
			Debug.WriteLine("SendResponsefromFile:" + filename);
		}

		public override void SendResponseFromFile(System.IntPtr handle, long offset, long length)
		{
			Debug.WriteLine("SendResponsefromFileHandle");
		}

		byte[] buffer = new byte[0x200];
	}

	[System.Runtime.InteropServices.ComVisible(false)] 
	public class AspNetRequestData : System.MarshalByRefObject
	{
		public AspNetRequestData(MemoryStream Stream)
		{
			ResponseStream = Stream;
		}

		public void WriteData(byte[] data, int length)
		{
			ResponseStream.Write(data, 0, length);
		}

		public void EndOfRequest()
		{
			ResponseStream.Position = 0;
		}

		public void SendResponseHeader(string name, string value)
		{
			ResponseHeaders[name] = value;
		}

		public string ReadRequestHeader(string name)
		{
			if (name == "Content-Length")
				return PostData.Length.ToString();
			string result = RequestHeaders[name];
			return (result == null) ? string.Empty : result.Trim();
		}
		public void SetRequestHeaders(string Headers)
		{
			if (Headers == null || Headers == String.Empty)
				return;
			string[] arrHeaders = Headers.Split('\r', '\n');
			foreach(string FullHeader in arrHeaders)
			{
				if (FullHeader != String.Empty)
				{
					int loc = FullHeader.IndexOf(':');
					if (loc < 0)
						RequestHeaders[FullHeader] = "";
					else
					{
						RequestHeaders[FullHeader.Substring(0, loc)] = FullHeader.Substring(loc+1);
						Debug.WriteLine(string.Format("Writing Header:{0}={1}", FullHeader.Substring(0, loc), FullHeader.Substring(loc+1)));
					}
				}
			}
		}

		public string GetResponseHeaders()
		{
			StringWriter Writer = new StringWriter();
			foreach(string HeaderName in ResponseHeaders)
			{
				Writer.Write(HeaderName);
				Writer.Write(":");
				Writer.Write(ResponseHeaders[HeaderName]);
				Writer.Write("\r\n");
			}
			Writer.Write("\r\n");
			return Writer.GetStringBuilder().ToString();
		}

		public uint Status;
		public string StatusDescription;

		public string Verb;
		public string Url
		{
			set 
			{ 
				int loc = value.IndexOf('?');
				if (loc < 0)
					_baseUrl = value.Replace("/", "\\");
				else
				{
					_baseUrl = value.Substring(0, loc).Replace("/", "\\");
					QueryString = value.Substring(loc+1);
				}
			}
			get { return _baseUrl; }
		}

		public string Page
		{
			get 
			{
				int loc = _baseUrl.LastIndexOf('\\');
				if (loc < 0)
					return _baseUrl;
				return _baseUrl.Remove(0, loc+1);
			}
		}

		private string _baseUrl;
		public string QueryString = string.Empty;

		public NameValueCollection RequestHeaders = new NameValueCollection();
		public NameValueCollection ResponseHeaders = new NameValueCollection();
		public byte[] PostData = new byte[0];

		public MemoryStream ResponseStream;
	}

}
