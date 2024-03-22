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

namespace Remotion.ObjectBinding.UnitTests
{
  [TestFixture]
  public class BusinessObjectExtensionMethodsTest
  {
    [Test]
    public void GetProperty ()
    {
      object exptected = new object();
      var businessObjectStub = new Mock<IBusinessObject>();
      var classStub = new Mock<IBusinessObjectClass>();
      var propertyStub = new Mock<IBusinessObjectProperty>();

      businessObjectStub.Setup(_ => _.BusinessObjectClass).Returns(classStub.Object);
      classStub.Setup(_ => _.GetPropertyDefinition("TheProperty")).Returns(propertyStub.Object);
      businessObjectStub.Setup(_ => _.GetProperty(propertyStub.Object)).Returns(exptected);

      object actual = businessObjectStub.Object.GetProperty("TheProperty");

      Assert.That(actual, Is.EqualTo(exptected));
    }

    [Test]
    public void SetProperty ()
    {
      object exptected = new object();
      var businessObjectMock = new Mock<IBusinessObject>();
      var classStub = new Mock<IBusinessObjectClass>();
      var propertyStub = new Mock<IBusinessObjectProperty>();

      businessObjectMock.Setup(_ => _.BusinessObjectClass).Returns(classStub.Object);
      classStub.Setup(_ => _.GetPropertyDefinition("TheProperty")).Returns(propertyStub.Object);
      businessObjectMock.Setup(_ => _.SetProperty(propertyStub.Object, exptected)).Verifiable();

      businessObjectMock.Object.SetProperty("TheProperty", exptected);

      businessObjectMock.Verify();
    }

    [Test]
    public void GetPropertyString ()
    {
      string exptected = "TheValue";
      var businessObjectStub = new Mock<IBusinessObject>();
      var classStub = new Mock<IBusinessObjectClass>();
      var propertyStub = new Mock<IBusinessObjectProperty>();

      businessObjectStub.Setup(_ => _.BusinessObjectClass).Returns(classStub.Object);
      classStub.Setup(_ => _.GetPropertyDefinition("TheProperty")).Returns(propertyStub.Object);
      businessObjectStub.Setup(_ => _.GetPropertyString(propertyStub.Object, null)).Returns(exptected);

      string actual = businessObjectStub.Object.GetPropertyString("TheProperty");

      Assert.That(actual, Is.EqualTo(exptected));
    }

    [Test]
    public void GetProperty_WithInvalidPropertyIdentifier ()
    {
      var businessObjectStub = new Mock<IBusinessObject>();
      var classStub = new Mock<IBusinessObjectClass>();

      businessObjectStub.Setup(_ => _.BusinessObjectClass).Returns(classStub.Object);
      classStub.Setup(_ => _.GetPropertyDefinition("InvalidProperty")).Returns((IBusinessObjectProperty)null);
      classStub.Setup(_ => _.Identifier).Returns("TheClass");

      Assert.That(
          () => businessObjectStub.Object.GetProperty("InvalidProperty"),
          Throws.InvalidOperationException.With.Message.EqualTo(
              "The business object's class ('TheClass') does not contain a property named 'InvalidProperty'."));
    }

    [Test]
    public void GetAccessibleDisplayName_WithoutBusinessObjectProperty_ReturnsDisplayNamePropertyFromInterfaceImplementation ()
    {
      var businessObjectStub = new Mock<IBusinessObjectWithIdentity>();
      var classStub = new Mock<IBusinessObjectClass>();

      classStub.Setup(_ => _.GetPropertyDefinition("DisplayName")).Returns((IBusinessObjectProperty)null);
      businessObjectStub.Setup(_ => _.BusinessObjectClass).Returns(classStub.Object);
      businessObjectStub.Setup(_ => _.DisplayName).Returns("The DisplayName");

      string actual = businessObjectStub.Object.GetAccessibleDisplayName();

      Assert.That(actual, Does.StartWith("The DisplayName"));
    }

    [Test]
    public void GetAccessibleDisplayName_WithBusinessObjectProperty_AndAccessGranted_ReturnsValueViaBusinessObjectProperty ()
    {
      var businessObjectStub = new Mock<IBusinessObjectWithIdentity>();
      var classStub = new Mock<IBusinessObjectClass>();
      var propertyMock = new Mock<IBusinessObjectProperty>();

      classStub.Setup(_ => _.GetPropertyDefinition("DisplayName")).Returns(propertyMock.Object);
      propertyMock.Setup(_ => _.IsAccessible(businessObjectStub.Object)).Returns(true).Verifiable();
      businessObjectStub.Setup(_ => _.BusinessObjectClass).Returns(classStub.Object);
      businessObjectStub.Setup(_ => _.DisplayName).Throws(new AssertionException("Should not be called."));
      businessObjectStub.Setup(_ => _.GetProperty(propertyMock.Object)).Returns("The DisplayName");

      string actual = businessObjectStub.Object.GetAccessibleDisplayName();

      propertyMock.Verify();
      Assert.That(actual, Is.EqualTo("The DisplayName"));
    }

