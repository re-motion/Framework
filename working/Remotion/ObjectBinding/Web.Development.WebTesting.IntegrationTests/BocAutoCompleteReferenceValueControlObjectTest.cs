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
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.ObjectBinding.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class BocAutoCompleteReferenceValueControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var bocAutoComplete = home.GetAutoComplete().ByID ("body_DataEditControl_PartnerField_Normal");
      Assert.That (bocAutoComplete.Scope.Id, Is.EqualTo ("body_DataEditControl_PartnerField_Normal"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      var bocAutoComplete = home.GetAutoComplete().ByIndex (2);
      Assert.That (bocAutoComplete.Scope.Id, Is.EqualTo ("body_DataEditControl_PartnerField_Normal_AlternativeRendering"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_Normal");
      Assert.That (bocAutoComplete.Scope.Id, Is.EqualTo ("body_DataEditControl_PartnerField_Normal"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      var bocAutoComplete = home.GetAutoComplete().First();
      Assert.That (bocAutoComplete.Scope.Id, Is.EqualTo ("body_DataEditControl_PartnerField_Normal"));
    }

    [Test]
    [Category ("LongRunning")]
    public void TestSelection_Single ()
    {
      var home = Start();

      try
      {
        home.GetAutoComplete().Single();
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

      var bocAutoComplete = home.GetAutoComplete().ByDisplayName ("Partner");
      Assert.That (bocAutoComplete.Scope.Id, Is.EqualTo ("body_DataEditControl_PartnerField_Normal"));
    }

    [Test]
    public void TestSelection_DomainProperty ()
    {
      var home = Start();

      var bocAutoComplete = home.GetAutoComplete().ByDomainProperty ("Partner");
      Assert.That (bocAutoComplete.Scope.Id, Is.EqualTo ("body_DataEditControl_PartnerField_Normal"));
    }

    [Test]
    public void TestSelection_DomainPropertyAndClass ()
    {
      var home = Start();

      var bocAutoComplete = home.GetAutoComplete().ByDomainProperty ("Partner", "Remotion.ObjectBinding.Sample.Person, Remotion.ObjectBinding.Sample");
      Assert.That (bocAutoComplete.Scope.Id, Is.EqualTo ("body_DataEditControl_PartnerField_Normal"));
    }

    [Test]
    public void TestIsReadOnly ()
    {
      var home = Start();

      var bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_Normal");
      Assert.That (bocAutoComplete.IsReadOnly(), Is.False);

      bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_ReadOnly");
      Assert.That (bocAutoComplete.IsReadOnly(), Is.True);
    }

    [Test]
    public void TestGetText ()
    {
      var home = Start();

      var bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_Normal");
      Assert.That (bocAutoComplete.GetText(), Is.EqualTo ("D, A"));

      bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_Normal_AlternativeRendering");
      Assert.That (bocAutoComplete.GetText(), Is.EqualTo ("D, A"));

      bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_ReadOnly");
      Assert.That (bocAutoComplete.GetText(), Is.EqualTo ("D, A"));

      bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_ReadOnly_AlternativeRendering");
      Assert.That (bocAutoComplete.GetText(), Is.EqualTo ("D, A"));

      bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_Disabled");
      Assert.That (bocAutoComplete.GetText(), Is.EqualTo ("D, A"));

      bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_NoAutoPostBack");
      Assert.That (bocAutoComplete.GetText(), Is.EqualTo ("D, A"));

      bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_NoCommandNoMenu");
      Assert.That (bocAutoComplete.GetText(), Is.EqualTo ("D, A"));
    }

    [Test]
    public void TestFillWith ()
    {
      var home = Start();

      const string baLabel = "c8ace752-55f6-4074-8890-130276ea6cd1";
      const string daLabel = "00000000-0000-0000-0000-000000000009";

      var bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_Normal");
      bocAutoComplete.FillWith ("Invalid");
      Assert.That (home.Scope.FindIdEndingWith ("BOUINormalLabel").Text, Is.Empty);

      bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_Normal");
      bocAutoComplete.FillWith ("B, A");
      Assert.That (home.Scope.FindIdEndingWith ("BOUINormalLabel").Text, Is.EqualTo (baLabel));

      bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_NoAutoPostBack");
      bocAutoComplete.FillWith ("B, A"); // no auto post back
      Assert.That (home.Scope.FindIdEndingWith ("BOUINoAutoPostBackLabel").Text, Is.EqualTo (daLabel));

      bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_Normal");
      bocAutoComplete.FillWith ("B, A", Opt.ContinueImmediately()); // same value, does not trigger post back
      Assert.That (home.Scope.FindIdEndingWith ("BOUINoAutoPostBackLabel").Text, Is.EqualTo (daLabel));

      bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_Normal");
      bocAutoComplete.FillWith ("D, A");
      Assert.That (home.Scope.FindIdEndingWith ("BOUINormalLabel").Text, Is.EqualTo (daLabel));
      Assert.That (home.Scope.FindIdEndingWith ("BOUINoAutoPostBackLabel").Text, Is.EqualTo (baLabel));
    }

    [Test]
    public void TestExecuteCommand ()
    {
      var home = Start();

      var bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_Normal");
      bocAutoComplete.ExecuteCommand();
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("PartnerField_Normal"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("CommandClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.Empty);

      bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_Normal_AlternativeRendering");
      bocAutoComplete.ExecuteCommand();
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("PartnerField_Normal_AlternativeRendering"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("CommandClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.Empty);

      bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_ReadOnly");
      bocAutoComplete.ExecuteCommand();
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("PartnerField_ReadOnly"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("CommandClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.Empty);

      bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_ReadOnly_AlternativeRendering");
      bocAutoComplete.ExecuteCommand();
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("PartnerField_ReadOnly_AlternativeRendering"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("CommandClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.Empty);
    }

    [Test]
    public void TestGetDropDownMenu ()
    {
      var home = Start();

      var bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_Normal");
      var dropDownMenu = bocAutoComplete.GetDropDownMenu();
      dropDownMenu.SelectItem ("OptCmd2");
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("PartnerField_Normal"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("MenuItemClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("OptCmd2|My menu command 2"));

      bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_Normal_AlternativeRendering");
      dropDownMenu = bocAutoComplete.GetDropDownMenu();
      dropDownMenu.SelectItem ("OptCmd2");
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("PartnerField_Normal_AlternativeRendering"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("MenuItemClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("OptCmd2|My menu command 2"));

      bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_ReadOnly");
      dropDownMenu = bocAutoComplete.GetDropDownMenu();
      dropDownMenu.SelectItem ("OptCmd2");
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("PartnerField_ReadOnly"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("MenuItemClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("OptCmd2|My menu command 2"));

      bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_ReadOnly_AlternativeRendering");
      dropDownMenu = bocAutoComplete.GetDropDownMenu();
      dropDownMenu.SelectItem ("OptCmd2");
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("PartnerField_ReadOnly_AlternativeRendering"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("MenuItemClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("OptCmd2|My menu command 2"));
    }

    [Test]
    public void TestGetSearchServiceResults ()
    {
      var home = Start();

      var bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_Normal");

      var searchResults = bocAutoComplete.GetSearchServiceResults ("D", 1);
      Assert.That (searchResults.Count, Is.EqualTo (1));
      Assert.That (searchResults[0].UniqueIdentifier, Is.EqualTo ("a2752869-e46b-4cfa-b89f-0b824e42b250"));
      Assert.That (searchResults[0].DisplayName, Is.EqualTo ("D, "));
      Assert.That (searchResults[0].IconUrl, Is.EqualTo ("/Images/Remotion.ObjectBinding.Sample.Person.gif"));

      searchResults = bocAutoComplete.GetSearchServiceResults ("D", 5);
      Assert.That (searchResults.Count, Is.EqualTo (3));
      Assert.That (searchResults[0].DisplayName, Is.EqualTo ("D, "));

      searchResults = bocAutoComplete.GetSearchServiceResults ("unexistentValue", 5);
      Assert.That (searchResults.Count, Is.EqualTo (0));
    }

    [Test]
    public void TestGetExactSearchServiceResult ()
    {
      var home = Start();

      var bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_Normal");

      var searchResult = bocAutoComplete.GetExactSearchServiceResult ("D, ");
      Assert.That (searchResult.UniqueIdentifier, Is.EqualTo ("a2752869-e46b-4cfa-b89f-0b824e42b250"));
      Assert.That (searchResult.DisplayName, Is.EqualTo ("D, "));
      Assert.That (searchResult.IconUrl, Is.EqualTo ("/Images/Remotion.ObjectBinding.Sample.Person.gif"));

      searchResult = bocAutoComplete.GetExactSearchServiceResult ("D");
      Assert.That (searchResult, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (WebServiceExceutionException))]
    public void TestGetSearchServiceResultsException ()
    {
      var home = Start();

      var bocAutoComplete = home.GetAutoComplete().ByLocalID ("PartnerField_Normal");

      bocAutoComplete.GetSearchServiceResults ("throw", 1);
    }

    private WxePageObject Start ()
    {
      return Start ("BocAutoCompleteReferenceValue");
    }
  }
}