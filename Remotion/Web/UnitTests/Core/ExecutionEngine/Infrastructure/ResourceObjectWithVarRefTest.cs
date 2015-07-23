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
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure
{
  [TestFixture]
  public class ResourceObjectWithVarRefTest
  {
    private NameObjectCollection _variables;

    [SetUp]
    public void SetUp ()
    {
      _variables = new NameObjectCollection { { "ThePath", "path.aspx" } };
    }

    [Test]
    public void InitializeWithResourceAssembly_WithoutAssembly ()
    {
      var resourceObject = new ResourceObjectWithVarRef (new WxeVariableReference ("ThePage"));

      Assert.That (resourceObject.ResourceRoot, Is.EqualTo ("~/"));
      Assert.That (resourceObject.PathReference.Name, Is.EqualTo ("ThePage"));
    }

    [Test]
    public void InitializeWithResourceAssembly_WithAssembly ()
    {
      var resourceObject = new ResourceObjectWithVarRef (new FakeResourcePathBuilder(), GetType().Assembly, new WxeVariableReference ("ThePage"));

      Assert.That (resourceObject.ResourceRoot, Is.EqualTo ("/fake/Remotion.Web.UnitTests"));
      Assert.That (resourceObject.PathReference.Name, Is.EqualTo ("ThePage"));
    }

    [Test]
    public void GetResourcePath_WithoutAssembly ()
    {
      var resourceObject = new ResourceObjectWithVarRef (new WxeVariableReference ("ThePath"));

      Assert.That (resourceObject.GetResourcePath (_variables), Is.EqualTo ("~/path.aspx"));
    }

    [Test]
    public void GetResourcePath_WithAssembly ()
    {
      var resourceObject = new ResourceObjectWithVarRef (new FakeResourcePathBuilder(), GetType().Assembly, new WxeVariableReference ("ThePath"));

      Assert.That (resourceObject.GetResourcePath (_variables), Is.EqualTo ("/fake/Remotion.Web.UnitTests/path.aspx"));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The variable 'InvalidIdentifier' could not be found in the list of variables.")]
    public void GetResourcePath_WithInvalidReference ()
    {
      var resourceObject = new ResourceObjectWithVarRef (new WxeVariableReference ("InvalidIdentifier"));

      resourceObject.GetResourcePath (_variables);
    }

    [Test]
    [ExpectedException (typeof (InvalidCastException), ExpectedMessage =
        "The variable 'InvalidType' was of type 'System.Int32'. Expected type is 'System.String'.")]
    public void GetResourcePath_WithInvalidTypeInVariable ()
    {
      _variables.Add ("InvalidType", 1);
      var resourceObject = new ResourceObjectWithVarRef (new WxeVariableReference ("InvalidType"));

      resourceObject.GetResourcePath (_variables);
    }
  }
}