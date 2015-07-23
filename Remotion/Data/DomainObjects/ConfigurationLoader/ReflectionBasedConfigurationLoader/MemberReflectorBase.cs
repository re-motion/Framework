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
    public const StorageClass DefaultStorageClass = StorageClass.Persistent;

    private readonly IPropertyInformation _propertyInfo;
    private readonly IMemberInformationNameResolver _nameResolver;
    private readonly IPropertyMetadataProvider _propertyMetadataProvider;
    private readonly StorageClassAttribute _storageClassAttribute;

    protected MemberReflectorBase (
        IPropertyInformation propertyInfo,
        IMemberInformationNameResolver nameResolver,
        IPropertyMetadataProvider propertyMetadataProvider)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNull ("nameResolver", nameResolver);
      ArgumentUtility.CheckNotNull ("propertyMetadataProvider", propertyMetadataProvider);

      _propertyInfo = propertyInfo;
      _nameResolver = nameResolver;
      _propertyMetadataProvider = propertyMetadataProvider;
      _storageClassAttribute = PropertyInfo.GetCustomAttribute<StorageClassAttribute> (true);
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

    public StorageClassAttribute StorageClassAttribute
    {
      get { return _storageClassAttribute; }
    }

    public StorageClass StorageClass
    {
      get { return StorageClassAttribute != null ? StorageClassAttribute.StorageClass : DefaultStorageClass; }
    }

    protected string GetPropertyName ()
    {
      return _nameResolver.GetPropertyName (PropertyInfo);
    }
    
  }
}
