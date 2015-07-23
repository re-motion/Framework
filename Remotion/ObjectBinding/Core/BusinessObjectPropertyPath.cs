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
using Remotion.ObjectBinding.BusinessObjectPropertyPaths;
using Remotion.ObjectBinding.BusinessObjectPropertyPaths.Results;
using Remotion.Utilities;

namespace Remotion.ObjectBinding
{
  /// <summary>
  /// Facade for creating and working with business object property paths.
  /// </summary>
  public static class BusinessObjectPropertyPath
  {
    #region Obsolete

    /// <summary> Parses the string representation of a property path into a list of properties. </summary>
    /// <param name="objectClass"> The <see cref="IBusinessObjectClass"/> containing the first property in the path. Must no be <see langword="null"/>. </param>
    /// <param name="propertyPathIdentifier"> A string with a valid property path syntax. Must no be <see langword="null"/> or empty. </param>
    /// <returns> An object implementing <see cref="IBusinessObjectPropertyPath"/>. </returns>
    [Obsolete ("Use CreateStatic to create a statically parsed property path. (1.13.177.0)")]
    public static IBusinessObjectPropertyPath Parse (IBusinessObjectClass objectClass, string propertyPathIdentifier)
    {
      return CreateStatic (objectClass, propertyPathIdentifier);
    }

    /// <summary> Gets the value of this property path for the specified object. </summary>
    /// <param name="propertyPath">The property path for which to retrieve the value.</param>
    /// <param name="obj"> The object that has the first property in the path. Must not be <see langword="null"/>. </param>
    /// <param name="throwExceptionIfNotReachable"> 
    ///   If <see langword="true"/>, an <see cref="InvalidOperationException"/> is thrown if any but the last property 
    ///   in the path is <see langword="null"/>. If <see langword="false"/>, <see langword="null"/> is returned instead. 
    /// </param>
    /// <param name="getFirstListEntry">
    ///   If <see langword="true"/>, the first value of each list property is processed.
    ///   If <see langword="false"/>, evaluation of list properties causes an <see cref="InvalidOperationException"/>.
    ///   (This does not apply to the last property in the path. If the last property is a list property, the return value is always a list.)
    /// </param>
    /// <exception cref="InvalidOperationException"> 
    ///   Thrown if any but the last property in the path is <see langword="null"/>, or is not a single-value reference property. 
    /// </exception>
    [Obsolete ("Use GetResult(...).GetValue(...) to retrieve the property path's value. (1.13.178.0)")]
    public static object GetValue (
        this IBusinessObjectPropertyPath propertyPath, IBusinessObject obj, bool throwExceptionIfNotReachable, bool getFirstListEntry)
    {
      ArgumentUtility.CheckNotNull ("propertyPath", propertyPath);
      var result = propertyPath.GetResult (
          obj,
          throwExceptionIfNotReachable ? UnreachableValueBehavior.FailForUnreachableValue : UnreachableValueBehavior.ReturnNullForUnreachableValue,
          getFirstListEntry ? ListValueBehavior.GetResultForFirstListEntry : ListValueBehavior.FailForListProperties);
      return result.GetValue();
    }

    /// <summary> Gets the string representation of the value of this property path for the specified object. </summary>
    /// <param name="propertyPath">The property path for which to retrieve the string value.</param>
    /// <param name="obj"> The object that has the first property in the path. Must not be <see langword="null"/>. </param>
    /// <param name="format"> The format string passed to <see cref="IBusinessObject.GetPropertyString">IBusinessObject.GetPropertyString</see>. </param>
    [Obsolete ("Use GetResult(...).GetString(...) to retrieve the property path's string value. (1.13.178.0)")]
    public static string GetString (this IBusinessObjectPropertyPath propertyPath, IBusinessObject obj, string format)
    {
      ArgumentUtility.CheckNotNull ("format", format);
      var result = propertyPath.GetResult (
          obj,
          UnreachableValueBehavior.ReturnNullForUnreachableValue,
          ListValueBehavior.GetResultForFirstListEntry);
      return result.GetString (format);
    }

    /// <summary> Sets the value of this property path for the specified object. </summary>
    /// <param name="propertyPath">The property path for which to set the value.</param>
    /// <param name="obj">
    ///   The object that has the first property in the path. Must not be <see langword="null"/>. 
    /// </param>
    /// <param name="value"> The value to be assiged to the property. </param>
    /// <remarks> <b>SetValue</b> is not implemented in the current version. </remarks>
    /// <exception cref="NotImplementedException"> This method is not implemented. </exception>
    [Obsolete ("SetValue was never supported. (1.13.178.0)", true)]
    public static void SetValue (this IBusinessObjectPropertyPath propertyPath, IBusinessObject obj, object value)
    {
      throw new NotImplementedException();
    }

    /// <summary> Gets the <see cref="IBusinessObject"/> that is used to retrieve the value of the property path. </summary>
    /// <param name="propertyPath">The property path for which to retrieve the business object.</param>
    /// <param name="obj"> The object that has the first property in the path. Must not be <see langword="null"/>. </param>
    /// <param name="throwExceptionIfNotReachable"> 
    ///   If <see langword="true"/>, an <see cref="InvalidOperationException"/> is thrown if the <see cref="IBusinessObject"/> cannot be reached 
    ///   because one of the properties in the path is <see langword="null"/>. If <see langword="false"/>, <see langword="null"/> is returned instead. 
    /// </param>
    /// <param name="getFirstListEntry">
    ///   If <see langword="true"/>, the first value of each list property is processed.
    ///   If <see langword="false"/>, evaluation of list properties causes an <see cref="InvalidOperationException"/>.
    /// </param>
    /// <exception cref="InvalidOperationException"> 
    ///   Thrown if any but the last property in the path is <see langword="null"/>, or is not a single-value reference property. 
    /// </exception>
    [Obsolete ("Use GetResult(...).GetString(...) to retrieve the property path's string value. (1.13.178.0)")]
    public static IBusinessObject GetBusinessObject (
        this IBusinessObjectPropertyPath propertyPath, IBusinessObject obj, bool throwExceptionIfNotReachable, bool getFirstListEntry)
    {
      ArgumentUtility.CheckNotNull ("propertyPath", propertyPath);
      var result = propertyPath.GetResult (
          obj,
          throwExceptionIfNotReachable ? UnreachableValueBehavior.FailForUnreachableValue : UnreachableValueBehavior.ReturnNullForUnreachableValue,
          getFirstListEntry ? ListValueBehavior.GetResultForFirstListEntry : ListValueBehavior.FailForListProperties);
      return result.ResultObject;
    }

