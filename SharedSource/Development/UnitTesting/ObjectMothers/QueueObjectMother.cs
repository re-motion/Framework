// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections.Generic;

#nullable enable
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
      var container = new System.Collections.Generic.Queue<T>(values);
      return container;
    }
  }
}
