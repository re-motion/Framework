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
using Remotion.Mixins.CrossReferencer.UnitTests.TestDomain;

namespace Remotion.Mixins.CrossReferencer.UnitTests
{
  [TestFixture]
  public class SummaryPickerTest
  {
    private readonly XElement _noSummary = new XElement("summary", "No summary found.");
    private SummaryPicker _summaryPicker;

    [SetUp]
    public void SetUp ()
    {
      _summaryPicker = new SummaryPicker();
    }

    [Test]
    public void GetSummary_ForNonExistingXmlFile ()
    {
      var output = _summaryPicker.GetSummary(typeof(TargetClass1));
      var expectedOutput = _noSummary;

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }

    [Test]
    public void GetSummary_ForValidTypeWithXmlFile_SummaryPresent ()
    {
      var output = _summaryPicker.GetSummary(typeof(MixinConfiguration));
      var expectedOutput =
          "<summary>Constitutes a mixin configuration (ie. a set of classes associated with mixins) and manages the mixin configuration for the current thread (actually:";

      Assert.That(output.ToString(), Does.StartWith(expectedOutput));
    }

    [Test]
    public void NormalizeAndTrim_PlainElement ()
    {
      var element1 = new XElement("TestElement", "   test1   \r\n test2 is     Test    ");

      var output = _summaryPicker.NormalizeAndTrim(element1);

      var expectedOutput = new XElement("TestElement", "test1 test2 is Test");

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }

    [Test]
    public void NormalizeAndTrim_WithNestedElements ()
    {
      var element1 = new XElement(
          "OuterElement",
          "   test   ",
          new XElement("innerElement", " test     of   \t inner   \r\nelement   "),
          "end  "
      );

      var output = _summaryPicker.NormalizeAndTrim(element1);

      var expectedOutput = new XElement("OuterElement", "test", new XElement("innerElement", "test of inner element"), "end");

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }
  }
}
