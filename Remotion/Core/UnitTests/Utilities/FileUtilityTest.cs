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
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public partial class FileUtilityTest
  {
    [Test]
    [Obsolete]
    public void CopyStream ()
    {
      byte[] buffer = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
      MemoryStream outputStream = new MemoryStream();

      FileUtility.CopyStream (new MemoryStream (buffer), outputStream);
      Assert.That (outputStream.ToArray(), Is.EqualTo (new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }));
    }

    [Test]
    public void WriteEmbeddedStringResourceToFileTest ()
    {
      string testFileName = "FileUtilityTest_WriteEmbeddedStringResourceToFileTest.txt";
      try
      {
        FileUtility.WriteEmbeddedStringResourceToFile (GetType(), "TestData.WriteEmbeddedStringResourceToFileTestData.txt", testFileName);
        Assert.That (File.Exists (testFileName));
        string result = File.ReadAllText (testFileName);
        Assert.That (result, Is.EqualTo ("Hat der alte Hexenmeister sich doch einmal fortbegeben und nun sollen seine Geister..."));
      }
      finally
      {
        if (File.Exists (testFileName))
          File.Delete (testFileName);
      }
    }
  }
}