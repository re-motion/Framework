// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
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