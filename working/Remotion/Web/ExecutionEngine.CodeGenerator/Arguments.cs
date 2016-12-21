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

namespace Remotion.Web.ExecutionEngine.CodeGenerator
{
  public enum Language
  {
    CSharp,
    VB
  }
  
  public class Arguments
  {
    [CommandLineStringArgument (false,
        Description = "File name or file mask for the input file(s)",
        Placeholder = "filemask")]
    public string FileMask;

    [CommandLineStringArgument (false,
        Description = "Output file",
        Placeholder = "outputfile")]
    public string OutputFile;

    [CommandLineFlagArgument ("recursive", false,
        Description = "Resolve file mask recursively (default is off)")]
    public bool Recursive;

    [CommandLineEnumArgument ("language", true, 
        Description = "Language (default is CSharp)",
        Placeholder = "{CSharp|VB}")]
    public Language Language = Language.CSharp;

    //[CommandLineStringArgument ("lineprefix", true,
    //    Description = "Line prefix for WxePageFunction elements (default is // for C#, ' for VB.NET")]
    //public string LinePrefix = null;

    //[CommandLineStringArgument ("using", true,
    //    Description = "Regular expression for parsing namespace import statements")]
    //public string UsingExpression = null;

    [CommandLineFlagArgument ("verbose", false,
        Description = "Verbose error information (default is off)")]
    public bool Verbose;

    [CommandLineStringArgument ("prjfile", true,
        Description = "Visual Studio project file (csprj). If specified, the output file is only generated if any of the input files OR the project file is newer than the output file.")]
    public string ProjectFile;

    [CommandLineStringArgument ("functionbase", true,
        Description = "Default base type for generated WXE functions (default is Remotion.Web.ExecutionEngine.WxeFunction).")]
    public string FunctionBaseType = typeof(WxeFunction).FullName;
  }
}
