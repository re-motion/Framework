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
using System.Reflection;
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.Serialization.TestDomain;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Mixins.Utilities;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.Serialization
{
  [TestFixture]
  public class MixinSerializationTest : CodeGenerationBaseTest
  {
    [Test]
    public void SerializationOfMixinThisWorks ()
    {
      BaseType3 bt3 = CreateMixedObject<BaseType3>(typeof(BT3Mixin2), typeof(BT3Mixin2B));
      var mixin = Mixin.Get<BT3Mixin2>(bt3);
      Assert.That(mixin.Target, Is.SameAs(bt3));

      var mixin2 = Mixin.Get<BT3Mixin2B>(bt3);
      Assert.That(mixin2.Target, Is.SameAs(bt3));

      BaseType3 bt3A = Serializer.SerializeAndDeserialize(bt3);
      var mixinA = Mixin.Get<BT3Mixin2>(bt3A);
      Assert.That(mixinA, Is.Not.SameAs(mixin));
      Assert.That(mixinA.Target, Is.SameAs(bt3A));

      var mixin2A = Mixin.Get<BT3Mixin2B>(bt3A);
      Assert.That(mixin2A, Is.Not.SameAs(mixin2));
      Assert.That(mixin2A.Target, Is.SameAs(bt3A));
    }

    [Test]
    public void SerializationOfMixinBaseWorks ()
    {
      BaseType3 bt3 = CreateMixedObject<BaseType3>(typeof(BT3Mixin1), typeof(BT3Mixin1B));
      var mixin = Mixin.Get<BT3Mixin1>(bt3);
      Assert.That(mixin.Next, Is.Not.Null);
      Assert.That(mixin.Next.GetType(), Is.SameAs(bt3.GetType().GetField("__first", BindingFlags.NonPublic | BindingFlags.Instance).FieldType));

      var mixin2 = Mixin.Get<BT3Mixin1B>(bt3);
      Assert.That(mixin2.Next, Is.Not.Null);
      Assert.That(mixin2.Next.GetType(), Is.SameAs(bt3.GetType().GetField("__first", BindingFlags.NonPublic | BindingFlags.Instance).FieldType));

      BaseType3 bt3A = Serializer.SerializeAndDeserialize(bt3);
      var mixinA = Mixin.Get<BT3Mixin1>(bt3A);
      Assert.That(mixinA, Is.Not.SameAs(mixin));
      Assert.That(mixinA.Next, Is.Not.Null);
      Assert.That(mixinA.Next.GetType(), Is.SameAs(bt3A.GetType().GetField("__first", BindingFlags.NonPublic | BindingFlags.Instance).FieldType));

      var mixin2A = Mixin.Get<BT3Mixin1B>(bt3A);
      Assert.That(mixin2A, Is.Not.SameAs(mixin2));
      Assert.That(mixin2A.Next, Is.Not.Null);
      Assert.That(mixin2A.Next.GetType(), Is.SameAs(bt3A.GetType().GetField("__first", BindingFlags.NonPublic | BindingFlags.Instance).FieldType));
    }

#pragma warning disable SYSLIB0050
    [Test]
    public void GeneratedTypeIsSerializable ()
    {
      ClassOverridingMixinMembers targetInstance = CreateMixedObject<ClassOverridingMixinMembers>(typeof(MixinWithAbstractMembers));
      var mixin = Mixin.Get<MixinWithAbstractMembers>(targetInstance);
      Assert.That(mixin.GetType().IsSerializable, Is.True);
      Serializer.Serialize(targetInstance);
    }
#pragma warning restore SYSLIB0050

    [Test]
    public void GeneratedTypeIsDeserializable ()
    {
      ClassOverridingMixinMembers targetInstance = CreateMixedObject<ClassOverridingMixinMembers>(typeof(MixinWithAbstractMembers));
      var mixin = Mixin.Get<MixinWithAbstractMembers>(targetInstance);

      mixin.I = 13;

      MixinWithAbstractMembers mixinA = Serializer.SerializeAndDeserialize(mixin);
      Assert.That(mixinA.I, Is.EqualTo(mixin.I));
      Assert.That(mixinA, Is.Not.SameAs(mixin));
    }

    [Test]
    public void GeneratedTypeCorrectlySerializesThisAndBase ()
    {
      ClassOverridingMixinMembers targetInstance = CreateMixedObject<ClassOverridingMixinMembers>(typeof(MixinWithAbstractMembers));
      var mixin = Mixin.Get<MixinWithAbstractMembers>(targetInstance);

      Assert.That(MixinReflector.GetTargetProperty(mixin.GetType()).GetValue(mixin, null), Is.EqualTo(targetInstance));
      Assert.That(MixinReflector.GetNextProperty(mixin.GetType()).GetValue(mixin, null).GetType(), Is.EqualTo(MixinReflector.GetNextCallProxyType(targetInstance)));

      ClassOverridingMixinMembers targetInstanceA = Serializer.SerializeAndDeserialize(targetInstance);
      var mixinA = Mixin.Get<MixinWithAbstractMembers>(targetInstanceA);

      Assert.That(MixinReflector.GetTargetProperty(mixinA.GetType()).GetValue(mixinA, null), Is.EqualTo(targetInstanceA));
      Assert.That(MixinReflector.GetNextProperty(mixinA.GetType()).GetValue(mixinA, null).GetType(), Is.EqualTo(MixinReflector.GetNextCallProxyType(targetInstanceA)));
    }

    [Test]
    public void RespectsISerializable ()
    {
      ClassOverridingMixinMembers targetInstance =
          CreateMixedObject<ClassOverridingMixinMembers>(typeof(AbstractMixinImplementingISerializable));
      var mixin = Mixin.Get<AbstractMixinImplementingISerializable>(targetInstance);

      mixin.I = 15;
      Assert.That(mixin.I, Is.EqualTo(15));

      AbstractMixinImplementingISerializable mixinA = Serializer.SerializeAndDeserialize(mixin);
      Assert.That(mixinA.I, Is.EqualTo(32));
    }

    [Test]
    public void ThrowsIfAbstractMixinTypeNotSerializable ()
    {
      ClassOverridingMixinMembers targetInstance =
          CreateMixedObject<ClassOverridingMixinMembers>(typeof(NotSerializableMixin));
      Assert.That(
          () => Serializer.SerializeAndDeserialize(targetInstance),
          Throws.InstanceOf<SerializationException>()
              .With.Message.Contains("is not marked as serializable"));
    }

    [Test]
    public void ThrowsIfAbstractMixinTypeNotSerializable_EvenWithISerializable ()
    {
      ClassOverridingMixinMembers targetInstance = CreateMixedObject<ClassOverridingMixinMembers>(typeof(NotSerializableMixinWithISerializable));
      Assert.That(
          () => Serializer.SerializeAndDeserialize(targetInstance),
          Throws.InstanceOf<SerializationException>()
              .With.Message.Contains("is not marked as serializable"));
    }

    [Test]
    public void SerializationOfGeneratedMixinWorks ()
    {
      ClassOverridingSingleMixinMethod com = CreateMixedObject<ClassOverridingSingleMixinMethod>(typeof(MixinOverridingClassMethod));
      var comAsIfc = com as IMixinOverridingClassMethod;
      Assert.That(Mixin.Get<MixinOverridingClassMethod>(com), Is.Not.Null);

      Assert.That(comAsIfc, Is.Not.Null);
      Assert.That(comAsIfc.AbstractMethod(25), Is.EqualTo("ClassOverridingSingleMixinMethod.AbstractMethod-25"));
      Assert.That(com.OverridableMethod(13), Is.EqualTo("MixinOverridingClassMethod.OverridableMethod-13"));

      ClassOverridingSingleMixinMethod com2 = Serializer.SerializeAndDeserialize(com);
      var com2AsIfc = com as IMixinOverridingClassMethod;
      Assert.That(Mixin.Get<MixinOverridingClassMethod>(com2), Is.Not.Null);
      Assert.That(Mixin.Get<MixinOverridingClassMethod>(com2), Is.Not.SameAs(Mixin.Get<MixinOverridingClassMethod>(com)));

      Assert.That(com2AsIfc, Is.Not.Null);
      Assert.That(com2AsIfc.AbstractMethod(25), Is.EqualTo("ClassOverridingSingleMixinMethod.AbstractMethod-25"));
      Assert.That(com2.OverridableMethod(13), Is.EqualTo("MixinOverridingClassMethod.OverridableMethod-13"));
    }
  }
}
