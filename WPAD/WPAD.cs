#if NETCOREAPP && !NET5_0_OR_GREATER
using System.Net.Http;
using WinAPI = Interop.WinHttp;
#endif

namespace System.Net
{
    public class WPAD
    {
        private readonly IWebProxy proxy;

        public WPAD(bool autoDetect = true)
        {
#if NETFRAMEWORK
            proxy = new WebProxyEx();
#elif NETCOREAPP && !NET5_0_OR_GREATER
            var proxyHelper = new WinInetProxyHelper { UseProxy = true };
            WinAPI.SafeWinHttpHandle sessionHandle = WinAPI.WinHttpOpen(
                userAgent: null,
                accessType: WinAPI.AccessType.WINHTTP_ACCESS_TYPE_NO_PROXY,
                proxyName: null,
                proxyBypass: null,
                flags: WinAPI.WINHTTP_FLAG_ASYNC);
            proxy = new HttpWindowsProxy(proxyHelper, sessionHandle);
#else
            throw new NotSupportedException();
#endif
            AutoDetect = autoDetect;
        }

        public WPAD(string pacScriptAddress) : this(autoDetect: false)
        {
            PacScriptAddress = pacScriptAddress;
        }

        public bool AutoDetect
        {
#if NETFRAMEWORK
            get { return (proxy as WebProxyEx).AutoDetect; }
            set { (proxy as WebProxyEx).AutoDetect = value; }
#elif NETCOREAPP && !NET5_0_OR_GREATER
            get { return (proxy as HttpWindowsProxy).ProxyHelper.AutoDetect; }
            set { (proxy as HttpWindowsProxy).ProxyHelper.AutoDetect = value; }
#else
            get { return false; }
#endif
        }

        public string PacScriptAddress
        {
#if NETFRAMEWORK
            get { return (proxy as WebProxyEx).ScriptLocation.OriginalString; }
            set { (proxy as WebProxyEx).ScriptLocation = new Uri(value); }
#elif NETCOREAPP && !NET5_0_OR_GREATER
            get { return (proxy as HttpWindowsProxy).ProxyHelper.AutoConfigUrl; }
            set { (proxy as HttpWindowsProxy).ProxyHelper.AutoConfigUrl = value; }
#else
            get { return ""; }
#endif
        }

        public bool AutoSettingsUsed
        {
            get { return AutoDetect || !string.IsNullOrEmpty(PacScriptAddress); }
        }

        public ICredentials Credentials
        {
            get { return proxy.Credentials; }
            set { proxy.Credentials = value; }
        }

        public Uri GetProxy(Uri destination) => proxy.GetProxy(destination);
    }
}
