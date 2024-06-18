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
using System.Xml.Linq;
using MixinXRef.Reflection;
using MixinXRef.Reflection.Utility;
using MixinXRef.Utility;


namespace MixinXRef.Report
{
  public class MemberOverrideReportGenerator : IReportGenerator
  {
    // IEnumerable<MemberDefinitionBase>
    private readonly ReflectedObject _memberDefinitions;

    public MemberOverrideReportGenerator(ReflectedObject memberDefinitions)
    {
      ArgumentUtility.CheckNotNull ("memberDefinitions", memberDefinitions);

      _memberDefinitions = memberDefinitions;
    }

    public XElement GenerateXml ()
    {
      return new XElement (
          "MemberOverrides",
          from overriddenMember in _memberDefinitions
          select GenerateOverriddenMemberElement (overriddenMember));
    }

    private XElement GenerateOverriddenMemberElement(ReflectedObject overriddenMember)
    {
      return new XElement (
          "OverriddenMember",
          new XAttribute ("type", overriddenMember.GetProperty("MemberType")),
          new XAttribute ("name", overriddenMember.GetProperty("Name"))
          );
    }
  }
}