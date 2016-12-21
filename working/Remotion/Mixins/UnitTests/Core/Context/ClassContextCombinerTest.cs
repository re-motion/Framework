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
using Remotion.Mixins.Context;

namespace Remotion.Mixins.UnitTests.Core.Context
{
  [TestFixture]
  public class ClassContextCombinerTest
  {
    private ClassContextCombiner _combiner;
    private ClassContext _context1;
    private ClassContext _context2;

    [SetUp]
    public void SetUp ()
    {
      _combiner = new ClassContextCombiner ();
      _context1 = new ClassContext (typeof (object), new MixinContext[0], new[] { typeof (int), typeof (float) });
      _context2 = ClassContextObjectMother.Create(typeof (string), new[] { typeof (double), typeof (int) });
    }

    [Test]
    public void Add_NonNull ()
    {
      _combiner.AddIfNotNull (_context1);
      _combiner.AddIfNotNull (_context2);
      Assert.That (_combiner.CollectedContexts, Is.EquivalentTo (new[] {_context1, _context2}));
    }

    [Test]
    public void Add_Null ()
    {
      _combiner.AddIfNotNull (null);
      Assert.That (_combiner.CollectedContexts, Is.Empty);
    }

    [Test]
    public void AddRange_NonNull ()
    {
      _combiner.AddRangeAllowingNulls (new[] { _context1, _context2 });
      Assert.That (_combiner.CollectedContexts, Is.EquivalentTo (new[] { _context1, _context2 }));
    }

    [Test]
    public void AddRange_Null ()
    {
      _combiner.AddRangeAllowingNulls (new[] { _context1, null });
      Assert.That (_combiner.CollectedContexts, Is.EquivalentTo (new[] { _context1 }));
    }

    [Test]
    public void GetCombinedContexts_Null ()
    {
      Assert.That (_combiner.GetCombinedContexts(typeof (int)), Is.Null);
    }

    [Test]
    public void GetCombinedContexts_One ()
    {
      _combiner.AddIfNotNull (_context1);
      ClassContext result = _combiner.GetCombinedContexts (typeof (int));
      Assert.That (result.Type, Is.EqualTo (typeof (int)));
      Assert.That (result.ComposedInterfaces, Is.EquivalentTo (_context1.ComposedInterfaces));
    }

    [Test]
    public void GetCombinedContexts_Many ()
    {
      _combiner.AddIfNotNull (_context1);
      _combiner.AddIfNotNull (_context2);

      ClassContext result = _combiner.GetCombinedContexts (typeof (int));
      Assert.That (result.Type, Is.EqualTo (typeof (int)));

      var expectedInterfaces = _context1.ComposedInterfaces.Union (_context2.ComposedInterfaces);
      Assert.That (result.ComposedInterfaces, Is.EquivalentTo (expectedInterfaces));
    }
  }
}
