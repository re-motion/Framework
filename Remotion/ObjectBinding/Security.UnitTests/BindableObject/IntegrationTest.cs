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
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.ObjectBinding.Security.UnitTests.TestDomain;
using Remotion.Security;
using Remotion.ServiceLocation;
using Remotion.TypePipe;

namespace Remotion.ObjectBinding.Security.UnitTests.BindableObject
{
  [TestFixture]
  public class IntegrationTest : TestBase
  {
    private Mock<ISecurityProvider> _securityProviderStub;
    private Mock<IPrincipalProvider> _principalProviderStub;
    private Mock<ISecurityPrincipal> _securityPrincipalStub;
    private ServiceLocatorScope _serviceLocatorScope;

    public override void SetUp ()
    {
      base.SetUp();

      _securityProviderStub = new Mock<ISecurityProvider>();
      _principalProviderStub = new Mock<IPrincipalProvider>();
      _securityPrincipalStub = new Mock<ISecurityPrincipal>();

      _principalProviderStub.Setup(stub => stub.GetPrincipal()).Returns(_securityPrincipalStub.Object);

      var serviceLocator = DefaultServiceLocator.CreateWithBootstrappedServices();
      serviceLocator.RegisterSingle(() => _securityProviderStub.Object);
      serviceLocator.RegisterSingle(() => _principalProviderStub.Object);
      _serviceLocatorScope = new ServiceLocatorScope(serviceLocator);
    }

    public override void TearDown ()
    {
      _serviceLocatorScope.Dispose();

      base.TearDown();
    }

    [Test]
    public void AccessGranted_PropertyWithDefaultPermission_IsReadonly ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);

