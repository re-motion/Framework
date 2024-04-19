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
using Remotion.Logging;

namespace Remotion.Extensions.UnitTests.Logging.LoggerExtensionsObsoleteTests
{
  [TestFixture]
  public class InfoTest : BaseTest
  {
    [Test]
    public void IsEnabled_WithLevelDebug ()
    {
      SetLoggingThreshold(Level.Debug);
      Assert.That(Log.IsInformationEnabled(), Is.True);
    }

    [Test]
    public void IsEnabled_WithLevelInfo ()
    {
      SetLoggingThreshold(Level.Info);
      Assert.That(Log.IsInformationEnabled(), Is.True);
    }

    [Test]
    public void IsEnabled_WithLevelWarn ()
    {
      SetLoggingThreshold(Level.Error);
      Assert.That(Log.IsInformationEnabled(), Is.False);
    }

    [Test]
    public void Logger_Log ()
    {
      SetLoggingThreshold(Level.Info);
      Logger.Log(GetType(), Level.Info, "The message.", null);

      LoggingEvent[] events = GetLoggingEvents();
      Assert.That(events.Length, Is.EqualTo(1));
      Assert.That(events[0].Level, Is.EqualTo(Level.Info));
      Assert.That(events[0].MessageObject, Is.EqualTo("The message."));
    }

    [Test]
    public void Test_WithMessageEventIDAndException ()
    {
      Exception exception = new Exception();
      SetLoggingThreshold(Level.Info);

      Log.Info(2, (object)"The message.", exception);

      LoggingEvent[] events = GetLoggingEvents();
      Assert.That(events.Length, Is.EqualTo(1));
      LoggingEvent loggingEvent = events[0];
      Assert.That(loggingEvent.Level, Is.EqualTo(Level.Info));
      Assert.That(loggingEvent.MessageObject, Is.EqualTo("The message."));
      Assert.That(loggingEvent.Properties["EventID"], Is.EqualTo(2));
      Assert.That(loggingEvent.ExceptionObject, Is.SameAs(exception));
      Assert.That(loggingEvent.Repository, Is.SameAs(Logger.Repository));
      Assert.That(loggingEvent.LoggerName, Is.EqualTo(Logger.Name));
    }

    [Test]
    public void Test_WithMessageAndEventID ()
    {
      SetLoggingThreshold(Level.Info);

      Log.Info(1, (object)"The message.");

      LoggingEvent[] events = GetLoggingEvents();
      Assert.That(events.Length, Is.EqualTo(1));
      LoggingEvent loggingEvent = events[0];
      Assert.That(loggingEvent.Level, Is.EqualTo(Level.Info));
      Assert.That(loggingEvent.MessageObject, Is.EqualTo("The message."));
      Assert.That(loggingEvent.Properties["EventID"], Is.EqualTo(1));
      Assert.That(loggingEvent.ExceptionObject, Is.Null);
      Assert.That(loggingEvent.Repository, Is.SameAs(Logger.Repository));
      Assert.That(loggingEvent.LoggerName, Is.EqualTo(Logger.Name));
    }

    [Test]
    public void Test_WithMessageAndException ()
    {
      Exception exception = new Exception();
      SetLoggingThreshold(Level.Info);

      Log.Info((object)"The message.", exception);

      LoggingEvent[] events = GetLoggingEvents();
      Assert.That(events.Length, Is.EqualTo(1));
      LoggingEvent loggingEvent = events[0];
      Assert.That(loggingEvent.Level, Is.EqualTo(Level.Info));
      Assert.That(loggingEvent.MessageObject, Is.EqualTo("The message."));
      Assert.That(loggingEvent.ExceptionObject, Is.SameAs(exception));
      Assert.That(loggingEvent.Repository, Is.SameAs(Logger.Repository));
      Assert.That(loggingEvent.LoggerName, Is.EqualTo(Logger.Name));
    }

    [Test]
    public void Test_WithMessage ()
    {
      SetLoggingThreshold(Level.Info);

      Log.Info((object)"The message.");

      LoggingEvent[] events = GetLoggingEvents();
      Assert.That(events.Length, Is.EqualTo(1));
      LoggingEvent loggingEvent = events[0];
      Assert.That(loggingEvent.Level, Is.EqualTo(Level.Info));
      Assert.That(loggingEvent.MessageObject, Is.EqualTo("The message."));
      Assert.That(loggingEvent.ExceptionObject, Is.Null);
      Assert.That(loggingEvent.Repository, Is.SameAs(Logger.Repository));
      Assert.That(loggingEvent.LoggerName, Is.EqualTo(Logger.Name));
    }

