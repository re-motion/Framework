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
using Microsoft.CodeAnalysis.CSharp.Testing.NUnit;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Remotion.Web.Development.Analyzers.IntegrationTests
{
  public static class CSharpAnalyzerVerifier<TAnalyzer>
      where TAnalyzer : DiagnosticAnalyzer, new()
  {
    private class Test : CSharpAnalyzerTest<TAnalyzer, NUnitVerifier>
    {
    }

    private static readonly Lazy<ReferenceAssemblies> s_net80 =
        new(() => new ReferenceAssemblies("net8.0", new PackageIdentity("Microsoft.NETCore.App.Ref", "8.0.0"), Path.Combine("ref", "net8.0")));

    public static DiagnosticResult Diagnostic () => AnalyzerVerifier<TAnalyzer>.Diagnostic();

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
          // RM-8930 Previous syntax for when the assemblies were easily accessible: ".NETCoreApp,Version=v6.0" => ReferenceAssemblies.Net.Net60
          ".NETCoreApp,Version=v8.0" => s_net80.Value,
          var frameworkName => throw new NotSupportedException($"'{frameworkName}' is not supported.")
      };
    }
  }
}
