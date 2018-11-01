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

namespace Remotion.UnitTests.Logging.Log4NetLogTests
{
  [TestFixture]
  public class ConvertTest
  {
    [Test]
    public void Test_Info ()
    {
      Assert.That (Log4NetLog.Convert (LogLevel.Info), Is.EqualTo (Level.Info));
    }

    [Test]
    public void Test_Debug ()
    {
      Assert.That (Log4NetLog.Convert (LogLevel.Debug), Is.EqualTo (Level.Debug));
    }

    [Test]
    public void Test_Warn ()
    {
      Assert.That (Log4NetLog.Convert (LogLevel.Warn), Is.EqualTo (Level.Warn));
    }

    [Test]
    public void Test_Error ()
    {
      Assert.That (Log4NetLog.Convert (LogLevel.Error), Is.EqualTo (Level.Error));
    }

    [Test]
    public void Test_Fatal ()
    {
      Assert.That (Log4NetLog.Convert (LogLevel.Fatal), Is.EqualTo (Level.Fatal));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "LogLevel does not support value 10.\r\nParameter name: logLevel")]
    public void Test_InvalidLevel ()
    {
      Log4NetLog.Convert ((LogLevel) 10);
    }
  }
}
