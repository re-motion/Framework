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
using NUnit.Framework;
using Remotion.Tools.Console.CommandLine;

namespace Remotion.Tools.UnitTests.Console.CommandLine
{
  public class Arguments
  {
    [CommandLineStringArgument (true, Placeholder = "source-directory", Description = "Directory to copy from")]
    public string SourceDirectory;

    [CommandLineStringArgument (true, Placeholder = "destination-directory", Description = "This is the directory to copy to. This is the directory to copy to. This is the directory to copy to. This is the directory to copy to. This is the directory to copy to. This is the directory to copy to. This is the directory to copy to. This is the directory to copy to.")]
    public string DestinationDirectory;

    [CommandLineFlagArgument ("b", true, Description = "binary copy on (+, default) or off (-)")]
    public bool CopyBinary = true;

    [CommandLineEnumArgument ("rep", true)]
    public TestOption ReplaceTarget = TestOption.yes;

    [CommandLineModeArgument (true)]
    public TestMode Mode = TestMode.Mode1;
  }

  [TestFixture]
  public class CommandLineClassParserTest
  {
    [Test] 
    public void TestParser ()
    {
      CommandLineClassParser parser = new CommandLineClassParser (typeof (Arguments));
      Arguments arguments = (Arguments) parser.Parse ("sdir ddir /b- /rep:y", true);
      Assert.That (arguments.SourceDirectory, Is.EqualTo ("sdir"));
      Assert.That (arguments.DestinationDirectory, Is.EqualTo ("ddir"));
      Assert.That (arguments.CopyBinary, Is.EqualTo (false));
      Assert.That (arguments.ReplaceTarget, Is.EqualTo (TestOption.yes));
    }

    [Test] 
    public void TestModeArgDefault ()
    {
      CommandLineClassParser parser = new CommandLineClassParser (typeof (Arguments));
      Arguments arguments = (Arguments) parser.Parse ("", true);
      Assert.That (arguments.Mode, Is.EqualTo (TestMode.Mode1));
    }

    [Test] 
    public void TestModeArgMode2 ()
    {
      CommandLineClassParser parser = new CommandLineClassParser (typeof (Arguments));
      Arguments arguments = (Arguments) parser.Parse ("/m2", true);
      Assert.That (arguments.Mode, Is.EqualTo (TestMode.Mode2));
    }

    [Test] 
    [ExpectedException (typeof (ConflictCommandLineParameterException))]
    public void TestModeArgConfict ()
    {
      CommandLineClassParser parser = new CommandLineClassParser (typeof (Arguments));
      Arguments arguments = (Arguments) parser.Parse ("/m1 /m2", true);
    }

    [Test] 
    [ExpectedException (typeof (InvalidCommandLineArgumentValueException))]
    public void TestModeArgInvalidValue ()
    {
      CommandLineClassParser parser = new CommandLineClassParser (typeof (Arguments));
      Arguments arguments = (Arguments) parser.Parse ("/m1+", true);
    }

    [Test]
    [ExpectedException (typeof (InvalidCommandLineArgumentNameException))]
    public void TestFlagArgInvalidValue ()
    {
      CommandLineClassParser parser = new CommandLineClassParser (typeof (Arguments));
      Arguments arguments = (Arguments) parser.Parse ("/b~", true);
    }
    
    [Test] 
    public void TestOptional ()
    {
      CommandLineClassParser parser = new CommandLineClassParser (typeof (Arguments));
      Arguments arguments = (Arguments) parser.Parse ("", true);
      Assert.That (arguments.SourceDirectory, Is.EqualTo (null));
      Assert.That (arguments.DestinationDirectory, Is.EqualTo (null));
      Assert.That (arguments.CopyBinary, Is.EqualTo (true));
      Assert.That (arguments.ReplaceTarget, Is.EqualTo (TestOption.yes));
    }
  }
}
