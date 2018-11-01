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
using System.Collections.Generic;
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  public class UnsupportedStoragePropertyDefinition : IRdbmsStoragePropertyDefinition
  {
    private readonly Type _propertyType;
    private readonly string _message;
    private readonly Exception _innerException;

    public UnsupportedStoragePropertyDefinition (Type propertyType, string message, Exception innerException)
    {
      ArgumentUtility.CheckNotNull ("propertyType", propertyType);
      ArgumentUtility.CheckNotNullOrEmpty ("message", message);

      _propertyType = propertyType;
      _message = message;
      _innerException = innerException;
    }

    public string Message
    {
      get { return _message; }
    }

    public Type PropertyType
    {
      get { return _propertyType; }
    }

    public Exception InnerException
    {
      get { return _innerException; }
    }

    public IEnumerable<ColumnDefinition> GetColumns ()
    {
      throw CreateNotSupportedException ();
    }

    public IEnumerable<ColumnDefinition> GetColumnsForComparison ()
    {
      throw CreateNotSupportedException();
    }

    public IEnumerable<ColumnValue> SplitValue (object value)
    {
      throw CreateNotSupportedException();
    }

    public IEnumerable<ColumnValue> SplitValueForComparison (object value)
    {
      throw CreateNotSupportedException ();
    }

    public ColumnValueTable SplitValuesForComparison (IEnumerable<object> values)
    {
      throw CreateNotSupportedException ();
    }

    public object CombineValue (IColumnValueProvider columnValueProvider)
    {
      throw CreateNotSupportedException ();
    }

    public IRdbmsStoragePropertyDefinition UnifyWithEquivalentProperties (IEnumerable<IRdbmsStoragePropertyDefinition> equivalentProperties)
    {
      ArgumentUtility.CheckNotNull ("equivalentProperties", equivalentProperties);
      equivalentProperties.Select (property => StoragePropertyDefinitionUnificationUtility.CheckAndConvertEquivalentProperty (
          this,
          property,
          "equivalentProperties",
          prop => Tuple.Create<string, object> ("property type", prop.PropertyType),
          prop => Tuple.Create<string, object> ("message", prop.Message),
          prop => Tuple.Create<string, object> ("inner exception type", prop.InnerException.GetType())
          )).ToArray ();

      return new UnsupportedStoragePropertyDefinition (_propertyType, _message, _innerException);
    }

    private NotSupportedException CreateNotSupportedException ()
    {
      return new NotSupportedException ("This operation is not supported because the storage property is invalid. Reason: " + Message, InnerException);
    }
  }
}