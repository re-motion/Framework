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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Parameters;

/// <summary>
/// Adds a <see cref="SqlFulltextDataParameterDefinitionDecorator"/> to <see cref="IDataParameterDefinition"/>s
/// created for <see cref="string"/> or <see cref="char"/>[] values.
/// </summary>
public class SqlFulltextDataParameterDefinitionFactory : IDataParameterDefinitionFactory
{
  public IDataParameterDefinitionFactory NextDataParameterDefinitionFactory { get; }

  public SqlFulltextDataParameterDefinitionFactory (IDataParameterDefinitionFactory nextDataParameterDefinitionFactory)
  {
    ArgumentUtility.CheckNotNull(nameof(nextDataParameterDefinitionFactory), nextDataParameterDefinitionFactory);

    NextDataParameterDefinitionFactory = nextDataParameterDefinitionFactory;
  }

  public IDataParameterDefinition CreateDataParameterDefinition (QueryParameter queryParameter)
  {
    ArgumentUtility.CheckNotNull(nameof(queryParameter), queryParameter);

    var innerDataParameterDefinition = NextDataParameterDefinitionFactory.CreateDataParameterDefinition(queryParameter);
    if (queryParameter.Value is string or char[])
      return new SqlFulltextDataParameterDefinitionDecorator(innerDataParameterDefinition);

    return innerDataParameterDefinition;
  }
}
