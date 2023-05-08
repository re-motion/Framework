// Copyright (c) RUBICON IT GmbH, www.rubicon.eu
// Copyright (c) Microsoft Corporation All rights reserved.
// 
// MIT License
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the ""Software""), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED AS IS, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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
