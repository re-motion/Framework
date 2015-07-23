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
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BusinessObjectPropertyPaths.Enumerators
{
  /// <summary>
  /// Impelements <see cref="IBusinessObjectPropertyPathPropertyEnumerator"/> 
  /// and throws a <see cref="ParseException"/> when the property path cannot be resolved. 
  /// </summary>
  /// <remarks>Used by <see cref="StaticBusinessObjectPropertyPath"/> for resolving the property path identifier.</remarks>
  public sealed class StaticBusinessObjectPropertyPathPropertyEnumerator : BusinessObjectPropertyPathPropertyEnumeratorBase
  {
    public StaticBusinessObjectPropertyPathPropertyEnumerator (string propertyPathIdentifier)
        : base (propertyPathIdentifier)
    {
    }

    protected override void HandlePropertyNotFound (IBusinessObjectClass businessObjectClass, string propertyIdentifier)
    {
      throw new ParseException (
          string.Format (
              "BusinessObjectClass '{0}' does not contain a property named '{1}'.",
              businessObjectClass.Identifier,
              propertyIdentifier));
    }

    protected override void HandlePropertyNotLastPropertyAndNotReferenceProperty (
        IBusinessObjectClass businessObjectClass, IBusinessObjectProperty property)
    {
      throw new ParseException (
          string.Format (
              "Each property in a property path except the last one must be a reference property. Property '{0}' is of type '{1}'.",
              property.Identifier,
              property.GetType().Name));
    }
  }
}