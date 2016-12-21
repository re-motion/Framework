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
using System.Collections.ObjectModel;
using Remotion.ObjectBinding.BusinessObjectPropertyPaths.Enumerators;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BusinessObjectPropertyPaths
{
  /// <summary>
  /// Implements <see cref="IBusinessObjectPropertyPath"/> for late binding of the property path identifier. 
  /// If the specified property path cannot be parsed in the context of the individual <see cref="IBusinessObject"/>, 
  /// a corresponding null-object is returned by <see cref="BusinessObjectPropertyPathBase.GetResult"/>.
  /// </summary>
  public sealed class DynamicBusinessObjectPropertyPath : BusinessObjectPropertyPathBase
  {
    public static DynamicBusinessObjectPropertyPath Create (string propertyPathIdentifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyPathIdentifier", propertyPathIdentifier);

      return new DynamicBusinessObjectPropertyPath (propertyPathIdentifier);
    }

    private readonly string _propertyPathIdentifier;

    private DynamicBusinessObjectPropertyPath (string propertyPathIdentifier)
    {
      _propertyPathIdentifier = propertyPathIdentifier;
    }

    public override bool IsDynamic
    {
      get { return true; }
    }

    public override string Identifier
    {
      get { return _propertyPathIdentifier; }
    }

    public override ReadOnlyCollection<IBusinessObjectProperty> Properties
    {
      get { throw new NotSupportedException ("Properties collection cannot be retrieved for dynamic property paths."); }
    }

    protected override IBusinessObjectPropertyPathPropertyEnumerator GetResultPropertyEnumerator ()
    {
      return new DynamicBusinessObjectPropertyPathPropertyEnumerator (_propertyPathIdentifier);
    }
  }
}