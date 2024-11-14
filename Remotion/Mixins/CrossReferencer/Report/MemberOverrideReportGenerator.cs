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
using System.Linq;
using System.Xml.Linq;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;

namespace Remotion.Mixins.CrossReferencer.Report
{
  public class MemberOverrideReportGenerator : IReportGenerator
  {
    // IEnumerable<MemberDefinitionBase>
    private readonly IEnumerable<MemberDefinitionBase> _memberDefinitions;

    public MemberOverrideReportGenerator (IEnumerable<MemberDefinitionBase> memberDefinitions)
    {
      ArgumentUtility.CheckNotNull("memberDefinitions", memberDefinitions);

      _memberDefinitions = memberDefinitions;
    }

    public XElement GenerateXml ()
    {
      return new XElement(
          "MemberOverrides",
          from overriddenMember in _memberDefinitions
          select GenerateOverriddenMemberElement(overriddenMember));
    }

    private XElement GenerateOverriddenMemberElement (MemberDefinitionBase overriddenMember)
    {
      return new XElement(
          "OverriddenMember",
          new XAttribute("type", overriddenMember.MemberType),
          new XAttribute("name", overriddenMember.Name)
      );
    }
  }
}
