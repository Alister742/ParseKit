using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ProxyFactory
{
    interface IProxySiteProvider
    {
        List<RatedProxy> ParsePage(string data);
    }
}
