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
            if(string.IsNullOrEmpty(address))
            {
                return new string[3] { string.Empty, string.Empty, string.Empty };
            }

            int fullLength = address.Length;
            var result = new string[3] { string.Empty, string.Empty, string.Empty };

            if (fullLength <= 45)
            {
                result[0] = address;
            }
            else if (fullLength <= 90)
            {
                result[0] = address.Substring(0, 45);
                result[1] = address.Substring(45, fullLength - 45);
            }
            else
            {
                result[0] = address.Substring(0, 45);
                result[1] = address.Substring(45, 45);
                result[2] = address.Substring(90, fullLength - 90);
            }
            return result;
        }
    }
}
