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
using NUnit.Framework;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.Definitions.Building
{
  [TestFixture]
  public class OverrideAttributeHierarchyIntegrationTest
  {
    [Test]
    public void BaseWithOverrideAttributes ()
    {
      MethodInfo method = typeof (TargetForOverridesAndShadowing).GetMethod ("Method");
      PropertyInfo property = typeof (TargetForOverridesAndShadowing).GetProperty ("Property");
      EventInfo eve = typeof (TargetForOverridesAndShadowing).GetEvent ("Event");

      TargetClassDefinition def1 =
          DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (TargetForOverridesAndShadowing),typeof (BaseWithOverrideAttributes));
      MixinDefinition mix1 = def1.Mixins[typeof (BaseWithOverrideAttributes)];
      Assert.That (mix1, Is.Not.Null);

      Assert.That (def1.Methods[method].Overrides.Count, Is.EqualTo (1));
      Assert.That (def1.Methods[method].Overrides[typeof (BaseWithOverrideAttributes)].DeclaringClass, Is.SameAs (mix1));

      Assert.That (def1.Properties[property].Overrides.Count, Is.EqualTo (1));
      Assert.That (def1.Properties[property].Overrides[typeof (BaseWithOverrideAttributes)].DeclaringClass, Is.SameAs (mix1));

      Assert.That (def1.Events[eve].Overrides.Count, Is.EqualTo (1));
      Assert.That (def1.Events[eve].Overrides[typeof (BaseWithOverrideAttributes)].DeclaringClass, Is.SameAs (mix1));
    }

    [Test]
    public void DerivedWithOverridesWithoutAttributes ()
    {
      MethodInfo method = typeof (TargetForOverridesAndShadowing).GetMethod ("Method");
      PropertyInfo property = typeof (TargetForOverridesAndShadowing).GetProperty ("Property");
      EventInfo eve = typeof (TargetForOverridesAndShadowing).GetEvent ("Event");

      TargetClassDefinition def1 = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (TargetForOverridesAndShadowing),
                                                                                      typeof (DerivedWithoutOverrideAttributes));
      MixinDefinition mix1 = def1.Mixins[typeof (DerivedWithoutOverrideAttributes)];
      Assert.That (mix1, Is.Not.Null);

      Assert.That (def1.Methods[method].Overrides.Count, Is.EqualTo (1));
      Assert.That (def1.Methods[method].Overrides[typeof (DerivedWithoutOverrideAttributes)].DeclaringClass, Is.SameAs (mix1));

      Assert.That (def1.Properties[property].Overrides.Count, Is.EqualTo (1));
      Assert.That (def1.Properties[property].Overrides[typeof (DerivedWithoutOverrideAttributes)].DeclaringClass, Is.SameAs (mix1));

      Assert.That (def1.Events[eve].Overrides.Count, Is.EqualTo (1));
      Assert.That (def1.Events[eve].Overrides[typeof (DerivedWithoutOverrideAttributes)].DeclaringClass, Is.SameAs (mix1));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Mixin .* overrides method .* twice", MatchType = MessageMatch.Regex)]
    public void DerivedWithNewAdditionalOverrides ()
    {
      TargetClassDefinition def1 = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (TargetForOverridesAndShadowing),
                                                                                      typeof (DerivedNewWithAdditionalOverrideAttributes));
    }

    [Test]
    public void BaseWithoutAttributes ()
    {
      MethodInfo method = typeof (TargetForOverridesAndShadowing).GetMethod ("Method");
      PropertyInfo property = typeof (TargetForOverridesAndShadowing).GetProperty ("Property");
      EventInfo eve = typeof (TargetForOverridesAndShadowing).GetEvent ("Event");

      TargetClassDefinition def1 = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (TargetForOverridesAndShadowing),
                                                                                      typeof (BaseWithoutOverrideAttributes));
      MixinDefinition mix1 = def1.Mixins[typeof (BaseWithoutOverrideAttributes)];
      Assert.That (mix1, Is.Not.Null);

      Assert.That (def1.Methods[method].Overrides.Count, Is.EqualTo (0));
      Assert.That (def1.Properties[property].Overrides.Count, Is.EqualTo (0));
      Assert.That (def1.Events[eve].Overrides.Count, Is.EqualTo (0));
    }

    [Test]
    public void DerivedWithNewAttributes ()
    {
      MethodInfo method = typeof (TargetForOverridesAndShadowing).GetMethod ("Method");
      PropertyInfo property = typeof (TargetForOverridesAndShadowing).GetProperty ("Property");
      EventInfo eve = typeof (TargetForOverridesAndShadowing).GetEvent ("Event");

      TargetClassDefinition def1 = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (TargetForOverridesAndShadowing),
                                                                                      typeof (DerivedNewWithOverrideAttributes));
      MixinDefinition mix1 = def1.Mixins[typeof (DerivedNewWithOverrideAttributes)];
      Assert.That (mix1, Is.Not.Null);

      Assert.That (def1.Methods[method].Overrides.Count, Is.EqualTo (1));
      Assert.That (def1.Methods[method].Overrides[typeof (DerivedNewWithOverrideAttributes)].DeclaringClass, Is.SameAs (mix1));
      Assert.That (def1.Properties[property].Overrides.Count, Is.EqualTo (1));
      Assert.That (def1.Properties[property].Overrides[typeof (DerivedNewWithOverrideAttributes)].DeclaringClass, Is.SameAs (mix1));
      Assert.That (def1.Events[eve].Overrides.Count, Is.EqualTo (1));
      Assert.That (def1.Events[eve].Overrides[typeof (DerivedNewWithOverrideAttributes)].DeclaringClass, Is.SameAs (mix1));
    }

    [Test]
    public void DerivedNewWithoutAttributes ()
    {
      MethodInfo method = typeof (TargetForOverridesAndShadowing).GetMethod ("Method");
      PropertyInfo property = typeof (TargetForOverridesAndShadowing).GetProperty ("Property");
      EventInfo eve = typeof (TargetForOverridesAndShadowing).GetEvent ("Event");

      TargetClassDefinition def1 = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (TargetForOverridesAndShadowing),
                                                                                      typeof (DerivedNewWithoutOverrideAttributes));
      MixinDefinition mix1 = def1.Mixins[typeof (DerivedNewWithoutOverrideAttributes)];
      Assert.That (mix1, Is.Not.Null);

      Assert.That (def1.Methods[method].Overrides.Count, Is.EqualTo (0));
      Assert.That (def1.Properties[property].Overrides.Count, Is.EqualTo (0));
      Assert.That (def1.Events[eve].Overrides.Count, Is.EqualTo (0));
    }
  }
}
