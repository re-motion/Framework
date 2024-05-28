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
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class BindableObjectProviderTest : TestBase
  {
    private BindableObjectProvider _provider;
    private Mock<IMetadataFactory> _metadataFactoryStub;
    private Mock<IBusinessObjectServiceFactory> _serviceFactoryStub;

    public override void SetUp ()
    {
      base.SetUp();

      _provider = new BindableObjectProvider();
      BindableObjectProvider.SetProvider(typeof(BindableObjectProviderAttribute), _provider);
      _metadataFactoryStub = new Mock<IMetadataFactory>();
      _serviceFactoryStub = new Mock<IBusinessObjectServiceFactory>();
    }

    [Test]
    public void GetProviderForBindableObjectType ()
    {
      BindableObjectProvider provider = BindableObjectProvider.GetProviderForBindableObjectType(typeof(SimpleBusinessObjectClass));

      Assert.That(provider, Is.Not.Null);
      Assert.That(provider, Is.SameAs(BusinessObjectProvider.GetProvider(typeof(BindableObjectProviderAttribute))));
    }

    [Test]
    public void GetProviderForBindableObjectType_WithIdentityType ()
    {
      BindableObjectProvider provider = BindableObjectProvider.GetProviderForBindableObjectType(typeof(ClassWithIdentity));

      Assert.That(provider, Is.Not.Null);
      Assert.That(provider, Is.SameAs(BusinessObjectProvider.GetProvider(typeof(BindableObjectWithIdentityProviderAttribute))));
      Assert.That(provider, Is.Not.SameAs(BusinessObjectProvider.GetProvider(typeof(BindableObjectProviderAttribute))));
    }

    [Test]
    public void GetProviderForBindableObjectType_WithAttributeFromTypeOverridingAttributeFromMixin ()
    {
      BindableObjectProvider provider =
          BindableObjectProvider.GetProviderForBindableObjectType(typeof(DerivedBusinessObjectClassWithSpecificBusinessObjectProviderAttribute));

      Assert.That(provider, Is.Not.Null);
      Assert.That(provider, Is.SameAs(BusinessObjectProvider.GetProvider(typeof(BindableObjectWithIdentityProviderAttribute))));
      Assert.That(provider, Is.Not.SameAs(BusinessObjectProvider.GetProvider(typeof(BindableObjectProviderAttribute))));
    }

    [Test]
    public void GetProviderForBindableObjectType_WithAttributeFromTypeOverridingAttributeFromInheritedMixin ()
    {
      BindableObjectProvider provider =
          BindableObjectProvider.GetProviderForBindableObjectType(
              typeof(DerivedBusinessObjectClassWithoutAttributeAndWithSpecificBusinessObjectProviderAttribute));

      Assert.That(provider, Is.Not.Null);
      Assert.That(provider, Is.SameAs(BusinessObjectProvider.GetProvider(typeof(BindableObjectWithIdentityProviderAttribute))));
      Assert.That(provider, Is.Not.SameAs(BusinessObjectProvider.GetProvider(typeof(BindableObjectProviderAttribute))));
    }

    [Test]
    public void GetProviderForBindableObjectType_WithAttributeFromMixinOverridingAttributeInheritedFromBase ()
    {
      BindableObjectProvider provider =
          BindableObjectProvider.GetProviderForBindableObjectType(
              typeof(DerivedBusinessObjectClassWithSpecificBusinessObjectProviderAttributeFromMixin));

      Assert.That(provider, Is.Not.Null);
      Assert.That(provider, Is.SameAs(BusinessObjectProvider.GetProvider(typeof(BindableObjectWithIdentityProviderAttribute))));
      Assert.That(provider, Is.Not.SameAs(BusinessObjectProvider.GetProvider(typeof(BindableObjectProviderAttribute))));
    }

    [Test]
    public void GetProviderForBindableObjectType_WithMissingProviderAttribute ()
    {
      Assert.That(
          () => BindableObjectProvider.GetProviderForBindableObjectType(typeof(ManualBusinessObject)),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The type 'Remotion.ObjectBinding.UnitTests.TestDomain.ManualBusinessObject' does not have the "
                  + "'Remotion.ObjectBinding.BusinessObjectProviderAttribute' applied.", "type"));
    }

    [Test]
    public void GetProviderForBindableObjectType_WithInvalidProviderType ()
    {
      Assert.That(
          () => BindableObjectProvider.GetProviderForBindableObjectType(typeof(BindableObjectWithStubBusinessObjectProvider)),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The business object provider associated with the type 'Remotion.ObjectBinding.UnitTests.TestDomain.BindableObjectWithStubBusinessObjectProvider' "
                  + "is not of type 'Remotion.ObjectBinding.BindableObject.BindableObjectProvider'.", "type"));
    }

    [Test]
    public void GetBindableObjectClass_WithMissingBindableObjectBaseClassAttribute ()
    {
      var provider = (BindableObjectProvider)BindableObjectProvider.GetProvider(typeof(BindableObjectProviderAttribute));
      Assert.That(
          () => provider.GetBindableObjectClass(typeof(StubBusinessObjectWithoutBindableObjectBaseClassAttributeClass)),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "The type 'Remotion.ObjectBinding.UnitTests.TestDomain.StubBusinessObjectWithoutBindableObjectBaseClassAttributeClass' "
              + "is not a bindable object implementation. It must either have a mixin derived from BindableObjectMixinBase<T> applied "
              + "or implement the IBusinessObject interface and apply the BindableObjectBaseClassAttribute.", "type"));
    }

    [Test]
    public void GetBindableObjectClass_WithMissingBusinessObjectInterace ()
    {
      var provider = (BindableObjectProvider)BindableObjectProvider.GetProvider(typeof(BindableObjectProviderAttribute));
      Assert.That(
          () => provider.GetBindableObjectClass(typeof(StubBusinessObjectWithoutBusinessObjectInterface)),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "The type 'Remotion.ObjectBinding.UnitTests.TestDomain.StubBusinessObjectWithoutBusinessObjectInterface' "
              + "is not a bindable object implementation. It must either have a mixin derived from BindableObjectMixinBase<T> applied "
              + "or implement the IBusinessObject interface and apply the BindableObjectBaseClassAttribute.", "type"));
    }

    [Test]
    public void GetBindableObjectClass_WithOpenGeneric ()
    {
      var provider = (BindableObjectProvider)BindableObjectProvider.GetProvider(typeof(BindableObjectProviderAttribute));
      Assert.That(
          () => provider.GetBindableObjectClass(typeof(GenericBindableObject<>)),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "The type 'Remotion.ObjectBinding.UnitTests.TestDomain.GenericBindableObject`1' "
              + "is not a bindable object implementation. Open generic types are not supported.", "type"));
    }

    [Test]
    public void GetBindableObjectClass ()
    {
      var metadataFactoryMock = new Mock<IMetadataFactory>(MockBehavior.Strict);
      var classReflectorMock = new Mock<IClassReflector>(MockBehavior.Strict);

      var provider = new BindableObjectProvider(metadataFactoryMock.Object, _serviceFactoryStub.Object);
      BindableObjectProvider.SetProvider(typeof(BindableObjectProviderAttribute), provider);
      Type targetType = typeof(SimpleBusinessObjectClass);
      Type concreteType = MixinTypeUtility.GetConcreteMixedType(targetType);
      var expectedBindableObjectClass = new BindableObjectClass(
          concreteType,
          provider,
          SafeServiceLocator.Current.GetInstance<BindableObjectGlobalizationService>(),
          new PropertyBase[0]);

      metadataFactoryMock.Setup(_ => _.CreateClassReflector(targetType, provider)).Returns(classReflectorMock.Object).Verifiable();
      classReflectorMock.Setup(_ => _.GetMetadata()).Returns(expectedBindableObjectClass).Verifiable();

      BindableObjectClass actual = provider.GetBindableObjectClass(targetType);

      metadataFactoryMock.Verify();
      classReflectorMock.Verify();

      Assert.That(actual, Is.SameAs(expectedBindableObjectClass));
    }

    [Test]
    public void GetBindableObjectClass_SameTwice ()
    {
      Assert.That(
          _provider.GetBindableObjectClass(typeof(SimpleBusinessObjectClass)),
          Is.SameAs(_provider.GetBindableObjectClass(typeof(SimpleBusinessObjectClass))));
    }

    [Test]
    public void GetBindableObjectClass_WithTypeDerivedFromBindableObjectBase ()
    {
      var bindableObjectClass = _provider.GetBindableObjectClass(typeof(ClassDerivedFromBindableObjectBase));
      Assert.That(bindableObjectClass.TargetType, Is.EqualTo(typeof(ClassDerivedFromBindableObjectBase)));
      Assert.That(bindableObjectClass.ConcreteType, Is.EqualTo(typeof(ClassDerivedFromBindableObjectBase)));
    }

    [Test]
    public void GetBindableObjectClass_WithTypeDerivedFromBindableObjectWithIdentityBase ()
    {
      var bindableObjectClass = _provider.GetBindableObjectClass(typeof(ClassDerivedFromBindableObjectWithIdentityBase));
      Assert.That(bindableObjectClass.TargetType, Is.EqualTo(typeof(ClassDerivedFromBindableObjectWithIdentityBase)));
      Assert.That(bindableObjectClass.ConcreteType, Is.EqualTo(typeof(ClassDerivedFromBindableObjectWithIdentityBase)));
    }

    [Test]
    public void GetBindableObjectClass_WithTypeNotUsingBindableObjectBaseClassAttribute ()
    {
      var bindableObjectClass = _provider.GetBindableObjectClass(typeof(StubBindableObject));
      Assert.That(bindableObjectClass.TargetType, Is.EqualTo(typeof(StubBindableObject)));
      Assert.That(bindableObjectClass.ConcreteType, Is.EqualTo(typeof(StubBindableObject)));
    }

    [Test]
    public void GetMetadataFactory_WithDefaultFactory ()
    {
      Assert.That(_provider.MetadataFactory, Is.InstanceOf(typeof(BindableObjectMetadataFactory)));
    }

    [Test]
    public void GetMetadataFactoryForType_WithCustomMetadataFactory ()
    {
      var provider = new BindableObjectProvider(_metadataFactoryStub.Object, _serviceFactoryStub.Object);
      Assert.That(provider.MetadataFactory, Is.SameAs(_metadataFactoryStub.Object));
    }

    [Test]
    public void GetServiceFactory_WithDefaultFactory ()
    {
      Assert.That(_provider.ServiceFactory, Is.InstanceOf(typeof(BindableObjectServiceFactory)));
    }

    [Test]
    public void GetServiceFactory_WithMixin ()
    {
      using (MixinConfiguration.BuildNew().ForClass(typeof(BindableObjectServiceFactory)).AddMixin<MixinStub>().EnterScope())
      {
        var provider = new BindableObjectProvider();
        Assert.That(provider.ServiceFactory, Is.InstanceOf(typeof(BindableObjectServiceFactory)));
        Assert.That(provider.ServiceFactory, Is.InstanceOf(typeof(IMixinTarget)));
      }
    }

    [Test]
    public void GetServiceFactoryForType_WithCustomServiceFactory ()
    {
      var provider = new BindableObjectProvider(_metadataFactoryStub.Object, _serviceFactoryStub.Object);
      Assert.That(provider.ServiceFactory, Is.SameAs(_serviceFactoryStub.Object));
    }

    [Test]
    public void IsBindableObjectImplementation_TrueWithMixin_TargetType ()
    {
      Assert.That(BindableObjectProvider.IsBindableObjectImplementation(typeof(SimpleBusinessObjectClass)), Is.True);
    }

    [Test]
    public void IsBindableObjectImplementation_TrueWithMixin_ConcreteType ()
    {
      Assert.That(BindableObjectProvider.IsBindableObjectImplementation(TypeFactory.GetConcreteType(typeof(SimpleBusinessObjectClass))), Is.True);
    }

    [Test]
    public void IsBindableObjectImplementation_TrueWithBase ()
    {
      Assert.That(BindableObjectProvider.IsBindableObjectImplementation(typeof(ClassDerivedFromBindableObjectBase)), Is.True);
    }

    [Test]
    public void IsBindableObjectImplementation_False ()
    {
      Assert.That(BindableObjectProvider.IsBindableObjectImplementation(typeof(object)), Is.False);
      Assert.That(BindableObjectProvider.IsBindableObjectImplementation(typeof(ManualBusinessObject)), Is.False);
    }

    [Test]
    public void IsBindableObjectImplementation_TrueWithSealedTypeHavingAMixin ()
    {
      var mixinTargetType = typeof(ManualBusinessObject);
      var businessObjectType = typeof(SealedBindableObject);
      Assertion.IsTrue(mixinTargetType.IsAssignableFrom(businessObjectType));

      using (MixinConfiguration.BuildNew()
          .AddMixinToClass(
              MixinKind.Extending,
              mixinTargetType,
              typeof(MixinStub),
              MemberVisibility.Public,
              Enumerable.Empty<Type>(),
              Enumerable.Empty<Type>())
          .EnterScope())
      {
        Assert.That(BindableObjectProvider.IsBindableObjectImplementation(businessObjectType), Is.True);
      }
    }

    [Test]
    public void IsBindableObjectImplementation_FalseWithOpenGenericType ()
    {
      var mixinTargetType = typeof(ManualBusinessObject);
      var businessObjectType = typeof(GenericBindableObject<>);
      Assertion.IsTrue(mixinTargetType.IsAssignableFrom(businessObjectType));

      Assert.That(BindableObjectProvider.IsBindableObjectImplementation(businessObjectType), Is.False);
    }

    [Test]
    public void IsBindableObjectImplementation_TrueWithClosedGenericType ()
    {
      var mixinTargetType = typeof(ManualBusinessObject);
      var businessObjectType = typeof(GenericBindableObject<string>);
      Assertion.IsTrue(mixinTargetType.IsAssignableFrom(businessObjectType));

      Assert.That(BindableObjectProvider.IsBindableObjectImplementation(businessObjectType), Is.True);
    }

    [Test]
    public void IsBindableObjectImplementation_TrueWithValueType ()
    {
      Assert.That(BindableObjectProvider.IsBindableObjectImplementation(typeof(ValueTypeBindableObject)), Is.True);
    }
  }
}
