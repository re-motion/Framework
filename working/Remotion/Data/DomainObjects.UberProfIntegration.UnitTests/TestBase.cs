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
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UberProfIntegration.UnitTests
{
  public abstract class TestBase
  {
    private LinqToSqlAppenderProxy _appenderProxy;
    private MockableLinqToSqlAppender _mockableAppender;
    private LinqToSqlAppenderProxy _originalAppender;
    private DoubleCheckedLockingContainer<LinqToSqlAppenderProxy> _container;

    [SetUp]
    public virtual void SetUp ()
    {
      _appenderProxy = (LinqToSqlAppenderProxy) PrivateInvoke.CreateInstanceNonPublicCtor (
          typeof (LinqToSqlAppenderProxy),
          "Test",
          typeof (FakeLinqToSqlProfiler),
          typeof (MockableLinqToSqlAppender));
      _mockableAppender = (MockableLinqToSqlAppender) _appenderProxy.LinqToSqlAppender;

      _container = (DoubleCheckedLockingContainer<LinqToSqlAppenderProxy>)
                   PrivateInvoke.GetNonPublicStaticField (typeof (LinqToSqlAppenderProxy), "s_instance");
      Assertion.IsNotNull (_container);

      if (_container.HasValue)
        _originalAppender = _container.Value;
      else
        _originalAppender = null;

      _container.Value = _appenderProxy;
    }

    [TearDown]
    public virtual void TearDown()
    {
      _container.Value = _originalAppender;
    }

    public LinqToSqlAppenderProxy AppenderProxy
    {
      get { return _appenderProxy; }
    }

    protected void SetAppender (MockableLinqToSqlAppender.ILinqToSqlAppender appender)
    {
      _mockableAppender.AppenderMock = appender;
    }
  }
}