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
using System.Text;

namespace Remotion.Tools.Console.CommandLine
{
/// <summary>
/// The base class for command line argument definitions
/// </summary>
public abstract class CommandLineArgument
{
  // fields 

  private string _name = null;
  private string _placeholder = null;
  private string _description = null;
  private bool _isOptional;
  private CommandLineParser _parser = null;

  private string _stringValue = null;

  // construction and disposal

  /// <summary>
  /// Creates a positional command line argument.
  /// </summary>
  protected CommandLineArgument (bool isOptional)
  {
    _isOptional = isOptional;
  }

  /// <summary>
  /// Creates a named command line argument.
  /// </summary>
  protected CommandLineArgument (string name, bool isOptional)
  {
    _name = name;
    _isOptional = isOptional;
  }

  // methods and properties

  public string Name
  {
    get { return _name; }
    set { _name = (value != null && value.Length == 0) ? null : value; }
  }

  public virtual string Placeholder
  {
    get { return _placeholder; }
    set { _placeholder = value; }
  }

  public string Description
  {
    get { return _description; }
    set { _description = value; }
  }

  public virtual bool IsPositional
  {
    get { return _name == null; }
  }

  public bool IsOptional
  {
    get { return _isOptional; }
    set { _isOptional = value; }
  }

  internal protected virtual void SetStringValue (string value)
  {
    if (value == null) throw new ArgumentNullException ("value");

    _stringValue = value;
  }

  public string StringValue
  {
    get { return _stringValue; }
  }

  public bool IsSpecified
  {
    get { return _stringValue != null; }
  }

  public abstract void AppendSynopsis (StringBuilder sb);
  
  public string GetSynopsis ()
  {
    StringBuilder sb = new StringBuilder ();
    AppendSynopsis (sb);
    return sb.ToString();
  }

  public CommandLineParser Parser
  {
    get { return _parser; }
  }

  internal protected virtual void AttachParser (CommandLineParser parser)
  {
    _parser = parser;
  }

  public int Position
  {
    get
    {
      if (_parser == null)
        throw new InvalidOperationException ("Cannot determine Position because CommandLineArgument is not attached to a CommandLineParser");
      return _parser.Arguments.IndexOf (this);
    }
  }

  public abstract object ValueObject { get; }
}

}
