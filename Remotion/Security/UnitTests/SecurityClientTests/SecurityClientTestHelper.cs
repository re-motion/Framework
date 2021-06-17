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
using System.Reflection;
using Moq;
using Remotion.Reflection;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.SampleDomain;

namespace Remotion.Security.UnitTests.SecurityClientTests
{
  public class SecurityClientTestHelper
  {
    private readonly Mock<ISecurityPrincipal> _userStub;
    private readonly Mock<ISecurityProvider> _mockSecurityProvider;
    private readonly Mock<IPermissionProvider> _mockPermissionReflector;
    private readonly Mock<IObjectSecurityStrategy> _mockObjectSecurityStrategy;
    private readonly Mock<IFunctionalSecurityStrategy> _mockFunctionalSecurityStrategy;
    private readonly Mock<IMemberResolver> _mockMemberResolver;
    private readonly Mock<IPrincipalProvider> _stubPrincipalProvider;
    private readonly SecurableObject _securableObject;

    public SecurityClientTestHelper ()
    {
      _mockSecurityProvider = new Mock<ISecurityProvider> (MockBehavior.Strict);
      _mockPermissionReflector = new Mock<IPermissionProvider> (MockBehavior.Strict);
      _mockObjectSecurityStrategy = new Mock<IObjectSecurityStrategy> (MockBehavior.Strict);
      _mockFunctionalSecurityStrategy = new Mock<IFunctionalSecurityStrategy> (MockBehavior.Strict);
      _mockMemberResolver = new Mock<IMemberResolver> (MockBehavior.Strict);
      _userStub = new Mock<ISecurityPrincipal>();
      _userStub.Setup (_ => _.User).Returns ("user");
      _stubPrincipalProvider = new Mock<IPrincipalProvider>();
      _stubPrincipalProvider.Setup (_ => _.GetPrincipal()).Returns (_userStub.Object);

      _securableObject = new SecurableObject (_mockObjectSecurityStrategy.Object);
    }

    public SecurityClient CreateSecurityClient ()
    {
      return new SecurityClient (_mockSecurityProvider.Object, _mockPermissionReflector.Object, _stubPrincipalProvider.Object, _mockFunctionalSecurityStrategy.Object, _mockMemberResolver.Object);
    }

    public SecurableObject SecurableObject
    {
      get { return _securableObject; }
    }

    public void ExpectMemberResolverGetMethodInformation (string methodName, MemberAffiliation memberAffiliation, IMethodInformation returnValue)
    {
      _mockMemberResolver.Setup (_ => _.GetMethodInformation (typeof (SecurableObject), methodName, memberAffiliation)).Returns (returnValue).Verifiable();
    }

    public void ExpectMemberResolverGetMethodInformation (MethodInfo methodInfo, MemberAffiliation memberAffiliation, IMethodInformation returnValue)
    {
      _mockMemberResolver.Setup (_ => _.GetMethodInformation (typeof (SecurableObject), methodInfo, memberAffiliation)).Returns (returnValue).Verifiable();
    }

    public void ExpectPermissionReflectorGetRequiredMethodPermissions (IMethodInformation methodInformation, params Enum[] returnValue)
    {
      _mockPermissionReflector.Setup (_ => _.GetRequiredMethodPermissions (typeof (SecurableObject), methodInformation)).Returns (returnValue).Verifiable();
    }

    public void ExpectObjectSecurityStrategyHasAccess (Enum requiredAccessType, bool returnValue)
    {
      ExpectObjectSecurityStrategyHasAccess (new[] { requiredAccessType }, returnValue);
    }

    public void ExpectObjectSecurityStrategyHasAccess (Enum[] requiredAccessTypes, bool returnValue)
    {
      var value = ConvertAccessTypeEnums (requiredAccessTypes);
      _mockObjectSecurityStrategy
          .Setup (_ => _.HasAccess (
                  _mockSecurityProvider.Object,
                  _userStub.Object,
                  value))
          .Returns (returnValue)
          .Verifiable();
    }

    public void ExpectFunctionalSecurityStrategyHasAccess (Enum requiredAccessType, bool returnValue)
    {
      ExpectFunctionalSecurityStrategyHasAccess (new[] { requiredAccessType }, returnValue);
    }

    public void ExpectFunctionalSecurityStrategyHasAccess (Enum[] requiredAccessTypes, bool returnValue)
    {
      var value = ConvertAccessTypeEnums (requiredAccessTypes);
      _mockFunctionalSecurityStrategy
          .Setup (_ => _.HasAccess (
                  typeof (SecurableObject),
                  _mockSecurityProvider.Object,
                  _userStub.Object,
                  value))
          .Returns (returnValue)
          .Verifiable();
    }

    public void VerifyAll ()
    {
      _mockSecurityProvider.Verify();
      _mockPermissionReflector.Verify();
      _mockObjectSecurityStrategy.Verify();
      _mockFunctionalSecurityStrategy.Verify();
      _mockMemberResolver.Verify();
    }

    private IReadOnlyList<AccessType> ConvertAccessTypeEnums (Enum[] accessTypeEnums)
    {
      return Array.ConvertAll (accessTypeEnums, AccessType.Get);
    }

  }
}
