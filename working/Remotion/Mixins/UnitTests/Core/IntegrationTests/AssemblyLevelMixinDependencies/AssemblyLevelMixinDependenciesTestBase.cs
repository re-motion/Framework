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

namespace Remotion.Mixins.UnitTests.Core.IntegrationTests.AssemblyLevelMixinDependencies
{
  public class AssemblyLevelMixinDependenciesTestBase
  {
    private MixinConfiguration _previousConfiguration;

    [SetUp]
    public virtual void SetUp ()
    {
      _previousConfiguration = MixinConfiguration.HasActiveConfiguration ? MixinConfiguration.ActiveConfiguration : null;
    }

    [TearDown]
    public virtual void TearDown ()
    {
      MixinConfiguration.SetActiveConfiguration (_previousConfiguration);
    }


    protected void PrepareMixinConfigurationWithAttributeDeclarations (params AdditionalMixinDependencyAttribute[] attributes)
    {
      var assemblyWithDeclarations = CreateAssemblyWithAdditionalAttributeDeclarations (attributes);

      var builder = new DeclarativeConfigurationBuilder (null);
      builder.AddAssembly (GetType().Assembly);
      builder.AddAssembly (assemblyWithDeclarations);

      MixinConfiguration.SetActiveConfiguration (builder.BuildConfiguration());
    }

    private Assembly CreateAssemblyWithAdditionalAttributeDeclarations (params AdditionalMixinDependencyAttribute[] attributes)
    {
      var ctor = typeof (AdditionalMixinDependencyAttribute).GetConstructor (new[] { typeof (Type), typeof (Type), typeof (Type) });
      Assert.That (ctor, Is.Not.Null, "AdditionalMixinDependencyAttribute ctor not found.");

      var assemblyName = "TestAssembly_" + GetType().Name;
      var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly (new AssemblyName (assemblyName), AssemblyBuilderAccess.Run);
      foreach (var attribute in attributes)
      {
        var attributeBuilder = new CustomAttributeBuilder (ctor, new[] { attribute.TargetType, attribute.DependentMixin, attribute.Dependency });
        assemblyBuilder.SetCustomAttribute (attributeBuilder);
      }

      // Required due to RM-5136
      var moduleBuilder = assemblyBuilder.DefineDynamicModule (assemblyBuilder + ".dll");
      moduleBuilder.DefineType ("Dummy", TypeAttributes.Public, typeof (object)).CreateType();

      return assemblyBuilder;
    }
  }
}