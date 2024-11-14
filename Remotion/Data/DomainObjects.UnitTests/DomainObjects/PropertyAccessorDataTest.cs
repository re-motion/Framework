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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class PropertyAccessorDataTest : StandardMappingTest
  {
    [Test]
    public void ConstructorThrows_OnWrongIdentifier ()
    {
      Assert.That(
          () => CreateAccessorData(typeof(IndustrialSector), "FooBarFredBaz"),
          Throws.ArgumentException
              .With.Message.Contains("does not have a mapping property named"));
    }

    [Test]
    public void GetPropertyObjects_StringProperty ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(IndustrialSector));
      var propertyIdentifier = GetPropertyIdentifier(classDefinition, "Name");

      var propertyObjects = PropertyAccessorData.GetPropertyDefinitionObjects(classDefinition, propertyIdentifier);
      Assert.That(propertyObjects.Item1, Is.Not.Null);
      Assert.That(propertyObjects.Item1.ClassDefinition, Is.SameAs(classDefinition));
      Assert.That(propertyObjects.Item1.PropertyName, Is.EqualTo(propertyIdentifier));
      Assert.That(propertyObjects.Item2, Is.Null);
    }

    [Test]
    public void GetPropertyObjects_CollectionProperty ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(IndustrialSector));
      var propertyIdentifier = GetPropertyIdentifier(classDefinition, "Companies");

      var propertyObjects = PropertyAccessorData.GetPropertyDefinitionObjects(classDefinition, propertyIdentifier);
      Assert.That(propertyObjects.Item2, Is.Not.Null);
      Assert.That(propertyObjects.Item2.ClassDefinition, Is.SameAs(classDefinition));
      Assert.That(propertyObjects.Item2.PropertyName, Is.EqualTo(propertyIdentifier));
    }

    [Test]
    public void GetPropertyObjects_CollectionProperty_BackReference ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(Company));
      var propertyIdentifier = GetPropertyIdentifier(classDefinition, "IndustrialSector");

      var propertyObjects = PropertyAccessorData.GetPropertyDefinitionObjects(classDefinition, propertyIdentifier);
      Assert.That(propertyObjects.Item2, Is.Not.Null);
      Assert.That(propertyObjects.Item2.ClassDefinition, Is.SameAs(classDefinition));
      Assert.That(propertyObjects.Item2.PropertyName, Is.EqualTo(propertyIdentifier));
    }

    [Test]
    public void GetPropertyObjects_OneOne_VirtualSide ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(Employee));
      var propertyIdentifier = GetPropertyIdentifier(classDefinition, "Computer");

      var propertyObjects = PropertyAccessorData.GetPropertyDefinitionObjects(classDefinition, propertyIdentifier);
      Assert.That(propertyObjects.Item2, Is.Not.Null);
      Assert.That(propertyObjects.Item2.ClassDefinition, Is.SameAs(classDefinition));
      Assert.That(propertyObjects.Item2.PropertyName, Is.EqualTo(propertyIdentifier));
    }

    [Test]
    public void GetPropertyObjects_OneOne_RealSide ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(Computer));
      var propertyIdentifier = GetPropertyIdentifier(classDefinition, "Employee");

      var propertyObjects = PropertyAccessorData.GetPropertyDefinitionObjects(classDefinition, propertyIdentifier);
      Assert.That(propertyObjects.Item2, Is.Not.Null);
      Assert.That(propertyObjects.Item2.ClassDefinition, Is.SameAs(classDefinition));
      Assert.That(propertyObjects.Item2.PropertyName, Is.EqualTo(propertyIdentifier));
    }

    [Test]
    public void GetPropertyObjects_ThrowsOnInvalidPropertyID1 ()
    {
      Assert.That(
          () => PropertyAccessorData.GetPropertyDefinitionObjects(
          MappingConfiguration.Current.GetTypeDefinition(typeof(IndustrialSector)),
          "Bla"),
          Throws.ArgumentException
              .With.Message.Contains("does not have a mapping property named"));
    }

    [Test]
    public void GetPropertyObjects_ThrowsOnInvalidPropertyID2 ()
    {
      Assert.That(
          () => PropertyAccessorData.GetPropertyDefinitionObjects(
          MappingConfiguration.Current.GetTypeDefinition(typeof(Company)),
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies"),
          Throws.ArgumentException
              .With.Message.Contains("does not have a mapping property named"));
    }

    [Test]
    public void InstancePropertyObjects_CollectionProperty ()
    {
      PropertyAccessorData accessor = CreateAccessorData(typeof(IndustrialSector), "Companies");

      Assert.That(accessor.ClassDefinition, Is.SameAs(MappingConfiguration.Current.GetTypeDefinition(typeof(IndustrialSector))));
      Assert.That(accessor.PropertyIdentifier, Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies"));
      Assert.That(accessor.PropertyDefinition, Is.Null);
      Assert.That(accessor.RelationEndPointDefinition, Is.Not.Null);
      Assert.That(
          accessor.RelationEndPointDefinition,
          Is.SameAs(
              MappingConfiguration.Current.GetTypeDefinition(typeof(IndustrialSector))
                  .GetRelationEndPointDefinition("Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies")));
    }

    [Test]
    public void InstancePropertyObjects_StringProperty ()
    {
      PropertyAccessorData accessor = CreateAccessorData(typeof(IndustrialSector), "Name");

      Assert.That(accessor.PropertyDefinition, Is.Not.Null);
      Assert.That(
          accessor.PropertyDefinition,
          Is.SameAs(
              MappingConfiguration.Current.GetTypeDefinition(typeof(IndustrialSector))
                  .GetPropertyDefinition("Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name")));
      Assert.That(accessor.RelationEndPointDefinition, Is.Null);
    }

    [Test]
    public void InstancePropertyObjects_OneOneProperty_RealSide ()
    {
      PropertyAccessorData accessor = CreateAccessorData(typeof(Computer), "Employee");

      Assert.That(accessor.PropertyDefinition, Is.Not.Null);
      Assert.That(
          accessor.PropertyDefinition,
          Is.SameAs(
              MappingConfiguration.Current.GetTypeDefinition(typeof(Computer))
                  .GetPropertyDefinition("Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee")));

      Assert.That(accessor.RelationEndPointDefinition, Is.Not.Null);
      Assert.That(
          accessor.RelationEndPointDefinition,
          Is.SameAs(
              MappingConfiguration.Current.GetTypeDefinition(typeof(Computer))
                  .GetRelationEndPointDefinition("Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee")));
    }

    [Test]
    public void GetPropertyKind ()
    {
      Assert.That(
          PropertyAccessorData.GetPropertyKind(
              MappingConfiguration.Current.GetTypeDefinition(typeof(IndustrialSector)),
              "Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name"),
          Is.EqualTo(PropertyKind.PropertyValue),
          "Property value type");

      Assert.That(
          PropertyAccessorData.GetPropertyKind(
              MappingConfiguration.Current.GetTypeDefinition(typeof(IndustrialSector)),
              "Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies"),
          Is.EqualTo(PropertyKind.RelatedObjectCollection),
          "Related object collection type - bidirectional relation 1:n, 1 side");

      Assert.That(
          PropertyAccessorData.GetPropertyKind(
              MappingConfiguration.Current.GetTypeDefinition(typeof(Company)),
              "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.IndustrialSector"),
          Is.EqualTo(PropertyKind.RelatedObject),
          "Related object type - bidirectional relation 1:n, n side");

      Assert.That(
          PropertyAccessorData.GetPropertyKind(
              MappingConfiguration.Current.GetTypeDefinition(typeof(Employee)),
              "Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Computer"),
          Is.EqualTo(PropertyKind.RelatedObject),
          "Related object type - bidirectional relation 1:1, referenced side");

      Assert.That(
          PropertyAccessorData.GetPropertyKind(
              MappingConfiguration.Current.GetTypeDefinition(typeof(Computer)),
              "Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee"),
          Is.EqualTo(PropertyKind.RelatedObject),
          "Related object type - bidirectional relation 1:1, foreign key side");

      Assert.That(
          PropertyAccessorData.GetPropertyKind(
              MappingConfiguration.Current.GetTypeDefinition(typeof(Client)),
              "Remotion.Data.DomainObjects.UnitTests.TestDomain.Client.ParentClient"),
          Is.EqualTo(PropertyKind.RelatedObject),
          "Related object type - unidirectional relation 1:n, 1 side");
    }

    [Test]
    public void Kind ()
    {
      Assert.That(CreateAccessorData(typeof(IndustrialSector), "Name").Kind, Is.EqualTo(PropertyKind.PropertyValue), "Property value type");

      Assert.That(
          CreateAccessorData(typeof(IndustrialSector), "Companies").Kind,
          Is.EqualTo(PropertyKind.RelatedObjectCollection),
          "Related object collection type - bidirectional relation 1:n, 1 side");

      Assert.That(CreateAccessorData(typeof(Company), "IndustrialSector").Kind, Is.EqualTo(PropertyKind.RelatedObject), "Related object type - bidirectional relation 1:n, n side");

      Assert.That(
          CreateAccessorData(typeof(Employee), "Computer").Kind,
          Is.EqualTo(PropertyKind.RelatedObject),
          "Related object type - bidirectional relation 1:1, referenced side");

      Assert.That(
          CreateAccessorData(typeof(Computer), "Employee").Kind,
          Is.EqualTo(PropertyKind.RelatedObject),
          "Related object type - bidirectional relation 1:1, foreign key side");

      Assert.That(CreateAccessorData(typeof(Client), "ParentClient").Kind, Is.EqualTo(PropertyKind.RelatedObject), "Related object type - unidirectional relation 1:n, 1 side");
    }

    [Test]
    public void GetPropertyType ()
    {
      Assert.That(
          PropertyAccessorData.GetPropertyType(
              MappingConfiguration.Current.GetTypeDefinition(typeof(IndustrialSector)),
              "Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name"),
          Is.EqualTo(typeof(string)),
          "Property value type");

      Assert.That(
          PropertyAccessorData.GetPropertyType(
              MappingConfiguration.Current.GetTypeDefinition(typeof(IndustrialSector)),
              "Remotion.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies"),
          Is.EqualTo(typeof(ObjectList<Company>)),
          "Related object collection type - bidirectional relation 1:n, 1 side");

      Assert.That(
          PropertyAccessorData.GetPropertyType(
              MappingConfiguration.Current.GetTypeDefinition(typeof(Company)),
              "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.IndustrialSector"),
          Is.EqualTo(typeof(IndustrialSector)),
          "Related object type - bidirectional relation 1:n, n side");

      Assert.That(
          PropertyAccessorData.GetPropertyType(
              MappingConfiguration.Current.GetTypeDefinition(typeof(Employee)),
              "Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Computer"),
          Is.EqualTo(typeof(Computer)),
          "Related object type - bidirectional relation 1:1, referenced side");

      Assert.That(
          PropertyAccessorData.GetPropertyType(
              MappingConfiguration.Current.GetTypeDefinition(typeof(Computer)),
              "Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee"),
          Is.EqualTo(typeof(Employee)),
          "Related object type - bidirectional relation 1:1, foreign key side");

      Assert.That(
          PropertyAccessorData.GetPropertyType(
              MappingConfiguration.Current.GetTypeDefinition(typeof(Client)),
              "Remotion.Data.DomainObjects.UnitTests.TestDomain.Client.ParentClient"),
          Is.EqualTo(typeof(Client)),
          "Related object type - unidirectional relation 1:n, 1 side");
    }

    [Test]
    public void PropertyType ()
    {
      Assert.That(
          CreateAccessorData(typeof(IndustrialSector), "Name").PropertyType,
          Is.EqualTo(typeof(string)),
          "Property value type");

      Assert.That(
          CreateAccessorData(typeof(IndustrialSector), "Companies").PropertyType,
          Is.EqualTo(typeof(ObjectList<Company>)),
          "Related object collection type - bidirectional relation 1:n, 1 side");

      Assert.That(
          CreateAccessorData(typeof(Company), "IndustrialSector").PropertyType,
          Is.EqualTo(typeof(IndustrialSector)),
          "Related object type - bidirectional relation 1:n, n side");

      Assert.That(
          CreateAccessorData(typeof(Employee), "Computer").PropertyType,
          Is.EqualTo(typeof(Computer)),
          "Related object type - bidirectional relation 1:1, referenced side");

      Assert.That(
          CreateAccessorData(typeof(Computer), "Employee").PropertyType,
          Is.EqualTo(typeof(Employee)),
          "Related object type - bidirectional relation 1:1, foreign key side");

      Assert.That(
          CreateAccessorData(typeof(Client), "ParentClient").PropertyType,
          Is.EqualTo(typeof(Client)),
          "Related object type - unidirectional relation 1:n, 1 side");
    }

    [Test]
    public void Equals_True ()
    {
      var data1 = CreateAccessorData(typeof(IndustrialSector), "Name");
      var data2 = CreateAccessorData(typeof(IndustrialSector), "Name");

      Assert.That(data1, Is.EqualTo(data2));
    }

    [Test]
    public void Equals_False_ClassDefinition ()
    {
      var data1 = CreateAccessorData(typeof(IndustrialSector), "Name");
      var data2 = CreateAccessorData(typeof(Company), "Name");

      Assert.That(data1, Is.Not.EqualTo(data2));
    }

    [Test]
    public void Equals_False_PropertyIfentifier ()
    {
      var data1 = CreateAccessorData(typeof(IndustrialSector), "Name");
      var data2 = CreateAccessorData(typeof(IndustrialSector), "Companies");

      Assert.That(data1, Is.Not.EqualTo(data2));
    }

    [Test]
    public void GetHashCode_Equal ()
    {
      var data1 = CreateAccessorData(typeof(IndustrialSector), "Name");
      var data2 = CreateAccessorData(typeof(IndustrialSector), "Name");

      Assert.That(data1.GetHashCode(), Is.EqualTo(data2.GetHashCode()));
    }

    private static string GetPropertyIdentifier (ClassDefinition classDefinition, string shortPropertyName)
    {
      return classDefinition.ClassType.FullName + "." + shortPropertyName;
    }

    private PropertyAccessorData CreateAccessorData (Type type, string shortIdentifier)
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition(type);
      return new PropertyAccessorData(classDefinition, GetPropertyIdentifier(classDefinition, shortIdentifier));
    }
  }
}
