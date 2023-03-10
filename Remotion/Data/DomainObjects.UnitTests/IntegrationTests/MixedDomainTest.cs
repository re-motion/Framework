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
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.MixedDomains.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests
{
  [TestFixture]
  public class MixedDomainTest : ClientTransactionBaseTest
  {
    [Test]
    public void GetSetCommitRollbackPersistentProperties ()
    {
      using (TestableClientTransaction.CreateSubTransaction().EnterNonDiscardingScope())
      {
        IMixinAddingPersistentProperties properties = TargetClassForPersistentMixin.NewObject() as IMixinAddingPersistentProperties;
        Assert.That(properties, Is.Not.Null);

        properties.ExtraPersistentProperty = 10;
        properties.PersistentProperty = 11;
        properties.NonPersistentProperty = 12;

        Assert.That(properties.ExtraPersistentProperty, Is.EqualTo(10));
        Assert.That(properties.PersistentProperty, Is.EqualTo(11));
        Assert.That(properties.NonPersistentProperty, Is.EqualTo(12));

        ClientTransaction.Current.Commit();

        Assert.That(properties.ExtraPersistentProperty, Is.EqualTo(10));
        Assert.That(properties.PersistentProperty, Is.EqualTo(11));
        Assert.That(properties.NonPersistentProperty, Is.EqualTo(12));

        properties.ExtraPersistentProperty = 13;
        properties.PersistentProperty = 14;
        properties.NonPersistentProperty = 15;

        Assert.That(properties.ExtraPersistentProperty, Is.EqualTo(13));
        Assert.That(properties.PersistentProperty, Is.EqualTo(14));
        Assert.That(properties.NonPersistentProperty, Is.EqualTo(15));

        ClientTransaction.Current.Rollback();

        Assert.That(properties.ExtraPersistentProperty, Is.EqualTo(10));
        Assert.That(properties.PersistentProperty, Is.EqualTo(11));
        Assert.That(properties.NonPersistentProperty, Is.EqualTo(15));
      }
    }

    [Test]
    public void LoadStoreMixedDomainObject ()
    {
      ClientTransaction clientTransaction = ClientTransaction.CreateRootTransaction();

      var mixedInstance = clientTransaction.ExecuteInScope(() => TargetClassForPersistentMixin.NewObject());
      var mixin = Mixin.Get<MixinAddingPersistentProperties>(mixedInstance);
      var relationTarget1 = clientTransaction.ExecuteInScope(() => RelationTargetForPersistentMixin.NewObject());
      var relationTarget2 = clientTransaction.ExecuteInScope(() => RelationTargetForPersistentMixin.NewObject());
      var relationTarget3 = clientTransaction.ExecuteInScope(() => RelationTargetForPersistentMixin.NewObject());
      var relationTarget4 = clientTransaction.ExecuteInScope(() => RelationTargetForPersistentMixin.NewObject());
      var relationTarget5 = clientTransaction.ExecuteInScope(() => RelationTargetForPersistentMixin.NewObject());

      mixin.PersistentProperty = 10;
      mixin.NonPersistentProperty = 100;
      mixin.ExtraPersistentProperty = 1000;
      mixin.RelationProperty = relationTarget1;
      mixin.VirtualRelationProperty = relationTarget2;
      mixin.CollectionProperty1Side.Add(relationTarget3);
      mixin.CollectionPropertyNSide = relationTarget4;
      mixin.UnidirectionalRelationProperty = relationTarget5;

      mixedInstance.RootTransaction.Commit();

      var otherClientTransaction = ClientTransaction.CreateRootTransaction();

      var loadedInstance = mixedInstance.ID.GetObject<TargetClassForPersistentMixin>(otherClientTransaction);
      var loadedMixin = Mixin.Get<MixinAddingPersistentProperties>(loadedInstance);

      Assert.That(loadedInstance, Is.Not.SameAs(mixedInstance));
      Assert.That(loadedMixin, Is.Not.SameAs(mixin));

      Assert.That(loadedMixin.PersistentProperty, Is.EqualTo(10));
      Assert.That(loadedMixin.NonPersistentProperty, Is.EqualTo(0));
      Assert.That(loadedMixin.ExtraPersistentProperty, Is.EqualTo(1000));
      Assert.That(loadedMixin.RelationProperty.ID, Is.EqualTo(mixin.RelationProperty.ID));
      Assert.That(loadedMixin.VirtualRelationProperty.ID, Is.EqualTo(mixin.VirtualRelationProperty.ID));
      Assert.That(loadedMixin.CollectionProperty1Side.Count, Is.EqualTo(mixin.CollectionProperty1Side.Count));
      Assert.That(loadedMixin.CollectionProperty1Side[0].ID, Is.EqualTo(mixin.CollectionProperty1Side[0].ID));
      Assert.That(loadedMixin.CollectionPropertyNSide.ID, Is.EqualTo(mixin.CollectionPropertyNSide.ID));
      Assert.That(loadedMixin.UnidirectionalRelationProperty.ID, Is.EqualTo(mixin.UnidirectionalRelationProperty.ID));
    }

    [Test]
    public void OneDomainClassTwoMixins ()
    {
      var clientTransaction = ClientTransaction.CreateRootTransaction();

      var mixedInstance = clientTransaction.ExecuteInScope(() => TargetClassWithTwoUnidirectionalMixins.NewObject());
      var mixin1 = Mixin.Get<MixinAddingUnidirectionalRelation1>(mixedInstance);
      var mixin2 = Mixin.Get<MixinAddingUnidirectionalRelation2>(mixedInstance);

      var relationTarget1 = clientTransaction.ExecuteInScope(() => Computer.NewObject());
      relationTarget1.SerialNumber = "12345";
      var relationTarget2 = clientTransaction.ExecuteInScope(() => Computer.NewObject());
      relationTarget2.SerialNumber = "09876";

      mixin1.Computer = relationTarget1;
      mixin2.Computer = relationTarget2;
      mixedInstance.RootTransaction.Commit();

      var otherClientTransaction = ClientTransaction.CreateRootTransaction();

      TargetClassWithTwoUnidirectionalMixins loadedInstance = mixedInstance.GetHandle().GetObject(otherClientTransaction);
      var loadedMixin1 = Mixin.Get<MixinAddingUnidirectionalRelation1>(loadedInstance);
      var loadedMixin2 = Mixin.Get<MixinAddingUnidirectionalRelation2>(loadedInstance);

      Assert.That(loadedMixin1.Computer.ID, Is.EqualTo(mixin1.Computer.ID));
      Assert.That(loadedMixin2.Computer.ID, Is.EqualTo(mixin2.Computer.ID));
    }

    [Test]
    public void OneMixinTwoDomainClasses ()
    {
      var clientTransaction = ClientTransaction.CreateRootTransaction();

      TargetClassWithUnidirectionalMixin1 mixedInstance1 = clientTransaction.ExecuteInScope(() => TargetClassWithUnidirectionalMixin1.NewObject());
      MixinAddingUnidirectionalRelation1 mixin1 = Mixin.Get<MixinAddingUnidirectionalRelation1>(mixedInstance1);
      TargetClassWithUnidirectionalMixin2 mixedInstance2 = clientTransaction.ExecuteInScope(() => TargetClassWithUnidirectionalMixin2.NewObject());
      MixinAddingUnidirectionalRelation1 mixin2 = Mixin.Get<MixinAddingUnidirectionalRelation1>(mixedInstance2);

      Computer relationTarget1 = clientTransaction.ExecuteInScope(() => Computer.NewObject());
      relationTarget1.SerialNumber = "12345";
      var relationTarget2 = clientTransaction.ExecuteInScope(() => Computer.NewObject());
      relationTarget2.SerialNumber = "09876";

      mixin1.Computer = relationTarget1;
      mixin2.Computer = relationTarget2;
      mixedInstance1.RootTransaction.Commit();

      var otherClientTransaction = ClientTransaction.CreateRootTransaction();

      TargetClassWithUnidirectionalMixin1 loadedInstance1 = mixedInstance1.GetHandle().GetObject(otherClientTransaction);
      MixinAddingUnidirectionalRelation1 loadedMixin1 = Mixin.Get<MixinAddingUnidirectionalRelation1>(loadedInstance1);
      TargetClassWithUnidirectionalMixin2 loadedInstance2 = mixedInstance2.GetHandle().GetObject(otherClientTransaction);
      MixinAddingUnidirectionalRelation1 loadedMixin2 = Mixin.Get<MixinAddingUnidirectionalRelation1>(loadedInstance2);

      Assert.That(loadedMixin1.Computer.ID, Is.EqualTo(mixin1.Computer.ID));
      Assert.That(loadedMixin2.Computer.ID, Is.EqualTo(mixin2.Computer.ID));
    }
  }
}
