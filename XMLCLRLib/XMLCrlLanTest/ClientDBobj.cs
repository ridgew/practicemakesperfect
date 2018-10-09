using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLCrlLanTest
{
    public static class ClientDBobj
    {
        public static object GetParamValue(string key, bool ignoreCase)
        {
            return "0";
        }

        public static string ToText(this object obj)
        {
            if (obj == null) return string.Empty;
            return obj.ToString();
        }
    }
}
