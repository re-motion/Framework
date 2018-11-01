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
using NUnit.Framework;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.IntegrationTests.Ordering
{
  [TestFixture]
  public class BigTestDomainScenarioTest : OrderingTestBase
  {
    public static Type[] ExpectedBaseType7OrderedMixinTypesSmall
    {
      get
      {
        return new[]
               {
                   typeof (BT7Mixin0),
                   typeof (BT7Mixin10),
                   typeof (BT7Mixin5),
                   typeof (BT7Mixin2),
                   typeof (BT7Mixin9),
                   typeof (BT7Mixin3),
                   typeof (BT7Mixin1)
               };
      }
    }

    [Test]
    public void MixinDefinitionsAreSortedCorrectlySmall ()
    {
      var bt7 = BuildMixedInstanceWithActiveConfiguration<BaseType7>();
      Assert.That (
          bt7.One (5),
          Is.EqualTo (
              "BT7Mixin0.One(5)-BT7Mixin2.One(5)"
              + "-BT7Mixin3.One(5)-BT7Mixin1.BT7Mixin1Specific"
              + "-BaseType7.Three"
              + "-BT7Mixin2.Three"
              + "-BaseType7.Three-BT7Mixin1.One(5)-BaseType7.One(5)"
              + "-BT7Mixin3.One(5)-BT7Mixin1.BT7Mixin1Specific"
              + "-BaseType7.Three"
              + "-BT7Mixin2.Three"
              + "-BaseType7.Three-BT7Mixin1.One(5)-BaseType7.One(5)"
              + "-BaseType7.Two-BT7Mixin2.Two"));

      Assert.That (
          bt7.One ("foo"),
          Is.EqualTo (
              "BT7Mixin0.One(foo)-BT7Mixin2.One(foo)"
              + "-BT7Mixin3.One(foo)-BT7Mixin1.BT7Mixin1Specific"
              + "-BaseType7.Three"
              + "-BT7Mixin2.Three"
              + "-BaseType7.Three-BT7Mixin1.One(foo)-BaseType7.One(foo)"
              + "-BT7Mixin3.One(foo)-BT7Mixin1.BT7Mixin1Specific"
              + "-BaseType7.Three"
              + "-BT7Mixin2.Three"
              + "-BaseType7.Three-BT7Mixin1.One(foo)-BaseType7.One(foo)"
              + "-BaseType7.Two-BT7Mixin2.Two"));

      Assert.That (bt7.Two (), Is.EqualTo ("BT7Mixin2.Two"));
      Assert.That (bt7.Three (), Is.EqualTo ("BT7Mixin2.Three-BaseType7.Three"));
      Assert.That (bt7.Four (), Is.EqualTo ("BT7Mixin2.Four-BaseType7.Four-BT7Mixin9.Five-BaseType7.Five-BaseType7.NotOverridden"));
      Assert.That (bt7.Five (), Is.EqualTo ("BT7Mixin9.Five-BaseType7.Five"));

      TargetClassDefinition targetClassDefinition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType7));
      Assert.That (targetClassDefinition.Mixins.Count, Is.EqualTo (7));

      // This part is fixed, independent of the algorithm:

      // Group 1 with internal ordering
      CheckRelativeMixinOrdering (
          targetClassDefinition,
          typeof (BT7Mixin0),
          typeof (BT7Mixin2),
          typeof (BT7Mixin3),
          typeof (BT7Mixin1));

      // Group 2 with internal ordering
      CheckRelativeMixinOrdering (
          targetClassDefinition,
          typeof (BT7Mixin10),
          typeof (BT7Mixin9));

      // Group 3 consists of just BT7Mixin5

      // The three groups must be ordered lexicographically
      CheckRelativeMixinOrdering (
          targetClassDefinition,
          typeof (BT7Mixin0),
          typeof (BT7Mixin10),
          typeof (BT7Mixin5));

      // This part depends on the algorithm:
      Assert.That (targetClassDefinition.Mixins.Select (m => m.Type), Is.EqualTo (ExpectedBaseType7OrderedMixinTypesSmall));
    }

    [Test]
    public void MixinDefinitionsAreSortedCorrectlyGrand ()
    {
      using (MixinConfiguration
          .BuildFromActive()
          .ForClass<BaseType7>()
          .Clear()
          .AddMixins (
              typeof (BT7Mixin0),
              typeof (BT7Mixin1),
              typeof (BT7Mixin2),
              typeof (BT7Mixin3),
              typeof (BT7Mixin4),
              typeof (BT7Mixin5),
              typeof (BT7Mixin6),
              typeof (BT7Mixin7),
              typeof (BT7Mixin8),
              typeof (BT7Mixin9),
              typeof (BT7Mixin10))
          .EnterScope())
      {
        CheckGrandOrdering();
      }

      using (MixinConfiguration
          .BuildFromActive()
          .ForClass<BaseType7>()
          .Clear()
          .AddMixins (
              typeof (BT7Mixin10),
              typeof (BT7Mixin9),
              typeof (BT7Mixin8),
              typeof (BT7Mixin7),
              typeof (BT7Mixin6),
              typeof (BT7Mixin5),
              typeof (BT7Mixin4),
              typeof (BT7Mixin3),
              typeof (BT7Mixin2),
              typeof (BT7Mixin1),
              typeof (BT7Mixin0))
          .EnterScope())
      {
        CheckGrandOrdering();
      }
      using (MixinConfiguration
          .BuildFromActive()
          .ForClass<BaseType7>()
          .Clear()
          .AddMixins (
              typeof (BT7Mixin5),
              typeof (BT7Mixin8),
              typeof (BT7Mixin9),
              typeof (BT7Mixin2),
              typeof (BT7Mixin1),
              typeof (BT7Mixin10),
              typeof (BT7Mixin4),
              typeof (BT7Mixin0),
              typeof (BT7Mixin6),
              typeof (BT7Mixin3),
              typeof (BT7Mixin7))
          .EnterScope())
      {
        CheckGrandOrdering();
      }
    }

    [Test]
    public void ThrowsIfConnectedMixinsCannotBeSorted ()
    {
      using (
          MixinConfiguration
              .BuildFromActive ()
              .ForClass<BaseType7> ()
              .Clear ()
              .AddMixins (typeof (BT7Mixin0), typeof (BT7Mixin4), typeof (BT7Mixin6), typeof (BT7Mixin7), typeof (BT7Mixin2), typeof (BT7Mixin5))
              .EnterScope ())
      {
        CheckOrderingException (
            () => DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType7)),
            typeof (BaseType7),
            Tuple.Create (new[] { typeof (BT7Mixin0), typeof (BT7Mixin4), typeof (BT7Mixin6) }, "One"));
      }
    }

    private void CheckGrandOrdering ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass<BaseType7> ()
          .EnsureMixin (typeof (BT7Mixin0)).WithDependency<IBT7Mixin7> ()
          .EnsureMixin (typeof (BT7Mixin7)).WithDependency<IBT7Mixin4> ()
          .EnsureMixin (typeof (BT7Mixin4)).WithDependency<IBT7Mixin6> ()
          .EnsureMixin (typeof (BT7Mixin6)).WithDependency<IBT7Mixin2> ()
          .EnsureMixin (typeof (BT7Mixin9)).WithDependency<IBT7Mixin8> ()
          .EnterScope ())
      {
        var bt7 = BuildMixedInstanceWithActiveConfiguration<BaseType7>();
        Assert.That (
            bt7.One (7),
            Is.EqualTo (
                "BT7Mixin0.One(7)-BT7Mixin4.One(7)-BT7Mixin6.One(7)-BT7Mixin2.One(7)"
                + "-BT7Mixin3.One(7)-BT7Mixin1.BT7Mixin1Specific"
                + "-BaseType7.Three"
                + "-BT7Mixin2.Three-BaseType7.Three"
                + "-BT7Mixin1.One(7)-BaseType7.One(7)"
                + "-BT7Mixin3.One(7)-BT7Mixin1.BT7Mixin1Specific"
                + "-BaseType7.Three"
                + "-BT7Mixin2.Three-BaseType7.Three"
                + "-BT7Mixin1.One(7)-BaseType7.One(7)"
                + "-BaseType7.Two"
                + "-BT7Mixin2.Two"));

        Assert.That (
            bt7.One ("bar"),
            Is.EqualTo (
                "BT7Mixin0.One(bar)-BT7Mixin4.One(bar)-BT7Mixin6.One(bar)-BT7Mixin2.One(bar)"
                + "-BT7Mixin3.One(bar)-BT7Mixin1.BT7Mixin1Specific"
                + "-BaseType7.Three"
                + "-BT7Mixin2.Three-BaseType7.Three"
                + "-BT7Mixin1.One(bar)-BaseType7.One(bar)"
                + "-BT7Mixin3.One(bar)-BT7Mixin1.BT7Mixin1Specific"
                + "-BaseType7.Three"
                + "-BT7Mixin2.Three-BaseType7.Three"
                + "-BT7Mixin1.One(bar)-BaseType7.One(bar)"
                + "-BaseType7.Two"
                + "-BT7Mixin2.Two"));

        Assert.That (bt7.Two (), Is.EqualTo ("BT7Mixin2.Two"));
        Assert.That (bt7.Three (), Is.EqualTo ("BT7Mixin2.Three-BaseType7.Three"));
        Assert.That (bt7.Four (), Is.EqualTo ("BT7Mixin2.Four-BaseType7.Four-BT7Mixin9.Five-BT7Mixin8.Five-BaseType7.Five-BaseType7.NotOverridden"));
        Assert.That (bt7.Five (), Is.EqualTo ("BT7Mixin9.Five-BT7Mixin8.Five-BaseType7.Five"));

        var targetClassDefinition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType7));
        Assert.That (targetClassDefinition.Mixins.Count, Is.EqualTo (11));

        // This part is fixed, independent of the algorithm:

        // Group 1 with internal ordering
        CheckRelativeMixinOrdering (
            targetClassDefinition,
            typeof (BT7Mixin0),
            typeof (BT7Mixin7),
            typeof (BT7Mixin4),
            typeof (BT7Mixin6),
            typeof (BT7Mixin2),
            typeof (BT7Mixin3),
            typeof (BT7Mixin1));

        // Group 2 with internal ordering
        CheckRelativeMixinOrdering (
            targetClassDefinition,
            typeof (BT7Mixin10),
            typeof (BT7Mixin9),
            typeof (BT7Mixin8));

        // Group 3 consists of just BT7Mixin5

        // This part depends on the algorithm:
        var expectedBaseType7OrderedMixinTypesGrand =
            new[]
            {
                typeof (BT7Mixin0), 
                typeof (BT7Mixin10), 
                typeof (BT7Mixin5), 
                typeof (BT7Mixin7),
                typeof (BT7Mixin9), 
                typeof (BT7Mixin4), 
                typeof (BT7Mixin8), 
                typeof (BT7Mixin6),
                typeof (BT7Mixin2),
                typeof (BT7Mixin3),
                typeof (BT7Mixin1)
            };
        Assert.That (targetClassDefinition.Mixins.Select (m => m.Type), Is.EqualTo (expectedBaseType7OrderedMixinTypesGrand));
      }
    }

    private static void CheckRelativeMixinOrdering (TargetClassDefinition targetClassDefinition, params Type[] mixinTypes)
    {
      var group = targetClassDefinition.Mixins.Where (m => mixinTypes.Contains (m.Type)).OrderBy (m => m.MixinIndex);
      Assert.That (group.Select (m => m.Type), Is.EqualTo (mixinTypes));
    }
  }
}