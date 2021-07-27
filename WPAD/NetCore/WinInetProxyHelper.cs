using System.Reflection;

namespace System.Net.Http
{
    internal class WinInetProxyHelper // Fake class (does not contains a real fields)
    {
        private static readonly Type type;
        private static readonly ConstructorInfo constructor;
        private static readonly FieldInfo autoConfigUrlFI;
        private static readonly FieldInfo autoDetectFI;
        private static readonly FieldInfo proxyFI;
        private static readonly FieldInfo proxyBypassFI;
        private static readonly FieldInfo useProxyFI;

        static WinInetProxyHelper()
        {
            var bindingFlagPrivate = BindingFlags.Instance | BindingFlags.NonPublic;
            type = Type.GetType("System.Net.Http.WinInetProxyHelper, System.Net.Http");
            constructor = type?.GetConstructor(Type.EmptyTypes);

            autoConfigUrlFI = GetBackingField(type, "AutoConfigUrl", bindingFlagPrivate);
            autoDetectFI = GetBackingField(type, "AutoDetect", bindingFlagPrivate);
            proxyFI = GetBackingField(type, "Proxy", bindingFlagPrivate);
            proxyBypassFI = GetBackingField(type, "ProxyBypass", bindingFlagPrivate);

            useProxyFI = type?.GetField("_useProxy", bindingFlagPrivate);
        }

        private static FieldInfo GetBackingField(Type type, string name, BindingFlags bindingAttr)
        {
            return type?.GetField($"<{name}>k__BackingField", bindingAttr)
                ?? type?.GetField($"_{char.ToLower(name[0]) + name[1..]}", bindingAttr);
        }

        public WinInetProxyHelper()
        {
            RealObject = constructor?.Invoke(parameters: null);

            // Re-init after constructor logic
            AutoConfigUrl = null;
            AutoDetect = false;
            Proxy = null;
            ProxyBypass = null;
            UseProxy = false;
        }

        public object RealObject { get; set; } // The real typed object

        public string AutoConfigUrl
        {
            get { return (string)autoConfigUrlFI?.GetValue(RealObject); }
            set { autoConfigUrlFI?.SetValue(RealObject, value); }
        }

        public bool AutoDetect
        {
            get { return (bool)(autoDetectFI?.GetValue(RealObject) ?? false); }
            set { autoDetectFI?.SetValue(RealObject, value); }
        }

        public bool AutoSettingsUsed
        {
            get { return AutoDetect || !string.IsNullOrEmpty(AutoConfigUrl); }
        }

        public string Proxy
        {
            get { return (string)proxyFI?.GetValue(RealObject); }
            set { proxyFI?.SetValue(RealObject, value); }
        }

        public string ProxyBypass
        {
            get { return (string)proxyBypassFI?.GetValue(RealObject); }
            set { proxyBypassFI?.SetValue(RealObject, value); }
        }

        public bool UseProxy
        {
            get { return (bool)(useProxyFI?.GetValue(RealObject) ?? false); }
            set { useProxyFI?.SetValue(RealObject, value); }
        }

        public static new Type GetType() => type;
    }
}
