using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace HandleKiller
{
    class Program
    {
        static void Main(string[] args)
        {
            var pipeIn = Console.In.ReadToEnd();
            if (!string.IsNullOrEmpty(pipeIn))
            {
                /*
                 Nthandle v4.11 - Handle viewer
                https://download.sysinternals.com/files/Handle.zip
                https://docs.microsoft.com/zh-cn/sysinternals/downloads/handle
Copyright (C) 1997-2017 Mark Russinovich
Sysinternals - www.sysinternals.com
explorer.exe       pid: 11672  type: File          36EC: netcoreapp3.1
WebApiHost.exe pid: 37912  type: File           1A0: WebApiHost.dll
              */
                List<int> handlerPids = new List<int>();
                foreach (Match m in Regex.Matches(pipeIn, "pid:\\s(?<pid>[\\d]+)"))
                {
                    int pid = Convert.ToInt32(m.Groups["pid"].Value);
                    handlerPids.Add(pid);
                }
                var allPids = handlerPids.ToArray().Distinct();
                if (allPids.Count() == 0)
                {
                    Console.WriteLine("没有进程占用资源！");
                }
                else
                {
                    foreach (var kid in allPids)
                    {
                        KillPocess(kid);
                    }
                }
            }
        }

        static void KillPocess(int pid)
        {
            Process p = Process.GetProcessById(pid);
            if (p != null)
            {
                try
                {
                    p.Kill();
                    Console.WriteLine($"成功: 已终止进程 \"{p.MainModule.FileName}\"，其 PID 为 {pid}。");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"失败: 终止进程 \"{p.ProcessName}\"，其 PID 为 {pid}。原因：{ex.Message}");
                }
            }
        }
    }
}
