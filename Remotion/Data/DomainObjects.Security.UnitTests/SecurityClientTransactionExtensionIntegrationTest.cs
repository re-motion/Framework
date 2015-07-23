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
using Remotion.Collections;
using Remotion.Data.DomainObjects.Security.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Security;
using Remotion.ServiceLocation;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.Security.UnitTests
{
  [TestFixture]
  public class SecurityClientTransactionExtensionIntegrationTest : TestBase
  {
    private ISecurityProvider _securityProviderStub;
    private IPrincipalProvider _principalProviderStub;
    private ISecurityPrincipal _securityPrincipalStub;
    private IFunctionalSecurityStrategy _functionalSecurityStrategyStub;

    private ISecurityContext _securityContextStub;
    private ISecurityContextFactory _securityContextFactoryStub;
    
    private ClientTransaction _clientTransaction;
    private ServiceLocatorScope _serviceLocatorScope;

    public override void SetUp ()
    {
      base.SetUp();

      _securityProviderStub = MockRepository.GenerateStub<ISecurityProvider>();
      _principalProviderStub = MockRepository.GenerateStub<IPrincipalProvider>();
      _securityPrincipalStub = MockRepository.GenerateStub<ISecurityPrincipal>();
      _functionalSecurityStrategyStub = MockRepository.GenerateStub<IFunctionalSecurityStrategy>();

      _principalProviderStub.Stub (stub => stub.GetPrincipal()).Return (_securityPrincipalStub);

      _securityContextStub = MockRepository.GenerateStub<ISecurityContext> ();
      _securityContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory> ();

      _securityContextFactoryStub.Stub (mock => mock.CreateSecurityContext ()).Return (_securityContextStub);

      _clientTransaction = ClientTransaction.CreateRootTransaction();
      _clientTransaction.Extensions.Add (new SecurityClientTransactionExtension());

      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle (() => _securityProviderStub);
      serviceLocator.RegisterSingle (() => _principalProviderStub);
      serviceLocator.RegisterSingle (() => _functionalSecurityStrategyStub);
      _serviceLocatorScope = new ServiceLocatorScope (serviceLocator);

      _clientTransaction.EnterNonDiscardingScope();
    }

    public override void TearDown ()
    {
      ClientTransactionScope.ResetActiveScope();
      _serviceLocatorScope.Dispose();

      base.TearDown();
    }

    [Test]
    public void AccessGranted_PropertyWithDefaultPermission ()
    {
      _securityProviderStub
          .Stub (mock => mock.GetAccess (_securityContextStub, _securityPrincipalStub))
          .Return (new[] { AccessType.Get (GeneralAccessTypes.Read) });

      SecurableObject securableObject;
      using (SecurityFreeSection.Activate())
      {
        securableObject = CreateSecurableObject (_securityContextFactoryStub);
      }

      Dev.Null = securableObject.PropertyWithDefaultPermission;
    }

    [Test]
    public void AccessGranted_PropertyWithCustomPermission ()
    {
      _securityProviderStub
          .Stub (mock => mock.GetAccess (_securityContextStub, _securityPrincipalStub))
          .Return (new[] { AccessType.Get (TestAccessTypes.First) });

      SecurableObject securableObject;
      using (SecurityFreeSection.Activate())
      {
        securableObject = CreateSecurableObject (_securityContextFactoryStub);
      }

      Dev.Null = securableObject.PropertyWithCustomPermission;
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException), ExpectedMessage =
        "Access to method 'get_PropertyWithDefaultPermission' on type 'Remotion.Data.DomainObjects.Security.UnitTests.TestDomain.SecurableObject' has been denied.")]
    public void AccessDenied_PropertyWithDefaultPermission ()
    {
      _securityProviderStub.Stub (mock => mock.GetAccess (_securityContextStub, _securityPrincipalStub)).Return (new AccessType[0]);

      SecurableObject securableObject;
      using (SecurityFreeSection.Activate())
      {
        securableObject = CreateSecurableObject (_securityContextFactoryStub);
      }

      Dev.Null = securableObject.PropertyWithDefaultPermission;
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException), ExpectedMessage =
        "Access to method 'get_PropertyWithCustomPermission' on type 'Remotion.Data.DomainObjects.Security.UnitTests.TestDomain.SecurableObject' has been denied.")]
    public void AccessDenied_PropertyWithCustomPermission ()
    {
      _securityProviderStub.Stub (mock => mock.GetAccess (_securityContextStub, _securityPrincipalStub)).Return (new AccessType[0]);

      SecurableObject securableObject;
      using (SecurityFreeSection.Activate())
      {
        securableObject = CreateSecurableObject (_securityContextFactoryStub);
      }

      Dev.Null = securableObject.PropertyWithCustomPermission;
    }

    [Test]
    public void AccessGranted_MixedPropertyWithDefaultPermission ()
    {
      _securityProviderStub
          .Stub (mock => mock.GetAccess (_securityContextStub, _securityPrincipalStub))
          .Return (new[] { AccessType.Get (GeneralAccessTypes.Read) });

      SecurableObject securableObject;
      using (SecurityFreeSection.Activate())
      {
        securableObject = CreateSecurableObject (_securityContextFactoryStub);
      }

      Dev.Null = ((ISecurableObjectMixin) securableObject).MixedPropertyWithDefaultPermission;
    }

    [Test]
    public void AccessGranted_MixedPropertyWithCustomPermission ()
    {
      _securityProviderStub
          .Stub (mock => mock.GetAccess (_securityContextStub, _securityPrincipalStub))
          .Return (new[] { AccessType.Get (TestAccessTypes.First) });

      SecurableObject securableObject;
      using (SecurityFreeSection.Activate())
      {
        securableObject = CreateSecurableObject (_securityContextFactoryStub);
      }

      Dev.Null = ((ISecurableObjectMixin) securableObject).MixedPropertyWithCustomPermission;
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException), ExpectedMessage =
        "Access to method 'get_MixedPropertyWithDefaultPermission' on type 'Remotion.Data.DomainObjects.Security.UnitTests.TestDomain.SecurableObject' has been denied.")]
    public void AccessDenied_MixedPropertyWithDefaultPermission ()
    {
      _securityProviderStub.Stub (mock => mock.GetAccess (_securityContextStub, _securityPrincipalStub)).Return (new AccessType[0]);

      SecurableObject securableObject;
      using (SecurityFreeSection.Activate())
      {
        securableObject = CreateSecurableObject (_securityContextFactoryStub);
      }

      Dev.Null = ((ISecurableObjectMixin) securableObject).MixedPropertyWithDefaultPermission;
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException), ExpectedMessage =
        "Access to method 'get_MixedPropertyWithCustomPermission' on type 'Remotion.Data.DomainObjects.Security.UnitTests.TestDomain.SecurableObject' has been denied.")]
    public void AccessDenied_MixedPropertyWithCustomPermission ()
    {
      _securityProviderStub.Stub (mock => mock.GetAccess (_securityContextStub, _securityPrincipalStub)).Return (new AccessType[0]);

      SecurableObject securableObject;
      using (SecurityFreeSection.Activate())
      {
        securableObject = CreateSecurableObject (_securityContextFactoryStub);
      }

      Dev.Null = ((ISecurableObjectMixin) securableObject).MixedPropertyWithCustomPermission;
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException), ExpectedMessage =
        "Access to method 'get_PropertyWithDefaultPermission' on type 'Remotion.Data.DomainObjects.Security.UnitTests.TestDomain.SecurableObject' has been denied.")]
    public void AccessDenied_SubTransaction ()
    {
      _securityProviderStub.Stub (mock => mock.GetAccess (_securityContextStub, _securityPrincipalStub)).Return (new AccessType[0]);

      Assert.That (_clientTransaction.Extensions[SecurityClientTransactionExtension.DefaultKey], Is.Not.Null);

      var subTransaction = _clientTransaction.CreateSubTransaction();
      Assert.That (subTransaction.Extensions[SecurityClientTransactionExtension.DefaultKey], Is.Not.Null);

      using (subTransaction.EnterDiscardingScope ())
      {
        SecurableObject securableObject;
        using (SecurityFreeSection.Activate())
        {
          securableObject = CreateSecurableObject (_securityContextFactoryStub, clientTransaction: subTransaction);
        }

        Dev.Null = securableObject.PropertyWithDefaultPermission;
      }
    }

    [Test]
    public void AutomaticCleanup_WhenDomainObjectCtorThrows_DoesNotRequireDeletePermissions ()
    {
      _securityProviderStub.Stub (mock => mock.GetAccess (_securityContextStub, _securityPrincipalStub)).Return (new AccessType[0]);

      _functionalSecurityStrategyStub
          .Stub (
              mock => mock.HasAccess (
                  Arg.Is(  typeof (SecurableObject)),
                  Arg.Is(_securityProviderStub),
                  Arg.Is(_securityPrincipalStub),
                  Arg<IReadOnlyList<AccessType>>.List.Equal( new[] { AccessType.Get (GeneralAccessTypes.Create) })))
          .Return (true);

      var exception = new Exception ("Test.");
      SecurableObject throwingObject = null;

      Assert.That (
          () => CreateSecurableObject (
              _securityContextFactoryStub,
              action: obj =>
              {
                throwingObject = obj;
                throw exception;
              }),
          Throws.Exception.SameAs (exception));

      Assert.That (_clientTransaction.IsEnlisted (throwingObject), Is.False);
    }

    private SecurableObject CreateSecurableObject (ISecurityContextFactory securityContextFactory, ClientTransaction clientTransaction = null, Action<SecurableObject> action = null)
    {
      return SecurableObject.NewObject (
          clientTransaction ?? _clientTransaction,
          ObjectSecurityStrategy.Create (securityContextFactory, InvalidationToken.Create()),
          action);
    }
  }
}
