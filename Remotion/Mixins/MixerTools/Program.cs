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
using Remotion.Reflection;
using Remotion.Tools.Console;
using Remotion.Tools.Console.CommandLine;

namespace Remotion.Mixins.MixerTools
{
  class Program
  {
    static int Main (string[] args)
    {
      MixerParameters parameters;
      CommandLineClassParser<MixerParameters> parser = new CommandLineClassParser<MixerParameters>();
      try
      {
        parameters = parser.Parse(args);
      }
      catch (CommandLineArgumentException e)
      {
        System.Console.WriteLine(e.Message);
        System.Console.WriteLine("Usage:");
        System.Console.WriteLine(parser.GetAsciiSynopsis(Environment.GetCommandLineArgs()[0], System.Console.BufferWidth));
        return 1;
      }

      try
      {
        throw new PlatformNotSupportedException("MixerTools is not supported on this platform");
#pragma warning disable CS0162 // Unreachable code detected

        // TODO: re-enable nuget packaging in project file.
        MixerRunner mixerRunner = new MixerRunner(parameters);
        mixerRunner.Run();
      }
      catch (Exception e)
      {
        using (ConsoleUtility.EnterColorScope(ConsoleColor.White, ConsoleColor.DarkRed))
        {
          System.Console.Error.WriteLine("Execution aborted. Exception stack:");
          for (; e != null; e = e.InnerException)
            System.Console.Error.WriteLine("{0}: {1}\n{2}", e.GetType().GetFullNameSafe(), e.Message, e.StackTrace);
        }
        return 1;
      }
      return 0;
    }
  }
}
