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
using Castle.DynamicProxy;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.Utilities;

namespace Remotion.Reflection.CodeGeneration.UnitTests
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private static ModuleScope s_scope;
    private static ModuleScope s_unsavedScope;

    public static ModuleScope Scope
    {
      get
      {
        if (s_scope == null)
          throw new InvalidOperationException ("SetUp must be called before the scope is accessed.");
        return s_scope;
      }
    }

    public static ModuleScope UnsavedScope
    {
      get
      {
        if (s_scope == null)
          throw new InvalidOperationException ("SetUp must be called before the scope is accessed.");
        return s_unsavedScope;
      }
    }

    [SetUp]
    public virtual void SetUp ()
    {
      Console.WriteLine ("Setting up code generation tests");
      s_scope = new ModuleScope (true, false, "Remotion.Reflection.CodeGeneration.Generated.Signed", "Remotion.Reflection.CodeGeneration.Generated.Signed.dll", "Remotion.Reflection.CodeGeneration.Generated.Unsigned", "Remotion.Reflection.CodeGeneration.Generated.Unsigned.dll");
      s_unsavedScope = new ModuleScope (true);
      DeleteIfExists (Path.Combine (s_scope.StrongNamedModuleDirectory ?? Environment.CurrentDirectory, s_scope.StrongNamedModuleName));
      DeleteIfExists (Path.Combine (s_scope.WeakNamedModuleDirectory ?? Environment.CurrentDirectory, s_scope.WeakNamedModuleName));
    }

    [TearDown]
    public virtual void TearDown ()
    {
      Console.WriteLine ("Tearing down code generation tests");
#if !NO_PEVERIFY
      string[] paths = AssemblySaver.SaveAssemblies (s_scope);
      s_scope = null;
      s_unsavedScope = null;

      foreach (string path in paths)
      {
        PEVerifier.CreateDefault ().VerifyPEFile (path);
        FileUtility.DeleteAndWaitForCompletion (path);
        FileUtility.DeleteAndWaitForCompletion (path.Replace (".dll", ".pdb"));
      }
#endif
    }

    private void DeleteIfExists (string path)
    {
      if (File.Exists (path))
        FileUtility.DeleteAndWaitForCompletion (path);
    }
  }
}
