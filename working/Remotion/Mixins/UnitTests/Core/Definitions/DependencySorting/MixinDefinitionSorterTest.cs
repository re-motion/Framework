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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Definitions.Building.DependencySorting;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.ServiceLocation;

namespace Remotion.Mixins.UnitTests.Core.Definitions.DependencySorting
{
  [TestFixture]
  public class MixinDefinitionSorterTest
  {
    private IMixinDefinitionSorter _sorter;

    [SetUp]
    public void SetUp ()
    {
      _sorter = new MixinDefinitionSorter();
    }

    [Test]
    public void Singleton_RegisteredAsDefaultInterfaceImplementation ()
    {
      var instance = SafeServiceLocator.Current.GetInstance<IMixinDefinitionSorter> ();
      Assert.That (instance, Is.TypeOf<MixinDefinitionSorter> ());

      Assert.That (instance, Is.SameAs (SafeServiceLocator.Current.GetInstance<IMixinDefinitionSorter> ()));
    }

    [Test]
    public void SortMixins_SortsAccordingToDependencies_OrderingFullyDefinedByDependencies ()
    {
      // NullMixin2 -> NullMixin3 -> NullMixin
      var targetClassDefinition = CreateTargetClassDefinition (
          Tuple.Create (typeof (NullMixin), false, Type.EmptyTypes),
          Tuple.Create (typeof (NullMixin2), false, new[] { typeof (NullMixin3) }),
          Tuple.Create (typeof (NullMixin3), false, new[] { typeof (NullMixin) }));

      var result = _sorter.SortMixins (targetClassDefinition.Mixins);

      CheckResult (result, typeof (NullMixin2), typeof (NullMixin3), typeof (NullMixin));
    }

    [Test]
    public void SortMixins_CyclicDependencies ()
    {
      // NullMixin -> NullMixin2 -> NullMixin3 -> NullMixin, NullMixin4
      var targetClassDefinition = CreateTargetClassDefinition (
          Tuple.Create (typeof (NullMixin), false, new[] { typeof (NullMixin2) }),
          Tuple.Create (typeof (NullMixin2), false, new[] { typeof (NullMixin3) }),
          Tuple.Create (typeof (NullMixin3), false, new[] { typeof (NullMixin), typeof (NullMixin4) }),
          Tuple.Create (typeof (NullMixin4), false, Type.EmptyTypes));

      Assert.That (
          () =>_sorter.SortMixins (targetClassDefinition.Mixins).ToArray(),
          Throws.InvalidOperationException.With.Message.EqualTo (
                  "The following group of mixins contains circular dependencies:" + Environment.NewLine
                  + "'Remotion.Mixins.UnitTests.Core.TestDomain.NullMixin'," + Environment.NewLine
                  + "'Remotion.Mixins.UnitTests.Core.TestDomain.NullMixin2'," + Environment.NewLine
                  + "'Remotion.Mixins.UnitTests.Core.TestDomain.NullMixin3'," + Environment.NewLine
                  + "'Remotion.Mixins.UnitTests.Core.TestDomain.NullMixin4'."));
    }

    [Test]
    public void SortMixins_MultipleRoots_WithoutOverlaps_AreSortedLexicographically ()
    {
      // NullMixin -> NullMixin2 / NullMixin3 -> NullMixin4
      var targetClassDefinition = CreateTargetClassDefinition (
          Tuple.Create (typeof (NullMixin2), false, Type.EmptyTypes),
          Tuple.Create (typeof (NullMixin3), false, new[] { typeof (NullMixin4) }),
          Tuple.Create (typeof (NullMixin), false, new[] { typeof (NullMixin2) }),
          Tuple.Create (typeof (NullMixin4), false, Type.EmptyTypes));

      var result = _sorter.SortMixins (targetClassDefinition.Mixins);

      CheckResult (result, typeof (NullMixin), typeof (NullMixin3), typeof (NullMixin2), typeof (NullMixin4));
    }

    [Test]
    public void SortMixins_MultipleRoots_WithOverrides_ButNoOverlaps_AreSortedLexicographically ()
    {
      // NullMixin / NullMixin2
      var targetClassDefinition = CreateTargetClassDefinition (
          Tuple.Create (typeof (NullMixin2), false, Type.EmptyTypes),
          Tuple.Create (typeof (NullMixin), false, Type.EmptyTypes));

      AddOverride (targetClassDefinition, typeof (NullMixin), "ToString");
      AddOverride (targetClassDefinition, typeof (NullMixin2), "GetHashCode");

      var result = _sorter.SortMixins (targetClassDefinition.Mixins);

      CheckResult (result, typeof (NullMixin), typeof (NullMixin2));
    }

