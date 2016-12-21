﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Xml.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.MappingExport
{
  /// <summary>
  /// Default implementation of the <see cref="IStorageProviderSerializer"/> interface.
  /// </summary>
  public class StorageProviderSerializer : IStorageProviderSerializer
  {
    private readonly IClassSerializer _classSerializer;

    public StorageProviderSerializer (IClassSerializer classSerializer)
    {
      ArgumentUtility.CheckNotNull ("classSerializer", classSerializer);

      _classSerializer = classSerializer;
    }

    public IClassSerializer ClassSerializer
    {
      get { return _classSerializer; }
    }

    public XElement Serialize (IEnumerable<ClassDefinition> classDefinitions, RdbmsProviderDefinition providerDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);
      ArgumentUtility.CheckNotNull ("providerDefinition", providerDefinition);

      return new XElement (Constants.Namespace + "storageProvider",
              new XAttribute("name", providerDefinition.Name),
              classDefinitions.Select (c => _classSerializer.Serialize (c)));
    }
  }
}