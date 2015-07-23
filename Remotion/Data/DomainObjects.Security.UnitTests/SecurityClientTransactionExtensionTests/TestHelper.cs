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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.Security.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Reflection;
using Remotion.Security;
using Remotion.Security.Metadata;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.Security.UnitTests.SecurityClientTransactionExtensionTests
{
  public delegate bool HasAccessDelegate (ISecurityProvider securityProvider, ISecurityPrincipal principal, IReadOnlyList<AccessType> requiredAccessTypes);
  public delegate bool HasStatelessAccessDelegate (Type type, ISecurityProvider securityProvider, ISecurityPrincipal principal, IReadOnlyList<AccessType> requiredAccessTypes);

  public class TestHelper
  {
    // types

    // static members

    // member fields

    private readonly MockRepository _mocks;
    private readonly ISecurityPrincipal _stubUser;
    private readonly ISecurityProvider _mockSecurityProvider;
    private readonly IPrincipalProvider _stubPrincipalProvider;
    private readonly IFunctionalSecurityStrategy _mockFunctionalSecurityStrategy;
    private readonly IPermissionProvider _mockPermissionReflector;
    private readonly IMemberResolver _mockMemberResolver;
    private readonly ClientTransaction _transaction;
    private ServiceLocatorScope _serviceLocatorScope;

    // construction and disposing

    public TestHelper ()
    {
      _mocks = new MockRepository ();
      _mockSecurityProvider = _mocks.StrictMock<ISecurityProvider> ();
      _stubUser = _mocks.Stub<ISecurityPrincipal> ();
      SetupResult.For (_stubUser.User).Return ("user");
      _stubPrincipalProvider = _mocks.StrictMock<IPrincipalProvider> ();
      SetupResult.For (_stubPrincipalProvider.GetPrincipal ()).Return (_stubUser);
      _mockFunctionalSecurityStrategy = _mocks.StrictMock<IFunctionalSecurityStrategy> ();
      _mockPermissionReflector = _mocks.StrictMock<IPermissionProvider> ();
      _mockMemberResolver = _mocks.StrictMock<IMemberResolver>();
      _transaction = ClientTransaction.CreateRootTransaction();

      SetupResult.For (_mockSecurityProvider.IsNull).Return (false);
    }

    // methods and properties

    public ClientTransaction Transaction
    {
      get { return _transaction; }
    }

    public IQuery CreateSecurableObjectQuery ()
    {
      return new Query (
          new QueryDefinition (
              StubStorageProvider.GetSecurableObjectsQueryID,
              MappingConfiguration.Current.GetTypeDefinition (typeof (SecurableObject)).StorageEntityDefinition.StorageProviderDefinition,
              "SELECT 1",
              QueryType.Collection),
          new QueryParameterCollection());
    }

    public SecurableObject CreateSecurableObject ()
    {
      return SecurableObject.NewObject (_transaction, CreateObjectSecurityStrategy ());
    }

    public NonSecurableObject CreateNonSecurableObject ()
    {
      return NonSecurableObject.NewObject (_transaction);
    }

    public IObjectSecurityStrategy CreateObjectSecurityStrategy ()
    {
      return _mocks.StrictMock<IObjectSecurityStrategy> ();
    }

    public void SetupSecurityIoCConfiguration ()
    {
      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle (() => _mockSecurityProvider);
      serviceLocator.RegisterSingle (() => _stubPrincipalProvider);
      serviceLocator.RegisterSingle (() => _mockMemberResolver);
      serviceLocator.RegisterSingle (() => _mockPermissionReflector);
      serviceLocator.RegisterSingle (() => _mockFunctionalSecurityStrategy);
      _serviceLocatorScope = new ServiceLocatorScope (serviceLocator);
    }

    public void TearDownSecurityIoCConfiguration ()
    {
      _serviceLocatorScope.Dispose();
    }

    public void ReplayAll ()
    {
      _mocks.ReplayAll ();
    }

    public void VerifyAll ()
    {
      _mocks.VerifyAll ();
    }

    public IDisposable Ordered ()
    {
      return _mocks.Ordered ();
    }

    public void AddExtension (IClientTransactionExtension extension)
    {
      ArgumentUtility.CheckNotNullAndType<SecurityClientTransactionExtension> ("extension", extension);

      _transaction.Extensions.Add (extension);
    }

    public void ExpectObjectSecurityStrategyHasAccess (SecurableObject securableObject, Enum accessTypeEnum, HasAccessDelegate doDelegate)
    {
      IObjectSecurityStrategy objectSecurityStrategy = securableObject.GetSecurityStrategy();
      Expect.Call (
          objectSecurityStrategy.HasAccess (
              Arg.Is (_mockSecurityProvider),
              Arg.Is (_stubUser),
              Arg<IReadOnlyList<AccessType>>.List.Equal (new[] { AccessType.Get (accessTypeEnum) })))
          .WhenCalled (mi => CheckTransaction())
          .Do (doDelegate);
    }

    public void ExpectObjectSecurityStrategyHasAccess (SecurableObject securableObject, Enum accessTypeEnum, bool returnValue)
    {
      IObjectSecurityStrategy objectSecurityStrategy = securableObject.GetSecurityStrategy();
      Expect.Call (
          objectSecurityStrategy.HasAccess (
              Arg.Is (_mockSecurityProvider),
              Arg.Is (_stubUser),
              Arg<IReadOnlyList<AccessType>>.List.Equal (new[] { AccessType.Get (accessTypeEnum) })))
          .WhenCalled (mi => CheckTransaction())
          .WhenCalled (mi => CheckTransaction())
          .Return (returnValue);
    }

    public void ExpectFunctionalSecurityStrategyHasAccess (Type securableObjectType, Enum accessTypeEnum, HasStatelessAccessDelegate doDelegate)
    {
      Expect.Call (
          _mockFunctionalSecurityStrategy.HasAccess (
              Arg.Is (securableObjectType),
              Arg.Is (_mockSecurityProvider),
              Arg.Is (_stubUser),
              Arg<IReadOnlyList<AccessType>>.List.Equal (new[] { AccessType.Get (accessTypeEnum) })))
          .WhenCalled (mi => CheckTransaction())
          .WhenCalled (mi => CheckTransaction())
          .Do (doDelegate);
    }

    public void ExpectFunctionalSecurityStrategyHasAccess (Type securableObjectType, Enum accessTypeEnum, bool returnValue)
    {
      Expect.Call (
          _mockFunctionalSecurityStrategy.HasAccess (
              Arg.Is (securableObjectType),
              Arg.Is (_mockSecurityProvider),
              Arg.Is (_stubUser),
              Arg<IReadOnlyList<AccessType>>.List.Equal (new[] { AccessType.Get (accessTypeEnum) })))
          .WhenCalled (mi => CheckTransaction())
          .WhenCalled (mi => CheckTransaction())
          .Return (returnValue);
    }

    public void ExpectPermissionReflectorGetRequiredMethodPermissions (IMethodInformation methodInformation, params Enum[] returnedAccessTypes)
    {
      Expect.Call (
          _mockPermissionReflector.GetRequiredMethodPermissions (
              Arg.Is (typeof (SecurableObject)),
              Arg<IMethodInformation>.Matches (mi => mi.Equals (methodInformation))))
          .Return (returnedAccessTypes);
    }

    public void ExpectSecurityProviderGetAccess (SecurityContext context, params Enum[] returnedAccessTypes)
    {
      Expect.Call (_mockSecurityProvider.GetAccess (context, _stubUser))
          .WhenCalled (mi => CheckTransaction())
          .Return (Array.ConvertAll (returnedAccessTypes, AccessType.Get));
    }

    public void ExpectObjectSecurityStrategyHasAccessWithMatchingScope (SecurableObject securableObject, ClientTransactionScope expectedScope)
    {
      IObjectSecurityStrategy objectSecurityStrategy = securableObject.GetSecurityStrategy ();
      Expect.Call (objectSecurityStrategy.HasAccess (null, null, null))
          .IgnoreArguments()
          .WhenCalled (mi => CheckScope (expectedScope))
          .Return (true);
    }

    public void ExpectFunctionalSecurityStrategyHasAccessWithMatchingScope (ClientTransactionScope expectedScope)
    {
      Expect.Call (_mockFunctionalSecurityStrategy.HasAccess (null, null, null, null))
          .IgnoreArguments()
          .WhenCalled (mi => CheckScope (expectedScope))
          .Return (true);
    }

    private void CheckScope (ClientTransactionScope expectedScope)
    {
      Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (expectedScope));
    }

    private void CheckTransaction ()
    {
      Assert.That (ClientTransaction.Current, Is.SameAs (_transaction));
      Assert.That (_transaction.ActiveTransaction, Is.SameAs (_transaction));
    }
  }
}
