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
using System.Reflection;
using System.Text;

namespace Remotion.Tools.Console.CommandLine
{

public abstract class CommandLineGroupArgument: CommandLineArgument
{
  public CommandLineGroupArgument (bool isOptional)
    : base (isOptional)
  {
  }

  public override bool IsPositional
  {
    get { return false; }
  }

  /// <summary> IList&lt;CommandLineArgument&gt;</summary>
  public abstract IList Parts { get; }

  public override string Placeholder
  {
    get
    {
      StringBuilder sb = new StringBuilder();
      foreach (CommandLineArgument part in Parts)
      {
        if (sb.Length == 0)
          sb.Append ('{');
        else
          sb.Append ('|');
        sb.Append (Parser.ArgumentDeclarationPrefix);
        sb.Append (part.Name);
      }
      sb.Append ('}');
      return sb.ToString();
    }
    set
    {
      base.Placeholder = value;
    }
  }

}

/// <summary>
/// Implemented by argument types that are part of a <see cref="CommandLineGroupArgument"/>.
/// </summary>
public interface ICommandLinePartArgument
{
  CommandLineGroupArgument Group { get; }
}

public class CommandLineModeArgument: CommandLineGroupArgument
{
  /// <summary> ArrayList&lt;CommandLineModeFlagArgument&gt;</summary>
  private ArrayList _flags = new ArrayList();
  private CommandLineModeFlagArgument _value = null;
  private Type _enumType = null;

  public CommandLineModeArgument (bool isOptional, Type enumType)
    : base (isOptional)
  {
    _enumType = enumType;
    CreateChildren();
  }

  public void CreateChildren()
  {
    if (_enumType != null)
    {
      _flags.Clear();
      foreach (FieldInfo field in _enumType.GetFields (BindingFlags.Public | BindingFlags.Static))
      {
        CommandLineModeAttribute attribute = CommandLineModeAttribute.GetAttribute (field);

        string name = field.Name;
        if (attribute != null && attribute.Name != null)
          name = attribute.Name;

        CommandLineFlagArgument argument = new CommandLineModeFlagArgument (
            this, name, (Enum) field.GetValue (null));

        if (attribute != null)
          argument.Description = attribute.Description;
      }      
    }
  }

  public void Add (CommandLineModeFlagArgument flag)
  {
    _flags.Add (flag);
  }

  public override IList Parts
  {
    get { return ArrayList.ReadOnly (_flags); }
  }

  public override object ValueObject
  {
    get 
    { 
      if (_value != null)
        return _value.EnumValue;
      else
        return null;
    }
  }

  internal void SetValue (CommandLineModeFlagArgument value)
  {
    if (_value != null)
      throw new ConflictCommandLineParameterException (_value, value); 
    _value = value;
    SetStringValue (value.Name);
  }

  public override void AppendSynopsis (StringBuilder sb)
  {    
    sb.Append (Placeholder);
  }

  public Type EnumType
  {
    get { return _enumType; }
    set { _enumType = value; }
  }
}

/// <summary>
/// Several flag arguments are used for one mode argument group. E.g. /mode1 or /mode2 are two flags that specify either of two modes.
/// </summary>
public class CommandLineModeFlagArgument: CommandLineFlagArgument, ICommandLinePartArgument
{
  private Enum _enumValue;

  private CommandLineModeArgument _modeFlagGroup;

  public CommandLineModeFlagArgument (CommandLineModeArgument modeFlagGroup, string name, Enum enumValue)
    : base (name, false)
  {
    _enumValue = enumValue;
    _modeFlagGroup = modeFlagGroup;
    _modeFlagGroup.Add (this);
  }

  protected internal override void SetStringValue (string value)
  {
    if (value != string.Empty)
      throw new InvalidCommandLineArgumentValueException (this, "");

    base.SetStringValue (value);
    _modeFlagGroup.SetValue (this);
  }

  public Enum EnumValue
  {
    get { return _enumValue; }
  }

  public CommandLineGroupArgument Group 
  { 
    get { return _modeFlagGroup; }
  }
}

}
