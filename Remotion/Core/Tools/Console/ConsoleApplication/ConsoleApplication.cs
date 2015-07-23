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
using System.Diagnostics;
using System.IO;
using Remotion.Tools.Console.CommandLine;

namespace Remotion.Tools.Console.ConsoleApplication
{
  /// <summary>
  /// Console application class: Supplies command line parsing (including standard command line switches; 
  /// see <see cref="ConsoleApplicationSettings"/>) and standardized error handling and output.
  /// </summary>
  /// <remarks>
  /// Standard command line switches: "/?" to show the usage information, "/wait+" to wait for a keypress at the end of program execution.
  /// </remarks>
  /// <example>
  /// <code>
  /// <![CDATA[
  /// public class Program 
  /// {
  ///   public static int Main (string[] args)
  ///   {
  ///     var consoleApplication = new ConsoleApplication<AclExpanderApplication, AclExpanderApplicationSettings>();
  ///     return consoleApplication.Main (args);
  ///   }
  /// }
  /// ]]>
  /// </code>
  /// </example>
  /// <typeparam name="TApplication">The application implementation class. Supplied with an log- and error-output-stream
  /// by the <see cref="ConsoleApplication"/>.
  /// Needs to implement <see cref="IApplicationRunner{TApplicationSettings}"/>.
  /// </typeparam>
  /// <typeparam name="TApplicationSettings">The settings for the <typeparamref name="TApplication"/>. 
  /// Needs to derive from <see cref="ConsoleApplicationSettings"/>.
  /// </typeparam>

  public class ConsoleApplication<TApplication, TApplicationSettings> 
      where TApplication: IApplicationRunner<TApplicationSettings>, new()
      where TApplicationSettings : ConsoleApplicationSettings, new()
  {
    private readonly TextWriter _errorWriter;
    private readonly TextWriter _logWriter;
    private readonly CommandLineClassParser<TApplicationSettings> _parser = new CommandLineClassParser<TApplicationSettings> ();
    private readonly int _bufferWidth;
    private readonly IWaiter _waitAtEnd;
    private string _synopsis = "(Application synopsis not yet retrieved)";

    public ConsoleApplication (TextWriter errorWriter, TextWriter logWriter, int bufferWidth, IWaiter waitAtEnd)
    {
      _errorWriter = errorWriter;
      _logWriter = logWriter;
      _bufferWidth = bufferWidth;
      _waitAtEnd = waitAtEnd;
    }

    public ConsoleApplication () : this (System.Console.Error, System.Console.Out, 80, new ConsoleKeypressWaiter()) {}

    public TApplicationSettings Settings { get; set; }


    public int Main (string[] args)
    {
      ParseSynopsis (args);
      int result = ParseCommandLineArguments (args);
      if (result == 0) {
        result = ConsoleApplicationMain ();
      }
      WaitForKeypress();
      return result;
    }

    private int ConsoleApplicationMain ()
    {
      int result = 0;
      if (Settings.Mode == ConsoleApplicationSettings.ShowUsageMode.ShowUsage)
      {
        OutputApplicationUsage();
      }
      else
      {
        result = RunApplication (Settings);
      }
      return result;
    }


    public string Synopsis
    {
      get { return _synopsis; }
    }

 
    public int BufferWidth
    {
      get { return _bufferWidth; }
    }


    private void OutputApplicationUsage ()
    {
      _logWriter.WriteLine();
      _logWriter.WriteLine ();
      _logWriter.WriteLine ("Application Usage: ");
      _logWriter.WriteLine ();
      _logWriter.WriteLine (Synopsis);
      _logWriter.WriteLine ();
    }

    private void WaitForKeypress ()
    {
      if (Settings.WaitForKeypress)
      {
        _logWriter.WriteLine ();
        _logWriter.WriteLine ();
        _logWriter.WriteLine ("Press any-key...");
        _waitAtEnd.Wait();
      }
    }

    public virtual int RunApplication (TApplicationSettings settings)
    {
      try
      {
        var application = CreateApplication();
        application.Run (settings, _errorWriter, _logWriter);
      }
      catch (Exception e)
      {
        //_result = 1;
        using (ConsoleUtility.EnterColorScope (ConsoleColor.White, ConsoleColor.DarkRed))
        {
          _errorWriter.WriteLine ("Execution aborted. Exception stack:");
          for (; e != null; e = e.InnerException)
          {
            _errorWriter.Write (e.GetType ().FullName);
            _errorWriter.Write (": ");
            _errorWriter.WriteLine (e.Message);
            _errorWriter.WriteLine (e.StackTrace);
            _errorWriter.WriteLine();
          }
        }
        return 1;
      }
      return 0;
    }


    public virtual IApplicationRunner<TApplicationSettings> CreateApplication ()
    {
      return new TApplication();
    }


    public virtual int ParseCommandLineArguments (string[] args)
    {
      try
      {
        Settings = _parser.Parse (args);
      }
      catch (CommandLineArgumentException e)
      {
        _errorWriter.WriteLine ();
        _errorWriter.Write ("An error occured: ");
        _errorWriter.WriteLine (e.Message);
        OutputApplicationUsage ();
        Settings = new TApplicationSettings(); // Use default settings
        return 1;
      }
      return 0;
    }

    public void ParseSynopsis (string[] args)
    {
      try
      {
        string applicationName = Path.GetFileName (Process.GetCurrentProcess().MainModule.FileName);
        _synopsis = _parser.GetAsciiSynopsis (applicationName, _bufferWidth);
      }
      catch (Exception e)
      {
        _synopsis = "(An error occured while retrieving the application usage synopsis: " + e.Message + ")";  
      }
    }
  }
}
