// This file is part of the MixinXRef project
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using MixinXRef.Formatting;
using MixinXRef.Utility;

namespace MixinXRef.Report
{
  public class MemberReportGenerator : IReportGenerator
  {
    private readonly Type _type;
    private readonly InvolvedType _involvedType;
    private readonly IIdentifierGenerator<Type> _involvedTypeIdentifierGenerator;
    private readonly IIdentifierGenerator<MemberInfo> _memberIdentifierGenerator;
    private readonly IOutputFormatter _outputFormatter;
    private readonly MemberModifierUtility _memberModifierUtility = new MemberModifierUtility ();
    private readonly MemberSignatureUtility _memberSignatureUtility;

    public MemberReportGenerator (
      Type type,
      InvolvedType involvedTypeOrNull,
      IIdentifierGenerator<Type> involvedTypeIdentifierGeneratorOrNull,
      IIdentifierGenerator<MemberInfo> memberIdentifierGeneratorOrNull,
      IOutputFormatter outputFormatter)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      // may be null
      // ArgumentUtility.CheckNotNull ("involvedTypeOrNull", involvedTypeOrNull);
      // ArgumentUtility.CheckNotNull ("involvedTypeIdentifierGeneratorOrNull", involvedTypeIdentifierGeneratorOrNull);
      // ArgumentUtility.CheckNotNull ("memberIdentifierGeneratorOrNull", memberIdentifierGeneratorOrNull);
      ArgumentUtility.CheckNotNull ("outputFormatter", outputFormatter);

      _type = type;
      _involvedType = involvedTypeOrNull;
      _involvedTypeIdentifierGenerator = involvedTypeIdentifierGeneratorOrNull;
      _memberIdentifierGenerator = memberIdentifierGeneratorOrNull;
      _outputFormatter = outputFormatter;
      _memberSignatureUtility = new MemberSignatureUtility (outputFormatter);
    }

    public XElement GenerateXml ()
    {
      var type = _involvedType ?? new InvolvedType (_type);
      return new XElement ("Members", type.Members.Select (CreateMemberElement));
    }

    private string GetMemberName(MemberInfo memberInfo)
    {
      // remove interface name if member is explicit interface implementation
      // greater 0 because ".ctor" would be changed to "ctor"
      var lastPoint = memberInfo.Name.LastIndexOf ('.');
      return (lastPoint > 0) ? memberInfo.Name.Substring (lastPoint + 1) : memberInfo.Name;
    }

    private int GetMetadataToken(MemberInfo memberInfo)
    {
      if (memberInfo.MemberType == MemberTypes.Property)
        memberInfo = ((PropertyInfo) memberInfo).GetAccessors(true).First();

      if (memberInfo.MemberType == MemberTypes.Event)
        memberInfo = ((EventInfo) memberInfo).GetAddMethod(true);

      return memberInfo.MetadataToken;
    }

    private XElement CreateMemberElement (InvolvedTypeMember member)
    {
      MemberInfo memberInfo = member.MemberInfo;

      var memberModifier = _memberModifierUtility.GetMemberModifiers (memberInfo);
      if (memberModifier.Contains ("private")) // memberModifier.Contains ("internal")
        return null;

      var memberName = GetMemberName(memberInfo);

      var attributes = new StringBuilder ();

      XElement overridesElement = null;
      XElement overriddenElement = null;
      if (_involvedType != null)
      {
        if (HasOverrideMixinAttribute (memberInfo))
          attributes.Append ("OverrideMixin ");
        if (HasOverrideTargetAttribute (memberInfo))
          attributes.Append ("OverrideTarget ");

        overridesElement = CreateOverridesElement (member);
        overriddenElement = CreateOverriddenElement (member.MemberInfo);
      }

      if (memberInfo.DeclaringType != _type &&
          overridesElement == null && overriddenElement == null)
        return null;

      var element = new XElement("Member", new XAttribute("id", _memberIdentifierGenerator.GetIdentifier(memberInfo)),
                                 new XAttribute("metadataToken", GetMetadataToken(memberInfo)),
                                 new XAttribute("type", memberInfo.MemberType),
                                 new XAttribute("name", memberName),
                                 new XAttribute("is-declared-by-this-class", memberInfo.DeclaringType == _type),
                                 _outputFormatter.CreateModifierMarkup(attributes.ToString(), memberModifier),
                                 _memberSignatureUtility.GetMemberSignature(memberInfo),
                                 member.SubMemberInfos.Select(CreateSubMemberElement),
                                 overridesElement,
                                 overriddenElement);
      return element;
    }

