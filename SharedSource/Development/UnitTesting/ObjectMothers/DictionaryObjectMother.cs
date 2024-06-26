// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections.Generic;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.Development.UnitTesting.ObjectMothers
{
  /// <summary>
  /// Supplies factories to easily create <see cref="Dictionary{TKey,TValue}"/> instances initialized with up to 4 key-value-pairs.
  /// </summary>
  /// <example><code>
  /// <![CDATA[  
  /// var d = DictionaryObjectMother.New("A",1, "B",2, "C",3); // d["A"]=1, d["B"]=2,...
  /// ]]>
  /// </code></example>
  static partial class DictionaryObjectMother
  {
    public static Dictionary<TKey, TValue> New<TKey, TValue> ()
        where TKey : notnull
    {
      var container = new Dictionary<TKey, TValue>();
      return container;
    }

    public static Dictionary<TKey, TValue> New<TKey, TValue> (TKey key0, TValue value0)
        where TKey : notnull
    {
      var container = new Dictionary<TKey, TValue>(1);
      container[key0] = value0;
      return container;
    }

    public static Dictionary<TKey, TValue> New<TKey, TValue> (TKey key0, TValue value0, TKey key1, TValue value1)
        where TKey : notnull
    {
      var container = new Dictionary<TKey, TValue>(2);
      container[key0] = value0;
      container[key1] = value1;
      return container;
    }

    public static Dictionary<TKey, TValue> New<TKey, TValue> (TKey key0, TValue value0, TKey key1, TValue value1, TKey key2, TValue value2)
        where TKey : notnull
    {
      var container = new Dictionary<TKey, TValue>(3);
      container[key0] = value0;
      container[key1] = value1;
      container[key2] = value2;
      return container;
    }

    public static Dictionary<TKey, TValue> New<TKey, TValue> (TKey key0, TValue value0, TKey key1, TValue value1, TKey key2, TValue value2, TKey key3, TValue value3)
        where TKey : notnull
    {
      var container = new Dictionary<TKey, TValue>(4);
      container[key0] = value0;
      container[key1] = value1;
      container[key2] = value2;
      container[key3] = value3;
      return container;
    }
  }
}
