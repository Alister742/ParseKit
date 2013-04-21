using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ProxyFactory
{
    public interface IProxyContainer
    {
        RatedProxy Proxy { get; }
    }
}
