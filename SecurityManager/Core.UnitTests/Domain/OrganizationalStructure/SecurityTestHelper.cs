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
using System.Linq;
using Moq;
using Remotion.Security;
using Remotion.Security.Metadata;
using Remotion.Utilities;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  /// <summary>
  /// The <see cref="SecurityTestHelper"/> is used for providing security specific test objects, such as a stubbed <see cref="SecurityClient"/>.
  /// </summary>
  public class SecurityTestHelper
  {
    public SecurityClient CreatedStubbedSecurityClient<T> (params Enum[] accessTypes)
        where T: ISecurableObject
    {
      ArgumentUtility.CheckNotNull("accessTypes", accessTypes);

      var principalStub = CreatePrincipalStub();

      return new SecurityClient(
          CreateSecurityProviderStub(typeof(T), principalStub, accessTypes),
          new PermissionReflector(),
          CreateUserProviderStub(principalStub),
          new Mock<IFunctionalSecurityStrategy>().Object,
          new ReflectionBasedMemberResolver());
    }

    private ISecurityProvider CreateSecurityProviderStub (Type securableClassType, ISecurityPrincipal principal, Enum[] returnedAccessTypes)
    {
      var securityProviderStub = new Mock<ISecurityProvider>();
      securityProviderStub
          .Setup(stub => stub.GetAccess(It.Is<ISecurityContext>(sc => TypeUtility.GetType(sc.Class, true) == securableClassType), principal))
          .Returns(returnedAccessTypes.Select(accessType => AccessType.Get(accessType)).ToArray());

      return securityProviderStub.Object;
    }

    private IPrincipalProvider CreateUserProviderStub (ISecurityPrincipal principal)
    {
      var userProviderStub = new Mock<IPrincipalProvider>();
      userProviderStub.Setup(stub => stub.GetPrincipal()).Returns(principal);

      return userProviderStub.Object;
    }

    private static ISecurityPrincipal CreatePrincipalStub ()
    {
      var principalStub = new Mock<ISecurityPrincipal>();

      principalStub.Setup(stub => stub.User).Returns("user");

      return principalStub.Object;
    }
  }
}
