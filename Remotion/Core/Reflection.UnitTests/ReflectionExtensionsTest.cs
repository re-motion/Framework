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
using Moq.Protected;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.Reflection.UnitTests
{
  [TestFixture]
  public class ReflectionExtensionsTest
  {
    private class TheType
    {
      public int TheProperty { get; set; }
    }

    [Test]
    public void IsOriginalDeclaration_DeclaringTypeEqualsOrignalDeclaringType_True ()
    {
      var memberInfoStub = new Mock<IMemberInformation>();
      var typeInformationStub = new Mock<ITypeInformation>();
      memberInfoStub.Setup (stub => stub.DeclaringType).Returns (typeInformationStub.Object);
      memberInfoStub.Setup (stub => stub.GetOriginalDeclaringType()).Returns (typeInformationStub.Object);
      Assert.That (memberInfoStub.Object.IsOriginalDeclaration(), Is.True);
    }

    [Test]
    public void IsOriginalDeclaration_DeclaringTypeNotEqualToOrignalDeclaringType_False ()
    {
      var memberInfoStub = new Mock<IMemberInformation>();
      memberInfoStub.Setup (stub => stub.DeclaringType).Returns (new Mock<ITypeInformation>().Object);
      memberInfoStub.Setup (stub => stub.GetOriginalDeclaringType()).Returns (new Mock<ITypeInformation>().Object);

      Assert.That (memberInfoStub.Object.IsOriginalDeclaration (), Is.False);
    }

    [Test]
    public void IsOriginalDeclaration_DeclaringTypeIsNull_False ()
    {
      var memberInfoStub = new Mock<IMemberInformation>();
      memberInfoStub.Setup (stub => stub.DeclaringType).Returns ((ITypeInformation) null);
      memberInfoStub.Setup (stub => stub.GetOriginalDeclaringType()).Returns (new Mock<ITypeInformation>().Object);

      Assert.That (memberInfoStub.Object.IsOriginalDeclaration (), Is.False);
    }

    [Test]
    public void IsOriginalDeclaration_OrignalDeclaringTypeIsNull_False ()
    {
      var memberInfoStub = new Mock<IMemberInformation>();
      memberInfoStub.Setup (stub => stub.DeclaringType).Returns (new Mock<ITypeInformation>().Object);
      memberInfoStub.Setup (stub => stub.GetOriginalDeclaringType ()).Returns ((ITypeInformation) null);

      Assert.That (memberInfoStub.Object.IsOriginalDeclaration (), Is.False);
    }

    [Test]
    public void IsOriginalDeclaration_DeclaringTypeIsNullAndOrignalDeclaringTypeIsNull_True ()
    {
      var memberInfoStub = new Mock<IMemberInformation>();
      memberInfoStub.Setup (stub => stub.DeclaringType).Returns ((ITypeInformation) null);
      memberInfoStub.Setup (stub => stub.GetOriginalDeclaringType ()).Returns ((ITypeInformation) null);

      Assert.That (memberInfoStub.Object.IsOriginalDeclaration (), Is.True);
    }

    [Test]
    public void AsRuntimeType_WithTypeAdapter_ReturnsRuntimeType ()
    {
      var expectedType = typeof (TheType);
      ITypeInformation typeInformation = TypeAdapter.Create (expectedType);

      Assert.That (typeInformation.AsRuntimeType(), Is.SameAs (expectedType));
    }

    [Test]
    public void AsRuntimeType_WithOtherITypeInformation_ReturnsNull ()
    {
      var typeInformation = new Mock<ITypeInformation>();

      Assert.That (typeInformation.Object.AsRuntimeType(), Is.Null);
    }

    [Test]
    public void ConvertToRuntimeType_WithTypeAdapter_ReturnsRuntimeType ()
    {
      var expectedType = typeof (TheType);
      ITypeInformation typeInformation = TypeAdapter.Create (expectedType);

      Assert.That (typeInformation.ConvertToRuntimeType(), Is.SameAs (expectedType));
    }

    [Test]
    public void ConvertToRuntimeType_WithOtherITypeInformation_ThrowsInvalidOperationException ()
    {
      var typeInformation = new Mock<ITypeInformation>();
      typeInformation.Setup (_ => _.Name).Returns ("TheName");

      Assert.That (
          () => typeInformation.Object.ConvertToRuntimeType(),
          Throws.InvalidOperationException.And.Message.EqualTo (
              string.Format (
                  "The type 'TheName' cannot be converted to a runtime type because no conversion is registered for '{0}'.",
                  typeInformation.Object.GetType().FullName)));
    }

    [Test]
    public void AsRuntimeProperty_WithPropertyAdapter_ReturnsRuntimeType ()
    {
      var expectedProperty = MemberInfoFromExpressionUtility.GetProperty ((TheType t) => t.TheProperty);
      IPropertyInformation propertyInformation = PropertyInfoAdapter.Create (expectedProperty);

      Assert.That (propertyInformation.AsRuntimePropertyInfo(), Is.SameAs (expectedProperty));
    }

    [Test]
    public void AsRuntimeProperty_WithOtherIPropertyInformation_ReturnsNull ()
    {
      var propertyInformation = new Mock<IPropertyInformation>();

      Assert.That (propertyInformation.Object.AsRuntimePropertyInfo(), Is.Null);
    }

    [Test]
    public void ConvertToRuntimeProperty_WithPropertyAdapter_ReturnsRuntimeType ()
    {
      var expectedProperty = MemberInfoFromExpressionUtility.GetProperty ((TheType t) => t.TheProperty);
      IPropertyInformation propertyInformation = PropertyInfoAdapter.Create (expectedProperty);

      Assert.That (propertyInformation.ConvertToRuntimePropertyInfo(), Is.SameAs (expectedProperty));
    }

    [Test]
    public void ConvertToRuntimeProperty_WithOtherIPropertyInformation_ThrowsInvalidOperationException ()
    {
      var propertyInformation = new Mock<IPropertyInformation>();
      propertyInformation.Setup (_ => _.Name).Returns ("TheName");

      Assert.That (
          () => propertyInformation.Object.ConvertToRuntimePropertyInfo(),
          Throws.InvalidOperationException.And.Message.EqualTo (
              string.Format (
                  "The property 'TheName' cannot be converted to a runtime property because no conversion is registered for '{0}'.",
                  propertyInformation.Object.GetType().Name)));
    }
  }
}