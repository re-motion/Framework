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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;

/// <summary>
/// Describes a property of a .NET <see cref="Type"/> that corresponds to a custom structured database type.
/// </summary>
/// <remarks>Analogous to <see cref="PropertyDefinition"/>, but instead for a property on a class derived from <see cref="DomainObject"/> that is stored in a database column,
/// an <see cref="RecordPropertyDefinition"/> describes a property of a POCO (plain old CLR object) that can be mapped onto a structured type in the database.</remarks>
public class RecordPropertyDefinition
{
  /// <summary>
  /// Creates and returns a <see cref="RecordPropertyDefinition"/> that represents the source object itself as a property with name "Self";
  /// the source object must be a scalar value. 
  /// </summary>
  /// <param name="storagePropertyDefinition">Defines the type of the source object and how it is stored in the "Value" column.</param>
  public static RecordPropertyDefinition ScalarAsValue (IRdbmsStoragePropertyDefinition storagePropertyDefinition)
  {
    ArgumentUtility.CheckNotNull(nameof(storagePropertyDefinition), storagePropertyDefinition);
    return new RecordPropertyDefinition("Self", storagePropertyDefinition, o => o);
  }

  /// <summary>
  /// The name of the property as defined on the .NET <see cref="Type"/>.
  /// </summary>
  public string PropertyName { get; }

  /// <summary>
  /// The <see cref="IRdbmsStoragePropertyDefinition"/> that defines the corresponding property on the custom structured database type.
  /// </summary>
  public IRdbmsStoragePropertyDefinition StoragePropertyDefinition { get; }

  private readonly Func<object, object?> _getValue;

  protected RecordPropertyDefinition (string propertyName, IRdbmsStoragePropertyDefinition storagePropertyDefinition, Func<object, object?> getValue)
  {
    ArgumentUtility.CheckNotNullOrEmpty(nameof(propertyName), propertyName);
    ArgumentUtility.CheckNotNull(nameof(storagePropertyDefinition), storagePropertyDefinition);
    ArgumentUtility.CheckNotNull(nameof(getValue), getValue);

    PropertyName = propertyName;
    StoragePropertyDefinition = storagePropertyDefinition;
    _getValue = getValue;
  }

  /// <summary>
  /// Gets the property value from the given <paramref name="sourceObject"/>.
  /// </summary>
  public object? GetValue (object sourceObject)
  {
    return _getValue(sourceObject);
  }
}
