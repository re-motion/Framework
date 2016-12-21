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
using System.Collections;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins
{
  /// <summary>
  /// When applied to a mixin, specifies a class whose custom attributes should be added to the mixin's target class. This is useful when a mixin
  /// should add certain attributes without itself exposing those attributes.
  /// </summary>
  [AttributeUsage (AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
  public class CopyCustomAttributesAttribute : Attribute
  {
    private readonly Type _attributeSourceType;
    private readonly string _attributeSourceMemberName;
    
    private Type[] _copiedAttributeTypes;

    // For CLS compatibility...
    public CopyCustomAttributesAttribute (Type attributeSourceType)
      : this (attributeSourceType, Type.EmptyTypes)
    {
    }

    // For CLS compatibility...
    public CopyCustomAttributesAttribute (Type attributeSourceType, string attributeSourceMemberName)
      : this (attributeSourceType, attributeSourceMemberName, Type.EmptyTypes)
    {
    }

    public CopyCustomAttributesAttribute (Type attributeSourceType, params Type[] copiedAttributeTypes)
    {
      _attributeSourceType = ArgumentUtility.CheckNotNull ("attributeSourceType", attributeSourceType);
      _attributeSourceMemberName = null;
      _copiedAttributeTypes = ArgumentUtility.CheckNotNull ("copiedAttributeTypes", copiedAttributeTypes);
    }

    public CopyCustomAttributesAttribute (Type attributeSourceType, string attributeSourceMemberName, params Type[] copiedAttributeTypes)
    {
      _attributeSourceType = ArgumentUtility.CheckNotNull ("attributeSourceType", attributeSourceType);
      _attributeSourceMemberName = ArgumentUtility.CheckNotNull ("attributeSourceMemberName", attributeSourceMemberName);
      _copiedAttributeTypes = ArgumentUtility.CheckNotNull ("copiedAttributeTypes", copiedAttributeTypes);
    }

    public Type AttributeSourceType
    {
      get { return _attributeSourceType; }
    }

    public string AttributeSourceMemberName
    {
      get { return _attributeSourceMemberName; }
    }

    public object AttributeSourceName
    {
      get
      {
        return AttributeSourceMemberName != null ? AttributeSourceType.FullName + "." + AttributeSourceMemberName : AttributeSourceType.FullName;
      }
    }

    public Type[] CopiedAttributeTypes
    {
      get { return _copiedAttributeTypes; }
      set { _copiedAttributeTypes = value; }
    }

    public MemberInfo GetAttributeSource (MemberTypes memberType)
    {
      if (AttributeSourceMemberName == null)
        return AttributeSourceType;
      else
      {
        MemberInfo[] members =
            AttributeSourceType.GetMember (AttributeSourceMemberName, memberType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        if (members.Length == 0)
          return null;
        else if (members.Length == 1)
          return members[0];
        else
        {
          throw new AmbiguousMatchException (
              string.Format (
                  "The source member string {0} matches several members on type {1}.",
                  AttributeSourceMemberName,
                  AttributeSourceType.FullName));
        }
      }
    }

    public bool IsCopiedAttributeType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return CopiedAttributeTypes.Length == 0 || ((IList) CopiedAttributeTypes).Contains (type);
    }
  }
}
