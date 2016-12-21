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

namespace Remotion.Web.ExecutionEngine.CodeGenerator
{
  public enum InputError
  {
    InvalidSchema = 1,
    ClassNotFound = 2,
    XmlError = 3,
    Unknown = 4
  }
  
  public class InputException : Exception
  {
    private string _path;
    private int _line;
    private int _position;
    private int _errorCode;

    public InputException (InputError error, string path, int line, int position, Exception innerException)
      : base (innerException.Message, innerException)
    {
      _path = path;
      _line = line;
      _position = position;
      _errorCode = (int) error;
    }

    public string Path
    {
      get { return _path; }
    }

    public int Line
    {
      get { return _line; }
    }

    public int Position
    {
      get { return _position; }
    }

    public int ErrorCode
    {
      get { return _errorCode; }
    }
  }
}
