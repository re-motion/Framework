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
using Remotion.Validation.Utilities;

namespace Remotion.Validation.UnitTests.Utilities
{
  public class TestContent
  {
    public TestContent (string name)
    {
      Name = name;
      Dependencies = new List<TestContent>();
    }

    public string Name { get; private set; }
    public List<TestContent> Dependencies { get; private set; }

    public TestContent Add (params TestContent[] dependencies)
    {
      Dependencies.AddRange (dependencies);
      return this;
    }
  }

  [TestFixture]
  public class TopologySortLinqExtensionsIntegrationTests
  {
    [Test]
    public void SimpleSort ()
    {
      var a = new TestContent ("a");
      var b = new TestContent ("b").Add (a);
      var c = new TestContent ("c").Add (a, b);
      var d = new TestContent ("d");

      var unsorted = new[] { a, b, d, c };
      var sorted = unsorted.TopologySort (content => content.Dependencies).ToArray();

      Assert.That (sorted.Count(), Is.EqualTo (3));
      Assert.That (sorted[0], Is.EqualTo (new[] { d, c }));
      Assert.That (sorted[1], Is.EqualTo (new[] { b }));
      Assert.That (sorted[2], Is.EqualTo (new[] { a }));
    }

    [Test]
    public void SimpleSortDesc ()
    {
      var a = new TestContent ("a");
      var b = new TestContent ("b").Add (a);
      var c = new TestContent ("c").Add (a, b);
      var d = new TestContent ("d");

      var unsorted = new[] { a, b, d, c };
      var sorted = unsorted.TopologySortDesc (content => content.Dependencies).ToArray();

      Assert.That (sorted.Count(), Is.EqualTo (3));
      Assert.That (sorted[0], Is.EqualTo (new[] { a }));
      Assert.That (sorted[1], Is.EqualTo (new[] { b }));
      Assert.That (sorted[2], Is.EqualTo (new[] { d, c }));
    }

    [Test]
    public void SubSort ()
    {
      var a = new TestContent ("a");
      var b1 = new TestContent ("b1").Add (a);
      var b2 = new TestContent ("b2").Add (a);
      var c1 = new TestContent ("c1").Add (a, b1);
      var c2 = new TestContent ("c2").Add (a, b2);
      var c = new TestContent ("c").Add (a, b1, b2);
      var unsorted = new[] { a, b2, b1, c2, c1, c };
      var sorted = unsorted.TopologySort (content => content.Dependencies, contents => contents.OrderBy (content => content.Name)).ToArray();

      Assert.That (sorted.Count(), Is.EqualTo (3));
      Assert.That (sorted[0], Is.EqualTo (new[] { c, c1, c2 }));
      Assert.That (sorted[1], Is.EqualTo (new[] { b1, b2 }));
      Assert.That (sorted[2], Is.EqualTo (new[] { a }));
    }

    [Test]
    public void SubSortDesc ()
    {
      var a = new TestContent ("a");
      var b1 = new TestContent ("b1").Add (a);
      var b2 = new TestContent ("b2").Add (a);
      var c1 = new TestContent ("c1").Add (a, b1);
      var c2 = new TestContent ("c2").Add (a, b2);
      var c = new TestContent ("c").Add (a, b1, b2);
      var unsorted = new[] { a, b2, b1, c2, c1, c };
      var sorted = unsorted.TopologySortDesc (content => content.Dependencies, contents => contents.OrderBy (content => content.Name)).ToArray();

      Assert.That (sorted.Count(), Is.EqualTo (3));
      Assert.That (sorted[0], Is.EqualTo (new[] { a }));
      Assert.That (sorted[1], Is.EqualTo (new[] { b1, b2 }));
      Assert.That (sorted[2], Is.EqualTo (new[] { c, c1, c2 }));
    }

    [Test]
    public void Sort_Missing_Ignore ()
    {
      var a = new TestContent ("a");
      var b = new TestContent ("b").Add (a);
      var c = new TestContent ("c").Add (a, b);
      var d = new TestContent ("d").Add (a, c);
      var unsorted = new[] { a, b, d };
      var sorted = unsorted.TopologySort (
          content => content.Dependencies,
          contents => contents.OrderBy (cnt => cnt.Name),
          TopologySortMissingDependencyBehavior.Ignore).ToArray();

      Assert.That (sorted.Count(), Is.EqualTo (2));
      Assert.That (sorted[0], Is.EqualTo (new[] { b, d }));
      Assert.That (sorted[1], Is.EqualTo (new[] { a }));
    }

