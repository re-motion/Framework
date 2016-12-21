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
using Remotion.Security.UnitTests.SampleDomain;

namespace Remotion.Security.UnitTests
{
  [TestFixture]
  public class DemandPermissionAttributeTest
  {
    [Test]
    public void AcceptValidAccessType ()
    {
      var methodPermissionAttribute = new DemandPermissionAttribute (TestAccessTypes.Second);
      Assert.That (methodPermissionAttribute.GetAccessTypes()[0], Is.EqualTo (TestAccessTypes.Second));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "Enumerated Type 'Remotion.Security.UnitTests.SampleDomain.TestAccessTypesWithoutAccessTypeAttribute' cannot be used as an access type. "
        + "Valid access types must have the Remotion.Security.AccessTypeAttribute applied.\r\nParameter name: accessType")]
    public void RejectAccessTypeWithoutAccessTypeAttribute ()
    {
      new DemandPermissionAttribute (TestAccessTypesWithoutAccessTypeAttribute.First);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Item 0 of parameter 'accessTypes' has the type 'Remotion.Security.UnitTests.SampleDomain.SimpleType' instead of 'System.Enum'."
        + "\r\nParameter name: accessTypes")]
    public void RejectOtherObjectTypes ()
    {
      new DemandPermissionAttribute (new SimpleType());
    }

    [Test]
    public void AcceptMultipleAccessTypes ()
    {
      var methodPermissionAttribute = new DemandPermissionAttribute (TestAccessTypes.Second, TestAccessTypes.Fourth);

      Assert.That (methodPermissionAttribute.GetAccessTypes().Length, Is.EqualTo (2));
      Assert.That (methodPermissionAttribute.GetAccessTypes(), Has.Member (TestAccessTypes.Second));
      Assert.That (methodPermissionAttribute.GetAccessTypes(), Has.Member (TestAccessTypes.Fourth));
    }
  }
}
