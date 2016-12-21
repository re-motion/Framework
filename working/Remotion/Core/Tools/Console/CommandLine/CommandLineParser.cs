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
using System.Collections;
using System.Globalization;
using System.Text;

namespace Remotion.Tools.Console.CommandLine
{
/// <summary>
/// Provides methods for declaring command line syntax and providing values.
/// </summary>
public class CommandLineParser
{
  // fields

  public readonly CommandLineArgumentCollection Arguments;

  private char _separator = ':';
  private bool _incrementalNameValidation = true;
  private bool _isCaseSensitive = false;
  private string _argumentDeclarationPrefix = "/";

  // construction and disposal

  public CommandLineParser ()
  {
    Arguments = new CommandLineArgumentCollection (this);
  }


  // properties and methods

  public char Separator
  {
    get 
    { 
      return _separator; 
    }
    set 
    { 
      if (char.IsWhiteSpace(value))  throw new ArgumentOutOfRangeException ("value", value, "Whitespace is not supported as separator.");
      _separator = value; 
    }
  }

  public bool IncrementalNameValidation
  {
    get { return _incrementalNameValidation; }
    set { _incrementalNameValidation = value; }
  }

  public bool IsCaseSensitive
  {
    get { return _isCaseSensitive; }
    set { _isCaseSensitive = value; }
  }

  public string ArgumentDeclarationPrefix
  {
    get { return _argumentDeclarationPrefix; }
    set { _argumentDeclarationPrefix = value; }
  }

  /// <summary>
  /// Splits a command line string into an array of command line arguments, separated by spaces.
  /// </summary>
  /// <include file='..\..\doc\include\Console\CommandLine\CommandLineParser.xml' path='CommandLineParser/SplitCommandLine/*' />
  /// <include file='..\..\doc\include\Console\CommandLine\CommandLineParser.xml' path='CommandLineParser/Parameters/param[@name="includeFirstArgument"]' />
  public static string[] SplitCommandLine (string commandLine, bool includeFirstArgument)
  {
    StringBuilder current = new StringBuilder();
    ArrayList argsArray = new ArrayList();
    int len = commandLine.Length;
    int state = 0; // 0 ... between arguments, 1 ... within argument, 2 ... within quotes
    for (int i = 0; i < len; ++i)
    {
      char c = commandLine[i];
      if (state == 0)
      {
        switch (c)
        {
          case '\"':
            state = 2;
            break;
          case ' ':
            break;
          default:
            state = 1;
            current.Append (c);
            break;
        }
      }
      else if (state == 1)
      {
        switch (c)
        {
          case '\"':
            state = 2;
            break;
          case ' ':
            state = 0;
            if (current.Length > 0)
            {
              argsArray.Add (current.ToString());
              current.Length = 0;
            }
            break;
          default:
            current.Append (c);
            break;
        }
      }
      else if (state == 2)
      {
        switch (c)
        {
          case '\"':
            if (((i + 1) < len) && (commandLine[i+1] == '\"'))
            {
              current.Append ('\"');
              ++i;
            }
            else
            {
              state = 1;
            }
            break;

          default:
            current.Append (c);
            break;
        }
      }
    }
    if (current.Length > 0)
      argsArray.Add (current.ToString());

    int copyStart = 0; 
    int copyCount = argsArray.Count;
    if (! includeFirstArgument)
    {
      copyStart = 1;
      -- copyCount;
    }
    string[] args = new string[copyCount];
    argsArray.CopyTo (copyStart, args, 0, copyCount);
    return args;
  }

  /// <summary>
  /// This method reads a command line and initializes the arguments contained in <see cref="Arguments"/>.
  /// </summary>
  /// <include file='..\..\doc\include\Console\CommandLine\CommandLineParser.xml' path='CommandLineParser/Parameters/param[@name="commandLine" or @name="includeFirstArgument"]' />
  /// <exception cref="InvalidCommandLineArgumentValueException">The value of a parameter cannot be interpreted.</exception>
  /// <exception cref="InvalidCommandLineArgumentNameException">The command line contains a named argument that is not defined.</exception>
  /// <exception cref="InvalidNumberOfCommandLineArgumentsException">The command line contains too many unnamed arguments.</exception>
  /// <exception cref="MissingRequiredCommandLineParameterException">A non-optional command line argument is not contained in the command line.</exception>
  public void Parse (string commandLine, bool includeFirstArgument)
  {
    Parse (SplitCommandLine (commandLine, includeFirstArgument));
  }

