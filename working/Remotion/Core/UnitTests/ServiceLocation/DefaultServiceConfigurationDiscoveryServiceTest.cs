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
using System.Collections;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.ServiceLocation;
using Remotion.UnitTests.ServiceLocation.TestDomain;
using Rhino.Mocks;

namespace Remotion.UnitTests.ServiceLocation
{
  [TestFixture]
  public class DefaultServiceConfigurationDiscoveryServiceTest
  {
    private DefaultServiceConfigurationDiscoveryService _defaultServiceConfigurationDiscoveryService;
    private ITypeDiscoveryService _typeDiscoveryServiceStub;
    private bool _excludeGlobalTypes;

    [SetUp]
    public void SetUp ()
    {
      _typeDiscoveryServiceStub = MockRepository.GenerateStub<ITypeDiscoveryService> ();
      _defaultServiceConfigurationDiscoveryService = new DefaultServiceConfigurationDiscoveryService (_typeDiscoveryServiceStub);
      _excludeGlobalTypes = !typeof (ImplementationForAttribute).Assembly.GlobalAssemblyCache;
    }

    [Test]
    public void GetDefaultConfiguration_TypeDiscoveryService ()
    {
      _typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (null, _excludeGlobalTypes))
          .Return (new ArrayList { typeof (ITestSingletonConcreteImplementationAttributeType) });
      _typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (typeof (ITestSingletonConcreteImplementationAttributeType), true))
          .Return (new ArrayList { typeof (TestConcreteImplementationAttributeType) });

      var serviceConfigurationEntries = _defaultServiceConfigurationDiscoveryService.GetDefaultConfiguration().ToArray();

      Assert.That (serviceConfigurationEntries, Has.Length.EqualTo (1));
      var serviceConfigurationEntry = serviceConfigurationEntries.Single();
      Assert.That (serviceConfigurationEntry.ServiceType, Is.SameAs (typeof (ITestSingletonConcreteImplementationAttributeType)));

