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
using System.Reflection.Emit;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.Reflection.UnitTests.CodeGeneration.MethodWrapperEmitterTests
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private static ModuleBuilder s_moduleBuilder;
    private static AssemblyBuilder s_assemblyBuilder;

    public static ModuleBuilder ModuleBuilder
    {
      get
      {
        if (s_moduleBuilder == null)
          throw new InvalidOperationException("SetUp must be called before the scope is accessed.");
        return s_moduleBuilder;
      }
    }

    [OneTimeSetUp]
    public virtual void OneTimeSetUp ()
    {
      Console.WriteLine("Setting up MethodWrapperEmitterTests");

      var assemblyName = new AssemblyName("Remotion.Reflection.CodeGeneration.MethodWrapperEmitterTests.Generated.Unsigned");
      var moduleName = assemblyName + ".dll";

      s_assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
      s_moduleBuilder = s_assemblyBuilder.DefineDynamicModule(moduleName);

      DeleteIfExists(s_moduleBuilder.FullyQualifiedName);
    }

    [OneTimeTearDown]
    public virtual void OneTimeTearDown ()
    {
      Console.WriteLine("Tearing down MethodWrapperEmitterTests");
      //TODO RM-9285: Re-Introduce assembly verification
    }

    private void DeleteIfExists (string path)
    {
      if (File.Exists(path))
        FileUtility.DeleteAndWaitForCompletion(path);
    }
  }
}
