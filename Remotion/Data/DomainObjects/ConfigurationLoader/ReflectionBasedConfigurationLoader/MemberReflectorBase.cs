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
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Base class for reflecting on the properties and relations of a class.</summary>
  public abstract class MemberReflectorBase
  {
    private readonly ClassDefinition _classDefinition;
    private readonly IPropertyInformation _propertyInfo;
    private readonly IMemberInformationNameResolver _nameResolver;
    private readonly IPropertyMetadataProvider _propertyMetadataProvider;

    protected MemberReflectorBase (
        ClassDefinition classDefinition,
        IPropertyInformation propertyInfo,
        IMemberInformationNameResolver nameResolver,
        IPropertyMetadataProvider propertyMetadataProvider)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNull ("nameResolver", nameResolver);
      ArgumentUtility.CheckNotNull ("propertyMetadataProvider", propertyMetadataProvider);

      _classDefinition = classDefinition;
      _propertyInfo = propertyInfo;
      _nameResolver = nameResolver;
      _propertyMetadataProvider = propertyMetadataProvider;
    }

    public ClassDefinition ClassDefinition
    {
      get { return _classDefinition; }
    }

    public IPropertyInformation PropertyInfo
    {
      get { return _propertyInfo; }
    }

    public IMemberInformationNameResolver NameResolver
    {
      get { return _nameResolver; }
    }

    protected IPropertyMetadataProvider PropertyMetadataProvider
    {
      get { return _propertyMetadataProvider; }
    }

    public StorageClass GetStorageClass()
    {
      return _propertyMetadataProvider.GetStorageClass (_propertyInfo) ?? GetDefaultStorageClass();
    }

    private StorageClass GetDefaultStorageClass ()
    {
      switch (_classDefinition.DefaultStorageClass)
      {
        case DefaultStorageClass.Persistent:
          return StorageClass.Persistent;
        case DefaultStorageClass.Transaction:
          return StorageClass.Transaction;
        default:
          throw new NotImplementedException ($"{nameof (DefaultStorageClass)} '{_classDefinition.DefaultStorageClass}' is not supported.");
      }
    }

    protected string GetPropertyName ()
    {
      return _nameResolver.GetPropertyName (PropertyInfo);
    }
    
  }
}
