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
using Remotion.Utilities;

namespace Remotion.Tools.Console.CommandLine
{
  /// <summary>
  /// Use descendants of <see cref="CommandLineArgumentAttribute"/> (<see cref="CommandLineStringArgument"/>, <see cref="CommandLineFlagArgument"/>,...)
  /// to qualify class properties as command line arguments, to be able to be
  /// filled from a <c>string[] args</c> by a <see cref="CommandLineClassParser"/>.
  /// </summary>
  /// <example>
  /// <code>
  /// <![CDATA[
  /// // [CommandLineStringArgument (argumentName, isOptional, Placeholder = argumentExample, Description = argumentHelpDescription)]
  /// [CommandLineStringArgument ("user", true, Placeholder = "accountants/john.doe", Description = "Fully qualified name of user(s) to query access types for.")]
  /// public string UserName;
  /// ]]>
  /// </code>
  /// </example>
  /// <example>
  /// <code>
  /// <![CDATA[
  /// [CommandLineFlagArgument ("keepTypeNames", false,
  /// Description = "Specifies that the mixer should not use GUIDs to name the generated types.")]
  /// public bool KeepTypeNames;
  /// ]]>
  /// </code>
  /// </example>
  [AttributeUsage (AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
  public abstract class CommandLineArgumentAttribute : Attribute
  {
    private readonly CommandLineArgument _argument;

    #region dummy constructor

    /// <summary> do not use this constructor </summary>
    /// <remarks> 
    ///   This constructor is necessary because, even in <see langword="abstract"/> attribute classes, one constructor 
    ///   must have arguments that meet the constraints of attribute declarations. 
    /// </remarks>
    [Obsolete ("Do not use this constructor.", true)]
    protected CommandLineArgumentAttribute (int doNotUseThisConstructor)
    {
      throw new NotSupportedException();
    }

    #endregion

    protected CommandLineArgumentAttribute (CommandLineArgument argument)
    {
      _argument = argument;
    }

    public string Name
    {
      get { return _argument.Name; }
      set { _argument.Name = value; }
    }

    public bool IsOptional
    {
      get { return _argument.IsOptional; }
      set { _argument.IsOptional = value; }
    }

    public string Placeholder
    {
      get { return _argument.Placeholder; }
      set { _argument.Placeholder = value; }
    }

    public string Description
    {
      get { return _argument.Description; }
      set { _argument.Description = value; }
    }

    public CommandLineArgument Argument
    {
      get { return _argument; }
    }

    public virtual void SetMember (MemberInfo fieldOrProperty)
    {
    }

    public virtual void AddArgument (CommandLineArgumentCollection argumentCollection, IDictionary dictionary, MemberInfo member)
    {
      argumentCollection.Add (this.Argument);
      dictionary.Add (this.Argument, member);
    }
  }

  /// <summary>
  /// Use <see cref="CommandLineStringArgumentAttribute"/> to qualify positional or named string command line arguments.
  /// </summary>
  /// <remarks>
  /// <para>Pass named arguments using e.g. <c>app.exe /user:john.doe</c>.</para>
  /// <para>Pass positional arguments using e.g. <c>app.exe input.csv output.xml</c>.</para>
  /// <para>
  /// <example>
  /// Named argument example:
  /// <code>
  /// <![CDATA[
  /// // [CommandLineStringArgument (argumentName, isOptional, Placeholder = argumentExample, Description = argumentHelpDescription)]
  /// [CommandLineStringArgument ("user", true, Placeholder = "accountants/john.doe", Description = "Fully qualified name of user(s) to query access types for.")]
  /// public string UserName;
  /// ]]>
  /// </code>
  /// </example>
  /// </para>
  /// <para>
  /// <example>
  /// Positional argument example.
  /// Argument order corresponds to the order of 
  /// <see cref="CommandLineArgument"/> qualified properties in the C# sourcefile:
  /// <code>
  /// <![CDATA[
  /// // [CommandLineStringArgument (isOptional, Placeholder = argumentExample, Description = argumentHelpDescription)]
  /// [CommandLineStringArgument (false, Placeholder = "input.csv", Description = "The input CSV file to be transformed to XML.")]
  /// public string InputFileName;
  /// [CommandLineStringArgument (false, Placeholder = "output.xml", Description = "The resulting XML file.")]
  /// public string OutputFileName;
  /// ]]>
  /// </code>
  /// </example>
  /// </para>
  /// </remarks>
  [AttributeUsage (AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
  public class CommandLineStringArgumentAttribute : CommandLineArgumentAttribute
  {
    public CommandLineStringArgumentAttribute (bool isOptional)
        : base (new CommandLineStringArgument (isOptional))
    {
    }

    public CommandLineStringArgumentAttribute (string name, bool isOptional)
        : base (new CommandLineStringArgument (name, isOptional))
    {
    }
  }

  [AttributeUsage (AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
  public class CommandLineFlagArgumentAttribute : CommandLineArgumentAttribute
  {
    public CommandLineFlagArgumentAttribute (string name)
        : base (new CommandLineFlagArgument (name))
    {
    }

    public CommandLineFlagArgumentAttribute (string name, bool defaultValue)
        : base (new CommandLineFlagArgument (name, defaultValue))
    {
    }
  }


  [AttributeUsage (AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
  public class CommandLineInt32ArgumentAttribute : CommandLineArgumentAttribute
  {
    public CommandLineInt32ArgumentAttribute (string name, bool isOptional)
        : base (new CommandLineInt32Argument (name, isOptional))
    {
    }

    public CommandLineInt32ArgumentAttribute (bool isOptional)
        : base (new CommandLineInt32Argument (isOptional))
    {
    }
  }

  [AttributeUsage (AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
  public class CommandLineEnumArgumentAttribute : CommandLineArgumentAttribute
  {
    public CommandLineEnumArgumentAttribute (bool isOptional)
        : base (new CommandLineEnumArgument (isOptional, null))
    {
    }

    public CommandLineEnumArgumentAttribute (string name, bool isOptional)
        : base (new CommandLineEnumArgument (name, isOptional, null))
    {
    }

    public override void SetMember (MemberInfo fieldOrProperty)
    {
      Type enumType = CommandLineReflectionUtility.GetFieldOrPropertyType (fieldOrProperty);
      if (! enumType.IsEnum)
      {
        throw new ApplicationException (
            string.Format (
                "Attribute {0} can only be applied to enumeration fields or properties.", typeof (CommandLineEnumArgumentAttribute).FullName));
      }
      ((CommandLineEnumArgument) Argument).EnumType = enumType;
    }
  }

  [AttributeUsage (AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
  public class CommandLineModeArgumentAttribute : CommandLineArgumentAttribute
  {
    private Type _enumType;

    public CommandLineModeArgumentAttribute (bool isOptional)
        : base (new CommandLineModeArgument (isOptional, null))
    {
    }

    public new CommandLineModeArgument Argument
    {
      get { return (CommandLineModeArgument) base.Argument; }
    }

    public override void SetMember (MemberInfo fieldOrProperty)
    {
      _enumType = CommandLineReflectionUtility.GetFieldOrPropertyType (fieldOrProperty);
      if (! _enumType.IsEnum)
      {
        throw new ApplicationException (
            string.Format (
                "Attribute {0} can only be applied to enumeration fields or properties.", typeof (CommandLineEnumArgumentAttribute).FullName));
      }

      Argument.EnumType = _enumType;
      Argument.CreateChildren();
    }

    public override void AddArgument (CommandLineArgumentCollection argumentCollection, IDictionary dictionary, MemberInfo member)
    {
      if (_enumType == null)
        throw new InvalidOperationException ("SetMember must be called before AddArgument");

      foreach (CommandLineModeFlagArgument flag in Argument.Parts)
      {
        argumentCollection.Add (flag);
        dictionary.Add (flag, member);
      }
      argumentCollection.Add (Argument);
    }
  }

  [AttributeUsage (AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
  public class CommandLineModeAttribute : Attribute
  {
    public static CommandLineModeAttribute GetAttribute (FieldInfo field)
    {
      return (CommandLineModeAttribute) AttributeUtility.GetCustomAttribute (field, typeof (CommandLineModeAttribute), false);
    }

    private string _name;
    private string _description;

    public CommandLineModeAttribute (string name)
    {
      _name = name;
    }

    public string Name
    {
      get { return _name; }
    }

    public string Description
    {
      get { return _description; }
      set { _description = value; }
    }
  }
}
