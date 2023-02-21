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
