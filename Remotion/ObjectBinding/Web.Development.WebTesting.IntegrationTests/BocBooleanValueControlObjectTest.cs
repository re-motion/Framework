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
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class BocBooleanValueControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var bocBooleanValue = home.GetBooleanValue().ByID ("body_DataEditControl_DeceasedField_Normal");
      Assert.That (bocBooleanValue.Scope.Id, Is.EqualTo ("body_DataEditControl_DeceasedField_Normal"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      var bocBooleanValue = home.GetBooleanValue().ByIndex (2);
      Assert.That (bocBooleanValue.Scope.Id, Is.EqualTo ("body_DataEditControl_DeceasedField_ReadOnly"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var bocBooleanValue = home.GetBooleanValue().ByLocalID ("DeceasedField_Normal");
      Assert.That (bocBooleanValue.Scope.Id, Is.EqualTo ("body_DataEditControl_DeceasedField_Normal"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      var bocBooleanValue = home.GetBooleanValue().First();
      Assert.That (bocBooleanValue.Scope.Id, Is.EqualTo ("body_DataEditControl_DeceasedField_Normal"));
    }

    [Test]
    [Category ("LongRunning")]
    public void TestSelection_Single ()
    {
      var home = Start();

      try
      {
        home.GetBooleanValue().Single();
        Assert.Fail ("Should not be able to unambigously find a BOC boolean value.");
      }
      catch (AmbiguousException)
      {
      }
    }

    [Test]
    public void TestSelection_DisplayName ()
    {
      var home = Start();

      var bocBooleanValue = home.GetBooleanValue().ByDisplayName ("Deceased");
      Assert.That (bocBooleanValue.Scope.Id, Is.EqualTo ("body_DataEditControl_DeceasedField_Normal"));
    }

    [Test]
    public void TestSelection_DomainProperty ()
    {
      var home = Start();

      var bocBooleanValue = home.GetBooleanValue().ByDomainProperty ("Deceased");
      Assert.That (bocBooleanValue.Scope.Id, Is.EqualTo ("body_DataEditControl_DeceasedField_Normal"));
    }

    [Test]
    public void TestSelection_DomainPropertyAndClass ()
    {
      var home = Start();

      var bocBooleanValue = home.GetBooleanValue()
          .ByDomainProperty ("Deceased", "Remotion.ObjectBinding.Sample.Person, Remotion.ObjectBinding.Sample");
      Assert.That (bocBooleanValue.Scope.Id, Is.EqualTo ("body_DataEditControl_DeceasedField_Normal"));
    }

    [Test]
    public void TestIsReadOnly ()
    {
      var home = Start();

      var bocBooleanValue = home.GetBooleanValue().ByLocalID ("DeceasedField_Normal");
      Assert.That (bocBooleanValue.IsReadOnly(), Is.False);

      bocBooleanValue = home.GetBooleanValue().ByLocalID ("DeceasedField_ReadOnly");
      Assert.That (bocBooleanValue.IsReadOnly(), Is.True);
    }

    [Test]
    public void TestGetState ()
    {
      var home = Start();

      var bocBooleanValue = home.GetBooleanValue().ByLocalID ("DeceasedField_Normal");
      Assert.That (bocBooleanValue.GetState(), Is.EqualTo (false));

      bocBooleanValue = home.GetBooleanValue().ByLocalID ("DeceasedField_ReadOnly");
      Assert.That (bocBooleanValue.GetState(), Is.EqualTo (false));

      bocBooleanValue = home.GetBooleanValue().ByLocalID ("DeceasedField_Disabled");
      Assert.That (bocBooleanValue.GetState(), Is.EqualTo (false));

      bocBooleanValue = home.GetBooleanValue().ByLocalID ("DeceasedField_NoAutoPostBack");
      Assert.That (bocBooleanValue.GetState(), Is.EqualTo (false));

      bocBooleanValue = home.GetBooleanValue().ByLocalID ("DeceasedField_TriState");
      Assert.That (bocBooleanValue.GetState(), Is.EqualTo (false));
    }

    [Test]
    public void TestIsTriState ()
    {
      var home = Start();

      var bocBooleanValue = home.GetBooleanValue().ByLocalID ("DeceasedField_Normal");
      Assert.That (bocBooleanValue.IsTriState(), Is.EqualTo (false));

      bocBooleanValue = home.GetBooleanValue().ByLocalID ("DeceasedField_TriState");
      Assert.That (bocBooleanValue.IsTriState(), Is.EqualTo (true));
    }

    [Test]
    public void TestSetTo ()
    {
      var home = Start();

      var normalBocBooleanValue = home.GetBooleanValue().ByLocalID ("DeceasedField_Normal");
      var noAutoPostBackBocBooleanValue = home.GetBooleanValue().ByLocalID ("DeceasedField_NoAutoPostBack");

      normalBocBooleanValue.SetTo (true);
      Assert.That (home.Scope.FindIdEndingWith ("NormalCurrentValueLabel").Text, Is.EqualTo ("True"));

      noAutoPostBackBocBooleanValue.SetTo (true);
      Assert.That (home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text, Is.EqualTo ("False"));

      normalBocBooleanValue.SetTo (true, Opt.ContinueImmediately()); // same value, does not trigger post back
      Assert.That (home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text, Is.EqualTo ("False"));

      normalBocBooleanValue.SetTo (false);
      Assert.That (home.Scope.FindIdEndingWith ("NormalCurrentValueLabel").Text, Is.EqualTo ("False"));
      Assert.That (home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text, Is.EqualTo ("True"));
    }

    [Test]
    public void TestSetToTriState ()
    {
      var home = Start();

      var bocBooleanValue = home.GetBooleanValue().ByLocalID ("DeceasedField_TriState");

      bocBooleanValue.SetTo (null);
      Assert.That (home.Scope.FindIdEndingWith ("TriStateCurrentValueLabel").Text, Is.Empty);

      bocBooleanValue.SetTo (false);
      Assert.That (home.Scope.FindIdEndingWith ("TriStateCurrentValueLabel").Text, Is.EqualTo ("False"));

      bocBooleanValue.SetTo (true);
      Assert.That (home.Scope.FindIdEndingWith ("TriStateCurrentValueLabel").Text, Is.EqualTo ("True"));
    }

    private WxePageObject Start ()
    {
      return Start ("BocBooleanValue");
    }
  }
}