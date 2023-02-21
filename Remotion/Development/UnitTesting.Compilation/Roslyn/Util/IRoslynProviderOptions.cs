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

// Source code from https://github.com/aspnet/RoslynCodeDomProvider
// Checkout commit:  213c6bdf00e7f56557024e3b2b1287f8529c7155

using System;
using System.Collections.Generic;

namespace Remotion.Development.UnitTesting.Compilation.Roslyn.Util {

#pragma warning disable CS0618

	/// <summary>
	/// Provides settings for the C# and VB CodeProviders
	/// </summary>
	internal interface IRoslynProviderOptions
	{
		///// <summary>
		///// The full path to csc.exe or vbc.exe
		///// </summary>
		string CompilerFullPath { get; }

		///// <summary>
		///// TTL in seconds
		///// </summary>
		int CompilerServerTimeToLive { get; }

		/// <summary>
		/// A string representing the in-box .Net Framework compiler version to be used.
		/// Not applicable to this Roslyn-based package which contains it's own compiler.
		/// </summary>
		string CompilerVersion { get; }

		/// <summary>
		/// Returns true if the codedom provider has warnAsError set to true
		/// </summary>
		bool WarnAsError { get; }

		/// <summary>
		/// Returns true if the codedom provider is requesting to use similar default
		/// compiler options as ASP.Net does with in-box .Net Framework compilers.
		/// These options are programatically enforced on top of parameters passed
		/// in to the codedom provider.
		/// </summary>
		bool UseAspNetSettings { get; }

		/// <summary>
		/// Returns the entire set of options - known or not - as configured in &lt;providerOptions&gt;
		/// </summary>
		IDictionary<string, string> AllOptions { get; }
	}
#pragma warning restore CS0618

}
