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

namespace Remotion.Security.UnitTests
{
  [TestFixture]
  public class NullSecurityProviderTest
  {
    private ISecurityProvider _securityProvider;

    [SetUp]
    public void SetUp ()
    {
      _securityProvider = new NullSecurityProvider();
    }

    [Test]
    public void GetAccess_ReturnsEmptyList ()
    {
      AccessType[] accessTypes = _securityProvider.GetAccess (null, null);
      Assert.That (accessTypes, Is.Not.Null);
      Assert.That (accessTypes.Length, Is.EqualTo (0));
    }

    [Test]
    public void GetIsNull ()
    {
      Assert.That (_securityProvider.IsNull, Is.True);
    }

    [Test]
    public void GetHashcode_DifferentInstancesAreEqual ()
    {
      Assert.That (new NullSecurityPrincipal().GetHashCode(), Is.EqualTo (new NullSecurityPrincipal().GetHashCode()));
    }

    [Test]
    public void Equals_DifferentInstancesAreEqual ()
    {
      Assert.That (new NullSecurityPrincipal().Equals (new NullSecurityPrincipal()), Is.True);
    }
  }
}