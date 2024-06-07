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
