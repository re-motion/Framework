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
using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;

/// <summary>
/// Describes a .NET <see cref="Type"/> that corresponds to a custom structured database type.
/// </summary>
/// <remarks>Analogous to <see cref="ClassDefinition"/>, but instead for a class derived from <see cref="DomainObject"/> that is stored in a database entity,
/// an <see cref="RecordDefinition"/> describes the properties of a POCO (plain old CLR object) that can be mapped onto a structured type in the database.</remarks>
public class RecordDefinition
{
  /// <summary>
  /// The simple name of the .NET <see cref="Type"/>.
  /// </summary>
  public string RecordName { get; }

  /// <summary>
  /// The <see cref="IRdbmsStructuredTypeDefinition"/> that defines the corresponding custom structured database type.
  /// </summary>
  public IRdbmsStructuredTypeDefinition StructuredTypeDefinition { get; }

  /// <summary>
  /// The properties of the described .NET <see cref="Type"/>. 
  /// </summary>
  public IReadOnlyCollection<RecordPropertyDefinition> PropertyDefinitions { get; }

  public RecordDefinition (string name, IRdbmsStructuredTypeDefinition structuredTypeDefinition, IReadOnlyCollection<RecordPropertyDefinition> propertyDefinitions)
  {
    ArgumentUtility.CheckNotNullOrEmpty(nameof(name), name);
    ArgumentUtility.CheckNotNull(nameof(structuredTypeDefinition), structuredTypeDefinition);
    ArgumentUtility.CheckNotNull(nameof(propertyDefinitions), propertyDefinitions);

    if (!structuredTypeDefinition.Properties.SequenceEqual(propertyDefinitions.Select(p => p.StoragePropertyDefinition)))
    {
      throw new ArgumentException(
          $"The given {nameof(structuredTypeDefinition)}'s properties do not match those of the given {nameof(propertyDefinitions)}.");
    }

    RecordName = name;
    StructuredTypeDefinition = structuredTypeDefinition;
    PropertyDefinitions = propertyDefinitions;
  }

  /// <summary>
  /// Gets the values of the given <paramref name="item"/> that can be mapped onto the of the <see cref="StructuredTypeDefinition"/>'s <see cref="ColumnDefinition"/>s,
  /// in the corresponding order.
  /// </summary>
  /// <param name="item">The <see cref="object"/> from which to read the values.</param>
  public object[] GetColumnValues (object item)
  {
    ArgumentUtility.CheckNotNull(nameof(item), item);

    var columnValues = PropertyDefinitions.SelectMany(pd => pd.StoragePropertyDefinition.SplitValue(pd.GetValue(item)));
    var convertedValues = columnValues.Select(cv => cv.Column.StorageTypeInfo.ConvertToStorageType(cv.Value)).ToArray();
    return convertedValues;
  }
}
