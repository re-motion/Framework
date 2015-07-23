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
using Coypu;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class BocReferenceValueControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var bocReferenceValue = home.GetReferenceValue().ByID ("body_DataEditControl_PartnerField_Normal");
      Assert.That (bocReferenceValue.Scope.Id, Is.EqualTo ("body_DataEditControl_PartnerField_Normal"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      var bocReferenceValue = home.GetReferenceValue().ByIndex (2);
      Assert.That (bocReferenceValue.Scope.Id, Is.EqualTo ("body_DataEditControl_PartnerField_Normal_AlternativeRendering"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_Normal");
      Assert.That (bocReferenceValue.Scope.Id, Is.EqualTo ("body_DataEditControl_PartnerField_Normal"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      var bocReferenceValue = home.GetReferenceValue().First();
      Assert.That (bocReferenceValue.Scope.Id, Is.EqualTo ("body_DataEditControl_PartnerField_Normal"));
    }

    [Test]
    [Category ("LongRunning")]
    public void TestSelection_Single ()
    {
      var home = Start();

      try
      {
        home.GetReferenceValue().Single();
        Assert.Fail ("Should not be able to unambigously find a BOC auto complete reference value.");
      }
      catch (AmbiguousException)
      {
      }
    }

    [Test]
    public void TestSelection_DisplayName ()
    {
      var home = Start();

      var bocReferenceValue = home.GetReferenceValue().ByDisplayName ("Partner");
      Assert.That (bocReferenceValue.Scope.Id, Is.EqualTo ("body_DataEditControl_PartnerField_Normal"));
    }

    [Test]
    public void TestSelection_DomainProperty ()
    {
      var home = Start();

      var bocReferenceValue = home.GetReferenceValue().ByDomainProperty ("Partner");
      Assert.That (bocReferenceValue.Scope.Id, Is.EqualTo ("body_DataEditControl_PartnerField_Normal"));
    }

    [Test]
    public void TestSelection_DomainPropertyAndClass ()
    {
      var home = Start();

      var bocReferenceValue = home.GetReferenceValue()
          .ByDomainProperty ("Partner", "Remotion.ObjectBinding.Sample.Person, Remotion.ObjectBinding.Sample");
      Assert.That (bocReferenceValue.Scope.Id, Is.EqualTo ("body_DataEditControl_PartnerField_Normal"));
    }

    [Test]
    public void TestIsReadOnly ()
    {
      var home = Start();

      var bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_Normal");
      Assert.That (bocReferenceValue.IsReadOnly(), Is.False);

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_ReadOnly");
      Assert.That (bocReferenceValue.IsReadOnly(), Is.True);
    }

    [Test]
    public void TestGetSelectedOption ()
    {
      var home = Start();

      const string daValue = "00000000-0000-0000-0000-000000000009";

      var bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_Normal");
      Assert.That (bocReferenceValue.GetSelectedOption().ItemID, Is.EqualTo (daValue));
      Assert.That (bocReferenceValue.GetSelectedOption().Index, Is.EqualTo (-1));
      Assert.That (bocReferenceValue.GetSelectedOption().Text, Is.EqualTo ("D, A"));
    }

    [Test]
    public void TestGetOptionDefinitions ()
    {
      var home = Start();

      const string fValue = "ef898bb1-7095-46d2-872b-5f732a7c0036";

      var bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_Normal");
      
      var options = bocReferenceValue.GetOptionDefinitions();
      Assert.That (options.Count, Is.EqualTo (16));
      
      Assert.That (options[0].ItemID, Is.EqualTo ("==null=="));
      Assert.That (options[0].Index, Is.EqualTo (1));
      Assert.That (options[0].Text, Is.Empty);

      Assert.That (options[15].ItemID, Is.EqualTo (fValue));
      Assert.That (options[15].Index, Is.EqualTo (16));
      if (WebTestingConfiguration.Current.BrowserIsInternetExplorer())
        Assert.That (options[15].Text, Is.EqualTo ("F,"));
      else
        Assert.That (options[15].Text, Is.EqualTo ("F, "));
    }

    [Test]
    [ExpectedException(typeof(InvalidOperationException))]
    public void TestGetOptionDefinitionsException ()
    {
      var home = Start();

      var bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_ReadOnly");
      bocReferenceValue.GetOptionDefinitions();
    }

    [Test]
    public void TestGetText ()
    {
      var home = Start();

      var bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_Normal");
      Assert.That (bocReferenceValue.GetText(), Is.EqualTo ("D, A"));

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_Normal_AlternativeRendering");
      Assert.That (bocReferenceValue.GetText(), Is.EqualTo ("D, A"));

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_ReadOnly");
      Assert.That (bocReferenceValue.GetText(), Is.EqualTo ("D, A"));

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_ReadOnly_AlternativeRendering");
      Assert.That (bocReferenceValue.GetText(), Is.EqualTo ("D, A"));

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_Disabled");
      Assert.That (bocReferenceValue.GetText(), Is.EqualTo ("D, A"));

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_NoAutoPostBack");
      Assert.That (bocReferenceValue.GetText(), Is.EqualTo ("D, A"));

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_NoCommandNoMenu");
      Assert.That (bocReferenceValue.GetText(), Is.EqualTo ("D, A"));
    }

    [Test]
    public void TestSelectOption ()
    {
      var home = Start();

      const string baValue = "c8ace752-55f6-4074-8890-130276ea6cd1";
      const string daValue = "00000000-0000-0000-0000-000000000009";

      var bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_Normal");
      bocReferenceValue.SelectOption (baValue);
      Assert.That (home.Scope.FindIdEndingWith ("BOUINormalLabel").Text, Is.EqualTo (baValue));

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_Normal");
      bocReferenceValue.SelectOption().WithIndex (1);
      Assert.That (home.Scope.FindIdEndingWith ("BOUINormalLabel").Text, Is.Empty);

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_Normal");
      bocReferenceValue.SelectOption().WithDisplayText ("D, A");
      Assert.That (home.Scope.FindIdEndingWith ("BOUINormalLabel").Text, Is.EqualTo (daValue));
    }

    [Test]
    public void TestSelectOptionPostBack ()
    {
      var home = Start();

      const string baValue = "c8ace752-55f6-4074-8890-130276ea6cd1";
      const string daValue = "00000000-0000-0000-0000-000000000009";

      var bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_Normal");
      bocReferenceValue.SelectOption ("==null==");
      Assert.That (home.Scope.FindIdEndingWith ("BOUINormalLabel").Text, Is.Empty);

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_Normal");
      bocReferenceValue.SelectOption (baValue);
      Assert.That (home.Scope.FindIdEndingWith ("BOUINormalLabel").Text, Is.EqualTo (baValue));

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_NoAutoPostBack");
      bocReferenceValue.SelectOption (baValue); // no auto post back
      Assert.That (home.Scope.FindIdEndingWith ("BOUINoAutoPostBackLabel").Text, Is.EqualTo (daValue));

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_Normal");
      bocReferenceValue.SelectOption (baValue, Opt.ContinueImmediately()); // same value, does not trigger post back
      Assert.That (home.Scope.FindIdEndingWith ("BOUINoAutoPostBackLabel").Text, Is.EqualTo (daValue));

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_Normal");
      bocReferenceValue.SelectOption (daValue);
      Assert.That (home.Scope.FindIdEndingWith ("BOUINormalLabel").Text, Is.EqualTo (daValue));
      Assert.That (home.Scope.FindIdEndingWith ("BOUINoAutoPostBackLabel").Text, Is.EqualTo (baValue));
    }

    [Test]
    public void TestExecuteCommand ()
    {
      var home = Start();

      var bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_Normal");
      bocReferenceValue.ExecuteCommand();
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("PartnerField_Normal"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("CommandClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.Empty);

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_Normal_AlternativeRendering");
      bocReferenceValue.ExecuteCommand();
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("PartnerField_Normal_AlternativeRendering"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("CommandClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.Empty);

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_ReadOnly");
      bocReferenceValue.ExecuteCommand();
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("PartnerField_ReadOnly"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("CommandClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.Empty);

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_ReadOnly_AlternativeRendering");
      bocReferenceValue.ExecuteCommand();
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("PartnerField_ReadOnly_AlternativeRendering"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("CommandClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.Empty);
    }

    [Test]
    public void TestGetDropDownMenu ()
    {
      var home = Start();

      var bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_Normal");
      var dropDownMenu = bocReferenceValue.GetDropDownMenu();
      dropDownMenu.SelectItem ("OptCmd2");
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("PartnerField_Normal"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("MenuItemClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("OptCmd2|My menu command 2"));

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_Normal_AlternativeRendering");
      dropDownMenu = bocReferenceValue.GetDropDownMenu();
      dropDownMenu.SelectItem ("OptCmd2");
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("PartnerField_Normal_AlternativeRendering"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("MenuItemClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("OptCmd2|My menu command 2"));

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_ReadOnly");
      dropDownMenu = bocReferenceValue.GetDropDownMenu();
      dropDownMenu.SelectItem ("OptCmd2");
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("PartnerField_ReadOnly"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("MenuItemClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("OptCmd2|My menu command 2"));

      bocReferenceValue = home.GetReferenceValue().ByLocalID ("PartnerField_ReadOnly_AlternativeRendering");
      dropDownMenu = bocReferenceValue.GetDropDownMenu();
      dropDownMenu.SelectItem ("OptCmd2");
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("PartnerField_ReadOnly_AlternativeRendering"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("MenuItemClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("OptCmd2|My menu command 2"));
    }

    private WxePageObject Start ()
    {
      return Start ("BocReferenceValue");
    }
  }
}