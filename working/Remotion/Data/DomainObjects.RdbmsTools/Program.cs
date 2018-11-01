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
  public class Program
  {
    private static int Main (string[] args)
    {
      RdbmsToolsParameters rdbmsToolsParameters;
      CommandLineClassParser<RdbmsToolsParameters> parser = new CommandLineClassParser<RdbmsToolsParameters> ();
      try
      {
        rdbmsToolsParameters = parser.Parse (args);
      }
      catch (CommandLineArgumentException e)
      {
        System.Console.WriteLine (e.Message);
        System.Console.WriteLine ("Usage:");
        System.Console.WriteLine (parser.GetAsciiSynopsis (Environment.GetCommandLineArgs()[0], System.Console.BufferWidth));
        return 1;
      }

      try
      {
        RdbmsToolsRunner rdbmsToolsRunner = RdbmsToolsRunner.Create (rdbmsToolsParameters);
        rdbmsToolsRunner.Run();
      }
      catch (Exception e)
      {
        if (rdbmsToolsParameters.Verbose)
        {
          System.Console.Error.WriteLine ("Execution aborted. Exception stack:");
          for (; e != null; e = e.InnerException)
            System.Console.Error.WriteLine ("{0}: {1}\n{2}", e.GetType().FullName, e.Message, e.StackTrace);
        }
        else
        {
          System.Console.Error.WriteLine ("Execution aborted: {0}", e.Message);
        }
        return 1;
      }
      return 0;
    }
  }
}
