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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  public class PropertyAccessorData
  {
    /// <summary>
    /// Gets the <see cref="PropertyKind"/> for a given property identifier and class definition.
    /// </summary>
    /// <param name="classDefinition">The <see cref="Mapping.ClassDefinition"/> object describing the property's declaring class.</param>
    /// <param name="propertyIdentifier">The property identifier.</param>
    /// <returns>The <see cref="PropertyKind"/> of the property.</returns>
    /// <exception cref="ArgumentNullException">One of the method's arguments is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The domain object does not have a property with the given identifier.</exception>
    public static PropertyKind GetPropertyKind (ClassDefinition classDefinition, string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyIdentifier", propertyIdentifier);

      Tuple<PropertyDefinition, IRelationEndPointDefinition> propertyObjects = GetPropertyDefinitionObjects (classDefinition, propertyIdentifier);
      return GetPropertyKind (propertyObjects.Item2);
    }

    private static PropertyKind GetPropertyKind (IRelationEndPointDefinition relationEndPointDefinition)
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
          Assertion.IsTrue (kind == PropertyKind.RelatedObjectCollection);
          return RelatedObjectCollectionPropertyAccessorStrategy.Instance;
      }
    }

    /// <summary>
    /// Returns the value type of the given property.
    /// </summary>
    /// <param name="classDefinition">The <see cref="Mapping.ClassDefinition"/> object describing the property's declaring class.</param>
    /// <param name="propertyIdentifier">The property identifier.</param>
    /// <returns>The property's value type.</returns>
    /// <remarks>For simple value properties, this returns the simple property type. For related objects, it
    /// returns the related object's type. For related object collections, it returns type <see cref="ObjectList{T}"/>, where "T" is the related
    /// objects' type.</remarks>
    /// <exception cref="ArgumentNullException">One of the method's arguments is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The class definition does not have a property with the given identifier.</exception>
    public static Type GetPropertyType (ClassDefinition classDefinition, string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyIdentifier", propertyIdentifier);

      Tuple<PropertyDefinition, IRelationEndPointDefinition> definitionObjects =
          GetPropertyDefinitionObjects (classDefinition, propertyIdentifier);

      return GetStrategy (GetPropertyKind (definitionObjects.Item2)).GetPropertyType (definitionObjects.Item1, definitionObjects.Item2);
    }

    /// <summary>
    /// Returns mapping objects for the given property.
    /// </summary>
    /// <param name="classDefinition">The <see cref="Mapping.ClassDefinition"/> object describing the property's declaring class.</param>
    /// <param name="propertyIdentifier">The property identifier.</param>
    /// <returns>The property's <see cref="Mapping.PropertyDefinition"/> and <see cref="IRelationEndPointDefinition"/> objects.</returns>
    /// <exception cref="ArgumentNullException">One of the method's arguments is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The class definition does not have a property with the given identifier.</exception>
    public static Tuple<PropertyDefinition, IRelationEndPointDefinition> GetPropertyDefinitionObjects (
        ClassDefinition classDefinition,
        string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyIdentifier", propertyIdentifier);

      PropertyDefinition propertyDefinition = classDefinition.GetPropertyDefinition (propertyIdentifier);
      IRelationEndPointDefinition relationEndPointDefinition = classDefinition.GetRelationEndPointDefinition (propertyIdentifier);

      if (propertyDefinition == null && relationEndPointDefinition == null)
      {
        string message = String.Format (
            "The domain object type {0} does not have a mapping property named '{1}'.",
            classDefinition.ClassType.FullName,
            propertyIdentifier);

        throw new ArgumentException (message, "propertyIdentifier");
      }
      else
        return new Tuple<PropertyDefinition, IRelationEndPointDefinition> (propertyDefinition, relationEndPointDefinition);
    }

    private readonly string _propertyIdentifier;
    private readonly PropertyKind _kind;

    private readonly PropertyDefinition _propertyDefinition;
    private readonly IRelationEndPointDefinition _relationEndPointDefinition;
    private readonly ClassDefinition _classDefinition;
    private readonly Type _propertyType;

    private readonly IPropertyAccessorStrategy _strategy;

    public PropertyAccessorData (ClassDefinition classDefinition, string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyIdentifier", propertyIdentifier);

      _propertyIdentifier = propertyIdentifier;
      _classDefinition = classDefinition;

      Tuple<PropertyDefinition, IRelationEndPointDefinition> propertyObjects = GetPropertyDefinitionObjects (_classDefinition, propertyIdentifier);
      _propertyDefinition = propertyObjects.Item1;
      _relationEndPointDefinition = propertyObjects.Item2;

      _kind = GetPropertyKind (_relationEndPointDefinition);
      _strategy = GetStrategy (_kind);

      _propertyType = _strategy.GetPropertyType (_propertyDefinition, _relationEndPointDefinition);
    }

    /// <summary>
    /// The definition object for the property's declaring class.
    /// </summary>
    public ClassDefinition ClassDefinition
    {
      get { return _classDefinition; }
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
    public PropertyDefinition PropertyDefinition
    {
      get { return _propertyDefinition; }
    }

    /// <summary>
    /// The encapsulated object's relation end point definition object (can be <see langword="null"/>).
    /// </summary>
    public IRelationEndPointDefinition RelationEndPointDefinition
    {
      get { return _relationEndPointDefinition; }
    }

    internal IPropertyAccessorStrategy GetStrategy ()
    {
      return _strategy;
    }

    /// <summary>
    /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="PropertyAccessorData"/> by comparing
    /// <see cref="PropertyIdentifier"/> and <see cref="ClassDefinition"/>.
    /// </summary>
    /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="PropertyAccessorData"/>.</param>
    /// <returns>
    /// true if the specified <see cref="T:System.Object"/> is equivalent to the current <see cref="PropertyAccessorData"/>, ie. it is another
    /// instance of <see cref="PropertyAccessorData"/> with equal <see cref="PropertyIdentifier"/> and <see cref="ClassDefinition"/>; otherwise, false.
    /// </returns>
    public override bool Equals (object obj)
    {
      var other = obj as PropertyAccessorData;
      return other != null
          && Equals (PropertyIdentifier, other.PropertyIdentifier)
          && Equals (ClassDefinition, other.ClassDefinition);
    }

    public override int GetHashCode ()
    {
      return EqualityUtility.GetRotatedHashCode (PropertyIdentifier, ClassDefinition);
    }
  }
}
