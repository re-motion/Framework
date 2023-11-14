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
using Moq;
using Remotion.Security;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UnitTests.Core.Security.Domain;

namespace Remotion.Web.UnitTests.Core.Security.UI.WebSecurityAdapterTests
{
  public class WebPermissionProviderTestHelper
  {
    // types

    // static members

    // member fields
    private readonly Mock<ISecurityPrincipal> _stubUser;
    private readonly Mock<ISecurityProvider> _mockSecurityProvider;
    private readonly Mock<IPrincipalProvider> _mockPrincipalProvider;
    private readonly Mock<IObjectSecurityStrategy> _mockObjectSecurityStrategy;
    private readonly Mock<IFunctionalSecurityStrategy> _mockFunctionalSecurityStrategy;
    private readonly Mock<IWxeSecurityAdapter> _mockWxeSecurityAdapter;

    // construction and disposing

    public WebPermissionProviderTestHelper ()
    {
      _mockSecurityProvider = new Mock<ISecurityProvider>(MockBehavior.Strict);
      _mockSecurityProvider.Setup(_ => _.IsNull).Returns(false);
      _mockObjectSecurityStrategy = new Mock<IObjectSecurityStrategy>(MockBehavior.Strict);
      _mockFunctionalSecurityStrategy = new Mock<IFunctionalSecurityStrategy>(MockBehavior.Strict);
      _mockWxeSecurityAdapter = new Mock<IWxeSecurityAdapter>(MockBehavior.Strict);

      _stubUser = new Mock<ISecurityPrincipal>();
      _stubUser.Setup(_ => _.User).Returns("user");
      _mockPrincipalProvider = new Mock<IPrincipalProvider>(MockBehavior.Strict);
      _mockPrincipalProvider.Setup(_ => _.GetPrincipal()).Returns(_stubUser.Object);
    }

    // methods and properties

    public void ExpectHasAccess (Enum[] accessTypeEnums, bool returnValue)
    {
      AccessType[] accessTypes = Array.ConvertAll<Enum, AccessType>(accessTypeEnums, AccessType.Get);
      _mockObjectSecurityStrategy
          .Setup(
              _ => _.HasAccess(
                  _mockSecurityProvider.Object,
                  _stubUser.Object,
                  accessTypes))
          .Returns(returnValue)
          .Verifiable();
    }

    public void ExpectHasStatelessAccessForSecurableObject (Enum[] accessTypeEnums, bool returnValue)
    {
      AccessType[] accessTypes = Array.ConvertAll<Enum, AccessType>(accessTypeEnums, AccessType.Get);
      _mockFunctionalSecurityStrategy
          .Setup(
              _ => _.HasAccess(
                  typeof(SecurableObject),
                  _mockSecurityProvider.Object,
                  _stubUser.Object,
                  accessTypes))
          .Returns(returnValue)
          .Verifiable();
    }

    public void ExpectHasStatelessAccessForWxeFunction (Type functionType, bool returnValue)
    {
      _mockWxeSecurityAdapter.Setup(_ => _.HasStatelessAccess(functionType)).Returns(returnValue).Verifiable();
    }

    public void VerifyAll ()
    {
      _mockSecurityProvider.Verify();
      _mockObjectSecurityStrategy.Verify();
      _mockFunctionalSecurityStrategy.Verify();
      _mockWxeSecurityAdapter.Verify();
      _stubUser.Verify();
      _mockPrincipalProvider.Verify();
    }

    public ISecurityProvider SecurityProvider
    {
      get { return _mockSecurityProvider.Object; }
    }

    public IPrincipalProvider PrincipalProvider
    {
      get { return _mockPrincipalProvider.Object; }
    }

    public IFunctionalSecurityStrategy FunctionalSecurityStrategy
    {
      get { return _mockFunctionalSecurityStrategy.Object; }
    }

    public IWxeSecurityAdapter WxeSecurityAdapter
    {
      get { return _mockWxeSecurityAdapter.Object; }
    }

    public SecurableObject CreateSecurableObject ()
    {
      return new SecurableObject(_mockObjectSecurityStrategy.Object);
    }
  }
}
