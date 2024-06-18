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
using MixinXRef.Report;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using Remotion.Mixins;

namespace MixinXRef.UnitTests.Report
{
  [TestFixture]
  public class MemberOverrideReportGeneratorTest
  {
    [Test]
    public void GenerateXml_NoOverriddenMembers ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<TargetClass1>().AddMixin<Mixin1>()
          .BuildConfiguration();

      var type1 = new InvolvedType (typeof (TargetClass1));
      type1.ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.First());

      var memberOverrides = GetMemberOverrides (type1, typeof (Mixin1), mixinConfiguration);

      var reportGenerator = new MemberOverrideReportGenerator (memberOverrides);

      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement ("MemberOverrides");

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }


    [Test]
    public void GenerateXml_WithOverriddenMembers ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<TargetDoSomething>().AddMixin<MixinDoSomething>()
          .BuildConfiguration();

      var type1 = new InvolvedType (typeof (TargetDoSomething));
      type1.ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.First());

      var memberOverrides = GetMemberOverrides (type1, typeof (MixinDoSomething), mixinConfiguration);

      var reportGenerator = new MemberOverrideReportGenerator (memberOverrides);

      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement (
          "MemberOverrides",
          new XElement (
              "OverriddenMember",
              new XAttribute ("type", "Method"),
              new XAttribute ("name", "DoSomething")
              ));

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    private ReflectedObject GetMemberOverrides (InvolvedType targetType, Type mixinType, MixinConfiguration mixinConfiguration)
    {
      var targetClassDefinition = TargetClassDefinitionUtility.GetConfiguration (targetType.Type, mixinConfiguration);
      return new ReflectedObject (targetClassDefinition.GetMixinByConfiguredType (mixinType).GetAllOverrides());
    }
  }
}