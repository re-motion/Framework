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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Security.UnitTests.SampleDomain;

namespace Remotion.Security.UnitTests.SecurityClientTests
{
  [TestFixture]
  public class HasAccessTest
  {
    private SecurityClientTestHelper _testHelper;
    private SecurityClient _securityClient;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = new SecurityClientTestHelper();
      _securityClient = _testHelper.CreateSecurityClient();
    }

    [Test]
    public void Test_WithParamsArray ()
    {
      _testHelper.ExpectObjectSecurityStrategyHasAccess(TestAccessTypes.First, true);

      bool hasAccess = _securityClient.HasAccess(_testHelper.SecurableObject, AccessType.Get(TestAccessTypes.First));

      _testHelper.VerifyAll();
      Assert.That(hasAccess, Is.EqualTo(true));
    }

    [Test]
    public void Test_WithParamsArray_AndSecurityPrincipal ()
    {
      _testHelper.ExpectObjectSecurityStrategyHasAccess(TestAccessTypes.First, true);

      var securityPrincipal = _securityClient.PrincipalProvider.GetPrincipal();
      bool hasAccess = _securityClient.HasAccess(_testHelper.SecurableObject, securityPrincipal, AccessType.Get(TestAccessTypes.First));

      _testHelper.VerifyAll();
      Assert.That(hasAccess, Is.EqualTo(true));
    }

    [Test]
    public void Test_AccessGranted ()
    {
      _testHelper.ExpectObjectSecurityStrategyHasAccess(TestAccessTypes.First, true);

      bool hasAccess = _securityClient.HasAccess(
          _testHelper.SecurableObject,
          (IReadOnlyList<AccessType>) new[] { AccessType.Get(TestAccessTypes.First) });

      _testHelper.VerifyAll();
      Assert.That(hasAccess, Is.EqualTo(true));
    }

    [Test]
    public void Test_AccessDenied ()
    {
      _testHelper.ExpectObjectSecurityStrategyHasAccess(TestAccessTypes.First, false);

      bool hasAccess = _securityClient.HasAccess(
          _testHelper.SecurableObject,
          (IReadOnlyList<AccessType>) new[] { AccessType.Get(TestAccessTypes.First) });

      _testHelper.VerifyAll();
      Assert.That(hasAccess, Is.EqualTo(false));
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted ()
    {
      bool hasAccess;
      using (SecurityFreeSection.Activate())
      {
        hasAccess = _securityClient.HasAccess(
            _testHelper.SecurableObject,
            (IReadOnlyList<AccessType>) new[] { AccessType.Get(TestAccessTypes.First) });
      }

      _testHelper.VerifyAll();
      Assert.That(hasAccess, Is.True);
    }

#if !DEBUG
    [Ignore ("Skipped unless DEBUG build")]
#endif
    [Test]
    public void Test_WithSecurityStrategyIsNull ()
    {
      Assert.That(
          () =>  _securityClient.HasAccess(new SecurableObject(null), (IReadOnlyList<AccessType>) new[] { AccessType.Get(TestAccessTypes.First) }),
          Throws.InvalidOperationException
              .With.Message.EqualTo("The securableObject did not return an IObjectSecurityStrategy."));

      _testHelper.VerifyAll();
    }
  }
}
