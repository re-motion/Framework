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
using Remotion.Mixins.Definitions;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.Definitions.Building
{
  [TestFixture]
  public class InheritanceDefinitionBuildingIntegrationTest
  {
    [Test]
    public void InheritedIntroducedInterfaces ()
    {
      TargetClassDefinition bt1 =
          DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinIntroducingInheritedInterface));
      Assert.That (bt1.ReceivedInterfaces.ContainsKey (typeof (IMixinIII1)), Is.True);
      Assert.That (bt1.ReceivedInterfaces.ContainsKey (typeof (IMixinIII2)), Is.True);
      Assert.That (bt1.ReceivedInterfaces.ContainsKey (typeof (IMixinIII3)), Is.True);
      Assert.That (bt1.ReceivedInterfaces.ContainsKey (typeof (IMixinIII4)), Is.True);
    }

    [Test]
    public void InheritedFaceDependencies ()
    {
      TargetClassDefinition bt1 =
          DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinTargetCallDependingOnInheritedInterface),
                                                             typeof (MixinIntroducingInheritedInterface));
      Assert.That (bt1.RequiredTargetCallTypes.ContainsKey (typeof (IMixinIII1)), Is.True);
      Assert.That (bt1.RequiredTargetCallTypes.ContainsKey (typeof (IMixinIII2)), Is.True);
      Assert.That (bt1.RequiredTargetCallTypes.ContainsKey (typeof (IMixinIII3)), Is.True);
      Assert.That (bt1.RequiredTargetCallTypes.ContainsKey (typeof (IMixinIII4)), Is.True);

      MixinDefinition m1 = bt1.Mixins[typeof (MixinTargetCallDependingOnInheritedInterface)];
      Assert.That (m1.TargetCallDependencies.ContainsKey (typeof (IMixinIII1)), Is.True);
      Assert.That (m1.TargetCallDependencies.ContainsKey (typeof (IMixinIII2)), Is.True);
      Assert.That (m1.TargetCallDependencies.ContainsKey (typeof (IMixinIII3)), Is.True);
      Assert.That (m1.TargetCallDependencies.ContainsKey (typeof (IMixinIII4)), Is.True);
    }

    [Test]
    public void InheritedNextCallDependencies ()
    {
      TargetClassDefinition bt1 =
          DefinitionObjectMother.BuildUnvalidatedDefinition (
              typeof (BaseType1),
              typeof (MixinBaseDependingOnInheritedInterface),
              typeof (MixinIntroducingInheritedInterface));
      Assert.That (bt1.RequiredNextCallTypes.ContainsKey (typeof (IMixinIII1)), Is.True);
      Assert.That (bt1.RequiredNextCallTypes.ContainsKey (typeof (IMixinIII2)), Is.True);
      Assert.That (bt1.RequiredNextCallTypes.ContainsKey (typeof (IMixinIII3)), Is.True);
      Assert.That (bt1.RequiredNextCallTypes.ContainsKey (typeof (IMixinIII4)), Is.True);

      MixinDefinition m1 = bt1.Mixins[typeof (MixinBaseDependingOnInheritedInterface)];
      Assert.That (m1.NextCallDependencies.ContainsKey (typeof (IMixinIII1)), Is.True);
      Assert.That (m1.NextCallDependencies.ContainsKey (typeof (IMixinIII2)), Is.True);
      Assert.That (m1.NextCallDependencies.ContainsKey (typeof (IMixinIII3)), Is.True);
      Assert.That (m1.NextCallDependencies.ContainsKey (typeof (IMixinIII4)), Is.True);
    }


  }
}
