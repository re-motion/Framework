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
using System.Reflection;
using Remotion.Reflection;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions.Building
{
  public class AttributeDefinitionBuilder
  {
    private readonly IAttributableDefinition _attributableDefinition;

    public AttributeDefinitionBuilder (IAttributableDefinition attributableDefinition)
    {
      ArgumentUtility.CheckNotNull("attributableDefinition", attributableDefinition);
      _attributableDefinition = attributableDefinition;
    }

    public void Apply (MemberInfo attributeSource)
    {
      ArgumentUtility.CheckNotNull("attributeSource", attributeSource);

      var attributes = TypePipeCustomAttributeData.GetCustomAttributes(attributeSource, inherit: true);
      Apply(attributeSource, attributes, isCopyTemplate: false);
    }

    public void Apply (MemberInfo attributeSource, IEnumerable<ICustomAttributeData> attributes, bool isCopyTemplate)
    {
      ArgumentUtility.CheckNotNull("attributeSource", attributeSource);
      ArgumentUtility.CheckNotNull("attributes", attributes);

      foreach (var attributeData in attributes)
      {
        Type? attributeType = attributeData.Constructor.DeclaringType;
        Assertion.IsNotNull(attributeType);
        if (attributeType == typeof(CopyCustomAttributesAttribute))
          ApplyViaCopyAttribute(attributeSource, attributeData);
        else if (attributeType.IsVisible && !IsIgnoredAttributeType(attributeType))
          _attributableDefinition.CustomAttributes.Add(new AttributeDefinition(_attributableDefinition, attributeData, isCopyTemplate));
      }
    }

    private bool IsIgnoredAttributeType (Type type)
    {
      return typeof(ExtendsAttribute).Assembly.Equals(type.Assembly) && type.GetNamespaceChecked().StartsWith("Remotion.Mixins");
    }

    private void ApplyViaCopyAttribute (MemberInfo copyAttributeSource, ICustomAttributeData copyAttributeData)
    {
      Assertion.IsTrue(copyAttributeData.Constructor.DeclaringType == typeof(CopyCustomAttributesAttribute));
      string sourceName = GetFullMemberNameSafe(copyAttributeSource);

      var copyAttribute = (CopyCustomAttributesAttribute)copyAttributeData.CreateInstance();

      MemberInfo? copiedAttributesSource;
      try
      {
        copiedAttributesSource = copyAttribute.GetAttributeSource(UnifyTypeMemberTypes(copyAttributeSource.MemberType));
      }
      catch (AmbiguousMatchException ex)
      {
        string message = string.Format("The CopyCustomAttributes attribute on {0} specifies an ambiguous attribute source: {1}",
            sourceName, ex.Message);
        throw new ConfigurationException(message, ex);
      }

      if (copiedAttributesSource == null)
      {
        string message = string.Format("The CopyCustomAttributes attribute on {0} specifies an unknown attribute source {1}.",
            sourceName, copyAttribute.AttributeSourceName);
        throw new ConfigurationException(message);
      }

      if (!AreCompatibleMemberTypes(copiedAttributesSource.MemberType, copyAttributeSource.MemberType))
      {
        string message = string.Format("The CopyCustomAttributes attribute on {0} specifies an attribute source {1} of a different member kind.",
            sourceName, copyAttribute.AttributeSourceName);
        throw new ConfigurationException(message);
      }

      // A CopyCustomAttribute can specify the same type/member it itself is sitting on; this can be used to get non-inheritable attributes to be 
      // copied to the target. (Normally, only inheritable attributes are copied.)
      // In this case, we must avoid parsing copy attributes again to avoid recursion.
      // In addition, we will add non-inheritable attributes as copy templates. Inheritable attributes are not added as copy templates, since they'll
      // be inherited anyway.
      var isCopyingFromSameMember = copiedAttributesSource.Equals(copyAttributeSource);
      var copiedAttributesData = GetCopiedAttributesData(
          copiedAttributesSource: copiedAttributesSource,
          copyAttribute: copyAttribute,
          includeCopyAttributes: !isCopyingFromSameMember,
          includeInheritableAttributes: !isCopyingFromSameMember);
      Apply(copiedAttributesSource, copiedAttributesData, true);
    }

    private IEnumerable<ICustomAttributeData> GetCopiedAttributesData (
        MemberInfo copiedAttributesSource,
        CopyCustomAttributesAttribute copyAttribute,
        bool includeCopyAttributes,
        bool includeInheritableAttributes)
    {
      foreach (var attributeData in TypePipeCustomAttributeData.GetCustomAttributes(copiedAttributesSource, inherit: true))
      {
        Type attributeType = attributeData.Constructor.DeclaringType!;
        if (typeof(CopyCustomAttributesAttribute).IsAssignableFrom(attributeType))
        {
          if (includeCopyAttributes)
            yield return attributeData;
        }
        else if (copyAttribute.IsCopiedAttributeType(attributeType)
                 && (includeInheritableAttributes || !AttributeUtility.IsAttributeInherited(attributeType)))
        {
          yield return attributeData;
        }
      }
    }

    private bool AreCompatibleMemberTypes (MemberTypes one, MemberTypes two)
    {
      return UnifyTypeMemberTypes(one) == UnifyTypeMemberTypes(two);
    }

    private MemberTypes UnifyTypeMemberTypes (MemberTypes memberType)
    {
      if (memberType == MemberTypes.NestedType || memberType == MemberTypes.TypeInfo)
        return MemberTypes.NestedType | MemberTypes.TypeInfo;
      else
        return memberType;
    }

    private string GetFullMemberNameSafe (MemberInfo attributeSource)
    {
      return attributeSource.DeclaringType != null ? attributeSource.DeclaringType.GetFullNameSafe() + "." + attributeSource.Name : attributeSource.Name;
    }
  }
}
