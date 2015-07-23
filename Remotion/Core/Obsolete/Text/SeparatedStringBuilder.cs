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

// ReSharper disable once CheckNamespace

namespace Remotion.Text
{
  [Obsolete ("Dummy declaration for DependDB. Moved to Remotion.Extensions.dll", true)]
  internal abstract class SeparatedStringBuilder
  {
    public static string Build<T> (string separator, IEnumerable<T> list, Func<T, string> selector)
    {
      throw new NotImplementedException();
    }

    public static string Build<T> (string separator, IEnumerable<T> list)
    {
      throw new NotImplementedException();
    }

    public static string Build<T> (string separator, IEnumerable list, Func<T, string> selector)
    {
      throw new NotImplementedException();
    }

    public static string Build (string separator, IEnumerable list)
    {
      throw new NotImplementedException();
    }

    public SeparatedStringBuilder (string separator, int capacity)
    {
      throw new NotImplementedException();
    }

    public SeparatedStringBuilder (string separator)
    {
      throw new NotImplementedException();
    }

    public abstract void Append (string s);

    public abstract void Append<T> (T arg);

    public abstract void AppendFormat (string format, params object[] args);


    public abstract int Length { get; }
  }
}