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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model.Building
{
  /// <summary>
  /// Decorates implementations of <see cref="IStorageTypeInformationProvider"/> and wraps the <see cref="IStorageTypeInformation"/> objects so
  /// for <see cref="string"/> properties with a <b>max</b>-size in a <see cref="SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator"/>.
  /// This is required to ensure <see cref="string"/> values remain compatible with fulltext parameters, which do not support a <b>max</b>-size.
  /// </summary>
  public class SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationProviderDecorator : IStorageTypeInformationProvider
  {
    private readonly IStorageTypeInformationProvider _innerStorageTypeInformationProvider;

    public SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationProviderDecorator (
        IStorageTypeInformationProvider innerStorageTypeInformationProvider)
    {
      ArgumentUtility.CheckNotNull ("innerStorageTypeInformationProvider", innerStorageTypeInformationProvider);

      _innerStorageTypeInformationProvider = innerStorageTypeInformationProvider;
    }

    public IStorageTypeInformationProvider InnerStorageTypeInformationProvider
    {
      get { return _innerStorageTypeInformationProvider; }
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
      var innerStorageTypeInformation = _innerStorageTypeInformationProvider.GetStorageType (propertyDefinition, forceNullable);
      return DecorateStringPropertyStorageTypeInformationWithFulltextQueryCompatiblity (innerStorageTypeInformation);
    }

    public IStorageTypeInformation GetStorageType (Type type)
    {
      var innerStorageTypeInformation = _innerStorageTypeInformationProvider.GetStorageType (type);
      return DecorateStringPropertyStorageTypeInformationWithFulltextQueryCompatiblity (innerStorageTypeInformation);
    }

    public IStorageTypeInformation GetStorageType (object value)
    {
      var innerStorageTypeInformation = _innerStorageTypeInformationProvider.GetStorageType (value);
      return DecorateStringPropertyStorageTypeInformationWithFulltextQueryCompatiblity (innerStorageTypeInformation);
    }

    private IStorageTypeInformation DecorateStringPropertyStorageTypeInformationWithFulltextQueryCompatiblity (
        IStorageTypeInformation storageTypeInformation)
    {
      ArgumentUtility.CheckNotNull ("storageTypeInformation", storageTypeInformation);

      var hasMaxLength = storageTypeInformation.StorageTypeLength == SqlStorageTypeInformationProvider.StorageTypeLengthRepresentingMax;
      var isAnsiString = storageTypeInformation.StorageDbType == DbType.AnsiString; //AnsiStringFixedLength represents char(...), which does not support char(max)
      var isUnicodeString = storageTypeInformation.StorageDbType == DbType.String; //StringFixedLength represents char(...), which does not support char(max)

      if (hasMaxLength && isAnsiString)
        return new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator (storageTypeInformation, 8000);

      if (hasMaxLength && isUnicodeString)
        return new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator (storageTypeInformation, 4000);

      return storageTypeInformation;
    }
  }
}