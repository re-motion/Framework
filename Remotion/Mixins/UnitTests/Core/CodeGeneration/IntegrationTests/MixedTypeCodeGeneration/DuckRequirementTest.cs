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
  public class DuckRequirementTest : CodeGenerationBaseTest
  {
    [Test]
    public void GeneratedTypeImplementsRequiredDuckInterfaces ()
    {
      ClassFulfillingAllMemberRequirementsDuck cfrd = ObjectFactory.Create<ClassFulfillingAllMemberRequirementsDuck> (ParamList.Empty);
      Assert.That (cfrd is IMixinRequiringAllMembersRequirements, Is.True);
      var mixin = Mixin.Get<MixinRequiringAllMembersTargetCall> (cfrd);
      Assert.That (mixin, Is.Not.Null);
      Assert.That (mixin.PropertyViaThis, Is.EqualTo (42));
    }

    [Test]
    public void RequiredTargetCallInterfaceViaDuck ()
    {
      ClassFulfillingAllMemberRequirementsExplicitly cfamre = ObjectFactory.Create<ClassFulfillingAllMemberRequirementsExplicitly> (ParamList.Empty);
      var mixin = Mixin.Get<MixinRequiringAllMembersTargetCall> (cfamre);
      Assert.That (mixin, Is.Not.Null);
      Assert.That (mixin.PropertyViaThis, Is.EqualTo (37));
    }

    [Test]
    public void RequiredBaseInterfaceViaDuck ()
    {
      ClassFulfillingAllMemberRequirements cfamr = ObjectFactory.Create<ClassFulfillingAllMemberRequirements> (ParamList.Empty);
      var mixin = Mixin.Get<MixinRequiringAllMembersNextCall> (cfamr);
      Assert.That (mixin, Is.Not.Null);
      Assert.That (mixin.PropertyViaBase, Is.EqualTo (11));
    }

    [Test]
    public void ThisCallToDuckInterface ()
    {
      BaseTypeWithDuckTargetCallMixin duckTargetCall = ObjectFactory.Create<BaseTypeWithDuckTargetCallMixin> (ParamList.Empty);
      Assert.That (Mixin.Get<DuckTargetCallMixin> (duckTargetCall).CallMethodsOnThis (), Is.EqualTo ("DuckTargetCallMixin.CallMethodsOnThis-DuckTargetCallMixin.MethodImplementedOnBase-BaseTypeWithDuckTargetCallMixin.ProtectedMethodImplementedOnBase"));
    }

  }
}