    [Test]
    public void SortMixins_MultipleRoots_WithOverlaps_AreSortedLexicographically_IfAcceptsAlphabeticalOrderingIsAccepted ()
    {
      // NullMixin / NullMixin2 / NullMixin3
      var targetClassDefinition = CreateTargetClassDefinition (
          Tuple.Create (typeof (NullMixin3), true, Type.EmptyTypes),
          Tuple.Create (typeof (NullMixin), true, Type.EmptyTypes),
          Tuple.Create (typeof (NullMixin2), true, Type.EmptyTypes));
      
      AddOverride (targetClassDefinition, typeof (NullMixin), "ToString");
      AddOverride (targetClassDefinition, typeof (NullMixin2), "ToString");
      AddOverride (targetClassDefinition, typeof (NullMixin3), "ToString");

      var result = _sorter.SortMixins (targetClassDefinition.Mixins);

      CheckResult (result, typeof (NullMixin), typeof (NullMixin2), typeof (NullMixin3));
    }

    [Test]
    public void SortMixins_MultipleRoots_WithOverlaps_AreSortedLexicographically_IfAcceptsAlphabeticalOrderingIsDisaccepted_OnlyOnce ()
    {
      // NullMixin / NullMixin2 / NullMixin3
      var targetClassDefinition = CreateTargetClassDefinition (
          Tuple.Create (typeof (NullMixin3), true, Type.EmptyTypes),
          Tuple.Create (typeof (NullMixin), false, Type.EmptyTypes),
          Tuple.Create (typeof (NullMixin2), true, Type.EmptyTypes));

      AddOverride (targetClassDefinition, typeof (NullMixin), "ToString");
      AddOverride (targetClassDefinition, typeof (NullMixin2), "ToString");
      AddOverride (targetClassDefinition, typeof (NullMixin3), "ToString");

      var result = _sorter.SortMixins (targetClassDefinition.Mixins);

      CheckResult (result, typeof (NullMixin), typeof (NullMixin2), typeof (NullMixin3));
    }

    [Test]
    public void SortMixins_MultipleRoots_WithOverlaps_Throws_IfAcceptsAlphabeticalOrderingIsDisaccepted_MoreThanOnce ()
    {
      // NullMixin / NullMixin2 / NullMixin3
      var targetClassDefinition = CreateTargetClassDefinition (
          Tuple.Create (typeof (NullMixin3), true, Type.EmptyTypes),
          Tuple.Create (typeof (NullMixin), false, Type.EmptyTypes),
          Tuple.Create (typeof (NullMixin2), false, Type.EmptyTypes));

      AddOverride (targetClassDefinition, typeof (NullMixin), "ToString");
      AddOverride (targetClassDefinition, typeof (NullMixin2), "ToString");
      AddOverride (targetClassDefinition, typeof (NullMixin3), "ToString");

      Assert.That (
          () => _sorter.SortMixins (targetClassDefinition.Mixins).ToArray(),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "The following mixin groups require a clear base call ordering, but do not provide enough dependency information:"
                  + Environment.NewLine
                  + "{'Remotion.Mixins.UnitTests.Core.TestDomain.NullMixin', "
                  + "'Remotion.Mixins.UnitTests.Core.TestDomain.NullMixin2', "
                  + "'Remotion.Mixins.UnitTests.Core.TestDomain.NullMixin3'} (overriding: 'ToString')." + Environment.NewLine
                  + "Please supply additional dependencies to the mixin definitions, use the AcceptsAlphabeticOrderingAttribute, or adjust the "
                  + "mixin configuration accordingly."));
    }

