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
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Mixins.UnitTests.Core.CodeGeneration.TestDomain;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Mixins.Utilities;
using Remotion.TypePipe;
using Remotion.TypePipe.Implementation;

// ReSharper disable SuspiciousTypeConversion.Global
namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class InitializableMixinTargetTest : CodeGenerationBaseTest
  {
    [Test]
    public void GeneratedTypeImplementsIInitializableObject ()
    {
      Type concreteType = CreateMixedType(typeof(BaseType1), typeof(NullMixin));

      Assert.That(typeof(IMixinTarget).IsAssignableFrom(concreteType));
      Assert.That(typeof(IInitializableObject).IsAssignableFrom(concreteType));
    }

    [Test]
    public void Initialize_SetsFirstProxy ()
    {
      var instance = (IMixinTarget)CreateMixedObject<BaseType1>(typeof(NullMixin));

      var oldProxy = instance.FirstNextCallProxy;
      CallInitializeMixins(instance, InitializationSemantics.Construction);

      Assert.That(instance.FirstNextCallProxy, Is.Not.SameAs(oldProxy));
      Assert.That(PrivateInvoke.GetPublicField(instance.FirstNextCallProxy, "__depth"), Is.EqualTo(0));
    }

    [Test]
    public void Initialize_CreatesMixins ()
    {
      var instance = (IMixinTarget)CreateMixedObject<BaseType1>(typeof(NullMixin));

      var oldMixins = instance.Mixins;

      CallInitializeMixins(instance, InitializationSemantics.Construction);

      Assert.That(instance.Mixins, Is.Not.Null);
      Assert.That(instance.Mixins, Is.Not.SameAs(oldMixins));
      Assert.That(instance.Mixins.Length, Is.EqualTo(1));
      Assert.That(instance.Mixins[0], Is.InstanceOf(typeof(NullMixin)));
    }

    [Test]
    public void Initialize_CreatesMixins_OnlyOnce ()
    {
      var instance = (IMixinTarget)CreateMixedObject<BaseType1>(typeof(NullMixin));
      var oldMixins = instance.Mixins;

      CallInitializeMixins(instance, InitializationSemantics.Construction, extensionsInitializedValue: true);

      Assert.That(instance.Mixins, Is.SameAs(oldMixins));
    }

    [Test]
    public void Initialize_InitializesMixins ()
    {
      var instance = (IMixinTarget)CreateMixedObject<NullTarget>(typeof(MixinWithOnInitialized));
      ((MixinWithOnInitialized)instance.Mixins[0]).OnInitializedCalled = false;

      CallInitializeMixins(instance, InitializationSemantics.Construction);

      Assert.That(((MixinWithOnInitialized)instance.Mixins[0]).OnInitializedCalled, Is.True);
    }

    [Test]
    public void Initialize_InitializesMixins_OnlyOnce ()
    {
      var instance = (IMixinTarget)CreateMixedObject<NullTarget>(typeof(MixinWithOnInitialized));
      ((MixinWithOnInitialized)instance.Mixins[0]).OnInitializedCalled = false;

      CallInitializeMixins(instance, InitializationSemantics.Construction, extensionsInitializedValue: true);

      Assert.That(((MixinWithOnInitialized)instance.Mixins[0]).OnInitializedCalled, Is.False);
    }

    [Test]
    public void Initialize_InitializesMixins_WithNextCallProxies ()
    {
      var instance = (IMixinTarget)ObjectFactory.Create<BaseType7>(ParamList.Empty);

      Assert.That(GetDepthValue(instance.Mixins[0]), Is.EqualTo(1));
      Assert.That(GetDepthValue(instance.Mixins[1]), Is.EqualTo(2));
      Assert.That(GetDepthValue(instance.Mixins[2]), Is.EqualTo(3));
      Assert.That(GetDepthValue(instance.Mixins[3]), Is.EqualTo(4));
      Assert.That(GetDepthValue(instance.Mixins[4]), Is.EqualTo(5));
      Assert.That(GetDepthValue(instance.Mixins[5]), Is.EqualTo(6));
      Assert.That(GetDepthValue(instance.Mixins[6]), Is.EqualTo(7));
    }

    private object GetDepthValue (object mixin)
    {
      var nextProperty = MixinReflector.GetNextProperty(mixin.GetType());
      var baseValue = nextProperty.GetValue(mixin, null);
      return PrivateInvoke.GetPublicField(baseValue, "__depth");
    }

    private void CallInitializeMixins (
        object instance, InitializationSemantics initializationSemantics, bool extensionsInitializedValue = false, object[] mixinInstances = null)
    {
      Assert.That(instance, Is.InstanceOf<IInitializableObject>());

      // Forces re-initialization if set to false.
      PrivateInvoke.SetNonPublicField(instance, "__extensionsInitialized", extensionsInitializedValue);

      if (mixinInstances != null)
        PrivateInvoke.SetNonPublicField(instance, "__extensions", mixinInstances);

      // This code depends on the implementation detail that we simple call the "__InitializeMixins" method frome the TypePipe Initialize method.
      ((IInitializableObject)instance).Initialize(initializationSemantics);
    }
  }
}
