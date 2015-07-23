// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.IO;
using NUnit.Framework;
using Remotion.SecurityManager.AclTools.Expansion.TextWriterFactory;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion.TextWriterFactory
{
  [TestFixture]
  public class StreamWriterFactoryTest
  {
    [Test]
    public void NewTextWriterArgumentTest ()
    {
      var streamWriterFactory = new StreamWriterFactory ();
      string directory = Path.Combine (Path.GetTempPath (), "StreamWriterFactoryTest_DirectoryTest");
      const string extension = "xyz";
      const string fileName = "someFile";
      using (StreamWriter streamWriter = (StreamWriter) streamWriterFactory.CreateTextWriter (directory, fileName, extension))
      {
        var fileStream = (FileStream) streamWriter.BaseStream;
        string filePathExpected = Path.Combine (directory, fileName + "." + extension);
        Assert.That (fileStream.Name, Is.EqualTo (filePathExpected));
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Directory must not be null. Set using \"Directory\"-property before calling \"CreateTextWriter\"")]
    public void NewTextWriterWithNullDirectoryThrowsTest ()
    {
      var streamWriterFactory = new StreamWriterFactory ();
      streamWriterFactory.Directory = null;
      streamWriterFactory.CreateTextWriter ("whatever");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = @"TextWriter with name ""abc"" already exists.")]
    public void NewTextWriterNameAlreadyExistsTest ()
    {
      const string textWriterName = "abc";
      var streamWriterFactory = new StreamWriterFactory ();
      streamWriterFactory.Directory = "xyz";
      streamWriterFactory.CreateTextWriter (textWriterName);
      streamWriterFactory.CreateTextWriter (textWriterName);
    }


    [Test]
    public void NewTextWriterOnlyNameArgumentTest ()
    {
      var mocks = new MockRepository();
      var streamWriterFactoryMock = mocks.PartialMock<StreamWriterFactory> ();
      const string directory = "the\\dir\\ect\\ory";
      streamWriterFactoryMock.Directory = directory;
      const string extension = "xyz";
      streamWriterFactoryMock.Extension = extension;
      const string fileName = "someFile";

      streamWriterFactoryMock.Expect (x => x.CreateTextWriter (directory, fileName, extension)).Return(TextWriter.Null);
      
      streamWriterFactoryMock.Replay();

      streamWriterFactoryMock.CreateTextWriter (fileName);

      streamWriterFactoryMock.VerifyAllExpectations ();
    }
  }
}
