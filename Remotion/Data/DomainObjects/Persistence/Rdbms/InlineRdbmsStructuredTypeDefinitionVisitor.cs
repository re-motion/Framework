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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  /// <summary>
  /// Visits the given <see cref="IRdbmsStructuredTypeDefinition"/> and executes a handler based on the implementation type.
  /// </summary>
  public class InlineRdbmsStructuredTypeDefinitionVisitor
  {
    public static void Visit (IRdbmsStructuredTypeDefinition typeDefinition, Action<SqlTableTypeDefinition, Action<IRdbmsStructuredTypeDefinition>> tableTypeDefinitionHandler)
    {
      ArgumentUtility.CheckNotNull(nameof(typeDefinition), typeDefinition);

      var visitor = new RdbmsStructuredTypeDefinitionVisitor(tableTypeDefinitionHandler);

      typeDefinition.Accept(visitor);
    }

    private class RdbmsStructuredTypeDefinitionVisitor : IRdbmsStructuredTypeDefinitionVisitor
    {
      private readonly Action<SqlTableTypeDefinition, Action<IRdbmsStructuredTypeDefinition>> _tableTypeDefinitionHandler;

      public RdbmsStructuredTypeDefinitionVisitor (Action<SqlTableTypeDefinition, Action<IRdbmsStructuredTypeDefinition>> tableTypeDefinitionHandler)
      {
        ArgumentUtility.CheckNotNull(nameof(tableTypeDefinitionHandler), tableTypeDefinitionHandler);

        _tableTypeDefinitionHandler = tableTypeDefinitionHandler;
      }

      public void VisitTableTypeDefinition (SqlTableTypeDefinition tableTypeDefinition)
      {
        ArgumentUtility.CheckNotNull(nameof(tableTypeDefinition), tableTypeDefinition);

        _tableTypeDefinitionHandler(tableTypeDefinition, ContinueWithNextType);
      }

      private void ContinueWithNextType (IRdbmsStructuredTypeDefinition typeDefinition)
      {
        typeDefinition.Accept(this);
      }
    }
  }
}
