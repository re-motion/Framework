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
using System.Reflection;
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.NUnit;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.Globalization;
using Remotion.Globalization.ExtensibleEnums;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.BusinessObjectPropertyConstraints;
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Remotion.Reflection;
using Remotion.TypePipe;

namespace Remotion.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class PropertyBaseTest : TestBase
  {
    private BindableObjectProvider _bindableObjectProvider;
    private BindableObjectClass _bindableObjectClass;

    public override void SetUp ()
    {
      base.SetUp();

      _bindableObjectProvider = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory();
      BusinessObjectProvider.SetProvider<BindableObjectProviderAttribute>(_bindableObjectProvider);
      BusinessObjectProvider.SetProvider<BindableObjectWithIdentityProviderAttribute>(_bindableObjectProvider);
      _bindableObjectClass = BindableObjectProviderTestHelper.GetBindableObjectClass(typeof(SimpleBusinessObjectClass));
    }

    [Test]
    public void Initialize ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo(typeof(ClassWithReferenceType<SimpleReferenceType>), "Scalar");
      PropertyBase propertyBase = new StubPropertyBase(
          CreateParameters(
              propertyInfo: propertyInfo,
              underlyingType: propertyInfo.PropertyType,
              concreteType: propertyInfo.PropertyType,
              listInfo: null,
              isRequired: true,
              isReadOnly: true));

      Assert.That(propertyBase.PropertyInfo, Is.SameAs(propertyInfo));
      Assert.That(propertyBase.PropertyType, Is.SameAs(propertyInfo.PropertyType));
      Assert.That(propertyBase.IsRequired, Is.True);
      Assert.That(propertyBase.IsReadOnly(null), Is.True);
      Assert.That(propertyBase.BusinessObjectProvider, Is.SameAs(_bindableObjectProvider));
      Assert.That(((IBusinessObjectProperty) propertyBase).BusinessObjectProvider, Is.SameAs(_bindableObjectProvider));
    }

    [Test]
    public void Initialize_IndexedProperty ()
    {
      IPropertyInformation propertyInfo =
          PropertyInfoAdapter.Create(typeof(ClassWithReferenceType<SimpleReferenceType>).GetProperty("Item", new[] { typeof(int) }));
      Assert.That(
          () => new StubPropertyBase(
          CreateParameters(
              propertyInfo: propertyInfo,
              underlyingType: propertyInfo.PropertyType,
              concreteType: propertyInfo.PropertyType,
              listInfo: null,
              isRequired: true,
              isReadOnly: true)),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Indexed properties are not supported."));
    }

    [Test]
    public void GetValue ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo(typeof(ClassWithReferenceType<SimpleReferenceType>), "Scalar");
      PropertyBase propertyBase = new StubPropertyBase(
          CreateParameters(
              propertyInfo: propertyInfo,
              underlyingType: propertyInfo.PropertyType,
              concreteType: propertyInfo.PropertyType,
              listInfo: null,
              isRequired: true,
              isReadOnly: true));

      var instance = ObjectFactory.Create<ClassWithReferenceType<SimpleReferenceType>>(ParamList.Empty);

      var value = new SimpleReferenceType();
      instance.Scalar = value;

      Assert.That(propertyBase.GetValue(((IBusinessObject) instance)), Is.SameAs(value));
    }

    [Test]
    public void GetValue_WithPrivatAccessor ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo(typeof(ClassWithReferenceType<SimpleReferenceType>), "PrivateProperty");
      PropertyBase propertyBase = new StubPropertyBase(
          CreateParameters(
              propertyInfo: propertyInfo,
              underlyingType: propertyInfo.PropertyType,
              concreteType: propertyInfo.PropertyType,
              listInfo: null,
              isRequired: true,
              isReadOnly: true));

      var instance = ObjectFactory.Create<ClassWithReferenceType<SimpleReferenceType>>(ParamList.Empty);

      var value = new SimpleReferenceType();
      PrivateInvoke.SetNonPublicProperty(instance, "PrivateProperty", value);

      Assert.That(propertyBase.GetValue(((IBusinessObject) instance)), Is.SameAs(value));
    }

    [Test]
    public void GetValue_NoGetter ()
    {
      var propertyInfo = new Mock<IPropertyInformation>();
      propertyInfo.Setup(stub => stub.PropertyType).Returns(typeof(bool));
      propertyInfo.Setup(stub => stub.GetIndexParameters()).Returns(new ParameterInfo[0]);
      propertyInfo.Setup(stub => stub.GetSetMethod(true)).Returns((IMethodInformation) null);
      PropertyBase propertyBase = new StubPropertyBase(
          CreateParameters(
              propertyInfo: propertyInfo.Object,
              underlyingType: propertyInfo.Object.PropertyType,
              concreteType: propertyInfo.Object.PropertyType,
              listInfo: null,
              isRequired: true,
              isReadOnly: true));

      var instance = ObjectFactory.Create<ClassWithReferenceType<SimpleReferenceType>>(ParamList.Empty);
      Assert.That(
          () => propertyBase.GetValue(((IBusinessObject) instance)),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Property has no getter."));
    }

    [Test]
    public void GetValue_WithExceptionHandledByPropertyAccessStrategy_ThrowsBusinessObjectPropertyAccessException ()
    {
      var bindablePropertyReadAccessStrategyStub = new Mock<IBindablePropertyReadAccessStrategy>();

      IPropertyInformation propertyInfo = GetPropertyInfo(typeof(ClassWithReferenceType<SimpleReferenceType>), "ThrowingProperty");
      PropertyBase propertyBase = new StubPropertyBase(
          CreateParameters(
              propertyInfo: propertyInfo,
              underlyingType: propertyInfo.PropertyType,
              concreteType: propertyInfo.PropertyType,
              listInfo: null,
              bindablePropertyReadAccessStrategy: bindablePropertyReadAccessStrategyStub.Object));

      var instance = ObjectFactory.Create<ClassWithReferenceType<SimpleReferenceType>>(ParamList.Empty);

      var originalException = new Exception("The Exception");
      var expectedException = new BusinessObjectPropertyAccessException("The Message", null);
      bindablePropertyReadAccessStrategyStub
          .Setup(
              mock => mock.IsPropertyAccessException(
                  (IBusinessObject) instance,
                  propertyBase,
                  originalException,
                  out expectedException))
          .Returns(true);

      instance.PrepareException(originalException);

      var actualException = Assert.Throws<BusinessObjectPropertyAccessException>(() => propertyBase.GetValue(((IBusinessObject) instance)));
      Assert.That(actualException, Is.SameAs(expectedException));
    }

    [Test]
    public void GetValue_WithExceptionNotHandledByPropertyAccessStrategy_RethrowsOriginalException ()
    {
      var bindablePropertyReadAccessStrategyStub = new Mock<IBindablePropertyReadAccessStrategy>();
      var expectedException = new BusinessObjectPropertyAccessException("Unexpected", null);

      IPropertyInformation propertyInfo = GetPropertyInfo(typeof(ClassWithReferenceType<SimpleReferenceType>), "ThrowingProperty");
      PropertyBase propertyBase = new StubPropertyBase(
          CreateParameters(
              propertyInfo: propertyInfo,
              underlyingType: propertyInfo.PropertyType,
              concreteType: propertyInfo.PropertyType,
              listInfo: null,
              bindablePropertyReadAccessStrategy: bindablePropertyReadAccessStrategyStub.Object));

      var instance = ObjectFactory.Create<ClassWithReferenceType<SimpleReferenceType>>(ParamList.Empty);

      var originalException = new Exception("The Exception");
      bindablePropertyReadAccessStrategyStub
          .Setup(
              mock => mock.IsPropertyAccessException(
                  (IBusinessObject) instance,
                  propertyBase,
                  originalException,
                  out expectedException))
          .Returns(false);

      instance.PrepareException(originalException);

      var actualException = Assert.Throws<Exception>(() => propertyBase.GetValue(((IBusinessObject) instance)));
      Assert.That(actualException, Is.SameAs(originalException));
#if DEBUG
      Assert.That(
          originalException.StackTrace,
          Does.StartWith("   at Remotion.ObjectBinding.UnitTests.TestDomain.ClassWithReferenceType`1.get_ThrowingProperty()"));
#endif
    }

    [Test]
    public void SetValue ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo(typeof(ClassWithReferenceType<SimpleReferenceType>), "Scalar");
      PropertyBase propertyBase = new StubPropertyBase(
          CreateParameters(
              propertyInfo: propertyInfo,
              underlyingType: propertyInfo.PropertyType,
              concreteType: propertyInfo.PropertyType,
              listInfo: null,
              isRequired: true,
              isReadOnly: true));

      var instance = ObjectFactory.Create<ClassWithReferenceType<SimpleReferenceType>>(ParamList.Empty);

      var value = new SimpleReferenceType();
      propertyBase.SetValue((IBusinessObject) instance, value);

      Assert.That(instance.Scalar, Is.SameAs(value));
    }

    [Test]
    public void SetValue_PrivateAccessor ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo(typeof(ClassWithReferenceType<SimpleReferenceType>), "PrivateProperty");
      PropertyBase propertyBase = new StubPropertyBase(
          CreateParameters(
              propertyInfo: propertyInfo,
              underlyingType: propertyInfo.PropertyType,
              concreteType: propertyInfo.PropertyType,
              listInfo: null,
              isRequired: true,
              isReadOnly: true));

      var instance = ObjectFactory.Create<ClassWithReferenceType<SimpleReferenceType>>(ParamList.Empty);

      var value = new SimpleReferenceType();
      propertyBase.SetValue((IBusinessObject) instance, value);

      Assert.That(PrivateInvoke.GetNonPublicProperty(instance, "PrivateProperty"), Is.SameAs(value));
    }

    [Test]
    public void GetValue_NoSetter ()
    {
      var propertyInfo = new Mock<IPropertyInformation>();
      propertyInfo.Setup(stub => stub.PropertyType).Returns(typeof(bool));
      propertyInfo.Setup(stub => stub.GetIndexParameters()).Returns(new ParameterInfo[0]);
      propertyInfo.Setup(stub => stub.GetSetMethod(true)).Returns((IMethodInformation) null);
      PropertyBase propertyBase = new StubPropertyBase(
          CreateParameters(
              propertyInfo: propertyInfo.Object,
              underlyingType: propertyInfo.Object.PropertyType,
              concreteType: propertyInfo.Object.PropertyType,
              listInfo: null,
              isRequired: true,
              isReadOnly: true));

      var instance = ObjectFactory.Create<ClassWithReferenceType<SimpleReferenceType>>(ParamList.Empty);
      Assert.That(
          () => propertyBase.SetValue(((IBusinessObject) instance), new object()),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Property has no setter."));
    }

    [Test]
    public void SetValue_WithExceptionHandledByPropertyAccessStrategy_ThrowsBusinessObjectPropertyAccessException ()
    {
      var bindablePropertyWriteAccessStrategyStub = new Mock<IBindablePropertyWriteAccessStrategy>();

      IPropertyInformation propertyInfo = GetPropertyInfo(typeof(ClassWithReferenceType<SimpleReferenceType>), "ThrowingProperty");
      PropertyBase propertyBase = new StubPropertyBase(
          CreateParameters(
              propertyInfo: propertyInfo,
              underlyingType: propertyInfo.PropertyType,
              concreteType: propertyInfo.PropertyType,
              listInfo: null,
              bindablePropertyWriteAccessStrategy: bindablePropertyWriteAccessStrategyStub.Object));

      var instance = ObjectFactory.Create<ClassWithReferenceType<SimpleReferenceType>>(ParamList.Empty);

      var originalException = new Exception("The Exception");
      var expectedException = new BusinessObjectPropertyAccessException("The Message", null);
      bindablePropertyWriteAccessStrategyStub
          .Setup(
              mock => mock.IsPropertyAccessException(
                  (IBusinessObject) instance,
                  propertyBase,
                  originalException,
                  out expectedException))
          .Returns(true);

      instance.PrepareException(originalException);

      var actualException =
          Assert.Throws<BusinessObjectPropertyAccessException>(() => propertyBase.SetValue((IBusinessObject) instance, new SimpleReferenceType()));
      Assert.That(actualException, Is.SameAs(expectedException));
    }

    [Test]
    public void SetValue_WithExceptionNotHandledByPropertyAccessStrategy_RethrowsOriginalException ()
    {
      var bindablePropertyWriteAccessStrategyStub = new Mock<IBindablePropertyWriteAccessStrategy>();
      var expectedException = new BusinessObjectPropertyAccessException("Unexpected", null);

      IPropertyInformation propertyInfo = GetPropertyInfo(typeof(ClassWithReferenceType<SimpleReferenceType>), "ThrowingProperty");
      PropertyBase propertyBase = new StubPropertyBase(
          CreateParameters(
              propertyInfo: propertyInfo,
              underlyingType: propertyInfo.PropertyType,
              concreteType: propertyInfo.PropertyType,
              listInfo: null,
              bindablePropertyWriteAccessStrategy: bindablePropertyWriteAccessStrategyStub.Object));

      var instance = ObjectFactory.Create<ClassWithReferenceType<SimpleReferenceType>>(ParamList.Empty);

      var originalException = new Exception("The Exception");
      bindablePropertyWriteAccessStrategyStub
          .Setup(
              mock => mock.IsPropertyAccessException(
                  (IBusinessObject) instance,
                  propertyBase,
                  originalException,
                  out expectedException))
          .Returns(false);

      instance.PrepareException(originalException);

      var actualException = Assert.Throws<Exception>(() => propertyBase.SetValue((IBusinessObject) instance, new SimpleReferenceType()));
      Assert.That(actualException, Is.SameAs(originalException));
#if DEBUG
      Assert.That(
          originalException.StackTrace,
          Does.StartWith("   at Remotion.ObjectBinding.UnitTests.TestDomain.ClassWithReferenceType`1.set_ThrowingProperty(T value)"));
#endif
    }
   
    [Test]
    public void IsAccessible_ReturnsValueFromStrategy ()
    {
      var bindablePropertyReadAccessStrategyMock = new Mock<IBindablePropertyReadAccessStrategy>();

      IPropertyInformation propertyInfo = GetPropertyInfo(typeof(ClassWithReferenceType<SimpleReferenceType>), "ThrowingProperty");
      PropertyBase propertyBase = new StubPropertyBase(
          CreateParameters(
              propertyInfo: propertyInfo,
              underlyingType: propertyInfo.PropertyType,
              concreteType: propertyInfo.PropertyType,
              listInfo: null,
              bindablePropertyReadAccessStrategy: bindablePropertyReadAccessStrategyMock.Object));

      var instance = ObjectFactory.Create<ClassWithReferenceType<SimpleReferenceType>>(ParamList.Empty);

      var expectedValue = BooleanObjectMother.GetRandomBoolean();
      bindablePropertyReadAccessStrategyMock.Setup(mock => mock.CanRead((IBusinessObject) instance, propertyBase)).Returns(expectedValue).Verifiable();

      Assert.That(propertyBase.IsAccessible((IBusinessObject)instance), Is.EqualTo(expectedValue));
      bindablePropertyReadAccessStrategyMock.Verify();
    }

    [Test]
    public void IsReadOnly_WithReadOnlyProperty_ReturnsTrue_DoesNotUseStrategy ()
    {
      var bindablePropertyWriteAccessStrategyMock = new Mock<IBindablePropertyWriteAccessStrategy>();

      IPropertyInformation propertyInfo = GetPropertyInfo(typeof(ClassWithReferenceType<SimpleReferenceType>), "ThrowingProperty");
      PropertyBase propertyBase = new StubPropertyBase(
          CreateParameters(
              propertyInfo: propertyInfo,
              underlyingType: propertyInfo.PropertyType,
              concreteType: propertyInfo.PropertyType,
              listInfo: null,
              isReadOnly: true,
              bindablePropertyWriteAccessStrategy: bindablePropertyWriteAccessStrategyMock.Object));

      var instance = ObjectFactory.Create<ClassWithReferenceType<SimpleReferenceType>>(ParamList.Empty);

      Assert.That(propertyBase.IsReadOnly((IBusinessObject)instance), Is.True);
      bindablePropertyWriteAccessStrategyMock.Verify(mock => mock.CanWrite(null, null), Times.Never());
    }

    [Test]
    public void IsReadOnly_WithWritableProperty_ReturnsValueFromStrategy ()
    {
      var bindablePropertyWriteAccessStrategyMock = new Mock<IBindablePropertyWriteAccessStrategy>();

      IPropertyInformation propertyInfo = GetPropertyInfo(typeof(ClassWithReferenceType<SimpleReferenceType>), "ThrowingProperty");
      PropertyBase propertyBase = new StubPropertyBase(
          CreateParameters(
              propertyInfo: propertyInfo,
              underlyingType: propertyInfo.PropertyType,
              concreteType: propertyInfo.PropertyType,
              listInfo: null,
              isReadOnly: false,
              bindablePropertyWriteAccessStrategy: bindablePropertyWriteAccessStrategyMock.Object));

      var instance = ObjectFactory.Create<ClassWithReferenceType<SimpleReferenceType>>(ParamList.Empty);

      var expectedValue = BooleanObjectMother.GetRandomBoolean();
      bindablePropertyWriteAccessStrategyMock.Setup(mock => mock.CanWrite((IBusinessObject) instance, propertyBase)).Returns(!expectedValue).Verifiable();

      Assert.That(propertyBase.IsReadOnly((IBusinessObject)instance), Is.EqualTo(expectedValue));
      bindablePropertyWriteAccessStrategyMock.Verify();
    }

    [Test]
    public void GetConstraints_WithBusinessObject_ReturnsValueFromProvider ()
    {
      var businessObject = new Mock<IBusinessObject>();
        var propertyInfo = GetPropertyInfo(typeof(ClassWithReferenceType<SimpleReferenceType>), "Scalar");
        var constraintProviderStub = new Mock<IBusinessObjectPropertyConstraintProvider>();
        PropertyBase propertyBase =
            new StubPropertyBase(
                CreateParameters(
                    propertyInfo: propertyInfo,
                    underlyingType: propertyInfo.PropertyType,
                    concreteType: propertyInfo.PropertyType,
                    businessObjectPropertyConstraintProvider: constraintProviderStub.Object));
        propertyBase.SetReflectedClass(_bindableObjectClass);

        var result = new[] { new Mock<IBusinessObjectPropertyConstraint>().Object };
        constraintProviderStub.Setup(_ => _.GetPropertyConstraints(_bindableObjectClass, propertyBase, businessObject.Object)).Returns(result);

        Assert.That(propertyBase.GetConstraints(businessObject.Object), Is.SameAs(result));
    }

    [Test]
    public void GetConstraints_WithoutBusinessObject_ReturnsValueFromProvider ()
    {
      var propertyInfo = GetPropertyInfo(typeof(ClassWithReferenceType<SimpleReferenceType>), "Scalar");
      var constraintProviderStub = new Mock<IBusinessObjectPropertyConstraintProvider>();
      PropertyBase propertyBase =
          new StubPropertyBase(
              CreateParameters(
                  propertyInfo: propertyInfo,
                  underlyingType: propertyInfo.PropertyType,
                  concreteType: propertyInfo.PropertyType,
                  businessObjectPropertyConstraintProvider: constraintProviderStub.Object));
      propertyBase.SetReflectedClass(_bindableObjectClass);

      var result = new[] { new Mock<IBusinessObjectPropertyConstraint>().Object };
      constraintProviderStub.Setup(_ => _.GetPropertyConstraints(_bindableObjectClass, propertyBase, null)).Returns(result);

      Assert.That(propertyBase.GetConstraints(null), Is.SameAs(result));
    }

    [Test]
    public void GetDefaultValueStrategy ()
    {
      var businessObject = new Mock<IBusinessObject>();
      IPropertyInformation propertyInfo = GetPropertyInfo(typeof(ClassWithReferenceType<SimpleReferenceType>), "Scalar");
      PropertyBase propertyBase =
          new StubPropertyBase(
              CreateParameters(
                  propertyInfo: propertyInfo,
                  underlyingType: propertyInfo.PropertyType,
                  concreteType: propertyInfo.PropertyType,
                  listInfo: null,
                  isRequired: true,
                  isReadOnly: true));

      Assert.That(propertyBase.IsDefaultValue(businessObject.Object), Is.False);
    }

    [Test]
    public void GetListInfo ()
    {
      IListInfo expected = new ListInfo(typeof(SimpleReferenceType[]), typeof(SimpleReferenceType));
      PropertyBase property = new StubPropertyBase(
          CreateParameters(
              propertyInfo: GetPropertyInfo(typeof(ClassWithReferenceType<SimpleReferenceType>), "Array"),
              underlyingType: typeof(SimpleReferenceType),
              concreteType: typeof(SimpleReferenceType),
              listInfo: expected,
              isRequired: false,
              isReadOnly: false));

      Assert.That(property.IsList, Is.True);
      Assert.That(property.ListInfo, Is.SameAs(expected));
    }

    [Test]
    public void GetListInfo_WithNonListProperty ()
    {
      PropertyBase property = new StubPropertyBase(
          CreateParameters(
              propertyInfo: GetPropertyInfo(typeof(ClassWithReferenceType<SimpleReferenceType>), "Scalar"),
              underlyingType: typeof(SimpleReferenceType),
              concreteType: typeof(SimpleReferenceType),
              listInfo: null,
              isRequired: false,
              isReadOnly: false));

      Assert.That(property.IsList, Is.False);
      Assert.That(
          () => property.ListInfo,
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Cannot access ListInfo for non-list properties.\r\nProperty: Scalar"));
    }


    [Test]
    public void ConvertFromNativePropertyType ()
    {
      PropertyBase property = new StubPropertyBase(
          CreateParameters(
              propertyInfo: GetPropertyInfo(typeof(ClassWithReferenceType<SimpleReferenceType>), "Scalar"),
              underlyingType: typeof(SimpleReferenceType),
              concreteType: typeof(SimpleReferenceType),
              listInfo: null,
              isRequired: false,
              isReadOnly: false));
      var expected = new SimpleReferenceType();

      Assert.That(property.ConvertFromNativePropertyType(expected), Is.SameAs(expected));
    }

    [Test]
    public void ConvertToNativePropertyType_Scalar ()
    {
      PropertyBase property = new StubPropertyBase(
          CreateParameters(
              propertyInfo: GetPropertyInfo(typeof(ClassWithReferenceType<SimpleReferenceType>), "Scalar"),
              underlyingType: typeof(SimpleReferenceType),
              concreteType: typeof(SimpleReferenceType),
              listInfo: null,
              isRequired: false,
              isReadOnly: false));
      var expected = new SimpleReferenceType();

      Assert.That(property.ConvertToNativePropertyType(expected), Is.SameAs(expected));
    }

    [Test]
    public void GetDisplayName_ReflectedClassNotSet ()
    {
      var propertyInfo = GetPropertyInfo(typeof(SimpleBusinessObjectClass), "String");
      var property = new StubPropertyBase(
          CreateParameters(
              propertyInfo: propertyInfo,
              underlyingType: typeof(string),
              concreteType: typeof(string),
              listInfo: null,
              isRequired: false,
              isReadOnly: false,
              bindableObjectGlobalizationService: new BindableObjectGlobalizationService(
                  new Mock<IGlobalizationService>().Object,
                  new Mock<IMemberInformationGlobalizationService>().Object,
                  new Mock<IEnumerationGlobalizationService>().Object,
                  new Mock<IExtensibleEnumGlobalizationService>().Object)));
      Assert.That(
          () => property.DisplayName,
          Throws.InvalidOperationException
              .With.Message.EqualTo("The reflected class for the property 'SimpleBusinessObjectClass.String' is not set."));
    }

    [Test]
    public void GetDisplayName_WithGlobalizationSerivce ()
    {
      var outValue = "MockString";
      var mockMemberInformationGlobalizationService = new Mock<IMemberInformationGlobalizationService>(MockBehavior.Strict);
      IPropertyInformation propertyInfo = GetPropertyInfo(typeof(SimpleBusinessObjectClass), "String");
      PropertyBase property = new StubPropertyBase(
          CreateParameters(
              propertyInfo: propertyInfo,
              underlyingType: typeof(string),
              concreteType: typeof(string),
              listInfo: null,
              isRequired: false,
              isReadOnly: false,
              bindableObjectGlobalizationService: new BindableObjectGlobalizationService(
                  new Mock<IGlobalizationService>().Object,
                  mockMemberInformationGlobalizationService.Object,
                  new Mock<IEnumerationGlobalizationService>().Object,
                  new Mock<IExtensibleEnumGlobalizationService>().Object)));
      property.SetReflectedClass(_bindableObjectClass);

      mockMemberInformationGlobalizationService.Setup(_ => _.TryGetPropertyDisplayName(
              propertyInfo,
              It.Is<ITypeInformation>(c => c.ConvertToRuntimeType() == _bindableObjectClass.TargetType),
              out outValue))
          .Returns(true)
          .Verifiable();

      string actual = property.DisplayName;

      mockMemberInformationGlobalizationService.Verify();
      Assert.That(actual, Is.EqualTo("MockString"));
    }

    [Test]
    public void SetAndGetReflectedClass ()
    {
      PropertyBase property = new StubPropertyBase(
          CreateParameters(
              propertyInfo: GetPropertyInfo(typeof(SimpleBusinessObjectClass), "String"),
              underlyingType: typeof(string),
              concreteType: typeof(string),
              listInfo: null,
              isRequired: false,
              isReadOnly: false));

      property.SetReflectedClass(_bindableObjectClass);

      Assert.That(property.ReflectedClass, Is.SameAs(_bindableObjectClass));
      Assert.That(((IBusinessObjectProperty) property).ReflectedClass, Is.SameAs(_bindableObjectClass));
    }

    [Test]
    public void SetReflectedClass_FromDifferentProviders ()
    {
      var provider = new BindableObjectProvider();
      BindableObjectClass bindableObjectClass = BindableObjectProviderTestHelper.GetBindableObjectClass(typeof(SimpleBusinessObjectClass));

      PropertyBase property = new StubPropertyBase(
          CreateParameters(
              businessObjectProvider: provider,
              propertyInfo: GetPropertyInfo(typeof(SimpleBusinessObjectClass), "String"),
              underlyingType: typeof(string),
              concreteType: typeof(string),
              listInfo: null,
              isRequired: false,
              isReadOnly: false));
      Assert.That(
          () => property.SetReflectedClass(bindableObjectClass),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The BusinessObjectProvider of property 'String' does not match the BusinessObjectProvider of class "
                  + "'Remotion.ObjectBinding.UnitTests.TestDomain.SimpleBusinessObjectClass, Remotion.ObjectBinding.UnitTests'.",
                  "reflectedClass"));
    }

    [Test]
    public void SetReflectedClass_Twice ()
    {
      BindableObjectClass bindableObjectClass = BindableObjectProviderTestHelper.GetBindableObjectClass(typeof(SimpleBusinessObjectClass));

      PropertyBase property = new StubPropertyBase(
          CreateParameters(
              propertyInfo: GetPropertyInfo(typeof(SimpleBusinessObjectClass), "String"),
              underlyingType: typeof(string),
              concreteType: typeof(string),
              listInfo: null,
              isRequired: false,
              isReadOnly: false));

      property.SetReflectedClass(bindableObjectClass);
      Assert.That(
          () => property.SetReflectedClass(BindableObjectProviderTestHelper.GetBindableObjectClass(typeof(ClassWithIdentity))),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The ReflectedClass of a property cannot be changed after it was assigned."
                  + "\r\nClass 'Remotion.ObjectBinding.UnitTests.TestDomain.SimpleBusinessObjectClass, Remotion.ObjectBinding.UnitTests'"
                  + "\r\nProperty 'String'"));
    }

    [Test]
    public void GetReflectedClass_WithoutBusinessObjectClass ()
    {
      PropertyBase property = new StubPropertyBase(
          CreateParameters(
              propertyInfo: GetPropertyInfo(typeof(SimpleBusinessObjectClass), "String"),
              underlyingType: typeof(string),
              concreteType: typeof(string),
              listInfo: null,
              isRequired: false,
              isReadOnly: false));
      Assert.That(
          () => property.ReflectedClass,
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Accessing the ReflectedClass of a property is invalid until the property has been associated with a class.\r\nProperty 'String'"));
    }

    private new PropertyBase.Parameters CreateParameters (
        BindableObjectProvider businessObjectProvider = null,
        IPropertyInformation propertyInfo = null,
        Type underlyingType = null,
        Type concreteType = null,
        IListInfo listInfo = null,
        bool isNullable = true,
        bool isRequired = false,
        bool isReadOnly = false,
        IBindablePropertyReadAccessStrategy bindablePropertyReadAccessStrategy = null,
        IBindablePropertyWriteAccessStrategy bindablePropertyWriteAccessStrategy = null,
        BindableObjectGlobalizationService bindableObjectGlobalizationService = null,
        IBusinessObjectPropertyConstraintProvider businessObjectPropertyConstraintProvider = null)
    {
      return base.CreateParameters(
          businessObjectProvider ?? _bindableObjectProvider,
          propertyInfo,
          underlyingType,
          concreteType,
          listInfo,
          isNullable,
          isRequired,
          isReadOnly,
          bindablePropertyReadAccessStrategy,
          bindablePropertyWriteAccessStrategy,
          bindableObjectGlobalizationService,
          businessObjectPropertyConstraintProvider);
    }

  }
}