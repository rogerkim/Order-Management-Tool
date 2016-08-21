using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangoOrderImporter
{
    public class Util
    {
        public static string[] SplitAddress(string address)
        {
            var result = new string[3];
            result[0] = address.Substring(0, 45);
            result[1] = address.Substring(45, 45);
            result[2] = address.Substring(90, 45);
            return result;
        }
    }
}