    [Test]
    public void Sort_Missing_Respect ()
    {
      var a = new TestContent ("a");
      var b = new TestContent ("b").Add (a);
      var c = new TestContent ("c").Add (a, b);
      var d = new TestContent ("d").Add (a, c);
      var unsorted = new[] { a, b, d };
      var sorted = unsorted.TopologySort (
          content => content.Dependencies,
          contents => contents.OrderBy (cnt => cnt.Name),
          TopologySortMissingDependencyBehavior.Respect).ToArray();

      Assert.That (sorted.Count(), Is.EqualTo (3));
      Assert.That (sorted[0], Is.EqualTo (new[] { d }));
      Assert.That (sorted[1], Is.EqualTo (new[] { b }));
      Assert.That (sorted[2], Is.EqualTo (new[] { a }));
    }

    [Test]
    public void Sort_Missing_Include ()
    {
      var a = new TestContent ("a");
      var b = new TestContent ("b").Add (a);
      var c = new TestContent ("c").Add (a, b);
      var d = new TestContent ("d").Add (a, c);
      var unsorted = new[] { a, b, d };
      var sorted = unsorted.TopologySort (
          content => content.Dependencies,
          contents => contents.OrderBy (cnt => cnt.Name),
          TopologySortMissingDependencyBehavior.Include).ToArray();

      Assert.That (sorted.Count(), Is.EqualTo (4));
      Assert.That (sorted[0], Is.EqualTo (new[] { d }));
      Assert.That (sorted[1], Is.EqualTo (new[] { c }));
      Assert.That (sorted[2], Is.EqualTo (new[] { b }));
      Assert.That (sorted[3], Is.EqualTo (new[] { a }));
    }

    [Test]
    public void Cyclic_Missing_Respect ()
    {
      var a = new TestContent ("a");
      var b = new TestContent ("b").Add (a);
      var c = new TestContent ("c").Add (a, b);
      var c1 = new TestContent ("c1").Add (c);
      c.Add (c1);
      var d = new TestContent ("d").Add (a, c);
      var unsorted = new[] { a, b, d };
      var sorted = unsorted.TopologySort (
          content => content.Dependencies,
          contents => contents.OrderBy (cnt => cnt.Name),
          TopologySortMissingDependencyBehavior.Respect).ToArray();

      Assert.That (sorted.Count(), Is.EqualTo (3));
      Assert.That (sorted[0], Is.EqualTo (new[] { d }));
      Assert.That (sorted[1], Is.EqualTo (new[] { b }));
      Assert.That (sorted[2], Is.EqualTo (new[] { a }));
    }

    [Test]
    public void Cyclic_Self_OK ()
    {
      var a = new TestContent ("a");
      var b = new TestContent ("b").Add (a);
      var c = new TestContent ("c").Add (a, b);
      c.Add (c);
      var unsorted = new[] { a, b, c };
      var sorted = unsorted.TopologySort (content => content.Dependencies).ToArray();

      Assert.That (sorted.Count(), Is.EqualTo (3));
      Assert.That (sorted[0], Is.EqualTo (new[] { c }));
      Assert.That (sorted[1], Is.EqualTo (new[] { b }));
      Assert.That (sorted[2], Is.EqualTo (new[] { a }));
    }

    [Test]
    public void Cyclic_Missing_Include ()
    {
      var a = new TestContent ("a");
      var b = new TestContent ("b").Add (a);
      var c = new TestContent ("c").Add (a, b);
      var c1 = new TestContent ("c1").Add (c);
      c.Add (c1);
      var d = new TestContent ("d").Add (a, c);
      var unsorted = new[] { a, b, d };

      Assert.That (
          () =>
          unsorted.TopologySort (
              content => content.Dependencies,
              contents => contents.OrderBy (cnt => cnt.Name),
              TopologySortMissingDependencyBehavior.Include).ToArray(),
          Throws.InvalidOperationException.And.Message.EqualTo ("Cyclic dependency detected - cannot perform topology sort"));
    }

    [Test]
    public void Cyclic ()
    {
      var a = new TestContent ("a");
      var b = new TestContent ("b").Add (a);
      var c = new TestContent ("c").Add (a, b);
      a.Add (c);

      var unsorted = new[] { a, b, c };
      Assert.That (
          () => unsorted.TopologySort (content => content.Dependencies).ToArray(),
          Throws.InvalidOperationException.And.Message.EqualTo ("Cyclic dependency detected - cannot perform topology sort"));
    }
  }
}