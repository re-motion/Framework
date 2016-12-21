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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remotion.Text
{

/// <summary>
/// Builds a string adding separators between appended strings.
/// </summary>
[Obsolete ("Use StringBuilder instead. (Version 1.15.7.0)")]
public class SeparatedStringBuilder
{
  /// <summary>
  /// Appends the result of selector(item) for each item in the specified list.
  /// </summary>
  [Obsolete ("Use string.Join (separator, list.Select (selector)) instead. (Version 1.15.7.0)")]
  public static string Build<T> (string separator, IEnumerable<T> list, Func<T, string> selector)
  {
    return string.Join (separator, list.Select (selector));
  }

  /// <summary>
  /// Appends each item in the specified list.
  /// </summary>
  [Obsolete ("Use string.Join (separator, list) instead. (Version 1.15.7.0)")]
  public static string Build<T> (string separator, IEnumerable<T> list)
  {
    return string.Join (separator, list);
  }

  /// <summary>
  /// Appends the result of selector(item) for each item in the specified list.
  /// </summary>
  [Obsolete ("Use string.Join (separator, list.Cast<T>().Select (selector)) instead. (Version 1.15.7.0)")]
  public static string Build<T> (string separator, IEnumerable list, Func<T, string> selector)
  {
    return string.Join (separator, list.Cast<T>().Select (selector));
  }

  /// <summary>
  /// Appends each item in the specified list.
  /// </summary>
  [Obsolete ("Use string.Join (separator, list.Cast<object>()) instead. (Version 1.15.7.0)")]
  public static string Build (string separator, IEnumerable list)
  {
    return string.Join (separator, list.Cast<object>());
  }

  private string _separator;
  private StringBuilder _stringBuilder;

  [Obsolete ("Use StringBuilder instead. (Version 1.15.7.0)")]
  public SeparatedStringBuilder (string separator, int capacity)
  {
    _stringBuilder = new StringBuilder (capacity);
    _separator = separator;
  }

  [Obsolete ("Use StringBuilder instead. (Version 1.15.7.0)")]
  public SeparatedStringBuilder (string separator)
  {
    _stringBuilder = new StringBuilder ();
    _separator = separator;
  }

  public void Append (string s)
  {
    AppendSeparator();
    _stringBuilder.Append (s);
  }

  public void Append<T> (T arg)
  {
    AppendSeparator();
    _stringBuilder.Append (arg.ToString());
  }

  public void AppendFormat (string format, params object[] args)
  {
    AppendSeparator();
    _stringBuilder.AppendFormat (format, args);
  }

  private void AppendSeparator()
  {
    if (_stringBuilder.Length > 0)
      _stringBuilder.Append (_separator);
  }

  public int Length
  {
    get { return _stringBuilder.Length; }
  }

  public override string ToString()
  {
    return _stringBuilder.ToString();
  }

}

}
