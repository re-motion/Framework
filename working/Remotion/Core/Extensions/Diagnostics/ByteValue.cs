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

namespace Remotion.Diagnostics
{
  public struct ByteValue
  {
    private readonly long _bytes;

    public ByteValue (long bytes)
        : this ()
    {
      _bytes = bytes;
    }

    public long Bytes
    {
      get { return _bytes; }
    }

    public decimal MegaBytes { get { return Bytes / 1024.0m / 1024.0m; } }

    public override string ToString ()
    {
      if (MegaBytes > 1)
        return MegaBytes.ToString ("N2") + " MB";
      else
        return Bytes.ToString ("N0") + " bytes";
    }

    public string ToDifferenceString ()
    {
      if (Bytes > 0)
        return "+" + ToString ();
      else
        return ToString ();
    }

    public static ByteValue operator + (ByteValue left, ByteValue right)
    {
      return new ByteValue (left.Bytes + right.Bytes);
    }

    public static ByteValue operator - (ByteValue left, ByteValue right)
    {
      return new ByteValue (left.Bytes - right.Bytes);
    }
  }
}
