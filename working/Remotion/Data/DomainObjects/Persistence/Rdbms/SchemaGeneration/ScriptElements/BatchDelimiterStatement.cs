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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements
{
  /// <summary>
  /// The <see cref="BatchDelimiterStatement"/> adds a batch delimiter to a script-statement for a relational database.
  /// </summary>
  public class BatchDelimiterStatement : IScriptElement
  {
    private readonly string _delimiter;

    public BatchDelimiterStatement (string delimiter)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("delimiter", delimiter);

      _delimiter = delimiter;
    }

    public string Delimiter
    {
      get { return _delimiter; }
    }

    public void AppendToScript (List<ScriptStatement> script)
    {
      ArgumentUtility.CheckNotNull ("script", script);
 
      var lastStatement = script.LastOrDefault();
      if (lastStatement != null && lastStatement.Statement != _delimiter)
        script.Add (new ScriptStatement (_delimiter));
    }
  }
}