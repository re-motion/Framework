// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it
// and/or modify it under the terms of the GNU Lesser General Public License
// as published by the Free Software Foundation; either version 2.1 of the
// License, or (at your option) any later version.
//
// re-motion is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
//
using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.CSharp.Testing.NUnit;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Remotion.Context;

namespace Remotion.Core.Development.Analyzers.IntegrationTests
{
  public static class CSharpAnalyzerVerifier<TAnalyzer>
      where TAnalyzer : DiagnosticAnalyzer, new()
  {
    private class Test : CSharpAnalyzerTest<TAnalyzer, NUnitVerifier>
    {
    }

    public static DiagnosticResult Diagnostic (DiagnosticDescriptor desc) => AnalyzerVerifier<TAnalyzer>.Diagnostic(desc);

    public static Task VerifyAnalyzerAsync (string source, bool withSafeContextReference, params DiagnosticResult[] expected )
    {
      var contextAssemblyLocation = typeof(SafeContext).Assembly.Location;

      var test = new Test
                 {
                     TestCode = source,
                     ReferenceAssemblies = ReferenceAssemblies.Net.Net60,
                     SolutionTransforms =
                     {
                         (solution, id) =>
                         {
                           var project = solution.GetProject(id);
                           if (withSafeContextReference)
                           {
                             project = project
                                 .AddMetadataReference(MetadataReference.CreateFromFile(contextAssemblyLocation));
                           }
                           return project.Solution;
                         }
                     }
                 };
      test.ExpectedDiagnostics.AddRange(expected);

      return test.RunAsync();
    }
  }
}
