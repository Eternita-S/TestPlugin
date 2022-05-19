using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPlugin
{
    internal static class Extensions
    {
        internal static string PrepareSig(this string s) 
        {
            return s.Replace("?", "??");
        }
    }
}
