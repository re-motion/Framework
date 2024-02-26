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
using Remotion.Data.DomainObjects.ObjectBinding.IntegrationTests.TestDomain.Security;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Security;
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding;
using Remotion.Security;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.ObjectBinding.IntegrationTests
{
  [TestFixture]
  public class SecurityIntegrationTest : TestBase
  {
    private Mock<ISecurityProvider> _securityProviderStub;
    private Mock<IPrincipalProvider> _principalProviderStub;
    private ClientTransaction _clientTransaction;
    private Mock<ISecurityPrincipal> _securityPrincipalStub;
    private ServiceLocatorScope _serviceLocatorScope;

    public override void SetUp ()
    {
      base.SetUp();

      BusinessObjectProvider.SetProvider(typeof(BindableDomainObjectProviderAttribute), null);

      _securityProviderStub = new Mock<ISecurityProvider>();
      _principalProviderStub = new Mock<IPrincipalProvider>();
      _securityPrincipalStub = new Mock<ISecurityPrincipal>();

      _principalProviderStub.Setup(stub => stub.GetPrincipal()).Returns(_securityPrincipalStub.Object);

      _clientTransaction = ClientTransaction.CreateRootTransaction();
      _clientTransaction.Extensions.Add(new SecurityClientTransactionExtension());

      _clientTransaction.EnterNonDiscardingScope();

      var storageSettings = SafeServiceLocator.Current.GetInstance<IStorageSettings>();

      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle(() => _securityProviderStub.Object);
      serviceLocator.RegisterSingle(() => _principalProviderStub.Object);
      serviceLocator.RegisterSingle(() => storageSettings);
      _serviceLocatorScope = new ServiceLocatorScope(serviceLocator);
    }

    public override void TearDown ()
    {
      ClientTransactionScope.ResetActiveScope();
      _serviceLocatorScope.Dispose();

      BusinessObjectProvider.SetProvider(typeof(BindableDomainObjectProviderAttribute), null);

      base.TearDown();
    }

    [Test]
    public void IsReadonly_PropertyWithDefaultPermission_False ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);
      _securityProviderStub.Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object)).Returns(new[] { AccessType.Get(GeneralAccessTypes.Edit) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateBindableSecurableObject(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("PropertyWithDefaultPermission");

      Assert.That(property.IsReadOnly(bindableSecurableObject), Is.False);
    }

    [Test]
    public void IsReadonly_PropertyWithDefaultPermission_True ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);
      _securityProviderStub.Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object)).Returns(new AccessType[0]);

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateBindableSecurableObject(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("PropertyWithDefaultPermission");

      Assert.That(property.IsReadOnly(bindableSecurableObject), Is.True);
    }

    [Test]
    public void IsAccessible_PropertyWithDefaultPermission_True ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);
      _securityProviderStub.Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object)).Returns(new[] { AccessType.Get(GeneralAccessTypes.Read) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateBindableSecurableObject(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("PropertyWithDefaultPermission");

      Assert.That(property.IsAccessible(bindableSecurableObject), Is.True);
    }

    [Test]
    public void IsAccessible_PropertyWithDefaultPermission_False ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);
      _securityProviderStub.Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object)).Returns(new AccessType[0]);

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateBindableSecurableObject(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("PropertyWithDefaultPermission");

      Assert.That(property.IsAccessible(bindableSecurableObject), Is.False);
    }

    [Test]
    public void IsReadonly_PropertyWitCustomPermission_False ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);
      _securityProviderStub.Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object)).Returns(new[] { AccessType.Get(TestAccessTypes.Second) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateBindableSecurableObject(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("PropertyWithCustomPermission");

      Assert.That(property.IsReadOnly(bindableSecurableObject), Is.False);
    }

    [Test]
    public void IsReadonly_PropertyWithCustomPermission_True ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);
      _securityProviderStub.Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object)).Returns(new AccessType[0]);

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateBindableSecurableObject(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("PropertyWithCustomPermission");

      Assert.That(property.IsReadOnly(bindableSecurableObject), Is.True);
    }

    [Test]
    public void IsAccessible_PropertyWithCustomPermission_True ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);
      _securityProviderStub.Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object)).Returns(new[] { AccessType.Get(TestAccessTypes.First) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateBindableSecurableObject(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("PropertyWithCustomPermission");

      Assert.That(property.IsAccessible(bindableSecurableObject), Is.True);
    }

    [Test]
    public void IsAccessible_PropertyWithCustomPermission_False ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);
      _securityProviderStub.Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object)).Returns(new AccessType[0]);

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateBindableSecurableObject(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("PropertyWithCustomPermission");

      Assert.That(property.IsAccessible(bindableSecurableObject), Is.False);
    }


    [Test]
    public void IsReadonly_ReadOnlyProperty_True ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateBindableSecurableObject(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("ReadOnlyProperty");

      Assert.That(property.IsReadOnly(bindableSecurableObject), Is.True);
    }

    [Test]
    public void IsReadonly_CollectionPropertyWithoutSetter_False ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);
      _securityProviderStub.Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object)).Returns(new[] { AccessType.Get(GeneralAccessTypes.Edit) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateBindableSecurableObject(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("Children");

      Assert.That(property.IsReadOnly(bindableSecurableObject), Is.False);
    }

    [Test]
    public void IsReadonly_CollectionPropertyWithoutSetter_True ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);
      _securityProviderStub.Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object)).Returns(new AccessType[0]);

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateBindableSecurableObject(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("Children");

      Assert.That(property.IsReadOnly(bindableSecurableObject), Is.True);
    }

    [Test]
    public void IsReadOnly_MixedPropertyWithDefaultPermission ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);
      _securityProviderStub.Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object)).Returns(new[] { AccessType.Get(GeneralAccessTypes.Read) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateBindableSecurableObject(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("MixedPropertyWithDefaultPermission");

      Assert.That(property.IsReadOnly(bindableSecurableObject), Is.True);
    }

    [Test]
    public void IsAccessible_MixedPropertyWithDefaultPermission_True ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);
      _securityProviderStub.Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object)).Returns(new[] { AccessType.Get(GeneralAccessTypes.Read) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateBindableSecurableObject(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("MixedPropertyWithDefaultPermission");

      Assert.That(property.IsAccessible(bindableSecurableObject), Is.True);
    }

    [Test]
    public void IsReadOnly_MixedPropertyWithReadPermission ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);
      _securityProviderStub.Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object)).Returns(new[] { AccessType.Get(GeneralAccessTypes.Read) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateBindableSecurableObject(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("MixedPropertyWithReadPermission");

      Assert.That(property.IsReadOnly(bindableSecurableObject), Is.True);
    }

    [Test]
    public void IsAccessible_MixedPropertyWithReadPermission_False ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);
      _securityProviderStub.Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object)).Returns(new AccessType[0]);

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateBindableSecurableObject(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("MixedPropertyWithReadPermission");

      Assert.That(property.IsAccessible(bindableSecurableObject), Is.False);
    }

    [Test]
    public void IsReadOnly_MixedPropertyWithWritePermission ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);
      _securityProviderStub.Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object)).Returns(new[] { AccessType.Get(GeneralAccessTypes.Read) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateBindableSecurableObject(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("MixedPropertyWithWritePermission");

      Assert.That(property.IsReadOnly(bindableSecurableObject), Is.True);
    }

    [Test]
    public void IsAccessible_MixedPropertyWithWritePermission_True ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);
      _securityProviderStub.Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object)).Returns(new[] { AccessType.Get(GeneralAccessTypes.Read) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateBindableSecurableObject(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("MixedPropertyWithWritePermission");

      Assert.That(property.IsAccessible(bindableSecurableObject), Is.True);
    }

    [Test]
    public void IsReadOnly_DerivedReadOnlyProperty ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);
      _securityProviderStub.Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object)).Returns(new[] { AccessType.Get(GeneralAccessTypes.Read) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateDerivedBindableSecurableObject(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("PropertyToOverride");

      Assert.That(property.IsReadOnly(bindableSecurableObject), Is.True);
    }

    [Test]
    public void NoTest_DerivedReadOnlyProperty_IsNotAccessible ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);
      _securityProviderStub.Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object)).Returns(new[] { AccessType.Get(TestAccessTypes.First) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateDerivedBindableSecurableObject(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("PropertyToOverride");

      Assert.That(property.IsAccessible(bindableSecurableObject), Is.False);
    }

    #region MixinPermissionTests

    [Test]
    public void IsReadOnly_DefaultPermissionMixedProperty_False ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);
      _securityProviderStub.Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object)).Returns(new[] { AccessType.Get(GeneralAccessTypes.Edit) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateBindableSecurableObject(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("DefaultPermissionMixedProperty");

      Assert.That(property.IsReadOnly(bindableSecurableObject), Is.False);
    }

    [Test]
    public void IsReadOnly_DefaultPermissionMixedProperty_True ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);
      _securityProviderStub.Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object)).Returns(new AccessType[0]);

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateBindableSecurableObject(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("DefaultPermissionMixedProperty");

      Assert.That(property.IsReadOnly(bindableSecurableObject), Is.True);
    }

    [Test]
    public void IsAccessible_DefaultPermissionMixedProperty_True ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);
      _securityProviderStub.Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object)).Returns(new[] { AccessType.Get(GeneralAccessTypes.Read) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateBindableSecurableObject(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("DefaultPermissionMixedProperty");

      Assert.That(property.IsAccessible(bindableSecurableObject), Is.True);
    }

    [Test]
    public void IsAccessible_DefaultPermissionMixedProperty_False ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);
      _securityProviderStub.Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object)).Returns(new AccessType[0]);

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateBindableSecurableObject(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("DefaultPermissionMixedProperty");

      Assert.That(property.IsAccessible(bindableSecurableObject), Is.False);
    }

    [Test]
    public void IsReadonly_CustomPermissionMixedProperty_False ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);
      _securityProviderStub.Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object)).Returns(new[] { AccessType.Get(TestAccessTypes.Second) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateBindableSecurableObject(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("CustomPermissionMixedProperty");

      Assert.That(property.IsReadOnly(bindableSecurableObject), Is.False);
    }

    [Test]
    public void IsReadonly_CustomPermissionMixedProperty_True ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);
      _securityProviderStub.Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object)).Returns(new AccessType[0]);

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateBindableSecurableObject(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("CustomPermissionMixedProperty");

      Assert.That(property.IsReadOnly(bindableSecurableObject), Is.True);
    }

    [Test]
    public void IsAccessible_CustomPermissionMixedProperty_True ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);
      _securityProviderStub.Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object)).Returns(new[] { AccessType.Get(TestAccessTypes.First) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateBindableSecurableObject(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("CustomPermissionMixedProperty");

      Assert.That(property.IsAccessible(bindableSecurableObject), Is.True);
    }

    [Test]
    public void IsAccessible_CustomPermissionMixedProperty_False ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);
      _securityProviderStub.Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object)).Returns(new AccessType[0]);

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateBindableSecurableObject(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("CustomPermissionMixedProperty");

      Assert.That(property.IsAccessible(bindableSecurableObject), Is.False);
    }

    #endregion

    private IBusinessObject CreateBindableSecurableObject (ISecurityContextFactory securityContextFactoryStub)
    {
      return (IBusinessObject)BindableSecurableObject.NewObject(
              _clientTransaction,
              ObjectSecurityStrategy.Create(securityContextFactoryStub, InvalidationToken.Create()));
    }

    private IBusinessObject CreateDerivedBindableSecurableObject (ISecurityContextFactory securityContextFactoryStub)
    {
      return (IBusinessObject)DerivedBindableSecurableObject.NewObject(
              _clientTransaction,
              ObjectSecurityStrategy.Create(securityContextFactoryStub, InvalidationToken.Create()));
    }
  }
}
