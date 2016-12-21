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
using Remotion.TypePipe;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class DependencyTest : CodeGenerationBaseTest
  {
    [Test]
    public void CircularTargetCallDependenciesWork ()
    {
      using (MixinConfiguration.BuildFromActive()
          .ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinWithCircularTargetCallDependency1), typeof (MixinWithCircularTargetCallDependency2))
          .EnterScope())
      {
        object o = ObjectFactory.Create<NullTarget> (ParamList.Empty);
        var c1 = (ICircular2) o;
        Assert.That (c1.Circular12 (), Is.EqualTo ("MixinWithCircularTargetCallDependency2.Circular12-MixinWithCircularTargetCallDependency1.Circular1-"
                                                   + "MixinWithCircularTargetCallDependency2.Circular2"));
      }
    }

    [Test]
    public void ThisCallToClassImplementingInternalInterface ()
    {
      ClassImplementingInternalInterface ciii = ObjectFactory.Create<ClassImplementingInternalInterface> (ParamList.Empty);
      var mixin = Mixin.Get<MixinWithClassTargetCallImplementingInternalInterface> (ciii);
      Assert.That (mixin.GetStringViaThis (), Is.EqualTo ("ClassImplementingInternalInterface.Foo"));
    }

    [Test]
    public void ThisCallsToIndirectlyRequiredInterfaces ()
    {
      ClassImplementingIndirectRequirements ciir = ObjectFactory.Create<ClassImplementingIndirectRequirements> (ParamList.Empty);
      var mixin = Mixin.Get<MixinWithIndirectRequirements> (ciir);
      Assert.That (mixin.GetStuffViaThis (), Is.EqualTo ("ClassImplementingIndirectRequirements.Method1-ClassImplementingIndirectRequirements.BaseMethod1-"
                                                         + "ClassImplementingIndirectRequirements.Method3"));
    }
  }
}
