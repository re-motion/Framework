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
using System.Linq;
using NUnit.Framework;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class BocControlObjectTest : IntegrationTest
  {
    [Test]
    public void GetLabelWithMutlipleLabels ()
    {
      var home = Start();
      var label = home.Scope.FindIdEndingWith ("_DeceasedField_Normal_Label");
      var bocCheckBox = home.CheckBoxes().GetByLocalID ("DeceasedField_Normal");
      var driver = Helper.MainBrowserSession.Driver;

      var secondLabelText = "secondLabelText";

      // Duplicate the label and add the information to the attributes of bocCheckBox
      driver.ExecuteScript (
          string.Format (
              @"
var label = document.getElementById('{0}');
var clonedLabel = label.cloneNode();
var secondLabelId = 'secondLabelID';
clonedLabel.id = secondLabelId;
clonedLabel.innerHTML = '{1}'

label.parentNode.appendChild (clonedLabel);

var checkBox = document.getElementById('{2}').firstChild;
var checkBoxLabeledByAttribute = checkBox.getAttribute ('aria-labelledby');
checkBoxLabeledByAttribute = checkBoxLabeledByAttribute + ' ' + secondLabelId;
checkBox.setAttribute('aria-labelledby', checkBoxLabeledByAttribute);

var checkBoxNameLabelIndex = checkBox.getAttribute ('{3}');
checkBoxNameLabelIndex = checkBoxNameLabelIndex + ' 1';
checkBox.setAttribute('{3}', checkBoxNameLabelIndex);",
              label.Id,
              secondLabelText,
              bocCheckBox.GetHtmlID(),
              DiagnosticMetadataAttributes.LabelIDIndex),
          home.Scope);


      var labels = bocCheckBox.GetLabels();

      Assert.That (labels.Count, Is.EqualTo (2));
      Assert.That (labels.First().GetText(), Is.EqualTo ("Deceased"));
      Assert.That (labels.Last().GetText(), Is.EqualTo (secondLabelText));
    }
    private WxePageObject Start ()
    {
      return Start ("BocCheckBox");
    }
  }
}
