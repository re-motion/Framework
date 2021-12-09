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
using System.Linq;
using Remotion.ExtensibleEnums;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.Validation.Reflection
{
  /// <summary>
  /// Validates that all applied attribute types of a property are supported.
  /// </summary>
  public class MappingAttributesAreSupportedForPropertyTypeValidationRule : IPropertyDefinitionValidationRule
  {
    private sealed class AttributeConstraint
    {
      private readonly Type[] _propertyTypes;
      private readonly string _message;

      public AttributeConstraint (string message, params Type[] propertyTypes)
      {
        ArgumentUtility.CheckNotNullOrEmpty("message", message);
        ArgumentUtility.CheckNotNullOrEmptyOrItemsNull("propertyTypes", propertyTypes);

        _propertyTypes = propertyTypes;
        _message = message;
      }

      public Type[] PropertyTypes
      {
        get { return _propertyTypes; }
      }

      public string Message
      {
        get { return _message; }
      }
    }

    private Dictionary<Type, AttributeConstraint>? _attributeConstraints;

    private Dictionary<Type, AttributeConstraint> AttributeConstraints
    {
      get
      {
        if (_attributeConstraints == null)
        {
          _attributeConstraints = new Dictionary<Type, AttributeConstraint>();
          AddAttributeConstraints(_attributeConstraints);
        }
        return _attributeConstraints;
      }
    }

    public MappingAttributesAreSupportedForPropertyTypeValidationRule ()
    {
    }

    public IEnumerable<MappingValidationResult> Validate (TypeDefinition typeDefinition)
    {
      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);

      return from PropertyDefinition propertyDefinition in typeDefinition.MyPropertyDefinitions
             select Validate(propertyDefinition.PropertyInfo);
    }

    //  //TODO 3467:
    //  // StringPropertyAttribute
    //  public class StringxxxValidationRule : xxxValidationRule<StringPropertyAttribute>
    //  {
    //    override string GetMEssage()
    //  }

    //  protected MappingValidationResultValidate (PropertyInfo pi)
    //{ 
    //    var attr = AttributeUtility.GetCustomAttributes<TAttribute> (pi, true))
    //    if (attr != null && !IsPropertyTypeSupported (pi))
    //    {
    //        return new MappingValidationResult (false, GetMessage (pi));
    //    }
    //    return new MappingValidationResult (true);
    //  }

    private MappingValidationResult Validate (IPropertyInformation propertyInfo)
    {
      ArgumentUtility.CheckNotNull("propertyInfo", propertyInfo);

      foreach (var attribute in propertyInfo.GetCustomAttributes<Attribute>(true))
      {
        var constraint = GetAttributeConstraint(attribute.GetType());
        if (constraint != null && !Array.Exists(constraint.PropertyTypes, t => IsPropertyTypeSupported(propertyInfo, t)))
        {
          return MappingValidationResult.CreateInvalidResultForProperty(propertyInfo, constraint.Message);
        }
      }
      return MappingValidationResult.CreateValidResult();
    }

    private bool IsPropertyTypeSupported (IPropertyInformation propertyInfo, Type type)
    {
      if (type == typeof(ObjectList<>))
        return ReflectionUtility.IsObjectList(propertyInfo.PropertyType);
      if (type == typeof(IObjectList<>))
        return ReflectionUtility.IsIObjectList(propertyInfo.PropertyType);
      return type.IsAssignableFrom(propertyInfo.PropertyType);
    }

    private AttributeConstraint? GetAttributeConstraint (Type attributeType)
    {
      return (AttributeConstraints.Where(c => c.Key.IsAssignableFrom(attributeType)).Select(c => c.Value)).FirstOrDefault();
    }

    private void AddAttributeConstraints (Dictionary<Type, AttributeConstraint> attributeConstraints)
    {
      ArgumentUtility.CheckNotNull("attributeConstraints", attributeConstraints);

      attributeConstraints.Add(typeof(StringPropertyAttribute), CreateAttributeConstraintForPropertyType<StringPropertyAttribute, string>());
      attributeConstraints.Add(typeof(BinaryPropertyAttribute), CreateAttributeConstraintForPropertyType<BinaryPropertyAttribute, byte[]>());
      attributeConstraints.Add(
          typeof(ExtensibleEnumPropertyAttribute), CreateAttributeConstraintForPropertyType<ExtensibleEnumPropertyAttribute, IExtensibleEnum>());
      attributeConstraints.Add(typeof(MandatoryAttribute), CreateAttributeConstraintForRelationProperty<MandatoryAttribute>());
      attributeConstraints.Add(
          typeof(DBBidirectionalRelationAttribute), CreateAttributeConstraintForRelationProperty<DBBidirectionalRelationAttribute>());
    }

    private AttributeConstraint CreateAttributeConstraintForPropertyType<TAttribute, TProperty> ()
        where TAttribute: Attribute
    {
      return new AttributeConstraint(
          string.Format("The '{0}' may be only applied to properties of type '{1}'.", typeof(TAttribute).Name, typeof(TProperty).Name),
          typeof(TProperty));
    }

    private AttributeConstraint CreateAttributeConstraintForRelationProperty<TAttribute> ()
        where TAttribute: Attribute
    {
      return new AttributeConstraint(
          string.Format(
              "The '{0}' may be only applied to properties assignable to types '{1}', '{2}', or '{3}'.",
              typeof(TAttribute).Name,
              typeof(DomainObject).Name,
              typeof(ObjectList<>).Name,
              typeof(IObjectList<>).Name),
          typeof(DomainObject),
          typeof(ObjectList<>),
          typeof(IObjectList<>));
    }
  }
}
