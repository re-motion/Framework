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
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text;
using Remotion.Development.UnitTesting.Compilation.Roslyn;
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting.Compilation
{
  public class AssemblyCompiler
  {
    public static AssemblyCompiler CreateInMemoryAssemblyCompiler (string sourceDirectory, params string[] referencedAssemblies)
    {
      return new AssemblyCompiler(sourceDirectory, referencedAssemblies);
    }

    private readonly string _sourceDirectory;
    private CompilerResults? _results;
    private readonly CompilerParameters _compilerParameters;

    public AssemblyCompiler (string sourceDirectory, string outputAssembly, params string[] referencedAssemblies)
    {
      ArgumentUtility.CheckNotNullOrEmpty("sourceDirectory", sourceDirectory);
      ArgumentUtility.CheckNotNullOrEmpty("outputAssembly", outputAssembly);
      ArgumentUtility.CheckNotNullOrItemsNull("referencedAssemblies", referencedAssemblies);

      _sourceDirectory = sourceDirectory;

      _compilerParameters = new CompilerParameters();
      _compilerParameters.GenerateExecutable = false;
      _compilerParameters.OutputAssembly = outputAssembly;
      _compilerParameters.GenerateInMemory = false;
      _compilerParameters.TreatWarningsAsErrors = false;
      _compilerParameters.ReferencedAssemblies.AddRange(referencedAssemblies);
    }

    private AssemblyCompiler (string sourceDirectory, params string[] referencedAssemblies)
    {
      ArgumentUtility.CheckNotNullOrEmpty("sourceDirectory", sourceDirectory);
      ArgumentUtility.CheckNotNullOrItemsNull("referencedAssemblies", referencedAssemblies);

      _sourceDirectory = sourceDirectory;

      _compilerParameters = new CompilerParameters();
      _compilerParameters.GenerateExecutable = false;
      _compilerParameters.OutputAssembly = null!;
      _compilerParameters.GenerateInMemory = true;
      _compilerParameters.TreatWarningsAsErrors = false;
      _compilerParameters.ReferencedAssemblies.AddRange(referencedAssemblies);
    }

    public CompilerParameters CompilerParameters
    {
      get { return _compilerParameters; }
    }

    public Assembly? CompiledAssembly
    {
      get { return _results != null ? _results.CompiledAssembly : null; }
    }

    public CompilerResults? Results
    {
      get { return _results; }
    }

    public string OutputAssemblyPath
    {
      get { return _compilerParameters.OutputAssembly; }
    }

    public void Compile ()
    {
      CodeDomProvider provider = new CSharpRoslynCodeProvider();

      string[] sourceFiles = Directory.GetFiles(_sourceDirectory, "*.cs");

      string[] resourceFiles = Directory.GetFiles(_sourceDirectory, "*.resources");
      if (provider.Supports(GeneratorSupport.Resources))
        _compilerParameters.EmbeddedResources.AddRange(resourceFiles);

      _results = provider.CompileAssemblyFromFile(_compilerParameters, sourceFiles);

      if (_results.Errors.Count > 0)
      {
        StringBuilder errorBuilder = new StringBuilder();
        errorBuilder.AppendFormat("Errors building {0} into {1}", _sourceDirectory, _results.PathToAssembly).AppendLine();
        foreach (CompilerError compilerError in _results.Errors)
          errorBuilder.AppendFormat("  ").AppendLine(compilerError.ToString());

        throw new AssemblyCompilationException(errorBuilder.ToString());
      }
    }
  }
}
