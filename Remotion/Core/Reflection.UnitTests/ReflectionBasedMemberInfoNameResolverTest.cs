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

namespace Remotion.Reflection.UnitTests
{
  [TestFixture]
  public class ReflectionBasedMemberInfoNameResolverTest
  {
    private ReflectionBasedMemberInformationNameResolver _resolver;
    private Mock<IPropertyInformation> _propertyInformationStub;
    private Mock<ITypeInformation> _typeInformationStub;

    [SetUp]
    public void SetUp ()
    {
      _resolver = new ReflectionBasedMemberInformationNameResolver();
      _propertyInformationStub = new Mock<IPropertyInformation>();
      _typeInformationStub = new Mock<ITypeInformation>();
    }

    [Test]
    public void GetPropertyName ()
    {
      _typeInformationStub.Setup(stub => stub.FullName).Returns("Namespace.Class");

      _propertyInformationStub.Setup(stub => stub.Name).Returns("Property");
      _propertyInformationStub.Setup(stub => stub.GetOriginalDeclaringType()).Returns(_typeInformationStub.Object);

      Assert.That(_resolver.GetPropertyName(_propertyInformationStub.Object), Is.EqualTo("Namespace.Class.Property"));
    }

    [Test]
    public void GetTypeName ()
    {
      _typeInformationStub.Setup(stub => stub.FullName).Returns("Namespace.Class");

      Assert.That(_resolver.GetTypeName(_typeInformationStub.Object), Is.EqualTo("Namespace.Class"));
    }

    [Test]
    public void GetPropertyName_Twice_ReturnsSameResultFromCache ()
    {
      _typeInformationStub.Setup(stub => stub.FullName).Returns("Namespace.Class");
      _propertyInformationStub.Setup(stub => stub.GetOriginalDeclaringType()).Returns(_typeInformationStub.Object);

      _propertyInformationStub.SetupSequence(stub => stub.Name)
          .Returns("Property1")
          .Throws(new InvalidOperationException("Method is supposed to be called only once!"));
      string name1 = _resolver.GetPropertyName(_propertyInformationStub.Object);

      _propertyInformationStub.Setup(stub => stub.Name).Returns("Property2");
      Assert.That(_propertyInformationStub.Object.Name, Is.EqualTo("Property2"));
      string name2 = _resolver.GetPropertyName(_propertyInformationStub.Object);

      Assert.That(name1, Is.SameAs(name2));
      Assert.That(name1, Is.EqualTo("Namespace.Class.Property1"));
    }

    [Test]
    public void GetTypeName_Twice_ReturnsSameResultFromCache ()
    {
      _typeInformationStub.Setup(stub => stub.FullName).Returns("Namespace.Class");
      string name1 = _resolver.GetTypeName(_typeInformationStub.Object);

      _typeInformationStub.Setup(stub => stub.FullName).Returns("IgnoredNewTypeName");
      string name2 = _resolver.GetTypeName(_typeInformationStub.Object);

      Assert.That(name1, Is.SameAs(name2));
      Assert.That(name1, Is.EqualTo("Namespace.Class"));
    }

    [Test]
    public void GetPropertyAndTypeName_ForOverriddenProperty ()
    {
      var derivedTypeStub = new Mock<ITypeInformation>();
      derivedTypeStub.Setup(stub => stub.FullName).Returns("Namespace.Derived");
      _propertyInformationStub.Setup(stub => stub.DeclaringType).Returns(derivedTypeStub.Object);

      _typeInformationStub.Setup(stub => stub.FullName).Returns("Namespace.Class");
      _propertyInformationStub.Setup(stub => stub.GetOriginalDeclaringType()).Returns(_typeInformationStub.Object);
      _propertyInformationStub.Setup(stub => stub.Name).Returns("Property");
      Assert.That(_resolver.GetPropertyName(_propertyInformationStub.Object), Is.EqualTo("Namespace.Class.Property"));
    }

    [Test]
    public void GetPropertyAndTypeName_ForPropertyInClosedGenericType ()
    {
      var genericTypeDefinitionStub = new Mock<ITypeInformation>();
      genericTypeDefinitionStub.Setup(stub => stub.FullName).Returns("Namespace.OpenGeneric<>");

      _typeInformationStub.Setup(stub => stub.FullName).Returns("Namespace.ClosedGeneric");
      _typeInformationStub.Setup(stub => stub.IsGenericType).Returns(true);
      _typeInformationStub.Setup(stub => stub.IsGenericTypeDefinition).Returns(false);
      _typeInformationStub.Setup(stub => stub.GetGenericTypeDefinition()).Returns(genericTypeDefinitionStub.Object);

      _propertyInformationStub.Setup(stub => stub.Name).Returns("Property");
      _propertyInformationStub.Setup(stub => stub.GetOriginalDeclaringType()).Returns(_typeInformationStub.Object);
      Assert.That(_resolver.GetPropertyName(_propertyInformationStub.Object), Is.EqualTo("Namespace.OpenGeneric<>.Property"));
    }

    [Test]
    public void GetPropertyAndTypeName_ForPropertyInOpenGenericType ()
    {
      _typeInformationStub.Setup(stub => stub.FullName).Returns("Namespace.OpenGeneric<>");
      _typeInformationStub.Setup(stub => stub.IsGenericType).Returns(true);
      _typeInformationStub.Setup(stub => stub.IsGenericTypeDefinition).Returns(true);

      _propertyInformationStub.Setup(stub => stub.Name).Returns("Property");
      _propertyInformationStub.Setup(stub => stub.GetOriginalDeclaringType()).Returns(_typeInformationStub.Object);
      Assert.That(_resolver.GetPropertyName(_propertyInformationStub.Object), Is.EqualTo("Namespace.OpenGeneric<>.Property"));
    }
  }
}
