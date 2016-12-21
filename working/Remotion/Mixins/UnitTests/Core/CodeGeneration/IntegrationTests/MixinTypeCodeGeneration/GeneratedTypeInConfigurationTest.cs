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
using Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.MixinTypeCodeGeneration.TestDomain;
using Remotion.Reflection.TypeDiscovery;
using Remotion.TypePipe;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.MixinTypeCodeGeneration
{
  [TestFixture]
  public class GeneratedTypeInConfigurationTest : CodeGenerationBaseTest
  {
    private Type _generatedMixinType;
    private Type _generatedTargetTypeWithMethodOverride;

    [TestFixtureSetUp]
    public void TestFixtureSetUp ()
    {
      var generator = new AdHocCodeGenerator ("MixinTypeCodeGeneration.GeneratedTypeInConfigurationTest");
      generator.AddCustomAttribute(typeof(NonApplicationAssemblyAttribute));

      var generatedMixinTypeBuilder = generator.CreateType("GeneratedMixinType", typeof (Mixin<object>));
      _generatedMixinType = generatedMixinTypeBuilder.CreateType();

      var generatedTargetTypeWithMethodOverrideBuilder = generator.CreateType ("GeneratedTargetType");
      var methodBuilder = generator.CreateMethod (
          generatedTargetTypeWithMethodOverrideBuilder, "ToString", MethodAttributes.Public, typeof (string), Type.EmptyTypes);
      var gen = methodBuilder.GetILGenerator();
      gen.Emit (OpCodes.Ldstr, "Generated _and_ overridden");
      gen.Emit (OpCodes.Ret);
      methodBuilder.SetCustomAttribute (generator.CreateCustomAttributeBuilder (typeof (OverrideMixinAttribute)));
      _generatedTargetTypeWithMethodOverride = generatedTargetTypeWithMethodOverrideBuilder.CreateType();

      var generatedAssemblyPath = generator.Save();
      AddSavedAssembly(generatedAssemblyPath);
    }

    [Test]
    public void GeneratedMixinTypeWithOverriddenMethodWorks ()
    {
      using (MixinConfiguration.BuildNew().ForClass<ClassOverridingMixinMethod> ().Clear().AddMixins (_generatedMixinType).EnterScope())
      {
        object instance = ObjectFactory.Create (typeof (ClassOverridingMixinMethod), ParamList.Empty);
        Assert.That (Mixin.Get (_generatedMixinType, instance).ToString (), Is.EqualTo ("Overridden!"));
      }
    }

    [Test]
    public void GeneratedTargetTypeOverridingMixinMethodWorks ()
    {
      using (MixinConfiguration.BuildNew().ForClass(_generatedTargetTypeWithMethodOverride).Clear().AddMixins(typeof(SimpleMixin)).EnterScope())
      {
        object instance = ObjectFactory.Create(_generatedTargetTypeWithMethodOverride, ParamList.Empty);
        Assert.That (Mixin.Get<SimpleMixin> (instance).ToString (), Is.EqualTo ("Generated _and_ overridden"));
      }
    }
  }
}
