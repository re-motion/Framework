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
using System.IO;
using Remotion.Utilities;

// ReSharper disable once CheckNamespace
namespace Remotion.Development.UnitTesting.IO
{
  /// <summary>
  /// The <see cref="TempFile"/> class represents a disposable temp file created via the <see cref="Path.GetTempFileName"/> method.
  /// </summary>
  partial class TempFile : IDisposable
  {
    private string _fileName;

    public TempFile ()
    {
      _fileName = Path.GetTempFileName();
    }

    public void Dispose ()
    {
      if (_fileName != null && File.Exists (_fileName))
      {
        File.Delete (_fileName);
        _fileName = null;
      }
    }

    public string FileName
    {
      get
      {
        if (_fileName == null)
          throw new InvalidOperationException ("Object disposed.");
        return _fileName;
      }
    }

    public void WriteStream (Stream stream)
    {
      ArgumentUtility.CheckNotNull ("stream", stream);

      using (StreamReader streamReader = new StreamReader (stream))
      {
        using (StreamWriter streamWriter = new StreamWriter (_fileName))
        {
          while (!streamReader.EndOfStream)
          {
            char[] buffer = new char[100];
            streamWriter.Write (buffer, 0, streamReader.Read (buffer, 0, buffer.Length));
          }
        }
      }
    }

    public void WriteAllBytes (byte[] bytes)
    {
      ArgumentUtility.CheckNotNull ("bytes", bytes);

      File.WriteAllBytes (_fileName, bytes);
    }

    public void WriteAllText (string text)
    {
      ArgumentUtility.CheckNotNull ("text", text);

      File.WriteAllText (_fileName, text);
    }

    public long Length
    {
      get
      {
        var fileInfo = new FileInfo (_fileName);
        return fileInfo.Length;
      }
    }
  }
}