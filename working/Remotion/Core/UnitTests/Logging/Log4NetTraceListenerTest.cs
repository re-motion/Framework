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
using System.Reflection;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Repository;
using NUnit.Framework;
using Remotion.Logging;
using Rhino.Mocks;
using LogManager = log4net.LogManager;

namespace Remotion.UnitTests.Logging
{
  [TestFixture]
  public class Log4NetTraceListenerTest
  {
    private MemoryAppender _memoryAppender;
    private Log4NetTraceListener _listener;
    private Log4NetTraceListener _filterListener;
    private ILoggerRepository _repository;
    private TraceEventCache _traceEventCache;
    private MockRepository _mocks;
    private TraceFilter _mockFilter;

    [SetUp]
    public void SetUp ()
    {
      _memoryAppender = new MemoryAppender ();
      BasicConfigurator.Configure (_memoryAppender);

      _repository = LoggerManager.GetRepository (Assembly.GetExecutingAssembly());
      _repository.Threshold = Level.All;

      _listener = new Log4NetTraceListener ();
      _filterListener = new Log4NetTraceListener ("FilterListener");
      
      _mocks = new MockRepository();
      _mockFilter = _mocks.StrictMock<TraceFilter>();

      _filterListener.Filter = _mockFilter;

      _traceEventCache = new TraceEventCache ();
    }

    [TearDown]
    public void TearDown ()
    {
      _listener.Dispose ();
      _filterListener.Dispose ();

      LogManager.ResetConfiguration ();
    }
    
    [Test]
    public void Test_ListenerName ()
    {
      Assert.That ("FilterListener", Is.EqualTo (_filterListener.Name));
    }

    [Test]
    public void Test_Write ()
    {
      _listener.Write ("The message.");

      LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.That (events.Length, Is.EqualTo (1));
      Assert.That (events[0].Level, Is.EqualTo (Level.Debug));
      Assert.That (events[0].MessageObject.ToString (), Is.EqualTo ("The message."));
    }

    [Test]
    public void Test_WriteLine ()
    {
      _listener.WriteLine ("The message.");

      LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.That (events.Length, Is.EqualTo (1));
      Assert.That (events[0].Level, Is.EqualTo (Level.Debug));
      Assert.That (events[0].MessageObject.ToString (), Is.EqualTo ("The message."));
    }

#if TRACE
    [Test]
    public void Test_Write_WithTrace ()
    {
      Trace.Listeners.Add (_listener);
      Trace.Write ("The message.");
      Trace.Listeners.Remove (_listener);

      LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.That (events.Length, Is.EqualTo (1));
      Assert.That (events[0].Level, Is.EqualTo (Level.Debug));
      Assert.That (events[0].MessageObject.ToString (), Is.EqualTo ("The message."));
    }
#endif

#if DEBUG
    [Test]
    public void Test_Write_WithDebug ()
    {
      Debug.Listeners.Add (_listener);
      Debug.Write ("The message.");
      Debug.Listeners.Remove (_listener);

      LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.That (events.Length, Is.EqualTo (1));
      Assert.That (events[0].Level, Is.EqualTo (Level.Debug));
      Assert.That (events[0].MessageObject.ToString (), Is.EqualTo ("The message."));
    }
#endif

    [Test]
    public void Test_TraceInformation_WithTraceSource ()
    {
      TraceSource traceSource = new TraceSource ("TestSource");
      traceSource.Switch.Level = SourceLevels.All;

      traceSource.Listeners.Add (_listener);
      traceSource.TraceInformation ("The message.");
      traceSource.Listeners.Remove (_listener);

      LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.That (events.Length, Is.EqualTo (1));
      Assert.That (events[0].Level, Is.EqualTo (Level.Info));
      Assert.That (events[0].MessageObject.ToString (), Is.EqualTo ("The message."));
    }


    [Test]
    public void Test_TraceEvent ()
    {
      _listener.TraceEvent (null, "Test", TraceEventType.Information, 1);

      LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.That (events.Length, Is.EqualTo (1));
      Assert.That (events[0].Level, Is.EqualTo (Level.Info));
      Assert.That (events[0].MessageObject.ToString (), Is.Empty);
    }

    [Test]
    public void Test_TraceEvent_WithMessage ()
    {
      _listener.TraceEvent (null, "Test", TraceEventType.Information, 1, "The message.");

      LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.That (events.Length, Is.EqualTo (1));
      Assert.That (events[0].Level, Is.EqualTo (Level.Info));
      Assert.That (events[0].MessageObject.ToString (), Is.EqualTo ("The message."));
    }

    [Test]
    public void Test_TraceEvent_WithFormat ()
    {
      _listener.TraceEvent (null, "Test", TraceEventType.Information, 1, "{0} {1}", "The", "message.");

      LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.That (events.Length, Is.EqualTo (1));
      Assert.That (events[0].Level, Is.EqualTo (Level.Info));
      Assert.That (events[0].MessageObject.ToString (), Is.EqualTo ("The message."));
    }

    [Test]
    public void Test_TraceEvent_WithFormatAndFilterReturnsTrue ()
    {
      Expect
          .Call (_mockFilter.ShouldTrace (_traceEventCache, "Test", TraceEventType.Information, 1, "{0} {1}", new object[] { "The", "message." }, null, null))
          .Return (true);
      _mocks.ReplayAll();

      _filterListener.TraceEvent (_traceEventCache, "Test", TraceEventType.Information, 1, "{0} {1}", "The", "message.");

      _mocks.VerifyAll();
      LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.That (events.Length, Is.EqualTo (1));
      Assert.That (events[0].Level, Is.EqualTo (Level.Info));
      Assert.That (events[0].MessageObject.ToString (), Is.EqualTo ("The message."));
    }

