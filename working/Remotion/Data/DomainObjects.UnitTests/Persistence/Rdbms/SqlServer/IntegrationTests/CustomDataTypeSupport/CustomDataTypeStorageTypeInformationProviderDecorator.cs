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
using System.Data;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests.CustomDataTypeSupport.TestDomain;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests.CustomDataTypeSupport
{
  public class CustomDataTypeStorageTypeInformationProviderDecorator : IStorageTypeInformationProvider
  {
    private readonly IStorageTypeInformationProvider _innerStorageTypeInformationProvider;

    public CustomDataTypeStorageTypeInformationProviderDecorator (IStorageTypeInformationProvider innerStorageTypeInformationProvider)
    {
      ArgumentUtility.CheckNotNull ("innerStorageTypeInformationProvider", innerStorageTypeInformationProvider);

      _innerStorageTypeInformationProvider = innerStorageTypeInformationProvider;
    }

    public IStorageTypeInformation GetStorageTypeForID (bool isStorageTypeNullable)
    {
      return _innerStorageTypeInformationProvider.GetStorageTypeForID (isStorageTypeNullable);
    }

    public IStorageTypeInformation GetStorageTypeForSerializedObjectID (bool isStorageTypeNullable)
    {
      return _innerStorageTypeInformationProvider.GetStorageTypeForSerializedObjectID (isStorageTypeNullable);
    }

    public IStorageTypeInformation GetStorageTypeForClassID (bool isStorageTypeNullable)
    {
      return _innerStorageTypeInformationProvider.GetStorageTypeForClassID (isStorageTypeNullable);
    }

    public IStorageTypeInformation GetStorageTypeForTimestamp (bool isStorageTypeNullable)
    {
      return _innerStorageTypeInformationProvider.GetStorageTypeForTimestamp (isStorageTypeNullable);
    }

    public IStorageTypeInformation GetStorageType (PropertyDefinition propertyDefinition, bool forceNullable)
    {
      if (typeof (SimpleDataType).IsAssignableFrom (propertyDefinition.PropertyType))
        return CreateSimpleDataTypeStorageTypeInformation (propertyDefinition.IsNullable, propertyDefinition.MaxLength);
      return _innerStorageTypeInformationProvider.GetStorageType (propertyDefinition, forceNullable);
    }

    public IStorageTypeInformation GetStorageType (Type type)
    {
      if (typeof (SimpleDataType).IsAssignableFrom (type))
        return CreateSimpleDataTypeStorageTypeInformation (isStorageTypeNullable: true, maxLength: null);
      return _innerStorageTypeInformationProvider.GetStorageType (type);
    }

    public IStorageTypeInformation GetStorageType (object value)
    {
      if (value is SimpleDataType)
        return CreateSimpleDataTypeStorageTypeInformation (isStorageTypeNullable: true, maxLength: null);
      return _innerStorageTypeInformationProvider.GetStorageType (value);
    }

    private IStorageTypeInformation CreateSimpleDataTypeStorageTypeInformation (bool isStorageTypeNullable, int? maxLength)
    {
      return new SimpleDataTypeStorageTypeInformation (
          maxLength.HasValue ? string.Format ("nvarchar ({0})", maxLength.Value) : "nvarchar (max)",
          DbType.String,
          isStorageTypeNullable,
          maxLength);
    }
  }
}