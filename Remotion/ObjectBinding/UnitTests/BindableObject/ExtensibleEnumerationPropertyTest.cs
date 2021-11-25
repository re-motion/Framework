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
using Remotion.ExtensibleEnums;
using Remotion.Globalization;
using Remotion.Globalization.ExtensibleEnums;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Remotion.Reflection;

namespace Remotion.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class ExtensibleEnumerationPropertyTest: TestBase
  {
    private BindableObjectProvider _businessObjectProvider;

    public override void SetUp ()
    {
      base.SetUp();

      _businessObjectProvider = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory();
    }

    [Test]
    public void CreateEnumerationValueInfo_ValueAndIdentifier ()
    {
      var extensibleEnumInfo = ExtensibleEnumWithResources.Values.Value1().GetValueInfo();
      var property = CreateProperty(typeof(ExtensibleEnumWithResources));
      var info = property.CreateEnumerationValueInfo(extensibleEnumInfo, null);

      Assert.That(info.Value, Is.SameAs(extensibleEnumInfo.Value));
      Assert.That(info.Identifier, Is.EqualTo(extensibleEnumInfo.Value.ID));
    }

    [Test]
    public void CreateEnumerationValueInfo_DisplayName_WithGlobalizationService ()
    {
      var extensibleEnumInfo = ExtensibleEnumWithResources.Values.Value1().GetValueInfo();
      var displayNameOutValue = "DisplayName 1";
      var mockExtensibleEnumerationGlobalizationService = new Mock<IExtensibleEnumGlobalizationService>();

      var property = CreateProperty(
          typeof(ExtensibleEnumWithResources),
          new BindableObjectGlobalizationService(
              new Mock<IGlobalizationService>().Object,
              new Mock<IMemberInformationGlobalizationService>().Object,
              new Mock<IEnumerationGlobalizationService>().Object,
              mockExtensibleEnumerationGlobalizationService.Object));
      mockExtensibleEnumerationGlobalizationService
          .Setup(mock => mock.TryGetExtensibleEnumValueDisplayName(extensibleEnumInfo.Value, out displayNameOutValue))
          .Returns(true)
          .Verifiable();

      var info = property.CreateEnumerationValueInfo(extensibleEnumInfo, null);

      mockExtensibleEnumerationGlobalizationService.Verify();
      Assert.That(info.DisplayName, Is.EqualTo("DisplayName 1"));
    }

    [Test]
    public void CreateEnumerationValueInfo_IsEnabled_True ()
    {
      var extensibleEnumInfo = ExtensibleEnumWithResources.Values.Value1().GetValueInfo();
      var property = CreateProperty(typeof(ExtensibleEnumWithResources));
      var info = property.CreateEnumerationValueInfo(extensibleEnumInfo, null);

      Assert.That(info.IsEnabled, Is.True);
    }

    [Test]
    public void CreateEnumerationValueInfo_IsEnabled_False_ViaFilter ()
    {
      var extensibleEnumInfo = ExtensibleEnumWithResources.Values.Value1().GetValueInfo();
      var businessObjectStub = new Mock<IBusinessObject>();

      var filterMock = new Mock<IEnumerationValueFilter>(MockBehavior.Strict);

      var property = CreatePropertyWithSpecificFilter(filterMock.Object);

      // the filter must be called exactly as specified
      filterMock
          .Setup(mock => mock.IsEnabled(
              It.Is<IEnumerationValueInfo>(
                  i => i.Value.Equals(extensibleEnumInfo.Value)
                       && i.Identifier == extensibleEnumInfo.Value.ID
                       && i.DisplayName == string.Empty
                       && i.IsEnabled),
              businessObjectStub.Object,
              property))
          .Returns(false)
          .Verifiable();
      
      var info = property.CreateEnumerationValueInfo(extensibleEnumInfo, businessObjectStub.Object);

      filterMock.Verify();

      Assert.That(info.IsEnabled, Is.False);
    }

    [Test]
    public void IsEnabled_IntegrationTest ()
    {
      var property = CreateProperty(typeof(ExtensibleEnumWithFilter));

      Assert.That(IsEnabled(property, ExtensibleEnumWithFilter.Values.Value1()),  Is.False);
      Assert.That(IsEnabled(property, ExtensibleEnumWithFilter.Values.Value2()), Is.True);
      Assert.That(IsEnabled(property, ExtensibleEnumWithFilter.Values.Value3()), Is.False);
      Assert.That(IsEnabled(property, ExtensibleEnumWithFilter.Values.Value4()), Is.True);

      var propertyInfoStub = new Mock<IPropertyInformation>();
      propertyInfoStub.Setup(stub => stub.PropertyType).Returns(typeof(ExtensibleEnumWithResources));
      propertyInfoStub.Setup(stub => stub.GetIndexParameters()).Returns(new ParameterInfo[0]);
      propertyInfoStub
          .Setup(stub => stub.GetCustomAttribute<DisableExtensibleEnumValuesAttribute>(true))
          .Returns(new DisableExtensibleEnumValuesAttribute(ExtensibleEnumWithResources.Values.Value1().ID));

      var propertyWithFilter = new ExtensibleEnumerationProperty(GetPropertyParameters(propertyInfoStub.Object, _businessObjectProvider));
      Assert.That(IsEnabled(propertyWithFilter, ExtensibleEnumWithResources.Values.Value1()), Is.False);
      Assert.That(IsEnabled(propertyWithFilter, ExtensibleEnumWithResources.Values.Value2()), Is.True);
    }

    [Test]
    public void GetAllValues ()
    {
      var property = CreateProperty(typeof(ExtensibleEnumWithResources));
      var valueInfos = property.GetAllValues(null);

      Assert.That(valueInfos.Length, Is.EqualTo(3));

      Assert.That(valueInfos[0].Value, Is.EqualTo(ExtensibleEnumWithResources.Values.Value1()));
      Assert.That(valueInfos[1].Value, Is.EqualTo(ExtensibleEnumWithResources.Values.Value2()));
      Assert.That(valueInfos[2].Value, Is.EqualTo(ExtensibleEnumWithResources.Values.ValueWithoutResource()));
    }

    [Test]
    public void GetEnabledValues ()
    {
      var property = CreateProperty(typeof(ExtensibleEnumWithFilter));

      var valueInfos = property.GetEnabledValues(null);

      Assert.That(valueInfos.Length, Is.EqualTo(2));

      Assert.That(valueInfos[0].Value, Is.EqualTo(ExtensibleEnumWithFilter.Values.Value2()));
      Assert.That(valueInfos[1].Value, Is.EqualTo(ExtensibleEnumWithFilter.Values.Value4()));
    }

    [Test]
    public void GetValueInfoByIdentifier ()
    {
      var property = CreateProperty(typeof(ExtensibleEnumWithResources));
      var info = property.GetValueInfoByIdentifier(ExtensibleEnumWithResources.Values.Value1().ID, null);
      Assert.That(info.Value, Is.EqualTo(ExtensibleEnumWithResources.Values.Value1()));
    }

    [Test]
    public void GetValueInfoByIdentifier_NullOrEmpty ()
    {
      var property = CreateProperty(typeof(ExtensibleEnumWithResources));
      Assert.That(property.GetValueInfoByIdentifier("", null), Is.Null);
      Assert.That(property.GetValueInfoByIdentifier(null, null), Is.Null);
    }

    [Test]
    public void GetValueInfoByIdentifier_InvalidID ()
    {
      var property = CreateProperty(typeof(ExtensibleEnumWithResources));
      Assert.That(
          () => property.GetValueInfoByIdentifier("?", null),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The identifier '?' does not identify a defined value for type " 
                  + "'Remotion.ObjectBinding.UnitTests.TestDomain.ExtensibleEnumWithResources'.", "identifier"));
    }

    [Test]
    public void GetValueInfoByValue ()
    {
      var property = CreateProperty(typeof(ExtensibleEnumWithResources));
      var info = property.GetValueInfoByValue(ExtensibleEnumWithResources.Values.Value1(), null);
      Assert.That(info.Value, Is.EqualTo(ExtensibleEnumWithResources.Values.Value1()));
    }

    [Test]
    public void GetValueInfoByValue_Null ()
    {
      var property = CreateProperty(typeof(ExtensibleEnumWithResources));
      var info = property.GetValueInfoByValue(null, null);
      Assert.That(info, Is.Null);
    }

    [Test]
    public void GetValueInfoByValue_UndefinedValue ()
    {
      var property = CreateProperty(typeof(ExtensibleEnumWithResources));
      var info = property.GetValueInfoByValue(new ExtensibleEnumWithResources(MethodBase.GetCurrentMethod()), null);
      Assert.That(info, Is.Null);
    }

    [Test]
    public void GetValueInfoByValue_InvalidType ()
    {
      var property = CreateProperty(typeof(ExtensibleEnumWithResources));
      var info = property.GetValueInfoByValue("?", null);
      Assert.That(info, Is.Null);
    }

    private ExtensibleEnumerationProperty CreateProperty (
        Type propertyType,
        BindableObjectGlobalizationService bindableObjectGlobalizationService = null)
    {
      var propertyStub = new Mock<IPropertyInformation>();
      propertyStub.Setup(stub => stub.PropertyType).Returns(propertyType);
      propertyStub.Setup(stub => stub.GetIndexParameters()).Returns(new ParameterInfo[0]);

      var parameters = GetPropertyParameters(
          propertyStub.Object,
          _businessObjectProvider,
          bindableObjectGlobalizationService: bindableObjectGlobalizationService);
      return new ExtensibleEnumerationProperty(parameters);
    }

    private ExtensibleEnumerationProperty CreatePropertyWithSpecificFilter (IEnumerationValueFilter filterMock)
    {
      var attribute = new DisableExtensibleEnumValuesAttribute("x");
      PrivateInvoke.SetNonPublicField(attribute, "_filter", filterMock);

      var propertyInfoStub = new Mock<IPropertyInformation>();
      propertyInfoStub.Setup(stub => stub.PropertyType).Returns(typeof(ExtensibleEnumWithResources));
      propertyInfoStub.Setup(stub => stub.GetIndexParameters()).Returns(new ParameterInfo[0]);
      propertyInfoStub
          .Setup(stub => stub.GetCustomAttribute<DisableExtensibleEnumValuesAttribute>(true))
          .Returns(attribute);

      return new ExtensibleEnumerationProperty(GetPropertyParameters(propertyInfoStub.Object, _businessObjectProvider));
    }
    
    private bool IsEnabled (ExtensibleEnumerationProperty propertyWithFilteredType, IExtensibleEnum value)
    {
      return propertyWithFilteredType.CreateEnumerationValueInfo(value.GetValueInfo(), null).IsEnabled;
    }
  }
}