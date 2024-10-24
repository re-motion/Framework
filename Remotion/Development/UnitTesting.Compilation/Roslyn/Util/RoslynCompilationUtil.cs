// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-FileCopyrightText: (c) Microsoft Corporation All rights reserved.
// SPDX-License-Identifier: MIT
#nullable disable

// Source code from https://github.com/aspnet/RoslynCodeDomProvider
// Checkout commit:  213c6bdf00e7f56557024e3b2b1287f8529c7155

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Remotion.Development.UnitTesting.Compilation.Roslyn.Util {
    internal static class RoslynCompilationUtil {
        private const int DefaultCompilerServerTTL = 10; // 10 seconds
        private const int DefaultCompilerServerTTLInDevEnvironment = 60 * 15; // 15 minutes

        static RoslynCompilationUtil()
        {
            CSC2 = GetProviderOptionsFor(".cs");
            VBC2 = GetProviderOptionsFor(".vb");

            if (IsDebuggerAttached)
            {
                Environment.SetEnvironmentVariable("IN_DEBUG_MODE", "1", EnvironmentVariableTarget.Process);
            }
        }

        public static IRoslynProviderOptions CSC2 { get; }

        public static IRoslynProviderOptions VBC2 { get; }

        public static IRoslynProviderOptions GetProviderOptionsFor(string fileExt)
        {
            //
            // AllOptions
            //
            IDictionary<string, string> options = GetProviderOptionsCollection(fileExt);

            //
            // CompilerFullPath
            //
            string compilerFullPath = Environment.GetEnvironmentVariable("ROSLYN_COMPILER_LOCATION");
            if (String.IsNullOrEmpty(compilerFullPath))
                compilerFullPath = RoslynAppSettings.RoslynCompilerLocation;
            if (String.IsNullOrEmpty(compilerFullPath))
                options.TryGetValue("CompilerLocation", out compilerFullPath);
            if (String.IsNullOrEmpty(compilerFullPath))
                compilerFullPath = CompilerFullPath(@"roslyn");

            if (fileExt.Equals(".cs", StringComparison.InvariantCultureIgnoreCase))
                compilerFullPath = Path.Combine(compilerFullPath, "csc.exe");
            else if (fileExt.Equals(".vb", StringComparison.InvariantCultureIgnoreCase))
                compilerFullPath = Path.Combine(compilerFullPath, "vbc.exe");


            //
            // CompilerServerTimeToLive - default 10 seconds in production, 15 minutes in dev environment.
            //
            int ttl;
            string ttlstr = Environment.GetEnvironmentVariable("VBCSCOMPILER_TTL");
            if (String.IsNullOrEmpty(ttlstr))
                options.TryGetValue("CompilerServerTTL", out ttlstr);
            if (!Int32.TryParse(ttlstr, out ttl)) {
                ttl = DefaultCompilerServerTTL;

                if (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("DEV_ENVIRONMENT")) ||
                    !String.IsNullOrEmpty(Environment.GetEnvironmentVariable("IN_DEBUG_MODE")) ||
                    RoslynCompilationUtil.IsDebuggerAttached)
                {
                    ttl = DefaultCompilerServerTTLInDevEnvironment;
                }
            }

            //
            // CompilerVersion - if this is null, we don't care.
            //
            string compilerVersion;
            options.TryGetValue("CompilerVersion", out compilerVersion);    // Failure to parse sets to null

            //
            // WarnAsError - default false.
            //
            bool warnAsError = false;
            if (options.TryGetValue("WarnAsError", out string sWAE)) {
                Boolean.TryParse(sWAE, out warnAsError); // Failure to parse sets to 'false'
            }

            //
            // UseAspNetSettings - default true. This was meant to be an ASP.Net support package first and foremost.
            //
            bool useAspNetSettings = true;
            if (options.TryGetValue("UseAspNetSettings", out string sUANS))
            {
                // Failure to parse sets to 'false', but we want to keep the default 'true'.
                if (!Boolean.TryParse(sUANS, out useAspNetSettings))
                    useAspNetSettings = true;
            }

            RoslynProviderOptions providerOptions = new RoslynProviderOptions() {
                CompilerFullPath = compilerFullPath,
                CompilerServerTimeToLive = ttl,
                CompilerVersion = compilerVersion,
                WarnAsError = warnAsError,
                UseAspNetSettings = useAspNetSettings,
                AllOptions = options
            };

            return providerOptions;
        }

        internal static IDictionary<string, string> GetProviderOptionsCollection(string fileExt)
        {
            Dictionary<string, string> opts = new Dictionary<string, string>();

            if (!CodeDomProvider.IsDefinedExtension(fileExt))
                return new ReadOnlyDictionary<string, string>(opts);

            CompilerInfo ci = CodeDomProvider.GetCompilerInfo(CodeDomProvider.GetLanguageFromExtension(fileExt));

            if (ci == null)
                return new ReadOnlyDictionary<string, string>(opts);

            // There is a fun little comment about this property in the framework code about making it
            // public after 3.5. Guess that didn't happen. Oh well. :)
            PropertyInfo pi = ci.GetType().GetProperty("ProviderOptions",
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
            if (pi == null)
                return new ReadOnlyDictionary<string, string>(opts);

            return new ReadOnlyDictionary<string, string>((IDictionary<string, string>)pi.GetValue(ci, null));
        }

        internal static void PrependCompilerOption(CompilerParameters compilParams, string compilerOptions)
        {
            if (compilParams.CompilerOptions == null)
            {
                compilParams.CompilerOptions = compilerOptions;
            }
            else
            {
                compilParams.CompilerOptions = compilerOptions + " " + compilParams.CompilerOptions;
            }
        }

        internal static string CompilerFullPath(string relativePath)
        {
            string compilerFullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            return compilerFullPath;
        }

        internal static bool IsDebuggerAttached
        {
            get {
                return IsDebuggerPresent() || Debugger.IsAttached;
            }
        }

        [DllImport("kernel32.dll")]
        private extern static bool IsDebuggerPresent();
    }
}
