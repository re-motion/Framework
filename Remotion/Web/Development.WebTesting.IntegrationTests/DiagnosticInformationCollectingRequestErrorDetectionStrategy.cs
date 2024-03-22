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
using Coypu;
using Remotion.Web.Development.WebTesting.RequestErrorDetectionStrategies;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  /// <summary>
  /// Testclass to be able to seperate Tests for <see cref="IRequestErrorDetectionStrategy"/> implementation and calls.
  /// </summary>
  public class DiagnosticInformationCollectingRequestErrorDetectionStrategy : IRequestErrorDetectionStrategy
  {
    private class RequestErrorDetectionStrategyScope : IDisposable
    {
      private readonly DiagnosticInformationCollectingRequestErrorDetectionStrategy _parent;
      private readonly IRequestErrorDetectionStrategy _previousRequestErrorDetectionStrategy;

      public RequestErrorDetectionStrategyScope (
          DiagnosticInformationCollectingRequestErrorDetectionStrategy parent,
          IRequestErrorDetectionStrategy previousRequestErrorDetectionStrategy)
      {
        _parent = parent;
        _previousRequestErrorDetectionStrategy = previousRequestErrorDetectionStrategy;
      }

      public void Dispose ()
      {
        _parent._requestErrorDetectionStrategy = _previousRequestErrorDetectionStrategy;
      }
    }

    private IRequestErrorDetectionStrategy _requestErrorDetectionStrategy;
    private int _callCounter;
    private ElementScope _lastPassedScope;

    public DiagnosticInformationCollectingRequestErrorDetectionStrategy ()
    {
      _requestErrorDetectionStrategy = new NullRequestErrorDetectionStrategy();
    }

    public IDisposable CreateAspNetRequestErrorDetectionStrategyScope ()
    {
      var formerRequestErrorDetectionStrategy = _requestErrorDetectionStrategy;
      _requestErrorDetectionStrategy = new AspNetRequestErrorDetectionStrategy();
      return new RequestErrorDetectionStrategyScope(this, formerRequestErrorDetectionStrategy);
    }

    public void CheckPageForError (ElementScope scope)
    {
      _callCounter++;
      _lastPassedScope = scope;

      //As this class is configured for all Integration Tests,
      //we want to call the asp net request error detection strategy so we can detect yellow pages
      _requestErrorDetectionStrategy.CheckPageForError(scope);
    }

    public int GetCallCounter ()
    {
      return _callCounter;
    }

    public ElementScope GetLastPassedScope ()
    {
      return _lastPassedScope;
    }
  }
}
