using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ProxyFactory
{
    //public delegate void ProxyReleaseDel(ProxyContainer proxy);
    public delegate void FreeProxyDel(ProxyContainer proxy);

    public interface IProxyProvider
    {
        event FreeProxyDel OnProxyFreed;
        event EventHandler OnProxiesResulted;

        bool UseLocalhost { get; }
        int SlotsAvailable { get; }

        void Fire(ProxyContainer proxy);
        bool TryGet(out ProxyContainer proxy);
        void Release(ProxyContainer proxy, bool success);
    }
}
