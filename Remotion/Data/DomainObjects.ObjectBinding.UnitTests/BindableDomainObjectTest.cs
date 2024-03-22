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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.Data.DomainObjects.ObjectBinding.UnitTests
{
  [TestFixture]
  public class BindableDomainObjectTest : TestBase
  {
    private Mock<IBindableDomainObjectImplementation> _implementationMock;
    private Mock<IBusinessObjectProperty> _propertyFake;
    private Mock<IBusinessObjectClass> _businessObjectClassFake;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _implementationMock = new Mock<IBindableDomainObjectImplementation>();

      _propertyFake = new Mock<IBusinessObjectProperty>();
      _businessObjectClassFake = new Mock<IBusinessObjectClass>();
    }

    [Test]
    public void BindableObjectProviderAttribute ()
    {
      Assert.That(typeof(BindableDomainObject).IsDefined(typeof(BindableDomainObjectProviderAttribute), false), Is.True);
    }

    [Test]
    public void BindableObjectBaseClassAttribute ()
    {
      Assert.That(typeof(BindableDomainObject).IsDefined(typeof(BindableObjectBaseClassAttribute), false), Is.True);
    }

    [Test]
    public void CreateImplementation ()
    {
      var instance = SampleBindableDomainObject.NewObject();
      Assert.That(PrivateInvoke.GetNonPublicField(instance, "_implementation"), Is.InstanceOf(typeof(BindableDomainObjectImplementation)));
    }

    [Test]
    public void Implementation_IsInitialized ()
    {
      var instance = SampleBindableDomainObject.NewObject();
      var implementation = (BindableDomainObjectImplementation)PrivateInvoke.GetNonPublicField(instance, "_implementation");
      Assert.That(implementation.BusinessObjectClass, Is.Not.Null);
    }

    [Test]
    public void Implementation_IsInitialized_BeforeDerivedCtorRuns ()
    {
      var instance = SampleBindableDomainObject_AccessingImplementationFromCtor.NewObject();
      Assert.That(instance.DisplayNameFromCtor, Is.Not.Null);
      Assert.That(instance.DisplayNameFromCtor, Is.EqualTo(instance.DisplayName));
    }

    [Test]
    public void Serialization ()
    {
      Assert2.IgnoreIfFeatureSerializationIsDisabled();

      var instance = SampleBindableDomainObject.NewObject();
      instance = Serializer.SerializeAndDeserialize(instance);
      var implementation = (BindableDomainObjectImplementation)PrivateInvoke.GetNonPublicField(instance, "_implementation");
      Assert.That(implementation, Is.Not.Null);
      Assert.That(implementation.BusinessObjectClass, Is.Not.Null);
      Assert.That(implementation.BusinessObjectClass.TargetType, Is.SameAs(typeof(SampleBindableDomainObject)));
    }

    [Test]
    public void Serialization_ViaISerializable ()
    {
      Assert2.IgnoreIfFeatureSerializationIsDisabled();

      var instance = SampleBindableDomainObject_ImplementingISerializable.NewObject();
      instance = Serializer.SerializeAndDeserialize(instance);

      var implementation = (BindableDomainObjectImplementation)PrivateInvoke.GetNonPublicField(instance, "_implementation");
      Assert.That(implementation, Is.Not.Null);
      Assert.That(implementation.BusinessObjectClass, Is.Not.Null);
      Assert.That(implementation.BusinessObjectClass.TargetType, Is.SameAs(typeof(SampleBindableDomainObject_ImplementingISerializable)));
    }

    [Test]
    public void Loading ()
    {
      var newInstanceID = new ObjectID(typeof(SampleBindableDomainObject), Guid.NewGuid());
      try
      {
        StubStorageProvider.LoadDataContainerResult = DataContainer.CreateNew(newInstanceID);
        {
          var instance = newInstanceID.GetObject<SampleBindableDomainObject>();
          var implementation = (BindableDomainObjectImplementation)PrivateInvoke.GetNonPublicField(instance, "_implementation");
          Assert.That(implementation, Is.Not.Null);
          Assert.That(implementation.BusinessObjectClass, Is.Not.Null);
        }
      }
      finally
      {
        StubStorageProvider.LoadDataContainerResult = null;
      }
    }

    [Test]
    public void Reloading ()
    {
      var instance1 = SampleBindableDomainObject.NewObject();
      var implementation1 = (BindableDomainObjectImplementation)PrivateInvoke.GetNonPublicField(instance1, "_implementation");
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        var instance2 = instance1.ID.GetObject<SampleBindableDomainObject>();
        var implementation2 = (BindableDomainObjectImplementation)PrivateInvoke.GetNonPublicField(instance2, "_implementation");
        Assert.That(implementation2, Is.SameAs(implementation1));
      }
    }

    [Test]
    public void ObjectReference ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(SampleBindableDomainObject));
      var instance = LifetimeService.GetObjectReference(TestableClientTransaction, new ObjectID(classDefinition, Guid.NewGuid()));

      var implementation = (BindableDomainObjectImplementation)PrivateInvoke.GetNonPublicField(instance, "_implementation");
      Assert.That(implementation, Is.Not.Null);
      Assert.That(implementation.BusinessObjectClass, Is.Not.Null);
    }

    [Test]
    public void GetProperty ()
    {
      var instance = SampleBindableDomainObject.NewObject(_implementationMock.Object);

      _implementationMock.Setup(mock => mock.GetProperty(_propertyFake.Object)).Returns(12).Verifiable();

      Assert.That(((IBusinessObject)instance).GetProperty(_propertyFake.Object), Is.EqualTo(12));
      _implementationMock.Verify();
    }

    [Test]
    public void SetProperty ()
    {
      var instance = SampleBindableDomainObject.NewObject(_implementationMock.Object);

      _implementationMock.Setup(mock => mock.SetProperty(_propertyFake.Object, 174)).Verifiable();

      ((IBusinessObject)instance).SetProperty(_propertyFake.Object, 174);
      _implementationMock.Verify();
    }

    [Test]
    public void GetPropertyString ()
    {
      var instance = SampleBindableDomainObject.NewObject(_implementationMock.Object);

      _implementationMock.Setup(mock => mock.GetPropertyString(_propertyFake.Object, "gj")).Returns("yay").Verifiable();

      Assert.That(((IBusinessObject)instance).GetPropertyString(_propertyFake.Object, "gj"), Is.EqualTo("yay"));
      _implementationMock.Verify();
    }

    [Test]
    public void DisplayName ()
    {
      var instance = SampleBindableDomainObject.NewObject(_implementationMock.Object);

      _implementationMock.Setup(mock => mock.BaseDisplayName).Returns("Philips").Verifiable();

      Assert.That(instance.DisplayName, Is.EqualTo("Philips"));
      _implementationMock.Verify();
    }

    [Test]
    public void BusinessObjectClass ()
    {
      var instance = SampleBindableDomainObject.NewObject(_implementationMock.Object);

      _implementationMock.Setup(mock => mock.BusinessObjectClass).Returns(_businessObjectClassFake.Object).Verifiable();

      Assert.That(((IBusinessObject)instance).BusinessObjectClass, Is.SameAs(_businessObjectClassFake.Object));
      _implementationMock.Verify();
    }

    [Test]
    public void UniqueIdentifier ()
    {
      var instance = SampleBindableDomainObject.NewObject(_implementationMock.Object);

      _implementationMock.Setup(mock => mock.BaseUniqueIdentifier).Returns("123").Verifiable();

      Assert.That(((IBusinessObjectWithIdentity)instance).UniqueIdentifier, Is.EqualTo("123"));
      _implementationMock.Verify();
    }

    [Test]
    public void BindableDomainObject_IsNotPartOfMapping ()
    {
      Assert.That(MappingConfiguration.Current.GetTypeDefinitions().Where(o => o.ClassType == typeof(BindableDomainObject)), Is.Empty);
    }

    [Test]
    public void RefectionUtilityIsTypeIgnoredForMappingConfiguration_BindableDomainObject_ReturnsTrue ()
    {
      Assert.That(ReflectionUtility.IsTypeIgnoredForMappingConfiguration(typeof(BindableDomainObject)), Is.True);
    }
  }
}
