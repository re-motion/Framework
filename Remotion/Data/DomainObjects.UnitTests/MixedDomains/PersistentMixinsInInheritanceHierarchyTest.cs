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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.UnitTests.MixedDomains.TestDomain.ConcreteInheritance;
using Remotion.Data.DomainObjects.UnitTests.MixedDomains.TestDomain.SingleInheritance;

namespace Remotion.Data.DomainObjects.UnitTests.MixedDomains
{
  [TestFixture]
  public class PersistentMixinsInInheritanceHierarchyTest : StandardMappingTest
  {
    [Test]
    public void SingleInheritance_QueryOverView ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var firstDerivedClass = SingleInheritanceFirstDerivedClass.NewObject();
        var secondDerivedClass = SingleInheritanceSecondDerivedClass.NewObject();

        firstDerivedClass.BaseProperty = "BasePropertyValue 1";
        firstDerivedClass.FirstDerivedProperty = "FirstDerivedPropertyValue 1";
        ((ISingleInheritancePersistentMixin)firstDerivedClass).PersistentProperty = "PersistentPropertyValue 1";

        secondDerivedClass.BaseProperty = "BasePropertyValue 2";
        secondDerivedClass.SecondDerivedProperty = "SecondDerivedPropertyValue 2";
        ((ISingleInheritancePersistentMixin)secondDerivedClass).PersistentProperty = "PersistentPropertyValue 2";

        ClientTransaction.Current.Commit();
      }

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var query = new Query(new QueryDefinition(
                "QueryOverUnionView",
                TestDomainStorageProviderDefinition,
                "SELECT * FROM [SingleInheritanceBaseClassView]",
                QueryType.CollectionReadOnly),
            new QueryParameterCollection());
        var actualObjects = ClientTransaction.Current.QueryManager.GetCollection<SingleInheritanceBaseClass>(query);

        Assert.That(actualObjects.Count, Is.EqualTo(2));
        var actualFirstDerivedClass = actualObjects.AsEnumerable().OfType<SingleInheritanceFirstDerivedClass>().Single();
        var actualSecondDerivedClass = actualObjects.AsEnumerable().OfType<SingleInheritanceSecondDerivedClass>().Single();

        Assert.That(actualFirstDerivedClass.BaseProperty, Is.EqualTo("BasePropertyValue 1"));
        Assert.That(actualFirstDerivedClass.FirstDerivedProperty, Is.EqualTo("FirstDerivedPropertyValue 1"));
        Assert.That(((ISingleInheritancePersistentMixin)actualFirstDerivedClass).PersistentProperty, Is.EqualTo("PersistentPropertyValue 1"));

