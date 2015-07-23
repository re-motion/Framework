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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectIDStringSerialization
{
  /// <summary>
  /// Parses the <see cref="Int32"/> value of an <see cref="ObjectID"/>.
  /// </summary>
  public class Int32ObjectIDValueParser : IObjectIDValueParser
  {
    public static readonly Int32ObjectIDValueParser Instance = new Int32ObjectIDValueParser ();

    private Int32ObjectIDValueParser ()
    {
    }

    public bool TryParse (string stringValue, out object resultValue)
    {
      ArgumentUtility.CheckNotNull ("stringValue", stringValue);

      int intValue;
      if (stringValue != string.Empty && int.TryParse (stringValue, out intValue))
      {
        resultValue = intValue;
        return true;
      }

      resultValue = null;
      return false;
    }
  }
}
