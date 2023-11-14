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
using NUnit.Framework;
using Remotion.Collections.Caching;
using Remotion.Data.DomainObjects.Security.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Security;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.Security.UnitTests
{
  [TestFixture]
  public class SecurityClientTransactionExtensionIntegrationTest : TestBase
  {
    private Mock<ISecurityProvider> _securityProviderStub;
    private Mock<IPrincipalProvider> _principalProviderStub;
    private Mock<ISecurityPrincipal> _securityPrincipalStub;
    private Mock<IFunctionalSecurityStrategy> _functionalSecurityStrategyStub;

    private Mock<ISecurityContext> _securityContextStub;
    private Mock<ISecurityContextFactory> _securityContextFactoryStub;

    private ClientTransaction _clientTransaction;
    private ServiceLocatorScope _serviceLocatorScope;

    public override void SetUp ()
    {
      base.SetUp();

      _securityProviderStub = new Mock<ISecurityProvider>();
      _principalProviderStub = new Mock<IPrincipalProvider>();
      _securityPrincipalStub = new Mock<ISecurityPrincipal>();
      _functionalSecurityStrategyStub = new Mock<IFunctionalSecurityStrategy>();

      _principalProviderStub.Setup(stub => stub.GetPrincipal()).Returns(_securityPrincipalStub.Object);

      _securityContextStub = new Mock<ISecurityContext>();
      _securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      _securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(_securityContextStub.Object);

      _clientTransaction = ClientTransaction.CreateRootTransaction();
      _clientTransaction.Extensions.Add(new SecurityClientTransactionExtension());

      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle(() => _securityProviderStub.Object);
      serviceLocator.RegisterSingle(() => _principalProviderStub.Object);
      serviceLocator.RegisterSingle(() => _functionalSecurityStrategyStub.Object);
      _serviceLocatorScope = new ServiceLocatorScope(serviceLocator);

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
          .Setup(mock => mock.GetAccess(_securityContextStub.Object, _securityPrincipalStub.Object))
          .Returns(new[] { AccessType.Get(GeneralAccessTypes.Read) });

      SecurableObject securableObject;
      using (SecurityFreeSection.Activate())
      {
        securableObject = CreateSecurableObject(_securityContextFactoryStub.Object);
      }

      Dev.Null = securableObject.PropertyWithDefaultPermission;
    }

    [Test]
    public void AccessGranted_PropertyWithCustomPermission ()
    {
      _securityProviderStub
          .Setup(mock => mock.GetAccess(_securityContextStub.Object, _securityPrincipalStub.Object))
          .Returns(new[] { AccessType.Get(TestAccessTypes.First) });

      SecurableObject securableObject;
      using (SecurityFreeSection.Activate())
      {
        securableObject = CreateSecurableObject(_securityContextFactoryStub.Object);
      }

      Dev.Null = securableObject.PropertyWithCustomPermission;
    }

    [Test]
    public void AccessDenied_PropertyWithDefaultPermission ()
    {
      _securityProviderStub.Setup(mock => mock.GetAccess(_securityContextStub.Object, _securityPrincipalStub.Object)).Returns(new AccessType[0]);

      SecurableObject securableObject;
      using (SecurityFreeSection.Activate())
      {
        securableObject = CreateSecurableObject(_securityContextFactoryStub.Object);
      }
      Assert.That(
          () => securableObject.PropertyWithDefaultPermission,
          Throws.InstanceOf<PermissionDeniedException>()
              .With.Message.EqualTo(
                  "Access to method 'get_PropertyWithDefaultPermission' on type 'Remotion.Data.DomainObjects.Security.UnitTests.TestDomain.SecurableObject' has been denied."));
    }

    [Test]
    public void AccessDenied_PropertyWithCustomPermission ()
    {
      _securityProviderStub.Setup(mock => mock.GetAccess(_securityContextStub.Object, _securityPrincipalStub.Object)).Returns(new AccessType[0]);

      SecurableObject securableObject;
      using (SecurityFreeSection.Activate())
      {
        securableObject = CreateSecurableObject(_securityContextFactoryStub.Object);
      }
      Assert.That(
          () => securableObject.PropertyWithCustomPermission,
          Throws.InstanceOf<PermissionDeniedException>()
              .With.Message.EqualTo(
                  "Access to method 'get_PropertyWithCustomPermission' on type 'Remotion.Data.DomainObjects.Security.UnitTests.TestDomain.SecurableObject' has been denied."));
    }

    [Test]
    public void AccessGranted_MixedPropertyWithDefaultPermission ()
    {
      _securityProviderStub
          .Setup(mock => mock.GetAccess(_securityContextStub.Object, _securityPrincipalStub.Object))
          .Returns(new[] { AccessType.Get(GeneralAccessTypes.Read) });

      SecurableObject securableObject;
      using (SecurityFreeSection.Activate())
      {
        securableObject = CreateSecurableObject(_securityContextFactoryStub.Object);
      }

      Dev.Null = ((ISecurableObjectMixin)securableObject).MixedPropertyWithDefaultPermission;
    }

    [Test]
    public void AccessGranted_MixedPropertyWithCustomPermission ()
    {
      _securityProviderStub
          .Setup(mock => mock.GetAccess(_securityContextStub.Object, _securityPrincipalStub.Object))
          .Returns(new[] { AccessType.Get(TestAccessTypes.First) });

      SecurableObject securableObject;
      using (SecurityFreeSection.Activate())
      {
        securableObject = CreateSecurableObject(_securityContextFactoryStub.Object);
      }

      Dev.Null = ((ISecurableObjectMixin)securableObject).MixedPropertyWithCustomPermission;
    }

    [Test]
    public void AccessDenied_MixedPropertyWithDefaultPermission ()
    {
      _securityProviderStub.Setup(mock => mock.GetAccess(_securityContextStub.Object, _securityPrincipalStub.Object)).Returns(new AccessType[0]);

      SecurableObject securableObject;
      using (SecurityFreeSection.Activate())
      {
        securableObject = CreateSecurableObject(_securityContextFactoryStub.Object);
      }

      Assert.That(
          () => ((ISecurableObjectMixin)securableObject).MixedPropertyWithDefaultPermission,
          Throws.InstanceOf<PermissionDeniedException>()
              .With.Message.EqualTo(
                  "Access to method 'get_MixedPropertyWithDefaultPermission' on type 'Remotion.Data.DomainObjects.Security.UnitTests.TestDomain.SecurableObject' has been denied."));
    }

    [Test]
    public void AccessDenied_MixedPropertyWithCustomPermission ()
    {
      _securityProviderStub.Setup(mock => mock.GetAccess(_securityContextStub.Object, _securityPrincipalStub.Object)).Returns(new AccessType[0]);

      SecurableObject securableObject;
      using (SecurityFreeSection.Activate())
      {
        securableObject = CreateSecurableObject(_securityContextFactoryStub.Object);
      }
      Assert.That(
          () => ((ISecurableObjectMixin)securableObject).MixedPropertyWithCustomPermission,
          Throws.InstanceOf<PermissionDeniedException>()
              .With.Message.EqualTo(
                  "Access to method 'get_MixedPropertyWithCustomPermission' on type 'Remotion.Data.DomainObjects.Security.UnitTests.TestDomain.SecurableObject' has been denied."));
    }

    [Test]
    public void AccessDenied_SubTransaction ()
    {
      _securityProviderStub.Setup(mock => mock.GetAccess(_securityContextStub.Object, _securityPrincipalStub.Object)).Returns(new AccessType[0]);

      Assert.That(_clientTransaction.Extensions[SecurityClientTransactionExtension.DefaultKey], Is.Not.Null);

      var subTransaction = _clientTransaction.CreateSubTransaction();
      Assert.That(subTransaction.Extensions[SecurityClientTransactionExtension.DefaultKey], Is.Not.Null);

      using (subTransaction.EnterDiscardingScope())
      {
        SecurableObject securableObject;
        using (SecurityFreeSection.Activate())
        {
          securableObject = CreateSecurableObject(_securityContextFactoryStub.Object, clientTransaction: subTransaction);
        }

        Assert.That(
            () => securableObject.PropertyWithDefaultPermission,
            Throws.InstanceOf<PermissionDeniedException>()
                .With.Message.EqualTo(
                    "Access to method 'get_PropertyWithDefaultPermission' on type 'Remotion.Data.DomainObjects.Security.UnitTests.TestDomain.SecurableObject' has been denied."));
      }
    }

    [Test]
    public void AutomaticCleanup_WhenDomainObjectCtorThrows_DoesNotRequireDeletePermissions ()
    {
      _securityProviderStub.Setup(mock => mock.GetAccess(_securityContextStub.Object, _securityPrincipalStub.Object)).Returns(new AccessType[0]);

      _functionalSecurityStrategyStub
          .Setup(
              mock => mock.HasAccess(
                  typeof(SecurableObject),
                  _securityProviderStub.Object,
                  _securityPrincipalStub.Object,
                  new[] { AccessType.Get(GeneralAccessTypes.Create) }))
          .Returns(true);

      var exception = new Exception("Test.");
      SecurableObject throwingObject = null;

      Assert.That(
          () => CreateSecurableObject(
              _securityContextFactoryStub.Object,
              action: obj =>
              {
                throwingObject = obj;
                throw exception;
              }),
          Throws.Exception.SameAs(exception));

      Assert.That(_clientTransaction.IsEnlisted(throwingObject), Is.False);
    }

    private SecurableObject CreateSecurableObject (
        ISecurityContextFactory securityContextFactory,
        ClientTransaction clientTransaction = null,
        Action<SecurableObject> action = null)
    {
      return SecurableObject.NewObject(
          clientTransaction ?? _clientTransaction,
          ObjectSecurityStrategy.Create(securityContextFactory, InvalidationToken.Create()),
          action);
    }
  }
}
