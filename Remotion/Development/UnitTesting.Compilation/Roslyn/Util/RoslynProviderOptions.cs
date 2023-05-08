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
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remotion.Development.UnitTesting.Compilation.Roslyn.Util {

    /// <summary>
    /// A set of options for the C# and VB CodeProviders.
    /// </summary>
    internal sealed class RoslynProviderOptions : IRoslynProviderOptions {

        private IDictionary<string, string> _allOptions;

        /// <summary>
        /// Create a default set of options for the C# and VB CodeProviders.
        /// </summary>
        public RoslynProviderOptions()
        {
            this.CompilerFullPath = null;
            this.CompilerVersion = null;
            this.WarnAsError = false;

            // To be consistent, make sure there is always a dictionary here. It is less than ideal
            // for some parts of code to be checking AllOptions.count and some part checking
            // for AllOptions == null.
            this.AllOptions = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());

            // This results in no keep-alive for the compiler. This will likely result in
            // slower performance for any program that calls out the the compiler any
            // significant number of times. This is why the CompilerUtil.GetProviderOptionsFor
            // does not leave this as 0.
            this.CompilerServerTimeToLive = 0;

            // This is different from the default that the CompilerUtil.GetProviderOptionsFor
            // factory method uses. The primary known user of the factory method is us, and
            // this package is first intended to support ASP.Net. However, if somebody is
            // creating an instance of this directly, they are probably not an ASP.Net
            // project. Thus the different default here.
            this.UseAspNetSettings = false; 
        }

        /// <summary>
        /// Create a set of options for the C# or VB CodeProviders using the specified inputs.
        /// </summary>
        public RoslynProviderOptions(IRoslynProviderOptions opts)
        {
            this.CompilerFullPath = opts.CompilerFullPath;
            this.CompilerServerTimeToLive = opts.CompilerServerTimeToLive;
            this.CompilerVersion = opts.CompilerVersion;
            this.WarnAsError = opts.WarnAsError;
            this.UseAspNetSettings = opts.UseAspNetSettings;
            this.AllOptions = new ReadOnlyDictionary<string, string>(opts.AllOptions);
        }

        /// <summary>
        /// Create a set of options for the C# or VB CodeProviders using some specified inputs.
        /// </summary>
        public RoslynProviderOptions(string compilerFullPath, int compilerServerTimeToLive) : this()
        {
            this.CompilerFullPath = compilerFullPath;
            this.CompilerServerTimeToLive = compilerServerTimeToLive;
        }

        /// <summary>
        /// The full path to csc.exe or vbc.exe
        /// </summary>
        public string CompilerFullPath { get; internal set; }

        /// <summary>
        /// TTL in seconds
        /// </summary>
        public int CompilerServerTimeToLive { get; internal set; }

        /// <summary>
        /// Used by in-box framework code providers to determine which compat version of the compiler to use.
        /// </summary>
        public string CompilerVersion { get; internal set; }

        // smolloy todo debug degub - Does it really override everything? Is that the right thing to do?
        /// <summary>
        /// Treat all warnings as errors. Will override defaults and command-line options given for a compiler.
        /// </summary>
        public bool WarnAsError { get; internal set; }

        /// <summary>
        /// Use the set of compiler options that was traditionally added programatically for ASP.Net.
        /// </summary>
        public bool UseAspNetSettings { get; internal set; }

        /// <summary>
        /// A collection of all &lt;providerOptions&gt; specified in config for the given CodeDomProvider.
        /// </summary>
        public IDictionary<string, string> AllOptions {
            get {
                return _allOptions;
            }
            internal set {
                _allOptions = (value != null) ? new ReadOnlyDictionary<string, string>(value) : null;
            }
        }
    }
}
