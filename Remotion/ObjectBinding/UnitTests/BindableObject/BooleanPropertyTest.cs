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
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class BooleanPropertyTest : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;
    private IBusinessObjectClass _businessObjectClass;
    private BindableObjectGlobalizationService _bindableObjectGlobalizationService;

    public override void SetUp ()
    {
      base.SetUp();

      _businessObjectProvider = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory();
      _bindableObjectGlobalizationService = SafeServiceLocator.Current.GetInstance<BindableObjectGlobalizationService>();
      ClassReflector classReflector = new ClassReflector(
          typeof(ClassWithValueType<bool>),
          _businessObjectProvider,
          BindableObjectMetadataFactory.Create(),
          _bindableObjectGlobalizationService);
      _businessObjectClass = classReflector.GetMetadata();
    }

    [Test]
    public void GetDefaultValue_Scalar ()
    {
      IBusinessObjectBooleanProperty property = CreateProperty("Scalar");

      Assert.That(property.GetDefaultValue(_businessObjectClass), Is.False);
    }

    [Test]
    public void GetDefaultValue_NullableScalar ()
    {
      IBusinessObjectBooleanProperty property = CreateProperty("NullableScalar");

      Assert.That(property.GetDefaultValue(_businessObjectClass), Is.Null);
    }

    [Test]
    public void GetDefaultValue_Array ()
    {
      IBusinessObjectBooleanProperty property = new BooleanProperty(
          CreateParameters(
              _businessObjectProvider,
              GetPropertyInfo(typeof(ClassWithValueType<bool>), "Array"),
              typeof(bool),
              typeof(bool),
              new ListInfo(typeof(bool[]), typeof(bool)),
              true,
              false,
              false));

      Assert.That(property.GetDefaultValue(_businessObjectClass), Is.False);
    }

    [Test]
    public void GetDefaultValue_NullableArray ()
    {
      IBusinessObjectBooleanProperty property = new BooleanProperty(
          CreateParameters(
              _businessObjectProvider,
              GetPropertyInfo(typeof(ClassWithValueType<bool>), "NullableArray"),
              typeof(bool),
              typeof(bool),
              new ListInfo(typeof(bool?[]), typeof(bool?)),
              false,
              false,
              false));

      Assert.That(property.GetDefaultValue(_businessObjectClass), Is.Null);
    }

    [Test]
    public void GetDisplayName_WithGlobalizationSerivce ()
    {
      var outValue = "MockTrue";

      var mockglobalizationService = new Mock<IGlobalizationService>(MockBehavior.Strict);
      IBusinessObjectBooleanProperty property = CreateProperty(
          "Scalar",
          bindableObjectGlobalizationService: new BindableObjectGlobalizationService(
              mockglobalizationService.Object,
              new Mock<IMemberInformationGlobalizationService>().Object,
              new Mock<IEnumerationGlobalizationService>().Object,
              new Mock<IExtensibleEnumGlobalizationService>().Object));

      var mockResourceManager = new Mock<IResourceManager>(MockBehavior.Strict);
      mockglobalizationService
          .Setup(_ => _.GetResourceManager(TypeAdapter.Create(typeof(BindableObjectGlobalizationService.ResourceIdentifier))))
          .Returns(mockResourceManager.Object)
          .Verifiable();

      mockResourceManager
          .Setup(_ => _.TryGetString("Remotion.ObjectBinding.BindableObject.BindableObjectGlobalizationService.True", out outValue))
          .Returns(true)
          .Verifiable();

      string actual = property.GetDisplayName(true);

      mockglobalizationService.Verify();
      mockResourceManager.Verify();
      Assert.That(actual, Is.EqualTo("MockTrue"));
    }

    [Test]
    public void IBusinessObjectEnumerationProperty_GetAllValues ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty("NullableScalar");
      BooleanEnumerationValueInfo[] expected = new []
          {
              new BooleanEnumerationValueInfo(true, (IBusinessObjectBooleanProperty)property),
              new BooleanEnumerationValueInfo(false, (IBusinessObjectBooleanProperty)property)
          };

      CheckEnumerationValueInfos(expected, property.GetAllValues(null));
    }

    [Test]
    public void IBusinessObjectEnumerationProperty_GetEnabledValues ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty("NullableScalar");
      BooleanEnumerationValueInfo[] expected = new []
          {
              new BooleanEnumerationValueInfo(true, (IBusinessObjectBooleanProperty)property),
              new BooleanEnumerationValueInfo(false, (IBusinessObjectBooleanProperty)property)
          };

      CheckEnumerationValueInfos(expected, property.GetEnabledValues(null));
    }

    [Test]
    public void IBusinessObjectEnumerationProperty_GetValueInfoByValue_WithTrue ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty("Scalar");

      CheckEnumerationValueInfo(
          new BooleanEnumerationValueInfo(true, (IBusinessObjectBooleanProperty)property),
          property.GetValueInfoByValue(true, null));
    }

    [Test]
    public void IBusinessObjectEnumerationProperty_GetValueInfoByValue_WithFalse ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty("Scalar");

      CheckEnumerationValueInfo(
          new BooleanEnumerationValueInfo(false, (IBusinessObjectBooleanProperty)property),
          property.GetValueInfoByValue(false, null));
    }

    [Test]
    public void IBusinessObjectEnumerationProperty_GetValueInfoByValue_WithNull ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty("Scalar");

      Assert.That(property.GetValueInfoByValue(null, null), Is.Null);
    }

    [Test]
    public void IBusinessObjectEnumerationProperty_GetValueInfoByIdentifier_WithTrue ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty("Scalar");

      CheckEnumerationValueInfo(
          new BooleanEnumerationValueInfo(true, (IBusinessObjectBooleanProperty)property),
          property.GetValueInfoByIdentifier("True", null));
    }

    [Test]
    public void IBusinessObjectEnumerationProperty_GetValueInfoByIdentifier_WithFalse ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty("Scalar");

      CheckEnumerationValueInfo(
          new BooleanEnumerationValueInfo(false, (IBusinessObjectBooleanProperty)property),
          property.GetValueInfoByIdentifier("False", null));
    }

    [Test]
    public void IBusinessObjectEnumerationProperty_GetValueInfoByIdentifier_WithNull ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty("Scalar");

      Assert.That(property.GetValueInfoByIdentifier(null, null), Is.Null);
    }

    [Test]
    public void IBusinessObjectEnumerationProperty_GetValueInfoByIdentifier_WithEmptyString ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty("Scalar");

      Assert.That(property.GetValueInfoByIdentifier(string.Empty, null), Is.Null);
    }

    private BooleanProperty CreateProperty (
        string propertyName,
        BindableObjectProvider provider = null,
        BindableObjectGlobalizationService bindableObjectGlobalizationService = null)
    {
      return new BooleanProperty(
          GetPropertyParameters(
              GetPropertyInfo(typeof(ClassWithValueType<bool>), propertyName),
              provider ?? _businessObjectProvider,
              bindableObjectGlobalizationService: bindableObjectGlobalizationService));
    }

    private void CheckEnumerationValueInfos (BooleanEnumerationValueInfo[] expected, IEnumerationValueInfo[] actual)
    {
      ArgumentUtility.CheckNotNull("expected", expected);

      Assert.That(actual, Is.Not.Null);
      Assert.That(actual.Length, Is.EqualTo(expected.Length));
      for (int i = 0; i < expected.Length; i++)
        CheckEnumerationValueInfo(expected[i], actual[i]);
    }

    private void CheckEnumerationValueInfo (BooleanEnumerationValueInfo expected, IEnumerationValueInfo actual)
    {
      ArgumentUtility.CheckNotNull("expected", expected);

      Assert.That(actual, Is.InstanceOf(expected.GetType()));
      Assert.That(actual.Value, Is.EqualTo(expected.Value));
      Assert.That(actual.Identifier, Is.EqualTo(expected.Identifier));
      Assert.That(actual.IsEnabled, Is.EqualTo(expected.IsEnabled));
      Assert.That(actual.DisplayName, Is.EqualTo(expected.DisplayName));
    }
  }
}
