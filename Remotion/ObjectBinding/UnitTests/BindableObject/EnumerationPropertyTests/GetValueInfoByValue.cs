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

namespace Remotion.ObjectBinding.UnitTests.BindableObject.EnumerationPropertyTests
{
  [TestFixture]
  public class GetValueInfoByValue : EnumerationTestBase
  {
    private BindableObjectProvider _businessObjectProvider;
    private Mock<IBusinessObject> _mockBusinessObject;

    public override void SetUp ()
    {
      base.SetUp();

      _businessObjectProvider = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory();

      _mockBusinessObject = new Mock<IBusinessObject>(MockBehavior.Strict);
    }

    [Test]
    public void ValidEnumValue ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty(typeof(ClassWithValueType<TestEnum>), "Scalar");

      CheckEnumerationValueInfo(
          new EnumerationValueInfo(TestEnum.Value1, "Value1", "Value1", true),
          property.GetValueInfoByValue(TestEnum.Value1, null));
    }

    [Test]
    public void Null ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty(typeof(ClassWithValueType<TestEnum>), "Scalar");

      Assert.That(property.GetValueInfoByValue(null, null), Is.Null);
    }

    [Test]
    public void UndefinedEnumValue ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty(typeof(ClassWithValueType<EnumWithUndefinedValue>), "Scalar");

      Assert.That(property.GetValueInfoByValue(EnumWithUndefinedValue.UndefinedValue, null), Is.Null);
    }

    [Test]
    public void InvalidEnumValue ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty(typeof(ClassWithValueType<TestEnum>), "Scalar");

      CheckEnumerationValueInfo(
          new EnumerationValueInfo((TestEnum)(-1), "-1", "-1", false),
          property.GetValueInfoByValue((TestEnum)(-1), null));
    }

    [Test]
    public void DisabledEnumValue ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty(typeof(ClassWithDisabledEnumValue), "DisabledFromProperty");

      IEnumerationValueInfo actual = property.GetValueInfoByValue(TestEnum.Value1, _mockBusinessObject.Object);

      _mockBusinessObject.Verify();
      CheckEnumerationValueInfo(new EnumerationValueInfo(TestEnum.Value1, "Value1", "Value1", false), actual);
    }

    [Test]
    public void EnumValueFromOtherType ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty(typeof(ClassWithValueType<TestEnum>), "Scalar");
      Assert.That(
          () => property.GetValueInfoByValue(EnumWithUndefinedValue.Value1, null),
          Throws.ArgumentException
              .With.Message.EqualTo(
                  "Object must be the same type as the enum. The type passed in was 'Remotion.ObjectBinding.UnitTests.TestDomain.EnumWithUndefinedValue'; "
                  + "the enum type was 'Remotion.ObjectBinding.UnitTests.TestDomain.TestEnum'."));
    }

    [Test]
    public void GetDisplayNameFromGlobalizationSerivce ()
    {
      var outValue = "MockValue1";
      var mockEnumerationGlobalizationService = new Mock<IEnumerationGlobalizationService>(MockBehavior.Strict);
      IBusinessObjectEnumerationProperty property = CreateProperty(
          typeof(ClassWithValueType<TestEnum>),
          "Scalar",
          bindableObjectGlobalizationService: new BindableObjectGlobalizationService(
              new Mock<IGlobalizationService>().Object,
              new Mock<IMemberInformationGlobalizationService>().Object,
              mockEnumerationGlobalizationService.Object,
              new Mock<IExtensibleEnumGlobalizationService>().Object));

      mockEnumerationGlobalizationService
          .Setup(_ => _.TryGetEnumerationValueDisplayName(TestEnum.Value1, out outValue))
          .Returns(true)
          .Verifiable();

      IEnumerationValueInfo actual = property.GetValueInfoByValue(TestEnum.Value1, null);

      _mockBusinessObject.Verify();
      mockEnumerationGlobalizationService.Verify();
      CheckEnumerationValueInfo(new EnumerationValueInfo(TestEnum.Value1, "Value1", "MockValue1", true), actual);
    }

    private EnumerationProperty CreateProperty (
        Type type,
        string propertyName,
        BindableObjectGlobalizationService bindableObjectGlobalizationService = null)
    {
      return new EnumerationProperty(
          GetPropertyParameters(
              GetPropertyInfo(type, propertyName),
              _businessObjectProvider,
              bindableObjectGlobalizationService: bindableObjectGlobalizationService));
    }
  }
}
