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
using System.IO;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;

namespace Remotion.UnitTests.Reflection.TypeDiscovery.TestAssemblies.FileLoadExceptionConsoleApplication
{
  public class Program
  {
    public static int Main (string[] args)
    {
      string shadowCopying = args[1];

      AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
      setup.ShadowCopyFiles = shadowCopying;

      AppDomain newDomain = AppDomain.CreateDomain ("FileLoadExceptionConsoleApplication_AppDomain", null, setup);
      newDomain.DoCallBack (Callback);
      
      return 99;
    }

    public static void Callback ()
    {
      Remotion.Logging.LogManager.InitializeConsole ();

      string path = Environment.GetCommandLineArgs ()[1];
      FilteringAssemblyLoader loader = new FilteringAssemblyLoader (ApplicationAssemblyLoaderFilter.Instance);
      try
      {
        if (loader.TryLoadAssembly (path) == null)
          Environment.Exit (0);
        else
        {
          Console.WriteLine ("Assembly was loaded, but should not be loaded.");
          Environment.Exit (3);
        }
      }
      catch (FileLoadException ex)
      {
        Console.WriteLine (ex.Message);
        Environment.Exit (1);
      }
      catch (Exception ex)
      {
        Console.WriteLine (ex.Message);
        Environment.Exit (2);
      }
    }
  }
}
