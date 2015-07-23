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
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.ObjectBinding.Security.UnitTests.TestDomain;
using Remotion.Security;
using Remotion.ServiceLocation;
using Remotion.TypePipe;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Security.UnitTests.BindableObject
{
  [TestFixture]
  public class IntegrationTest : TestBase
  {
    private ISecurityProvider _securityProviderStub;
    private IPrincipalProvider _principalProviderStub;
    private ISecurityPrincipal _securityPrincipalStub;
    private ServiceLocatorScope _serviceLocatorScope;

    public override void SetUp ()
    {
      base.SetUp();

      _securityProviderStub = MockRepository.GenerateStub<ISecurityProvider> ();
      _principalProviderStub = MockRepository.GenerateStub<IPrincipalProvider> ();
      _securityPrincipalStub = MockRepository.GenerateStub<ISecurityPrincipal> ();

      _principalProviderStub.Stub (stub => stub.GetPrincipal ()).Return (_securityPrincipalStub);

      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle (() => _securityProviderStub);
      serviceLocator.RegisterSingle (() => _principalProviderStub);
      _serviceLocatorScope = new ServiceLocatorScope (serviceLocator);
    }

    public override void TearDown ()
    {
      _serviceLocatorScope.Dispose();

      base.TearDown();
    }

    [Test]
    public void AccessGranted_PropertyWithDefaultPermission_IsReadonly ()
    {
      var securityContextStub = MockRepository.GenerateStub<ISecurityContext>();
      var securityContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory>();

      securityContextFactoryStub.Stub (mock => mock.CreateSecurityContext()).Return (securityContextStub);

      _securityProviderStub
          .Stub (mock => mock.GetAccess (securityContextStub, _securityPrincipalStub))
          .Return (new[] { AccessType.Get (GeneralAccessTypes.Read) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateSecurableClassWithProperties (securityContextFactoryStub);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition ("PropertyWithDefaultPermission");

      Assert.That (property.IsReadOnly (bindableSecurableObject), Is.True);
    }

    [Test]
    public void AccessGranted_PropertyWithDefaultPermission_IsAccessible ()
    {
      var securityContextStub = MockRepository.GenerateStub<ISecurityContext>();
      var securityContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory>();

      securityContextFactoryStub.Stub (mock => mock.CreateSecurityContext()).Return (securityContextStub);

      _securityProviderStub
          .Stub (mock => mock.GetAccess (securityContextStub, _securityPrincipalStub))
          .Return (new[] { AccessType.Get (GeneralAccessTypes.Read) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateSecurableClassWithProperties (securityContextFactoryStub);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition ("PropertyWithDefaultPermission");

      Assert.That (property.IsAccessible (bindableSecurableObject), Is.True);
    }

    [Test]
    public void AccessGranted_ReadOnlyProperty_IsReadonly ()
    {
      var securityContextStub = MockRepository.GenerateStub<ISecurityContext>();
      var securityContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory>();

      securityContextFactoryStub.Stub (mock => mock.CreateSecurityContext()).Return (securityContextStub);

      _securityProviderStub
          .Stub (mock => mock.GetAccess (securityContextStub, _securityPrincipalStub))
          .Return (new[] { AccessType.Get (GeneralAccessTypes.Read) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateSecurableClassWithProperties (securityContextFactoryStub);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition ("PropertyWithReadPermission");

      Assert.That (property.IsReadOnly (bindableSecurableObject), Is.True);
    }

    [Test]
    public void NoAccessGranted_ReadOnlyProperty_IsAccessible ()
    {
      var securityContextStub = MockRepository.GenerateStub<ISecurityContext> ();
      var securityContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory> ();

      securityContextFactoryStub.Stub (mock => mock.CreateSecurityContext ()).Return (securityContextStub);

      _securityProviderStub.Stub (mock => mock.GetAccess (securityContextStub, _securityPrincipalStub)).Return (new AccessType[0]);

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateSecurableClassWithProperties (securityContextFactoryStub);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition ("PropertyWithReadPermission");

      Assert.That (property.IsAccessible (bindableSecurableObject), Is.False);
    }

    [Test]
    public void AccessGranted_PropertyWithWriteAccess_IsReadonly ()
    {
      var securityContextStub = MockRepository.GenerateStub<ISecurityContext> ();
      var securityContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory> ();

      securityContextFactoryStub.Stub (mock => mock.CreateSecurityContext ()).Return (securityContextStub);

      _securityProviderStub
          .Stub (mock => mock.GetAccess (securityContextStub, _securityPrincipalStub))
          .Return (new[] { AccessType.Get (GeneralAccessTypes.Read) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateSecurableClassWithProperties (securityContextFactoryStub);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition ("PropertyWithWritePermission");

      Assert.That (property.IsReadOnly (bindableSecurableObject), Is.True);
    }

    [Test]
    public void AccessGranted_PropertyWithWriteAccess_IsAccessible ()
    {
      var securityContextStub = MockRepository.GenerateStub<ISecurityContext>();
      var securityContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory>();

      securityContextFactoryStub.Stub (mock => mock.CreateSecurityContext()).Return (securityContextStub);

      _securityProviderStub
          .Stub (mock => mock.GetAccess (securityContextStub, _securityPrincipalStub))
          .Return (new[] { AccessType.Get (GeneralAccessTypes.Read) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateSecurableClassWithProperties (securityContextFactoryStub);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition ("PropertyWithWritePermission");

      Assert.That (property.IsAccessible (bindableSecurableObject), Is.True);
    }

    [Test]
    public void AccessGranted_DefaultPropertyInMixedClass_IsReadOnly ()
    {
      var securityContextStub = MockRepository.GenerateStub<ISecurityContext>();
      var securityContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory>();

      securityContextFactoryStub.Stub (mock => mock.CreateSecurityContext()).Return (securityContextStub);

      _securityProviderStub
          .Stub (mock => mock.GetAccess (securityContextStub, _securityPrincipalStub))
          .Return (new[] { AccessType.Get (GeneralAccessTypes.Read) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateSecurableClassWithProperties (securityContextFactoryStub);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition ("MixedPropertyWithDefaultPermission");

      Assert.That (property.IsReadOnly (bindableSecurableObject), Is.True);
    }

    [Test]
    public void AccessGranted_DefaultPropertyInMixedClass_IsAccessible ()
    {
      var securityContextStub = MockRepository.GenerateStub<ISecurityContext>();
      var securityContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory>();

      securityContextFactoryStub.Stub (mock => mock.CreateSecurityContext()).Return (securityContextStub);

      _securityProviderStub
          .Stub (mock => mock.GetAccess (securityContextStub, _securityPrincipalStub))
          .Return (new[] { AccessType.Get (GeneralAccessTypes.Read) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateSecurableClassWithProperties (securityContextFactoryStub);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition ("MixedPropertyWithDefaultPermission");

      Assert.That (property.IsAccessible (bindableSecurableObject), Is.True);
    }

    [Test]
    public void AccessGranted_ReadOnlyPropertyInMixedClass_IsReadOnly ()
    {
      var securityContextStub = MockRepository.GenerateStub<ISecurityContext>();
      var securityContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory>();

      securityContextFactoryStub.Stub (mock => mock.CreateSecurityContext()).Return (securityContextStub);

      _securityProviderStub
          .Stub (mock => mock.GetAccess (securityContextStub, _securityPrincipalStub))
          .Return (new[] { AccessType.Get (GeneralAccessTypes.Read) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateSecurableClassWithProperties (securityContextFactoryStub);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition ("MixedPropertyWithReadPermission");

      Assert.That (property.IsReadOnly (bindableSecurableObject), Is.True);
    }

    [Test]
    public void NoAccessGranted_ReadOnlyPropertyInMixedClass_IsAccessible ()
    {
      var securityContextStub = MockRepository.GenerateStub<ISecurityContext> ();
      var securityContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory> ();

      securityContextFactoryStub.Stub (mock => mock.CreateSecurityContext ()).Return (securityContextStub);

      _securityProviderStub.Stub (mock => mock.GetAccess (securityContextStub, _securityPrincipalStub)).Return (new AccessType[0]);

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateSecurableClassWithProperties (securityContextFactoryStub);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition ("MixedPropertyWithReadPermission");

      Assert.That (property.IsAccessible (bindableSecurableObject), Is.False);
    }

    [Test]
    public void AccessGranted_WritablePropertyInMixedClass_IsReadOnly ()
    {
      var securityContextStub = MockRepository.GenerateStub<ISecurityContext>();
      var securityContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory>();

      securityContextFactoryStub.Stub (mock => mock.CreateSecurityContext()).Return (securityContextStub);

      _securityProviderStub
          .Stub (mock => mock.GetAccess (securityContextStub, _securityPrincipalStub))
          .Return (new[] { AccessType.Get (GeneralAccessTypes.Read) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateSecurableClassWithProperties (securityContextFactoryStub);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition ("MixedPropertyWithWritePermission");

      Assert.That (property.IsReadOnly (bindableSecurableObject), Is.True);
    }

    [Test]
    public void AccessGranted_WritablePropertyInMixedClass_IsAccessible ()
    {
      var securityContextStub = MockRepository.GenerateStub<ISecurityContext>();
      var securityContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory>();

      securityContextFactoryStub.Stub (mock => mock.CreateSecurityContext()).Return (securityContextStub);

      _securityProviderStub
          .Stub (mock => mock.GetAccess (securityContextStub, _securityPrincipalStub))
          .Return (new[] { AccessType.Get (GeneralAccessTypes.Read) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateSecurableClassWithProperties (securityContextFactoryStub);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition ("MixedPropertyWithWritePermission");

      Assert.That (property.IsAccessible (bindableSecurableObject), Is.True);
    }

    //derived

    [Test]
    public void AccessGranted_WritablePropertyInDerivedClass_IsReadOnly ()
    {
      var securityContextStub = MockRepository.GenerateStub<ISecurityContext>();
      var securityContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory>();

      securityContextFactoryStub.Stub (mock => mock.CreateSecurityContext()).Return (securityContextStub);

      _securityProviderStub
          .Stub (mock => mock.GetAccess (securityContextStub, _securityPrincipalStub))
          .Return (new[] { AccessType.Get (GeneralAccessTypes.Read) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateDerivedSecurableClassWithProperties (securityContextFactoryStub);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition ("PropertyToOverrideWithWritePermission");

      Assert.That (property.IsReadOnly (bindableSecurableObject), Is.True);
    }

    [Test]
    public void AccessGranted_WritablePropertyInDeriveClass_IsAccessible ()
    {
      var securityContextStub = MockRepository.GenerateStub<ISecurityContext>();
      var securityContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory>();

      securityContextFactoryStub.Stub (mock => mock.CreateSecurityContext()).Return (securityContextStub);

      _securityProviderStub
          .Stub (mock => mock.GetAccess (securityContextStub, _securityPrincipalStub))
          .Return (new[] { AccessType.Get (GeneralAccessTypes.Read) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateDerivedSecurableClassWithProperties (securityContextFactoryStub);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition ("PropertyToOverrideWithWritePermission");

      Assert.That (property.IsAccessible (bindableSecurableObject), Is.True);
    }

    [Test]
    public void AccessGranted_ReadOnlyPropertyInDerivedClass_IsReadOnly ()
    {
      var securityContextStub = MockRepository.GenerateStub<ISecurityContext>();
      var securityContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory>();

      securityContextFactoryStub.Stub (mock => mock.CreateSecurityContext()).Return (securityContextStub);

      _securityProviderStub
          .Stub (mock => mock.GetAccess (securityContextStub, _securityPrincipalStub))
          .Return (new[] { AccessType.Get (GeneralAccessTypes.Read) });

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateDerivedSecurableClassWithProperties (securityContextFactoryStub);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition ("PropertyToOverrideWithReadPermission");

      Assert.That (property.IsReadOnly (bindableSecurableObject), Is.True);
    }

    [Test]
    public void NoAccessGranted_ReadOnlyPropertyInDeriveClass_IsAccessible ()
    {
      var securityContextStub = MockRepository.GenerateStub<ISecurityContext> ();
      var securityContextFactoryStub = MockRepository.GenerateStub<ISecurityContextFactory> ();

      securityContextFactoryStub.Stub (mock => mock.CreateSecurityContext ()).Return (securityContextStub);

      _securityProviderStub.Stub (mock => mock.GetAccess (securityContextStub, _securityPrincipalStub)).Return (new AccessType[0]);

      IBusinessObject bindableSecurableObject;
      using (SecurityFreeSection.Activate())
      {
        bindableSecurableObject = CreateDerivedSecurableClassWithProperties (securityContextFactoryStub);
      }
      var businessObjectClass = bindableSecurableObject.BusinessObjectClass;
      var property = businessObjectClass.GetPropertyDefinition ("PropertyToOverrideWithReadPermission");

      Assert.That (property.IsAccessible (bindableSecurableObject), Is.False);
    }

    private IBusinessObject CreateSecurableClassWithProperties (ISecurityContextFactory securityContextFactoryStub)
    {
      return (IBusinessObject) ObjectFactory.Create (
          false,
          typeof (SecurableClassWithProperties),
          ParamList.Create (ObjectSecurityStrategy.Create (securityContextFactoryStub, InvalidationToken.Create())));
    }

    private IBusinessObject CreateDerivedSecurableClassWithProperties (ISecurityContextFactory securityContextFactoryStub)
    {
      return (IBusinessObject) ObjectFactory.Create (
          false,
          typeof (DerivedSecurableClassWithProperties),
          ParamList.Create (ObjectSecurityStrategy.Create (securityContextFactoryStub, InvalidationToken.Create())));
    }
  }
}