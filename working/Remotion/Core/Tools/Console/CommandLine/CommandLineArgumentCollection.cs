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

namespace Remotion.Tools.Console.CommandLine
{

/// <summary>
/// A collection of <see cref="CommandLineArgument"/> objects.
/// </summary>
public class CommandLineArgumentCollection: CollectionBase
{
  private const string c_msgInvalidArgumentType = "Argument must be of type CommandLineArgument.";
  
  private CommandLineParser _parser = null;

  public CommandLineArgumentCollection (CommandLineParser parser)
  {
    _parser = parser;
  }

  public CommandLineArgumentCollection ()
  {
  }

  public CommandLineArgument this[int index]
  {
    get { return (CommandLineArgument) List[index]; }
    set { List[index] = value; }
  }

  public int Add (CommandLineArgument value)
  {
    return List.Add (value);
  }

  public int IndexOf (CommandLineArgument value)  
  {
    return List.IndexOf (value);
  }

  public void Insert (int index, CommandLineArgument value)  
  {
    List.Insert (index, value);
  }

  public void Remove (CommandLineArgument value)  
  {
    List.Remove (value);
  }

  public bool Contains (CommandLineArgument value)   
  {
    return List.Contains (value);
  }

  #region event handlers for type-checking
  protected override void OnInsert (int index, object value)   
  {
    if (value == null) throw new ArgumentNullException ("value");
    CommandLineArgument argument = value as CommandLineArgument;
    if (argument == null) throw new ArgumentException (c_msgInvalidArgumentType, "value") ;

    if (_parser != null)
      argument.AttachParser (_parser);
  }

  protected override void OnRemove (int index, object value)
  {
    if (value == null) throw new ArgumentNullException ("value");
    CommandLineArgument argument = value as CommandLineArgument;
    if (argument == null) throw new ArgumentException (c_msgInvalidArgumentType, "value") ;

    if (argument.Parser == _parser)
      argument.AttachParser (null);
  }

  protected override void OnSet (int index, object oldValue, object newValue)   
  {
    if (newValue == null) throw new ArgumentNullException ("value");
    CommandLineArgument newArgument = newValue as CommandLineArgument;
    if (newArgument == null) throw new ArgumentException (c_msgInvalidArgumentType, "newValue") ;
    CommandLineArgument oldArgument = (CommandLineArgument) oldValue;

    if (oldArgument.Parser == _parser)
      oldArgument.AttachParser (null);
    if (_parser != null)
      newArgument.AttachParser (_parser);
  }

  protected override void OnValidate (object value)   
  {
    if (! (value is CommandLineArgument))
      throw new ArgumentException (c_msgInvalidArgumentType, "value") ;
  }
  #endregion

}

}
