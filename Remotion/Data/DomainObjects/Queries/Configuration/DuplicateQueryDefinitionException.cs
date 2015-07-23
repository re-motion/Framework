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
  public class DuplicateQueryDefinitionException : DomainObjectException
  {
    private readonly QueryDefinition _queryDefinition;
    private readonly QueryDefinition _duplicate;

    public DuplicateQueryDefinitionException (QueryDefinition queryDefinition, QueryDefinition duplicate)
        : base (GetMessage (queryDefinition))
    {
      _queryDefinition = queryDefinition;
      _duplicate = duplicate;
    }

    public QueryDefinition QueryDefinition
    {
      get { return _queryDefinition; }
    }

    public QueryDefinition Duplicate
    {
      get { return _duplicate; }
    }

    private static string GetMessage (QueryDefinition queryDefinition)
    {
      return string.Format ("The query with ID '{0}' has a duplicate.", queryDefinition.ID);
    }
  }
}
