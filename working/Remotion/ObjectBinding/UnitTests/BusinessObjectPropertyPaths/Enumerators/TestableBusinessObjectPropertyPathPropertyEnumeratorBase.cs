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
using Remotion.ObjectBinding.BusinessObjectPropertyPaths.Enumerators;

namespace Remotion.ObjectBinding.UnitTests.BusinessObjectPropertyPaths.Enumerators
{
  public class TestableBusinessObjectPropertyPathPropertyEnumeratorBase : BusinessObjectPropertyPathPropertyEnumeratorBase
  {
    public TestableBusinessObjectPropertyPathPropertyEnumeratorBase (string propertyPathIdentifier)
        : base (propertyPathIdentifier)
    {
    }

    protected override void HandlePropertyNotFound (IBusinessObjectClass businessObjectClass, string propertyIdentifier)
    {
      throw new Exception (
          string.Format (
              "HandlePropertyNotFound, class: {0}, property: {1}",
              businessObjectClass.Identifier,
              propertyIdentifier));
    }

    protected override void HandlePropertyNotLastPropertyAndNotReferenceProperty (
        IBusinessObjectClass businessObjectClass, IBusinessObjectProperty property)
    {
      throw new Exception (
          string.Format (
              "HandlePropertyNotLastPropertyAndNotReferenceProperty, class: {0}, property: {1}",
              businessObjectClass.Identifier,
              property.Identifier));
    }
  }
}