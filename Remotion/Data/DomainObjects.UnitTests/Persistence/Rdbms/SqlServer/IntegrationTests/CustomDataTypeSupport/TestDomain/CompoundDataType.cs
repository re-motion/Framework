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
using System.Collections;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests.CustomDataTypeSupport.TestDomain
{
  public sealed class CompoundDataType : IStructuralEquatable
  {
    private readonly string _stringValue;
    private readonly int _int32Value;

    public CompoundDataType (string stringValue, int int32Value)
    {
      ArgumentUtility.CheckNotNull ("stringValue", stringValue);

      _stringValue = stringValue;
      _int32Value = int32Value;
    }

    public string StringValue
    {
      get { return _stringValue; }
    }

    public int Int32Value
    {
      get { return _int32Value; }
    }

    public bool Equals (object other, IEqualityComparer comparer)
    {
      var otherCompoundDataType = other as CompoundDataType;
      if (otherCompoundDataType == null)
        return false;
      return comparer.Equals (_stringValue, otherCompoundDataType._stringValue) && comparer.Equals (_int32Value, otherCompoundDataType._int32Value);
    }

    public int GetHashCode (IEqualityComparer comparer)
    {
      return comparer.GetHashCode (_stringValue);
    }
  }
}