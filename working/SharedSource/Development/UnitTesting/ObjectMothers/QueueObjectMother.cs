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
  /// Supplies factories to easily create <see cref="Queue{T}"/> instances.
  /// </summary>
  /// <example><code>
  /// <![CDATA[  
  /// var queue = QueueObjectMother.New("process","emit0","wait");
  /// ]]>
  /// </code></example>
  static partial class QueueObjectMother
  {
    public static System.Collections.Generic.Queue<T> New<T> (params T[] values)
    {
      var container = new System.Collections.Generic.Queue<T> (values);
      return container;
    }
  }
}
