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

namespace Remotion.Logging
{
  //TODO: #### use Write/WriteLine with Verbose/Debug?

  /// <summary>
  /// A <see cref="TraceListener"/> that directs tracing or debugging output to <b>log4net</b>.
  /// </summary>
  /// <remarks>See <see cref="TraceSource"/> for information on how to configure a <see cref="TraceListener"/>.</remarks>
  public class Log4NetTraceListener : TraceListener
  {
    // static members

    /// <summary>
    /// Converts <see cref="TraceEventType"/> to <see cref="LogLevel"/>.
    /// </summary>
    /// <include file='..\doc\include\Logging\Log4NetTraceListener.xml' path='Log4NetTraceListener/Convert/param[@name="eventType"]' />
    /// <returns>Corresponding <see cref="LogLevel"/> needed when logging through the <see cref="ILog"/> interface.</returns>
    public static LogLevel Convert (TraceEventType eventType)
    {
      switch (eventType)
      {
        case TraceEventType.Verbose:
          return LogLevel.Debug;
        case TraceEventType.Information:
          return LogLevel.Info;
        case TraceEventType.Warning:
          return LogLevel.Warn;
        case TraceEventType.Error:
          return LogLevel.Error;
        case TraceEventType.Critical:
          return LogLevel.Fatal;
        default:
          throw new ArgumentException (string.Format ("LogLevel does not support value {0}.", eventType), "logLevel");
      }
    }

    // member fields
    private Log4NetLogManager _logManager = new Log4NetLogManager ();

    // construction and disposing

    /// <summary>
    /// Initializes a new instance of the <see cref="Log4NetTraceListener"/> class. 
    /// </summary>
    public Log4NetTraceListener()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Log4NetTraceListener"/> class using the specified name. 
    /// </summary>
    /// <include file='..\doc\include\Logging\Log4NetTraceListener.xml' path='Log4NetTraceListener/Constructor/param[@name="name"]' />
    public Log4NetTraceListener (string name) : base (name)
    {
    }

    // methods and properties

    /*
     * TODO: Note in Docu that no filtering is done!

       TODO: #### if ((this.Filter == null) || this.Filter.ShouldTrace(null, "", TraceEventType.Verbose, 0, message))
     
      Write/WriteLine end up with EventLogEntryType.Information when using EventLogTraceListener.
     */
    /// <overloads>Writes a message to the <b>log4net</b> log. </overloads>
    /// <summary>
    /// Writes a message to the <b>log4net</b> log. 
    /// </summary>
    /// <include file='..\doc\include\Logging\Log4NetTraceListener.xml' path='Log4NetTraceListener/Trace/param[@name="message"]' />
    /// <remarks>
    /// <see cref="Write(string)"/> does not use <see cref="TraceFilter"/>, even though the inherited overloads
    /// use it; this follows the behavior of the standard <see cref="TraceListener"/> implementations like 
    /// <see cref="EventLogTraceListener"/> and <see cref="TextWriterTraceListener"/>; 
    /// this behavior is not explicitely documented for the classes of System.Diagnostics.
    /// </remarks>
    public override void Write (string message)
    {
      _logManager.GetLogger (string.Empty).Log (LogLevel.Debug, 0, message);
    }

    /// <overloads>Writes a message to the <b>log4net</b> log. </overloads>
    /// <summary>
    /// Writes a message to the <b>log4net</b> log. 
    /// </summary>
    /// <include file='..\doc\include\Logging\Log4NetTraceListener.xml' path='Log4NetTraceListener/Trace/param[@name="message"]' />
    /// <remarks>
    /// <see cref="WriteLine"/> has identical behavior to <see cref="Write(String)"/>.
    /// </remarks>
    public override void WriteLine (string message)
    {
      Write (message);
    }

    /// <overloads>Writes trace and event information to the <b>log4net</b> log.</overloads>
    /// <summary>
    /// Writes trace and event information to the <b>log4net</b> log.
    /// </summary>
    /// <include file='..\doc\include\Logging\Log4NetTraceListener.xml' 
    ///     path='Log4NetTraceListener/Trace/param[@name="eventCache" or @name="source" or @name="eventType" or @name="id"]' />
    /// <include file='..\doc\include\Logging\Log4NetTraceListener.xml' path='Log4NetTraceListener/Trace/remarks' />
    public override void TraceEvent (TraceEventCache eventCache, string source, TraceEventType eventType, int id)
    {
      TraceEvent (eventCache, source, eventType, id, string.Empty, new object[0]);
    }

