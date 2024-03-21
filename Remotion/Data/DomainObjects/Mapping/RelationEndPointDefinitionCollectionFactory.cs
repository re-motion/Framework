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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// The <see cref="RelationEndPointDefinitionCollectionFactory"/> is used to get a <see cref="RelationEndPointDefinitionCollection"/> for a 
  /// <see cref="TypeDefinition"/>
  /// </summary>
  public class RelationEndPointDefinitionCollectionFactory
  {
    private readonly IMappingObjectFactory _mappingObjectFactory;
    private readonly IMemberInformationNameResolver _nameResolver;
    private readonly IPropertyMetadataProvider _propertyMetadataProvider;

    public RelationEndPointDefinitionCollectionFactory (
        IMappingObjectFactory mappingObjectFactory,
        IMemberInformationNameResolver nameResolver,
        IPropertyMetadataProvider propertyMetadataProvider)
    {
      ArgumentUtility.CheckNotNull("mappingObjectFactory", mappingObjectFactory);
      ArgumentUtility.CheckNotNull("nameResolver", nameResolver);
      ArgumentUtility.CheckNotNull("propertyMetadataProvider", propertyMetadataProvider);

      _mappingObjectFactory = mappingObjectFactory;
      _nameResolver = nameResolver;
      _propertyMetadataProvider = propertyMetadataProvider;
    }

    public RelationEndPointDefinitionCollection CreateRelationEndPointDefinitionCollection (TypeDefinition typeDefinition)
    {
      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);

      var endPoints = new RelationEndPointDefinitionCollection();
      foreach (var propertyInfo in GetRelationPropertyInfos(typeDefinition))
      {
        var relationEndPoint = _mappingObjectFactory.CreateRelationEndPointDefinition(typeDefinition, propertyInfo);
        endPoints.Add(relationEndPoint);
      }
      return endPoints;
    }

    private IEnumerable<IPropertyInformation> GetRelationPropertyInfos (TypeDefinition typeDefinition)
    {
      RelationPropertyFinder relationPropertyFinder;
      if (typeDefinition is ClassDefinition classDefinition)
      {
        relationPropertyFinder = new RelationPropertyFinder(
            classDefinition.Type,
            classDefinition.BaseClass == null,
            true,
            _nameResolver,
            classDefinition.PersistentMixinFinder,
            _propertyMetadataProvider);
      }
      else
      {
        throw new NotSupportedException("Only class definitions are supported."); // TODO R2I Mapping: property finder support for interfaces
      }

      return relationPropertyFinder.FindPropertyInfos();
    }
  }
}
