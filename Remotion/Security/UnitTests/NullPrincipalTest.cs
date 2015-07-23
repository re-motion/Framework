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
  public class NullPrincipalTest
  {
    private ISecurityPrincipal _principal;

    [SetUp]
    public void SetUp ()
    {
      _principal = new NullSecurityPrincipal();
    }

    [Test]
    public void Initialize ()
    {
      Assert.That (_principal.User, Is.Null);
      Assert.That (_principal.Role, Is.Null);
      Assert.That (_principal.SubstitutedUser, Is.Null);
      Assert.That (_principal.SubstitutedRole, Is.Null);
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (_principal.IsNull, Is.True);
    }
  }
}
