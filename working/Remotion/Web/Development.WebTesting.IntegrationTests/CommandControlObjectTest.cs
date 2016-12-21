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
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class CommandControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var command = home.GetCommand().ByID ("body_Command1");
      Assert.That (command.Scope.Id, Is.EqualTo ("body_Command1"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      var command = home.GetCommand().ByIndex (2);
      Assert.That (command.Scope.Id, Is.EqualTo ("body_Command2"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var command = home.GetCommand().ByLocalID ("Command1");
      Assert.That (command.Scope.Id, Is.EqualTo ("body_Command1"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      var command = home.GetCommand().First();
      Assert.That (command.Scope.Id, Is.EqualTo ("body_Command1"));
    }

    [Test]
    [Category ("LongRunning")]
    public void TestSelection_Single ()
    {
      var home = Start();
      var scope = home.GetScope().ByID ("scope");

      var command = scope.GetCommand().Single();
      Assert.That (command.Scope.Id, Is.EqualTo ("body_Command2"));

      try
      {
        home.GetCommand().Single();
        Assert.Fail ("Should not be able to unambigously find a command.");
      }
      catch (AmbiguousException)
      {
      }
    }

    [Test]
    public void TestClick ()
    {
      var home = Start();

      var command1 = home.GetCommand().ByLocalID ("Command1");
      home = command1.Click().Expect<WxePageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("Command1ItemID"));

      var command2 = home.GetCommand().ByLocalID ("Command2");
      home = command2.Click().Expect<WxePageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.Empty);
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject> ("CommandTest.wxe");
    }
  }
}