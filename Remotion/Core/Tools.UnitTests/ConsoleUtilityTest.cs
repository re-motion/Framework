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
using Moq;
using NUnit.Framework;
using Remotion.Tools.Console;

namespace Remotion.Tools.UnitTests
{
  [TestFixture]
  public class ConsoleUtilityTest
  {
    private Mock<IConsoleManager> _consoleStub;

    [SetUp]
    public void SetUp ()
    {
      _consoleStub = new Mock<IConsoleManager>();
      _consoleStub.SetupProperty(_ => _.ForegroundColor);
      _consoleStub.Object.ForegroundColor = ConsoleColor.Gray;
      _consoleStub.SetupProperty(_ => _.BackgroundColor);
      _consoleStub.Object.BackgroundColor = ConsoleColor.Black;
    }

    [Test]
    public void EnterColorScope_NullForegroundColor ()
    {
      Assert.That(_consoleStub.Object.ForegroundColor, Is.EqualTo(ConsoleColor.Gray));
      using (new ConsoleUtility.ColorScope(_consoleStub.Object, null, ConsoleColor.Gray))
      {
        Assert.That(_consoleStub.Object.ForegroundColor, Is.EqualTo(ConsoleColor.Gray));
        _consoleStub.Object.ForegroundColor = ConsoleColor.Yellow;
      }
      Assert.That(_consoleStub.Object.ForegroundColor, Is.EqualTo(ConsoleColor.Yellow));
    }

    [Test]
    public void EnterColorScope_ForegroundColor_SetsColor ()
    {
      Assert.That(_consoleStub.Object.ForegroundColor, Is.EqualTo(ConsoleColor.Gray));
      using (new ConsoleUtility.ColorScope(_consoleStub.Object, ConsoleColor.Green, ConsoleColor.Gray))
      {
        Assert.That(_consoleStub.Object.ForegroundColor, Is.EqualTo(ConsoleColor.Green));
      }
    }

    [Test]
    public void EnterColorScope_ForegroundColor_RestoresColor ()
    {
      _consoleStub.Object.ForegroundColor = ConsoleColor.Magenta;
      using (new ConsoleUtility.ColorScope(_consoleStub.Object, ConsoleColor.Blue, ConsoleColor.Gray))
      {
        Assert.That(_consoleStub.Object.ForegroundColor, Is.EqualTo(ConsoleColor.Blue));
      }
      Assert.That(_consoleStub.Object.ForegroundColor, Is.EqualTo(ConsoleColor.Magenta));
    }

    [Test]
    public void EnterColorScope_NullBackgroundColor ()
    {
      Assert.That(_consoleStub.Object.BackgroundColor, Is.EqualTo(ConsoleColor.Black));
      using (new ConsoleUtility.ColorScope(_consoleStub.Object, ConsoleColor.Green, null))
      {
        Assert.That(_consoleStub.Object.BackgroundColor, Is.EqualTo(ConsoleColor.Black));
        _consoleStub.Object.BackgroundColor = ConsoleColor.Green;
      }
      Assert.That(_consoleStub.Object.BackgroundColor, Is.EqualTo(ConsoleColor.Green));
    }

    [Test]
    public void EnterColorScope_BackgroundColor_SetsColor ()
    {
      Assert.That(_consoleStub.Object.BackgroundColor, Is.EqualTo(ConsoleColor.Black));
      using (new ConsoleUtility.ColorScope(_consoleStub.Object, ConsoleColor.Gray, ConsoleColor.Green))
      {
        Assert.That(_consoleStub.Object.BackgroundColor, Is.EqualTo(ConsoleColor.Green));
      }
    }

    [Test]
    public void EnterColorScope_BackgroundColor_RestoresColor ()
    {
      _consoleStub.Object.BackgroundColor = ConsoleColor.Magenta;
      using (new ConsoleUtility.ColorScope(_consoleStub.Object, ConsoleColor.Gray, ConsoleColor.Blue))
      {
        Assert.That(_consoleStub.Object.BackgroundColor, Is.EqualTo(ConsoleColor.Blue));
      }
      Assert.That(_consoleStub.Object.BackgroundColor, Is.EqualTo(ConsoleColor.Magenta));
    }

    [Test]
    public void EnterColorScope_DisposeTwiceIgnored ()
    {
      _consoleStub.Object.ForegroundColor = ConsoleColor.White;
      _consoleStub.Object.BackgroundColor = ConsoleColor.Red;
      IDisposable scope = new ConsoleUtility.ColorScope(_consoleStub.Object, ConsoleColor.Green, ConsoleColor.Magenta);
      Assert.That(_consoleStub.Object.ForegroundColor, Is.EqualTo(ConsoleColor.Green), "color was set");
      Assert.That(_consoleStub.Object.BackgroundColor, Is.EqualTo(ConsoleColor.Magenta), "color was set");
      scope.Dispose();
      Assert.That(_consoleStub.Object.ForegroundColor, Is.EqualTo(ConsoleColor.White), "color was restored");
      Assert.That(_consoleStub.Object.BackgroundColor, Is.EqualTo(ConsoleColor.Red), "color was restored");
      _consoleStub.Object.ForegroundColor = ConsoleColor.Yellow;
      _consoleStub.Object.BackgroundColor = ConsoleColor.DarkYellow;
      scope.Dispose();
      Assert.That(_consoleStub.Object.ForegroundColor, Is.EqualTo(ConsoleColor.Yellow), "second dispose ignored");
      Assert.That(_consoleStub.Object.BackgroundColor, Is.EqualTo(ConsoleColor.DarkYellow), "second dispose ignored");
    }
  }
}
