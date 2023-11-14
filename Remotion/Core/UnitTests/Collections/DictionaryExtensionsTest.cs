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
using Moq;
using NUnit.Framework;
using Remotion.Collections;

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class DictionaryExtensionsTest
  {
    private Dictionary<string, string> _dictionary;

    [SetUp]
    public void SetUp ()
    {
      _dictionary = new Dictionary<string, string> { { "a", "Alpha" } };
    }

    [Test]
    public void GetValueOrDefault_WithIReadOnlyDictionary ()
    {
      var foundValue = ((IReadOnlyDictionary<string, string>)_dictionary).GetValueOrDefault("a");
      Assert.That(foundValue, Is.EqualTo("Alpha"));

      var notFoundValue = ((IReadOnlyDictionary<string, string>)_dictionary).GetValueOrDefault("z");
      Assert.That(notFoundValue, Is.Null);
    }

    [Test]
    public void GetValueOrDefault_WithIDictionary ()
    {
      var foundValue = ((IDictionary<string, string>)_dictionary).GetValueOrDefault("a");
      Assert.That(foundValue, Is.EqualTo("Alpha"));

      var notFoundValue = ((IDictionary<string, string>)_dictionary).GetValueOrDefault("z");
      Assert.That(notFoundValue, Is.Null);
    }

    [Test]
    public void GetValueOrDefault_WithDictionary_ValueTypes ()
    {
      Dictionary<string, int> dictionary = new Dictionary<string, int> { { "a", 7 } };

      var foundValue = dictionary.GetValueOrDefault("a");
      Assert.That(foundValue, Is.EqualTo(7));

      var notFoundValue = dictionary.GetValueOrDefault("z");
      Assert.That(notFoundValue, Is.EqualTo(0));
    }

#if NETFRAMEWORK
    [Test]
    public void GetValueOrDefault_WithReadOnlyDictionary_ValueTypes ()
    {
      var dictionary = new Dictionary<string, int> { { "a", 7 } };
      ReadOnlyDictionary<string, int> readOnlyDictionary =  dictionary.AsReadOnly();

      var foundValue = readOnlyDictionary.GetValueOrDefault("a");
      Assert.That(foundValue, Is.EqualTo(7));

      var notFoundValue = readOnlyDictionary.GetValueOrDefault("z");
      Assert.That(notFoundValue, Is.EqualTo(0));
    }
#endif

    [Test]
    public void GetValueOrDefault_WithIReadOnlyDictionary_NullKey ()
    {
      var dictionaryStub = new Mock<IReadOnlyDictionary<string, string>>();
      var outResult = "out";
      dictionaryStub.Setup(stub => stub.TryGetValue(null, out outResult)).Returns(true);

      var foundValue = dictionaryStub.Object.GetValueOrDefault(null);
      Assert.That(foundValue, Is.EqualTo("out"));
    }

    [Test]
    public void GetValueOrDefault_WithIDictionary_NullKey ()
    {
      var dictionaryStub = new Mock<IDictionary<string, string>>();
      var outResult = "out";
      dictionaryStub.Setup(stub => stub.TryGetValue(null, out outResult)).Returns(true);

      var foundValue = dictionaryStub.Object.GetValueOrDefault(null);
      Assert.That(foundValue, Is.EqualTo("out"));
    }

    [Test]
    public void GetValueOrDefault_WithIReadOnlyDictionary_WithDefaultValue ()
    {
      var foundValue = DictionaryExtensions.GetValueOrDefault((IReadOnlyDictionary<string, string>)_dictionary, "a", "Beta");
      Assert.That(foundValue, Is.EqualTo("Alpha"));

      var substitutedDefaultValue = DictionaryExtensions.GetValueOrDefault((IReadOnlyDictionary<string, string>)_dictionary, "z", "Beta");
      Assert.That(substitutedDefaultValue, Is.EqualTo("Beta"));
    }

    [Test]
    public void GetValueOrDefault_WithIDictionary_WithDefaultValue ()
    {
      var foundValue = ((IDictionary<string, string>)_dictionary).GetValueOrDefault("a", "Beta");
      Assert.That(foundValue, Is.EqualTo("Alpha"));

      var substitutedDefaultValue = ((IDictionary<string, string>)_dictionary).GetValueOrDefault("z", "Beta");
      Assert.That(substitutedDefaultValue, Is.EqualTo("Beta"));
    }

    [Test]
    public void GetValueOrDefault_WithDictionary_WithDefaultValue ()
    {
      var foundValue = ((Dictionary<string, string>)_dictionary).GetValueOrDefault("a", "Beta");
      Assert.That(foundValue, Is.EqualTo("Alpha"));

      var substitutedDefaultValue = ((Dictionary<string, string>)_dictionary).GetValueOrDefault("z", "Beta");
      Assert.That(substitutedDefaultValue, Is.EqualTo("Beta"));
    }

#if NETFRAMEWORK
    [Test]
    public void GetValueOrDefault_WithReadOnlyDictionary_WithDefaultValue ()
    {
      var foundValue = ((ReadOnlyDictionary<string, string>)_dictionary.AsReadOnly()).GetValueOrDefault("a", "Beta");
      Assert.That(foundValue, Is.EqualTo("Alpha"));

      var substitutedDefaultValue = ((ReadOnlyDictionary<string, string>)_dictionary.AsReadOnly()).GetValueOrDefault("z", "Beta");
      Assert.That(substitutedDefaultValue, Is.EqualTo("Beta"));
    }
#endif

    [Test]
    public void GetValueOrDefault_WithIReadOnlyDictionary_WithDefaultValue_NullDefaultValue ()
    {
      var substitutedDefaultValue = DictionaryExtensions.GetValueOrDefault((IReadOnlyDictionary<string, string>)_dictionary, "z", null);
      Assert.That(substitutedDefaultValue, Is.Null);
    }

    [Test]
    public void GetValueOrDefault_WithIDictionary_WithDefaultValue_NullDefaultValue ()
    {
      var substitutedDefaultValue = ((IDictionary<string, string>)_dictionary).GetValueOrDefault("z", null);
      Assert.That(substitutedDefaultValue, Is.Null);
    }

    [Test]
    public void GetValueOrDefault_WithDictionary_WithDefaultValue_NullDefaultValue ()
    {
      var substitutedDefaultValue = ((Dictionary<string, string>)_dictionary).GetValueOrDefault("z", null);
      Assert.That(substitutedDefaultValue, Is.Null);
    }

#if NETFRAMEWORK
    [Test]
    public void GetValueOrDefault_WithReadOnlyDictionary_WithDefaultValue_NullDefaultValue ()
    {
      var substitutedDefaultValue = ((ReadOnlyDictionary<string, string>)_dictionary.AsReadOnly()).GetValueOrDefault("z", null);
      Assert.That(substitutedDefaultValue, Is.Null);
    }
#endif

    [Test]
    public void GetOrCreateValue_WithDictionary_WithNewKey ()
    {
      var foundValue = ((Dictionary<string,string>)_dictionary).GetOrCreateValue("key", key => "value");
      Assert.That(foundValue, Is.EqualTo("value"));

      Assert.That(_dictionary.ContainsKey("key"), Is.True);
      Assert.That(_dictionary["key"], Is.EqualTo("value"));
    }

    [Test]
    public void GetOrCreateValue_WithDictionary_WithExistingKey ()
    {
      var foundValue = ((Dictionary<string,string>)_dictionary).GetOrCreateValue("a", key => throw new InvalidOperationException());
      Assert.That(foundValue, Is.EqualTo("Alpha"));
    }

    [Test]
    public void GetOrCreateValue_WithDictionary_WithNullKey ()
    {
      Assert.That(() => ((Dictionary<string, string>)_dictionary).GetOrCreateValue(null, key => throw new InvalidOperationException()), Throws.ArgumentNullException);
    }

#if NETFRAMEWORK
    [Test]
    public void AsReadOnly ()
    {
      ReadOnlyDictionary<string, string> readOnlyDictionary = ((IDictionary<string, string>)_dictionary).AsReadOnly();

      Assert.That(readOnlyDictionary, Is.EqualTo(_dictionary));
    }

    [Test]
    public void AsReadOnly_WithReadOnlyDictionary_ReturnsSameInstance ()
    {
      var dictionary = new ReadOnlyDictionary<string, string>(_dictionary);

      var readOnlyDictionary = dictionary.AsReadOnly();

      Assert.That(readOnlyDictionary, Is.SameAs(dictionary));
    }
#endif
  }
}
