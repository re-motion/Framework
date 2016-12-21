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

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// The <see cref="RelationDefinitionCollectionFactory"/> is used to get a sequence of <see cref="RelationDefinition"/> objects for a set of 
  /// <see cref="ClassDefinition"/>s.
  /// sets base classes and derived classes correctly.
  /// </summary>
  public class RelationDefinitionCollectionFactory
  {
    private readonly IMappingObjectFactory _mappingObjectFactory;

    public RelationDefinitionCollectionFactory (IMappingObjectFactory mappingObjectFactory)
    {
      ArgumentUtility.CheckNotNull ("mappingObjectFactory", mappingObjectFactory);

      _mappingObjectFactory = mappingObjectFactory;
    }

    public RelationDefinition[] CreateRelationDefinitionCollection (IDictionary<Type, ClassDefinition> classDefinitions)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);

      var relationDefinitions = new Dictionary<string, RelationDefinition>();
      foreach (var classDefinition in classDefinitions.Values)
        GetRelationDefinitions (classDefinitions, classDefinition, relationDefinitions);

      return relationDefinitions.Values.ToArray();
    }

    private void GetRelationDefinitions (
        IDictionary<Type, ClassDefinition> classDefinitions,
        ClassDefinition classDefinition,
        IDictionary<string, RelationDefinition> relationDefinitions)
    {
      foreach (var endPoint in classDefinition.MyRelationEndPointDefinitions)
      {
        var relationDefinition = _mappingObjectFactory.CreateRelationDefinition (classDefinitions, classDefinition, endPoint.PropertyInfo);
        if (!relationDefinitions.ContainsKey (relationDefinition.ID))
        {
          relationDefinition.EndPointDefinitions[0].SetRelationDefinition (relationDefinition);
          relationDefinition.EndPointDefinitions[1].SetRelationDefinition (relationDefinition);

          relationDefinitions.Add (relationDefinition.ID, relationDefinition);
        }
      }
    }
  }
}