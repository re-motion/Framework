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
using Remotion.Development.UnitTesting;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Mixins.UnitTests.Core.Validation.ValidationTestDomain;
using Remotion.TypePipe;

namespace Remotion.Mixins.UnitTests.Core
{
  [TestFixture]
  public class ObjectFactoryTest
  {
    public interface IEmptyInterface
    {
    }

    public class MixinThrowingInOnInitialized : Mixin<object>
    {
      protected override void OnInitialized ()
      {
        throw new NotSupportedException();
      }
    }

    public class MixinThrowingInCtor : Mixin<object>
    {
      public MixinThrowingInCtor ()
      {
        throw new NotSupportedException();
      }
    }

    public class TargetClassWithProtectedCtors
    {
      protected TargetClassWithProtectedCtors ()
      {
      }

      protected TargetClassWithProtectedCtors (int i)
      {
        Dev.Null = i;
      }
    }

    [Test]
    public void AcceptsInstanceOfGeneratedMixinType_OverriddenMixinMembers ()
    {
      Type generatedMixinType = CodeGenerationTypeMother.GetGeneratedMixinTypeInActiveConfiguration (typeof (ClassOverridingMixinMembers), typeof (MixinWithAbstractMembers));
      object mixinInstance = Activator.CreateInstance (generatedMixinType);

      var classInstance = ObjectFactory.Create<ClassOverridingMixinMembers> (ParamList.Empty, mixinInstance);
      Assert.That (Mixin.Get<MixinWithAbstractMembers> (classInstance), Is.SameAs (mixinInstance));
    }

