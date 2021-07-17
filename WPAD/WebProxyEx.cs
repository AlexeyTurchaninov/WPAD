using System.Reflection;

namespace System.Net
{
    internal class WebProxyEx : WebProxy
    {
        private static readonly PropertyInfo autoDetectPI; // Has setter only
        private static readonly PropertyInfo scriptLocationPI; // Has setter only
        private bool autoDetect;
        private Uri scriptLocation;

        static WebProxyEx()
        {
            var bindingFlagPrivate = BindingFlags.Instance | BindingFlags.NonPublic;
            autoDetectPI = typeof(WebProxy).GetProperty("AutoDetect", bindingFlagPrivate);
            scriptLocationPI = typeof(WebProxy).GetProperty("ScriptLocation", bindingFlagPrivate);
        }

        public WebProxyEx() : base() { } // m_EnableAutoproxy = true; (initialized inside the base constructor)

        public WebProxyEx(Uri pacScriptLocation) : this()
        {
            ScriptLocation = pacScriptLocation;
        }

        public WebProxyEx(string pacScriptLocation) : this(new Uri(pacScriptLocation)) { }

        public bool AutoDetect
        {
            get { return autoDetect; }
            set { autoDetectPI.SetValue(this, autoDetect = value, index: null); }
        }

        public Uri ScriptLocation
        {
            get { return scriptLocation; }
            set { scriptLocationPI.SetValue(this, scriptLocation = value, index: null); }
        }
    }
}
