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
using System.Xml.Linq;
using NUnit.Framework;
using Remotion.Mixins.CrossReferencer.Reflectors;
using Remotion.Mixins.CrossReferencer.Report;
using Remotion.Mixins.CrossReferencer.UnitTests.TestDomain;
using Remotion.Mixins.CrossReferencer.Utilities;

namespace Remotion.Mixins.CrossReferencer.UnitTests.Report
{
  [TestFixture]
  public class InterfaceReferenceReportGeneratorTest
  {
    [Test]
    public void GenerateXml_ZeroInterfaces ()
    {
      var involvedType = new InvolvedType (typeof (object));
      var reportGenerator = new InterfaceReferenceReportGenerator (involvedType, new IdentifierGenerator<Type>(), Helpers.RemotionReflectorFactory.GetRemotionReflection ());

      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement ("ImplementedInterfaces");

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_WithInterfaces ()
    {
      // TargetClass1 implements IDisposealbe
      var involvedType = new InvolvedType (typeof (TargetClass1));
      var reportGenerator = new InterfaceReferenceReportGenerator (involvedType, new IdentifierGenerator<Type>(), Helpers.RemotionReflectorFactory.GetRemotionReflection ());

      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement (
          "ImplementedInterfaces",
          new XElement ("ImplementedInterface", new XAttribute ("ref", "0"))
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_WithComposedInterface ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<ComposedInterfacesTestClass.MyMixinTarget>()
          .AddCompleteInterface<ComposedInterfacesTestClass.ICMyMixinTargetMyMixin>()
          .AddMixin<ComposedInterfacesTestClass.MyMixin>()
          .BuildConfiguration();

      // MyMixinTarget does not implement any interfaces! (but ICMyMixinTargetMyMixin is added to class context as a composed interface)
      var involvedType = new InvolvedType (typeof (ComposedInterfacesTestClass.MyMixinTarget));
      var classContext = mixinConfiguration.ClassContexts.GetWithInheritance (typeof (ComposedInterfacesTestClass.MyMixinTarget));
      involvedType.ClassContext = new ReflectedObject (classContext);

      var reportGenerator = new InterfaceReferenceReportGenerator (involvedType, new IdentifierGenerator<Type>(), Helpers.RemotionReflectorFactory.GetRemotionReflection ());
      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement (
          "ImplementedInterfaces",
          new XElement ("ImplementedInterface", new XAttribute ("ref", "0"))
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }
  }
}
