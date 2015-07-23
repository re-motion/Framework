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
using System.Collections.ObjectModel;
using NUnit.Framework;
using Remotion.Collections;
using Rhino.Mocks;

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class DictionaryExtensionsTest
  {
    private IDictionary<string, string> _dictionary;

    [SetUp]
    public void SetUp ()
    {
      _dictionary = new Dictionary<string, string> { { "a", "Alpha" } };
    }

    [Test]
    public void GetValueOrDefault ()
    {
      var foundValue = _dictionary.GetValueOrDefault ("a");
      Assert.That (foundValue, Is.EqualTo ("Alpha"));

      var notFoundValue = _dictionary.GetValueOrDefault ("z");
      Assert.That (notFoundValue, Is.Null);
    }
    
    [Test]
    public void GetValueOrDefault_ValueTypes ()
    {
      var dictionary = new Dictionary<string, int> { { "a", 7 } };

      var foundValue = dictionary.GetValueOrDefault ("a");
      Assert.That (foundValue, Is.EqualTo (7));

      var notFoundValue = dictionary.GetValueOrDefault ("z");
      Assert.That (notFoundValue, Is.EqualTo (0));
    }

    [Test]
    public void GetValueOrDefault_NullKey ()
    {
      var dictionaryStub = MockRepository.GenerateStub<IDictionary<string, string>>();
      dictionaryStub.Stub (stub => stub.TryGetValue (Arg<string>.Is.Null, out Arg<string>.Out ("out").Dummy)).Return (true);

      var foundValue = dictionaryStub.GetValueOrDefault (null);
      Assert.That (foundValue, Is.EqualTo ("out"));
    }

    [Test]
    public void GetValueOrDefault_WithDefaultValue ()
    {
      var foundValue = _dictionary.GetValueOrDefault ("a", "Beta");
      Assert.That (foundValue, Is.EqualTo ("Alpha"));

      var substitutedDefaultValue = _dictionary.GetValueOrDefault ("z", "Beta");
      Assert.That (substitutedDefaultValue, Is.EqualTo ("Beta"));
    }

    [Test]
    public void GetValueOrDefault_WithDefaultValue_NullDefaultValue ()
    {
      var substitutedDefaultValue = _dictionary.GetValueOrDefault ("z", null);
      Assert.That (substitutedDefaultValue, Is.Null);
    }

    [Test]
    public void AsReadOnly ()
    {
      ReadOnlyDictionary<string, string> readOnlyDictionary = _dictionary.AsReadOnly ();

      Assert.That (readOnlyDictionary, Is.EqualTo (_dictionary));
    }
  }
}