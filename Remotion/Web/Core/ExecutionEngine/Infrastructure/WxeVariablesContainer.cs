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
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Remotion.Collections;
using Remotion.FunctionalProgramming;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  /// <summary>
  /// Holds all variables available in a <see cref="WxeFunction"/>.
  /// </summary>
  public class WxeVariablesContainer
  {
    private static readonly ITypeConversionProvider s_typeConversionProvider = SafeServiceLocator.Current.GetInstance<ITypeConversionProvider>();
    private static readonly ConcurrentDictionary<Type, WxeParameterDeclaration[]> s_parameterDeclarations =
        new ConcurrentDictionary<Type, WxeParameterDeclaration[]>();

    public static WxeParameterDeclaration[] GetParameterDeclarations (Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);
      if (!typeof(WxeFunction).IsAssignableFrom(type))
        throw new ArgumentException("Type " + type.GetFullNameSafe() + " is not derived from WxeFunction.", "type");

      return s_parameterDeclarations.GetOrAdd(type, s_getParameterDeclarationsUncheckedFunc);
    }

    private static WxeParameterDeclaration[] GetParameterDeclarationsUnchecked (Type type)
    {
      var result = type
          .CreateSequence(t => t.BaseType)
          .Reverse()
          .SelectMany(
              (t, i) => t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                  .Select(property => new { Property = property, Attribute = WxeParameterAttribute.GetAttribute(property) })
                  .Where(_ => _.Attribute != null)
                  .Select(
                      _ => new
                           {
                               Type = t,
                               TypeIndex = i,
                               ParameterIndex = _.Attribute!.Index,
                               Property = _.Property,
                               ParameterDeclaration =
                                   new WxeParameterDeclaration(_.Property.Name, _.Attribute.Required ?? false, _.Attribute.Direction, _.Property.PropertyType)
                           }))
          .OrderBy(_ => _.TypeIndex)
          .ThenBy(_ => _.ParameterIndex)
          .ToList();

      var duplicateIndices = result.GroupBy(_ => new { _.TypeIndex, _.ParameterIndex }).Where(_ => _.Count() > 1).ToList();
      if (duplicateIndices.Any())
      {
        var firstDuplicateIndex = duplicateIndices.First().ToArray();
        throw new WxeException(
            string.Format(
                "'{0}' declares WxeParameters '{1}' and '{2}' with the same index. The index of a WxeParameter must be unique within a type.",
                firstDuplicateIndex[0].Type,
                firstDuplicateIndex[0].Property.Name,
                firstDuplicateIndex[1].Property.Name));
      }

      return result.Select(_ => _.ParameterDeclaration).ToArray();
    }

    /// <summary>
    ///   Parses a string of comma separated actual parameters.
    /// </summary>
    /// <param name="parameterDeclarations">
    ///  The <see cref="WxeParameterDeclaration"/> list used for parsing the <paramref name="actualParameters"/>.
    ///  Must not be <see langword="null"/> or contain items that are <see langword="null"/>.
    /// </param>
    /// <param name="actualParameters"> 
    ///   The comma separated list of parameters. Must contain an entry for each required parameter.
    ///   Must not be <see langword="null"/>.
    /// </param>
    /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
    /// <returns> An array of parameter values. </returns>
    /// <remarks>
    ///   <list type="table">
    ///     <listheader>
    ///       <term> Type </term>
    ///       <description> Syntax </description>
    ///     </listheader>
    ///     <item>
    ///       <term> <see cref="String"/> </term>
    ///       <description> A quoted string. Escape quotes and line breaks using the backslash character.</description>
    ///     </item>
    ///     <item>
    ///       <term> Any type that has a <see langword="static"/> <b>Parse</b> method. </term>
    ///       <description> A quoted string that can be passed to the type's <b>Parse</b> method. For boolean constants 
    ///         (&quot;true&quot;, &quot;false&quot;) and numeric constants, quotes are optional.  </description>
    ///     </item>
    ///     <item>
    ///       <term> Variable Reference </term>
    ///       <description> An unquoted variable name. </description>
    ///     </item>
    ///   </list>
    /// </remarks>
    /// <example>
    ///   "the first \"string\" argument, containing quotes and a comma", "true", "12/30/04 12:00", variableName
    /// </example>
    public static object[] ParseActualParameters (WxeParameterDeclaration[] parameterDeclarations, string actualParameters, CultureInfo culture)
    {
      ArgumentUtility.CheckNotNullOrItemsNull("parameterDeclarations", parameterDeclarations);
      ArgumentUtility.CheckNotNull("actualParameters", actualParameters);

      StringUtility.ParsedItem[] parsedItems = StringUtility.ParseSeparatedList(actualParameters, ',');

      if (parsedItems.Length > parameterDeclarations.Length)
        throw new ApplicationException("Number of actual parameters exceeds number of declared parameters.");

      ArrayList arguments = new ArrayList();
      for (int i = 0; i < parsedItems.Length; ++i)
      {
        StringUtility.ParsedItem item = parsedItems[i];
        WxeParameterDeclaration paramDecl = parameterDeclarations[i];

        try
        {
          if (item.IsQuoted)
          {
            if (paramDecl.Type == typeof(string)) // string constant
              arguments.Add(item.Value);
            else // parse constant
              arguments.Add(s_typeConversionProvider.Convert(null, culture, typeof(string), paramDecl.Type, item.Value));
          }
          else
          {
            if (string.CompareOrdinal(item.Value, "true") == 0) // true
              arguments.Add(true);
            else if (string.CompareOrdinal(item.Value, "false") == 0) // false
              arguments.Add(false);
            else if (item.Value.Length > 0 && char.IsDigit(item.Value[0])) // starts with digit -> parse constant
              arguments.Add(s_typeConversionProvider.Convert(null, culture, typeof(string), paramDecl.Type, item.Value));
            else // variable name
              arguments.Add(new WxeVariableReference(item.Value));
          }
        }
        catch (ArgumentException e)
        {
          throw new ApplicationException("Parameter " + paramDecl.Name + ": " + e.Message, e);
        }
        catch (ParseException e)
        {
          throw new ApplicationException("Parameter " + paramDecl.Name + ": " + e.Message, e);
        }
      }

      return arguments.ToArray()!; // TODO RM-8118: Use generic collection
    }

    /// <summary>
    ///   Converts a list of parameter values into a <see cref="NameValueCollection"/>.
    /// </summary>
    /// <param name="parameterDeclarations">
    ///  The <see cref="WxeParameterDeclaration"/> list used for serializing the <paramref name="parameterValues"/>.
    ///  Must not be <see langword="null"/> or contain items that are <see langword="null"/>.
    /// </param>
    /// <param name="parameterValues"> 
    ///   The list parameter values. Must not be <see langword="null"/>.
    /// </param>
    /// <returns> 
    ///   A <see cref="NameValueCollection"/> containing the serialized <paramref name="parameterValues"/>.
    ///   The names of the parameters are used as keys.
    /// </returns>
    public static NameValueCollection SerializeParametersForQueryString (WxeParameterDeclaration[] parameterDeclarations, object[] parameterValues)
    {
      ArgumentUtility.CheckNotNullOrItemsNull("parameterDeclarations", parameterDeclarations);
      ArgumentUtility.CheckNotNull("parameterValues", parameterValues);

      NameValueCollection serializedParameters = new NameValueCollection();

      for (int i = 0; i < parameterDeclarations.Length; i++)
      {
        WxeParameterDeclaration parameterDeclaration = parameterDeclarations[i];
        object? parameterValue = null;
        if (i < parameterValues.Length)
          parameterValue = parameterValues[i];
        string? serializedValue = parameterDeclaration.Converter.ConvertToString(parameterValue, null);
        if (serializedValue != null)
          serializedParameters.Add(parameterDeclaration.Name, serializedValue);
      }
      return serializedParameters;
    }

    private readonly WxeFunction _function;
    private readonly WxeParameterDeclaration[] _parameterDeclarations;
    private readonly NameObjectCollection _variables;
    private object?[] _actualParameters;
    private bool _parametersInitialized;
    private static readonly Func<Type, WxeParameterDeclaration[]> s_getParameterDeclarationsUncheckedFunc = GetParameterDeclarationsUnchecked;

    public WxeVariablesContainer (WxeFunction function, object?[] actualParameters)
        : this(ArgumentUtility.CheckNotNull("function", function), actualParameters, GetParameterDeclarations(function.GetType()))
    {
    }

    public WxeVariablesContainer (WxeFunction function, object?[] actualParameters, WxeParameterDeclaration[] parameterDeclarations)
    {
      ArgumentUtility.CheckNotNull("function", function);
      ArgumentUtility.CheckNotNull("actualParameters", actualParameters);
      ArgumentUtility.CheckNotNullOrItemsNull("parameterDeclarations", parameterDeclarations);

      _function = function;
      _variables = new NameObjectCollection();
      _parameterDeclarations = parameterDeclarations;
      _actualParameters = actualParameters;
    }

    public NameObjectCollection Variables
    {
      get { return _variables; }
    }

    public object?[] ActualParameters
    {
      get { return _actualParameters; }
    }

    public WxeParameterDeclaration[] ParameterDeclarations
    {
      get { return _parameterDeclarations; }
    }

    /// <summary> Initalizes parameters by name. </summary>
    /// <param name="parameters"> 
    ///   The list of parameter. Must contain an entry for each required parameter. Must not be <see langword="null"/>. 
    /// </param>
    public void InitializeParameters (NameValueCollection parameters)
    {
      ArgumentUtility.CheckNotNull("parameters", parameters);
      CheckParametersNotInitialized();

      for (int i = 0; i < _parameterDeclarations.Length; ++i)
      {
        WxeParameterDeclaration paramDeclaration = _parameterDeclarations[i];
        string? strval = parameters[paramDeclaration.Name];
        if (strval != null)
        {
          try
          {
            _variables[paramDeclaration.Name] =
                s_typeConversionProvider.Convert(null, CultureInfo.InvariantCulture, typeof(string), paramDeclaration.Type, strval);
          }
          catch (Exception e)
          {
            throw new ApplicationException("Parameter " + paramDeclaration.Name + ": " + e.Message, e);
          }
        }
        else if (paramDeclaration.Required)
          throw new ApplicationException("Parameter '" + paramDeclaration.Name + "' is missing.");
      }

      _parametersInitialized = true; // since parameterString may not contain variable references, initialization is done right away
    }

    public void InitializeParameters (string parameterString, bool delayInitialization)
    {
      InitializeParameters(parameterString, null, delayInitialization);
    }

    /// <summary> Initializes the <see cref="WxeFunction"/> with the supplied parameters. </summary>
    /// <param name="parameterString"> 
    ///   The comma separated list of parameters. Must contain an entry for each required parameter.
    ///   Must not be <see langword="null"/>.
    /// </param>
    /// <param name="additionalParameters"> 
    ///   The parameters passed to the <see cref="WxeFunction"/> in addition to the executing function's variables.
    ///   Use <see langword="null"/> or an empty collection if all parameters are supplied by the 
    ///   <see cref="Command.WxeFunctionCommandInfo.Parameters"/> string and the function stack.
    /// </param>
    /// <exception cref="InvalidOperationException"> 
    ///   Thrown if the <see cref="WxeFunction"/>'s parameters have already been initialized, either because 
    ///   execution has started or <b>InitializeParameters</b> has been called before.
    /// </exception>
    public void InitializeParameters (string parameterString, NameObjectCollection? additionalParameters)
    {
      InitializeParameters(parameterString, additionalParameters, false);
    }

    private void InitializeParameters (string parameterString, NameObjectCollection? additionalParameters, bool delayInitialization)
    {
      CheckParametersNotInitialized();

      _actualParameters = ParseActualParameters(ParameterDeclarations, parameterString, CultureInfo.InvariantCulture);

      if (!delayInitialization)
        EnsureParametersInitialized(additionalParameters);
    }

    /// <summary> Pass actualParameters to Variables. </summary>
    public void EnsureParametersInitialized (NameObjectCollection? additionalParameters)
    {
      if (_parametersInitialized)
        return;

      NameObjectCollection? callerVariables = (_function.ParentStep != null) ? _function.ParentStep.Variables : null;
      callerVariables = NameObjectCollection.Merge(callerVariables, additionalParameters);

      if (_actualParameters.Length > _parameterDeclarations.Length)
      {
        throw new ApplicationException(
            string.Format("{0} parameters provided but only {1} were expected.", _actualParameters.Length, _parameterDeclarations.Length));
      }

      for (int i = 0; i < _parameterDeclarations.Length; ++i)
      {
        if (i < _actualParameters.Length && _actualParameters[i] != null)
        {
          WxeVariableReference? varRef = _actualParameters[i] as WxeVariableReference;
          if (callerVariables != null && varRef != null)
            _parameterDeclarations[i].CopyToCallee(varRef.Name, callerVariables, _variables);
          else
            _parameterDeclarations[i].CopyToCallee(_actualParameters[i], _variables);
        }
        else if (_parameterDeclarations[i].Required)
          throw new ApplicationException("Parameter '" + _parameterDeclarations[i].Name + "' is missing.");
      }

      _parametersInitialized = true;
    }

    internal void ReturnParametersToCaller ()
    {
      NameObjectCollection callerVariables = _function.ParentStep!.Variables!; // TODO RM-8118: not null assertion

      for (int i = 0; i < _parameterDeclarations.Length; ++i)
      {
        if (i < _actualParameters.Length)
        {
          WxeVariableReference? varRef = _actualParameters[i] as WxeVariableReference;
          if (varRef != null)
            _parameterDeclarations[i].CopyToCaller(varRef.Name, _variables, callerVariables);
        }
      }
    }

    private void CheckParametersNotInitialized ()
    {
      if (_parametersInitialized)
        throw new InvalidOperationException("Parameters are already initialized.");
    }

    public NameValueCollection SerializeParametersForQueryString ()
    {
      NameValueCollection serializedParameters = new NameValueCollection();
      WxeParameterDeclaration[] parameterDeclarations = ParameterDeclarations;
      NameObjectCollection? callerVariables = null;
      if (_function.ParentStep != null)
        callerVariables = _function.ParentStep.Variables;

      bool hasActualParameters = _actualParameters.Length > 0;
      for (int i = 0; i < parameterDeclarations.Length; i++)
      {
        WxeParameterDeclaration parameterDeclaration = parameterDeclarations[i];
        object? parameterValue = null;
        if (hasActualParameters)
        {
          if (i < _actualParameters.Length)
            parameterValue = _actualParameters[i];
        }
        else
          parameterValue = _variables[parameterDeclaration.Name];
        string? serializedValue = parameterDeclaration.Converter.ConvertToString(parameterValue, callerVariables);
        if (serializedValue != null)
          serializedParameters.Add(parameterDeclaration.Name, serializedValue);
      }
      return serializedParameters;
    }
  }
}
