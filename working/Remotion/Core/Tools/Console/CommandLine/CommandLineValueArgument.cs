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
using System.Text;

namespace Remotion.Tools.Console.CommandLine
{

public abstract class CommandLineValueArgument: CommandLineArgument
{
  public CommandLineValueArgument (string name, bool isOptional)
    : base (name, isOptional)
  {
  }

  public CommandLineValueArgument (bool isOptional)
    : base (isOptional)
  {
  }

  public override void AppendSynopsis (StringBuilder sb)
  {
    if (! IsPositional)
    {
      sb.Append (Parser.ArgumentDeclarationPrefix);
      sb.Append (Name);
      if (Placeholder != null)
        sb.Append (Parser.Separator);
    }
    sb.Append (Placeholder);
  }
}

public class CommandLineStringArgument: CommandLineValueArgument
{
  public CommandLineStringArgument (string name, bool isOptional)
    : base (name, isOptional)
  {
  }

  public CommandLineStringArgument (bool isOptional)
    : base (isOptional)
  {
  }

  public override object ValueObject
  {
    get { return Value; }
  }
  
  public string Value
  {
    get { return StringValue; }
  }
}

public class CommandLineInt32Argument: CommandLineValueArgument
{
  private int? _value;

  public CommandLineInt32Argument (string name, bool isOptional)
    : base (name, isOptional)
  {
  }

  public CommandLineInt32Argument (bool isOptional)
    : base (isOptional)
  {
  }

  public override object ValueObject
  {
    get { return Value; }
  }

  public int? Value
  {
    get { return _value; }
  }

  protected internal override void SetStringValue (string value)
  {
    if (value == null) throw new ArgumentNullException ("value");
    string strValue = value.Trim();
    if (strValue.Length == 0)
    {
      _value = null;
    }
    else
    {
      double result;
      if (! double.TryParse (value, NumberStyles.Integer, null, out result))
        throw new InvalidCommandLineArgumentValueException (this, "Specify a valid integer number.");
      _value = (int) result;
    }

    base.SetStringValue (value);
  }

}

}
