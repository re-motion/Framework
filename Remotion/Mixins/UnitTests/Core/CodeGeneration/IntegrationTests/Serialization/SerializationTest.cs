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
using Remotion.Mixins.UnitTests.Core.CodeGeneration.TestDomain;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.TypePipe;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.Serialization
{
#pragma warning disable SYSLIB0050
  [TestFixture]
  public class SerializationTest : CodeGenerationBaseTest
  {
    [Test]
    public void GeneratedTypeHasConfigurationField ()
    {
      Type t = TypeFactory.GetConcreteType(typeof(BaseType1));
      var classContextField = t.GetField("__classContext", BindingFlags.NonPublic | BindingFlags.Static);
      Assert.That(classContextField, Is.Not.Null);
      Assert.That(classContextField.IsStatic, Is.True);
    }

    [Test]
    public void GeneratedObjectFieldHoldsConfiguration ()
    {
      var bt1 = ObjectFactory.Create<BaseType1>(ParamList.Empty);

      var classContextField = bt1.GetType().GetField("__classContext", BindingFlags.NonPublic | BindingFlags.Static);
      Assert.That(classContextField.GetValue(null), Is.Not.Null);

      var expectedClassContext = MixinConfiguration.ActiveConfiguration.GetContext(typeof(BaseType1));
      Assert.That(classContextField.GetValue(null), Is.EqualTo(expectedClassContext));
    }

    [Test]
    public void GeneratedTypeIsSerializable ()
    {
      var bt1 = ObjectFactory.Create<BaseType1>(ParamList.Empty);
      Assert.That(bt1.GetType().IsSerializable, Is.True);

      bt1.I = 25;
      Serializer.Serialize(bt1);
    }

    [Test]
    public void GeneratedTypeIsDeserializable ()
    {
      var bt1 = ObjectFactory.Create<BaseType1>(ParamList.Empty);
      Assert.That(bt1.GetType().IsSerializable, Is.True);

      bt1.I = 25;
      Serializer.SerializeAndDeserialize(bt1);
      Assert.That(bt1.I, Is.EqualTo(25));
    }

    [Test]
    public void GeneratedTypeWithReferenceToMixinBaseIsDeserializable ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<OverridableBaseType>().Clear().AddMixins(typeof(MixinOverridingClassMethod)).EnterScope())
      {
        var instance = ObjectFactory.Create<OverridableBaseType>(ParamList.Empty);
        Assert.That(instance.GetType().IsSerializable, Is.True);

        Assert.That(instance.OverridableMethod(85), Is.EqualTo("MixinOverridingClassMethod.OverridableMethod-85"));

        OverridableBaseType deserialiedInstance = Serializer.SerializeAndDeserialize(instance);

        Assert.That(deserialiedInstance.OverridableMethod(85), Is.EqualTo("MixinOverridingClassMethod.OverridableMethod-85"));
        Assert.That(Mixin.Get<MixinOverridingClassMethod>(deserialiedInstance).Target, Is.SameAs(deserialiedInstance));

        Assert.That(Mixin.Get<MixinOverridingClassMethod>(deserialiedInstance).Next, Is.Not.Null);
        Assert.That(
            ((MixinOverridingClassMethod.IRequirements)Mixin.Get<MixinOverridingClassMethod>(deserialiedInstance).Next).OverridableMethod(84),
            Is.EqualTo("OverridableBaseType.OverridableMethod(84)"));
      }
    }

    [Test]
    public void DeserializedMembersFit ()
    {
      var bt1 = ObjectFactory.Create<BaseType1>(ParamList.Empty);
      Assert.That(bt1.GetType().IsSerializable, Is.True);

      bt1.I = 25;
      BaseType1 bt1a = Serializer.SerializeAndDeserialize(bt1);
      Assert.That(bt1a, Is.Not.SameAs(bt1));
      Assert.That(bt1a.I, Is.EqualTo(bt1.I));

      var bt2 = CreateMixedObject<BaseType2>(typeof(BT2Mixin1));
      Assert.That(bt2.GetType().IsSerializable, Is.True);

      bt2.S = "Bla";
      BaseType2 bt2a = Serializer.SerializeAndDeserialize(bt2);
      Assert.That(bt2a, Is.Not.SameAs(bt2));
      Assert.That(bt2a.S, Is.EqualTo(bt2.S));
    }

    [Test]
    public void ExtensionsAndConfigurationSerialized ()
    {
      var bt1 = ObjectFactory.Create<BaseType1>(ParamList.Empty);
      var mixinTarget = (IMixinTarget)bt1;

      BaseType1 deserializedBT1 = Serializer.SerializeAndDeserialize(bt1);
      var deserializedMixinTarget = (IMixinTarget)deserializedBT1;

      Assert.That(deserializedMixinTarget.ClassContext, Is.EqualTo(mixinTarget.ClassContext));

      Assert.That(deserializedMixinTarget.Mixins, Is.Not.Null);
      Assert.That(deserializedMixinTarget.Mixins.Length, Is.EqualTo(mixinTarget.Mixins.Length));
      Assert.That(deserializedMixinTarget.Mixins[0].GetType(), Is.EqualTo(mixinTarget.Mixins[0].GetType()));

      Assert.That(deserializedMixinTarget.FirstNextCallProxy, Is.Not.Null);
      Assert.That(deserializedMixinTarget.FirstNextCallProxy, Is.Not.EqualTo(mixinTarget.FirstNextCallProxy));
      Assert.That(deserializedMixinTarget.FirstNextCallProxy.GetType(), Is.EqualTo(deserializedMixinTarget.GetType().GetNestedType("NextCallProxy")));
      Assert.That(deserializedMixinTarget.FirstNextCallProxy.GetType().GetField("__depth").GetValue(deserializedMixinTarget.FirstNextCallProxy), Is.EqualTo(0));
      Assert.That(deserializedMixinTarget.FirstNextCallProxy.GetType().GetField("__this").GetValue(deserializedMixinTarget.FirstNextCallProxy), Is.SameAs(deserializedMixinTarget));
    }

    [Test]
    public void RespectsISerializable ()
    {
      ClassImplementingISerializable c = ObjectFactory.Create<ClassImplementingISerializable>(ParamList.Empty);
      Assert.That(c.GetType(), Is.Not.EqualTo(typeof(ClassImplementingISerializable)));

      c.I = 15;
      Assert.That(c.I, Is.EqualTo(15));

      ClassImplementingISerializable c2 = Serializer.SerializeAndDeserialize(c);
      Assert.That(c2, Is.Not.EqualTo(c));
      Assert.That(c2.I, Is.EqualTo(28));
    }

    [Test]
    public void ThrowsIfClassNotSerializable ()
    {
      NotSerializableClass targetInstance = CreateGeneratedTypeInstanceWithoutMixins<NotSerializableClass>();
      Assert.That(
          () => Serializer.SerializeAndDeserialize(targetInstance),
          Throws.InstanceOf<SerializationException>()
              .With.Message.Contains("is not marked as serializable"));
    }

    [Test]
    public void Throws_For_ClassNotSerializable_EvenWithISerializableImplementation ()
    {
      NotSerializableClassWithISerializable targetInstance = CreateGeneratedTypeInstanceWithoutMixins<NotSerializableClassWithISerializable>();
      Assert.That(
          () => Serializer.SerializeAndDeserialize(targetInstance),
          Throws.InstanceOf<SerializationException>()
              .With.Message.Contains("is not marked as serializable"));
    }

    [Test]
    public void WorksIfNoDefaultCtor ()
    {
      ClassWithoutDefaultCtor c = ObjectFactory.Create<ClassWithoutDefaultCtor>(ParamList.Create(35));
      Assert.That(c.GetType(), Is.Not.EqualTo(typeof(ClassImplementingISerializable)));

      Assert.That(c.S, Is.EqualTo("35"));

      ClassWithoutDefaultCtor c2 = Serializer.SerializeAndDeserialize(c);
      Assert.That(c2, Is.Not.EqualTo(c));
      Assert.That(c2.S, Is.EqualTo("35"));
    }

    [Test]
    public void OnInitializedNotCalledOnDeserialization ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget>().Clear().AddMixins(typeof(MixinWithOnInitializedAndOnDeserialized)).EnterScope())
      {
        NullTarget instance = ObjectFactory.Create<NullTarget>(ParamList.Empty);
        Assert.That(Mixin.Get<MixinWithOnInitializedAndOnDeserialized>(instance).OnInitializedCalled, Is.True);

        NullTarget deserializedInstance = Serializer.SerializeAndDeserialize(instance);
        Assert.That(Mixin.Get<MixinWithOnInitializedAndOnDeserialized>(deserializedInstance).OnInitializedCalled, Is.False);
      }
    }

    [Test]
    public void OnDeserializedCalledOnDeserialization ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget>().Clear().AddMixins(typeof(MixinWithOnInitializedAndOnDeserialized)).EnterScope())
      {
        NullTarget instance = ObjectFactory.Create<NullTarget>(ParamList.Empty);
        Assert.That(Mixin.Get<MixinWithOnInitializedAndOnDeserialized>(instance).OnDeserializedCalled, Is.False);

        NullTarget deserializedInstance = Serializer.SerializeAndDeserialize(instance);
        Assert.That(Mixin.Get<MixinWithOnInitializedAndOnDeserialized>(deserializedInstance).OnDeserializedCalled, Is.True);
      }
    }

    [Test]
    public void MixinConfigurationCanDifferAtDeserializationTime ()
    {
      byte[] serializedData;
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget>().Clear().AddMixins(typeof(NullMixin)).EnterScope())
      {
        NullTarget instance = ObjectFactory.Create<NullTarget>(ParamList.Empty);
        Assert.That(Mixin.Get<NullMixin>(instance), Is.Not.Null);
        serializedData = Serializer.Serialize(instance);
      }

      var deserializedInstance = (NullTarget)Serializer.Deserialize(serializedData);
      Assert.That(Mixin.Get<NullMixin>(deserializedInstance), Is.Not.Null);
    }
  }
#pragma warning restore SYSLIB0050
}
