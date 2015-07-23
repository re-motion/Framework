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
using System.Collections.Specialized;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Tools.Console.CommandLine
{
  public class CommandLineClassParser: CommandLineParser
  {
    private readonly Type _argumentClass;

    /// <summary> IDictionary &lt;CommandLineArgument, MemberInfo&gt; </summary>
    private readonly IDictionary _arguments;
    
    public CommandLineClassParser (Type argumentClass)
    {
      _argumentClass = argumentClass;
      _arguments = new ListDictionary();

      foreach (MemberInfo member in argumentClass.GetMembers (BindingFlags.Public | BindingFlags.Instance))
      {
        if (member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property)
        {
          CommandLineArgumentAttribute argumentAttribute = (CommandLineArgumentAttribute) AttributeUtility.GetCustomAttribute (
              member, typeof (CommandLineArgumentAttribute), false);
          if (argumentAttribute != null)
          {
            argumentAttribute.SetMember (member);
            argumentAttribute.AddArgument (Arguments, _arguments, member);
          }
        }
      }
    }

    public new object Parse (string commandLine, bool includeFirstArgument)
    {
      return Parse (SplitCommandLine (commandLine, includeFirstArgument));
    }

    public new object Parse (string[] args)
    {
      base.Parse (args);
      object obj = Activator.CreateInstance (_argumentClass);

      foreach (DictionaryEntry entry in _arguments)
      {
        CommandLineArgument argument = (CommandLineArgument) entry.Key;
        MemberInfo fieldOrProperty = (MemberInfo) entry.Value;
        Type memberType = CommandLineReflectionUtility.GetFieldOrPropertyType (fieldOrProperty);
        object value = argument.ValueObject;
        if (argument is ICommandLinePartArgument)
          value = ((ICommandLinePartArgument)argument).Group.ValueObject;

        if (memberType == typeof (bool))
        {
          if (value == null)
            throw new ApplicationException (string.Format ("{0} {1}: Cannot convert null to System.Boolean. Use Nullable<Boolean> type for optional attributes without default values.", fieldOrProperty.MemberType, fieldOrProperty.Name));
          else if (value is bool?)
            value = ((bool?) value).Value;
        }

        if (value != null)
        {
          try
          {
            CommandLineReflectionUtility.SetFieldOrPropertyValue (obj, fieldOrProperty, value);
          }
          catch (Exception e)
          {
            throw new ApplicationException (string.Format ("Error setting value of {0} {1}: {2}", fieldOrProperty.MemberType, fieldOrProperty.Name, e.Message), e);
          }
        }
      }
      return obj;
    }
  }

  public class CommandLineClassParser<T>: CommandLineClassParser
  {
    public CommandLineClassParser ()
        : base (typeof (T))
    {
    }

    public new T Parse (string commandLine, bool includeFirstArgument)
    {
      return (T) base.Parse (commandLine, includeFirstArgument);
    }

    public new T Parse (string[] args)
    {
      return (T) base.Parse (args);
    }
  }
}
