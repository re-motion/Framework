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
using System.Text;
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting.Data.SqlClient
{
  public class BatchCommand
  {
    private readonly int _startRowNumber;
    private int _lineCount;
    private readonly StringBuilder _content;

    public BatchCommand (int startRowNumber, int maxBatchLength)
    {
      _startRowNumber = startRowNumber;
      _content = new StringBuilder(maxBatchLength);
      _lineCount = 0;
    }

    public int StartRowNumber
    {
      get { return _startRowNumber; }
    }

    public int EndRowNumber
    {
      get { return _lineCount == 0 ? 0 : _startRowNumber + _lineCount - 1; }
    }

    public string Content
    {
      get
      {
        var batch = _content.ToString().Trim();
        return string.IsNullOrEmpty (batch) ? null : batch;
      }
    }

    public void AppendCommandBatchLine (string commandBatchLine)
    {
      ArgumentUtility.CheckNotNull ("commandBatchLine", commandBatchLine);
      
      _content.AppendLine (commandBatchLine);
      _lineCount++;
    }
  }
}