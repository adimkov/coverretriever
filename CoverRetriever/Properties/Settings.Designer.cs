﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CoverRetriever.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://ajax.googleapis.com/ajax/services/search/images")]
        public string SearhGoogle {
            get {
                return ((string)(this["SearhGoogle"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("{0} - {1}&imgtype=photo")]
        public string SearhGooglePattern {
            get {
                return ((string)(this["SearhGooglePattern"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ABQIAAAAyJOVurPXLFVPtU1RN7FF_RT1b26EPce9DeOid2McjHSSSzE4jRRSIw3t1tJbbma6w0bpHwFki" +
            "780Aw")]
        public string KeyGoogle {
            get {
                return ((string)(this["KeyGoogle"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("./CoverRetriever.cache")]
        public string CacheFilePath {
            get {
                return ((string)(this["CacheFilePath"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("d33202221533846659200a867cafcc81")]
        public string LastFmApiKey {
            get {
                return ((string)(this["LastFmApiKey"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://ws.audioscrobbler.com/2.0")]
        public string LastFmServiceBaseAddress {
            get {
                return ((string)(this["LastFmServiceBaseAddress"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("./LfmFpClient/lastfmfpclient.exe")]
        public string LastFmFingerprintClientPath {
            get {
                return ((string)(this["LastFmFingerprintClientPath"]));
            }
        }
    }
}
