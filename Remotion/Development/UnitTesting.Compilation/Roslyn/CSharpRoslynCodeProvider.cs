// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-FileCopyrightText: (c) Microsoft Corporation All rights reserved.
// SPDX-License-Identifier: MIT
#nullable disable

// Source code from https://github.com/aspnet/RoslynCodeDomProvider
// Checkout commit:  213c6bdf00e7f56557024e3b2b1287f8529c7155

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Remotion.Development.UnitTesting.Compilation.Roslyn.Util;

namespace Remotion.Development.UnitTesting.Compilation.Roslyn {
    /// <summary>
    /// Provides access to instances of the .NET Compiler Platform C# code generator and code compiler.
    /// </summary>
    [DesignerCategory("code")]
    internal sealed class CSharpRoslynCodeProvider : Microsoft.CSharp.CSharpCodeProvider {
        private readonly IRoslynProviderOptions _providerOptions;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public CSharpRoslynCodeProvider()
            : this(null) {
        }

        /// <summary>
        /// Creates an instance using the given IProviderOptions
        /// </summary>
        /// <param name="providerOptions"></param>
        public CSharpRoslynCodeProvider(IRoslynProviderOptions providerOptions = null) {
            _providerOptions = providerOptions == null ? Util.RoslynCompilationUtil.CSC2 : providerOptions;
        }

        /// <summary>
        /// Gets an instance of the .NET Compiler Platform C# code compiler.
        /// </summary>
        /// <returns>An instance of the .NET Compiler Platform C# code compiler</returns>
        [Obsolete("Callers should not use the ICodeCompiler interface and should instead use the methods directly on the CodeDomProvider class.")]
        public override ICodeCompiler CreateCompiler() {
            return new CSharpRoslynCompiler(this, _providerOptions);
        }
    }
}
