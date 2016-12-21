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

namespace Remotion.Security.UnitTests.SecurityClientTests
{
  [TestFixture]
  public class HasConstructorAccessTest
  {
    private SecurityClientTestHelper _testHelper;
    private SecurityClient _securityClient;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = new SecurityClientTestHelper ();
      _securityClient = _testHelper.CreateSecurityClient ();
    }

    [Test]
    public void Test_AccessGranted ()
    {
      _testHelper.ExpectFunctionalSecurityStrategyHasAccess (GeneralAccessTypes.Create, true);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasConstructorAccess (typeof (SecurableObject));

      _testHelper.VerifyAll ();
      Assert.That (hasAccess, Is.True);
    }

    [Test]
    public void Test_AccessDenied ()
    {
      _testHelper.ExpectFunctionalSecurityStrategyHasAccess (GeneralAccessTypes.Create, false);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasConstructorAccess (typeof (SecurableObject));

      _testHelper.VerifyAll ();
      Assert.That (hasAccess, Is.False);
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted ()
    {
      _testHelper.ReplayAll ();
      bool hasAccess;
      
      using (SecurityFreeSection.Activate())
      {
        hasAccess = _securityClient.HasConstructorAccess (typeof (SecurableObject));
      }

      _testHelper.VerifyAll ();
      Assert.That (hasAccess, Is.True);
    }
  }
}