    /// <summary>Creates a <see cref="IBusinessObjectPropertyPath"/> from the passed <see cref="IBusinessObjectProperty"/> list.</summary>
    /// <param name="provider">The business object provider.</param>
    /// <param name="properties"> An array of <see cref="IBusinessObjectProperty"/> instances. </param>
    /// <returns> A new instance of the <see cref="IBusinessObjectPropertyPath"/> type. </returns>
    [Obsolete ("Use BusinessObjectPropertyPath.CreateStatic(...) to create a property path from the list of properties. (1.13.178.0)")]
    public static IBusinessObjectPropertyPath CreatePropertyPath (this IBusinessObjectProvider provider, IBusinessObjectProperty[] properties)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("properties", properties);

      return StaticBusinessObjectPropertyPath.Create (properties);
    }

    #endregion

    /// <summary>
    /// Defines the behavior when the property path has to resolve a list-property.
    /// </summary>
    public enum ListValueBehavior
    {
      /// <summary>
      /// Evaluates the property path by traversing the first item of a list-property.
      /// </summary>
      GetResultForFirstListEntry,

      /// <summary>
      /// Throws an exception when reaching a list-property.
      /// </summary>
      FailForListProperties
    }

    /// <summary>
    /// Defines the behavior when the property path cannot be evaluated due to a <see langword="null" /> value.
    /// </summary>
    public enum UnreachableValueBehavior
    {
      /// <summary>
      /// Returns from the evaluation of the property path proving a corresponding null-object for the <see cref="IBusinessObjectPropertyPathResult"/>.
      /// </summary>
      ReturnNullForUnreachableValue,

      /// <summary>
      /// Throws an exception when the object graph is broken.
      /// </summary>
      FailForUnreachableValue
    }

    /// <summary> Property path formatters can be passed to <see cref="string.Format(string,object[])"/> for full <see cref="IFormattable"/> support. </summary>
    public class Formatter : IFormattable
    {
      private readonly IBusinessObject _object;
      private readonly IBusinessObjectPropertyPath _path;

      public Formatter (IBusinessObject obj, IBusinessObjectPropertyPath path)
      {
        ArgumentUtility.CheckNotNull ("obj", obj);
        ArgumentUtility.CheckNotNull ("path", path);

        _object = obj;
        _path = path;
      }

      public string ToString (string format, IFormatProvider formatProvider)
      {
        var result = _path.GetResult (_object, UnreachableValueBehavior.ReturnNullForUnreachableValue, ListValueBehavior.GetResultForFirstListEntry);
        return result.GetString (format);
      }

      public override string ToString ()
      {
        return ToString (null, null);
      }
    }

    /// <summary> Creates a static property path from the <paramref name="objectClass"/> and the <paramref name="propertyPathIdentifier"/>. </summary>
    /// <param name="objectClass"> The <see cref="IBusinessObjectClass"/> containing the first property in the path. Must no be <see langword="null"/>. </param>
    /// <param name="propertyPathIdentifier"> A string with a valid property path syntax. Must no be <see langword="null"/> or empty. </param>
    /// <returns> 
    /// An instance of type <see cref="IBusinessObjectPropertyPath"/> where <see cref="IBusinessObjectPropertyPath.IsDynamic"/> 
    /// evaluates <see langword="false" />.
    /// </returns>
    [NotNull]
    public static IBusinessObjectPropertyPath CreateStatic (IBusinessObjectClass objectClass, string propertyPathIdentifier)
    {
      return StaticBusinessObjectPropertyPath.Parse (propertyPathIdentifier, objectClass);
    }

    /// <summary> Creates a static property path from the list of <paramref name="properties"/>. </summary>
    /// <param name="properties"> An array of <see cref="IBusinessObjectProperty"/> instances. </param>
    /// <returns>
    /// An instance of type <see cref="IBusinessObjectPropertyPath"/> where <see cref="IBusinessObjectPropertyPath.IsDynamic"/>
    /// evaluates <see langword="false" />. 
    /// </returns>
    [NotNull]
    public static IBusinessObjectPropertyPath CreateStatic (IBusinessObjectProperty[] properties)
    {
      return StaticBusinessObjectPropertyPath.Create (properties);
    }

    /// <summary> Creates a dynamic property path based on the <paramref name="propertyPathIdentifier"/>.  </summary>
    /// <param name="propertyPathIdentifier"> A string with a valid property path syntax. Must no be <see langword="null"/> or empty. </param>
    /// <returns>
    /// An instance of type <see cref="IBusinessObjectPropertyPath"/> where <see cref="IBusinessObjectPropertyPath.IsDynamic"/>
    /// evaluates <see langword="true" />.
    /// </returns>
    [NotNull]
    public static IBusinessObjectPropertyPath CreateDynamic (string propertyPathIdentifier)
    {
      return DynamicBusinessObjectPropertyPath.Create (propertyPathIdentifier);
    }
  }
}