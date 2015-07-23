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

namespace Remotion.Security.Metadata.Extractor
{
  public class CommandLineArguments
  {
    [CommandLineStringArgument ("assembly", false,
        Description="The path to the assembly containing the application domain to analyze.",
        Placeholder="assemblyPath")]
    public string DomainAssemblyName;

    [CommandLineStringArgument ("output", false,
        Description = "The name of the XML metadata output file.",
        Placeholder = "metadata")]
    public string MetadataOutputFile;

    [CommandLineStringArgument ("language", true,
        Description="The language code for the multilingual descriptions of the metadata objects.",
        Placeholder="language")]
    public string Languages = string.Empty;

    [CommandLineFlagArgument ("suppress", false,
        Description = "Suppress export of metadata file.")]
    public bool SuppressMetadata = false;

    [CommandLineFlagArgument ("invariant", false,
        Description = "Export multilingual descriptions of the metadata objects for the invariant culture.")]
    public bool InvariantCulture;

    [CommandLineFlagArgument ("verbose", false,
        Description = "Verbose output")]
    public bool Verbose;

    public void CheckArguments ()
    {
      if (string.IsNullOrEmpty (Languages) && SuppressMetadata && !InvariantCulture)
        throw new CommandLineArgumentApplicationException ("You must export at least a localization file or a metadata file.");
    }
  }
}
