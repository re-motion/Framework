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
using NUnit.Framework;
using Remotion.Mixins.Context;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.Context
{
  [TestFixture]
  public class MixinContextCollectionTest
  {
    private MixinContextCollection _collection;
    private MixinContextCollection _genericCollection;

    private MixinContext _mcObject;
    private MixinContext _mcString;
    private MixinContext _mcList;
    private MixinContext _mcGeneric;
    private MixinContext _mcDerived;

    [SetUp]
    public void SetUp ()
    {
      _mcObject = MixinContextObjectMother.Create (mixinType: typeof (object));
      _mcString = MixinContextObjectMother.Create (mixinType: typeof (string));
      _mcList = MixinContextObjectMother.Create (mixinType: typeof (List<int>));
      _mcGeneric = MixinContextObjectMother.Create (mixinType: typeof (DerivedGenericMixin<object>));
      _mcDerived = MixinContextObjectMother.Create (mixinType: typeof (DerivedNullMixin));
      _collection = new MixinContextCollection (new[] { _mcObject, _mcString, _mcList, _mcDerived });
      _genericCollection = new MixinContextCollection (new[] { _mcGeneric });
    }

    [Test]
    public void ContainsAssignableMixin ()
    {
      Assert.That (_collection.ContainsAssignableMixin (typeof (object)), Is.True);
      Assert.That (_collection.ContainsAssignableMixin (typeof (string)), Is.True);
      Assert.That (_collection.ContainsAssignableMixin (typeof (ICollection<int>)), Is.True);
      Assert.That (_collection.ContainsAssignableMixin (typeof (ICollection<string>)), Is.False);
      Assert.That (_collection.ContainsAssignableMixin (typeof (double)), Is.False);
      Assert.That (_collection.ContainsAssignableMixin (typeof (MixinContextCollectionTest)), Is.False);
    }

    [Test]
    public void ContainsOverrideForMixin ()
    {
      Assert.That (_collection.ContainsOverrideForMixin (typeof (NullMixin)), Is.True);
      Assert.That (_collection.ContainsOverrideForMixin (typeof (DerivedDerivedNullMixin)), Is.False);
      Assert.That (_collection.ContainsOverrideForMixin (typeof (object)), Is.True);
      Assert.That (_collection.ContainsOverrideForMixin (typeof (int)), Is.False);
    }

    [Test]
    public void ContainsOverrideForMixin_DerivedAndSpecialized ()
    {
      Assert.That (_genericCollection.ContainsOverrideForMixin (typeof (GenericMixinWithVirtualMethod<>)), Is.True);
      Assert.That (_genericCollection.ContainsOverrideForMixin (typeof (GenericMixinWithVirtualMethod<object>)), Is.True);
      Assert.That (_genericCollection.ContainsOverrideForMixin (typeof (GenericMixinWithVirtualMethod<string>)), Is.True);
      Assert.That (_genericCollection.ContainsOverrideForMixin (typeof (DerivedGenericMixin<>)), Is.True);
      Assert.That (_genericCollection.ContainsOverrideForMixin (typeof (DerivedGenericMixin<object>)), Is.True);
      Assert.That (_genericCollection.ContainsOverrideForMixin (typeof (DerivedGenericMixin<string>)), Is.True);
    }
  }
}
