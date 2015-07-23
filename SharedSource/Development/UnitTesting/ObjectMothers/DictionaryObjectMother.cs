// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 

using System;
using System.Collections.Generic;

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
    {
      var container = new Dictionary<TKey, TValue> ();
      return container;
    }

    public static Dictionary<TKey, TValue> New<TKey, TValue> (TKey key0, TValue value0)
    {
      var container = new Dictionary<TKey, TValue> (1);
      container[key0] = value0;
      return container;
    }

    public static Dictionary<TKey, TValue> New<TKey, TValue> (TKey key0, TValue value0, TKey key1, TValue value1)
    {
      var container = new Dictionary<TKey, TValue> (2);
      container[key0] = value0;
      container[key1] = value1;
      return container;
    }

    public static Dictionary<TKey, TValue> New<TKey, TValue> (TKey key0, TValue value0, TKey key1, TValue value1, TKey key2, TValue value2)
    {
      var container = new Dictionary<TKey, TValue> (3);
      container[key0] = value0;
      container[key1] = value1;
      container[key2] = value2;
      return container;
    }

    public static Dictionary<TKey, TValue> New<TKey, TValue> (TKey key0, TValue value0, TKey key1, TValue value1, TKey key2, TValue value2, TKey key3, TValue value3)
    {
      var container = new Dictionary<TKey, TValue> (4);
      container[key0] = value0;
      container[key1] = value1;
      container[key2] = value2;
      container[key3] = value3;
      return container;
    }
  }
}
