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
using System.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;

namespace Remotion.Data.DomainObjects.UnitTests.Database
{
  /// <summary>
  /// Provides a quick way to get the setup/teardown scripts for a set of types.
  /// </summary>
  public static class ScriptDumper
  {
    public static void DumpScripts (params Type[] types)
    {
      var scriptGenerator = new ScriptGenerator (
          pd => pd.Factory.CreateSchemaScriptBuilder (pd), new RdbmsStorageEntityDefinitionProvider(), new ScriptToStringConverter());
      var scripts = scriptGenerator.GetScripts (types.Select (t => MappingConfiguration.Current.GetTypeDefinition (t)));
      foreach (var script in scripts)
      {
        Console.WriteLine ("Setup:");
        Console.WriteLine ("======");
        Console.WriteLine (script.SetUpScript);
        Console.WriteLine ("TearDown:");
        Console.WriteLine ("======");
        Console.WriteLine (script.TearDownScript);
      }
    }
  }
}