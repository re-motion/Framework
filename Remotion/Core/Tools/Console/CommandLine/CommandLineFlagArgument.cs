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

public class CommandLineFlagArgument: CommandLineArgument
{
  // fields

  private readonly bool? _defaultValue;
  private bool? _value;

  // construction and disposal

  public CommandLineFlagArgument (string name, bool? defaultValue)
    : base (name, true)
  {
    _defaultValue = defaultValue;
  }

  public CommandLineFlagArgument (string name)
    : base (name, true)
  {
    _defaultValue = null;
  }

  // properties and methods

  public bool? DefaultValue
  {
    get { return _defaultValue; }
  }

  protected internal override void SetStringValue (string value)
  {
    if (value == null) throw new ArgumentNullException ("value");

    switch (value)
    {
      case "": 
        _value = true;
        break;

      case "+":
        _value = true;
        break;
      
      case "-":
        _value = false;
        break;

      default:
        throw new InvalidCommandLineArgumentValueException (this, "Flag parameters support only + and - as arguments.");
    }

    base.SetStringValue (value);
  }


  public override object ValueObject
  {
    get { return Value; }
  }
  
  public bool? Value
  {
    get { return _value ?? _defaultValue; }
  }

  public override void AppendSynopsis (StringBuilder sb)
  {
    if (IsOptional && _defaultValue == false)
    {
      sb.Append (Parser.ArgumentDeclarationPrefix);
      sb.Append (Name);
    }
    else if (IsOptional && _defaultValue == true)
    {
      sb.Append (Parser.ArgumentDeclarationPrefix);
      sb.Append (Name);
      sb.Append ("-");
    }
    else
    {
      sb.Append (Parser.ArgumentDeclarationPrefix);
      sb.Append (Name);
      sb.Append ("+ | /");
      sb.Append (Name);
      sb.Append ("-");
    }
  }
}

}
