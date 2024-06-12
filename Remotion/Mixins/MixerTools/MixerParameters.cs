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

namespace Remotion.Mixins.MixerTools
{
#if NETFRAMEWORK
  [Serializable]
#endif  
  public class MixerParameters
  {
    [CommandLineStringArgument("baseDirectory", true,
        Description = "The base directory to use for looking up the files to be processed (default: current).",
        Placeholder = "directory")]
    public string BaseDirectory = Environment.CurrentDirectory;

    [CommandLineStringArgument("config", true,
        Description =
            "The config file holding the application's configuration. "
            + "Unless the path is rooted, the config file is located relative to the baseDirectory.",
        Placeholder = "app.config")]
    public string ConfigFile = string.Empty;

    [CommandLineStringArgument("assemblyDirectory", true,
        Description = "Create assembly file(s) in this directory (default: current).",
        Placeholder = "directory")]
    public string AssemblyOutputDirectory = Environment.CurrentDirectory;

    [CommandLineStringArgument("assemblyName", true,
        Description = "The simple name of the assembly generated (without extension; default: Remotion.Mixins.Persistent.{counter}).",
        Placeholder = "simpleName")]
    public string AssemblyName = "Remotion.Mixins.Persistent.{counter}";

    [CommandLineStringArgument("degreeOfParallelism", true,
        Description = "The maximum number of threads on which the code generation will be distributed. "
                      + "If a number greater than 1 is specified, the AssemblyName parameter must contain the {counter} placeholder. (default: 1).",
        Placeholder = "1")]
    public int DegreeOfParallelism = 1;

    [CommandLineFlagArgument("verbose", false,
        Description = "Enables verbose output. Verbose output will include all messages from all loggers in the framework.")]
    public bool Verbose;
  }
}
