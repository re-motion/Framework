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

namespace Remotion.Security.UnitTests.NullSecurityClientTests
{
  [TestFixture]
  public class CheckAccessTest
  {
    private NullSecurityClientTestHelper _testHelper;
    private SecurityClient _securityClient;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = NullSecurityClientTestHelper.CreateForStatefulSecurity();
      _securityClient = _testHelper.CreateSecurityClient();
    }

    [Test]
    public void Test_WithParamsArray ()
    {
      _securityClient.CheckAccess(_testHelper.SecurableObject, AccessType.Get(TestAccessTypes.First));

      _testHelper.VerifyAll();
    }

    [Test]
    public void Test_WithParamsArray_AndSecurityPrincipal ()
    {
      var securityPrincipal = _securityClient.PrincipalProvider.GetPrincipal();
      _securityClient.CheckAccess(_testHelper.SecurableObject, securityPrincipal, AccessType.Get(TestAccessTypes.First));

      _testHelper.VerifyAll();
    }


    [Test]
    public void Test_AccessGranted ()
    {
      _securityClient.CheckAccess(_testHelper.SecurableObject, (IReadOnlyList<AccessType>)new[] { AccessType.Get(TestAccessTypes.First) });

      _testHelper.VerifyAll();
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted ()
    {
      using (SecurityFreeSection.Activate())
      {
        _securityClient.CheckAccess(_testHelper.SecurableObject, (IReadOnlyList<AccessType>)new[] { AccessType.Get(TestAccessTypes.First) });
      }

      _testHelper.VerifyAll();
    }

    [Test]
    public void Test_WithSecurityStrategyIsNull ()
    {
      _securityClient.CheckAccess(new SecurableObject(null), (IReadOnlyList<AccessType>)new[] { AccessType.Get(TestAccessTypes.First) });

      _testHelper.VerifyAll();
    }
  }
}