    private XElement CreateSubMemberElement(OverridingMemberInfo subMember)
    {
      MemberInfo memberInfo = subMember;

      var memberModifier = _memberModifierUtility.GetMemberModifiers (subMember);
      if (memberModifier.Contains ("private")) // memberModifier.Contains ("internal")
        return null;

      var memberName = GetMemberName (subMember);

      var attributes = new StringBuilder ();

      var element = new XElement("SubMember", new XAttribute("id", _memberIdentifierGenerator.GetIdentifier(subMember)),
                                 new XAttribute("metadataToken", GetMetadataToken(memberInfo)),
                                 new XAttribute("type", memberInfo.MemberType),
                                 new XAttribute("name", memberName),
                                 _outputFormatter.CreateModifierMarkup(attributes.ToString(), memberModifier),
                                 _memberSignatureUtility.GetMemberSignature(subMember),
                                 CreateOverriddenElement(subMember));
      return element;
    }

    private XElement CreateOverridesElement (InvolvedTypeMember member)
    {
      var overridesElement = new XElement ("Overrides");

      var overridingMixinTypes = member.OverridingMixinTypes;
      var overridingTargetTypes = member.OverridingTargetTypes;

      if (!overridingMixinTypes.Any () && !overridingTargetTypes.Any ())
        return null;

      foreach (var overridingType in overridingMixinTypes)
        overridesElement.Add (CreateInvolvedTypeReferenceElement ("Mixin-Reference", overridingType));

      foreach (var overridingType in overridingTargetTypes)
        overridesElement.Add (CreateInvolvedTypeReferenceElement ("Target-Reference", overridingType));

      return overridesElement;
    }

    private XElement CreateOverriddenElement (OverridingMemberInfo member)
    {
      var overriddenMembersElement = new XElement ("OverriddenMembers");

      if (!member.OverriddenTargetMembers.Any () && !member.OverriddenMixinMembers.Any())
        return null;

      foreach(var overriddenMember in member.OverriddenMixinMembers)
        overriddenMembersElement.Add (CreateMemberReferenceElement ("OverrideMixin", overriddenMember));

      foreach (var overriddenMember in member.OverriddenTargetMembers)
        overriddenMembersElement.Add (CreateMemberReferenceElement ("OverrideTarget", overriddenMember));

      return overriddenMembersElement;
    }

    private XElement CreateInvolvedTypeReferenceElement (string tagName, Type overridingType)
    {
      return new XElement (tagName, new XAttribute ("ref", _involvedTypeIdentifierGenerator.GetIdentifier (overridingType)),
                                    new XAttribute ("instance-name", _outputFormatter.GetShortFormattedTypeName (overridingType)));
    }

    private XElement CreateMemberReferenceElement (string typeName, MemberInfo member)
    {
      return new XElement("Member-Reference",
                          new XAttribute("ref", _memberIdentifierGenerator.GetIdentifier(member)),
                          new XAttribute("type", typeName),
                          new XAttribute("member-name", member.Name),
                          new XAttribute("member-signature", member.ToString()));
    }

    private static bool HasOverrideMixinAttribute (MemberInfo memberInfo)
    {
      return memberInfo.GetCustomAttributes (true).Any (a => a.GetType ().Name == "OverrideMixinAttribute");
    }

    private static bool HasOverrideTargetAttribute (MemberInfo memberInfo)
    {
      return memberInfo.GetCustomAttributes (true).Any (a => a.GetType ().Name == "OverrideTargetAttribute");
    }
  }
}