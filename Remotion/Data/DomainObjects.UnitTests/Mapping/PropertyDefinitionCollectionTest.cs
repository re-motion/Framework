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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class PropertyDefinitionCollectionTest : MappingReflectionTestBase
  {
    private PropertyDefinitionCollection _collection;
    private PropertyDefinition _propertyDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo();
      _collection = new PropertyDefinitionCollection();
    }

    [Test]
    public void CreateForAllPropertyDefinitions_ClassDefinitionWithoutBaseClassDefinition_MakeCollectionReadOnlyIsFalse ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition();
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo(classDefinition);

      classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { propertyDefinition }, true));

      var propertyDefinitions = PropertyDefinitionCollection.CreateForAllProperties(classDefinition, false);

      Assert.That(propertyDefinitions.Count, Is.EqualTo(1));
      Assert.That(propertyDefinitions.IsReadOnly, Is.False);
      Assert.That(propertyDefinitions[0], Is.SameAs(propertyDefinition));
    }

    [Test]
    public void CreateForAllPropertyDefinitions_ClassDefinitionWithoutBaseClassDefinition_MakeCollectionReadOnlyIsTrue ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition();
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo(classDefinition);

      classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { propertyDefinition }, false));

      var propertyDefinitions = PropertyDefinitionCollection.CreateForAllProperties(classDefinition, true);

      Assert.That(propertyDefinitions.Count, Is.EqualTo(1));
      Assert.That(propertyDefinitions.IsReadOnly, Is.True);
      Assert.That(propertyDefinitions[0], Is.SameAs(propertyDefinition));
    }

    [Test]
    public void CreateForAllPropertyDefinitions_ClassDefinitionWithBaseClassDefinition ()
    {
      var baseClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(Company));
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(Partner), baseClass: baseClassDefinition);

      var propertyDefinitionInBaseClass = PropertyDefinitionObjectMother.CreateForFakePropertyInfo(baseClassDefinition, "Property1");
      var propertyDefinitionInDerivedClass = PropertyDefinitionObjectMother.CreateForFakePropertyInfo(classDefinition, "Property2");

      baseClassDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { propertyDefinitionInBaseClass }, true));
      classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { propertyDefinitionInDerivedClass }, true));

      var propertyDefinitions = PropertyDefinitionCollection.CreateForAllProperties(classDefinition, false);

      Assert.That(propertyDefinitions.Count, Is.EqualTo(2));
      Assert.That(propertyDefinitions[0], Is.SameAs(propertyDefinitionInDerivedClass));
      Assert.That(propertyDefinitions[1], Is.SameAs(propertyDefinitionInBaseClass));
    }

    [Test]
    public void Add ()
    {
      _collection.Add(_propertyDefinition);
      Assert.That(_collection.Count, Is.EqualTo(1));
    }

    [Test]
    public void PropertyNameIndexer ()
    {
      _collection.Add(_propertyDefinition);
      Assert.That(_collection[_propertyDefinition.PropertyName], Is.SameAs(_propertyDefinition));
    }

    [Test]
    public void NumericIndexer ()
    {
      _collection.Add(_propertyDefinition);
      Assert.That(_collection[0], Is.SameAs(_propertyDefinition));
    }

    [Test]
    public void ContainsPropertyNameTrue ()
    {
      _collection.Add(_propertyDefinition);
      Assert.That(_collection.Contains(_propertyDefinition.PropertyName), Is.True);
    }

    [Test]
    public void ContainsPropertyNameFalse ()
    {
      Assert.That(_collection.Contains("UndefinedPropertyName"), Is.False);
    }

    [Test]
    public void ContainsPropertyDefinitionTrue ()
    {
      _collection.Add(_propertyDefinition);
      Assert.That(_collection.Contains(_propertyDefinition), Is.True);
    }

    [Test]
    public void ContainsPropertyDefinitionFalse ()
    {
      _collection.Add(_propertyDefinition);

      var copy =
          new PropertyDefinition(
              _propertyDefinition.ClassDefinition,
              _propertyDefinition.PropertyInfo,
              _propertyDefinition.PropertyName,
              _propertyDefinition.IsObjectID,
              _propertyDefinition.IsNullable,
              _propertyDefinition.MaxLength,
              _propertyDefinition.StorageClass,
              _propertyDefinition.DefaultValue);

      Assert.That(_collection.Contains(copy), Is.False);
    }

    [Test]
    public void CopyConstructor ()
    {
      var copiedCollection = new PropertyDefinitionCollection(new[] { _propertyDefinition }, false);

      Assert.That(copiedCollection.Count, Is.EqualTo(1));
      Assert.That(copiedCollection[0], Is.SameAs(_propertyDefinition));
    }

    [Test]
    public void ContainsPropertyDefinition ()
    {
      _collection.Add(_propertyDefinition);

      Assert.That(_collection.Contains(_propertyDefinition), Is.True);
    }

    [Test]
    public void ContainsNullPropertyDefinition ()
    {
      Assert.That(
          () => _collection.Contains((PropertyDefinition)null),
          Throws.InstanceOf<ArgumentNullException>());
    }

    [Test]
    public void GetAllPersistent ()
    {
      var persistentProperty1 = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("P1", StorageClass.Persistent);
      var persistentProperty2 = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("P2", StorageClass.Persistent);
      var nonPersistentProperty1 = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("P3", StorageClass.Transaction);
      var nonPersistentProperty2 = PropertyDefinitionObjectMother.CreateForFakePropertyInfo("P4", StorageClass.None);

      _collection.Add(persistentProperty1);
      _collection.Add(persistentProperty2);
      _collection.Add(nonPersistentProperty1);
      _collection.Add(nonPersistentProperty2);

      Assert.That(_collection.GetAllPersistent().ToArray(), Is.EqualTo(new[] { persistentProperty1, persistentProperty2 }));
    }

    [Test]
    public void SetReadOnly ()
    {
      Assert.That(_collection.IsReadOnly, Is.False);

      _collection.SetReadOnly();

      Assert.That(_collection.IsReadOnly, Is.True);
    }

    [Test]
    public void GetEnumerator ()
    {
      _collection.Add(_propertyDefinition);

      IEnumerator<PropertyDefinition> enumerator = _collection.GetEnumerator();

      Assert.That(enumerator.MoveNext(), Is.True);
      Assert.That(enumerator.Current, Is.SameAs(_propertyDefinition));
      Assert.That(enumerator.MoveNext(), Is.False);
    }
  }
}
