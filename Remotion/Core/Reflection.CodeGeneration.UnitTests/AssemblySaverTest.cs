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
using Castle.DynamicProxy;
using NUnit.Framework;
using Remotion.Reflection.CodeGeneration.DPExtensions;

namespace Remotion.Reflection.CodeGeneration.UnitTests
{
  [TestFixture]
  public class AssemblySaverTest
  {
    [Test]
    public void SaveAssemblyNoGeneratedTypes ()
    {
      ModuleScope scope = new ModuleScope (true);
      string[] paths = AssemblySaver.SaveAssemblies (scope);
      Assert.That (paths.Length, Is.EqualTo (0));
    }

    [Test]
    public void SaveAssemblySigned ()
    {
      ModuleScope scope = new ModuleScope (true);
      CustomClassEmitter emitter = new CustomClassEmitter (scope, "SignedType", typeof (object));
      emitter.BuildType ();
      string[] paths = AssemblySaver.SaveAssemblies (scope);
      Assert.That (paths.Length, Is.EqualTo (1));
      Assert.That (paths[0], Is.EqualTo (Path.Combine (Environment.CurrentDirectory, scope.StrongNamedModuleName)));
      File.Delete (paths[0]);
      File.Delete (paths[0].Replace (".dll", ".pdb"));
    }

    [Test]
    public void SaveAssemblyUnsigned ()
    {
      ModuleScope scope = new ModuleScope (true);
      CustomClassEmitter emitter = new CustomClassEmitter (scope, "UnsignedType", typeof (object), Type.EmptyTypes, TypeAttributes.Public, true);
      emitter.BuildType ();
      string[] paths = AssemblySaver.SaveAssemblies (scope);
      Assert.That (paths.Length, Is.EqualTo (1));
      Assert.That (paths[0], Is.EqualTo (Path.Combine (Environment.CurrentDirectory, scope.WeakNamedModuleName)));
      File.Delete (paths[0]);
      File.Delete (paths[0].Replace (".dll", ".pdb"));
    }
  }
}
