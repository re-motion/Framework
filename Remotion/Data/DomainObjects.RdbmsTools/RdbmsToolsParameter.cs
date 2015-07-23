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
using Remotion.Tools.Console.CommandLine;

namespace Remotion.Data.DomainObjects.RdbmsTools
{
  [Flags]
  public enum OperationMode
  {
    [CommandLineMode ("schema", Description = "Generate the database setup script(s).")]
    BuildSchema = 1,

    [CommandLineMode ("exportMappingXml", Description = "Export mapping as a xml file.")]
    ExportMappingXml = 2,
  }

  /// <summary>
  /// <see cref="RdbmsToolsParameters"/> type is a combination of a parameter object for <see cref="RdbmsToolsRunner"/> 
  /// and a command line arguments class as required by <see cref="CommandLineClassParser"/>.
  /// </summary>
  [Serializable]
  public class RdbmsToolsParameters
  {
    [CommandLineModeArgument (false)]
    public OperationMode Mode = OperationMode.BuildSchema;

    [CommandLineStringArgument ("baseDirectory", true,
        Description = "The base directory to use for looking up the files to be processed (default: current).",
        Placeholder = "directory")]
    public string BaseDirectory = Environment.CurrentDirectory;

    [CommandLineStringArgument ("config", true,
        Description = 
            "The config file holding the application's configuration. "
            + "Unless the path is rooted, the config file is located relative to the current directory.",
        Placeholder = "app.config")]
    public string ConfigFile = string.Empty;

    [CommandLineStringArgument ("schemaDirectory", true,
        Description = "Create schema file(s) in this directory (default: current).",
        Placeholder = "directory")]
    public string SchemaOutputDirectory = string.Empty;

    [CommandLineStringArgument ("exportOutputFile", true,
        Description = "The output filename for the mapping export.",
        Placeholder = "filename")]
    public string MappingExportOutputFileName = string.Empty;

    //TODO: remove parameter (1.13.84)
    [CommandLineStringArgument ("schemaBuilder", true,
        Description = "This parameter is obsolete and should no longer be used. (The schema file builder is now retrieved from the storage provider definition.)",
        Placeholder = "Namespace.ClassName,AssemblyName")]
    public string SchemaFileBuilderTypeName;

    [CommandLineFlagArgument ("verbose", false,
        Description = "Verbose output")]
    public bool Verbose;

    public RdbmsToolsParameters ()
    {
    }
  }
}
