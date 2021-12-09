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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.MixedDomains.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class RelationEndPointIDTest : StandardMappingTest
  {
    private ObjectID _objectID;
    private string _propertyName;
    private RelationEndPointID _endPointID;
    private RelationEndPointID _nullEndPointID;
    private IRelationEndPointDefinition _endPointDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _objectID = DomainObjectIDs.Order1;
      _propertyName = "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket";
      _endPointDefinition = _objectID.ClassDefinition.GetMandatoryRelationEndPointDefinition(_propertyName);

      _endPointID = RelationEndPointID.Create(_objectID, _propertyName);
      _nullEndPointID = RelationEndPointID.Create(null, _endPointID.Definition);
    }

    [Test]
    public void Create_WithDefinition ()
    {
      var endPointID = RelationEndPointID.Create(_objectID, _endPointDefinition);

      Assert.That(endPointID.Definition, Is.EqualTo(_endPointDefinition));
      Assert.That(endPointID.ObjectID, Is.EqualTo(_objectID));
    }

    [Test]
    public void Create_WithDefinition_NullObjectID ()
    {
      var endPointID = RelationEndPointID.Create(null, _endPointDefinition);

      Assert.That(endPointID.Definition, Is.EqualTo(_endPointDefinition));
      Assert.That(endPointID.ObjectID, Is.Null);
    }

    [Test]
    public void Create_WithPropertyIdentifier ()
    {
      var endPointID = RelationEndPointID.Create(_objectID, _propertyName);
      Assert.That(endPointID.Definition, Is.EqualTo(_endPointDefinition));
      Assert.That(endPointID.ObjectID, Is.EqualTo(_objectID));
    }

    [Test]
    public void Create_WithPropertyIdentifier_NullObjectID ()
    {
      Assert.That(
          () => RelationEndPointID.Create(null, _propertyName),
          Throws.InstanceOf<ArgumentNullException>());
    }

    [Test]
    public void Create_WithPropertyIdentifier_NullPropertyName ()
    {
      Assert.That(
          () => RelationEndPointID.Create(_objectID, (string)null),
          Throws.InstanceOf<ArgumentNullException>());
    }

    [Test]
    public void Create_WithPropertyIdentifier_InvalidPropertyName ()
    {
      Assert.That(
          () => RelationEndPointID.Create(DomainObjectIDs.Order1, "PropertyName"),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("No relation found for type 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order' and property 'PropertyName'.", "propertyIdentifier"));
    }

    [Test]
    public void Create_WithTypeAndPropertyName ()
    {
      var endPointID = RelationEndPointID.Create(_objectID, typeof(Order), "OrderTicket");
      Assert.That(endPointID.Definition, Is.EqualTo(_endPointDefinition));
      Assert.That(endPointID.ObjectID, Is.EqualTo(_objectID));
    }

    [Test]
    public void Create_WithTypeAndPropertyName_NonExistingProperty ()
    {
      Assert.That(
          () => RelationEndPointID.Create(_objectID, typeof(Order), "OrderJoe"),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The domain object type 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order' does not have a mapping property named "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderJoe'.", "shortPropertyName"));
    }

    [Test]
    public void Create_WithTypeAndPropertyName_NonRelationProperty ()
    {
      Assert.That(
          () => RelationEndPointID.Create(_objectID, typeof(Order), "OrderNumber"),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber' is not a relation property.",
                  "shortPropertyName"));
    }

    [Test]
    public void Resolve_Expression ()
    {
      var instance = DomainObjectMother.CreateFakeObject<Order>(_objectID);
      var endPointID = RelationEndPointID.Resolve(instance, o => o.OrderTicket);

      Assert.That(endPointID.Definition, Is.EqualTo(_endPointDefinition));
      Assert.That(endPointID.ObjectID, Is.EqualTo(_objectID));
    }

    [Test]
    public void Resolve_Expression_Mixin ()
    {
      var instance = DomainObjectMother.CreateFakeObject<TargetClassForPersistentMixin>();
// ReSharper disable SuspiciousTypeConversion.Global
      var endPointID1 = RelationEndPointID.Resolve(instance, t => ((IMixinAddingPersistentProperties)t).RelationProperty);
// ReSharper restore SuspiciousTypeConversion.Global
      var endPointID2 = RelationEndPointID.Resolve(instance, t => Mixin.Get<MixinAddingPersistentProperties>(t).RelationProperty);

      Assert.That(
          endPointID1.Definition,
          Is.Not.Null.And.EqualTo(
              instance.ID.ClassDefinition.GetRelationEndPointDefinition(
                  GetPropertyIdentifier(typeof(MixinAddingPersistentProperties), "RelationProperty"))));
      Assert.That(endPointID2, Is.EqualTo(endPointID1));
    }

    [Test]
    public void Resolve_Expression_Interface ()
    {
      var instance = DomainObjectMother.CreateFakeObject<Order>(_objectID);
      var endPointID = RelationEndPointID.Resolve(instance, o => ((IOrder)o).OrderTicket);

      Assert.That(endPointID.Definition, Is.EqualTo(_endPointDefinition));
      Assert.That(endPointID.ObjectID, Is.EqualTo(_objectID));
    }

    [Test]
    public void Resolve_Expression_NonExistingProperty ()
    {
      var instance = DomainObjectMother.CreateFakeObject<Order>(_objectID);
      Assert.That(
          () => RelationEndPointID.Resolve(instance, o => ((OrderItem)(object)o).Product),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
#if NETFRAMEWORK
                  "The domain object type 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order' does not have a mapping property identified by expression "
                  + "'o => Convert(Convert(o)).Product'.", "propertyAccessExpression"
#else
                  "The domain object type 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order' does not have a mapping property identified by expression "
                  + "'o => Convert(Convert(o, Object), OrderItem).Product'.", "propertyAccessExpression"
#endif
          ));
    }

    [Test]
    public void Resolve_Expression_NonRelationProperty ()
    {
      var instance = DomainObjectMother.CreateFakeObject<Order>(_objectID);
      Assert.That(
          () => RelationEndPointID.Resolve(instance, o => o.OrderNumber),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber' is not a relation property.",
                  "propertyAccessExpression"));
    }

    [Test]
    public void CreateOpposite ()
    {
      var sourceEndPointDefinition =
          Configuration.GetTypeDefinition(typeof(OrderItem)).GetRelationEndPointDefinition(typeof(OrderItem).FullName + ".Order");

      var endPointID = RelationEndPointID.CreateOpposite(sourceEndPointDefinition, DomainObjectIDs.Order1);

      Assert.That(endPointID, Is.EqualTo(RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderItems")));
    }

    [Test]
    public void CreateOpposite_Null ()
    {
      var sourceEndPointDefinition =
          Configuration.GetTypeDefinition(typeof(OrderItem)).GetRelationEndPointDefinition(typeof(OrderItem).FullName + ".Order");

      var endPointID = RelationEndPointID.CreateOpposite(sourceEndPointDefinition, null);

      var expected = RelationEndPointID.Create(
          null,
          Configuration.GetTypeDefinition(typeof(Order)).GetRelationEndPointDefinition(typeof(Order).FullName + ".OrderItems"));
      Assert.That(endPointID, Is.EqualTo(expected));
    }

    [Test]
    public void CreateOpposite_Unidirectional ()
    {
      var sourceEndPointDefinition =
          Configuration.GetTypeDefinition(typeof(Location)).GetRelationEndPointDefinition(typeof(Location).FullName + ".Client");

      var endPointID = RelationEndPointID.CreateOpposite(sourceEndPointDefinition, DomainObjectIDs.Client1);

      Assert.That(endPointID.Definition.IsAnonymous, Is.True);
    }

    [Test]
    public void HashCode ()
    {
      int expectedHashCode = _objectID.GetHashCode() ^ _propertyName.GetHashCode();
      Assert.That(_endPointID.GetHashCode(), Is.EqualTo(expectedHashCode));
    }

    [Test]
    public void HashCode_NullID ()
    {
      int expectedHashCode = _propertyName.GetHashCode();
      Assert.That(_nullEndPointID.GetHashCode(), Is.EqualTo(expectedHashCode));
    }

    [Test]
    public void HashCode_AnonymousEndPoint ()
    {
      var anonymousDefinition = new AnonymousRelationEndPointDefinition(DomainObjectIDs.Client1.ClassDefinition);
      var anonymousEndPointID = RelationEndPointID.Create(DomainObjectIDs.Client1, anonymousDefinition);

      int expectedHashCode = DomainObjectIDs.Client1.GetHashCode();
      Assert.That(anonymousEndPointID.GetHashCode(), Is.EqualTo(expectedHashCode));
    }

    [Test]
    public void Equals ()
    {
      var endPointID2 = RelationEndPointID.Create(_objectID, _propertyName);

      Assert.That(_endPointID.Equals(endPointID2), Is.True);
    }

    [Test]
    public void EqualsForObjectID ()
    {
      var endPointID2 = RelationEndPointID.Create(ObjectID.Parse(_objectID.ToString()), _propertyName);
      var endPointID3 = RelationEndPointID.Create(DomainObjectIDs.Order3, _propertyName);

      Assert.That(_endPointID.Equals(endPointID2), Is.True);
      Assert.That(endPointID2.Equals(_endPointID), Is.True);
      Assert.That(_endPointID.Equals(endPointID3), Is.False);
      Assert.That(endPointID3.Equals(_endPointID), Is.False);
      Assert.That(endPointID2.Equals(endPointID3), Is.False);
      Assert.That(endPointID3.Equals(endPointID2), Is.False);
    }

    [Test]
    public void EqualsWithOtherType ()
    {
      Assert.That(_endPointID.Equals(new RelationEndPointIDTest()), Is.False);
    }

    [Test]
    public void EqualsWithNull ()
    {
      Assert.That(_endPointID.Equals(null), Is.False);
    }

    [Test]
    public new void ToString ()
    {
      string expected = _objectID + "/" + _propertyName;
      Assert.That(_endPointID.ToString(), Is.EqualTo(expected));
    }

    [Test]
    public void ToString_WithNull ()
    {
      string expected = "null/" + _propertyName;
      Assert.That(_nullEndPointID.ToString(), Is.EqualTo(expected));
    }

    [Test]
    public void GetAllRelationEndPointIDs ()
    {
      var endPointIDs = RelationEndPointID.GetAllRelationEndPointIDs(DomainObjectIDs.Order1);

      var expectedIDs = new[]
        {
          RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Order1, "Customer"),
          RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Order1, "OrderTicket"),
          RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Order1, "OrderItems"),
          RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Order1, "Official"),
        };
      Assert.That(endPointIDs, Is.EquivalentTo(expectedIDs));
    }

    [Test]
    public void StaticEquals ()
    {
      var id1 = RelationEndPointID.Create(_objectID, _propertyName);
      var id2 = RelationEndPointID.Create(_objectID, _propertyName);

      Assert.That(RelationEndPointID.Equals(id1, id2), Is.True);
    }

    [Test]
    public void StaticNotEquals ()
    {
      var id1 = RelationEndPointID.Create(_objectID, _propertyName);
      var id2 = RelationEndPointID.Create(DomainObjectIDs.OrderTicket1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order");

      Assert.That(RelationEndPointID.Equals(id1, id2), Is.False);
    }

    [Test]
    public void EqualityOperatorTrue ()
    {
      var id1 = RelationEndPointID.Create(_objectID, _propertyName);
      var id2 = RelationEndPointID.Create(_objectID, _propertyName);

      Assert.That(id1 == id2, Is.True);
      Assert.That(id1 != id2, Is.False);
    }

    [Test]
    public void EqualityOperatorFalse ()
    {
      var id1 = RelationEndPointID.Create(_objectID, _propertyName);
      var id2 = RelationEndPointID.Create(DomainObjectIDs.OrderTicket1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order");

      Assert.That(id1 == id2, Is.False);
      Assert.That(id1 != id2, Is.True);
    }

    [Test]
    public void EqualityOperatorForSameObject ()
    {
      var id1 = RelationEndPointID.Create(_objectID, _propertyName);
      var id2 = id1;

      Assert.That(id1 == id2, Is.True);
      Assert.That(id1 != id2, Is.False);
    }

    [Test]
    public void EqualityOperatorWithBothNull ()
    {
      var nullID1 = (RelationEndPointID)null;
      var nullID2 = (RelationEndPointID)null;
      Assert.That(nullID1 == nullID2, Is.True);
      Assert.That(nullID1 != nullID2, Is.False);
    }

    [Test]
    public void EqualityOperatorID1Null ()
    {
      var id2 = RelationEndPointID.Create(DomainObjectIDs.OrderTicket1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order");

      Assert.That(null == id2, Is.False);
      Assert.That(null != id2, Is.True);
    }

    [Test]
    public void EqualityOperatorID2Null ()
    {
      var id1 = RelationEndPointID.Create(_objectID, _propertyName);

      Assert.That(id1 == null, Is.False);
      Assert.That(id1 != null, Is.True);
    }
  }
}
