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
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Reflection.TypeDiscovery;
using Remotion.TypePipe;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class GeneratedTypeInConfigurationTest : CodeGenerationBaseTest
  {
    private Type _generatedType;

    [TestFixtureSetUp]
    public void TestFixtureSetUp ()
    {
      var generator = new AdHocCodeGenerator("MixedTypeCodeGeneration.GeneratedTypeInConfigurationTest");
      var typeBuilder = generator.CreateType ("GeneratedType");
      generator.AddCustomAttribute (typeof (NonApplicationAssemblyAttribute));
      _generatedType = typeBuilder.CreateType();
      
      var generatedAssemblyPath = generator.Save();
      AddSavedAssembly (generatedAssemblyPath);
    }

    [Test]
    public void GeneratedMixinTypeWorks ()
    {
      using (MixinConfiguration.BuildNew().ForClass<NullTarget>().Clear().AddMixins(_generatedType).EnterScope())
      {
        object instance = ObjectFactory.Create(typeof(NullTarget), ParamList.Empty);
        Assert.That(Mixin.Get(_generatedType, instance), Is.Not.Null);
      }
    }

    [Test]
    public void GeneratedTargetTypeWorks ()
    {
      using (MixinConfiguration.BuildNew().ForClass(_generatedType).Clear().AddMixins(typeof(NullMixin)).EnterScope())
      {
        object instance = ObjectFactory.Create(_generatedType, ParamList.Empty);
        Assert.That(Mixin.Get(typeof(NullMixin), instance), Is.Not.Null);
      }
    }
  }
}
