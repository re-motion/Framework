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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building
{
  /// <summary>
  /// The <see cref="DataStoragePropertyDefinitionFactory"/> creates <see cref="IRdbmsStoragePropertyDefinition"/> objects for 
  /// <see cref="PropertyDefinition"/> instances, delegating to <see cref="IValueStoragePropertyDefinitionFactory"/> and 
  /// <see cref="IRelationStoragePropertyDefinitionFactory"/>.
  /// </summary>
  public class DataStoragePropertyDefinitionFactory : IDataStoragePropertyDefinitionFactory
  {
    private readonly IValueStoragePropertyDefinitionFactory _valueStoragePropertyDefinitionFactory;
    private readonly IRelationStoragePropertyDefinitionFactory _relationStoragePropertyDefinitionFactory;

    public DataStoragePropertyDefinitionFactory (
        IValueStoragePropertyDefinitionFactory valueStoragePropertyDefinitionFactory,
        IRelationStoragePropertyDefinitionFactory relationStoragePropertyDefinitionFactory)
    {
      ArgumentUtility.CheckNotNull("valueStoragePropertyDefinitionFactory", valueStoragePropertyDefinitionFactory);
      ArgumentUtility.CheckNotNull("relationStoragePropertyDefinitionFactory", relationStoragePropertyDefinitionFactory);

      _valueStoragePropertyDefinitionFactory = valueStoragePropertyDefinitionFactory;
      _relationStoragePropertyDefinitionFactory = relationStoragePropertyDefinitionFactory;
    }

    public IValueStoragePropertyDefinitionFactory ValueStoragePropertyDefinitionFactory
    {
      get { return _valueStoragePropertyDefinitionFactory; }
    }

    public IRelationStoragePropertyDefinitionFactory RelationStoragePropertyDefinitionFactory
    {
      get { return _relationStoragePropertyDefinitionFactory; }
    }

    public virtual IRdbmsStoragePropertyDefinition CreateStoragePropertyDefinition (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull("propertyDefinition", propertyDefinition);

      var relationEndPointDefinition =
          (RelationEndPointDefinition?)propertyDefinition.ClassDefinition.GetRelationEndPointDefinition(propertyDefinition.PropertyName);
      if (relationEndPointDefinition != null)
      {
        Assertion.IsTrue(propertyDefinition.PropertyType == typeof(ObjectID));
        return _relationStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition(relationEndPointDefinition);
      }
      else
      {
        return _valueStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition(propertyDefinition);
      }
    }
  }
}