    [Test]
    public void Test_WithLogLevelNone ()
    {
      Logger.Repository.Threshold = Level.Off;

      Log.Info(1, (object)"The message.");

      Assert.That(GetLoggingEvents(), Is.Empty);
    }

    [Test]
    public void Test_FormatWithMessageAndEventIDAndException ()
    {
      Exception exception = new Exception();
      SetLoggingThreshold(Level.Info);

      Log.InfoFormat(1, exception, "{0} {1}", "The", "message.");

      LoggingEvent[] events = GetLoggingEvents();
      Assert.That(events.Length, Is.EqualTo(1));
      LoggingEvent loggingEvent = events[0];
      Assert.That(loggingEvent.Level, Is.EqualTo(Level.Info));
      Assert.That(loggingEvent.MessageObject.ToString(), Is.EqualTo("The message."));
      Assert.That(loggingEvent.Properties["EventID"], Is.EqualTo(1));
      Assert.That(loggingEvent.ExceptionObject, Is.SameAs(exception));
      Assert.That(loggingEvent.Repository, Is.SameAs(Logger.Repository));
      Assert.That(loggingEvent.LoggerName, Is.EqualTo(Logger.Name));
    }

    [Test]
    public void Test_FormatWithMessageAndEventID ()
    {
      SetLoggingThreshold(Level.Info);

      Log.InfoFormat(1, "{0} {1}", "The", "message.");

      LoggingEvent[] events = GetLoggingEvents();
      Assert.That(events.Length, Is.EqualTo(1));
      LoggingEvent loggingEvent = events[0];
      Assert.That(loggingEvent.Level, Is.EqualTo(Level.Info));
      Assert.That(loggingEvent.MessageObject.ToString(), Is.EqualTo("The message."));
      Assert.That(loggingEvent.Properties["EventID"], Is.EqualTo(1));
      Assert.That(loggingEvent.ExceptionObject, Is.Null);
      Assert.That(loggingEvent.Repository, Is.SameAs(Logger.Repository));
      Assert.That(loggingEvent.LoggerName, Is.EqualTo(Logger.Name));
    }

    [Test]
    public void Test_FormatWithMessageAndException ()
    {
      Exception exception = new Exception();
      SetLoggingThreshold(Level.Info);

      Log.InfoFormat(exception, "{0} {1}", "The", "message.");

      LoggingEvent[] events = GetLoggingEvents();
      Assert.That(events.Length, Is.EqualTo(1));
      LoggingEvent loggingEvent = events[0];
      Assert.That(loggingEvent.Level, Is.EqualTo(Level.Info));
      Assert.That(loggingEvent.MessageObject.ToString(), Is.EqualTo("The message."));
      Assert.That(loggingEvent.ExceptionObject, Is.SameAs(exception));
      Assert.That(loggingEvent.Repository, Is.SameAs(Logger.Repository));
      Assert.That(loggingEvent.LoggerName, Is.EqualTo(Logger.Name));
    }

    [Test]
    public void Test_FormatWithMessage ()
    {
      SetLoggingThreshold(Level.Info);

      Log.InfoFormat("{0} {1}", "The", "message.");

      LoggingEvent[] events = GetLoggingEvents();
      Assert.That(events.Length, Is.EqualTo(1));
      LoggingEvent loggingEvent = events[0];
      Assert.That(loggingEvent.Level, Is.EqualTo(Level.Info));
      Assert.That(loggingEvent.MessageObject.ToString(), Is.EqualTo("The message."));
      Assert.That(loggingEvent.ExceptionObject, Is.Null);
      Assert.That(loggingEvent.Repository, Is.SameAs(Logger.Repository));
      Assert.That(loggingEvent.LoggerName, Is.EqualTo(Logger.Name));
    }

    [Test]
    public void Test_FormatWithLogLevelNone ()
    {
      Logger.Repository.Threshold = Level.Off;

      Log.InfoFormat(1, "{0} {1}", "The", "message.");

      Assert.That(GetLoggingEvents(), Is.Empty);
    }
  }
}
