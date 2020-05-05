using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Bit.Core.Abstractions;
using Bit.Core.Enums;

namespace bw.lib.Services
{
    public class PlatformUtilsService : IPlatformUtilsService
    {
        public PlatformUtilsService(string identityClientId)
        {
            IdentityClientId = identityClientId;
        }

        public string IdentityClientId { get; }

        public string GetApplicationVersion()
        {
            return Assembly.GetEntryAssembly().GetName().Version.ToString();
        }

        public DeviceType GetDevice()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.MacOSX:
                    return DeviceType.MacOsDesktop;
                case PlatformID.Unix:
                    return DeviceType.LinuxDesktop;
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                case PlatformID.Xbox:
                    return DeviceType.WindowsDesktop;
                default:
                    return DeviceType.UnknownBrowser;
            }

            //return DeviceType.ChromeBrowser;
        }

        public string GetDeviceString()
        {
            return Environment.OSVersion.ToString();
            //return "chrome";
        }

        public bool IsDev()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }

        public bool IsSelfHost() => false;

        public bool IsViewOpen() => false;

        public Task<bool> SupportsBiometricAsync() => Task.FromResult(false);

        public bool SupportsDuo() => false;

        public bool SupportsU2f() => false;

        public int? LockTimeout() => null;

        public void ShowToast(string type, string title, string text, Dictionary<string, object> options = null)
        {
            ShowToast(type, title, new string[] { text }, options);
        }

        public void ShowToast(string type, string title, string[] text, Dictionary<string, object> options = null)
        {
            Console.WriteLine("Toast({0}): {1}", type, title);

            if (options != null)
                Console.WriteLine("  Options: {0}", JsonSerializer.Serialize(options));

            foreach (var t in text)
                Console.WriteLine(t);
        }

        public Task<bool> ShowDialogAsync(string text, string title = null,
            string confirmText = null, string cancelText = null, string type = null)
        {
            throw new NotImplementedException();
        }

        public Task CopyToClipboardAsync(string text, Dictionary<string, object> options = null)
        {
            throw new NotImplementedException();
        }

        public Task<string> ReadFromClipboardAsync(Dictionary<string, object> options = null)
        {
            throw new NotImplementedException();
        }

        public void LaunchUri(string uri, Dictionary<string, object> options = null)
        {
            throw new NotImplementedException();
        }

        public void SaveFile()
        {
            throw new NotImplementedException();
        }

        public Task<bool> AuthenticateBiometricAsync(string text = null, string fallbackText = null, Action fallback = null)
        {
            throw new NotImplementedException();
        }
    }
}
