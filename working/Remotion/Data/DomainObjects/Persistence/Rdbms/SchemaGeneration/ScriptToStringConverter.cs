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
using System.Linq;
using System.Text;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration
{
  /// <summary>
  /// The <see cref="ScriptToStringConverter"/> is responsible to convert the script statements, that are returned by the <see cref="IScriptBuilder"/>
  /// instance, to strings.
  /// </summary>
  public class ScriptToStringConverter : IScriptToStringConverter
  {
    public ScriptPair Convert (IScriptBuilder scriptBuilder)
    {
      ArgumentUtility.CheckNotNull ("scriptBuilder", scriptBuilder);

      var createScriptStatements = new List<ScriptStatement> ();
      var dropScriptStatements = new List<ScriptStatement> ();
      var createScriptCollection = scriptBuilder.GetCreateScript ();
      var dropScriptCollection = scriptBuilder.GetDropScript ();

      createScriptCollection.AppendToScript (createScriptStatements);
      dropScriptCollection.AppendToScript (dropScriptStatements);

      return new ScriptPair (
          createScriptStatements.Aggregate (new StringBuilder (), (sb, stmt) => sb.AppendLine (stmt.Statement)).ToString (),
          dropScriptStatements.Aggregate (new StringBuilder (), (sb, stmt) => sb.AppendLine (stmt.Statement)).ToString ());
    }
  }
}