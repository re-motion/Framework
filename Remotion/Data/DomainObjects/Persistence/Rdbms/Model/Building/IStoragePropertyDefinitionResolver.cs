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
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building
{
  /// <summary>
  /// <see cref="IStoragePropertyDefinitionResolver"/> defines the API for classes that calculate the <see cref="IRdbmsStoragePropertyDefinition"/> 
  /// objects for a <see cref="ClassDefinition"/>.
  /// The <see cref="IStoragePropertyDefinitionResolver"/> only returns the <see cref="IRdbmsStoragePropertyDefinition"/> instances, it doesn't create
  /// any.
  /// </summary>
  public interface IStoragePropertyDefinitionResolver
  {
    IEnumerable<IRdbmsStoragePropertyDefinition> GetStoragePropertiesForHierarchy (ClassDefinition classDefinition);
  }
}