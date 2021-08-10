using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;

namespace RestartService
{
    class Program
    {
        static void Main(string[] args)
        {
            //string s = GetNamedArguments(new string[] { "CamstarSecurityServer.exe", "-s=192.168.224.125" }, "-s");
            //string s2 = GetNamedArguments(new string[] { "CamstarSecurityServer.exe", "-s", "192.168.224.125" }, "-s");
            //RemoteTaskKillService("192.168.224.125", "MesHealthCheck");
            //string result = RemoteCamstarServiceMethod(server, "CamstarSecurityServer.exe", "StopService");

            string cmd = null;
            do
            {
                if (cmd != null)
                    args = cmd.Split(' ');

                if (args.Length > 0)
                {
                    string restartServer = GetNamedArguments(args, "-restart");
                    if (!string.IsNullOrEmpty(restartServer) && IsIPAddress(restartServer))
                    {
                        string ret = Reboot(restartServer);
                        if (ret == "0")
                            Console.WriteLine($"重启服务器{restartServer}成功！！");
                        else
                            Console.WriteLine($"重启{restartServer}失败：{ret}");
                    }

                    string listServer = GetNamedArguments(args, "-list");
                    if (!string.IsNullOrEmpty(listServer) && IsIPAddress(listServer))
                    {
                        string err = null;
                        var process = GetServerProcess(listServer, ref err);
                        if (process.Count == 0)
                        {
                            Console.WriteLine($"获取{listServer}服务列表失败：{err}");
                        }
                        else
                        {
                            foreach (var item in process)
                                Console.WriteLine(item);
                        }
                    }

                    string startServer = GetNamedArguments(args, "-start");
                    string stopServer = GetNamedArguments(args, "-stop");
                    string svrName = GetNamedArguments(args, "-serviceName");
                    if (!string.IsNullOrEmpty(svrName))
                    {
                        string ret = "", topic = "";
                        if (IsIPAddress(stopServer))
                        {
                            topic = "停止服务";
                            ret = RemoteCamstarServiceMethod(stopServer, svrName, "StopService");
                        }

                        if (IsIPAddress(startServer))
                        {
                            topic = (topic == "停止服务") ? "重启服务" : "启动服务";
                            ret = RemoteCamstarServiceMethod(startServer, svrName, "StartService");
                        }

                        Console.WriteLine($"{topic} {svrName} {((ret == "0") ? "成功" : ret)}");
                    }
                }

                Console.WriteLine($"请输入指令：{Environment.NewLine} q退出，-list 192.168.230.100 服务进程；{Environment.NewLine} -restart 重启服务器；{Environment.NewLine} -stop 停止服务IP； -start 启动服务IP； -serviceName 服务名称；{Environment.NewLine}");
                Console.WriteLine();
                cmd = Console.ReadLine();
            }
            while (cmd != "q");
        }

        public static string GetNamedArguments(string[] args, string argsName)
        {
            int idx = Array.FindIndex(args, s => s.Equals(argsName, StringComparison.InvariantCultureIgnoreCase));

            if (idx == -1)
                idx = Array.FindIndex(args, s => s.StartsWith(argsName));

            if (idx != -1)
            {
                string argRaw = args[idx];
                if (argRaw.Contains("="))
                    return argRaw.Substring(argRaw.IndexOf('=') + 1);

                if (args.Length > idx + 1)
                    return args[idx + 1];

                return "TRUE";
            }
            return null;
        }

