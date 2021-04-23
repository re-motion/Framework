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
using Remotion.Collections;
using Remotion.Development.UnitTesting;

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class AutoInitDictionaryTest
  {
    private MultiDictionary<string, string> _dictionary;

    [SetUp]
    public void SetUp ()
    {
      _dictionary = new MultiDictionary<string, string>();
    }

    [Test]
    public void Add ()
    {
      Assert.That (_dictionary["key"].Count, Is.EqualTo (0));
      _dictionary.Add ("key", "value1");
      _dictionary.Add ("key", "value2");
      Assert.That (_dictionary["key"].Count, Is.EqualTo (2));
      Assert.That (_dictionary["key"][0], Is.EqualTo ("value1"));
      Assert.That (_dictionary["key"][1], Is.EqualTo ("value2"));
    }

    [Test]
    public void Count()
    {
      Dev.Null = _dictionary["a"];
      Dev.Null = _dictionary["b"];
      Assert.That (_dictionary.Count, Is.EqualTo (2));
    }

    [Test]
    public void CountWithSameValues ()
    {
      _dictionary.Add ("key", "value1");
      _dictionary.Add ("key", "value2");
      _dictionary.Add ("key2", "value3");
      Assert.That (_dictionary.Count, Is.EqualTo (2));
      Assert.That (_dictionary.KeyCount, Is.EqualTo (2));
      Assert.That (_dictionary.CountValues(), Is.EqualTo (3));
    }
  }
}
