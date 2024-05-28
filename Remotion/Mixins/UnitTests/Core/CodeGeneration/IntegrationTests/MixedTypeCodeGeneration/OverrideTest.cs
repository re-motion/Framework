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
using System.Reflection;
using JetBrains.Annotations;
using NUnit.Framework;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.TypePipe;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class OverrideTest : CodeGenerationBaseTest
  {
    [Test]
    public void OverrideClassMethods ()
    {
      BaseType1 bt1 = CreateMixedObject<BaseType1>(typeof(BT1Mixin1));

      Assert.That(bt1.VirtualMethod(), Is.EqualTo("BT1Mixin1.VirtualMethod"));
      Assert.That(bt1.GetType().GetMethod("VirtualMethod", Type.EmptyTypes), Is.Not.Null, "overridden member is public and has the same name");
      Assert.That(bt1.GetType().GetMethod("VirtualMethod", Type.EmptyTypes).GetBaseDefinition().DeclaringType, Is.EqualTo(typeof(BaseType1)));
    }

    [Test]
    [Ignore("TODO: This does not work on the build server, check why.")]
    public void OverrideClassProperties ()
    {
      BaseType1 bt1 = CreateMixedObject<BaseType1>(typeof(BT1Mixin1));

      Assert.That(bt1.VirtualProperty, Is.EqualTo("BaseType1.BackingField"));
      Assert.That(Mixin.Get<BT1Mixin1>(bt1).BackingField, Is.Not.EqualTo("FooBar"));

      bt1.VirtualProperty = "FooBar";
      Assert.That(bt1.VirtualProperty, Is.EqualTo("BaseType1.BackingField"));
      Assert.That(Mixin.Get<BT1Mixin1>(bt1).BackingField, Is.EqualTo("FooBar"));

      Assert.That(bt1.GetType().GetProperty("VirtualProperty"), Is.Not.Null, "overridden member is public and has the same name");

      bt1 = CreateMixedObject<BaseType1>(typeof(BT1Mixin2));

      Assert.That(bt1.VirtualProperty, Is.EqualTo("Mixin2ForBT1.VirtualProperty"));
      bt1.VirtualProperty = "Foobar";
      Assert.That(bt1.VirtualProperty, Is.EqualTo("Mixin2ForBT1.VirtualProperty"));
    }

    [Test]
    public void OverrideClassPropertiesTemp ()
    {
      BaseType1 bt1 = CreateMixedObject<BaseType1>(typeof(BT1Mixin1));

      Assert.That(bt1.VirtualProperty, Is.EqualTo("BaseType1.BackingField"));
      Assert.That(Mixin.Get<BT1Mixin1>(bt1).BackingField, Is.Not.EqualTo("FooBar"));

      bt1.VirtualProperty = "FooBar";
      Assert.That(bt1.VirtualProperty, Is.EqualTo("BaseType1.BackingField"));
      Assert.That(Mixin.Get<BT1Mixin1>(bt1).BackingField, Is.EqualTo("FooBar"));

      Assert.That(
          bt1.GetType().GetProperty(
              "VirtualProperty",
              BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly),
          Is.Not.Null,
          "overridden member is public and has the same name");

      bt1 = CreateMixedObject<BaseType1>(typeof(BT1Mixin2));

      Assert.That(bt1.VirtualProperty, Is.EqualTo("Mixin2ForBT1.VirtualProperty"));
      bt1.VirtualProperty = "Foobar";
      Assert.That(bt1.VirtualProperty, Is.EqualTo("Mixin2ForBT1.VirtualProperty"));
    }

    [Test]
    public void OverrideClassEvents ()
    {
      BaseType1 bt1 = CreateMixedObject<BaseType1>(typeof(BT1Mixin1));

      EventHandler eventHandler = delegate { };

      Assert.That(Mixin.Get<BT1Mixin1>(bt1).VirtualEventAddCalled, Is.False);
      bt1.VirtualEvent += eventHandler;
      Assert.That(Mixin.Get<BT1Mixin1>(bt1).VirtualEventAddCalled, Is.True);

      Assert.That(Mixin.Get<BT1Mixin1>(bt1).VirtualEventRemoveCalled, Is.False);
      bt1.VirtualEvent -= eventHandler;
      Assert.That(Mixin.Get<BT1Mixin1>(bt1).VirtualEventRemoveCalled, Is.True);

      Assert.That(bt1.GetType().GetEvent("VirtualEvent"), Is.Not.Null, "overridden member is public and has the same name");
    }

    [Test]
    public void OverrideWithComposedBaseInterface ()
    {
      BaseType3 bt3 = CreateMixedObject<BaseType3>(typeof(BT3Mixin7Base), typeof(BT3Mixin4));
      Assert.That(bt3.IfcMethod(), Is.EqualTo("BT3Mixin7Base.IfcMethod-BT3Mixin4.Foo-BaseType3.IfcMethod-BaseType3.IfcMethod2"));
    }

    [Test]
    public void MixinOverridingInheritedClassMethod ()
    {
      ClassWithInheritedMethod cwim = ObjectFactory.Create<ClassWithInheritedMethod>(ParamList.Empty);
      Assert.That(
          cwim.InvokeInheritedMethods(),
          Is.EqualTo(
              "MixinOverridingInheritedMethod.ProtectedInheritedMethod-BaseClassWithInheritedMethod.ProtectedInheritedMethod-"
              + "MixinOverridingInheritedMethod.ProtectedInternalInheritedMethod-BaseClassWithInheritedMethod.ProtectedInternalInheritedMethod-"
              + "MixinOverridingInheritedMethod.PublicInheritedMethod-BaseClassWithInheritedMethod.PublicInheritedMethod"));
    }

    [Test]
    public void MixinWithProtectedOverrider ()
    {
      BaseType1 obj = CreateMixedObject<BaseType1>(typeof(MixinWithProtectedOverrider));
      Assert.That(obj.VirtualMethod(), Is.EqualTo("MixinWithProtectedOverrider.VirtualMethod-BaseType1.VirtualMethod"));
      Assert.That(obj.VirtualProperty, Is.EqualTo("MixinWithProtectedOverrider.VirtualProperty-BaseType1.BackingField"));

      Assert.That(obj.GetVirtualEventInvocationList(), Is.EqualTo(null));
      obj.VirtualEvent += delegate { };
      Assert.That(obj.GetVirtualEventInvocationList().Length, Is.EqualTo(2));
    }

    [Test]
    public void ValueTypeMixin ()
    {
      BaseType1 bt1 = CreateMixedObject<BaseType1>(typeof(ValueTypeMixin));
      Assert.That(bt1.VirtualMethod(), Is.EqualTo("ValueTypeMixin.VirtualMethod"));
    }

    [Test]
    public void AlphabeticOrdering ()
    {
      ClassWithMixinsAcceptingAlphabeticOrdering instance = ObjectFactory.Create<ClassWithMixinsAcceptingAlphabeticOrdering>(ParamList.Empty);
      Assert.That(
          instance.ToString(),
          Is.EqualTo("MixinAcceptingAlphabeticOrdering1.ToString-MixinAcceptingAlphabeticOrdering2.ToString-ClassWithMixinsAcceptingAlphabeticOrdering.ToString"));
    }

    [Test]
    public void MethodWithArrayArray ()
    {
      var instance = ObjectFactory.Create<TypeWithArrayArray>();
      var bytes = new[] { new byte[] { 1, 2, 3 }, new byte[] { 4, 5, 6 } };

      var result = instance.M(bytes);

      Assert.That(result, Is.EqualTo("{4,5,6},{1,2,3} mixed"));
    }

    public class TypeWithArrayArray
    {
      [UsedImplicitly]
      public virtual string M (byte[][] bytes)
      {
        return string.Join(",", bytes.Select(bs => "{"  + string.Join(",", bs) + "}"));
      }
    }

    [Extends(typeof(TypeWithArrayArray))]
    public class MixinMixingTypeWithArrayArray : Mixin<MixinMixingTypeWithArrayArray.IRequirements, MixinMixingTypeWithArrayArray.IRequirements>
    {
      public interface IRequirements
      {
        string M (byte[][] bytes);
      }

      [OverrideTarget]
      public virtual string M (byte[][] bytes)
      {
        return Next.M(bytes.Reverse().ToArray()) + " mixed";
      }
    }
  }
}
