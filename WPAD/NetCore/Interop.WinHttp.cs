// Replicated interop classes from System.Net.Http (.NET Core, .NET 5)

using System;
using System.Reflection;

namespace Interop
{
    internal static class WinHttp
    {
        private static readonly Type type;
        private static readonly MethodInfo winHttpOpenMI;

        static WinHttp()
        {
            type = Type.GetType("Interop+WinHttp, System.Net.Http");
            winHttpOpenMI = type?.GetMethod("WinHttpOpen");
        }

        public const int WINHTTP_FLAG_ASYNC = 0x10000000; // this session is asynchronous (where supported)

        [Flags]
        public enum AccessType : uint
        {
            WINHTTP_ACCESS_TYPE_DEFAULT_PROXY = 0,
            WINHTTP_ACCESS_TYPE_NO_PROXY = 1,
            WINHTTP_ACCESS_TYPE_NAMED_PROXY = 3,
            WINHTTP_ACCESS_TYPE_AUTOMATIC_PROXY = 4
        }

        public static SafeWinHttpHandle WinHttpOpen(
            string userAgent, AccessType accessType, string proxyName, string proxyBypass, int flags)
        {
            object[] parameters = { userAgent, accessType, proxyName, proxyBypass, flags };
            object sessionHandle = winHttpOpenMI?.Invoke(obj: null, parameters);
            return sessionHandle != null ? new SafeWinHttpHandle { RealObject = sessionHandle } : null;
        }

        public class SafeWinHttpHandle // Fake class (does not contains a real fields)
        {
            private static readonly Type type;

            static SafeWinHttpHandle()
            {
                type = Type.GetType("Interop+WinHttp+SafeWinHttpHandle, System.Net.Http");
            }

            public object RealObject { get; set; } // The real typed object

            public static new Type GetType() => type;
        }
    }
}
