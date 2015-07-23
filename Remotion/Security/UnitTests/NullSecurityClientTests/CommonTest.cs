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
using Remotion.Security.Metadata;

namespace Remotion.Security.UnitTests.NullSecurityClientTests
{
  [TestFixture]
  public class CommonTest
  {
    [Test]
    public void GetNullInstance ()
    {
      Assert.That (SecurityClient.Null, Is.InstanceOf<NullSecurityClient>());
    }

    [Test]
    public void GetNullInstance_ReturnsSameInstanceTwice ()
    {
      Assert.That (SecurityClient.Null, Is.SameAs (SecurityClient.Null));
    }

    [Test]
    public void Initialize ()
    {
      var securityClient = new NullSecurityClient();
      Assert.That (securityClient.SecurityProvider, Is.InstanceOf<NullSecurityProvider>());
      Assert.That (securityClient.PrincipalProvider, Is.InstanceOf<NullPrincipalProvider>());
      Assert.That (securityClient.PermissionProvider, Is.InstanceOf<PermissionReflector>());
      Assert.That (securityClient.MemberResolver, Is.InstanceOf<NullMemberResolver>());
      Assert.That (securityClient.FunctionalSecurityStrategy, Is.InstanceOf<NullFunctionalSecurityStrategy>());
    }
  }
}