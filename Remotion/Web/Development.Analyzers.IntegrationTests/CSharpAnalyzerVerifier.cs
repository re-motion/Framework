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
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Web.UI;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace Remotion.Web.Development.Analyzers.IntegrationTests
{
  public static class CSharpAnalyzerVerifier<TAnalyzer>
      where TAnalyzer : DiagnosticAnalyzer, new()
  {
    private class Test : CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
    {
    }

    // Workaround when .NET version is not yet available as a ReferenceAsseblies entry.
    private static readonly Lazy<ReferenceAssemblies> s_net100 =
        new(() => new ReferenceAssemblies("net10.0", new PackageIdentity("Microsoft.NETCore.App.Ref", "10.0.0"), Path.Combine("ref", "net10.0")));

    public static DiagnosticResult Diagnostic () => CSharpAnalyzerVerifier<TAnalyzer, DefaultVerifier>.Diagnostic();

    public static DiagnosticResult Diagnostic (DiagnosticDescriptor descriptor) => CSharpAnalyzerVerifier<TAnalyzer, DefaultVerifier>.Diagnostic(descriptor);

    public static Task VerifyAnalyzerAsync (string source, params DiagnosticResult[] expected)
    {
      var remotionWebAssemblyLocation = typeof(WebString).Assembly.Location;
      var coreWebAssemblyLocation = typeof(HtmlTextWriter).Assembly.Location;

      var test = new Test
                 {
                     TestCode = source,
                     ReferenceAssemblies = GetReferenceAssemblies(typeof(WebString).Assembly),
                     SolutionTransforms = { (solution, id) =>
                     {
                       var project = solution.GetProject(id);
                       project = project
                           .AddMetadataReference(MetadataReference.CreateFromFile(remotionWebAssemblyLocation))
                           .AddMetadataReference(MetadataReference.CreateFromFile(coreWebAssemblyLocation));
                       return project.Solution;
                     } }
                 };
      test.ExpectedDiagnostics.AddRange(expected);

      return test.RunAsync();
    }

    private static ReferenceAssemblies GetReferenceAssemblies (Assembly assembly)
    {
      return assembly.GetCustomAttribute<TargetFrameworkAttribute>()!.FrameworkName switch
      {
          ".NETCoreApp,Version=v8.0" => ReferenceAssemblies.Net.Net80,
          ".NETCoreApp,Version=v9.0" => ReferenceAssemblies.Net.Net90,
          ".NETCoreApp,Version=v10.0" => s_net100.Value,
          var frameworkName => throw new NotSupportedException($"'{frameworkName}' is not supported.")
      };
    }
  }
}
