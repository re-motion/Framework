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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class InvalidPropertyInformationTest
  {
    private InvalidPropertyInformation _propertyInformation;

    [SetUp]
    public void SetUp ()
    {
      _propertyInformation = new InvalidPropertyInformation(TypeAdapter.Create(typeof(string)), "PropertyName", typeof(int));
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_propertyInformation.Name, Is.EqualTo("PropertyName"));
      Assert.That(_propertyInformation.DeclaringType, Is.SameAs(TypeAdapter.Create(typeof(string))));
      Assert.That(_propertyInformation.PropertyType, Is.EqualTo(typeof(int)));
      Assert.That(_propertyInformation.CanBeSetFromOutside, Is.False);
    }

    [Test]
    public void GetOriginalDeclaringType ()
    {
      Assert.That(_propertyInformation.GetOriginalDeclaringType(), Is.SameAs(TypeAdapter.Create(typeof(string))));
    }

    [Test]
    public void GetOriginalDeclaration ()
    {
      Assert.That(_propertyInformation.GetOriginalDeclaration(), Is.SameAs(_propertyInformation));
    }

    [Test]
    public void GetCustomAttribute ()
    {
      Assert.That(_propertyInformation.GetCustomAttribute<ThreadStaticAttribute>(false), Is.Null);
      Assert.That(_propertyInformation.GetCustomAttribute<ThreadStaticAttribute>(true), Is.Null);
    }

    [Test]
    public void GetCustomAttributes ()
    {
      Assert.That(_propertyInformation.GetCustomAttributes<ThreadStaticAttribute>(false), Is.Empty);
      Assert.That(_propertyInformation.GetCustomAttributes<ThreadStaticAttribute>(true), Is.Empty);
    }

    [Test]
    public void IsDefined ()
    {
      Assert.That(_propertyInformation.IsDefined<ThreadStaticAttribute>(false), Is.False);
      Assert.That(_propertyInformation.IsDefined<ThreadStaticAttribute>(true), Is.False);
    }

    [Test]
    public void GetValue ()
    {
      Assert.That(_propertyInformation.GetValue(new object(), null), Is.Null);
    }

    [Test]
    public void GetGetMethod ()
    {
      var result = _propertyInformation.GetGetMethod(false);

      Assert.That(result, Is.Null);
    }

    [Test]
    public void GetSetMethod ()
    {
      var result = _propertyInformation.GetSetMethod(false);

      Assert.That(result, Is.Null);
    }

    [Test]
    public void GetIndexParameters ()
    {
      Assert.That(_propertyInformation.GetIndexParameters().Length, Is.EqualTo(0));
    }

    [Test]
    public void GetAccessors ()
    {
      Assert.That(_propertyInformation.GetAccessors(false).Length, Is.EqualTo(0));
    }

    [Test]
    public void FindInterfaceImplementation ()
    {
      Assert.That(
          () => _propertyInformation.FindInterfaceImplementation(typeof(object)),
          Throws.InvalidOperationException
              .With.Message.EqualTo("FindInterfaceImplementation can only be called on interface properties."));
    }

    [Test]
    public void FindInterfaceDeclaration ()
    {
      Assert.That(_propertyInformation.FindInterfaceDeclarations(), Is.Empty);
    }

    [Test]
    public void TestEquals ()
    {
      var propertyInformation2 = new InvalidPropertyInformation(_propertyInformation.DeclaringType, _propertyInformation.Name, _propertyInformation.PropertyType);

      Assert.That(_propertyInformation.Equals(_propertyInformation), Is.True);
      Assert.That(_propertyInformation.Equals(propertyInformation2), Is.False);
      Assert.That(_propertyInformation.Equals(null), Is.False);
    }

    [Test]
    public void TestGetHashCode ()
    {
      Assert.That(_propertyInformation.GetHashCode(), Is.Not.EqualTo(0));
    }

    [Test]
    public void To_String ()
    {
      Assert.That(_propertyInformation.ToString(), Is.EqualTo("PropertyName (invalid property)"));
    }

    [Test]
    public void IsNull ()
    {
      Assert.That(((IPropertyInformation)_propertyInformation).IsNull, Is.False);
    }
  }
}
