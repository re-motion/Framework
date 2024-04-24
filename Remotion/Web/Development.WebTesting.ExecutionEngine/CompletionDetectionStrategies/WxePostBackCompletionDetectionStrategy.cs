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
using Microsoft.Extensions.Logging;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.ExecutionEngine.CompletionDetectionStrategies
{
  /// <summary>
  /// Blocks until the WXE postback sequence number (for the current <see cref="PageObjectContext"/>) has increased by the given amount.
  /// </summary>
  public class WxePostBackCompletionDetectionStrategy : ICompletionDetectionStrategy
  {
    private static readonly ILogger s_logger = LogManager.GetLogger<WxePostBackCompletionDetectionStrategy>();
    private readonly int _expectedWxePostBackSequenceNumberIncrease;
    private readonly TimeSpan? _timeout;

    public WxePostBackCompletionDetectionStrategy (int expectedWxePostBackSequenceNumberIncrease, TimeSpan? timeout = null)
    {
      _expectedWxePostBackSequenceNumberIncrease = expectedWxePostBackSequenceNumberIncrease;
      _timeout = timeout;
    }

    /// <inheritdoc/>
    public object? PrepareWaitForCompletion (PageObjectContext context)
    {
      ArgumentUtility.CheckNotNull("context", context);

      return WxeCompletionDetectionHelpers.GetWxePostBackSequenceNumber(context);
    }

    /// <inheritdoc/>
    public void WaitForCompletion (PageObjectContext context, object? state)
    {
      ArgumentUtility.CheckNotNull("context", context);
      ArgumentUtility.CheckNotNull("state", state!);

      var oldWxePostBackSequenceNumber = (int)state;

      WxeCompletionDetectionHelpers.WaitForExpectedWxePostBackSequenceNumber(
          s_logger,
          context,
          oldWxePostBackSequenceNumber,
          _expectedWxePostBackSequenceNumberIncrease,
          _timeout);
    }
  }
}
