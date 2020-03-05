using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Impersonation
{
    public static class ImpersonationUtil
    {
        public const int LOGON32_PROVIDER_DEFAULT = 0;
        public const int LOGON32_LOGON_INTERACTIVE = 2;

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(string userName, string domainName, string password,int logonType, int loggonProvider, ref IntPtr token);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public extern static bool DuplicateToken(IntPtr existingToken,int imperonationLevel, ref IntPtr newToken);

        public static WindowsImpersonationContext Impersonate(string userName, string domainName, string password, TokenImpersonationLevel tokenImpersonationLevel)
        {
            IntPtr token = IntPtr.Zero;
            IntPtr duplicateToken = IntPtr.Zero;
            if (LogonUser(userName, domainName, password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref token))
            {
                int impersonationLevel;
                switch (tokenImpersonationLevel)
                {
                    case TokenImpersonationLevel.Identification:
                        { impersonationLevel = 1; break; }
                    case TokenImpersonationLevel.Impersonation:
                        { impersonationLevel = 2; break; }
                    case TokenImpersonationLevel.Delegation:
                        { impersonationLevel = 3; break; }
                    default:
                        { impersonationLevel = 0; break; }
                }
                if (DuplicateToken(token, impersonationLevel, ref duplicateToken))
                {
                    WindowsIdentity identity = new WindowsIdentity(duplicateToken);
                    return identity.Impersonate();
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Duplicate token error (Error Code: {0})", Marshal.GetLastWin32Error()));
                }
            }
            else
            {
                throw new InvalidOperationException(string.Format("Logon user error (Error Code: {0})", Marshal.GetLastWin32Error()));
            }
        }
    }
}