      Assert.That (serviceConfigurationEntry.ImplementationInfos.Count, Is.EqualTo (1));
      Assert.That (serviceConfigurationEntry.ImplementationInfos[0].ImplementationType, Is.EqualTo (typeof (TestConcreteImplementationAttributeType)));
      Assert.That (serviceConfigurationEntry.ImplementationInfos[0].Lifetime, Is.EqualTo (LifetimeKind.Singleton));
    }

    [Test]
    public void GetDefaultConfiguration_TypeDiscoveryService_WithMultipleConcreteImplementationAttributes ()
    {
      _typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (null, _excludeGlobalTypes))
          .Return (new ArrayList { typeof (ITestMultipleConcreteImplementationAttributesType) });
      _typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (typeof (ITestMultipleConcreteImplementationAttributesType), true))
          .Return (
              new ArrayList
              {
                  typeof (TestMultipleConcreteImplementationAttributesType1),
                  typeof (TestMultipleConcreteImplementationAttributesType2),
                  typeof (TestMultipleConcreteImplementationAttributesType3)
              });

      var serviceConfigurationEntries = _defaultServiceConfigurationDiscoveryService.GetDefaultConfiguration().ToArray();

      Assert.That (serviceConfigurationEntries, Has.Length.EqualTo (1));
      var serviceConfigurationEntry = serviceConfigurationEntries.Single();
      Assert.That (serviceConfigurationEntry.ServiceType, Is.SameAs (typeof (ITestMultipleConcreteImplementationAttributesType)));

      Assert.That (serviceConfigurationEntry.ImplementationInfos.Count, Is.EqualTo (3));
      Assert.That (
          serviceConfigurationEntry.ImplementationInfos[0].ImplementationType,
          Is.EqualTo (typeof (TestMultipleConcreteImplementationAttributesType2)));
      Assert.That (serviceConfigurationEntry.ImplementationInfos[0].Lifetime, Is.EqualTo (LifetimeKind.InstancePerDependency));

      Assert.That (
          serviceConfigurationEntry.ImplementationInfos[1].ImplementationType,
          Is.EqualTo (typeof (TestMultipleConcreteImplementationAttributesType3)));
      Assert.That (serviceConfigurationEntry.ImplementationInfos[1].Lifetime, Is.EqualTo (LifetimeKind.InstancePerDependency));

      Assert.That (
          serviceConfigurationEntry.ImplementationInfos[2].ImplementationType,
          Is.EqualTo (typeof (TestMultipleConcreteImplementationAttributesType1)));
      Assert.That (serviceConfigurationEntry.ImplementationInfos[2].Lifetime, Is.EqualTo (LifetimeKind.Singleton));
    }

    [Test]
    public void GetDefaultConfiguration_TypeDiscoveryService_WithMultipleConcreteImplementationAttributes_ReturnsSortedByPosition ()
    {
      _typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (null, _excludeGlobalTypes))
          .Return (new ArrayList { typeof (ITestMultipleConcreteImplementationAttributesType) });
      _typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (typeof (ITestMultipleConcreteImplementationAttributesType), true))
          .Return (
              new ArrayList
              {
                  typeof (TestMultipleConcreteImplementationAttributesType1),
                  typeof (TestMultipleConcreteImplementationAttributesType2),
                  typeof (TestMultipleConcreteImplementationAttributesType3)
              });

      var serviceConfigurationEntry = _defaultServiceConfigurationDiscoveryService.GetDefaultConfiguration().Single();
      Assert.That (
          serviceConfigurationEntry.ImplementationInfos.Select (i => i.ImplementationType),
          Is.EqualTo (
              new[]
              {
                  typeof (TestMultipleConcreteImplementationAttributesType2),
                  typeof (TestMultipleConcreteImplementationAttributesType3),
                  typeof (TestMultipleConcreteImplementationAttributesType1),
              }));
    }

    [Test]
    public void GetDefaultConfiguration_TypeDiscoveryService_WithMixedRegistrationTypes_ThrowsException ()
    {
      _typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (null, _excludeGlobalTypes))
          .Return (new ArrayList { typeof (ITestMixedRegistrationTypes) });
      _typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (typeof (ITestMixedRegistrationTypes), true))
          .Return (
              new ArrayList
              {
                  typeof (TestMixedRegistrationTypes1),
                  typeof (TestMixedRegistrationTypes2),
              });

      Assert.That (
          () => _defaultServiceConfigurationDiscoveryService.GetDefaultConfiguration ().ToArray(),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Invalid configuration of service type "
              + "'Remotion.UnitTests.ServiceLocation.TestDomain.ITestMixedRegistrationTypes'. "
              + "Registration types 'Single' and 'Multiple' cannot be used together.")
              .And.InnerException.Not.Null);
    }

    [Test]
    public void GetDefaultConfiguration_TypeDiscoveryService_WithTypeWithDuplicatePosition ()
    {
      _typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (null, _excludeGlobalTypes))
          .Return (new ArrayList { typeof (ITestMultipleConcreteImplementationAttributesWithDuplicatePositionType) });
      _typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (typeof (ITestMultipleConcreteImplementationAttributesWithDuplicatePositionType), true))
          .Return (new ArrayList { typeof (TestMultipleConcreteImplementationAttributesWithDuplicatePositionType1), typeof (TestMultipleConcreteImplementationAttributesWithDuplicatePositionType2) });

      Assert.That (
          () => _defaultServiceConfigurationDiscoveryService.GetDefaultConfiguration ().ToArray(),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Invalid configuration of service type "
              + "'Remotion.UnitTests.ServiceLocation.TestDomain.ITestMultipleConcreteImplementationAttributesWithDuplicatePositionType'. "
              + "Ambiguous ImplementationForAttribute: Position for registration type 'Single' must be unique.")
              .And.InnerException.Not.Null);
    }

    [Test]
    public void GetDefaultConfiguration_TypeDiscoveryService_WithTypeWithDuplicateImplementation ()
    {
      _typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (null, _excludeGlobalTypes))
          .Return (new ArrayList { typeof (ITestMultipleConcreteImplementationAttributesWithDuplicateImplementationType) });
      _typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (typeof (ITestMultipleConcreteImplementationAttributesWithDuplicateImplementationType), true))
          .Return (new ArrayList { typeof (TestMultipleConcreteImplementationAttributesWithDuplicateImplementationType) });

      Assert.That (
          () => _defaultServiceConfigurationDiscoveryService.GetDefaultConfiguration ().ToArray (),
          Throws.InvalidOperationException
            .With.Message.EqualTo (
              "Invalid configuration of service type "
              + "'Remotion.UnitTests.ServiceLocation.TestDomain.ITestMultipleConcreteImplementationAttributesWithDuplicateImplementationType'. "
              + "Ambiguous ImplementationForAttribute: Implementation type must be unique.")
            .And.InnerException.Not.Null);
    }

    [Test]
    public void GetDefaultConfiguration_TypeDiscoveryService_WithCompoundRegistrationType_NoException ()
    {
      _typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (null, _excludeGlobalTypes))
          .Return (new ArrayList { typeof (ITestCompoundRegistration) });
      _typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (typeof (ITestCompoundRegistration), true))
          .Return (
              new ArrayList
              {
                  typeof (TestCompoundImplementation1),
                  typeof (TestCompoundImplementation2),
                  typeof (TestCompoundRegistration),
              });

      _defaultServiceConfigurationDiscoveryService.GetDefaultConfiguration().ToArray();
    }

    [Test]
    public void GetDefaultConfiguration_TypeDiscoveryService_WithDecoratorRegistrationType_NoException ()
    {
      _typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (null, _excludeGlobalTypes))
          .Return (new ArrayList { typeof (ITestDecoratorRegistration) });
      _typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (typeof (ITestDecoratorRegistration), true))
          .Return (
              new ArrayList
              {
                  typeof (TestDecoratorRegistrationDecoratedObject1),
                  typeof (TestDecoratorRegistrationDecoratedObject2),
                  typeof (TestDecoratorRegistrationDecorator),
              });

      _defaultServiceConfigurationDiscoveryService.GetDefaultConfiguration().ToArray();
    }

    [Test]
    public void GetDefaultConfiguration_TypeDiscoveryService_WithCompoundAndSingleRegistrationType ()
    {
      _typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (null, _excludeGlobalTypes))
          .Return (new ArrayList { typeof (ITestCompoundMixedRegistrationTypes) });
      _typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (typeof (ITestCompoundMixedRegistrationTypes), true))
          .Return (
              new ArrayList
              {
                  typeof (TestCompoundMixedRegistrationTypes),
                  typeof (TestCompoundMixedRegistrationImplementation1),
                  typeof (TestCompoundMixedRegistrationImplementation2),
              });

      Assert.That (
          () => _defaultServiceConfigurationDiscoveryService.GetDefaultConfiguration().ToArray(),
          Throws.InvalidOperationException
              .With.Message.EqualTo (
                  "Invalid configuration of service type "
                  + "'Remotion.UnitTests.ServiceLocation.TestDomain.ITestCompoundMixedRegistrationTypes'. "
                  + "Registration types 'Compound' and 'Single' cannot be used together.")
              .And.InnerException.Not.Null);
    }

    [Test]
    public void GetDefaultConfiguration_WithNoImplementations ()
    {
      _typeDiscoveryServiceStub.Stub (_ => _.GetTypes (null, _excludeGlobalTypes)).IgnoreArguments().Return (new Type[0]);

      var entry = _defaultServiceConfigurationDiscoveryService.GetDefaultConfiguration (typeof (ICollection));

      Assert.That (entry, Is.Not.Null);
      Assert.That (entry.ImplementationInfos, Is.Empty);
    }

    [Test]
    public void GetDefaultConfiguration_WithGlobalType_IncludesGlobalTypes ()
    {
      _typeDiscoveryServiceStub.Stub (_ => _.GetTypes (typeof (ICollection), true)).Return (new Type[0]);

      var entry = _defaultServiceConfigurationDiscoveryService.GetDefaultConfiguration (typeof (ICollection));

      Assert.That (entry.ServiceType, Is.EqualTo (typeof (ICollection)));

      Assert.That (entry.ImplementationInfos, Is.Empty);
    }

    [Test]
    public void GetDefaultConfiguration_WithNonGlobalType_ExcludesGlobalTypes ()
    {
      _typeDiscoveryServiceStub.Stub (_ => _.GetTypes (typeof (ITestSingletonConcreteImplementationAttributeType), true))
          .Return (new[] { typeof (TestConcreteImplementationAttributeType) });

      var entry = _defaultServiceConfigurationDiscoveryService.GetDefaultConfiguration (typeof (ITestSingletonConcreteImplementationAttributeType));

      Assert.That (entry.ServiceType, Is.EqualTo (typeof (ITestSingletonConcreteImplementationAttributeType)));

      Assert.That (entry.ImplementationInfos.Count, Is.EqualTo (1));
      Assert.That (entry.ImplementationInfos[0].ImplementationType, Is.EqualTo (typeof (TestConcreteImplementationAttributeType)));
      Assert.That (entry.ImplementationInfos[0].Lifetime, Is.EqualTo (LifetimeKind.Singleton));
    }

    [Test]
    public void GetDefaultConfiguration_Types ()
    {
      _typeDiscoveryServiceStub.Stub (_ => _.GetTypes (typeof (ITestSingletonConcreteImplementationAttributeType), true))
        .Return (new [] { typeof (TestConcreteImplementationAttributeType) });

      var serviceConfigurationEntries = _defaultServiceConfigurationDiscoveryService.GetDefaultConfiguration (
          new[] { typeof (ITestSingletonConcreteImplementationAttributeType) }).ToArray();

      Assert.That (serviceConfigurationEntries, Has.Length.EqualTo (1));
      var entry = serviceConfigurationEntries.Single();
      Assert.That (entry.ServiceType, Is.EqualTo (typeof (ITestSingletonConcreteImplementationAttributeType)));

      Assert.That (entry.ImplementationInfos.Count, Is.EqualTo (1));
      Assert.That (entry.ImplementationInfos[0].ImplementationType, Is.EqualTo (typeof (TestConcreteImplementationAttributeType)));
      Assert.That (entry.ImplementationInfos[0].Lifetime, Is.EqualTo (LifetimeKind.Singleton));
    }

    [Test]
    public void GetDefaultConfiguration_Types_Unresolvable ()
    {
      _typeDiscoveryServiceStub.Stub (_ => _.GetTypes (typeof (ITestConcreteImplementationAttributeWithUnresolvableImplementationType), true))
        .Return (new Type[0]);
      
      var serviceConfigurationEntries = _defaultServiceConfigurationDiscoveryService.GetDefaultConfiguration (
          new[] { typeof (ITestConcreteImplementationAttributeWithUnresolvableImplementationType) }).ToArray();

      Assert.That (serviceConfigurationEntries, Is.Empty);
    }

    [Test]
    public void GetDefaultConfiguration_WithMultipleRegistrationsWithRegistrationTypeSingle_ReturnsFirstRegistration_OrderedByPositionAscending ()
    {
      _typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (null, _excludeGlobalTypes)).Return (new ArrayList { typeof (ITestRegistrationTypeSingle) });
      _typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (typeof (ITestRegistrationTypeSingle), true))
          .Return (
              new ArrayList
              {
                  typeof (TestRegistrationTypeSingle2),
                  typeof (TestRegistrationTypeSingle1),
              });

      var configurationEntry = _defaultServiceConfigurationDiscoveryService.GetDefaultConfiguration().Single();
      Assert.That (configurationEntry.ImplementationInfos, Has.Count.EqualTo (1));
      Assert.That (configurationEntry.ImplementationInfos.Single().ImplementationType, Is.EqualTo (typeof (TestRegistrationTypeSingle1)));
    }

    [Test]
    public void GetDefaultConfiguration_WithMultipleRegistrationsWithRegistrationTypeCompound_ReturnsFirstRegistration_OrderedByPositionAscending ()
    {
      _typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (null, _excludeGlobalTypes)).Return (new ArrayList { typeof (ITestCompoundRegistration) });
      _typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (typeof (ITestCompoundRegistration), true))
          .Return (
              new ArrayList
              {
                  typeof (TestCompoundRegistration2),
                  typeof (TestCompoundRegistration),
              });

      var configurationEntry = _defaultServiceConfigurationDiscoveryService.GetDefaultConfiguration().Single();
      Assert.That (configurationEntry.ImplementationInfos, Has.Count.EqualTo (1));
      Assert.That (configurationEntry.ImplementationInfos.Single().ImplementationType, Is.EqualTo (typeof (TestCompoundRegistration)));
    }

    [Test]
    public void GetDefaultConfiguration_Assembly ()
    {
      var defaultServiceConfigurationDiscoveryService = DefaultServiceConfigurationDiscoveryService.Create();

      // Because the TestDomain contains test classes with ambiguous attributes, we expect an exception here.
      Assert.That (
          () => defaultServiceConfigurationDiscoveryService.GetDefaultConfiguration (new[] { GetType().Assembly }).ToArray(), 
            Throws.InvalidOperationException);
    }

    [Test]
    public void EnsureAllMembersOfImplementationForAttributeAreCloned ()
    {
      Assert.That (
          typeof (ImplementationForAttribute).GetFields (BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic).Length,
          Is.EqualTo (4),
          "Additional state has been defined on ImplementationForAttributes. These new members need to be added to DefaultServiceConfigurationDiscoveryService.CloneAttribute(...) method.");
    }
  }
}