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
using System.Diagnostics;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  [DebuggerDisplay("{GetType().Name}: {PropertyName}")]
  public class PropertyDefinition : PropertyDefinitionBase
  {
    public PropertyDefinition (
        ClassDefinition classDefinition,
        IPropertyInformation propertyInfo,
        string propertyName,
        bool isObjectID,
        bool isNullable,
        int? maxLength,
        StorageClass storageClass,
        object? defaultValue)
        : base(propertyName, propertyInfo, isObjectID ? typeof(ObjectID) : propertyInfo.PropertyType, isNullable, maxLength)
    {
      ArgumentUtility.CheckNotNull(nameof(classDefinition), classDefinition);

      ClassDefinition = classDefinition;
      IsObjectID = isObjectID;
      StorageClass = storageClass;
      DefaultValue = defaultValue;
    }

    public ClassDefinition ClassDefinition { get; }

    public object? DefaultValue { get; }

    public bool IsObjectID { get; }

    public StorageClass StorageClass { get; }

    public override IStoragePropertyDefinition StoragePropertyDefinition
    {
      get
      {
        if (StorageClass != StorageClass.Persistent)
          throw new InvalidOperationException("Cannot access property 'storagePropertyDefinition' for non-persistent property definitions.");

        return base.StoragePropertyDefinition;
      }
    }

    protected override string GetTypeID ()
    {
      return ClassDefinition.ID;
    }
  }
}
