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
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.TypePipe.MutableReflection.Implementation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.TypePipe
{
  internal class InterceptedPropertyCollector
  {
    public static bool IsAutomaticPropertyAccessor (MethodInfo? accessorMethod)
    {
      return accessorMethod != null && (accessorMethod.IsAbstract || accessorMethod.IsDefined(typeof(CompilerGeneratedAttribute), false));
    }

    public static bool IsOverridable (MethodInfo? method)
    {
      return method != null && method.IsVirtual && !method.IsFinal;
    }

    private const BindingFlags _declaredInfrastructureBindingFlags =
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    private readonly Type _baseType;
    private readonly HashSet<Tuple<PropertyInfo, string>> _properties = new HashSet<Tuple<PropertyInfo, string>>();
    private readonly HashSet<MethodInfo> _validatedMethods = new HashSet<MethodInfo>();
    private readonly ClassDefinition _classDefinition;
    private readonly ITypeConversionProvider _typeConversionProvider;

    public InterceptedPropertyCollector (ClassDefinition classDefinition, ITypeConversionProvider typeConversionProvider)
    {
      ArgumentUtility.CheckNotNull("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull("typeConversionProvider", typeConversionProvider);

      _classDefinition = classDefinition;
      _typeConversionProvider = typeConversionProvider;
      _baseType = classDefinition.Type;

      if (_classDefinition.IsAbstract)
      {
        throw new NonInterceptableTypeException(
            string.Format("Cannot instantiate type {0} as it is abstract and not instantiable.", _baseType.GetFullNameSafe()),
            _baseType);
      }

      AnalyzeAndValidateBaseType();
    }

    public HashSet<Tuple<PropertyInfo, string>> GetProperties ()
    {
      return _properties;
    }

    private void AnalyzeAndValidateBaseType ()
    {
      ValidateBaseType();

      foreach (var propertyDefinition in _classDefinition.GetPropertyDefinitions())
      {
        var property = propertyDefinition.PropertyInfo;
        string propertyIdentifier = propertyDefinition.PropertyName;
        AnalyzeAndValidateProperty(property, propertyIdentifier);
      }

      foreach (var endPointDefinition in _classDefinition.GetRelationEndPointDefinitions())
      {
        if (endPointDefinition.IsVirtual)
        {
          Assertion.IsNotNull(endPointDefinition.PropertyInfo);

          string propertyIdentifier = endPointDefinition.PropertyName!;
          var property = endPointDefinition.PropertyInfo;

          AnalyzeAndValidateProperty(property, propertyIdentifier);
        }
      }

      ValidateRemainingMethods(_baseType);
    }

    private void AnalyzeAndValidateProperty (IPropertyInformation propertyInformation, string propertyIdentifier)
    {
      if (!_typeConversionProvider.CanConvert(propertyInformation.GetType(), typeof(PropertyInfo)))
        return;

      var property = (PropertyInfo?)_typeConversionProvider.Convert(propertyInformation.GetType(), typeof(PropertyInfo), propertyInformation);
      Assertion.DebugIsNotNull(property, "property != null when propertyInformation can be converted to PropertyInfo");
      var isMixinProperty = !property.DeclaringType!.IsAssignableFrom(_baseType);

      var getMethod = property.GetGetMethod(true);
      var setMethod = property.GetSetMethod(true);

      if (getMethod != null)
        ValidateAccessor(property, getMethod, isMixinProperty, "get accessor");

      if (setMethod != null)
        ValidateAccessor(property, setMethod, isMixinProperty, "set accessor");

      if (!isMixinProperty)
        _properties.Add(new Tuple<PropertyInfo, string>(property, propertyIdentifier));
    }

    private void ValidateAccessor (PropertyInfo property, MethodInfo accessor, bool isMixinProperty, string accessorDescription)
    {
      if (isMixinProperty)
      {
        ValidateMixinAccessor(property, accessor);
      }
      else
      {
        ValidateNonMixinAccessor(property, accessor, accessorDescription);
        _validatedMethods.Add(MethodBaseDefinitionCache.GetBaseDefinition(accessor));
      }
    }

    private void ValidateMixinAccessor (PropertyInfo property, MethodInfo accessor)
    {
      if (IsAutomaticPropertyAccessor(accessor))
      {
        var message = string.Format(
            "Cannot instantiate type '{0}' because the mixin member '{1}.{2}' is an automatic property. "
            + "Mixins must implement their persistent members by using 'Properties' to get and set property values.",
            _baseType.GetFullNameSafe(),
            property.DeclaringType!.Name,
            property.Name);
        throw new NonInterceptableTypeException(message, _baseType);
      }
    }

    private void ValidateNonMixinAccessor (PropertyInfo property, MethodInfo accessor, string accessorDescription)
    {
      if (IsAutomaticPropertyAccessor(accessor) && !IsOverridable(accessor))
      {
        string message = string.Format("Cannot instantiate type '{0}' as its member '{1}' has a non-virtual {2}.",
            _baseType.GetFullNameSafe(), property.Name, accessorDescription);
        throw new NonInterceptableTypeException(message, _baseType);
      }
    }

    private void ValidateBaseType ()
    {
      if (_baseType.IsSealed)
        throw new NonInterceptableTypeException(string.Format("Cannot instantiate type {0} as it is sealed.", _baseType.GetFullNameSafe()), _baseType);
    }

    private void ValidateRemainingMethods (Type currentType)
    {
      foreach (MethodInfo method in currentType.GetMethods(_declaredInfrastructureBindingFlags))
      {
        if (method.IsAbstract && !_validatedMethods.Contains(MethodBaseDefinitionCache.GetBaseDefinition(method)))
          throw new NonInterceptableTypeException(
              string.Format(
                  "Cannot instantiate type {0} as its member {1} (on type {2}) is abstract (and not an automatic property).",
                  _baseType.GetFullNameSafe(),
                  method.Name,
                  currentType.Name),
              _baseType);

        _validatedMethods.Add(MethodBaseDefinitionCache.GetBaseDefinition(method));
      }

      if (currentType.BaseType != null)
        ValidateRemainingMethods(currentType.BaseType);
    }
  }
}
