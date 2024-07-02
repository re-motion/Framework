// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-FileCopyrightText: (c) Microsoft Corporation All rights reserved.
// SPDX-License-Identifier: MIT
#nullable disable

// Source code from https://github.com/aspnet/RoslynCodeDomProvider
// Checkout commit:  213c6bdf00e7f56557024e3b2b1287f8529c7155

using System;
using System.Collections.Specialized;
using System.Configuration;

namespace Remotion.Development.UnitTesting.Compilation.Roslyn {
    internal static class RoslynAppSettings {
        private static volatile bool _settingsInitialized;
        private static object _lock = new object();

        private static void LoadSettings(NameValueCollection appSettings) {
            string disableProfilingDuringCompilation = appSettings["aspnet:DisableProfilingDuringCompilation"];

            if (!bool.TryParse(disableProfilingDuringCompilation, out _disableProfilingDuringCompilation)) {
                _disableProfilingDuringCompilation = true;
            }

            _roslynCompilerLocation =  appSettings["aspnet:RoslynCompilerLocation"];
        }

        private static void EnsureSettingsLoaded() {
            if (_settingsInitialized) {
                return;
            }

            lock (_lock) {
                if (!_settingsInitialized) {
                    try {
                        // I think it should be safe to straight up use regular ConfigurationManager here...
                        // but if it ain't broke, don't fix it.
                        LoadSettings(ConfigurationManager.AppSettings);
                        
                    }
                    finally {
                        _settingsInitialized = true;
                    }
                }
            }
        }

        private static bool _disableProfilingDuringCompilation = true;
        public static bool DisableProfilingDuringCompilation {
            get {
                EnsureSettingsLoaded();
                return _disableProfilingDuringCompilation;
            }
        }

        private static string _roslynCompilerLocation = string.Empty;
        public static string RoslynCompilerLocation {
            get {
                EnsureSettingsLoaded();
                return _roslynCompilerLocation;
            }
        }
    }
}
