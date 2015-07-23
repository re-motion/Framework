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
using System.Globalization;
using Remotion.Tools.Console.CommandLine;

namespace Remotion.Security.Metadata.Extractor
{
  public class Program
  {
    public static int Main (string[] args)
    {
      CommandLineArguments arguments = GetArguments (args);
      if (arguments == null)
        return 1;

      return ExtractMetadata (arguments);
    }

    private static CommandLineArguments GetArguments (string[] args)
    {
      CommandLineClassParser parser = new CommandLineClassParser (typeof (CommandLineArguments));

      try
      {
        return (CommandLineArguments) parser.Parse (args);
      }
      catch (CommandLineArgumentException e)
      {
        Console.WriteLine (e.Message);
        WriteUsage (parser);

        return null;
      }
    }

    private static void WriteUsage (CommandLineClassParser parser)
    {
      Console.WriteLine ("Usage:");

      string commandName = Environment.GetCommandLineArgs ()[0];
      Console.WriteLine (parser.GetAsciiSynopsis (commandName, Console.BufferWidth));
    }

    private static int ExtractMetadata (CommandLineArguments arguments)
    {
      try
      {
        MetadataExtractor extractor = new MetadataExtractor (GetMetadataConverter (arguments));

        extractor.AddAssembly (arguments.DomainAssemblyName);
        extractor.Save (arguments.MetadataOutputFile);
      }
      catch (Exception e)
      {
        HandleException (e, arguments.Verbose);
        return 1;
      }

      return 0;
    }

    private static void HandleException (Exception exception, bool verbose)
    {
      if (verbose)
      {
        Console.Error.WriteLine ("Execution aborted. Exception stack:");

        for (; exception != null; exception = exception.InnerException)
        {
          Console.Error.WriteLine ("{0}: {1}\n{2}", exception.GetType ().FullName, exception.Message, exception.StackTrace);
        }
      }
      else
      {
        Console.Error.WriteLine ("Execution aborted: {0}", exception.Message);
      }
    }

    private static IMetadataConverter GetMetadataConverter (CommandLineArguments arguments)
    {
      MetadataConverterBuilder converterBuilder = new MetadataConverterBuilder ();

      if (arguments.Languages.Length > 0)
      {
        string[] languages = arguments.Languages.Split (',');
        foreach (string language in languages)
          converterBuilder.AddLocalization (language);
      }

      if (arguments.InvariantCulture)
        converterBuilder.AddLocalization (CultureInfo.InvariantCulture);

      converterBuilder.ConvertMetadataToXml = !arguments.SuppressMetadata;
      return converterBuilder.Create ();
    }
  }
}
