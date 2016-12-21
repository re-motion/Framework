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
using System.Runtime.Serialization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Configuration
{
[Serializable]
public class IdentityTypeNotSupportedException : StorageProviderConfigurationException
{
  // types

  // static members and constants

  // member fields

  private readonly Type _storageProviderDefinitionType;
  private readonly Type _invalidIdentityType;

  // construction and disposing

  public IdentityTypeNotSupportedException () : this ("Storage provider does not support provided identity type.") 
  {
  }

  public IdentityTypeNotSupportedException (Type invalidIdentityType) 
    : this(string.Format("Storage provider does not support identity values of type '{0}'.", invalidIdentityType))
  {
    ArgumentUtility.CheckNotNull ("invalidIdentityType", invalidIdentityType);

    _invalidIdentityType = invalidIdentityType;
  }

  public IdentityTypeNotSupportedException (string message) : base (message) 
  {
  }
  
  public IdentityTypeNotSupportedException (string message, Exception inner) : base (message, inner) 
  {
  }

  public IdentityTypeNotSupportedException (Type storageProviderDefinitionType, Type invalidIdentityType) 
      : this (string.Format ("The storage provider defined by '{0}' does not support identity values of type '{1}'.", storageProviderDefinitionType.Name, invalidIdentityType),
            storageProviderDefinitionType, invalidIdentityType)
  {
  }

  public IdentityTypeNotSupportedException (string message, Type storageProviderDefinitionType, Type invalidIdentityType) : base (message)
  {
    ArgumentUtility.CheckNotNull ("storageProviderDefinitionType", storageProviderDefinitionType);
    ArgumentUtility.CheckNotNull ("invalidIdentityType", invalidIdentityType);

    _storageProviderDefinitionType = storageProviderDefinitionType;
    _invalidIdentityType = invalidIdentityType;
  }
  
  protected IdentityTypeNotSupportedException (SerializationInfo info, StreamingContext context) : base (info, context) 
  {
    _storageProviderDefinitionType = (Type) info.GetValue ("StorageProviderDefinitionType", typeof (Type));
    _invalidIdentityType = (Type) info.GetValue ("InvalidIdentityType", typeof (Type));
  }

  // methods and properties

  public Type StorageProviderDefinitionType
  {
    get { return _storageProviderDefinitionType; }
  }

  public Type InvalidIdentityType
  {
    get { return _invalidIdentityType; }
  }

  public override void GetObjectData (SerializationInfo info, StreamingContext context)
  {
    base.GetObjectData (info, context);

    info.AddValue ("StorageProviderDefinitionType", _storageProviderDefinitionType);
    info.AddValue ("InvalidIdentityType", _invalidIdentityType);
  }
}
}