    [Test]
    public void GetAccessibleDisplayName_WithBusinessObjectProperty_AndAccessDenied_ReturnsNotAccessiblePlaceHolder ()
    {
      var businessObjectStub = new Mock<IBusinessObjectWithIdentity>();
      var classStub = new Mock<IBusinessObjectClass>();
      var propertyMock = new Mock<IBusinessObjectProperty>();
      var businessObjectProviderStub = new Mock<IBusinessObjectProvider>();

      classStub.Setup(_ => _.GetPropertyDefinition("DisplayName")).Returns(propertyMock.Object);
      propertyMock.Setup(_ => _.IsAccessible(businessObjectStub.Object)).Returns(false).Verifiable();
      propertyMock.Setup(_ => _.BusinessObjectProvider).Returns(businessObjectProviderStub.Object);
      businessObjectStub.Setup(_ => _.BusinessObjectClass).Returns(classStub.Object);
      businessObjectStub.Setup(_ => _.DisplayName).Throws(new AssertionException("Should not be called."));
      businessObjectStub.Setup(_ => _.GetProperty(propertyMock.Object)).Throws(new AssertionException("Should not be called."));
      businessObjectProviderStub.Setup(_ => _.GetNotAccessiblePropertyStringPlaceHolder()).Returns("N/A");
      string actual = businessObjectStub.Object.GetAccessibleDisplayName();

      propertyMock.Verify();
      Assert.That(actual, Is.EqualTo("N/A"));
    }

    [Test]
    public void GetAccessibleDisplayName_WithBusinessObjectProperty_AndBusinessObjectPropertyAccessException_ReturnsNotAccessiblePlaceHolder ()
    {
      var businessObjectStub = new Mock<IBusinessObjectWithIdentity>();
      var classStub = new Mock<IBusinessObjectClass>();
      var propertyMock = new Mock<IBusinessObjectProperty>();
      var businessObjectProviderStub = new Mock<IBusinessObjectProvider>();

      classStub.Setup(_ => _.GetPropertyDefinition("DisplayName")).Returns(propertyMock.Object);
      propertyMock.Setup(_ => _.IsAccessible(businessObjectStub.Object)).Returns(true).Verifiable();
      propertyMock.Setup(_ => _.BusinessObjectProvider).Returns(businessObjectProviderStub.Object);
      businessObjectStub.Setup(_ => _.BusinessObjectClass).Returns(classStub.Object);
      businessObjectStub.Setup(_ => _.DisplayName).Throws(new AssertionException("Should not be called."));
      businessObjectStub.Setup(_ => _.GetProperty(propertyMock.Object)).Throws(new BusinessObjectPropertyAccessException("The Message", null));
      businessObjectProviderStub.Setup(_ => _.GetNotAccessiblePropertyStringPlaceHolder()).Returns("N/A");
      string actual = businessObjectStub.Object.GetAccessibleDisplayName();

      propertyMock.Verify();
      Assert.That(actual, Is.EqualTo("N/A"));
    }

    [Test]
    public void GetAccessibleDisplayName_WithBusinessObjectProperty_AndOtherException_ReThrowsOriginalException ()
    {
      var expectedException = new Exception("The Message", null);

      var businessObjectStub = new Mock<IBusinessObjectWithIdentity>();
      var classStub = new Mock<IBusinessObjectClass>();
      var propertyMock = new Mock<IBusinessObjectProperty>();
      var businessObjectProviderStub = new Mock<IBusinessObjectProvider>();

      classStub.Setup(_ => _.GetPropertyDefinition("DisplayName")).Returns(propertyMock.Object);
      propertyMock.Setup(_ => _.IsAccessible(businessObjectStub.Object)).Returns(true).Verifiable();
      businessObjectStub.Setup(_ => _.BusinessObjectClass).Returns(classStub.Object);
      businessObjectStub.Setup(_ => _.DisplayName).Throws(new AssertionException("Should not be called."));
      businessObjectStub.Setup(_ => _.GetProperty(propertyMock.Object)).Throws(expectedException);
      businessObjectProviderStub.Setup(_ => _.GetNotAccessiblePropertyStringPlaceHolder()).Returns("N/A");
      Assert.That(() => businessObjectStub.Object.GetAccessibleDisplayName(), Throws.Exception.SameAs(expectedException));

      propertyMock.Verify();
    }
  }
}
