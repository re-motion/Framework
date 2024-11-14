// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0

//
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.Utilities
{
  /// <summary>      
  /// An equality comparer that compares equality using ReferenceEquals rather than Equals. This is to ensure that two objects are actually the same 
  /// and not just equal for reference checking purposes.      
  /// </summary>      
  /// <typeparam name="T">the type of object to check</typeparam>  
  partial class ReferenceEqualityComparer<T> : IEqualityComparer<T> where T : class
  {
    public static readonly ReferenceEqualityComparer<T> Instance = new ReferenceEqualityComparer<T>();

    private ReferenceEqualityComparer ()
    {
    }

    public bool Equals (T? x, T? y)
    {
      return object.ReferenceEquals(x, y);
    }

    public int GetHashCode (T obj)
    {
      return RuntimeHelpers.GetHashCode(obj);
    }
  }
}