    [Test]
    public void SortMixins_MultipleRoots_WithOverlaps_Throws_IfAcceptsAlphabeticalOrderingIsDisaccepted_MoreThanOnce_WithMultipleDistinctGroups ()
    {
      // NullMixin / NullMixin2 + NullMixin3 / NullMixin4
      var targetClassDefinition = CreateTargetClassDefinition (
          Tuple.Create (typeof (NullMixin3), false, Type.EmptyTypes),
          Tuple.Create (typeof (NullMixin), false, Type.EmptyTypes),
          Tuple.Create (typeof (NullMixin4), false, Type.EmptyTypes),
          Tuple.Create (typeof (NullMixin2), false, Type.EmptyTypes));

      AddOverride (targetClassDefinition, typeof (NullMixin), "ToString");
      AddOverride (targetClassDefinition, typeof (NullMixin2), "ToString");
      AddOverride (targetClassDefinition, typeof (NullMixin3), "GetHashCode");
      AddOverride (targetClassDefinition, typeof (NullMixin4), "GetHashCode");

      Assert.That (
          () => _sorter.SortMixins (targetClassDefinition.Mixins).ToArray (),
          Throws.InvalidOperationException.With.Message.EqualTo (
                  "The following mixin groups require a clear base call ordering, but do not provide enough dependency information:"
                  + Environment.NewLine
                  + "{'Remotion.Mixins.UnitTests.Core.TestDomain.NullMixin', 'Remotion.Mixins.UnitTests.Core.TestDomain.NullMixin2'} "
                  + "(overriding: 'ToString')," + Environment.NewLine
                  + "{'Remotion.Mixins.UnitTests.Core.TestDomain.NullMixin3', 'Remotion.Mixins.UnitTests.Core.TestDomain.NullMixin4'} "
                  + "(overriding: 'GetHashCode')." + Environment.NewLine
                  + "Please supply additional dependencies to the mixin definitions, use the AcceptsAlphabeticOrderingAttribute, or adjust the "
                  + "mixin configuration accordingly."));
    }

    [Test]
    public void SortMixins_MultipleRoots_WithOverlaps_Throws_IfAcceptsAlphabeticalOrderingIsDisaccepted_MoreThanOnce_WithMultipleEqualGroups ()
    {
      // NullMixin / NullMixin2 + NullMixin3 / NullMixin4
      var targetClassDefinition = CreateTargetClassDefinition (
          Tuple.Create (typeof (NullMixin3), false, Type.EmptyTypes),
          Tuple.Create (typeof (NullMixin), false, Type.EmptyTypes),
          Tuple.Create (typeof (NullMixin4), false, Type.EmptyTypes),
          Tuple.Create (typeof (NullMixin2), false, Type.EmptyTypes));

      AddOverride (targetClassDefinition, typeof (NullMixin), "ToString");
      AddOverride (targetClassDefinition, typeof (NullMixin2), "ToString");
      AddOverride (targetClassDefinition, typeof (NullMixin), "GetHashCode");
      AddOverride (targetClassDefinition, typeof (NullMixin2), "GetHashCode");

      Assert.That (
          () => _sorter.SortMixins (targetClassDefinition.Mixins).ToArray (),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "The following mixin groups require a clear base call ordering, but do not provide enough dependency information:"
              + Environment.NewLine
              + "{'Remotion.Mixins.UnitTests.Core.TestDomain.NullMixin', 'Remotion.Mixins.UnitTests.Core.TestDomain.NullMixin2'} "
              + "(overriding: 'ToString', 'GetHashCode')." + Environment.NewLine
              + "Please supply additional dependencies to the mixin definitions, use the AcceptsAlphabeticOrderingAttribute, or adjust the "
              + "mixin configuration accordingly."));
    }

    private void AddOverride (TargetClassDefinition targetClassDefinition, Type mixinType, string methodName)
    {
      var baseMember = targetClassDefinition.GetAllMethods ().SingleOrDefault (m => m.Name == methodName);
      if (baseMember == null)
        baseMember = DefinitionObjectMother.CreateMethodDefinition (targetClassDefinition, targetClassDefinition.Type.GetMethod (methodName));

      var mixinDefinition = targetClassDefinition.Mixins.Single (m => m.Type == mixinType);
      var overridingMember = DefinitionObjectMother.CreateMethodDefinition (mixinDefinition, mixinType.GetMethod (methodName));
      DefinitionObjectMother.DeclareOverride (overridingMember, baseMember);
    }

    private void CheckResult (IEnumerable<MixinDefinition> result, params Type[] expectedMixinTypes)
    {
      Assert.That (result.Select (m => m.Type), Is.EqualTo (expectedMixinTypes));
    }

    private static TargetClassDefinition CreateTargetClassDefinition (params Tuple<Type, bool, Type[]>[] mixinTypesAndDependencies)
    {
      var targetClassDefinition = DefinitionObjectMother.CreateTargetClassDefinition (typeof (NullTarget));

      foreach (var tuple in mixinTypesAndDependencies)
      {
        DefinitionObjectMother.CreateMixinDefinition (targetClassDefinition, tuple.Item1, tuple.Item2);
      }

      foreach (var tuple in mixinTypesAndDependencies)
      {
        foreach (var dependency in tuple.Item3)
          DefinitionObjectMother.CreateMixinDependencyDefinition (targetClassDefinition.Mixins[tuple.Item1], targetClassDefinition.Mixins[dependency]);
      }

      return targetClassDefinition;
    }
  }
}