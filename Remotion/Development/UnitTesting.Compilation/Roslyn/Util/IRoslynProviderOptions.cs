// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-FileCopyrightText: (c) Microsoft Corporation All rights reserved.
// SPDX-License-Identifier: MIT

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
