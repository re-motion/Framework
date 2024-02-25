// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using Remotion.Tools.Console.CommandLine;

namespace Remotion.SecurityManager.Metadata.Importer
{
  public class CommandLineArguments
  {
    [CommandLineStringArgument("connectionString", false, Placeholder = "Integrated Security=SSPI;Initial Catalog=DemoDB;Data Source=localhost", Description = "The SQL Server connection string.")]
    public string ConnectionString = "Integrated Security=SSPI;Initial Catalog=DemoDB;Data Source=localhost";

    [CommandLineModeArgument(false)]
    public OperationMode Mode;

    [CommandLineStringArgument(false,
        Description = "The name of the XML metadata file.",
        Placeholder = "metadata")]
    public string MetadataFile = string.Empty;

    [CommandLineFlagArgument("verbose", false,
        Description = "Verbose output")]
    public bool Verbose;

    public bool ImportMetadata
    {
      get { return (Mode & OperationMode.Metadata) != 0; }
    }

    public bool ImportLocalization
    {
      get { return (Mode & OperationMode.Localization) != 0; }
    }
  }
}
