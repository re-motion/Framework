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
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.ExecutionEngine.CompletionDetectionStrategies
{
  /// <summary>
  /// Blocks until the WXE post back sequence number (for the given <see cref="PageObjectContext"/>) has increased by the given amount.
  /// </summary>
  public class WxePostBackInCompletionDetectionStrategy : ICompletionDetectionStrategy
  {
    private readonly PageObjectContext _context;
    private readonly int _expectedWxePostBackSequenceNumberIncrease;
    private readonly TimeSpan? _timeout;

    public WxePostBackInCompletionDetectionStrategy ([NotNull] PageObjectContext context, int expectedWxePostBackSequenceNumberIncrease, TimeSpan? timeout = null)
    {
      ArgumentUtility.CheckNotNull("context", context);

      _context = context;
      _expectedWxePostBackSequenceNumberIncrease = expectedWxePostBackSequenceNumberIncrease;
      _timeout = timeout;
    }

    /// <inheritdoc/>
    public object? PrepareWaitForCompletion (PageObjectContext context, ILogger logger)
    {
      ArgumentUtility.CheckNotNull("context", context);
      ArgumentUtility.CheckNotNull("logger", logger);

      return WxeCompletionDetectionHelpers.GetWxePostBackSequenceNumber(_context);
    }

    /// <inheritdoc/>
    public void WaitForCompletion (PageObjectContext context, object? state, ILogger logger)
    {
      ArgumentUtility.CheckNotNull("context", context);
      ArgumentUtility.CheckNotNull("state", state!);
      ArgumentUtility.CheckNotNull("logger", logger);

      var oldWxePostBackSequenceNumber = (int)state;

      WxeCompletionDetectionHelpers.WaitForExpectedWxePostBackSequenceNumber(
          logger,
          _context,
          oldWxePostBackSequenceNumber,
          _expectedWxePostBackSequenceNumberIncrease,
          _timeout);
    }
  }
}