  /// <summary>
  /// This method reads the strings of a command line (as passed to the C# Main method) and
  /// initializes the arguments contained in <see cref="Arguments"/>.
  /// </summary>
  /// <param name="args">An array of command line arguments. This is typically the <c>args</c> parameter passed to the C# Main method.</param>
  /// <exception cref="InvalidCommandLineArgumentValueException">The value of a parameter cannot be interpreted.</exception>
  /// <exception cref="InvalidCommandLineArgumentNameException">The command line contains a named argument that is not defined.</exception>
  /// <exception cref="InvalidNumberOfCommandLineArgumentsException">The command line contains too many unnamed arguments.</exception>
  /// <exception cref="MissingRequiredCommandLineParameterException">A non-optional command line argument is not contained in the command line.</exception>
  public void Parse (string[] args)
  {
    int nextPositionalArgument = 0;
    for (int i = 0; i < args.Length; ++i)
    {
      string arg = args[i];
      if (arg.StartsWith (_argumentDeclarationPrefix))
      {
        string name = null;
        string value = null;

        arg = arg.Substring (1);
        int pos = arg.IndexOf (_separator);
        if (pos >= 0)
        {
          name = arg.Substring (0, pos);
          value = arg.Substring (pos + 1);
        }
        else 
        {
          pos = arg.IndexOfAny (new char[] { '+', '-' });
          if (pos >= 0)
          {
            name = arg.Substring (0, pos);
            value = arg.Substring (pos);
          }
        }
        if (name == null)
          name = arg;

        CommandLineArgument argument = GetArgument (name);

        argument.SetStringValue ((value != null) ? value : string.Empty);
      }
      else
      {
        CommandLineArgument argument = GetPositionalArgument (nextPositionalArgument);
        if (argument == null)
          throw new InvalidNumberOfCommandLineArgumentsException (arg, nextPositionalArgument);
        ++ nextPositionalArgument;

        argument.SetStringValue (arg);
      }
    }

    foreach (CommandLineArgument argument in this.Arguments)
    {
      if (! argument.IsOptional && argument.StringValue == null)
        throw new MissingRequiredCommandLineParameterException (argument);
    }
  }

  private CommandLineArgument GetPositionalArgument (int position)
  {
    int currentPosition = 0;
    foreach (CommandLineArgument argument in this.Arguments)
    {
      if (argument.IsPositional)
      {
        if (currentPosition == position)
          return argument;
        else
          ++ currentPosition;
      }
    }
    return null;
  }

  private CommandLineArgument GetArgument (string name)
  {
    if (_incrementalNameValidation)
    {
      CommandLineArgument foundArgument = null;
      bool found2ndArgument = false;
      foreach (CommandLineArgument argument in this.Arguments)
      {
        string argumentName = argument.Name;
        if (argumentName == null)
          continue;

        if (string.Compare (argumentName, name, !_isCaseSensitive, CultureInfo.InvariantCulture) == 0)
        {
          return argument;
        }
        else if (argumentName.Length > name.Length
               && string.Compare (argumentName, 0, name, 0, name.Length, !_isCaseSensitive, CultureInfo.InvariantCulture) == 0)
        {
          if (foundArgument != null)
            found2ndArgument = true;
          else
            foundArgument = argument;
        }
      }

      if (foundArgument == null)
        throw new InvalidCommandLineArgumentNameException (name, InvalidCommandLineArgumentNameException.MessageNotFound);
      else if (found2ndArgument)
        throw new InvalidCommandLineArgumentNameException (name, InvalidCommandLineArgumentNameException.MessageAmbiguous);

      return foundArgument;
    }
    else
    {
      foreach (CommandLineArgument argument in this.Arguments)
      {
        if (argument.Name == name)
          return argument;
      }
      throw new InvalidCommandLineArgumentNameException (name, InvalidCommandLineArgumentNameException.MessageNotFound);
    }

  }

  /// <summary>
  /// Returns a string containing the syntax description of the arguments contained in <see cref="Arguments"/>.
  /// </summary>
  /// <param name="commandName">The file name of the program.</param>
  /// <param name="maxWidth">The maximum line length (for screen output, use 79 for 80 character output to avoid blank lines).</param>
  /// <returns>A syntax overview containing a short command line overview and a table of parameters and desciptions.</returns>
  public string GetAsciiSynopsis (string commandName, int maxWidth)
  {
    StringBuilder sb = new StringBuilder (2048); 

    sb.Append (commandName);
    int maxLength = 0;
    int openSquareBrackets = 0;
    for (int i = 0; i < Arguments.Count; ++i)
    {
      CommandLineArgument argument = Arguments[i];
      CommandLineArgument nextArgument = ((i + 1) < Arguments.Count) ? Arguments[i+1] : null;

      if (! (argument is ICommandLinePartArgument))
      {
        // append opening square bracket
        sb.Append (" ");
        if (argument.IsOptional)
        {
          sb.Append ("[");
          ++ openSquareBrackets;
        }

        argument.AppendSynopsis(sb);

        // append closing square brackets after last optional argument
        if (   nextArgument == null
            || ! nextArgument.IsOptional 
            || ! nextArgument.IsPositional)
        {
          for (int k = 0; k < openSquareBrackets; ++k)
            sb.Append ("]");
          openSquareBrackets = 0;
        }
      }

      if (! (argument is CommandLineGroupArgument))
      {
        if (argument.Name != null)
          maxLength = Math.Max (maxLength, argument.Name.Length + 1);
        else if (argument.Placeholder != null)
          maxLength = Math.Max (maxLength, argument.Placeholder.Length);
      }
    }

    // insert word breaks
    string synopsis = sb.ToString();
    sb.Length = 0;
    MonospaceTextFormat.AppendWrappedText (sb, maxWidth, synopsis);

    // create parameter name/description table
    sb.Append ("\n");
    foreach (CommandLineArgument argument in Arguments)
    {
      if (! (argument is CommandLineGroupArgument))
      {
        string name = argument.Placeholder;
        if (argument.Name != null)
          name = _argumentDeclarationPrefix + argument.Name;

        sb.AppendFormat ("\n  {0,-" + maxLength.ToString() + "}  ", name);
        MonospaceTextFormat.AppendIndentedText (sb, maxLength + 4, maxWidth, argument.Description);
      }
    }

    return sb.ToString();
  }

}

}
