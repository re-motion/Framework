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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests.CustomDataTypeSupport.TestDomain
{
  public sealed class SimpleDataType : IStructuralEquatable
  {
    private readonly string _stringValue;

    public SimpleDataType (string stringValue)
    {
      ArgumentUtility.CheckNotNull("stringValue", stringValue);
      _stringValue = stringValue;
    }

    public string StringValue
    {
      get { return _stringValue; }
    }

    public bool Equals (object other, IEqualityComparer comparer)
    {
      var otherSimpleDataType = other as SimpleDataType;
      if (otherSimpleDataType == null)
        return false;
      return comparer.Equals(_stringValue, otherSimpleDataType._stringValue);
    }

    public int GetHashCode (IEqualityComparer comparer)
    {
      return comparer.GetHashCode(_stringValue);
    }
  }
}