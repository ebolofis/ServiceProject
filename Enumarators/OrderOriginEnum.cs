using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosToWebPosBridge.Enumarators
{
    /// <summary>
    /// Enumarator for system calls
    /// </summary>
    public enum OrderOriginEnum
    {
        Local = 0,
        Web = 1,
        CallCenter = 2,
        MobileApp = 3
    }

}
