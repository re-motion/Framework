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
using Remotion.Globalization;
using Remotion.Globalization.ExtensibleEnums;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.BusinessObjectPropertyConstraints;
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class BindableObjectClassTest : TestBase
  {
    private class StubBindableObjectClass : BindableObjectClass
    {
      private PropertyCollection _properties;

      public StubBindableObjectClass (Type concreteType, BindableObjectProvider businessObjectProvider)
        : base(concreteType, businessObjectProvider, new PropertyBase[0])
      {
      }

      protected override PropertyCollection Properties
      {
        get
        {
          return _properties;
        }
      }

      public void SetProperties (PropertyCollection properties)
      {
        _properties = properties;
      }
    }

    private BindableObjectProvider _bindableObjectProvider;
    private BindableObjectGlobalizationService _bindableObjectGlobalizationService;

    public override void SetUp ()
    {
      base.SetUp();

      _bindableObjectProvider = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory();
      BusinessObjectProvider.SetProvider<BindableObjectProviderAttribute>(_bindableObjectProvider);
      BusinessObjectProvider.SetProvider<BindableObjectWithIdentityProviderAttribute>(_bindableObjectProvider);
      _bindableObjectGlobalizationService = SafeServiceLocator.Current.GetInstance<BindableObjectGlobalizationService>();
    }

    public void Initialize_WithTypeNotUsingBindableObjectMixin ()
    {
      var bindableObjectClass = new BindableObjectClass(
          MixinTypeUtility.GetConcreteMixedType(typeof(SimpleReferenceType)),
          _bindableObjectProvider,
          _bindableObjectGlobalizationService,
          new PropertyBase[0]);
      Assert.That(bindableObjectClass.TargetType, Is.EqualTo(typeof(SimpleReferenceType)));
      Assert.That(bindableObjectClass.ConcreteType, Is.EqualTo(typeof(SimpleReferenceType)));
    }

    private void CheckPropertyBase (IBusinessObjectProperty expectedProperty, IBusinessObjectProperty actualProperty)
    {
      ArgumentUtility.CheckNotNull("expectedProperty", expectedProperty);

      Assert.That(actualProperty, Is.Not.Null);
      Assert.That(actualProperty.GetType(), Is.SameAs(expectedProperty.GetType()), "BusinessObjectPropertyType");
      Assert.That(expectedProperty.PropertyType, Is.EqualTo(actualProperty.PropertyType), "PropertyType");
      Assert.That(expectedProperty.IsList, Is.EqualTo(actualProperty.IsList), "IsList");
      if (expectedProperty.IsList)
        Assert.That(expectedProperty.ListInfo.ItemType, Is.EqualTo(actualProperty.ListInfo.ItemType), "ListInfo.ItemType");
      Assert.That(expectedProperty.IsRequired, Is.EqualTo(actualProperty.IsRequired), "IsRequired");
      Assert.That(((PropertyBase)actualProperty).ReflectedClass, Is.Not.Null);

      if (typeof(IBusinessObjectStringProperty).IsAssignableFrom(actualProperty.GetType()))
        CheckStringProperty((IBusinessObjectStringProperty)actualProperty, expectedProperty);
    }

    private void CheckStringProperty (IBusinessObjectStringProperty expectedProperty, IBusinessObjectProperty actualProperty)
    {
      Assert.That(
          expectedProperty.MaxLength,
          Is.EqualTo(((IBusinessObjectStringProperty)actualProperty).MaxLength),
          "MaxLength");
    }

    private PropertyBase CreateProperty (Type type, string propertyName)
    {
      PropertyReflector propertyReflector = PropertyReflector.Create(GetPropertyInfo(type, propertyName), _bindableObjectProvider);
      return propertyReflector.GetMetadata();
    }

    [Test]
    public void GetDisplayName_WithGlobalizationService ()
    {
      var mockMemberInformationGlobalizationService = new Mock<IMemberInformationGlobalizationService>(MockBehavior.Strict);
      var classReflector = new ClassReflector(
          typeof(ClassWithAllDataTypes),
          _bindableObjectProvider,
          BindableObjectMetadataFactory.Create(),
          new BindableObjectGlobalizationService(
              new Mock<IGlobalizationService>().Object,
              mockMemberInformationGlobalizationService.Object,
              new Mock<IEnumerationGlobalizationService>().Object,
              new Mock<IExtensibleEnumGlobalizationService>().Object));
      var bindableObjectClass = classReflector.GetMetadata();
      var outValue = "MockString";

      mockMemberInformationGlobalizationService
          .Setup(_ => _.TryGetTypeDisplayName(
              It.Is<ITypeInformation>(c => c.ConvertToRuntimeType() == bindableObjectClass.TargetType),
              It.Is<ITypeInformation>(c => c.ConvertToRuntimeType() == bindableObjectClass.TargetType),
              out outValue))
          .Returns(true)
          .Verifiable();

      Assert.That(bindableObjectClass.GetDisplayName(), Is.EqualTo("MockString"));
    }

    [Test]
    public void GetPropertyDefinition ()
    {
      PropertyReflector propertyReflector =
          PropertyReflector.Create(GetPropertyInfo(typeof(SimpleBusinessObjectClass), "String"), _bindableObjectProvider);
      var classReflector = new ClassReflector(
          typeof(ClassWithAllDataTypes),
          _bindableObjectProvider,
          BindableObjectMetadataFactory.Create(),
          _bindableObjectGlobalizationService);
      BindableObjectClass bindableObjectClass = classReflector.GetMetadata();

      CheckPropertyBase(propertyReflector.GetMetadata(), bindableObjectClass.GetPropertyDefinition("String"));
    }

    [Test]
    public void GetPropertyDefinition_ForMixedProperty ()
    {
      PropertyReflector propertyReflector = PropertyReflector.Create(
          GetPropertyInfo(
              MixinTypeUtility.GetConcreteMixedType(typeof(ClassWithMixedProperty)),
              typeof(IMixinAddingProperty).FullName + ".MixedProperty"),
          _bindableObjectProvider);
      var classReflector = new ClassReflector(
          typeof(ClassWithMixedProperty),
          _bindableObjectProvider,
          BindableObjectMetadataFactory.Create(),
          _bindableObjectGlobalizationService);
      BindableObjectClass bindableObjectClass = classReflector.GetMetadata();

      CheckPropertyBase(propertyReflector.GetMetadata(), bindableObjectClass.GetPropertyDefinition("MixedProperty"));
    }

    [Test]
    public void GetPropertyDefinition_FromOverriddenPropertiesProperty ()
    {
      var property = CreateStubProperty();
      StubBindableObjectClass bindableObjectClass = CreateStubBindableObjectClass(property);

      Assert.That(bindableObjectClass.GetPropertyDefinition("Scalar"), Is.SameAs(property));
    }

    [Test]
    public void GetPropertyDefinition_WithInvalidPropertyName ()
    {
      var classReflector = new ClassReflector(
          typeof(ClassWithAllDataTypes),
          _bindableObjectProvider,
          BindableObjectMetadataFactory.Create(),
          _bindableObjectGlobalizationService);
      BindableObjectClass bindableObjectClass = classReflector.GetMetadata();

      Assert.That(bindableObjectClass.GetPropertyDefinition("Invalid"), Is.Null);
    }

    [Test]
    public void GetPropertyDefinition_FromOverriddenPropertiesProperty_WithInvalidPropertyName ()
    {
      StubBindableObjectClass bindableObjectClass = CreateStubBindableObjectClass(CreateStubProperty());

      Assert.That(bindableObjectClass.GetPropertyDefinition("Invalid"), Is.Null);
    }

    [Test]
    public void GetPropertyDefinitions ()
    {
      Type type = typeof(ClassWithReferenceType<SimpleReferenceType>);
      var expectedProperties = new[]
                               {
                                   CreateProperty(type, "Scalar"),
                                   CreateProperty(type, "ReadOnlyScalar"),
                                   CreateProperty(type, "ReadOnlyAttributeScalar"),
                                   CreateProperty(type, "ReadOnlyNonPublicSetterScalar"),
                                   CreateProperty(type, "Array"),
                                   CreateProperty(type, "ImplicitInterfaceScalar"),
                                   CreateProperty(type, "ImplicitInterfaceReadOnlyScalar"),
                                   CreateProperty(
                                       type,
                                       "Remotion.ObjectBinding.UnitTests.TestDomain.IInterfaceWithReferenceType<T>.ExplicitInterfaceScalar")
                                   ,
                                   CreateProperty(
                                       type,
                                       "Remotion.ObjectBinding.UnitTests.TestDomain.IInterfaceWithReferenceType<T>.ExplicitInterfaceReadOnlyScalar"),
                                  CreateProperty(type, "PropertyWithNoSetter"),
                                  CreateProperty(type, "ThrowingProperty"),
                               };

      var classReflector = new ClassReflector(
          type,
          _bindableObjectProvider,
          BindableObjectMetadataFactory.Create(),
          _bindableObjectGlobalizationService);
      BindableObjectClass bindableObjectClass = classReflector.GetMetadata();
      IBusinessObjectProperty[] actualProperties = bindableObjectClass.GetPropertyDefinitions();

      Assert.That(actualProperties.Length, Is.EqualTo(expectedProperties.Length));
      foreach (PropertyBase expectedProperty in expectedProperties)
      {
        bool isFound = false;
        foreach (IBusinessObjectProperty actualProperty in actualProperties)
        {
          if (actualProperty.Identifier == expectedProperty.Identifier)
          {
            Assert.That(isFound, Is.False, $"Multiple properties '{expectedProperty.Identifier}' found");
            CheckPropertyBase(expectedProperty, actualProperty);
            isFound = true;
          }
        }
        Assert.That(isFound, Is.True, $"Property '{expectedProperty.Identifier}' was not found");
      }
    }

    [Test]
    public void GetPropertyDefinitions_FromOverriddenPropertiesProperty ()
    {
      var property = CreateStubProperty();
      StubBindableObjectClass bindableObjectClass = CreateStubBindableObjectClass(property);

      Assert.That(bindableObjectClass.GetPropertyDefinitions(), Is.EqualTo(new []{property}));
    }

    [Test]
    public void Initialize ()
    {
      var bindableObjectClass = new BindableObjectClass(
          MixinTypeUtility.GetConcreteMixedType(typeof(SimpleBusinessObjectClass)),
          _bindableObjectProvider,
          _bindableObjectGlobalizationService,
          new PropertyBase[0]);

      Assert.That(bindableObjectClass.TargetType, Is.SameAs(typeof(SimpleBusinessObjectClass)));
      Assert.That(bindableObjectClass.ConcreteType, Is.Not.SameAs(typeof(SimpleBusinessObjectClass)));
      Assert.That(bindableObjectClass.ConcreteType, Is.SameAs(MixinTypeUtility.GetConcreteMixedType(typeof(SimpleBusinessObjectClass))));
      Assert.That(
          bindableObjectClass.Identifier,
          Is.EqualTo("Remotion.ObjectBinding.UnitTests.TestDomain.SimpleBusinessObjectClass, Remotion.ObjectBinding.UnitTests"));
      Assert.That(bindableObjectClass.RequiresWriteBack, Is.False);
      Assert.That(bindableObjectClass.BusinessObjectProvider, Is.SameAs(_bindableObjectProvider));
      Assert.That(bindableObjectClass.BusinessObjectProviderAttribute, Is.InstanceOf(typeof(BindableObjectProviderAttribute)));
    }

    [Test]
    public void Initialize_WithGeneric ()
    {
      var bindableObjectClass = new BindableObjectClass(
          MixinTypeUtility.GetConcreteMixedType(typeof(ClassWithReferenceType<SimpleReferenceType>)),
          _bindableObjectProvider,
          _bindableObjectGlobalizationService,
          new PropertyBase[0]);

      Assert.That(bindableObjectClass.TargetType, Is.SameAs(typeof(ClassWithReferenceType<SimpleReferenceType>)));
    }

    [Test]
    public void Initialize_WithTypeDerivedFromBindableObjectBase ()
    {
      var bindableObjectClass = new BindableObjectClass(
          typeof(ClassDerivedFromBindableObjectBase),
          _bindableObjectProvider,
          _bindableObjectGlobalizationService,
          new PropertyBase[0]);
      Assert.That(bindableObjectClass.TargetType, Is.EqualTo(typeof(ClassDerivedFromBindableObjectBase)));
      Assert.That(bindableObjectClass.ConcreteType, Is.EqualTo(typeof(ClassDerivedFromBindableObjectBase)));
    }

    [Test]
    public void Initialize_WithUnmixedType ()
    {
      var bindableObjectClass = new BindableObjectClass(
          typeof(ManualBusinessObject),
          _bindableObjectProvider,
          _bindableObjectGlobalizationService,
          new PropertyBase[0]);
      Assert.That(bindableObjectClass.TargetType, Is.EqualTo(typeof(ManualBusinessObject)));
      Assert.That(bindableObjectClass.ConcreteType, Is.EqualTo(typeof(ManualBusinessObject)));
    }

    [Test]
    public void SetPropertyDefinitions ()
    {
      Type type = typeof(ClassWithReferenceType<SimpleReferenceType>);
      var expectedProperties = new[] { CreateProperty(type, "Scalar"), CreateProperty(type, "ReadOnlyScalar"), };

      var bindableObjectClass = new BindableObjectClass(
          MixinTypeUtility.GetConcreteMixedType(type),
          _bindableObjectProvider,
          _bindableObjectGlobalizationService,
          expectedProperties);
      IBusinessObjectProperty[] actualProperties = bindableObjectClass.GetPropertyDefinitions();

      Assert.That(actualProperties, Is.EqualTo(expectedProperties));
      foreach (IBusinessObjectProperty actualProperty in actualProperties)
        Assert.That(((PropertyBase)actualProperty).ReflectedClass, Is.SameAs(bindableObjectClass));
    }

    private StubBindableObjectClass CreateStubBindableObjectClass (PropertyBase property)
    {
      var bindableObjectClass = new StubBindableObjectClass(typeof(ClassWithAllDataTypes), _bindableObjectProvider);
      bindableObjectClass.SetProperties(new PropertyCollection(new[] { property }));
      return bindableObjectClass;
    }

    private StubPropertyBase CreateStubProperty ()
    {
      return new StubPropertyBase(
          new PropertyBase.Parameters(
              new BindableObjectProvider(
                  new Mock<IMetadataFactory>().Object, new Mock<IBusinessObjectServiceFactory>().Object),
              GetPropertyInfo(typeof(ClassWithReferenceType<SimpleReferenceType>), "Scalar"),
              typeof(SimpleReferenceType),
              new Lazy<Type>(() => typeof(SimpleReferenceType)),
              null,
              true,
              false,
              false,
              new BindableObjectDefaultValueStrategy(),
              new Mock<IBindablePropertyReadAccessStrategy>().Object,
              new Mock<IBindablePropertyWriteAccessStrategy>().Object,
              SafeServiceLocator.Current.GetInstance<BindableObjectGlobalizationService>(),
              new Mock<IBusinessObjectPropertyConstraintProvider>().Object));
    }
  }
}
