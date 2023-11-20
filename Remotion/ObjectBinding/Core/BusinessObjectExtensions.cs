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
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.ObjectBinding
{
  /// <summary>
  /// This class contains extension methods for the <see cref="IBusinessObject"/> interface.
  /// </summary>
  public static class BusinessObjectExtensions
  {
    /// <summary>
    ///   Gets the value accessed through the <see cref="IBusinessObjectProperty"/> identified by the passed <paramref name="propertyIdentifier"/>. 
    /// </summary>
    /// <param name="businessObject">The <see cref="IBusinessObject"/> for which the property is declared. Must not be <see langword="null" />.</param>
    /// <param name="propertyIdentifier">
    /// A <see cref="String"/> identifing the <see cref="IBusinessObjectProperty"/> used to access the value. 
    /// Must not be <see langword="null" /> or empty.
    /// </param>
    /// <returns> The property value for the <paramref name="propertyIdentifier"/> parameter. </returns>
    /// <exception cref="BusinessObjectPropertyAccessException">
    ///   Thrown if the property's value could not be read.
    /// </exception>
    /// <exception cref="InvalidOperationException"> 
    ///   The <see cref="IBusinessObjectProperty"/> identified through the <paramref name="propertyIdentifier"/> is not part of this 
    ///   <paramref name="businessObject"/>'s <see cref="IBusinessObject.BusinessObjectClass"/>.
    /// </exception>
    public static object? GetProperty (this IBusinessObject businessObject, string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNull("businessObject", businessObject);
      ArgumentUtility.CheckNotNullOrEmpty("propertyIdentifier", propertyIdentifier);

      var propertyDefinition = GetPropertyDefinition(businessObject, propertyIdentifier);

      return businessObject.GetProperty(propertyDefinition);
    }

    /// <summary>
    ///   Sets the value accessed through the <see cref="IBusinessObjectProperty"/> identified by the passed <paramref name="propertyIdentifier"/>. 
    /// </summary>
    /// <param name="businessObject">The <see cref="IBusinessObject"/> for which the property is declared. Must not be <see langword="null" />.</param>
    /// <param name="propertyIdentifier"> 
    ///   A <see cref="String"/> identifing the <see cref="IBusinessObjectProperty"/> used to access the value. 
    /// </param>
    /// <param name="value"> 
    ///   The new value for the <see cref="IBusinessObjectProperty"/> identified by the 
    ///   <paramref name="propertyIdentifier"/> parameter. 
    /// </param>
    /// <exception cref="BusinessObjectPropertyAccessException">
    ///   Thrown if the property's value could not be read.
    /// </exception>
    /// <exception cref="InvalidOperationException"> 
    ///   The <see cref="IBusinessObjectProperty"/> identified through the <paramref name="propertyIdentifier"/> is not part of this 
    ///   <paramref name="businessObject"/>'s <see cref="IBusinessObject.BusinessObjectClass"/>.
    /// </exception>
    public static void SetProperty (this IBusinessObject businessObject, string propertyIdentifier, object? value)
    {
      ArgumentUtility.CheckNotNull("businessObject", businessObject);
      ArgumentUtility.CheckNotNullOrEmpty("propertyIdentifier", propertyIdentifier);

      var propertyDefinition = GetPropertyDefinition(businessObject, propertyIdentifier);

      businessObject.SetProperty(propertyDefinition, value);
    }

    /// <summary> 
    ///   Gets the string representation of the value accessed through the <see cref="IBusinessObjectProperty"/> 
    ///   identified by the passed <paramref name="propertyIdentifier"/>.
    /// </summary>
    /// <param name="businessObject">The <see cref="IBusinessObject"/> for which the property is declared. Must not be <see langword="null" />.</param>
    /// <param name="propertyIdentifier"> 
    ///   A <see cref="String"/> identifing the <see cref="IBusinessObjectProperty"/> used to access the value. 
    /// </param>
    /// <returns> 
    ///   The string representation of the property value for the <see cref="IBusinessObjectProperty"/> identified by the 
    ///   <paramref name="propertyIdentifier"/> parameter. 
    /// </returns>
    /// <exception cref="BusinessObjectPropertyAccessException">
    ///   Thrown if the property's value could not be read.
    /// </exception>
    /// <exception cref="InvalidOperationException"> 
    ///   The <see cref="IBusinessObjectProperty"/> identified through the <paramref name="propertyIdentifier"/> is not part of this 
    ///   <paramref name="businessObject"/>'s <see cref="IBusinessObject.BusinessObjectClass"/>.
    /// </exception>
    public static string GetPropertyString (this IBusinessObject businessObject, string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNull("businessObject", businessObject);
      ArgumentUtility.CheckNotNullOrEmpty("propertyIdentifier", propertyIdentifier);

      var propertyDefinition = GetPropertyDefinition(businessObject, propertyIdentifier);

      return businessObject.GetPropertyString(propertyDefinition, null);
    }

    /// <summary>
    /// Gets the <see cref="IBusinessObjectWithIdentity.DisplayName"/> property of <paramref name="businessObject"/> 
    /// after checking that the property's value can be read.
    /// </summary>
    /// <remarks>
    /// Getting the <see cref="IBusinessObjectWithIdentity.DisplayName"/> can still fail with an exception if the exception is not part of the 
    /// property access contract, i.e. the exception is not of type <see cref="BusinessObjectPropertyAccessException"/>.
    /// </remarks>
    public static string GetAccessibleDisplayName (this IBusinessObjectWithIdentity businessObject)
    {
      ArgumentUtility.CheckNotNull("businessObject", businessObject);

      var businessObjectClass = businessObject.BusinessObjectClass;
      Assertion.IsNotNull(businessObjectClass, "The business object's BusinessObjectClass-property evaluated and returned null.");

      var displayNameProperty = businessObjectClass.GetPropertyDefinition("DisplayName");
      if (displayNameProperty == null)
      {
        // No property-is-accessible checks can be performed.
        // This code path would only be exercised if the DisplayName property is not included in the bound properties.
        return businessObject.DisplayName;
      }

      if (displayNameProperty.IsAccessible(businessObject))
      {
        try
        {
          return (string?)businessObject.GetProperty(displayNameProperty) ?? string.Empty;
        }
        catch (BusinessObjectPropertyAccessException)
        {
          // Fallback to not-accessible-property behavior
        }
      }

      var businessObjectProvider = displayNameProperty.BusinessObjectProvider;
      Assertion.IsNotNull(businessObjectProvider, "IBusinessObjectProperty.BusinessObjectProvider cannot be null.");

      return businessObjectProvider.GetNotAccessiblePropertyStringPlaceHolder();
    }

    [NotNull]
    private static IBusinessObjectProperty GetPropertyDefinition (IBusinessObject businessObject, string propertyIdentifier)
    {
      var businessObjectClass = businessObject.BusinessObjectClass;
      Assertion.IsNotNull(businessObjectClass, "The business object's BusinessObjectClass-property evaluated and returned null.");

      var propertyDefinition = businessObjectClass.GetPropertyDefinition(propertyIdentifier);
      if (propertyDefinition == null)
      {
        throw new InvalidOperationException(
            string.Format(
                "The business object's class ('{0}') does not contain a property named '{1}'.",
                businessObjectClass.Identifier,
                propertyIdentifier));
      }

      return propertyDefinition;
    }
  }
}
