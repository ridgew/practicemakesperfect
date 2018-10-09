using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Protocol
{
    [System.Runtime.InteropServices.ComVisible(false)]
    public class ProtocolBase
    {
        #region IInternetProtocol Members
        /// <summary>
        /// 未来扩充需要，暂时未实现。
        /// </summary>
        public void Resume()
        {
            Debug.WriteLine("Resume");
        }

        /// <summary>
        /// 结束下载过程，释放扩展分配的资源。
        /// </summary>
        /// <param name="dwOptions"></param>
        public void Terminate(uint dwOptions)
        {
            Debug.WriteLine("Terminate");
        }

        public void Seek(_LARGE_INTEGER dlibMove, uint dwOrigin, out _ULARGE_INTEGER plibNewPosition)
        {
            Debug.WriteLine("Seek");
            plibNewPosition = new _ULARGE_INTEGER();
        }

        /// <summary>
        /// 锁定资源下载请求，这时IInternetProtocolRoot接口的Terminate方法将允许被调用，与此同时未下载完的数据仍然可以被读取。
        /// </summary>
        /// <param name="dwOptions"></param>
        public void LockRequest(uint dwOptions)
        {
            Debug.WriteLine("LockRequest");
        }

        /// <summary>
        /// 释放请求锁定
        /// </summary>
        public void UnlockRequest()
        {
            Debug.WriteLine("UnlockRequest");
        }

        /// <summary>
        /// 停止一个正在进行的资源下载过程
        /// </summary>
        /// <param name="hrReason"></param>
        /// <param name="dwOptions"></param>
        public void Abort(int hrReason, uint dwOptions)
        {
            Debug.WriteLine("Abort");
        }

        /// <summary>
        /// 未来扩充需要，暂时未实现
        /// </summary>
        public void Suspend()
        {
            Debug.WriteLine("Suspend");
        }

        /// <summary>
        /// 允许协议扩展继续进行进行资源数据下载过程。
        /// </summary>
        /// <param name="pProtocolData"></param>
        public void Continue(ref _tagPROTOCOLDATA pProtocolData)
        {
            Debug.WriteLine("Continue");
        }

        const int S_OK = 0;
        const int S_FALSE = 1;
        /// <summary>
        /// 浏览器调用这个方法从协议扩展获得相应的数据。
        /// </summary>
        /// <param name="pv"></param>
        /// <param name="cb"></param>
        /// <param name="pcbRead"></param>
        /// <returns></returns>
        public UInt32 Read(System.IntPtr pv, uint cb, out uint pcbRead)
        {
            pcbRead = (uint)Math.Min(cb, StreamBuffer.Length);
            pcbRead = (uint)Stream.Read(StreamBuffer, 0, (int)pcbRead);
            Marshal.Copy(StreamBuffer, 0, pv, (int)pcbRead);

            UInt32 response = (pcbRead == 0) ? (UInt32)S_FALSE : (UInt32)S_OK;
            return response;
        }

        #endregion

        [ComRegisterFunction]
        private static void RegisterProtocol(Type t)
        {
            ProtocolSupport.RegisterProtocol(t);
        }

        [ComUnregisterFunction]
        private static void UnregisterProtocol(Type t)
        {
            ProtocolSupport.UnregisterProtocol(t);
        }

        public static IHttpNegotiate GetHttpNegotiate(IInternetProtocolSink Sink)
        {
            if ((Sink is IServiceProvider) == false)
                throw new Exception("Error ProtocolSink does not support IServiceProvider.");
            Debug.WriteLine("ServiceProvider");

            IServiceProvider Provider = (IServiceProvider)Sink;
            object obj_Negotiate = new object();
            Provider.QueryService(ref Guids.IID_IHttpNegotiate, ref Guids.IID_IHttpNegotiate, out obj_Negotiate);
            return (IHttpNegotiate)obj_Negotiate;
        }

        public static BINDINFO GetBindInfo(IInternetBindInfo pOIBindInfo)
        {
            BINDINFO BindInfo = new BINDINFO();
            BindInfo.cbSize = (UInt32)Marshal.SizeOf(typeof(BINDINFO));
            UInt32 AsyncFlag;
            pOIBindInfo.GetBindInfo(out AsyncFlag, ref BindInfo);
            return BindInfo;
        }

        protected MemoryStream Stream = new MemoryStream(0x8000);
        protected byte[] StreamBuffer = new byte[0x8000];
    }

}