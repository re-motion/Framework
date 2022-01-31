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
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.BindableObject.ReferencePropertyTests.TestDomain;
using Remotion.TypePipe;

namespace Remotion.ObjectBinding.UnitTests.BindableObject.ReferencePropertyTests
{
#pragma warning disable 612,618
  [TestFixture]
  public class IsDefaultValue : TestBase
  {
    private BindableObjectProvider _bindableObjectProviderForDeclaringType;
    private BindableObjectProvider _bindableObjectProviderForPropertyType;

    public override void SetUp ()
    {
      base.SetUp();

      _bindableObjectProviderForDeclaringType = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory();
      _bindableObjectProviderForPropertyType = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory();

      BusinessObjectProvider.SetProvider<BindableObjectProviderAttribute>(_bindableObjectProviderForDeclaringType);
      BusinessObjectProvider.SetProvider<BindableObjectProviderForDefaultValueServiceAttribute>(_bindableObjectProviderForPropertyType);
    }

    [Test]
    public void IsDefaultValue_WithDefaultValueSupported ()
    {
      var stubBusinessObject = new Mock<IBusinessObject>();
      var mockService = new Mock<IDefaultValueServiceOnProperty>(MockBehavior.Strict);
      IBusinessObjectReferenceProperty property = CreateProperty("DefaultValueServiceFromPropertyDeclaration");
      var value = new Mock<IBusinessObject>();
      var emptyProperties = new IBusinessObjectProperty[0];

      var sequence = new VerifiableSequence();
      mockService.InVerifiableSequence(sequence).Setup(_ => _.SupportsProperty(property)).Returns(true).Verifiable();
      mockService.InVerifiableSequence(sequence).Setup(_ => _.IsDefaultValue(stubBusinessObject.Object, property, value.Object, emptyProperties)).Returns(true).Verifiable();

      _bindableObjectProviderForDeclaringType.AddService(mockService.Object);
      bool actual = property.IsDefaultValue(stubBusinessObject.Object, value.Object, emptyProperties);

      mockService.Verify();
      sequence.Verify();
      Assert.That(actual, Is.True);
    }

    [Test]
    public void IsDefaultValue_WithDefaultValueSupportedAndReferencingObjectNull ()
    {
      var mockService = new Mock<IDefaultValueServiceOnType>(MockBehavior.Strict);
      var property = CreateProperty("DefaultValueServiceFromPropertyType");
      var value = new Mock<IBusinessObject>();
      var emptyProperties = new IBusinessObjectProperty[0];

      var sequence = new VerifiableSequence();
      mockService.InVerifiableSequence(sequence).Setup(_ => _.SupportsProperty(property)).Returns(true).Verifiable();
      mockService.InVerifiableSequence(sequence).Setup(_ => _.IsDefaultValue(null, property, value.Object, emptyProperties)).Returns(true).Verifiable();

      _bindableObjectProviderForPropertyType.AddService(mockService.Object);
      bool actual = property.IsDefaultValue(null, value.Object, emptyProperties);

      mockService.Verify();
      sequence.Verify();
      Assert.That(actual, Is.True);
    }

    [Test]
    public void IsDefaultValue_WithDefaultValueNotSupported ()
    {
      IBusinessObject businessObject = (IBusinessObject)ObjectFactory.Create<ClassWithBusinessObjectProperties>(ParamList.Empty);
      var mockService = new Mock<IDefaultValueServiceOnProperty>(MockBehavior.Strict);
      IBusinessObjectReferenceProperty property = CreateProperty("DefaultValueServiceFromPropertyDeclaration");
      var value = new Mock<IBusinessObject>();
      var emptyProperties = new IBusinessObjectProperty[0];

      mockService.Setup(_ => _.SupportsProperty(property)).Returns(false).Verifiable();

      _bindableObjectProviderForDeclaringType.AddService(mockService.Object);

      Assert.That(
          () => property.IsDefaultValue(businessObject, value.Object, emptyProperties),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "Checking for a value's default is not supported for reference property 'DefaultValueServiceFromPropertyDeclaration' of business object class "
                  + "'Remotion.ObjectBinding.UnitTests.BindableObject.ReferencePropertyTests.TestDomain.ClassWithBusinessObjectProperties, "
                  + "Remotion.ObjectBinding.UnitTests'."));
      mockService.Verify();
    }

    private ReferenceProperty CreateProperty (string propertyName)
    {
      PropertyBase.Parameters propertyParameters =
          GetPropertyParameters(GetPropertyInfo(typeof(ClassWithBusinessObjectProperties), propertyName), _bindableObjectProviderForDeclaringType);
      ReferenceProperty property = new ReferenceProperty(propertyParameters);
      property.SetReflectedClass(BindableObjectProviderTestHelper.GetBindableObjectClass(typeof(ClassWithBusinessObjectProperties)));

      return property;
    }
  }
#pragma warning restore 612,618
}
