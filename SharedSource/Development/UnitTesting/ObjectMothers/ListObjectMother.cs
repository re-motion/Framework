// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections.Generic;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.Development.UnitTesting.ObjectMothers
{
  /// <summary>
  /// Supplies factories to easily create <see cref="List{T}"/> instances.
  /// </summary>
  /// <example><code>
  /// <![CDATA[  
  /// var listList = ListObjectMother.New( List.New(1,2), List.New(3,4) );
  /// ]]>
  /// </code></example>
  static partial class ListObjectMother
  {
    public static System.Collections.Generic.List<T> New<T> (params T[] values)
    {
      var container = new System.Collections.Generic.List<T>(values);
      return container;
    }
  }
}
