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
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.Suppression;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.Context.Suppression
{
  [TestFixture]
  public class MixinTreeSuppressionRuleTest
  {
    [Test]
    public void RemoveAffectedMixins_LeavesUnrelatedType ()
    {
      var rule = new MixinTreeSuppressionRule (typeof (string));
      var dictionary = CreateContextDictionary (typeof (int), typeof (double));

      rule.RemoveAffectedMixins (dictionary);

      Assert.That (dictionary.Keys, Is.EquivalentTo (new[] { typeof (int), typeof (double) }));
    }

    [Test]
    public void RemoveAffectedMixins_RemovesSameType ()
    {
      var rule = new MixinTreeSuppressionRule (typeof (string));
      var dictionary = CreateContextDictionary (typeof (string), typeof (double));

      rule.RemoveAffectedMixins (dictionary);

      Assert.That (dictionary.Keys, Is.EquivalentTo (new[] { typeof (double) }));
    }

    [Test]
    public void RemoveAffectedMixins_RemovesDerivedType ()
    {
      var rule = new MixinTreeSuppressionRule (typeof (object));
      var dictionary = CreateContextDictionary (typeof (string), typeof (double));

      rule.RemoveAffectedMixins (dictionary);

      Assert.That (dictionary.Keys, Is.Empty);
    }

    [Test]
    public void RemoveAffectedMixins_KeepsBaseType ()
    {
      var rule = new MixinTreeSuppressionRule (typeof (string));
      var dictionary = CreateContextDictionary (typeof (object), typeof (double));

      rule.RemoveAffectedMixins (dictionary);

      Assert.That (dictionary.Keys, Is.EquivalentTo (new[] { typeof (object), typeof (double) }));
    }

    [Test]
    public void RemoveAffectedMixins_RemovesGenericTypeSpecialization ()
    {
      var rule = new MixinTreeSuppressionRule (typeof (List<>));
      var dictionary = CreateContextDictionary (typeof (List<string>), typeof (double));

      rule.RemoveAffectedMixins (dictionary);

      Assert.That (dictionary.Keys, Is.EquivalentTo (new[] { typeof (double) }));
    }

    [Test]
    public void RemoveAffectedMixins_RemovesDerivedGenericTypeSpecialization ()
    {
      var rule = new MixinTreeSuppressionRule (typeof (GenericMixinWithVirtualMethod<>));
      var dictionary = CreateContextDictionary (typeof (DerivedGenericMixin<string>), typeof (double));

      rule.RemoveAffectedMixins (dictionary);

      Assert.That (dictionary.Keys, Is.EquivalentTo (new[] { typeof (double) }));
    }

    private Dictionary<Type, MixinContext> CreateContextDictionary (params Type[] types)
    {
      return types.ToDictionary (type => type, type => MixinContextObjectMother.Create (mixinType: type));
    }
  }
}
