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
using Remotion.Tools.Console.ConsoleApplication;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class AclExpanderApplicationSettings : ConsoleApplicationSettings
  {
    [CommandLineStringArgument("connectionString", false, Placeholder = "Integrated Security=SSPI;Initial Catalog=DemoDB;Data Source=localhost", Description = "The SQL Server connection string.")]
    public string ConnectionString = string.Empty;

    [CommandLineStringArgument("user", true, Placeholder = "accountants/john.doe", Description = "Fully qualified name of user(s) to query access types for.")]
    public string? UserName;

    [CommandLineStringArgument("last", true, Placeholder = "Doe", Description = "Last name of user(s) to query access types for.")]
    public string? UserLastName;

    [CommandLineStringArgument("first", true, Placeholder = "John", Description = "First name of user(s) to query access types for.")]
    public string? UserFirstName;

    [CommandLineStringArgument("dir", true, Placeholder = "c:\\temp", Description = "Directory the ACL-expansion gets written to (e.g. /dir:c:\\temp).")]
    public string Directory = ".";

    [CommandLineStringArgument("culture", true, Placeholder = "", Description = "Culture to set for output (e.g. /culture:en-US, /culture:de-AT).")]
    public string CultureName = "de-AT";

    [CommandLineFlagArgument("denied", true, Description = "Output the denied access rights (e.g. /denied+).")]
    public bool OutputDeniedRights;

    [CommandLineFlagArgument("multifile", false, Description = "Create a single file for all users + permissions or a master file and several detail files.")]
    public bool UseMultipleFileOutput;

    [CommandLineFlagArgument("rc", false, Description = "Output row count for user, role.... (/multifile- only)")]
    public bool OutputRowCount;
  }
}
