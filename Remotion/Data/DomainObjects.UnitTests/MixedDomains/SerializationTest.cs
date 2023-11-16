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
using Remotion.Data.DomainObjects.UnitTests.MixedDomains.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.MixedDomains
{
  [TestFixture]
  public class SerializationTest : ClientTransactionBaseTest
  {
    public override void SetUp ()
    {
      base.SetUp();
      Assert2.IgnoreIfFeatureSerializationIsDisabled();
    }

#pragma warning disable SYSLIB0050
    [Test]
    public void MixedTypesAreSerializable ()
    {
      var instance = TargetClassForMixinWithState.NewObject();
      Assert.That(((object)instance).GetType().IsSerializable, Is.True);
    }
#pragma warning restore SYSLIB0050

    [Test]
    public void MixedObjectsCanBeSerializedWithoutException ()
    {
      var instance = TargetClassForMixinWithState.NewObject();
      Serializer.Serialize(instance);
    }

    [Test]
    public void MixedObjectsCanBeDeserializedWithoutException ()
    {
      var instance = TargetClassForMixinWithState.NewObject();
      Serializer.SerializeAndDeserialize(instance);
    }

    [Test]
    public void DomainObjectStateIsRestored ()
    {
      Order order = Order.NewObject();
      order.OrderNumber = 5;
      order.OrderItems.Add(DomainObjectIDs.OrderItem4.GetObject<OrderItem>());
      Tuple<ClientTransaction, Order> deserializedObjects =
          Serializer.SerializeAndDeserialize(new Tuple<ClientTransaction, Order>(ClientTransactionScope.CurrentTransaction, order));

      using (deserializedObjects.Item1.EnterDiscardingScope())
      {
        Assert.That(deserializedObjects.Item2.OrderNumber, Is.EqualTo(5));
        Assert.That(deserializedObjects.Item2.OrderItems.Contains(DomainObjectIDs.OrderItem4), Is.True);
      }
    }

    [Test]
    public void MixinStateIsRestored ()
    {
        var instance = TargetClassForMixinWithState.NewObject();
        Mixin.Get<MixinWithState>(instance).State = "Sto stas stat stamus statis stant";
        var deserializedObjects = Serializer.SerializeAndDeserialize(Tuple.Create(ClientTransactionScope.CurrentTransaction, instance));

      Assert.That(Mixin.Get<MixinWithState>(deserializedObjects.Item2), Is.Not.SameAs(Mixin.Get<MixinWithState>(instance)));
      Assert.That(Mixin.Get<MixinWithState>(deserializedObjects.Item2).State, Is.EqualTo("Sto stas stat stamus statis stant"));
    }

    [Test]
    public void MixinConfigurationIsRestored ()
    {
      var instance = TargetClassForMixinWithState.NewObject();
      Assert.That(Mixin.Get<MixinWithState>(instance), Is.Not.Null);
      byte[] serializedData = Serializer.Serialize(instance);

      var deserializedInstance1 = (TargetClassForMixinWithState)Serializer.Deserialize(serializedData);
      Assert.That(Mixin.Get<MixinWithState>(deserializedInstance1), Is.Not.Null);

      using (MixinConfiguration.BuildNew().ForClass(typeof(TargetClassForMixinWithState)).AddMixins(typeof(NullMixin)).EnterScope())
      {
        var deserializedInstance2 = (TargetClassForMixinWithState)Serializer.Deserialize(serializedData);

        Assert.That(Mixin.Get<MixinWithState>(deserializedInstance2), Is.Not.Null);
        Assert.That(Mixin.Get<NullMixin>(deserializedInstance2), Is.Null);
      }
    }
  }
}
