﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.CodeDom;
using NUnit.Framework;
using Remotion.Web.Compilation;

namespace Remotion.Web.UnitTests.Core.Compilation
{
  [TestFixture]
  public class DefaultControlBuilderCodeProcessorTest
  {
    [Test]
    public void ExecutesBaseCall ()
    {
      var processor = new DefaultControlBuilderCodeProcessor();

      var unit = new CodeCompileUnit();
      var baseType = new CodeTypeDeclaration();
      var derivedType = new CodeTypeDeclaration();
      var buildMethod = new CodeMemberMethod();
      var dataBindingMethod = new CodeMemberMethod();
      bool called = false;

      Action<CodeCompileUnit, CodeTypeDeclaration, CodeTypeDeclaration, CodeMemberMethod, CodeMemberMethod> baseCall =
          (pUnit, pBaseType, pDerivedType, pBuildMethod, pDataBindingMethod) =>
          {
            Assert.That (pUnit, Is.SameAs (unit));
            Assert.That (pBaseType, Is.SameAs (baseType));
            Assert.That (pDerivedType, Is.SameAs (derivedType));
            Assert.That (pBuildMethod, Is.SameAs (buildMethod));
            Assert.That (pDataBindingMethod, Is.SameAs (dataBindingMethod));
            called = true;
          };

      processor.ProcessGeneratedCode (unit, baseType, derivedType, buildMethod, dataBindingMethod, baseCall);
      Assert.That (called, Is.True);
    }
  }
}