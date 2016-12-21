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
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// Defines that a property maps directly to a simple column in a relational database.
  /// </summary>
  public class SimpleStoragePropertyDefinition : IRdbmsStoragePropertyDefinition
  {
    private readonly Type _propertyType;
    private readonly ColumnDefinition _columnDefinition;

    public SimpleStoragePropertyDefinition (Type propertyType, ColumnDefinition columnDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyType", propertyType);
      ArgumentUtility.CheckNotNull ("columnDefinition", columnDefinition);

      _columnDefinition = columnDefinition;
      _propertyType = propertyType;
    }

    public ColumnDefinition ColumnDefinition
    {
      get { return _columnDefinition; }
    }

    public Type PropertyType
    {
      get { return _propertyType; }
    }

    public IEnumerable<ColumnDefinition> GetColumns ()
    {
      return EnumerableUtility.Singleton (_columnDefinition);
    }

    public IEnumerable<ColumnDefinition> GetColumnsForComparison ()
    {
      return EnumerableUtility.Singleton (_columnDefinition);
    }

    public IEnumerable<ColumnValue> SplitValue (object value)
    {
      return EnumerableUtility.Singleton (new ColumnValue (_columnDefinition, value));
    }

    public IEnumerable<ColumnValue> SplitValueForComparison (object value)
    {
      return SplitValue (value);
    }

    public ColumnValueTable SplitValuesForComparison (IEnumerable<object> values)
    {
      ArgumentUtility.CheckNotNull ("values", values);

      return new ColumnValueTable(
          EnumerableUtility.Singleton (_columnDefinition), 
          values.Select (v => new ColumnValueTable.Row (EnumerableUtility.Singleton (v))));
    }

    public object CombineValue (IColumnValueProvider columnValueProvider)
    {
      ArgumentUtility.CheckNotNull ("columnValueProvider", columnValueProvider);
      return columnValueProvider.GetValueForColumn (_columnDefinition);
    }

    public IRdbmsStoragePropertyDefinition UnifyWithEquivalentProperties (IEnumerable<IRdbmsStoragePropertyDefinition> equivalentProperties)
    {
      ArgumentUtility.CheckNotNull ("equivalentProperties", equivalentProperties);
      var checkedProperties = equivalentProperties.Select (property => StoragePropertyDefinitionUnificationUtility.CheckAndConvertEquivalentProperty (
          this,
          property,
          "equivalentProperties",
          prop => Tuple.Create<string, object> ("property type", prop.PropertyType),
          prop => Tuple.Create<string, object> ("column name", prop.ColumnDefinition.Name),
          prop => Tuple.Create<string, object> ("primary key flag", prop.ColumnDefinition.IsPartOfPrimaryKey)
          )).ToArray ();

      return new SimpleStoragePropertyDefinition (
          _propertyType,
          new ColumnDefinition (
              _columnDefinition.Name,
              _columnDefinition.StorageTypeInfo.UnifyForEquivalentProperties (checkedProperties.Select (p => p.ColumnDefinition.StorageTypeInfo)),
              _columnDefinition.IsPartOfPrimaryKey));
    }
  }
}