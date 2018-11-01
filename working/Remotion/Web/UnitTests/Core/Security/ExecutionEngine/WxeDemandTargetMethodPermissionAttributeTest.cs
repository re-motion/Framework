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
using Remotion.Web.Security.ExecutionEngine;
using Remotion.Web.UnitTests.Core.Security.Domain;

namespace Remotion.Web.UnitTests.Core.Security.ExecutionEngine
{
  [TestFixture]
  public class WxeDemandTargetMethodPermissionAttributeTest
  {
    [Test]
    public void Initialize_WithMethodName ()
    {
      WxeDemandTargetMethodPermissionAttribute attribute = new WxeDemandTargetMethodPermissionAttribute ("Show");

      Assert.That (attribute.MethodType, Is.EqualTo (MethodType.Instance));
      Assert.That (attribute.MethodName, Is.EqualTo ("Show"));
      Assert.That (attribute.SecurableClass, Is.Null);
    }

    [Test]
    public void Initialize_WithMethodNameAndSecurableClass ()
    {
      WxeDemandTargetMethodPermissionAttribute attribute = new WxeDemandTargetMethodPermissionAttribute ("Show", typeof (SecurableObject));

      Assert.That (attribute.MethodType, Is.EqualTo (MethodType.Instance));
      Assert.That (attribute.MethodName, Is.EqualTo ("Show"));
      Assert.That (attribute.SecurableClass, Is.SameAs (typeof (SecurableObject)));
    }

    [Test]
    public void Initialize_WithMethodNameEnum ()
    {
      WxeDemandTargetMethodPermissionAttribute attribute = new WxeDemandTargetMethodPermissionAttribute (SecurableObject.Method.Show);

      Assert.That (attribute.MethodType, Is.EqualTo (MethodType.Instance));
      Assert.That (attribute.MethodName, Is.EqualTo ("Show"));
      Assert.That (attribute.SecurableClass, Is.SameAs (typeof (SecurableObject)));
    }

    [Test]
    public void Initialize_WithMethodNameEnumAndSecurableClass ()
    {
      WxeDemandTargetMethodPermissionAttribute attribute = new WxeDemandTargetMethodPermissionAttribute (SecurableObject.Method.Show, typeof (DerivedSecurableObject));

      Assert.That (attribute.MethodType, Is.EqualTo (MethodType.Instance));
      Assert.That (attribute.MethodName, Is.EqualTo ("Show"));
      Assert.That (attribute.SecurableClass, Is.SameAs (typeof (DerivedSecurableObject)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), 
        ExpectedMessage = "Enumerated type 'Remotion.Web.UnitTests.Core.Security.Domain.MethodNameEnum' is not declared as a nested type.\r\nParameter name: methodNameEnum")]
    public void Initialize_WithMethodNameEnumNotNestedType ()
    {
      new WxeDemandTargetMethodPermissionAttribute (MethodNameEnum.Show);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The declaring type of enumerated type 'Remotion.Web.UnitTests.Core.Security.Domain.SimpleType+MethodNameEnum' does not implement interface"
        + " 'Remotion.Security.ISecurableObject'.\r\nParameter name: methodNameEnum")]
    public void Initialize_WithMethodNameEnumNotHavingValidDeclaringType ()
    {
      new WxeDemandTargetMethodPermissionAttribute (SimpleType.MethodNameEnum.Show);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "Type 'Remotion.Web.UnitTests.Core.Security.Domain.OtherSecurableObject' cannot be assigned to the declaring type of enumerated type"
        + " 'Remotion.Web.UnitTests.Core.Security.Domain.SecurableObject+Method'.\r\nParameter name: securableClass")]
    public void TestWithParameterNotOfNotMatchingType ()
    {
      new WxeDemandTargetMethodPermissionAttribute (SecurableObject.Method.Show, typeof (OtherSecurableObject));
    }
  }
}
