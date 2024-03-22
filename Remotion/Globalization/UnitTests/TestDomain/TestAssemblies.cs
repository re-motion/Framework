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
using NUnit.Framework;
using Remotion.Development.UnitTesting.Compilation;

namespace Remotion.Globalization.UnitTests.TestDomain
{
  public static class TestAssemblies
  {
    private const string c_testAssemblySourceDirectoryRoot = @"TestDomain\TestAssemblies";

    public static readonly Lazy<Assembly> Without = new Lazy<Assembly>(() => CompileTestAssemblyInMemory("Without"));
    public static readonly Lazy<Assembly> En = new Lazy<Assembly>(() => CompileTestAssemblyInMemory("En"));
    public static readonly Lazy<Assembly> EnUS = new Lazy<Assembly>(() => CompileTestAssemblyInMemory("EnUS"));

    private static Assembly CompileTestAssemblyInMemory (string assemblyName)
    {
      var assemblyCompiler = AssemblyCompiler.CreateInMemoryAssemblyCompiler(
          Path.Combine(TestContext.CurrentContext.TestDirectory, c_testAssemblySourceDirectoryRoot, assemblyName),
          Path.Combine(TestContext.CurrentContext.TestDirectory, typeof(MultiLingualNameAttribute).Module.Name),
          Path.Combine(TestContext.CurrentContext.TestDirectory, typeof(TestAssemblies).Module.Name));
      assemblyCompiler.Compile();
      return assemblyCompiler.CompiledAssembly;
    }
  }
}
