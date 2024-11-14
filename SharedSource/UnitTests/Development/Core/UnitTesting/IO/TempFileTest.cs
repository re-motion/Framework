// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using Remotion.Development.UnitTesting.IO;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.Development.UnitTests.Core.UnitTesting.IO
{
  [TestFixture]
  public class TempFileTest
  {
    [Test]
    public void Initialize ()
    {
      using (TempFile tempFile = new TempFile())
      {
        Assert.That(tempFile.FileName, Is.Not.Empty);
        Assert.That(File.Exists(tempFile.FileName), Is.True);
      }
    }

    [Test]
    public void Dispose ()
    {
      TempFile tempFile = new TempFile();
      string fileName = tempFile.FileName;
      tempFile.Dispose();
      Assert.That(File.Exists(fileName), Is.False);
    }

    [Test]
    public void GetFileNameAfterDispose_Throws ()
    {
      TempFile tempFile = new TempFile();
      tempFile.Dispose();
      Assert.That(
          () => tempFile.FileName,
          Throws.InvalidOperationException
              .With.Message.EqualTo("Object disposed."));
    }

    [Test]
    public void WriteStream ()
    {
      using (TempFile tempFile = new TempFile())
      {
        string expectedText = "Some\r\nText";
        byte[] buffer = Encoding.ASCII.GetBytes(expectedText);
        using (Stream stream = new MemoryStream(buffer.Length))
        {
          stream.Write(buffer, 0, buffer.Length);
          stream.Position = 0;
          tempFile.WriteStream(stream);
        }

        string actual = File.ReadAllText(tempFile.FileName);
        Assert.That(actual, Is.EqualTo(expectedText));
      }
    }

    [Test]
    public void WriteStream_WithContentLongerThanBuffer ()
    {
      using (TempFile tempFile = new TempFile())
      {
        string expectedText = "0123456789\r\n0123456789\r\n0123456789\r\n0123456789\r\n0123456789\r\n"
                              + "0123456789\r\n0123456789\r\n0123456789\r\n0123456789\r\n0123456789\r\n"
                              + "0123456789\r\n0123456789\r\n0123456789\r\n0123456789\r\n0123456789\r\n";

        byte[] buffer = Encoding.ASCII.GetBytes(expectedText);
        using (Stream stream = new MemoryStream(buffer.Length))
        {
          stream.Write(buffer, 0, buffer.Length);
          stream.Position = 0;
          tempFile.WriteStream(stream);
        }

        string actual = File.ReadAllText(tempFile.FileName);
        Assert.That(actual, Is.EqualTo(expectedText));
      }
    }

    [Test]
    public void WriteAllBytes ()
    {
      using (TempFile tempFile = new TempFile())
      {
        string expectedText = "Some\r\nText";
        tempFile.WriteAllBytes(Encoding.ASCII.GetBytes(expectedText));

        string actual = File.ReadAllText(tempFile.FileName);
        Assert.That(actual, Is.EqualTo(expectedText));
      }
    }

    [Test]
    public void WriteAllText ()
    {
      using (TempFile tempFile = new TempFile())
      {
        string expectedText = "Some\r\nText";
        tempFile.WriteAllText(expectedText);

        string actual = File.ReadAllText(tempFile.FileName);
        Assert.That(actual, Is.EqualTo(expectedText));
      }
    }
  }
}
