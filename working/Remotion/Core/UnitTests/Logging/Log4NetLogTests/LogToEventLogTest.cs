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
using System.Diagnostics;
using System.Security;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Util;
using NUnit.Framework;
using Remotion.Logging;
using LogManager = log4net.LogManager;

namespace Remotion.UnitTests.Logging.Log4NetLogTests
{
  [TestFixture]
  public class LogToEventLogTest
  {
    private static readonly string s_eventLogName = "Remotion_UnitTests";
    private static readonly string s_eventLogSource = "LogToEventLogTest_Log";
    private bool _skipFixtureTearDown;

    private ILogger _logger;
    private ILog _log;
    private EventLog _testEventLog;

    [TestFixtureSetUp]
    public void SetUpFixture ()
    {
      try
      {
        if (!EventLog.SourceExists (s_eventLogSource))
          EventLog.CreateEventSource (s_eventLogSource, s_eventLogName);
        _skipFixtureTearDown = false;
      }
      catch (SecurityException ex)
      {
        _skipFixtureTearDown = true;
        Assert.Ignore ("Event log access denied: " + ex.Message);
      }

      _testEventLog = Array.Find (EventLog.GetEventLogs(), delegate (EventLog current) { return current.Log == s_eventLogName; });
    }

    [TestFixtureTearDown]
    public void TearDownFixture ()
    {
      if (_skipFixtureTearDown)
        return;

      if (EventLog.SourceExists (s_eventLogSource))
        EventLog.DeleteEventSource (s_eventLogSource);

      if (EventLog.Exists (s_eventLogName))
        EventLog.Delete (s_eventLogName);

      _testEventLog.Dispose();
    }

    [SetUp]
    public void SetUp ()
    {
      EventLogAppender eventLogAppender = new EventLogAppender();
      eventLogAppender.LogName = s_eventLogName;
      eventLogAppender.ApplicationName = s_eventLogSource;
      eventLogAppender.SecurityContext = NullSecurityContext.Instance;
      eventLogAppender.Layout = new PatternLayout ("%m\r\n\r\n");
      BasicConfigurator.Configure (eventLogAppender);

      _logger = LogManager.GetLogger ("The Name").Logger;
      _log = new Log4NetLog (_logger);
      _testEventLog.Clear ();
    }

    [TearDown]
    public void TearDown ()
    {
      LogManager.ResetConfiguration ();
      _testEventLog.Clear ();
    }

    [Test]
    public void LogToEventLog ()
    {
      _logger.Repository.Threshold = Level.Info;

      _log.Log (LogLevel.Info, 1, (object) "The message.", (Exception) null);
      Assert.That (_testEventLog.Entries.Count, Is.EqualTo (1));
      EventLogEntry eventLogEntry = _testEventLog.Entries[0];
      Assert.That (eventLogEntry.EntryType, Is.EqualTo (EventLogEntryType.Information));
      Assert.That (eventLogEntry.Message, Is.EqualTo ("The message.\r\n\r\n"));
      Assert.That (eventLogEntry.InstanceId, Is.EqualTo (1));
    }

    [Test]
    [ExpectedException(typeof (ArgumentOutOfRangeException), ExpectedMessage = "An event id of value 65536 is not supported. Valid event ids must be within a range of 0 and 65535.\r\nParameter name: eventID")]
    public void Log_WithEventIDGreaterThan0xFFFF ()
    {
      _logger.Repository.Threshold = Level.Info;

      try
      {
        _log.Log (LogLevel.Info, 0x10000, (object) "The message.", (Exception) null);
      }
      catch (Exception)
      {
        Assert.That (_testEventLog.Entries.Count, Is.EqualTo (1));
        EventLogEntry eventLogEntry = _testEventLog.Entries[0];
        Assert.That (eventLogEntry.EntryType, Is.EqualTo (EventLogEntryType.Error));
        Assert.That (eventLogEntry.Message, Is.EqualTo ("Failure during logging of message:\r\nThe message.\r\nEvent ID: 65536\r\n\r\n"));
        Assert.That (eventLogEntry.InstanceId, Is.EqualTo (0xFFFF));

        throw;
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException), ExpectedMessage = "An event id of value -1 is not supported. Valid event ids must be within a range of 0 and 65535.\r\nParameter name: eventID")]
    public void Log_WithEventIDLessThanZero ()
    {
      _logger.Repository.Threshold = Level.Info;

      try
      {
        _log.Log (LogLevel.Info, -1, (object) "The message.", (Exception) null);
      }
      catch (Exception)
      {
        Assert.That (_testEventLog.Entries.Count, Is.EqualTo (1));
        EventLogEntry eventLogEntry = _testEventLog.Entries[0];
        Assert.That (eventLogEntry.EntryType, Is.EqualTo (EventLogEntryType.Error));
        Assert.That (eventLogEntry.Message, Is.EqualTo ("Failure during logging of message:\r\nThe message.\r\nEvent ID: -1\r\n\r\n"));
        Assert.That (eventLogEntry.InstanceId, Is.EqualTo (0x0));

        throw;
      }
    }
  }
}
