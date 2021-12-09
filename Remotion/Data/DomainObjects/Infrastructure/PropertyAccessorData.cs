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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  public class PropertyAccessorData
  {
    /// <summary>
    /// Gets the <see cref="PropertyKind"/> for a given property identifier and type definition.
    /// </summary>
    /// <param name="typeDefinition">The <see cref="TypeDefinition"/> object describing the property's declaring type.</param>
    /// <param name="propertyIdentifier">The property identifier.</param>
    /// <returns>The <see cref="PropertyKind"/> of the property.</returns>
    /// <exception cref="ArgumentNullException">One of the method's arguments is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The domain object does not have a property with the given identifier.</exception>
    public static PropertyKind GetPropertyKind (TypeDefinition typeDefinition, string propertyIdentifier)
    {
      // TODO RM-8246: possibly inline this method

      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);
      ArgumentUtility.CheckNotNull("propertyIdentifier", propertyIdentifier);

      Tuple<PropertyDefinition?, IRelationEndPointDefinition?> propertyObjects = GetPropertyDefinitionObjects(typeDefinition, propertyIdentifier);
      return GetPropertyKind(propertyObjects.Item2);
    }

    private static PropertyKind GetPropertyKind (IRelationEndPointDefinition? relationEndPointDefinition)
    {
      if (relationEndPointDefinition == null)
        return PropertyKind.PropertyValue;
      else if (relationEndPointDefinition.Cardinality == CardinalityType.One)
        return PropertyKind.RelatedObject;
      else
        return PropertyKind.RelatedObjectCollection;
    }

    private static IPropertyAccessorStrategy GetStrategy (PropertyKind kind)
    {
      switch (kind)
      {
        case PropertyKind.PropertyValue:
          return ValuePropertyAccessorStrategy.Instance;
        case PropertyKind.RelatedObject:
          return RelatedObjectPropertyAccessorStrategy.Instance;
        default:
          Assertion.IsTrue(kind == PropertyKind.RelatedObjectCollection);
          return RelatedObjectCollectionPropertyAccessorStrategy.Instance;
      }
    }

    /// <summary>
    /// Returns the value type of the given property.
    /// </summary>
    /// <param name="typeDefinition">The <see cref="Mapping.TypeDefinition"/> object describing the property's declaring type.</param>
    /// <param name="propertyIdentifier">The property identifier.</param>
    /// <returns>The property's value type.</returns>
    /// <remarks>For simple value properties, this returns the simple property type. For related objects, it
    /// returns the related object's type. For related object collections, it returns type <see cref="ObjectList{T}"/>, where "T" is the related
    /// objects' type.</remarks>
    /// <exception cref="ArgumentNullException">One of the method's arguments is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The type definition does not have a property with the given identifier.</exception>
    public static Type GetPropertyType (TypeDefinition typeDefinition, string propertyIdentifier)
    {
      // TODO RM-8246: possibly inline this method

      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);
      ArgumentUtility.CheckNotNull("propertyIdentifier", propertyIdentifier);

      // TODO RM-8246: this is actually a discriminating union, solved via nullable values
      Tuple<PropertyDefinition?, IRelationEndPointDefinition?> definitionObjects =
          GetPropertyDefinitionObjects(typeDefinition, propertyIdentifier);

      return GetStrategy(GetPropertyKind(definitionObjects.Item2)).GetPropertyType(definitionObjects.Item1, definitionObjects.Item2);
    }

    /// <summary>
    /// Returns mapping objects for the given property.
    /// </summary>
    /// <param name="typeDefinition">The <see cref="Mapping.TypeDefinition"/> object describing the property's declaring type.</param>
    /// <param name="propertyIdentifier">The property identifier.</param>
    /// <returns>The property's <see cref="Mapping.PropertyDefinition"/> and <see cref="IRelationEndPointDefinition"/> objects.</returns>
    /// <exception cref="ArgumentNullException">One of the method's arguments is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The type definition does not have a property with the given identifier.</exception>
    public static Tuple<PropertyDefinition?, IRelationEndPointDefinition?> GetPropertyDefinitionObjects (
        TypeDefinition typeDefinition,
        string propertyIdentifier)
    {
      // TODO RM-8246: possibly inline this method

      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);
      ArgumentUtility.CheckNotNull("propertyIdentifier", propertyIdentifier);

      PropertyDefinition? propertyDefinition = typeDefinition.GetPropertyDefinition(propertyIdentifier);
      IRelationEndPointDefinition? relationEndPointDefinition = typeDefinition.GetRelationEndPointDefinition(propertyIdentifier);

      if (propertyDefinition == null && relationEndPointDefinition == null)
      {
        string message = String.Format(
            "The domain object type {0} does not have a mapping property named '{1}'.",
            typeDefinition.Type.GetFullNameSafe(),
            propertyIdentifier);

        throw new ArgumentException(message, "propertyIdentifier");
      }
      else
        return new Tuple<PropertyDefinition?, IRelationEndPointDefinition?>(propertyDefinition, relationEndPointDefinition);
    }

    private readonly string _propertyIdentifier;
    private readonly PropertyKind _kind;

    private readonly PropertyDefinition? _propertyDefinition;
    private readonly IRelationEndPointDefinition? _relationEndPointDefinition;
    private readonly TypeDefinition _typeDefinition;
    private readonly Type _propertyType;

    private readonly IPropertyAccessorStrategy _strategy;

    public PropertyAccessorData (TypeDefinition typeDefinition, string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);
      ArgumentUtility.CheckNotNullOrEmpty("propertyIdentifier", propertyIdentifier);

      _propertyIdentifier = propertyIdentifier;
      _typeDefinition = typeDefinition;

      Tuple<PropertyDefinition?, IRelationEndPointDefinition?> propertyObjects = GetPropertyDefinitionObjects(_typeDefinition, propertyIdentifier);
      _propertyDefinition = propertyObjects.Item1;
      _relationEndPointDefinition = propertyObjects.Item2;

      _kind = GetPropertyKind(_relationEndPointDefinition);
      _strategy = GetStrategy(_kind);

      // TODO RM-8246: possibly change the strategy to use classDefinition and propertyIdentifier to get the propertyType in order to avoid the discriminated union.
      _propertyType = _strategy.GetPropertyType(_propertyDefinition, _relationEndPointDefinition);
    }

    /// <summary>
    /// The definition object for the property's declaring type.
    /// </summary>
    public TypeDefinition TypeDefinition
    {
      get { return _typeDefinition; }
    }

    /// <summary>
    /// Indicates which kind of property is encapsulated by this structure.
    /// </summary>
    public PropertyKind Kind
    {
      get { return _kind; }
    }

    /// <summary>
    /// The identifier for the property encapsulated by this structure.
    /// </summary>
    public string PropertyIdentifier
    {
      get { return _propertyIdentifier; }
    }

    /// <summary>
    /// The property value type. For simple value properties, this is the simple property type. For related objects, this
    /// is the related object's type. For related object collections, this is <see cref="ObjectList{T}"/>, where "T" is the
    /// related objects' type.
    /// </summary>
    public Type PropertyType
    {
      get { return _propertyType; }
    }

    /// <summary>
    /// The encapsulated object's property definition object (can be <see langword="null"/>).
    /// </summary>
    public PropertyDefinition? PropertyDefinition
    {
      get { return _propertyDefinition; }
    }

    /// <summary>
    /// The encapsulated object's relation end point definition object (can be <see langword="null"/>).
    /// </summary>
    public IRelationEndPointDefinition? RelationEndPointDefinition
    {
      get { return _relationEndPointDefinition; }
    }

    internal IPropertyAccessorStrategy GetStrategy ()
    {
      return _strategy;
    }

    /// <summary>
    /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="PropertyAccessorData"/> by comparing
    /// <see cref="PropertyIdentifier"/> and <see cref="TypeDefinition"/>.
    /// </summary>
    /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="PropertyAccessorData"/>.</param>
    /// <returns>
    /// true if the specified <see cref="T:System.Object"/> is equivalent to the current <see cref="PropertyAccessorData"/>, ie. it is another
    /// instance of <see cref="PropertyAccessorData"/> with equal <see cref="PropertyIdentifier"/> and <see cref="TypeDefinition"/>; otherwise, false.
    /// </returns>
    public override bool Equals (object? obj)
    {
      var other = obj as PropertyAccessorData;
      return other != null
          && Equals(PropertyIdentifier, other.PropertyIdentifier)
          && Equals(TypeDefinition, other.TypeDefinition);
    }

    public override int GetHashCode ()
    {
      return EqualityUtility.GetRotatedHashCode(PropertyIdentifier, TypeDefinition);
    }
  }
}