    /// <summary>
    /// Writes trace information, a message, and event information to the <b>log4net</b> log.
    /// </summary>
    /// <include file='..\doc\include\Logging\Log4NetTraceListener.xml' 
    ///     path='Log4NetTraceListener/Trace/param[@name="eventCache" or @name="source" or @name="eventType" or @name="id" or @name="message"]' />
    /// <include file='..\doc\include\Logging\Log4NetTraceListener.xml' path='Log4NetTraceListener/Trace/remarks' />
    public override void TraceEvent (TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
    {
      TraceEvent (eventCache, source, eventType, id, message, new object[0]);
    }

    /// <summary>
    /// Writes trace information, a formatted array of objects and event information to the <b>log4net</b> log.
    /// </summary>
    /// <include file='..\doc\include\Logging\Log4NetTraceListener.xml' 
    ///     path='Log4NetTraceListener/Trace/param[@name="eventCache" or @name="source" or @name="eventType" or @name="id"]' />
    /// <include file='..\doc\include\Logging\Log4NetTraceListener.xml' path='Log4NetTraceListener/TraceEvent/param[@name="format" or @name="args"]' />
    /// <include file='..\doc\include\Logging\Log4NetTraceListener.xml' path='Log4NetTraceListener/Trace/remarks' />
    public override void TraceEvent (
        TraceEventCache eventCache, 
        string source, 
        TraceEventType eventType, int id, 
        string format, 
        params Object[] args)
    {
      if (ShouldTrace(eventCache, source, eventType, id, format, args, null, null))
        _logManager.GetLogger (source).LogFormat (Convert (eventType), id, format, args);
    }


    /// <overloads>Writes trace data to the <b>log4net</b> log. </overloads>
    /// <summary>
    /// Writes trace information, a data object and event information to the <b>log4net</b> log.
    /// </summary>
    /// <include file='..\doc\include\Logging\Log4NetTraceListener.xml' 
    ///     path='Log4NetTraceListener/Trace/param[@name="eventCache" or @name="source" or @name="eventType" or @name="id"]' />
    /// <include file='..\doc\include\Logging\Log4NetTraceListener.xml' path='Log4NetTraceListener/TraceData_Object/param[@name="data"]' />
    /// <include file='..\doc\include\Logging\Log4NetTraceListener.xml' path='Log4NetTraceListener/Trace/remarks' />
    public override void TraceData (TraceEventCache eventCache, string source, TraceEventType eventType, int id, Object data)
    {
      TraceData (eventCache, source, eventType, id, new object[] { data } );
    }

    /// <summary>
    /// Writes trace information, an array of data objects and event information to the <b>log4net</b> log.
    /// </summary>
    /// <include file='..\doc\include\Logging\Log4NetTraceListener.xml' 
    ///     path='Log4NetTraceListener/Trace/param[@name="eventCache" or @name="source" or @name="eventType" or @name="id"]' />
    /// <include file='..\doc\include\Logging\Log4NetTraceListener.xml' path='Log4NetTraceListener/TraceData_Array/param[@name="data"]' />
    /// <include file='..\doc\include\Logging\Log4NetTraceListener.xml' path='Log4NetTraceListener/Trace/remarks' />
    public override void TraceData (TraceEventCache eventCache, string source, TraceEventType eventType, int id, params Object[] data)
    {
      if (ShouldTrace (eventCache, source, eventType, id, null, null, null, data)) 
      {
        string message = string.Empty;
        if (data != null)
        {
          message = string.Join (", ", data);
        }

        _logManager.GetLogger (source).Log (Convert (eventType), id, message);
      }
    }


    /*
    * The TraceTransfer method is used for the correlation of related traces. 
    * The TraceTransfer method calls the TraceEvent method to process the call, 
    * with the eventType level set to Transfer and the relatedActivityIdGuid as 
    * a string appended to the message.
     * 
     * //## INTO REMARKS (or NOTE when multiple entries)
      TraceEventType.Transfer is used in TraceListener.TraceTransfer :

      Like EventLogTraceListener.CreateEventInstance: treat TraceEventType.Transfer as "Information" level.
    */
    /// <summary>
    /// Writes trace information, a message, a related activity identity and event information to the <b>log4net</b> log.
    /// </summary>
    /// <include file='..\doc\include\Logging\Log4NetTraceListener.xml' 
    ///     path='Log4NetTraceListener/Trace/param[@name="eventCache" or @name="source" or @name="id" or @name="message"]' />
    /// <include file='..\doc\include\Logging\Log4NetTraceListener.xml' path='Log4NetTraceListener/TraceTransfer/param[@name="relatedActivityId"]' />
    /// <include file='..\doc\include\Logging\Log4NetTraceListener.xml' path='Log4NetTraceListener/Trace/remarks' />
    public override void TraceTransfer (TraceEventCache eventCache, string source, int id, string message, Guid relatedActivityId) 
    {
      TraceEvent (eventCache, source, TraceEventType.Information, id,
          message + ", relatedActivityId=" + relatedActivityId, new object[0]); 
    } 


    private bool ShouldTrace (
        TraceEventCache cache,
        string source,
        TraceEventType eventType,
        int id,
        string formatOrMessage,
        object[] args,
        object data1,
        object[] data)
    {
      return ((Filter == null) || Filter.ShouldTrace (cache, source, eventType, id, formatOrMessage, args, data1, data));
    }
  }
}
