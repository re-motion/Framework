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
using System.Reflection;
using System.Text;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;

namespace Remotion.Mixins.Validation
{
  public class DelegateValidationRule<TDefinition> : IValidationRule<TDefinition> where TDefinition : IVisitableDefinition
  {
    public struct Args
    {
      private readonly ValidatingVisitor _validator;
      private readonly TDefinition _definition;
      private readonly IValidationLog _log;
      private readonly DelegateValidationRule<TDefinition> _self;

      public Args (ValidatingVisitor validator, TDefinition definition, IValidationLog log, DelegateValidationRule<TDefinition> self)
      {
        ArgumentUtility.CheckNotNull ("validator", validator);
        ArgumentUtility.CheckNotNull ("definition", definition);
        ArgumentUtility.CheckNotNull ("log", log);
        ArgumentUtility.CheckNotNull ("self", self);

        _validator = validator;
        _self = self;
        _log = log;
        _definition = definition;
      }

      public ValidatingVisitor Validator
      {
        get { return _validator; }
      }

      public TDefinition Definition
      {
        get { return _definition; }
      }

      public IValidationLog Log
      {
        get { return _log; }
      }

      public DelegateValidationRule<TDefinition> Self
      {
        get { return _self; }
      }
    }

    private static string GetRuleName (Rule rule)
    {
      MethodInfo method = rule.Method;
      var attribute = AttributeUtility.GetCustomAttribute<DelegateRuleDescriptionAttribute> (method, true);
      if (attribute == null || attribute.RuleName == null)
        return method.DeclaringType.FullName + "." + rule.Method.Name;
      else
        return attribute.RuleName;
    }

    private static string GetMessage (Rule rule)
    {
      MethodInfo method = rule.Method;
      var attribute = AttributeUtility.GetCustomAttribute<DelegateRuleDescriptionAttribute> (method, true);
      if (attribute == null || attribute.Message == null)
        return FormatMessage (rule.Method.Name);
      else
        return attribute.Message;
    }

    private static string FormatMessage (string message)
    {
      var sb = new StringBuilder ();

      for (int i = 0; i < message.Length; ++i)
      {
        if (i > 0 && char.IsUpper (message[i]))
          sb.Append (' ').Append (char.ToLower (message[i]));
        else
          sb.Append (message[i]);
      }
      return sb.ToString ();
    }

    public delegate void Rule (Args args);

    private readonly Rule _rule;
    private readonly string _ruleName;
    private readonly string _message;

    public DelegateValidationRule(Rule rule, string ruleName, string message)
    {
      ArgumentUtility.CheckNotNull ("rule", rule);
      ArgumentUtility.CheckNotNull ("ruleName", ruleName);
      ArgumentUtility.CheckNotNull ("message", message);

      _rule = rule;
      _ruleName = ruleName;
      _message = message;
    }

    public DelegateValidationRule (Rule rule)
        : this (ArgumentUtility.CheckNotNull("rule", rule), GetRuleName(rule), GetMessage(rule))
    {
    }

    public Rule RuleDelegate
    {
      get { return _rule; }
    }

    public string RuleName
    {
      get { return _ruleName; }
    }

    public string Message
    {
      get { return _message; }
    }

    public void Execute (ValidatingVisitor validator, TDefinition definition, IValidationLog log)
    {
      ArgumentUtility.CheckNotNull ("validator", validator);
      ArgumentUtility.CheckNotNull ("definition", definition);
      ArgumentUtility.CheckNotNull ("log", log);
      RuleDelegate (new Args(validator, definition, log, this));
    }
  }
}