        Assert.That(actualSecondDerivedClass.BaseProperty, Is.EqualTo("BasePropertyValue 2"));
        Assert.That(actualSecondDerivedClass.SecondDerivedProperty, Is.EqualTo("SecondDerivedPropertyValue 2"));
        Assert.That(((ISingleInheritancePersistentMixin)actualSecondDerivedClass).PersistentProperty, Is.EqualTo("PersistentPropertyValue 2"));
      }
    }

    [Test]
    public void SingleInheritance_GetObject ()
    {
      ObjectID firstDerivedClassObjectID;
      ObjectID secondDerivedClassObjectID;
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var firstDerivedClass = SingleInheritanceFirstDerivedClass.NewObject();
        firstDerivedClassObjectID = firstDerivedClass.ID;
        var secondDerivedClass = SingleInheritanceSecondDerivedClass.NewObject();
        secondDerivedClassObjectID = secondDerivedClass.ID;

        ClientTransaction.Current.Commit();
      }

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        Assert.IsInstanceOf(typeof(SingleInheritanceFirstDerivedClass), LifetimeService.GetObject(ClientTransaction.Current, firstDerivedClassObjectID, false));
        Assert.IsInstanceOf(typeof(SingleInheritanceSecondDerivedClass), LifetimeService.GetObject(ClientTransaction.Current, secondDerivedClassObjectID, false));
      }
    }

    [Test]
    public void SingleInheritance_RelationsWorkCorrectly ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var objectWithRelations = SingleInheritanceObjectWithRelations.NewObject();
        objectWithRelations.ScalarProperty = SingleInheritanceFirstDerivedClass.NewObject();
        objectWithRelations.VectorProperty.Add(SingleInheritanceFirstDerivedClass.NewObject());
        objectWithRelations.VectorProperty.Add(SingleInheritanceSecondDerivedClass.NewObject());

        ClientTransaction.Current.Commit();
      }

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var query = new Query(new QueryDefinition("QueryOverUnionView", TestDomainStorageProviderDefinition,
                                               "SELECT * FROM [SingleInheritanceObjectWithRelationsView]", QueryType.CollectionReadOnly), new QueryParameterCollection());
        var actualObjectWithRelations = ClientTransaction.Current.QueryManager.GetCollection<SingleInheritanceObjectWithRelations>(query)
          .AsEnumerable().Single();

        Assert.IsInstanceOf(typeof(SingleInheritanceFirstDerivedClass), actualObjectWithRelations.ScalarProperty);
        Assert.That(actualObjectWithRelations.VectorProperty.Count, Is.EqualTo(2));
        Assert.That(actualObjectWithRelations.VectorProperty.OfType<SingleInheritanceFirstDerivedClass>().Single(), Is.Not.Null);
        Assert.That(actualObjectWithRelations.VectorProperty.OfType<SingleInheritanceSecondDerivedClass>().Single(), Is.Not.Null);
      }
    }

    [Test]
    public void ConcreteInheritance_QueryOverUnionView ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var firstDerivedClass = ConcreteInheritanceFirstDerivedClass.NewObject();
        var secondDerivedClass = ConcreteInheritanceSecondDerivedClass.NewObject();

        firstDerivedClass.BaseProperty = "BasePropertyValue 1";
        firstDerivedClass.FirstDerivedProperty = "FirstDerivedPropertyValue 1";
        ((IConcreteInheritancePersistentMixin)firstDerivedClass).PersistentProperty = "PersistentPropertyValue 1";

        secondDerivedClass.BaseProperty = "BasePropertyValue 2";
        secondDerivedClass.SecondDerivedProperty = "SecondDerivedPropertyValue 2";
        ((IConcreteInheritancePersistentMixin)secondDerivedClass).PersistentProperty = "PersistentPropertyValue 2";

        ClientTransaction.Current.Commit();
      }

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var query = new Query(new QueryDefinition("QueryOverUnionView", TestDomainStorageProviderDefinition,
                                                    "SELECT * FROM [ConcreteInheritanceBaseClassView]", QueryType.CollectionReadOnly), new QueryParameterCollection());
        var actualObjects = ClientTransaction.Current.QueryManager.GetCollection<ConcreteInheritanceBaseClass>(query);

        Assert.That(actualObjects.Count, Is.EqualTo(2));
        var actualFirstDerivedClass = actualObjects.AsEnumerable().OfType<ConcreteInheritanceFirstDerivedClass>().Single();
        var actualSecondDerivedClass = actualObjects.AsEnumerable().OfType<ConcreteInheritanceSecondDerivedClass>().Single();

        Assert.That(actualFirstDerivedClass.BaseProperty, Is.EqualTo("BasePropertyValue 1"));
        Assert.That(actualFirstDerivedClass.FirstDerivedProperty, Is.EqualTo("FirstDerivedPropertyValue 1"));
        Assert.That(((IConcreteInheritancePersistentMixin)actualFirstDerivedClass).PersistentProperty, Is.EqualTo("PersistentPropertyValue 1"));

        Assert.That(actualSecondDerivedClass.BaseProperty, Is.EqualTo("BasePropertyValue 2"));
        Assert.That(actualSecondDerivedClass.SecondDerivedProperty, Is.EqualTo("SecondDerivedPropertyValue 2"));
        Assert.That(((IConcreteInheritancePersistentMixin)actualSecondDerivedClass).PersistentProperty, Is.EqualTo("PersistentPropertyValue 2"));
      }
    }

    [Test]
    public void ConcreteInheritance_GetObject ()
    {
      ObjectID firstDerivedClassObjectID;
      ObjectID secondDerivedClassObjectID;
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var firstDerivedClass = ConcreteInheritanceFirstDerivedClass.NewObject();
        firstDerivedClassObjectID = firstDerivedClass.ID;
        var secondDerivedClass = ConcreteInheritanceSecondDerivedClass.NewObject();
        secondDerivedClassObjectID = secondDerivedClass.ID;

        ClientTransaction.Current.Commit();
      }

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        Assert.IsInstanceOf(typeof(ConcreteInheritanceFirstDerivedClass), LifetimeService.GetObject(ClientTransaction.Current, firstDerivedClassObjectID, false));
        Assert.IsInstanceOf(typeof(ConcreteInheritanceSecondDerivedClass), LifetimeService.GetObject(ClientTransaction.Current, secondDerivedClassObjectID, false));
      }
    }

    [Test]
    public void ConcreteInheritance_RelationsWorkCorrectly ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var objectWithRelations = ConcreteInheritanceObjectWithRelations.NewObject();
        objectWithRelations.ScalarProperty = ConcreteInheritanceFirstDerivedClass.NewObject();
        objectWithRelations.VectorProperty.Add(ConcreteInheritanceFirstDerivedClass.NewObject());
        objectWithRelations.VectorProperty.Add(ConcreteInheritanceSecondDerivedClass.NewObject());

        ClientTransaction.Current.Commit();
      }

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var query = new Query(new QueryDefinition("QueryOverUnionView", TestDomainStorageProviderDefinition,
                                               "SELECT * FROM [ConcreteInheritanceObjectWithRelationsView]", QueryType.CollectionReadOnly), new QueryParameterCollection());
        var actualObjectWithRelations = ClientTransaction.Current.QueryManager.GetCollection<ConcreteInheritanceObjectWithRelations>(query)
          .AsEnumerable().Single();

        Assert.IsInstanceOf(typeof(ConcreteInheritanceFirstDerivedClass), actualObjectWithRelations.ScalarProperty);
        Assert.That(actualObjectWithRelations.VectorProperty.Count, Is.EqualTo(2));
        Assert.That(actualObjectWithRelations.VectorProperty.OfType<ConcreteInheritanceFirstDerivedClass>().Single(), Is.Not.Null);
        Assert.That(actualObjectWithRelations.VectorProperty.OfType<ConcreteInheritanceSecondDerivedClass>().Single(), Is.Not.Null);
      }
    }

  }
}
