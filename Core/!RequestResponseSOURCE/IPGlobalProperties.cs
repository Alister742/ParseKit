 
using System;
using System.Net.Sockets;
using System.Net;
 
namespace System.Net.NetworkInformation
{ 
 
    public abstract class IPGlobalProperties
    { 
        public static IPGlobalProperties GetIPGlobalProperties(){
            (new NetworkInformationPermission(NetworkInformationAccess.Read)).Demand();
            return new SystemIPGlobalProperties();
        } 

        internal static IPGlobalProperties InternalGetIPGlobalProperties() 
        { 
            return new SystemIPGlobalProperties();
        } 

        /// Gets the Active Udp Listeners on this machine
        public abstract IPEndPoint[] GetActiveUdpListeners();
 
        /// Gets the Active Tcp Listeners on this machine
        public abstract IPEndPoint[] GetActiveTcpListeners (); 
 
        /// Gets the Active Udp Listeners on this machine
        public abstract TcpConnectionInformation[] GetActiveTcpConnections(); 

        /// Gets the Dynamic Host Configuration Protocol (DHCP) scope name.
        public abstract string DhcpScopeName {get;}
 
        /// Gets the domain in which the local computer is registered.
 
        public abstract string DomainName {get;} 

        /// Gets the host name for the local computer. 

        public abstract string HostName {get;}

        /// Gets a bool value that specifies whether the local computer is acting as a Windows Internet Name Service (WINS) proxy. 
        public abstract bool IsWinsProxy {get;}
 
        /// Gets the Network Basic Input/Output System (NetBIOS) node type of the local computer. 
        public abstract NetBiosNodeType NodeType{get;}
 

        public abstract TcpStatistics GetTcpIPv4Statistics();

        public abstract TcpStatistics GetTcpIPv6Statistics(); 

        /// Provides Internet Control Message Protocol (ICMP) version 4 statistical data for the local computer. 
        /// Provides User Datagram Protocol (UDP) statistical data for the local computer. 

        public abstract UdpStatistics GetUdpIPv4Statistics(); 
        public abstract UdpStatistics GetUdpIPv6Statistics();

        /// Provides Internet Control Message Protocol (ICMP) version 4 statistical data for the local computer.
 
        public abstract IcmpV4Statistics GetIcmpV4Statistics();
 
        /// Provides Internet Control Message Protocol (ICMP) version 6 statistical data for the local computer. 

        public abstract IcmpV6Statistics GetIcmpV6Statistics(); 

        /// Provides Internet Protocol (IP) statistical data for the local computer.
        public abstract IPGlobalStatistics GetIPv4GlobalStatistics();
        public abstract IPGlobalStatistics GetIPv6GlobalStatistics(); 
    }
} 
 

// File provided for Reference Use Only by Microsoft Corporation (c) 2007.
// Copyright (c) Microsoft Corporation. All rights reserved.
