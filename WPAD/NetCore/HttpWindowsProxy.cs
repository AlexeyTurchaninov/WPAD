#if NETCOREAPP3_0_OR_GREATER
using System.Reflection;
using SafeWinHttpHandle = Interop.WinHttp.SafeWinHttpHandle;

namespace System.Net.Http
{
    internal class HttpWindowsProxy : IWebProxy
    {
        private readonly object thisObject;
        private static readonly Type type;
        private static readonly ConstructorInfo constructor;
        private static readonly MethodInfo getProxyMI;
        private static readonly PropertyInfo credentialsPI;
        private static readonly FieldInfo proxyHelperFI;
        private static readonly FieldInfo sessionHandleFI;

        static HttpWindowsProxy()
        {
            var bindingFlagPrivate = BindingFlags.Instance | BindingFlags.NonPublic;
            type = Type.GetType("System.Net.Http.HttpWindowsProxy, System.Net.Http");
            Type[] paramTypes = { WinInetProxyHelper.GetType(), SafeWinHttpHandle.GetType() };
            constructor = type?.GetConstructor(
                bindingFlagPrivate,
                binder: null,
                paramTypes,
                modifiers: null);
            getProxyMI = type?.GetMethod("GetProxy"); // public
            credentialsPI = type?.GetProperty("Credentials"); // public
            proxyHelperFI = type?.GetField("_proxyHelper", bindingFlagPrivate);
            sessionHandleFI = type?.GetField("_sessionHandle", bindingFlagPrivate);
        }

        public HttpWindowsProxy(WinInetProxyHelper proxyHelper, SafeWinHttpHandle sessionHandle)
        {
            object[] parameters = { proxyHelper.RealObject, sessionHandle.RealObject };
            thisObject = constructor?.Invoke(parameters);
        }

        public Uri GetProxy(Uri uri)
        {
            object[] parameters = { uri };
            return (Uri)getProxyMI?.Invoke(thisObject, parameters);
        }

        public bool IsBypassed(Uri host) => false;

        public ICredentials Credentials
        {
            get { return (ICredentials)credentialsPI.GetValue(thisObject, index: null); }
            set { credentialsPI.SetValue(thisObject, value, index: null); }
        }

        public WinInetProxyHelper ProxyHelper
        {
            get { return new WinInetProxyHelper { RealObject = proxyHelperFI?.GetValue(thisObject) }; }
            set { proxyHelperFI?.SetValue(thisObject, value.RealObject); }
        }

        public SafeWinHttpHandle SessionHandle
        {
            get { return new SafeWinHttpHandle { RealObject = sessionHandleFI?.GetValue(thisObject) }; }
            set { sessionHandleFI?.SetValue(thisObject, value.RealObject); }
        }

        public static new Type GetType() => type;
    }
}
#endif // NETCOREAPP3_0_OR_GREATER
