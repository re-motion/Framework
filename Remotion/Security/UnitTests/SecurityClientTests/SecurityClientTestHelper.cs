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
using Remotion.Reflection;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.SampleDomain;
using Rhino.Mocks;

namespace Remotion.Security.UnitTests.SecurityClientTests
{
  public class SecurityClientTestHelper
  {
    private readonly MockRepository _mocks;
    private readonly ISecurityPrincipal _userStub;
    private readonly ISecurityProvider _mockSecurityProvider;
    private readonly IPermissionProvider _mockPermissionReflector;
    private readonly IObjectSecurityStrategy _mockObjectSecurityStrategy;
    private readonly IFunctionalSecurityStrategy _mockFunctionalSecurityStrategy;
    private readonly IMemberResolver _mockMemberResolver;
    private readonly IPrincipalProvider _stubPrincipalProvider;
    private readonly SecurableObject _securableObject;

    public SecurityClientTestHelper ()
    {
      _mocks = new MockRepository();
      _mockSecurityProvider = _mocks.StrictMock<ISecurityProvider>();
      _mockPermissionReflector = _mocks.StrictMock<IPermissionProvider>();
      _mockObjectSecurityStrategy = _mocks.StrictMock<IObjectSecurityStrategy>();
      _mockFunctionalSecurityStrategy = _mocks.StrictMock<IFunctionalSecurityStrategy>();
      _mockMemberResolver = _mocks.StrictMock<IMemberResolver>();
      _userStub = _mocks.Stub<ISecurityPrincipal>();
      SetupResult.For (_userStub.User).Return ("user");
      _stubPrincipalProvider = _mocks.Stub<IPrincipalProvider>();
      SetupResult.For (_stubPrincipalProvider.GetPrincipal()).Return (_userStub);

      _securableObject = new SecurableObject (_mockObjectSecurityStrategy);
    }

    public SecurityClient CreateSecurityClient ()
    {
      return new SecurityClient (_mockSecurityProvider, _mockPermissionReflector, _stubPrincipalProvider, _mockFunctionalSecurityStrategy, _mockMemberResolver);
    }

    public SecurableObject SecurableObject
    {
      get { return _securableObject; }
    }

    public void ExpectMemberResolverGetMethodInformation (string methodName, MemberAffiliation memberAffiliation, IMethodInformation returnValue)
    {
      Expect.Call (_mockMemberResolver.GetMethodInformation (typeof (SecurableObject), methodName, memberAffiliation)).Return (returnValue);
    }

    public void ExpectMemberResolverGetMethodInformation (MethodInfo methodInfo, MemberAffiliation memberAffiliation, IMethodInformation returnValue)
    {
      Expect.Call (_mockMemberResolver.GetMethodInformation (typeof (SecurableObject), methodInfo, memberAffiliation)).Return (returnValue);
    }

    public void ExpectPermissionReflectorGetRequiredMethodPermissions (IMethodInformation methodInformation, params Enum[] returnValue)
    {
      Expect.Call (_mockPermissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), methodInformation)).Return (returnValue);
    }

    public void ExpectObjectSecurityStrategyHasAccess (Enum requiredAccessType, bool returnValue)
    {
      ExpectObjectSecurityStrategyHasAccess (new[] { requiredAccessType }, returnValue);
    }

    public void ExpectObjectSecurityStrategyHasAccess (Enum[] requiredAccessTypes, bool returnValue)
    {
      Expect
          .Call (
              _mockObjectSecurityStrategy.HasAccess (
                  Arg.Is (_mockSecurityProvider),
                  Arg.Is (_userStub),
                  Arg<IReadOnlyList<AccessType>>.List.Equal (ConvertAccessTypeEnums (requiredAccessTypes))))
          .Return (returnValue);
    }

    public void ExpectFunctionalSecurityStrategyHasAccess (Enum requiredAccessType, bool returnValue)
    {
      ExpectFunctionalSecurityStrategyHasAccess (new[] { requiredAccessType }, returnValue);
    }

    public void ExpectFunctionalSecurityStrategyHasAccess (Enum[] requiredAccessTypes, bool returnValue)
    {
      Expect
          .Call (
              _mockFunctionalSecurityStrategy.HasAccess (
                  Arg.Is (typeof (SecurableObject)),
                  Arg.Is (_mockSecurityProvider),
                  Arg.Is (_userStub),
                  Arg<IReadOnlyList<AccessType>>.List.Equal (ConvertAccessTypeEnums (requiredAccessTypes))))
          .Return (returnValue);
    }

    public void ReplayAll ()
    {
      _mocks.ReplayAll();
    }

    public void VerifyAll ()
    {
      _mocks.VerifyAll();
    }

    private IReadOnlyList<AccessType> ConvertAccessTypeEnums (Enum[] accessTypeEnums)
    {
      return Array.ConvertAll (accessTypeEnums, AccessType.Get);
    }

  }
}
