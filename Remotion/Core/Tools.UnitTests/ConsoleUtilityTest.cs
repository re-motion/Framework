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
using NUnit.Framework;
using Remotion.Tools.Console;
using Rhino.Mocks;

namespace Remotion.Tools.UnitTests
{
  [TestFixture]
  public class ConsoleUtilityTest
  {
    private IConsoleManager _consoleStub;

    [SetUp]
    public void SetUp ()
    {
      _consoleStub = new MockRepository().Stub<IConsoleManager>();
      _consoleStub.ForegroundColor = ConsoleColor.Gray;
      _consoleStub.BackgroundColor = ConsoleColor.Black;
    }

    [Test]
    public void EnterColorScope_NullForegroundColor ()
    {
      Assert.That (_consoleStub.ForegroundColor, Is.EqualTo (ConsoleColor.Gray));
      using (new ConsoleUtility.ColorScope (_consoleStub, null, ConsoleColor.Gray))
      {
        Assert.That (_consoleStub.ForegroundColor, Is.EqualTo (ConsoleColor.Gray));
        _consoleStub.ForegroundColor = ConsoleColor.Yellow;
      }
      Assert.That (_consoleStub.ForegroundColor, Is.EqualTo (ConsoleColor.Yellow));
    }

    [Test]
    public void EnterColorScope_ForegroundColor_SetsColor ()
    {
      Assert.That (_consoleStub.ForegroundColor, Is.EqualTo (ConsoleColor.Gray));
      using (new ConsoleUtility.ColorScope (_consoleStub, ConsoleColor.Green, ConsoleColor.Gray))
      {
        Assert.That (_consoleStub.ForegroundColor, Is.EqualTo (ConsoleColor.Green));
      }
    }

    [Test]
    public void EnterColorScope_ForegroundColor_RestoresColor ()
    {
      _consoleStub.ForegroundColor = ConsoleColor.Magenta;
      using (new ConsoleUtility.ColorScope (_consoleStub, ConsoleColor.Blue, ConsoleColor.Gray))
      {
        Assert.That (_consoleStub.ForegroundColor, Is.EqualTo (ConsoleColor.Blue));
      }
      Assert.That (_consoleStub.ForegroundColor, Is.EqualTo (ConsoleColor.Magenta));
    }

    [Test]
    public void EnterColorScope_NullBackgroundColor ()
    {
      Assert.That (_consoleStub.BackgroundColor, Is.EqualTo (ConsoleColor.Black));
      using (new ConsoleUtility.ColorScope (_consoleStub, ConsoleColor.Green, null))
      {
        Assert.That (_consoleStub.BackgroundColor, Is.EqualTo (ConsoleColor.Black));
        _consoleStub.BackgroundColor = ConsoleColor.Green;
      }
      Assert.That (_consoleStub.BackgroundColor, Is.EqualTo (ConsoleColor.Green));
    }

    [Test]
    public void EnterColorScope_BackgroundColor_SetsColor ()
    {
      Assert.That (_consoleStub.BackgroundColor, Is.EqualTo (ConsoleColor.Black));
      using (new ConsoleUtility.ColorScope (_consoleStub, ConsoleColor.Gray, ConsoleColor.Green))
      {
        Assert.That (_consoleStub.BackgroundColor, Is.EqualTo (ConsoleColor.Green));
      }
    }

    [Test]
    public void EnterColorScope_BackgroundColor_RestoresColor ()
    {
      _consoleStub.BackgroundColor = ConsoleColor.Magenta;
      using (new ConsoleUtility.ColorScope (_consoleStub, ConsoleColor.Gray, ConsoleColor.Blue))
      {
        Assert.That (_consoleStub.BackgroundColor, Is.EqualTo (ConsoleColor.Blue));
      }
      Assert.That (_consoleStub.BackgroundColor, Is.EqualTo (ConsoleColor.Magenta));
    }

    [Test]
    public void EnterColorScope_DisposeTwiceIgnored ()
    {
      _consoleStub.ForegroundColor = ConsoleColor.White;
      _consoleStub.BackgroundColor = ConsoleColor.Red;
      IDisposable scope = new ConsoleUtility.ColorScope (_consoleStub, ConsoleColor.Green, ConsoleColor.Magenta);
      Assert.AreEqual (ConsoleColor.Green, _consoleStub.ForegroundColor, "color was set");
      Assert.AreEqual (ConsoleColor.Magenta, _consoleStub.BackgroundColor, "color was set");
      scope.Dispose();
      Assert.AreEqual (ConsoleColor.White, _consoleStub.ForegroundColor, "color was restored");
      Assert.AreEqual (ConsoleColor.Red, _consoleStub.BackgroundColor, "color was restored");
      _consoleStub.ForegroundColor = ConsoleColor.Yellow;
      _consoleStub.BackgroundColor = ConsoleColor.DarkYellow;
      scope.Dispose ();
      Assert.AreEqual (ConsoleColor.Yellow, _consoleStub.ForegroundColor, "second dispose ignored");
      Assert.AreEqual (ConsoleColor.DarkYellow, _consoleStub.BackgroundColor, "second dispose ignored");
    }
  }
}
