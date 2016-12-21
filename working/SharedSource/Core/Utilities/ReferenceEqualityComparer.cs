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

//
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

// ReSharper disable once CheckNamespace
namespace Remotion.Utilities
{
  /// <summary>      
  /// An equality comparer that compares equality using ReferenceEquals rather than Equals. This is to ensure that two objects are actually the same 
  /// and not just equal for reference checking purposes.      
  /// </summary>      
  /// <typeparam name="T">the type of object to check</typeparam>  
  partial class ReferenceEqualityComparer<T> : IEqualityComparer<T>
  {
    public static readonly ReferenceEqualityComparer<T> Instance = new ReferenceEqualityComparer<T>();

    private ReferenceEqualityComparer ()
    {
    }

    public bool Equals (T x, T y)
    {
      return object.ReferenceEquals (x, y);
    }

    public int GetHashCode (T obj)
    {
      return RuntimeHelpers.GetHashCode (obj); 
    }
  }
}