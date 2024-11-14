// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.IO;
using Remotion.Utilities;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.Development.UnitTesting.IO
{
  /// <summary>
  /// The <see cref="TempFile"/> class represents a disposable temp file created via the <see cref="Path.GetTempFileName"/> method.
  /// </summary>
  partial class TempFile : IDisposable
  {
    // TODO RM-7765: check if the instance is disposed before accessing _fileName.

    private string? _fileName;

    public TempFile ()
    {
      _fileName = Path.GetTempFileName();
    }

    public void Dispose ()
    {
      if (_fileName != null && File.Exists(_fileName))
      {
        File.Delete(_fileName);
        _fileName = null;
      }
    }

    public string FileName
    {
      get
      {
        if (_fileName == null)
          throw new InvalidOperationException("Object disposed.");
        return _fileName;
      }
    }

    public void WriteStream (Stream stream)
    {
      ArgumentUtility.CheckNotNull("stream", stream);

      using (StreamReader streamReader = new StreamReader(stream))
      {
        using (StreamWriter streamWriter = new StreamWriter(_fileName!))
        {
          while (!streamReader.EndOfStream)
          {
            char[] buffer = new char[100];
            streamWriter.Write(buffer, 0, streamReader.Read(buffer, 0, buffer.Length));
          }
        }
      }
    }

    public void WriteAllBytes (byte[] bytes)
    {
      ArgumentUtility.CheckNotNull("bytes", bytes);

      File.WriteAllBytes(_fileName!, bytes);
    }

    public void WriteAllText (string text)
    {
      ArgumentUtility.CheckNotNull("text", text);

      File.WriteAllText(_fileName!, text);
    }

    public long Length
    {
      get
      {
        var fileInfo = new FileInfo(_fileName!);
        return fileInfo.Length;
      }
    }
  }
}