        public static bool IsTrue(string argsName)
        {
            return argsName != null && (argsName == "1" || argsName == "TRUE" || argsName == "true");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="server">远程服务器IP</param>
        /// <param name="serviceName">camstar服务名称</param>
        /// <param name="method">StartService/StopService等</param>
        /// <returns></returns>
        public static string RemoteCamstarServiceMethod(string server, string serviceName, string method = "StopService")
        {
            try
            {
                switch (serviceName)
                {
                    case "InSiteXMLServer.exe":
                        serviceName = "InSite Server";
                        break;
                    case "Camstar.Security.LMServer.exe":
                        serviceName = "CamstarSecurityLMServer";
                        break;
                    case "CamstarSecurityServer.exe":
                        serviceName = "Camstar Security Server";
                        break;
                    case "CamstarNotificationServer.exe":
                        serviceName = "NotificationServer";
                        break;
                    default:
                        return "-1";
                }

                ManagementScope scope = new ManagementScope($@"\\{server}\root\cimv2");
                scope.Connect();
                ManagementClass c = new ManagementClass(scope, new ManagementPath("Win32_Service"), new ObjectGetOptions(null, System.TimeSpan.MaxValue, true));
                var o = c.CreateInstance();
                serviceName = $"\\\\{server}\\root\\cimv2:Win32_Service.Name=\"{serviceName}\"";
                o.Path = new ManagementPath(serviceName);
                o.InvokeMethod(method, null);
                return "0";
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        /// <summary>
        /// 重启服务器
        /// </summary>
        /// <param name="server">服务器IP</param>
        internal static string Reboot(string server)
        {
            try
            {
                ManagementScope scope = new ManagementScope($@"\\{server}\root\cimv2");
                scope.Connect();
                ObjectQuery query = new ObjectQuery("SELECT * FROM win32_OperatingSystem");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
                ManagementObjectCollection queryCollection = searcher.Get();
                foreach (ManagementObject m in queryCollection)
                {
                    m.InvokeMethod("Reboot", null);
                }
                return "0";
            }
            catch (Exception exception)
            {
                return exception.Message;
            }

        }

        /// <summary>
        /// iP地址验证
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIPAddress(string ip)
        {
            if (string.IsNullOrEmpty(ip) || ip.Length < 7 || ip.Length > 15) return false;
            string regformat = @"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$";
            Regex regex = new Regex(regformat, RegexOptions.IgnoreCase);
            return regex.IsMatch(ip);
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="server"></param>
        public static List<string> GetServerProcess(string server, ref string error)
        {
            try
            {
                ManagementScope scope = new ManagementScope($@"\\{server}\root\cimv2");
                scope.Connect();
                ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_Process where Name like 'Camstar%' or Name like 'InSite%'");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
                ManagementObjectCollection queryCollection = searcher.Get();
                return (from ManagementObject m in queryCollection select m["Name"].ToString()).ToList();
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return new List<string>();
            }
        }

        static void RemoteTaskKillService(string serverName, string serviceName)
        {
            //https://stackoverflow.com/questions/18993363/unable-to-remotely-terminate-a-process-using-wmi-and-c-sharp/19011636#19011636
            ConnectionOptions options = new ConnectionOptions();
            options.Impersonation = ImpersonationLevel.Impersonate;
            //options.Username = serverDomain + "\\" + serverUser;
            //options.Password = serverPassword;

            ManagementScope scope = new ManagementScope("\\\\" + serverName + "\\root\\cimv2");   //, options);
            Console.WriteLine("Connecting to scope");
            scope.Connect();

            //RPC 服务器不可用。 (异常来自 HRESULT:0x800706BA)
            //netsh firewall set service RemoteAdmin
            //netsh advfirewall firewall

            Console.WriteLine("Getting ManagementPath");
            ManagementPath servicePath = new ManagementPath("Win32_Service.Name='" + serviceName + "'");
            Console.WriteLine("Getting ManagementObject");
            ManagementObject serviceObj = new ManagementObject(scope, servicePath, new ObjectGetOptions());
            Console.WriteLine("Name of service is " + serviceObj["DisplayName"].ToString());
            Console.WriteLine("Process ID of service is " + serviceObj["ProcessId"].ToString());

            // use processid to kill process with taskkill
            ObjectGetOptions processObjGetOpt = new ObjectGetOptions();
            ManagementPath processPath = new ManagementPath("Win32_Process");
            ManagementClass processClass = new ManagementClass(scope, processPath, processObjGetOpt);
            ManagementBaseObject processInParams = processClass.GetMethodParameters("Create");
            processInParams["CommandLine"] = string.Format("cmd /c \"taskkill /f /pid {0}\"", serviceObj["ProcessId"].ToString());
            ManagementBaseObject outParams = processClass.InvokeMethod("Create", processInParams, null);
            Console.WriteLine("Return code for taskkill: " + outParams["returnValue"]);
            int returnCode = Convert.ToInt32(outParams["returnValue"]);
        }

        public int KillServiceWMI(string serviceName, string serverName, string serverUser, string serverDomain, string serverPassword)
        {
            try
            {
                ConnectionOptions options = new ConnectionOptions();
                options.Impersonation = ImpersonationLevel.Impersonate;
                options.Username = serverDomain + "\\" + serverUser;
                options.Password = serverPassword;

                ManagementScope scope = new ManagementScope("\\\\" + serverName + "\\root\\cimv2", options);
                Console.WriteLine("Connecting to scope");
                scope.Connect();

                Console.WriteLine("Getting ManagementPath");
                ManagementPath servicePath = new ManagementPath("Win32_Service.Name='" + serviceName + "'");
                Console.WriteLine("Getting ManagementObject");
                ManagementObject serviceObj = new ManagementObject(scope, servicePath, new ObjectGetOptions());
                Console.WriteLine("Name of service is " + serviceObj["DisplayName"].ToString());
                Console.WriteLine("Process ID of service is " + serviceObj["ProcessId"].ToString());
                ObjectQuery serviceQuery = new ObjectQuery("SELECT * from Win32_Process WHERE ProcessID = '" + serviceObj["ProcessId"].ToString() + "'");
                ManagementObjectSearcher serviceSearcher = new ManagementObjectSearcher(scope, serviceQuery);
                ManagementObjectCollection serviceColl = serviceSearcher.Get();
                int returnCode = 0;
                foreach (ManagementObject currentObj in serviceColl)
                {
                    if (currentObj["ProcessId"].ToString().Equals(serviceObj["ProcessId"].ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Found process " + currentObj["ProcessId"].ToString() + ". Terminating...");
                        ManagementBaseObject termParams = currentObj.InvokeMethod("Terminate", (ManagementBaseObject)null, null);
                        returnCode = Convert.ToInt32(termParams.Properties["ReturnValue"].Value);
                    }
                }
                return returnCode;
            }
            catch (Exception connectEx)
            {
                Console.WriteLine("Connecting to " + serviceName + " caused an exception");
                Console.Write(connectEx);
                return 99;
            }
        }

    }
}
