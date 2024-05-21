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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  public class ExtendedScriptElementBuilder : IScriptBuilder
  {
    private readonly IScriptBuilder _innerScriptBuilder;

    public ExtendedScriptElementBuilder (IScriptBuilder innerScriptBuilder)
    {
      ArgumentUtility.CheckNotNull("innerScriptBuilder", innerScriptBuilder);

      _innerScriptBuilder = innerScriptBuilder;
    }

    public void AddEntityDefinition (IRdbmsStorageEntityDefinition entityDefinition)
    {
      ArgumentUtility.CheckNotNull("entityDefinition", entityDefinition);

      _innerScriptBuilder.AddEntityDefinition(entityDefinition);
    }

    public void AddStructuredTypeDefinition (IRdbmsStructuredTypeDefinition typeDefinition)
    {
      ArgumentUtility.CheckNotNull(nameof(typeDefinition), typeDefinition);

      _innerScriptBuilder.AddStructuredTypeDefinition(typeDefinition);
    }

    public IScriptElement GetCreateScript ()
    {
     return new ScriptElementCollection();
    }

    public IScriptElement GetDropScript ()
    {
      return new ScriptElementCollection();
    }
  }
}
