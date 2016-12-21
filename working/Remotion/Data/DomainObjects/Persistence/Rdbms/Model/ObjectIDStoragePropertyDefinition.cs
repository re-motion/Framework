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
  /// <summary>
  /// The <see cref="ObjectIDStoragePropertyDefinition"/> represents an <see cref="ObjectID"/> property that is stored as an ID column and a ClassID
  /// column.
  /// </summary>
  public class ObjectIDStoragePropertyDefinition : IObjectIDStoragePropertyDefinition
  {
    private readonly IRdbmsStoragePropertyDefinition _valueProperty;
    private readonly IRdbmsStoragePropertyDefinition _classIDProperty;

    public ObjectIDStoragePropertyDefinition (IRdbmsStoragePropertyDefinition valueProperty, IRdbmsStoragePropertyDefinition classIDProperty)
    {
      ArgumentUtility.CheckNotNull ("valueProperty", valueProperty);
      ArgumentUtility.CheckNotNull ("classIDProperty", classIDProperty);

      _valueProperty = valueProperty;
      _classIDProperty = classIDProperty;
    }

    public Type PropertyType
    {
      get { return typeof (ObjectID); }
    }

    public IRdbmsStoragePropertyDefinition ValueProperty
    {
      get { return _valueProperty; }
    }

    public IRdbmsStoragePropertyDefinition ClassIDProperty
    {
      get { return _classIDProperty; }
    }

    public bool CanCreateForeignKeyConstraint
    {
      get { return true; }
    }

    public IEnumerable<ColumnDefinition> GetColumnsForComparison ()
    {
      // TODO in case of integer primary keys: 
      // If RdbmsProvider or one of its derived classes needs to support integer primary keys in addition to GUIDs,
      // two lookup columns should be used: ID and ClassID (because int IDs wouldn't be globally unique).
      // For GUID keys, we don't want to include the ClassID, however.

      return _valueProperty.GetColumnsForComparison();
    }

    public IEnumerable<ColumnDefinition> GetColumns ()
    {
      return _valueProperty.GetColumns().Concat (_classIDProperty.GetColumns());
    }

    public IEnumerable<ColumnValue> SplitValue (object value)
    {
      var objectID = ArgumentUtility.CheckType<ObjectID> ("value", value);
      if (objectID == null)
        return _valueProperty.SplitValue (null).Concat (_classIDProperty.SplitValue (null));

      return _valueProperty.SplitValue (objectID.Value).Concat (_classIDProperty.SplitValue (objectID.ClassID));
    }

    public IEnumerable<ColumnValue> SplitValueForComparison (object value)
    {
      var objectID = ArgumentUtility.CheckType<ObjectID> ("value", value);

      return _valueProperty.SplitValueForComparison (GetValueOrNull (objectID));
    }

    public ColumnValueTable SplitValuesForComparison (IEnumerable<object> values)
    {
      ArgumentUtility.CheckNotNull ("values", values);

      return _valueProperty.SplitValuesForComparison (values.Select (v => GetValueOrNull ((ObjectID) v)));
    }

    public object CombineValue (IColumnValueProvider columnValueProvider)
    {
      ArgumentUtility.CheckNotNull ("columnValueProvider", columnValueProvider);

      var value = _valueProperty.CombineValue (columnValueProvider);
      var classID = (string) _classIDProperty.CombineValue (columnValueProvider);
      if (value == null)
      {
        if (classID != null)
        {
          throw new RdbmsProviderException (
              string.Format (
                  "Incorrect database value encountered. The value read from '{0}' must contain null.",
                  string.Join (", ", _classIDProperty.GetColumns ().Select (c => c.Name))));
        }

        return null;
      }

      if (classID == null)
      {
        throw new RdbmsProviderException (
            string.Format (
                "Incorrect database value encountered. The value read from '{0}' must not contain null.",
                string.Join (", ", _classIDProperty.GetColumns ().Select (c => c.Name))));
      }

      return new ObjectID(classID, value);
    }

    public IRdbmsStoragePropertyDefinition UnifyWithEquivalentProperties (IEnumerable<IRdbmsStoragePropertyDefinition> equivalentProperties)
    {
      ArgumentUtility.CheckNotNull ("equivalentProperties", equivalentProperties);
      var checkedProperties = equivalentProperties.Select (property => StoragePropertyDefinitionUnificationUtility.CheckAndConvertEquivalentProperty (
          this, property, "equivalentProperties")).ToArray ();

      var unifiedValueProperty = _valueProperty.UnifyWithEquivalentProperties (checkedProperties.Select (p => p.ValueProperty));
      var unifiedClassIDProperty = _classIDProperty.UnifyWithEquivalentProperties (checkedProperties.Select (p => p.ClassIDProperty));
      return new ObjectIDStoragePropertyDefinition (unifiedValueProperty, unifiedClassIDProperty);
    }

    public ForeignKeyConstraintDefinition CreateForeignKeyConstraint (
        Func<IEnumerable<ColumnDefinition>, string> nameProvider,
        EntityNameDefinition referencedTableName,
        ObjectIDStoragePropertyDefinition referencedObjectIDProperty)
    {
      ArgumentUtility.CheckNotNull ("nameProvider", nameProvider);
      ArgumentUtility.CheckNotNull ("referencedTableName", referencedTableName);
      ArgumentUtility.CheckNotNull ("referencedObjectIDProperty", referencedObjectIDProperty);

      var referencingColumns = GetColumnsForComparison ();
      var referencedColumns = referencedObjectIDProperty.GetColumnsForComparison ();
      return new ForeignKeyConstraintDefinition (nameProvider (referencingColumns),  referencedTableName,  referencingColumns, referencedColumns);
    }

    private object GetValueOrNull (ObjectID objectID)
    {
      return objectID != null ? objectID.Value : null;
    }
  }
}