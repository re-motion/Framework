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

namespace Remotion.Utilities
{
  public struct AttributeWithMetadata
  {
    public static IEnumerable<AttributeWithMetadata> IncludeAll (IEnumerable<AttributeWithMetadata> source, Type attributeType)
    {
      ArgumentUtility.CheckNotNull ("source", source);
      ArgumentUtility.CheckNotNull ("attributeType", attributeType);

      foreach (AttributeWithMetadata attribute in source)
      {
        if (attribute.IsInstanceOfType (attributeType))
          yield return attribute;
      }
    }

    public static IEnumerable<AttributeWithMetadata> ExcludeAll (IEnumerable<AttributeWithMetadata> source, Type attributeType)
    {
      ArgumentUtility.CheckNotNull ("source", source);
      ArgumentUtility.CheckNotNull ("attributeType", attributeType);

      foreach (AttributeWithMetadata attribute in source)
      {
        if (!attribute.IsInstanceOfType (attributeType))
          yield return attribute;
      }
    }

    public static IEnumerable<AttributeWithMetadata> Suppress (IEnumerable<AttributeWithMetadata> source, AttributeWithMetadata[] suppressAttributes)
    {
      ArgumentUtility.CheckNotNull ("source", source);
      ArgumentUtility.CheckNotNull ("suppressAttributes", suppressAttributes);

      foreach (AttributeWithMetadata attribute in source)
      {
        bool suppressed = false;
        foreach (AttributeWithMetadata suppressAttribute in suppressAttributes) // assume that there are only few suppressAttributes, if any
        {
          SuppressAttributesAttribute suppressAttributeInstance = (SuppressAttributesAttribute)suppressAttribute.AttributeInstance;
          if (suppressAttributeInstance.IsSuppressed (attribute.AttributeInstance.GetType(), attribute.DeclaringType, suppressAttribute.DeclaringType))
            suppressed = true;
        }

        if (!suppressed)
          yield return attribute;
      }
    }

    public static IEnumerable<Attribute> ExtractInstances (IEnumerable<AttributeWithMetadata> source)
    {
      foreach (AttributeWithMetadata attribute in source)
        yield return attribute.AttributeInstance;
    }

    private readonly Type _declaringType;
    private readonly Attribute _attribute;

    public AttributeWithMetadata (Type declaringType, Attribute attribute)
    {
      ArgumentUtility.CheckNotNull ("declaringType", declaringType);
      ArgumentUtility.CheckNotNull ("attribute", attribute);

      _declaringType = declaringType;
      _attribute = attribute;
    }

    public Type DeclaringType
    {
      get { return _declaringType; }
    }

    public Attribute AttributeInstance
    {
      get { return _attribute; }
    }

    public bool IsInstanceOfType (Type attributeType)
    {
      ArgumentUtility.CheckNotNull ("attributeType", attributeType);

      return attributeType.IsInstanceOfType (AttributeInstance);
    }

    public override string ToString ()
    {
      return "AttributeMetadata: DeclaringType: " + _declaringType.FullName + "; Attibute: " + _attribute.ToString();
    }
  }
}
