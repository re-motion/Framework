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
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping;

public class TupleDefinition
{
  public string ID { get; }
  public Type TupleType { get; }
  public Type? StorageGroupType { get; }

  private IReadOnlyCollection<TuplePropertyDefinition>? _propertyDefinitions;
  private IStructuredTypeDefinition? _structuredTypeDefinition;

  public TupleDefinition (string id, Type tupleType, Type? storageGroupType)
  {
    ID = id;
    TupleType = tupleType;
    StorageGroupType = storageGroupType;
  }

  public IReadOnlyCollection<TuplePropertyDefinition> PropertyDefinitions
  {
    get
    {
      if (_propertyDefinitions == null)
        throw new InvalidOperationException($"No property definitions have been set for tuple '{ID}'.");

      return _propertyDefinitions;
    }
  }

  public IStructuredTypeDefinition StructuredTypeDefinition
  {
    get
    {
      Assertion.IsNotNull(_structuredTypeDefinition, $"{nameof(StructuredTypeDefinition)} has not been set for tuple definition '{ID}'.");
      return _structuredTypeDefinition;
    }
  }

  public void SetTuplePropertyDefinitions (IEnumerable<TuplePropertyDefinition> tupleElements)
  {
    ArgumentUtility.CheckNotNull(nameof(tupleElements), tupleElements);
    _propertyDefinitions = tupleElements.ToList().AsReadOnly();
  }

  public void SetStructuredTypeDefinition (IStructuredTypeDefinition structuredTypeDefinition)
  {
    ArgumentUtility.CheckNotNull(nameof(structuredTypeDefinition), structuredTypeDefinition);
    _structuredTypeDefinition = structuredTypeDefinition;
  }
}
