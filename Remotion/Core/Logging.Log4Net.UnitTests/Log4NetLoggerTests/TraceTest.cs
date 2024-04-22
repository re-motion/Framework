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
using log4net.Core;
using NUnit.Framework;

using MicrosoftLogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Remotion.Logging.Log4Net.UnitTests.Log4NetLoggerTests
{
  [TestFixture]
  public class TraceTest : BaseTest
  {
    [Test]
    public void IsEnabled_WithLevelAll ()
    {
      Logger.Repository.Threshold = Level.All;
      Assert.That(Log.IsEnabled(MicrosoftLogLevel.Trace), Is.True);
    }

    [Test]
    public void IsEnabled_WithLevelTrace ()
    {
      SetLoggingThreshold(Level.Trace);
      Assert.That(Log.IsEnabled(MicrosoftLogLevel.Trace), Is.True);
    }

    [Test]
    public void IsEnabled_WithLevelDebug ()
    {
      SetLoggingThreshold(Level.Debug);
      Assert.That(Log.IsEnabled(MicrosoftLogLevel.Trace), Is.False);
    }

    [Test]
    public void Logger_Log ()
    {
      SetLoggingThreshold(Level.Trace);
      Log.Log(MicrosoftLogLevel.Trace, 1, "The message.", (Exception)null, (s,_) => s);

      LoggingEvent[] events = GetLoggingEvents();
      Assert.That(events.Length, Is.EqualTo(1));
      Assert.That(events[0].Level, Is.EqualTo(Level.Trace));
      Assert.That(events[0].MessageObject, Is.EqualTo("The message."));
    }
  }
}