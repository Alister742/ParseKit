using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Browser_Emulator.Interfaces
{
    [Guid("eab22ac2-30c1-11cf-a7eb-0000c05bae0b"),
    InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IWebBrowserEvents
    {
        [DispId(100)]
        void RaiseBeforeNavigate(String url, int flags, String targetFrameName,
                   ref Object postData, String headers, ref Boolean cancel);
    }
}
