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
using System.Runtime.Serialization;

namespace Remotion.Tools.Console.CommandLine
{

internal abstract class FormatArgument
{
  private const string c_messageByName = "Argument \"{0}\"";
  private const string c_messageByPlaceholder = "Argument \"{0}\"";
  private const string c_messageByNumber = "Argument no. {0}";
  private const string c_messageUnknownArgument = "Unknown Argument";
  private const string c_messageGroupArgument = "Argument group {0}";

  public static string Format (CommandLineArgument argument)
  {
    if (argument is CommandLineGroupArgument)
      return string.Format(c_messageGroupArgument, argument.Placeholder);
    else if (argument.Name != null)
      return string.Format(c_messageByName, argument.Parser!.ArgumentDeclarationPrefix + argument.Name);
    else if (argument.Placeholder != null)
      return string.Format(c_messageByPlaceholder, argument.Placeholder);
    else if (argument.Parser != null)
      return string.Format(c_messageByNumber, argument.Position + 1);
    else
      return c_messageUnknownArgument;
  }
}

/// <summary>
/// Base class for all exceptions that indicate errors in the command line.
/// </summary>
/// <remarks>
/// Throw <see cref="CommandLineArgumentApplicationException"/> to indicate application-defined command line
/// errors (e.g., use of arguments that exclude each other).
/// </remarks>
[Serializable]
public abstract class CommandLineArgumentException: Exception
{
  protected CommandLineArgumentException (string message)
    : base(message)
  {
  }

  protected CommandLineArgumentException (string message, Exception innerException)
    : base(message, innerException)
  {
  }

#if NET8_0_OR_GREATER
  [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
  protected CommandLineArgumentException (SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }
}

/// <summary>
/// This exception is thrown if the value of a parameter cannot be interpreted.
/// </summary>
[Serializable]
public class InvalidCommandLineArgumentValueException: CommandLineArgumentException
{
  private const string c_message = "Invalid argument value";

  public InvalidCommandLineArgumentValueException (CommandLineArgument argument)
    : this(argument, c_message)
  {
  }

  public InvalidCommandLineArgumentValueException (CommandLineArgument argument, string message)
    : this(FormatArgument.Format(argument) + ": " + message)
  {
  }

  public InvalidCommandLineArgumentValueException (string message)
    : base(message)
  {
  }

#if NET8_0_OR_GREATER
  [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
  protected InvalidCommandLineArgumentValueException (SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }
}

/// <summary>
/// This exception is thrown if the command line contains a named argument that is not defined.
/// </summary>
/// <remarks>
/// The exception is thrown either because there is no argument definition with the specified name,
/// or (if <see cref="CommandLineParser.IncrementalNameValidation"/> is <see langword="true" />), because there 
/// is more than one argument that starts with the specified string.
/// </remarks>
[Serializable]
public class InvalidCommandLineArgumentNameException: CommandLineArgumentException
{
  internal const string MessageNotFound = "Argument /{0}: invalid argument name.";
  internal const string MessageAmbiguous = "Argument /{0}: ambiguous argument name.";

  public InvalidCommandLineArgumentNameException (string name, string message)
    : base(string.Format(message, name))
  {
  }

#if NET8_0_OR_GREATER
  [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
  protected InvalidCommandLineArgumentNameException (SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }
}

/// <summary>
/// This exception is thrown if the command line contains too many unnamed arguments.
/// </summary>
[Serializable]
public class InvalidNumberOfCommandLineArgumentsException: CommandLineArgumentException
{
  private const string c_message = "Argument /{0}: unexpected argument. Only {1} unnamed arguments are allowed.";

  public InvalidNumberOfCommandLineArgumentsException (string argument, int number)
    : base(string.Format(c_message, argument, number))
  {
  }

#if NET8_0_OR_GREATER
  [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
  protected InvalidNumberOfCommandLineArgumentsException (SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }
}

/// <summary>
/// This exception is thrown if a non-optional command line argument is not contained in the command line.
/// </summary>
[Serializable]
public class MissingRequiredCommandLineParameterException: CommandLineArgumentException
{
  private const string c_message = ": Required Argument not specified.";

  public MissingRequiredCommandLineParameterException (CommandLineArgument argument)
    : base(FormatArgument.Format(argument) + c_message)
  {
  }

#if NET8_0_OR_GREATER
  [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
  protected MissingRequiredCommandLineParameterException (SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }
}

/// <summary>
/// This exception is thrown if two or more conflictiong arguments are set.
/// </summary>
[Serializable]
public class ConflictCommandLineParameterException: CommandLineArgumentException
{
  private const string c_message = "Conflicting Arguments: {0} and {1} cannot be used together.";

  public ConflictCommandLineParameterException (CommandLineArgument argument1, CommandLineArgument argument2)
    : base(string.Format(c_message, FormatArgument.Format(argument1), FormatArgument.Format(argument2)))
  {
  }

#if NET8_0_OR_GREATER
  [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
  protected ConflictCommandLineParameterException (SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }
}

/// <summary>
/// This exception class indicates application-defined error conditions.
/// </summary>
/// <remarks>
/// Use this class to indicate application-defined command line errors (e.g., 
/// use of arguments that exclude each other).
/// </remarks>
[Serializable]
public class CommandLineArgumentApplicationException: CommandLineArgumentException
{
  public CommandLineArgumentApplicationException (string message)
    : base(message)
  {
  }

  public CommandLineArgumentApplicationException (string message, Exception innerException)
    : base(message, innerException)
  {
  }

#if NET8_0_OR_GREATER
  [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
  protected CommandLineArgumentApplicationException (SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }
}

}