      _securityProviderStub
          .Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object))
          .Returns(new[] { AccessType.Get(GeneralAccessTypes.Read) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateSecurableClassWithProperties(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("PropertyWithDefaultPermission");

      Assert.That(property.IsReadOnly(bindableSecurableObject), Is.True);
    }

    [Test]
    public void AccessGranted_PropertyWithDefaultPermission_IsAccessible ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);

      _securityProviderStub
          .Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object))
          .Returns(new[] { AccessType.Get(GeneralAccessTypes.Read) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateSecurableClassWithProperties(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("PropertyWithDefaultPermission");

      Assert.That(property.IsAccessible(bindableSecurableObject), Is.True);
    }

    [Test]
    public void AccessGranted_ReadOnlyProperty_IsReadonly ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);

      _securityProviderStub
          .Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object))
          .Returns(new[] { AccessType.Get(GeneralAccessTypes.Read) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateSecurableClassWithProperties(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("PropertyWithReadPermission");

      Assert.That(property.IsReadOnly(bindableSecurableObject), Is.True);
    }

    [Test]
    public void NoAccessGranted_ReadOnlyProperty_IsAccessible ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);

      _securityProviderStub.Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object)).Returns(new AccessType[0]);

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateSecurableClassWithProperties(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("PropertyWithReadPermission");

      Assert.That(property.IsAccessible(bindableSecurableObject), Is.False);
    }

    [Test]
    public void AccessGranted_PropertyWithWriteAccess_IsReadonly ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);

      _securityProviderStub
          .Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object))
          .Returns(new[] { AccessType.Get(GeneralAccessTypes.Read) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateSecurableClassWithProperties(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("PropertyWithWritePermission");

      Assert.That(property.IsReadOnly(bindableSecurableObject), Is.True);
    }

    [Test]
    public void AccessGranted_PropertyWithWriteAccess_IsAccessible ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);

      _securityProviderStub
          .Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object))
          .Returns(new[] { AccessType.Get(GeneralAccessTypes.Read) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateSecurableClassWithProperties(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("PropertyWithWritePermission");

      Assert.That(property.IsAccessible(bindableSecurableObject), Is.True);
    }

    [Test]
    public void AccessGranted_DefaultPropertyInMixedClass_IsReadOnly ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);

      _securityProviderStub
          .Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object))
          .Returns(new[] { AccessType.Get(GeneralAccessTypes.Read) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateSecurableClassWithProperties(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("MixedPropertyWithDefaultPermission");

      Assert.That(property.IsReadOnly(bindableSecurableObject), Is.True);
    }

    [Test]
    public void AccessGranted_DefaultPropertyInMixedClass_IsAccessible ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);

      _securityProviderStub
          .Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object))
          .Returns(new[] { AccessType.Get(GeneralAccessTypes.Read) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateSecurableClassWithProperties(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("MixedPropertyWithDefaultPermission");

      Assert.That(property.IsAccessible(bindableSecurableObject), Is.True);
    }

    [Test]
    public void AccessGranted_ReadOnlyPropertyInMixedClass_IsReadOnly ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);

      _securityProviderStub
          .Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object))
          .Returns(new[] { AccessType.Get(GeneralAccessTypes.Read) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateSecurableClassWithProperties(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("MixedPropertyWithReadPermission");

      Assert.That(property.IsReadOnly(bindableSecurableObject), Is.True);
    }

    [Test]
    public void NoAccessGranted_ReadOnlyPropertyInMixedClass_IsAccessible ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);

      _securityProviderStub.Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object)).Returns(new AccessType[0]);

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateSecurableClassWithProperties(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("MixedPropertyWithReadPermission");

      Assert.That(property.IsAccessible(bindableSecurableObject), Is.False);
    }

    [Test]
    public void AccessGranted_WritablePropertyInMixedClass_IsReadOnly ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);

      _securityProviderStub
          .Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object))
          .Returns(new[] { AccessType.Get(GeneralAccessTypes.Read) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateSecurableClassWithProperties(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("MixedPropertyWithWritePermission");

      Assert.That(property.IsReadOnly(bindableSecurableObject), Is.True);
    }

    [Test]
    public void AccessGranted_WritablePropertyInMixedClass_IsAccessible ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);

      _securityProviderStub
          .Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object))
          .Returns(new[] { AccessType.Get(GeneralAccessTypes.Read) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateSecurableClassWithProperties(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("MixedPropertyWithWritePermission");

      Assert.That(property.IsAccessible(bindableSecurableObject), Is.True);
    }

    //derived

    [Test]
    public void AccessGranted_WritablePropertyInDerivedClass_IsReadOnly ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);

      _securityProviderStub
          .Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object))
          .Returns(new[] { AccessType.Get(GeneralAccessTypes.Read) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateDerivedSecurableClassWithProperties(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("PropertyToOverrideWithWritePermission");

      Assert.That(property.IsReadOnly(bindableSecurableObject), Is.True);
    }

    [Test]
    public void AccessGranted_WritablePropertyInDeriveClass_IsAccessible ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);

      _securityProviderStub
          .Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object))
          .Returns(new[] { AccessType.Get(GeneralAccessTypes.Read) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateDerivedSecurableClassWithProperties(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("PropertyToOverrideWithWritePermission");

      Assert.That(property.IsAccessible(bindableSecurableObject), Is.True);
    }

    [Test]
    public void AccessGranted_ReadOnlyPropertyInDerivedClass_IsReadOnly ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);

      _securityProviderStub
          .Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object))
          .Returns(new[] { AccessType.Get(GeneralAccessTypes.Read) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateDerivedSecurableClassWithProperties(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("PropertyToOverrideWithReadPermission");

      Assert.That(property.IsReadOnly(bindableSecurableObject), Is.True);
    }

    [Test]
    public void NoAccessGranted_ReadOnlyPropertyInDeriveClass_IsAccessible ()
    {
      var securityContextStub = new Mock<ISecurityContext>();
      var securityContextFactoryStub = new Mock<ISecurityContextFactory>();

      securityContextFactoryStub.Setup(mock => mock.CreateSecurityContext()).Returns(securityContextStub.Object);

      _securityProviderStub.Setup(mock => mock.GetAccess(securityContextStub.Object, _securityPrincipalStub.Object)).Returns(new AccessType[0]);

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateDerivedSecurableClassWithProperties(securityContextFactoryStub.Object);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition("PropertyToOverrideWithReadPermission");

      Assert.That(property.IsAccessible(bindableSecurableObject), Is.False);
    }

    private IBusinessObject CreateSecurableClassWithProperties (ISecurityContextFactory securityContextFactoryStub)
    {
      return (IBusinessObject)ObjectFactory.Create(
          false,
          typeof(SecurableClassWithProperties),
          ParamList.Create(ObjectSecurityStrategy.Create(securityContextFactoryStub, InvalidationToken.Create())));
    }

    private IBusinessObject CreateDerivedSecurableClassWithProperties (ISecurityContextFactory securityContextFactoryStub)
    {
      return (IBusinessObject)ObjectFactory.Create(
          false,
          typeof(DerivedSecurableClassWithProperties),
          ParamList.Create(ObjectSecurityStrategy.Create(securityContextFactoryStub, InvalidationToken.Create())));
    }
  }
}
