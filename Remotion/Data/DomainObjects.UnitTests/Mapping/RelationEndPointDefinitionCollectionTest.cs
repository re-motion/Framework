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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class RelationEndPointDefinitionCollectionTest : MappingReflectionTestBase
  {
    private RelationEndPointDefinitionCollection _collection;
    private TypeDefinition _typeDefinition;
    private RelationEndPointDefinition _endPoint1;
    private RelationEndPointDefinition _endPoint2;
    private AnonymousRelationEndPointDefinition _anonymousEndPoint;

    public override void SetUp ()
    {
      base.SetUp();

      _typeDefinition = TypeDefinitionObjectMother.CreateClassDefinition();
      var propertyDefinition1 = PropertyDefinitionObjectMother.CreateForFakePropertyInfo_ObjectID(_typeDefinition, "Property1");
      var propertyDefinition2 = PropertyDefinitionObjectMother.CreateForFakePropertyInfo_ObjectID(_typeDefinition, "Property2");
      var propertyDefinition3 = PropertyDefinitionObjectMother.CreateForFakePropertyInfo_ObjectID(_typeDefinition, "Property3");
      var propertyDefinition4 = PropertyDefinitionObjectMother.CreateForFakePropertyInfo_ObjectID(_typeDefinition, "Property4");
      _typeDefinition.SetPropertyDefinitions(
          new PropertyDefinitionCollection(new[] { propertyDefinition1, propertyDefinition2, propertyDefinition3, propertyDefinition4 }, true));
      _endPoint1 = new RelationEndPointDefinition(propertyDefinition1, false);
      _endPoint2 = new RelationEndPointDefinition(propertyDefinition2, false);
      _anonymousEndPoint = new AnonymousRelationEndPointDefinition(_typeDefinition);
      _collection = new RelationEndPointDefinitionCollection();
    }

    [Test]
    public void CreateForAllRelationEndPoints_ClassDefinitionWithoutBaseClassDefinition_MakeCollectionReadOnlyIsFalse ()
    {
      _typeDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(new[] { _endPoint1, _endPoint2 }, true));

      var endPoints = RelationEndPointDefinitionCollection.CreateForAllRelationEndPoints(_typeDefinition, false);

      Assert.That(endPoints.Count, Is.EqualTo(2));
      Assert.That(endPoints.IsReadOnly, Is.False);
      Assert.That(endPoints[0], Is.SameAs(_endPoint1));
      Assert.That(endPoints[1], Is.SameAs(_endPoint2));
    }

    [Test]
    public void CreateForAllRelationEndPoints_ClassDefinitionWithoutBaseClassDefinition_MakeCollectionReadOnlyIsTrue ()
    {
      _typeDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(new[] { _endPoint1, _endPoint2 }, false));

      var endPoints = RelationEndPointDefinitionCollection.CreateForAllRelationEndPoints(_typeDefinition, true);

      Assert.That(endPoints.Count, Is.EqualTo(2));
      Assert.That(endPoints.IsReadOnly, Is.True);
      Assert.That(endPoints[0], Is.SameAs(_endPoint1));
      Assert.That(endPoints[1], Is.SameAs(_endPoint2));
    }

    [Test]
    public void CreateForAllRelationEndPoints_ClassDefinitionWithBaseClassDefinition ()
    {
      var baseClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(Company));
      var basedPropertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo_ObjectID(baseClassDefinition, "Property1");
      baseClassDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { basedPropertyDefinition }, true));
      var derivedClassDefinition = TypeDefinitionObjectMother.CreateClassDefinition(classType: typeof(Partner), baseClass: baseClassDefinition);
      var derivedPropertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo_ObjectID(derivedClassDefinition, "Property2");
      derivedClassDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { derivedPropertyDefinition }, true));

      var endPoint1 = new RelationEndPointDefinition(basedPropertyDefinition, false);
      var endPoint2 = new RelationEndPointDefinition(derivedPropertyDefinition, false);

      baseClassDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(new[] { endPoint1 }, true));
      derivedClassDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(new[] { endPoint2 }, true));

      var endPoints = RelationEndPointDefinitionCollection.CreateForAllRelationEndPoints(derivedClassDefinition, true);

      Assert.That(endPoints.Count, Is.EqualTo(2));
      Assert.That(endPoints[0], Is.SameAs(endPoint2));
      Assert.That(endPoints[1], Is.SameAs(endPoint1));
    }

    [Test]
    public void CreateForAllRelationEndPoints_ClassDefinitionWithImplementedInterfaces ()
    {
      var iPerson = InterfaceDefinitionObjectMother.CreateInterfaceDefinition();
      var iCustomer = InterfaceDefinitionObjectMother.CreateInterfaceDefinition(extendedInterfaces: new[] { iPerson });
      var customer = ClassDefinitionObjectMother.CreateClassDefinition(implementedInterfaces: new[] { iCustomer });

      var propertyDefinitionInIPerson = PropertyDefinitionObjectMother.CreateForFakePropertyInfo_ObjectID(iPerson, "A");
      var propertyDefinitionInICustomer = PropertyDefinitionObjectMother.CreateForFakePropertyInfo_ObjectID(iCustomer, "B");
      var propertyDefinitionInCustomer = PropertyDefinitionObjectMother.CreateForFakePropertyInfo_ObjectID(customer, "C");

      var endPointInIPerson = new RelationEndPointDefinition(propertyDefinitionInIPerson, false);
      var endPointInICustomer = new RelationEndPointDefinition(propertyDefinitionInICustomer, false);
      var endPointInCustomer = new RelationEndPointDefinition(propertyDefinitionInCustomer, false);

      iPerson.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(new[] { endPointInIPerson }, true));
      iCustomer.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(new[] { endPointInICustomer }, true));
      customer.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(new[] { endPointInCustomer }, true));

      var relationEndPoints = RelationEndPointDefinitionCollection.CreateForAllRelationEndPoints(customer, false);

      Assert.That(relationEndPoints.Count, Is.EqualTo(3));
      Assert.That(relationEndPoints[0], Is.SameAs(endPointInCustomer));
      Assert.That(relationEndPoints[1], Is.SameAs(endPointInICustomer));
      Assert.That(relationEndPoints[2], Is.SameAs(endPointInIPerson));
    }

    [Test]
    public void Add ()
    {
      _collection.Add(_endPoint1);
      Assert.That(_collection.Count, Is.EqualTo(1));
    }

    [Test]
    public void Add_PropertyNameIsNull ()
    {
      var endPoint = new AnonymousRelationEndPointDefinition(_typeDefinition);
      Assert.That(
          () => _collection.Add(endPoint),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo("Anonymous end points cannot be added to this collection.", "value"));
    }

    [Test]
    public void PropertyNameIndexer ()
    {
      _collection.Add(_endPoint1);
      Assert.That(_collection["Property1"], Is.SameAs(_endPoint1));
    }

    [Test]
    public void NumericIndexer ()
    {
      _collection.Add(_endPoint1);
      Assert.That(_collection[0], Is.SameAs(_endPoint1));
    }

    [Test]
    public void ContainsPropertyNameTrue ()
    {
      _collection.Add(_endPoint1);
      Assert.That(_collection.Contains("Property1"), Is.True);
    }

    [Test]
    public void ContainsPropertyNameFalse ()
    {
      Assert.That(_collection.Contains("UndefinedPropertyName"), Is.False);
    }

    [Test]
    public void ContainsRelationEndPointDefinitionTrue ()
    {
      _collection.Add(_endPoint1);
      Assert.That(_collection.Contains(_endPoint1), Is.True);
    }

    [Test]
    public void ContainsRelationEndPointDefinitionFalse ()
    {
      _collection.Add(_endPoint1);

      Assert.That(_collection.Contains(_endPoint2), Is.False);
    }

    [Test]
    public void ContainsAnonymousRelationEndPointDefinitionFalse ()
    {
      _collection.Add(_endPoint1);

      Assert.That(_collection.Contains(_anonymousEndPoint), Is.False);
    }

    [Test]
    public void CopyConstructor ()
    {
      var copiedCollection = new RelationEndPointDefinitionCollection(new[] { _endPoint1 }, false);

      Assert.That(copiedCollection.Count, Is.EqualTo(1));
      Assert.That(copiedCollection[0], Is.SameAs(_endPoint1));
    }

    [Test]
    public void ContainsNullRelationEndPointDefinitions ()
    {
      Assert.That(
          () => _collection.Contains((IRelationEndPointDefinition)null),
          Throws.InstanceOf<ArgumentNullException>());
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
      _collection.Add(_endPoint1);

      IEnumerator<IRelationEndPointDefinition> enumerator = _collection.GetEnumerator();

      Assert.That(enumerator.MoveNext(), Is.True);
      Assert.That(enumerator.Current, Is.SameAs(_endPoint1));
      Assert.That(enumerator.MoveNext(), Is.False);
    }
  }
}