    [Test]
    public void AcceptsInstanceOfGeneratedMixinType_WrappedProtectedMixinMembers ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1>().Clear().AddMixins (typeof (MixinWithProtectedOverrider)).EnterScope())
      {
        Type generatedMixinType = CodeGenerationTypeMother.GetGeneratedMixinTypeInActiveConfiguration (typeof (BaseType1), typeof (MixinWithProtectedOverrider));
        object mixinInstance = Activator.CreateInstance (generatedMixinType);
        var bt1 = ObjectFactory.Create<BaseType1> (ParamList.Empty, mixinInstance);
        bt1.VirtualMethod();
        Assert.That (Mixin.Get<MixinWithProtectedOverrider> (bt1), Is.SameAs (mixinInstance));
      }
    }

    [Test]
    public void ComposedFaceInterfacesAddedByMixins ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType3>().Clear().AddMixins (typeof (Bt3Mixin7TargetCall), typeof (BT3Mixin4)).EnterScope())
      {
        var composed = (ICBaseType3BT3Mixin4) ObjectFactory.Create<BaseType3> (ParamList.Empty);

        Assert.That (((IBaseType33) composed).IfcMethod(), Is.EqualTo ("BaseType3.IfcMethod"));
        Assert.That (((IBaseType34) composed).IfcMethod(), Is.EqualTo ("BaseType3.IfcMethod"));
        Assert.That (composed.IfcMethod2(), Is.EqualTo ("BaseType3.IfcMethod2"));
        Assert.That (Mixin.Get<Bt3Mixin7TargetCall> (composed).InvokeThisMethods(), Is.EqualTo ("BaseType3.IfcMethod-BT3Mixin4.Foo"));
      }
    }

    [Test]
    public void ComposedFaceInterfacesAddedExplicitly ()
    {
      object composed = ObjectFactory.Create<BaseType6> (ParamList.Empty);

      Assert.That (composed, Is.Not.Null);
      Assert.That (composed, Is.InstanceOf<BaseType6>());
      Assert.That (composed, Is.InstanceOf<ICBT6Mixin1>());
      Assert.That (composed, Is.InstanceOf<ICBT6Mixin2>());
      Assert.That (composed, Is.InstanceOf<ICBT6Mixin3>());
    }

    [Test]
    public void DefaultPolicyIsOnlyIfNecessary ()
    {
      object o = ObjectFactory.Create (typeof (object), ParamList.Empty);
      Assert.That (o.GetType(), Is.EqualTo (typeof (object)));

      o = ObjectFactory.Create<object> (ParamList.Empty);
      Assert.That (o.GetType(), Is.EqualTo (typeof (object)));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void ExceptionPropagated_WhenMixinOnInitializedThrows ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget>().Clear().AddMixins (typeof (MixinThrowingInOnInitialized)).EnterScope())
      {
        ObjectFactory.Create<NullTarget> (ParamList.Empty);
      }
    }

    [Test]
    public void TypesAreGeneratedOnlyIfNecessary ()
    {
      object o = ObjectFactory.Create (typeof (object), ParamList.Empty);
      Assert.That (o.GetType(), Is.EqualTo (typeof (object)));

      o = ObjectFactory.Create<object> (ParamList.Empty);
      Assert.That (o.GetType(), Is.EqualTo (typeof (object)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Cannot instantiate type 'Remotion.Mixins.UnitTests.Core.TestDomain.IBaseType2', it's an interface.\r\nParameter name: targetOrConcreteType")]
    public void InterfaceAsTypeArgument ()
    {
      ObjectFactory.Create<IBaseType2> (ParamList.Empty);
    }

    [Test]
    public void MixedObjectsCanBeCreated ()
    {
      object o = ObjectFactory.Create<BaseType3> (ParamList.Empty);
      Assert.That (o, Is.Not.Null);
      Assert.That (o, Is.InstanceOf<BaseType3>());
      Assert.That (o, Is.InstanceOf<IMixinTarget>());

      Assert.That (((IMixinTarget) o).Mixins[0], Is.Not.Null);
    }

    [Test]
    public void MixedObjectsCanBeCreated_Parameterless ()
    {
      object o = ObjectFactory.Create<BaseType3> ();
      Assert.That (o, Is.Not.Null);
      Assert.That (o, Is.InstanceOf <BaseType3>());
      Assert.That (o, Is.InstanceOf<IMixinTarget>());

      Assert.That (((IMixinTarget) o).Mixins[0], Is.Not.Null);
    }

    [Test]
    public void MixedObjectsCanBeCreatedFromType ()
    {
      object o = ObjectFactory.Create (typeof (BaseType3), ParamList.Empty);
      Assert.That (o, Is.Not.Null);
      Assert.That (o is IMixinTarget, Is.True);
      Assert.That (((IMixinTarget) o).Mixins[0], Is.Not.Null);
    }

    [Test]
    public void MixedObjectsCanBeCreatedFromType_Parameterless ()
    {
      object o = ObjectFactory.Create (typeof (BaseType3));
      Assert.That (o, Is.Not.Null);
      Assert.That (o is IMixinTarget, Is.True);
      Assert.That (((IMixinTarget) o).Mixins[0], Is.Not.Null);
    }

    [Test]
    public void MixedObjectsCanBeCreatedWithMixinInstances ()
    {
      var m1 = new BT1Mixin1();
      var bt1 = ObjectFactory.Create<BaseType1> (ParamList.Empty, m1);

      Assert.That (Mixin.Get<BT1Mixin1> (bt1), Is.Not.Null);
      Assert.That (Mixin.Get<BT1Mixin1> (bt1), Is.SameAs (m1));
      Assert.That (Mixin.Get<BT1Mixin2> (bt1), Is.Not.Null);
      Assert.That (Mixin.Get<BT1Mixin2> (bt1), Is.Not.SameAs (m1));
    }

    [Test]
    public void MixedObjectsWithMixinInstancesCanBeCreatedFromType ()
    {
      var m1 = new BT1Mixin1();
      var bt1 = (BaseType1) ObjectFactory.Create (typeof (BaseType1), ParamList.Empty, m1);

      Assert.That (Mixin.Get<BT1Mixin1> (bt1), Is.Not.Null);
      Assert.That (Mixin.Get<BT1Mixin1> (bt1), Is.SameAs (m1));
      Assert.That (Mixin.Get<BT1Mixin2> (bt1), Is.Not.Null);
      Assert.That (Mixin.Get<BT1Mixin2> (bt1), Is.Not.SameAs (m1));
    }

    [Test]
    public void MixinsAreInitializedWithBase ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType3>().Clear().AddMixins (typeof (BT3Mixin1)).EnterScope())
      {
        var bt3 = ObjectFactory.Create<BaseType3> (ParamList.Empty);
        var mixin = Mixin.Get<BT3Mixin1> (bt3);
        Assert.That (mixin, Is.Not.Null);
        Assert.That (mixin.Target, Is.SameAs (bt3));
        Assert.That (mixin.Next, Is.Not.Null);
        Assert.That (mixin.Next, Is.Not.SameAs (bt3));
      }
    }

    [Test]
    public void MixinsAreInitializedWithTarget ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType3>().Clear().AddMixins (typeof (BT3Mixin2)).EnterScope())
      {
        var bt3 = ObjectFactory.Create<BaseType3> (ParamList.Empty);
        var mixin = Mixin.Get<BT3Mixin2> (bt3);
        Assert.That (mixin, Is.Not.Null);
        Assert.That (mixin.Target, Is.SameAs (bt3));
      }
    }

    [Test]
    public void MixinWithoutPublicCtor ()
    {
      using (
          MixinConfiguration.BuildFromActive().ForClass<NullTarget>().Clear().AddMixins (typeof (MixinWithPrivateCtorAndVirtualMethod)).EnterScope())
      {
        MixinWithPrivateCtorAndVirtualMethod mixin = MixinWithPrivateCtorAndVirtualMethod.Create();
        object o = ObjectFactory.Create<NullTarget> (ParamList.Empty, mixin);
        Assert.That (o, Is.Not.Null);
        Assert.That (Mixin.Get<MixinWithPrivateCtorAndVirtualMethod> (o), Is.Not.Null);
        Assert.That (Mixin.Get<MixinWithPrivateCtorAndVirtualMethod> (o), Is.SameAs (mixin));
      }
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Type 'Remotion.Mixins.UnitTests.Core.ObjectFactoryTest+"
       + "TargetClassWithProtectedCtors' contains a constructor with the required signature, but it is not public (and the allowNonPublic flag is "
       + "not set).")]
    public void ProtectedDefaultConstructor_Mixed ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<TargetClassWithProtectedCtors>().Clear().AddMixins (typeof (NullMixin)).EnterScope())
      {
        ObjectFactory.Create<TargetClassWithProtectedCtors> (ParamList.Empty);
      }
    }

    [Test]
    public void ProtectedDefaultConstructor_Mixed_AllowProtected ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<TargetClassWithProtectedCtors>().Clear().AddMixins (typeof (NullMixin)).EnterScope())
      {
        Assert.That (ObjectFactory.Create<TargetClassWithProtectedCtors> (true, ParamList.Empty), Is.Not.Null);
      }
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Type 'Remotion.Mixins.UnitTests.Core.ObjectFactoryTest+"
       + "TargetClassWithProtectedCtors' contains a constructor with the required signature, but it is not public (and the allowNonPublic flag is "
       + "not set).")]
    public void ProtectedDefaultConstructor_NonMixed ()
    {
      using (MixinConfiguration.BuildNew().EnterScope())
      {
        ObjectFactory.Create<TargetClassWithProtectedCtors> (ParamList.Empty);
      }
    }

    [Test]
    public void ProtectedDefaultConstructor_NonMixed_AllowProtected ()
    {
      using (MixinConfiguration.BuildNew().EnterScope())
      {
        Assert.That (ObjectFactory.Create<TargetClassWithProtectedCtors> (true, ParamList.Empty), Is.Not.Null);
      }
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Type 'Remotion.Mixins.UnitTests.Core.ObjectFactoryTest+"
       + "TargetClassWithProtectedCtors' contains a constructor with the required signature, but it is not public (and the allowNonPublic flag is "
       + "not set).")]
    public void ProtectedNonDefaultConstructor_Mixed ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<TargetClassWithProtectedCtors>().Clear().AddMixins (typeof (NullMixin)).EnterScope())
      {
        ObjectFactory.Create<TargetClassWithProtectedCtors> (ParamList.Create (1));
      }
    }

    [Test]
    public void ProtectedNonDefaultConstructor_Mixed_AllowProtected ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<TargetClassWithProtectedCtors>().Clear().AddMixins (typeof (NullMixin)).EnterScope())
      {
        Assert.That (ObjectFactory.Create<TargetClassWithProtectedCtors> (true, ParamList.Create (1)), Is.Not.Null);
      }
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Type 'Remotion.Mixins.UnitTests.Core.ObjectFactoryTest+"
       + "TargetClassWithProtectedCtors' contains a constructor with the required signature, but it is not public (and the allowNonPublic flag is "
       + "not set).")]
    public void ProtectedNonDefaultConstructor_NonMixed ()
    {
      using (MixinConfiguration.BuildNew().EnterScope())
      {
        ObjectFactory.Create<TargetClassWithProtectedCtors> (ParamList.Create (1));
      }
    }

    [Test]
    public void ProtectedNonDefaultConstructor_NonMixed_AllowProtected ()
    {
      using (MixinConfiguration.BuildNew().EnterScope())
      {
        Assert.That (ObjectFactory.Create<TargetClassWithProtectedCtors> (true, ParamList.Create (1)), Is.Not.Null);
      }
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void TargetInvocationExceptionWhenMixinCtorThrows ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget>().Clear().AddMixins (typeof (MixinThrowingInCtor)).EnterScope())
      {
        ObjectFactory.Create<NullTarget> (ParamList.Empty);
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "There is no mixin configuration for type System.Object, so no mixin instances "
                                                                      + "must be specified.", MatchType = MessageMatch.Regex)]
    public void ThrowsOnMixinInstancesWhenNoGeneration ()
    {
      ObjectFactory.Create (typeof (object), ParamList.Empty, new object());
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The supplied mixin of type "
        + "'Remotion.Mixins.UnitTests.Core.TestDomain.BT2Mixin1' is not valid for target type 'Remotion.Mixins.UnitTests.Core.TestDomain.BaseType3' in the "
        + "current configuration.")]
    public void ThrowsOnWrongMixinInstances ()
    {
      var m1 = new BT2Mixin1();
      ObjectFactory.Create<BaseType3> (ParamList.Empty, m1);
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Cannot instantiate mixin "
       + "'Remotion.Mixins.UnitTests.Core.Validation.ValidationTestDomain.MixinWithPrivateCtorAndVirtualMethod' applied to class "
       + "'Remotion.Mixins.UnitTests.Core.TestDomain.NullTarget', there is no visible default constructor.")]
    public void ThrowsWhenMixinWithoutPublicDefaultCtorShouldBeInstantiated ()
    {
      using (
          MixinConfiguration.BuildFromActive().ForClass<NullTarget>().Clear().AddMixins (typeof (MixinWithPrivateCtorAndVirtualMethod)).EnterScope())
      {
        ObjectFactory.Create<NullTarget> (ParamList.Empty);
      }
    }
  }
}
