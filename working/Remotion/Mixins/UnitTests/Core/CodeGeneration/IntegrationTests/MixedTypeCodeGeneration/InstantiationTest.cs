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
using System.Reflection.Emit;
using NUnit.Framework;
using Remotion.Mixins.UnitTests.Core.CodeGeneration.TestDomain;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Mixins.Utilities;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class InstantiationTest : CodeGenerationBaseTest
  {
    [Test]
    public void GeneratedTypeCanBeInstantiatedViaCtorCall ()
    {
      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType3));
      var bt3 = (BaseType3) Activator.CreateInstance (generatedType);
      Assert.That (bt3, Is.Not.Null);
      Assert.That (Mixin.Get<BT3Mixin1> (bt3), Is.Not.Null);
      Assert.That (Mixin.Get<BT3Mixin1> (bt3).Target, Is.Not.Null);
      Assert.That (Mixin.Get<BT3Mixin1> (bt3).Next, Is.Not.Null);
      Assert.That (Mixin.Get<BT3Mixin1> (bt3).Target, Is.SameAs (bt3));
    }

    [Test]
    public void GeneratedTypeCanBeInstantiatedViaCtorCallEvenWhenDerived ()
    {
      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType3));

      AssemblyBuilder builder =
          AppDomain.CurrentDomain.DefineDynamicAssembly (new AssemblyName ("Foo"), AssemblyBuilderAccess.Run);
      TypeBuilder typeBuilder = builder.DefineDynamicModule ("Foo.dll").DefineType ("Derived", TypeAttributes.Public, generatedType);
      ConstructorBuilder ctor = typeBuilder.DefineConstructor (MethodAttributes.Public, CallingConventions.HasThis, Type.EmptyTypes);
      ILGenerator ilgen = ctor.GetILGenerator ();
      ilgen.Emit (OpCodes.Ldarg_0);
      ilgen.Emit (OpCodes.Callvirt, generatedType.GetConstructor (Type.EmptyTypes));
      ilgen.Emit (OpCodes.Ret);

      Type evenDerivedType = typeBuilder.CreateType ();

      var bt3 = (BaseType3) Activator.CreateInstance (evenDerivedType);
      Assert.That (bt3.GetType ().BaseType, Is.SameAs (generatedType));
      Assert.That (bt3, Is.Not.Null);
      Assert.That (Mixin.Get<BT3Mixin1> (bt3), Is.Not.Null);
      Assert.That (Mixin.Get<BT3Mixin1> (bt3).Target, Is.Not.Null);
      Assert.That (Mixin.Get<BT3Mixin1> (bt3).Next, Is.Not.Null);
      Assert.That (Mixin.Get<BT3Mixin1> (bt3).Target, Is.SameAs (bt3));
    }

    [Test]
    public void CtorsRespectMixedTypeInstantiationScope ()
    {
      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType3));
      var suppliedMixinInstance = new BT3Mixin1 ();

      using (new MixedObjectInstantiationScope (suppliedMixinInstance))
      {
        var bt3 = (BaseType3) Activator.CreateInstance (generatedType);
        Assert.That (Mixin.Get<BT3Mixin1> (bt3), Is.Not.Null);
        Assert.That (Mixin.Get<BT3Mixin1> (bt3), Is.SameAs (suppliedMixinInstance));
        Assert.That (suppliedMixinInstance.Target, Is.SameAs (bt3));
        Assert.That (Mixin.Get<BT3Mixin1> (bt3).Next, Is.Not.Null);
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "The supplied mixin of type 'Remotion.Mixins.UnitTests.Core.TestDomain.BT3Mixin1' is not valid for target type "
        + "'Remotion.Mixins.UnitTests.Core.TestDomain.BaseType1' in the current configuration.")]
    public void ThrowsIfWrongMixinInstancesInScope ()
    {
      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType1));
      var suppliedMixinInstance = new BT3Mixin1 ();

      using (new MixedObjectInstantiationScope (suppliedMixinInstance))
      {
        try
        {
          Activator.CreateInstance (generatedType);
        }
        catch (TargetInvocationException ex)
        {
          throw ex.InnerException;
        }
      }
    }

    [Test]
    public void OverriddenMethodCalledFromCtor ()
    {
      var instance = CreateMixedObject<TargetClassCallingOverriddenMethodFromCtor> (typeof (MixinOverridingMethodCalledFromCtor));
      var mixin = Mixin.Get<MixinOverridingMethodCalledFromCtor> (instance);
      Assert.That (instance.Result, Is.SameAs (mixin));
      Assert.That (mixin.MyThis, Is.SameAs (instance));
      Assert.That (mixin.MyBase, Is.Not.Null);
    }

    [Test]
    public void IntroducedMethodCalledFromCtor ()
    {
      var instance = CreateMixedObject<TargetClassCallingIntroducedMethodFromCtor> (typeof (MixinIntroducingMethodCalledFromCtor));
      
      var mixin = Mixin.Get<MixinIntroducingMethodCalledFromCtor> (instance);
      Assert.That (instance.Result, Is.SameAs (mixin));
      Assert.That (mixin.MyThis, Is.SameAs (instance));
      Assert.That (mixin.MyBase, Is.Not.Null);
    }

    [Test]
    public void MixinGet_CalledFromCtor ()
    {
      var instance = CreateMixedObject<TargetClassGettingMixinFromCtor> (typeof (NullMixin));

      var mixin = Mixin.Get<NullMixin> (instance);
      Assert.That (instance.MixinInstance, Is.SameAs (mixin));
    }

    [Test]
    public void MixinTarget_FirstCallProxy_AccessedFromCtor ()
    {
      var instance = CreateMixedObject<TargetClassAccessingFirstCallProxyFromCtor> (typeof (NullMixin));

      Assert.That (instance.FirstNextCallProxy, Is.Not.Null);

      // other assertions are performed by TargetClassAccessingFirstCallProxyFromCtor
    }

    [Test]
    public void MixinTarget_ClassContext_AccessedFromCtor ()
    {
      var instance = CreateMixedObject<TargetClassAccessingClassContextFromCtor> (typeof (NullMixin));

      Assert.That (instance.ClassContext, Is.Not.Null);

      // other assertions are performed by TargetClassAccessingClassContextFromCtor
    }

    [Test]
    public void MixinTarget_Mixins_AccessedFromCtor ()
    {
      var instance = CreateMixedObject<TargetClassAccessingMixinsFromCtor> (typeof (NullMixin));

      Assert.That (instance.Mixins, Is.Not.Null);

      // other assertions are performed by TargetClassAccessingMixinsFromCtor
    }
  }
}
