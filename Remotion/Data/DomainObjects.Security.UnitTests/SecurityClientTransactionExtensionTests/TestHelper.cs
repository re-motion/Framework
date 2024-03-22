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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.Security.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Reflection;
using Remotion.Security;
using Remotion.Security.Metadata;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Security.UnitTests.SecurityClientTransactionExtensionTests
{
  public delegate bool HasAccessDelegate (ISecurityProvider securityProvider, ISecurityPrincipal principal, IReadOnlyList<AccessType> requiredAccessTypes);
  public delegate bool HasStatelessAccessDelegate (Type type, ISecurityProvider securityProvider, ISecurityPrincipal principal, IReadOnlyList<AccessType> requiredAccessTypes);

  public class TestHelper
  {
    private readonly Mock<ISecurityPrincipal> _stubUser;
    private readonly Mock<ISecurityProvider> _mockSecurityProvider;
    private readonly Mock<IPrincipalProvider> _stubPrincipalProvider;
    private readonly Mock<IFunctionalSecurityStrategy> _mockFunctionalSecurityStrategy;
    private readonly Mock<IPermissionProvider> _mockPermissionReflector;
    private readonly Mock<IMemberResolver> _mockMemberResolver;
    private readonly ClientTransaction _transaction;
    private ServiceLocatorScope _serviceLocatorScope;

    // construction and disposing

    public TestHelper ()
    {
      _mockSecurityProvider = new Mock<ISecurityProvider>(MockBehavior.Strict);
      _stubUser = new Mock<ISecurityPrincipal>();
      _stubUser.Setup(_ => _.User).Returns("user");
      _stubPrincipalProvider = new Mock<IPrincipalProvider>(MockBehavior.Strict);
      _stubPrincipalProvider.Setup(_ => _.GetPrincipal()).Returns(_stubUser.Object);
      _mockFunctionalSecurityStrategy = new Mock<IFunctionalSecurityStrategy>(MockBehavior.Strict);
      _mockPermissionReflector = new Mock<IPermissionProvider>(MockBehavior.Strict);
      _mockMemberResolver = new Mock<IMemberResolver>(MockBehavior.Strict);
      _transaction = ClientTransaction.CreateRootTransaction();

      _mockSecurityProvider.Setup(_ => _.IsNull).Returns(false);
    }

    // methods and properties

    public ClientTransaction Transaction
    {
      get { return _transaction; }
    }

    public IQuery CreateSecurableObjectQuery ()
    {
      return new Query(
          new QueryDefinition(
              StubStorageProvider.GetSecurableObjectsQueryID,
              MappingConfiguration.Current.GetTypeDefinition(typeof(SecurableObject)).StorageEntityDefinition.StorageProviderDefinition,
              "SELECT 1",
              QueryType.Collection),
          new QueryParameterCollection());
    }

    public SecurableObject CreateSecurableObject ()
    {
      return SecurableObject.NewObject(_transaction, CreateObjectSecurityStrategy());
    }

    public NonSecurableObject CreateNonSecurableObject ()
    {
      return NonSecurableObject.NewObject(_transaction);
    }

    public IObjectSecurityStrategy CreateObjectSecurityStrategy ()
    {
      return new Mock<IObjectSecurityStrategy>(MockBehavior.Strict).Object;
    }

    public void SetupSecurityIoCConfiguration ()
    {
      var storageSettings = SafeServiceLocator.Current.GetInstance<IStorageSettings>();

      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle(() => _mockSecurityProvider.Object);
      serviceLocator.RegisterSingle(() => _stubPrincipalProvider.Object);
      serviceLocator.RegisterSingle(() => _mockMemberResolver.Object);
      serviceLocator.RegisterSingle(() => storageSettings);
      serviceLocator.RegisterSingle(() => _mockPermissionReflector.Object);
      serviceLocator.RegisterSingle(() => _mockFunctionalSecurityStrategy.Object);
      _serviceLocatorScope = new ServiceLocatorScope(serviceLocator);
    }

    public void TearDownSecurityIoCConfiguration ()
    {
      _serviceLocatorScope.Dispose();
    }

    public void VerifyAll ()
    {
      _mockSecurityProvider.Verify();
      _stubPrincipalProvider.Verify();
      _mockFunctionalSecurityStrategy.Verify();
      _mockPermissionReflector.Verify();
      _mockMemberResolver.Verify();
    }

    public void AddExtension (IClientTransactionExtension extension)
    {
      ArgumentUtility.CheckNotNullAndType<SecurityClientTransactionExtension>("extension", extension);

      _transaction.Extensions.Add(extension);
    }

    public void ExpectObjectSecurityStrategyHasAccess (SecurableObject securableObject, Enum accessTypeEnum, HasAccessDelegate doDelegate)
    {
      IObjectSecurityStrategy objectSecurityStrategy = securableObject.GetSecurityStrategy();
      Mock.Get(objectSecurityStrategy)
          .Setup(
              _ => _.HasAccess(
                  _mockSecurityProvider.Object,
                  _stubUser.Object,
                  new[] { AccessType.Get(accessTypeEnum) }))
          .Callback((ISecurityProvider securityProvider, ISecurityPrincipal principal, IReadOnlyList<AccessType> requiredAccessTypes) => CheckTransaction())
          .Returns(doDelegate)
          .Verifiable();
    }

    [CLSCompliant(false)]
    public void ExpectObjectSecurityStrategyHasAccess (VerifiableSequence sequence, SecurableObject securableObject, Enum accessTypeEnum, bool returnValue)
    {
      IObjectSecurityStrategy objectSecurityStrategy = securableObject.GetSecurityStrategy();
      Mock.Get(objectSecurityStrategy)
          .InVerifiableSequence(sequence)
          .Setup(
              _ => _.HasAccess(
                  _mockSecurityProvider.Object,
                  _stubUser.Object,
                  new[] { AccessType.Get(accessTypeEnum) }))
          .Callback((ISecurityProvider securityProvider, ISecurityPrincipal principal, IReadOnlyList<AccessType> requiredAccessTypes) => { CheckTransaction(); })
          .Returns(returnValue)
          .Verifiable();
    }

    [CLSCompliant(false)]
    public void ExpectFunctionalSecurityStrategyHasAccess (VerifiableSequence sequence, Type securableObjectType, Enum accessTypeEnum, HasStatelessAccessDelegate doDelegate)
    {
      _mockFunctionalSecurityStrategy
          .InVerifiableSequence(sequence)
          .Setup(
              _ => _.HasAccess(
                  securableObjectType,
                  _mockSecurityProvider.Object,
                  _stubUser.Object,
                  new[] { AccessType.Get(accessTypeEnum) }))
          .Callback((Type type, ISecurityProvider securityProvider, ISecurityPrincipal principal, IReadOnlyList<AccessType> requiredAccessTypes) => { CheckTransaction(); })
          .Returns(doDelegate)
          .Verifiable();
    }

    [CLSCompliant(false)]
    public void ExpectFunctionalSecurityStrategyHasAccess (VerifiableSequence sequence, Type securableObjectType, Enum accessTypeEnum, bool returnValue)
    {
      _mockFunctionalSecurityStrategy
          .InVerifiableSequence(sequence)
          .Setup(
              _ => _.HasAccess(
                  securableObjectType,
                  _mockSecurityProvider.Object,
                  _stubUser.Object,
                  new[] { AccessType.Get(accessTypeEnum) }))
          .Callback((Type type, ISecurityProvider securityProvider, ISecurityPrincipal principal, IReadOnlyList<AccessType> requiredAccessTypes) => { CheckTransaction(); })
          .Returns(returnValue)
          .Verifiable();
    }

    [CLSCompliant(false)]
    public void ExpectPermissionReflectorGetRequiredMethodPermissions (VerifiableSequence sequence, IMethodInformation methodInformation, params Enum[] returnedAccessTypes)
    {
      _mockPermissionReflector
          .InVerifiableSequence(sequence)
          .Setup(
              _ => _.GetRequiredMethodPermissions(
                  typeof(SecurableObject),
                  It.Is<IMethodInformation>(mi => mi.Equals(methodInformation))))
          .Returns(returnedAccessTypes)
          .Verifiable();
    }

    public void ExpectSecurityProviderGetAccess (SecurityContext context, params Enum[] returnedAccessTypes)
    {
      _mockSecurityProvider
          .Setup(_ => _.GetAccess(context, _stubUser.Object))
          .Callback((ISecurityContext context, ISecurityPrincipal principal) => CheckTransaction())
          .Returns(Array.ConvertAll(returnedAccessTypes, AccessType.Get))
          .Verifiable();
    }

    public void ExpectObjectSecurityStrategyHasAccessWithMatchingScope (SecurableObject securableObject, ClientTransactionScope expectedScope)
    {
      IObjectSecurityStrategy objectSecurityStrategy = securableObject.GetSecurityStrategy();
      Mock.Get(objectSecurityStrategy)
          .Setup(
              _ => _.HasAccess(
                  It.IsAny<ISecurityProvider>(),
                  It.IsAny<ISecurityPrincipal>(),
                  It.IsAny<IReadOnlyList<AccessType>>()))
          .Callback((ISecurityProvider securityProvider, ISecurityPrincipal principal, IReadOnlyList<AccessType> requiredAccessTypes) => CheckScope(expectedScope))
          .Returns(true)
          .Verifiable();
    }

    public void ExpectFunctionalSecurityStrategyHasAccessWithMatchingScope (ClientTransactionScope expectedScope)
    {
      _mockFunctionalSecurityStrategy
          .Setup(
              _ => _.HasAccess(
                  It.IsAny<Type>(),
                  It.IsAny<ISecurityProvider>(),
                  It.IsAny<ISecurityPrincipal>(),
                  It.IsAny<IReadOnlyList<AccessType>>()))
          .Callback(() => CheckScope(expectedScope))
          .Returns(true)
          .Verifiable();
    }

    private void CheckScope (ClientTransactionScope expectedScope)
    {
      Assert.That(ClientTransactionScope.ActiveScope, Is.SameAs(expectedScope));
    }

    private void CheckTransaction ()
    {
      Assert.That(ClientTransaction.Current, Is.SameAs(_transaction));
      Assert.That(_transaction.ActiveTransaction, Is.SameAs(_transaction));
    }
  }
}
