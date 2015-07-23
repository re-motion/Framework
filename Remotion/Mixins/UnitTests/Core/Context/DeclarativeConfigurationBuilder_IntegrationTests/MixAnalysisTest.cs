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
using Remotion.Mixins.Context;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.Context.DeclarativeConfigurationBuilder_IntegrationTests
{
  [TestFixture]
  public class MixAnalysisTest
  {
    [Test]
    public void MixAttributeIsAnalyzed ()
    {
      ClassContext context = MixinConfiguration.ActiveConfiguration.GetContext (typeof (TargetClassForGlobalMix));
      Assert.That (context.Mixins.ContainsKey (typeof (MixinForGlobalMix)), Is.True);
    }

    [Test]
    public void Origin ()
    {
      ClassContext context = MixinConfiguration.ActiveConfiguration.GetContext (typeof (TargetClassForGlobalMix));
      
      var expectedOrigin = new MixinContextOrigin ("MixAttribute", typeof (TargetClassForGlobalMix).Assembly, "assembly");
      Assert.That (context.Mixins[typeof (MixinForGlobalMix)].Origin, Is.EqualTo (expectedOrigin));
    }

    [Test]
    public void AdditionalDependenciesAreAnalyzed ()
    {
      ClassContext context = MixinConfiguration.ActiveConfiguration.GetContext (typeof (TargetClassForGlobalMix));
      Assert.That (context.Mixins[typeof (MixinForGlobalMix)].ExplicitDependencies, Has.Member(typeof (AdditionalDependencyForGlobalMix)));
    }

    [Test]
    public void SuppressedMixinsAreAnalyzed ()
    {
      ClassContext context = MixinConfiguration.ActiveConfiguration.GetContext (typeof (TargetClassForGlobalMix));
      Assert.That (context.Mixins.ContainsKey (typeof (SuppressedMixinForGlobalMix)), Is.False);
    }

    [Test]
    public void DuplicatesAreIgnored ()
    {
      // For some reason, the C# compiler will strip duplicate instances of MixAttribute from the assembly, so in order to test duplicate removal,
      // we need to use generated assemblies.

      var assemblyBuilder1 = DefineDynamicAssemblyWithMixAttribute ("Test1", typeof (TargetClassForGlobalMix), typeof (MixinForGlobalMix));
      var assemblyBuilder2 = DefineDynamicAssemblyWithMixAttribute ("Test2", typeof (TargetClassForGlobalMix), typeof (MixinForGlobalMix));

      var mixinConfiguration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (assemblyBuilder1, assemblyBuilder2);

      ClassContext context = mixinConfiguration.GetContext (typeof (TargetClassForGlobalMix));
      Assert.That (context.Mixins.ContainsKey (typeof (MixinForGlobalMix)), Is.True);
      Assert.That (context.Mixins.Count, Is.EqualTo (1));
    }

    private static AssemblyBuilder DefineDynamicAssemblyWithMixAttribute (string assemblyName, Type targetType, Type mixinType)
    {
      var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly (new AssemblyName (assemblyName), AssemblyBuilderAccess.Run);

      var constructor = typeof (MixAttribute).GetConstructor (new[] { typeof (Type), typeof (Type) });
      var customAttributeBuilder = new CustomAttributeBuilder (constructor, new[] { targetType, mixinType });
      assemblyBuilder.SetCustomAttribute (customAttributeBuilder);

      // RM-
      var moduleBuilder = assemblyBuilder.DefineDynamicModule (assemblyName + ".dll");
      moduleBuilder.DefineType ("Dummy", TypeAttributes.Public, typeof (object)).CreateType();
      return assemblyBuilder;
    }
  }
}
