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
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.Tools.Console.ConsoleApplication;

namespace Remotion.SecurityManager.AclTools.Expander
{

  public class Program
  {
    public static int Main (string[] args)
    {
      var consoleApplication = new ConsoleApplication<AclExpanderApplication, AclExpanderApplicationSettings>();
      return consoleApplication.Main(args);
    }
  }
}
