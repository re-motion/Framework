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
using log4net.Appender;
using log4net.Core;
using log4net.Repository;
using log4net.Repository.Hierarchy;
using NUnit.Framework;
using Remotion.Logging;

namespace Remotion.UnitTests.Logging.LogExtensionsTests
{
  public class BaseTest
  {
    private ILogger _logger;
    private ILog _log;
    private MemoryAppender _memoryAppender;

    [SetUp]
    public virtual void SetUp ()
    {
      _memoryAppender = new MemoryAppender ();
      var hierarchy = new Hierarchy();
      ((IBasicRepositoryConfigurator) hierarchy).Configure (_memoryAppender);
      _logger = hierarchy.GetLogger ("The Name");
      _log = new Log4NetLog (_logger);
    }

    protected ILog Log
    {
      get { return _log; }
    }

    protected ILogger Logger
    {
      get { return _logger; }
    }

    protected LoggingEvent[] GetLoggingEvents ()
    {
      return _memoryAppender.GetEvents ();
    }

    protected void SetLoggingThreshold (Level threshold)
    {
      _logger.Repository.Threshold = threshold;
    }
  }
}
