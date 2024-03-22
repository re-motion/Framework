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

namespace Remotion.Data.DomainObjects.Queries.Configuration
{
  [Obsolete("QueryDefinitionCollection is no longer supported. (Version 6.0.0)", true)]
  public abstract class QueryDefinitionCollection
  {
    public QueryDefinitionCollection ()
    {
      throw new NotSupportedException("QueryDefinitionCollection is no longer supported. (Version 6.0.0)");
    }

    public QueryDefinitionCollection (QueryDefinitionCollection collection, bool makeCollectionReadOnly)
    {
      throw new NotSupportedException("QueryDefinitionCollection is no longer supported. (Version 6.0.0)");
    }

    public QueryDefinition GetMandatory (string queryID) => throw new NotSupportedException("QueryDefinitionCollection is no longer supported. (Version 6.0.0)");

    public void Merge (QueryDefinitionCollection source) => throw new NotSupportedException("QueryDefinitionCollection is no longer supported. (Version 6.0.0)");

    public bool Contains (QueryDefinition queryDefinition) => throw new NotSupportedException("QueryDefinitionCollection is no longer supported. (Version 6.0.0)");

    public bool Contains (string queryID) => throw new NotSupportedException("QueryDefinitionCollection is no longer supported. (Version 6.0.0)");

    public QueryDefinition this [int index]
    {
      get => throw new NotSupportedException("QueryDefinitionCollection is no longer supported. (Version 6.0.0)");
    }

    public QueryDefinition? this [string queryID]
    {
      get => throw new NotSupportedException("QueryDefinitionCollection is no longer supported. (Version 6.0.0)");
    }

    public int Add (QueryDefinition queryDefinition) => throw new NotSupportedException("QueryDefinitionCollection is no longer supported. (Version 6.0.0)");
  }
}
