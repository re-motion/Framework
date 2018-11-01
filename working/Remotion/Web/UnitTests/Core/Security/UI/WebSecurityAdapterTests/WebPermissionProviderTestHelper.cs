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
using Remotion.Security;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UnitTests.Core.Security.Domain;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.Security.UI.WebSecurityAdapterTests
{
  public class WebPermissionProviderTestHelper
  {
    // types

    // static members

    // member fields

    private readonly MockRepository _mocks;
    private readonly ISecurityPrincipal _stubUser;
    private readonly ISecurityProvider _mockSecurityProvider;
    private readonly IPrincipalProvider _mockPrincipalProvider;
    private readonly IObjectSecurityStrategy _mockObjectSecurityStrategy;
    private readonly IFunctionalSecurityStrategy _mockFunctionalSecurityStrategy;
    private readonly IWxeSecurityAdapter _mockWxeSecurityAdapter;

    // construction and disposing

    public WebPermissionProviderTestHelper ()
    {
      _mocks = new MockRepository();

      _mockSecurityProvider = _mocks.StrictMock<ISecurityProvider>();
      SetupResult.For (_mockSecurityProvider.IsNull).Return (false);
      _mockObjectSecurityStrategy = _mocks.StrictMock<IObjectSecurityStrategy>();
      _mockFunctionalSecurityStrategy = _mocks.StrictMock<IFunctionalSecurityStrategy>();
      _mockWxeSecurityAdapter = _mocks.StrictMock<IWxeSecurityAdapter>();

      _stubUser = _mocks.Stub<ISecurityPrincipal>();
      SetupResult.For (_stubUser.User).Return ("user");
      _mockPrincipalProvider = _mocks.StrictMock<IPrincipalProvider>();
      SetupResult.For (_mockPrincipalProvider.GetPrincipal()).Return (_stubUser);
    }

    // methods and properties

    public void ExpectHasAccess (Enum[] accessTypeEnums, bool returnValue)
    {
      AccessType[] accessTypes = Array.ConvertAll<Enum, AccessType> (accessTypeEnums, AccessType.Get);
      Expect.Call (
          _mockObjectSecurityStrategy.HasAccess (
              Arg.Is (_mockSecurityProvider),
              Arg.Is (_stubUser),
              Arg<IReadOnlyList<AccessType>>.List.Equal (accessTypes)))
          .Return (returnValue);
    }

    public void ExpectHasStatelessAccessForSecurableObject (Enum[] accessTypeEnums, bool returnValue)
    {
      AccessType[] accessTypes = Array.ConvertAll<Enum, AccessType> (accessTypeEnums, AccessType.Get);
      Expect
          .Call (
              _mockFunctionalSecurityStrategy.HasAccess (
                  Arg.Is (typeof (SecurableObject)),
                  Arg.Is (_mockSecurityProvider),
                  Arg.Is (_stubUser),
                  Arg<IReadOnlyList<AccessType>>.List.Equal (accessTypes)))
          .Return (returnValue);
    }

    public void ExpectHasStatelessAccessForWxeFunction (Type functionType, bool returnValue)
    {
      Expect.Call (_mockWxeSecurityAdapter.HasStatelessAccess (functionType)).Return (returnValue);
    }

    public void ReplayAll ()
    {
      _mocks.ReplayAll ();
    }

    public void VerifyAll ()
    {
      _mocks.VerifyAll ();
    }

    public ISecurityProvider SecurityProvider
    {
      get { return _mockSecurityProvider; }
    }

    public IPrincipalProvider PrincipalProvider
    {
      get { return _mockPrincipalProvider; }
    }

    public IFunctionalSecurityStrategy FunctionalSecurityStrategy
    {
      get { return _mockFunctionalSecurityStrategy; }
    }

    public IWxeSecurityAdapter WxeSecurityAdapter
    {
      get { return _mockWxeSecurityAdapter; }
    }

    public SecurableObject CreateSecurableObject ()
    {
      return new SecurableObject (_mockObjectSecurityStrategy);
    }
  }
}
