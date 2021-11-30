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

namespace Remotion.ObjectBinding.UnitTests.BusinessObjectStringFormatterServiceTests
{
  [TestFixture]
  public class GetPropertyString_WithReferenceProperty
  {
    private BusinessObjectStringFormatterService _stringFormatterService;
    private Mock<IBusinessObject> _businessObjectStub;
    private Mock<IBusinessObjectReferenceProperty> _propertyStub;

    [SetUp]
    public void SetUp ()
    {
      _stringFormatterService = new BusinessObjectStringFormatterService();
      _businessObjectStub = new Mock<IBusinessObject>();
      _propertyStub = new Mock<IBusinessObjectReferenceProperty>();
    }

    [Test]
    public void Scalar_WithBusinessObjectValue ()
    {
      var valueStub = new Mock<IBusinessObject>();
      // Cannot stub ToString()
      // valueStub.Stub (_=>_.ToString()).Return ("ExpectedStringValue");
      _businessObjectStub.Setup(_=>_.GetProperty(_propertyStub.Object)).Returns(valueStub.Object);

      string actual = _stringFormatterService.GetPropertyString(_businessObjectStub.Object, _propertyStub.Object, null);

      Assert.That(actual, Is.EqualTo(valueStub.Object.ToString()));
    }

    [Test]
    public void Scalar_WithBusinessObjectWithIdentityValue ()
    {
      var classStub = new Mock<IBusinessObjectClassWithIdentity>();

      var displayNamePropertyStub = new Mock<IBusinessObjectStringProperty>();
      classStub.Setup(_ => _.GetPropertyDefinition("DisplayName")).Returns(displayNamePropertyStub.Object);

      var valueStub = new Mock<IBusinessObjectWithIdentity>();
      _businessObjectStub.Setup(_=>_.GetProperty(_propertyStub.Object)).Returns(valueStub.Object);
      valueStub.Setup(_ => _.GetProperty(displayNamePropertyStub.Object)).Returns("ExpectedStringValue");
      displayNamePropertyStub.Setup(_ => _.IsAccessible(valueStub.Object)).Returns(true);

      valueStub.Setup(_ => _.BusinessObjectClass).Returns(classStub.Object);
      //var providerStub=MockRepository.GenerateStub<IBusinessObjectProvider>();
      //providerStub.Stub (_ => _.GetService(typeof ()))
      //classStub.Stub (_ => _.BusinessObjectProvider).Return (providerStub);

      string actual = _stringFormatterService.GetPropertyString(_businessObjectStub.Object, _propertyStub.Object, null);

      Assert.That(actual, Is.EqualTo("ExpectedStringValue"));
    }

    [Test]
    public void Scalar_WithBusinessObjectWithIdentityValue_WithAccessDenied ()
    {
      var classStub = new Mock<IBusinessObjectClassWithIdentity>();

      var displayNamePropertyStub = new Mock<IBusinessObjectStringProperty>();
      classStub.Setup(_ => _.GetPropertyDefinition("DisplayName")).Returns(displayNamePropertyStub.Object);

      var valueStub = new Mock<IBusinessObjectWithIdentity>();
      _businessObjectStub.Setup(_=>_.GetProperty(_propertyStub.Object)).Returns(valueStub.Object);
      valueStub.Setup(_ => _.GetProperty(displayNamePropertyStub.Object)).Returns("ExpectedStringValue");
      displayNamePropertyStub.Setup(_ => _.IsAccessible(valueStub.Object)).Returns(false);

      valueStub.Setup(_ => _.BusinessObjectClass).Returns(classStub.Object);
      var providerStub = new Mock<IBusinessObjectProvider>();
      providerStub.Setup(_ => _.GetNotAccessiblePropertyStringPlaceHolder()).Returns("X");
      displayNamePropertyStub.Setup(_ => _.BusinessObjectProvider).Returns(providerStub.Object);

      string actual = _stringFormatterService.GetPropertyString(_businessObjectStub.Object, _propertyStub.Object, null);

      Assert.That(actual, Is.EqualTo("X"));
    }

    [Test]
    public void Scalar_WithNull ()
    {
      _businessObjectStub.Setup(_=>_.GetProperty(_propertyStub.Object)).Returns((object)null);

      string actual = _stringFormatterService.GetPropertyString(_businessObjectStub.Object, _propertyStub.Object, null);

      Assert.That(actual, Is.Empty);
    }
  }
}