    [Test]
    public void Test_TraceEvent_WithFormatAndFilterReturnsFalse ()
    {
      Expect
          .Call (_mockFilter.ShouldTrace (_traceEventCache, "Test", TraceEventType.Information, 1, "{0} {1}", new object[] { "The", "message." }, null, null))
          .Return (false);
      _mocks.ReplayAll ();

      _filterListener.TraceEvent (_traceEventCache, "Test", TraceEventType.Information, 1, "{0} {1}", "The", "message.");

      _mocks.VerifyAll ();
      Assert.That (_memoryAppender.GetEvents (), Is.Empty);
    }

    [Test]
    public void Test_TraceData ()
    {
      Exception exception = new Exception ("An exception.");

      _listener.TraceData (null, "Test", TraceEventType.Information, 1, exception);

      LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.That (events.Length, Is.EqualTo (1));
      Assert.That (events[0].Level, Is.EqualTo (Level.Info));
      Assert.That (events[0].MessageObject.ToString (), Is.EqualTo (exception.ToString()));
    } 

    [Test]
    public void Test_TraceData_WithArray ()
    {
      Exception exception = new Exception ("An exception.");

      Object[] data = new object[] { exception, "The message." };

      _listener.TraceData (null, "Test", TraceEventType.Information, 1, data);

      LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.That (events.Length, Is.EqualTo (1));
      Assert.That (events[0].Level, Is.EqualTo (Level.Info));
      Assert.That (events[0].MessageObject.ToString (), Is.EqualTo (data[0] + ", " + data[1]));
    }

    [Test]
    public void Test_TraceData_WithArrayAndFilterReturnsTrue ()
    {
      Exception exception = new Exception ("An exception.");
      Object[] data = new object[] { exception, "The message." };

      Expect
          .Call (_mockFilter.ShouldTrace (_traceEventCache, "Test", TraceEventType.Information, 1, null, null, null, data))
          .Return (true);
      _mocks.ReplayAll();

      _filterListener.TraceData (_traceEventCache, "Test", TraceEventType.Information, 1, data);

      _mocks.VerifyAll ();
      LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.That (events.Length, Is.EqualTo (1));
      Assert.That (events[0].Level, Is.EqualTo (Level.Info));
      Assert.That (events[0].MessageObject.ToString (), Is.EqualTo (data[0] + ", " + data[1]));
    }

    [Test]
    public void Test_TraceData_WithArrayAndFilterReturnsFalse ()
    {
      Exception exception = new Exception ("An exception.");
      Object[] data = new object[] { exception, "The message." };

      Expect
          .Call (_mockFilter.ShouldTrace (_traceEventCache, "Test", TraceEventType.Information, 1, null, null, null, data))
          .Return (false);
      _mocks.ReplayAll ();

      _filterListener.TraceData (_traceEventCache, "Test", TraceEventType.Information, 1, data);

      _mocks.VerifyAll ();
      Assert.That (_memoryAppender.GetEvents (), Is.Empty);
    }

    [Test]
    public void Test_TraceTransfer ()
    {
      Guid relatedActivityId = new Guid ();

      _listener.TraceTransfer (null, "Test", 1, "The message.", relatedActivityId);

      LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.That (events.Length, Is.EqualTo (1));
      Assert.That (events[0].Level, Is.EqualTo (Level.Info));
      string expected = "The message., relatedActivityId=" + relatedActivityId;
      Assert.That (events[0].MessageObject.ToString (), Is.EqualTo (expected));
    }


    [Test]
    public void Test_TraceTransfer_WithMessageNull ()
    {
      Guid relatedActivityId = new Guid ();

      _listener.TraceTransfer (null, "Test", 1, null, relatedActivityId);

      LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.That (events.Length, Is.EqualTo (1));
      Assert.That (events[0].Level, Is.EqualTo (Level.Info));
      string expected = ", relatedActivityId=" + relatedActivityId;
      Assert.That (events[0].MessageObject.ToString (), Is.EqualTo (expected));
    }


    [Test]
    public void Test_ConvertVerbose ()
    {
      Assert.That (Log4NetTraceListener.Convert (TraceEventType.Verbose), Is.EqualTo (LogLevel.Debug));
    }

    [Test]
    public void Test_ConvertInformation ()
    {
      Assert.That (Log4NetTraceListener.Convert (TraceEventType.Information), Is.EqualTo (LogLevel.Info));
    }

    [Test]
    public void Test_ConvertWarning ()
    {
      Assert.That (Log4NetTraceListener.Convert (TraceEventType.Warning), Is.EqualTo (LogLevel.Warn));
    }

    [Test]
    public void Test_ConvertError ()
    {
      Assert.That (Log4NetTraceListener.Convert (TraceEventType.Error), Is.EqualTo (LogLevel.Error));
    }

    [Test]
    public void Test_ConvertCritical ()
    {
      Assert.That (Log4NetTraceListener.Convert (TraceEventType.Critical), Is.EqualTo (LogLevel.Fatal));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "LogLevel does not support value 10000.\r\nParameter name: logLevel")]
    public void Test_ConvertInvalid ()
    {
      Log4NetTraceListener.Convert ((TraceEventType) 10000);
    }
  }
}
