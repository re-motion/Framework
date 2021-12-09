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
using System.Collections.Generic;
using System.Linq;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  [Serializable]
  public class PropertyDefinitionCollection : CommonCollection, IEnumerable<PropertyDefinition>
  {
    public static PropertyDefinitionCollection CreateForAllProperties (TypeDefinition typeDefinition, bool makeCollectionReadOnly)
    {
      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);

      if (typeDefinition is not ClassDefinition classDefinition) // TODO R2I Mapping: Maybe move this into TypeDefinition instead
        throw new NotSupportedException("Only class definitions are supported");

      return new PropertyDefinitionCollection(
          classDefinition.CreateSequence(cd => cd.BaseClass).SelectMany(cd => cd.MyPropertyDefinitions), makeCollectionReadOnly);
    }

    public PropertyDefinitionCollection ()
    {
    }

    public PropertyDefinitionCollection (IEnumerable<PropertyDefinition> collection, bool makeCollectionReadOnly)
    {
      ArgumentUtility.CheckNotNull("collection", collection);

      foreach (var propertyDefinition in collection)
        Add(propertyDefinition);

      SetIsReadOnly(makeCollectionReadOnly);
    }

    public void SetReadOnly ()
    {
      SetIsReadOnly(true);
    }

    public IEnumerable<PropertyDefinition> GetAllPersistent ()
    {
      return this.Where(propertyDefinition => propertyDefinition.StorageClass == StorageClass.Persistent);
    }

    public new IEnumerator<PropertyDefinition> GetEnumerator ()
    {
      // ReSharper disable LoopCanBeConvertedToQuery
      foreach (PropertyDefinition propertyDefinition in (IEnumerable)this) // use base implementation
          // ReSharper restore LoopCanBeConvertedToQuery
        yield return propertyDefinition;
    }

    #region Standard implementation for "add-only" collections

    public bool Contains (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull("propertyDefinition", propertyDefinition);

      return BaseContains(propertyDefinition.PropertyName, propertyDefinition);
    }

    public bool Contains (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty("propertyName", propertyName);
      return BaseContainsKey(propertyName);
    }

    public PropertyDefinition this [int index]
    {
      get { return (PropertyDefinition)BaseGetObject(index); }
    }

    public PropertyDefinition? this [string propertyName]
    {
      get
      {
        ArgumentUtility.CheckNotNullOrEmpty("propertyName", propertyName);
        return (PropertyDefinition?)BaseGetObject(propertyName);
      }
    }

    public int Add (PropertyDefinition value)
    {
      ArgumentUtility.CheckNotNull("value", value);

      int position = BaseAdd(value.PropertyName, value);

      return position;
    }

    #endregion
  }
}
