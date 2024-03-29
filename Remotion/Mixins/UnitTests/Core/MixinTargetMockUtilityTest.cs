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
using Remotion.Development.UnitTesting;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core
{
  [TestFixture]
  public class MixinTargetMockUtilityTest
  {
    [Test]
    public void Mock_ThisBaseConfig ()
    {
      var thisMock = new Mock<IBaseType31>(MockBehavior.Strict);
      var baseMock = new Mock<IBaseType31>(MockBehavior.Strict);

      var mixin = new BT3Mixin1();

      MixinTargetMockUtility.MockMixinTarget(mixin, thisMock.Object, baseMock.Object);

      Assert.That(mixin.Target, Is.SameAs(thisMock.Object));
      Assert.That(mixin.Next, Is.SameAs(baseMock.Object));
    }

    [Test]
    public void Mock_ThisConfig ()
    {
      var thisMock = new Mock<IBaseType32>(MockBehavior.Strict);
      var mixin = new BT3Mixin2();

      MixinTargetMockUtility.MockMixinTarget(mixin, thisMock.Object);
      Assert.That(mixin.Target, Is.SameAs(thisMock.Object));
    }

    [Test]
    public void UninitializedMixin_This ()
    {
      var mixin = new BT3Mixin1();
      Assert.That(
          () => mixin.Target,
          Throws.InvalidOperationException
              .With.Message.EqualTo("Mixin has not been initialized yet."));
    }

    [Test]
    public void UninitializedMixin_Base ()
    {
      var mixin = new BT3Mixin1();
      Assert.That(
          () => mixin.Next,
          Throws.InvalidOperationException
              .With.Message.EqualTo("Mixin has not been initialized yet."));
    }

    [Test]
    public void CreateMixinWithMockedTarget_AbstractMixin ()
    {
      var thisMock = new ClassOverridingMixinMembers();
      var baseMock = new object();
      Assert.That(
          () => MixinTargetMockUtility.CreateMixinWithMockedTarget<MixinWithAbstractMembers, object, object>(thisMock, baseMock),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The type 'Remotion.Mixins.UnitTests.Core.TestDomain.MixinWithAbstractMembers' cannot be constructed because the assembled type is abstract."));
    }

    [Test]
    public void CreateMixinWithMockedTarget_ThisBase ()
    {
      var thisMock = new Mock<IBaseType31>(MockBehavior.Strict);
      var baseMock = new Mock<IBaseType31>(MockBehavior.Strict);

      BT3Mixin1 mixin = MixinTargetMockUtility.CreateMixinWithMockedTarget<BT3Mixin1, IBaseType31, IBaseType31>(thisMock.Object, baseMock.Object);
      Assert.That(mixin.Target, Is.SameAs(thisMock.Object));
      Assert.That(mixin.Next, Is.SameAs(baseMock.Object));
    }

    [Test]
    public void CreateMixinWithMockedTarget_This ()
    {
      var thisMock = new Mock<IBaseType32>(MockBehavior.Strict);

      BT3Mixin2 mixin =
          MixinTargetMockUtility.CreateMixinWithMockedTarget<BT3Mixin2, IBaseType32>(thisMock.Object);
      Assert.That(mixin.Target, Is.SameAs(thisMock.Object));
    }

    [Test]
    public void CreateMixinWithMockedTarget_NonPublicCtor ()
    {
      var thisMock = new Mock<IBaseType32>(MockBehavior.Strict);

      BT3Mixin2 mixin = MixinTargetMockUtility.CreateMixinWithMockedTarget<BT3Mixin2, IBaseType32>(thisMock.Object, 7);
      Assert.That(mixin.Target, Is.SameAs(thisMock.Object));
      Assert.That(mixin.I, Is.EqualTo(7));
    }

    [Test]
    public void SignalOnDeserialized_This ()
    {
      var thisMock = new SerializableBaseType32Mock();

      BT3Mixin2 mixin = MixinTargetMockUtility.CreateMixinWithMockedTarget<BT3Mixin2, IBaseType32>(thisMock);
      var deserializedData = Serializer.SerializeAndDeserialize(Tuple.Create(thisMock, mixin));

      MixinTargetMockUtility.MockMixinTargetAfterDeserialization(deserializedData.Item2, deserializedData.Item1);
      Assert.That(deserializedData.Item2.Target, Is.Not.Null);
      Assert.That(deserializedData.Item2.Target, Is.SameAs(deserializedData.Item1));
    }

    [Test]
    public void SignalOnDeserialized_ThisBase ()
    {
      var thisMock = new SerializableBaseType31Mock();
      var baseMock = new SerializableBaseType31Mock();

      BT3Mixin1 mixin = MixinTargetMockUtility.CreateMixinWithMockedTarget<BT3Mixin1, IBaseType31, IBaseType31>(thisMock, baseMock);
      var deserializedData = Serializer.SerializeAndDeserialize(Tuple.Create(thisMock, baseMock, mixin));

      MixinTargetMockUtility.MockMixinTargetAfterDeserialization(deserializedData.Item3, deserializedData.Item1, deserializedData.Item2);
      Assert.That(deserializedData.Item3.Target, Is.Not.Null);
      Assert.That(deserializedData.Item3.Target, Is.SameAs(deserializedData.Item1));
      Assert.That(deserializedData.Item3.Next, Is.Not.Null);
      Assert.That(deserializedData.Item3.Next, Is.SameAs(deserializedData.Item2));
    }
  }
}
