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
using Remotion.Development.UnitTesting;
using Remotion.Tools.Console.CommandLine;

namespace Remotion.Tools.UnitTests.Console.CommandLine
{

public enum IncrementalTestOptions { no, nor, normal, anything };
public enum TestOption { yes, no, almost };
public enum TestMode 
{
  [CommandLineMode ("m1", Description = "Primary mode")]
  Mode1, 
  [CommandLineMode ("m2", Description = "Secondary mode")]
  Mode2 
};


[TestFixture]
public class CommandLineParserTest
{
  private CommandLineParser CreateParser (
      out CommandLineStringArgument argSourceDir, 
      out CommandLineStringArgument argDestinationDir, 
      out CommandLineFlagArgument argCopyBinary,
      out CommandLineEnumArgument argEnumOption)
  {
    CommandLineParser parser = new CommandLineParser();

    argSourceDir = new CommandLineStringArgument (true);
    argSourceDir.Placeholder = "source-directory";
    argSourceDir.Description = "Directory to copy from";
    parser.Arguments.Add (argSourceDir);

    argDestinationDir = new CommandLineStringArgument (true);
    argDestinationDir.Placeholder = "destination-directory";
    argDestinationDir.Description = "This is the directory to copy to. This is the directory to copy to. This is the directory to copy to. This is the directory to copy to. This is the directory to copy to. This is the directory to copy to. This is the directory to copy to. This is the directory to copy to.";
    parser.Arguments.Add (argDestinationDir);

    argCopyBinary = new CommandLineFlagArgument ("b", true);
    argCopyBinary.Description = "binary copy on (+, default) or off (-)";
    parser.Arguments.Add (argCopyBinary);

    argEnumOption = new CommandLineEnumArgument ("rep", true, typeof (TestOption));
    argEnumOption.Description = "replace target";
    parser.Arguments.Add (argEnumOption);

    CommandLineModeArgument modeGroup = new CommandLineModeArgument (true, typeof (TestMode));
    foreach (CommandLineModeFlagArgument flag in modeGroup.Parts)
      parser.Arguments.Add (flag);
    parser.Arguments.Add (modeGroup);

    return parser;
  }

  private CommandLineParser CreateParser()
  {
    CommandLineStringArgument argSourceDir;
    CommandLineStringArgument argDestinationDir;
    CommandLineFlagArgument argCopyBinary;
    CommandLineEnumArgument argEnumOption;
    return CreateParser (out argSourceDir, out argDestinationDir, out argCopyBinary, out argEnumOption);
  }

  [Test]
  public void TestParsingSucceed ()
  {
    CommandLineStringArgument argSourceDir;
    CommandLineStringArgument argDestinationDir;
    CommandLineFlagArgument argCopyBinary;
    CommandLineEnumArgument argEnumOption;
    CommandLineParser parser = CreateParser (out argSourceDir, out argDestinationDir, out argCopyBinary, out argEnumOption);

    parser.Parse (new string[] {
        "source", 
        "dest", 
        "/B-",
        "/Re:y" });

    Assert.That (argSourceDir.Value, Is.EqualTo ("source"));
    Assert.That (argDestinationDir.Value, Is.EqualTo ("dest"));
    Assert.That (argCopyBinary.Value, Is.EqualTo (false));
    Assert.That (argEnumOption.HasValue, Is.EqualTo (true));
    Assert.That (argEnumOption.Value, Is.EqualTo (TestOption.yes));
  }

  [Test]
  public void TestParsingLeaveOutOptional ()
  {
    CommandLineStringArgument argSourceDir;
    CommandLineStringArgument argDestinationDir;
    CommandLineFlagArgument argCopyBinary;
    CommandLineEnumArgument argEnumOption;
    CommandLineParser parser = CreateParser (out argSourceDir, out argDestinationDir, out argCopyBinary, out argEnumOption);

    parser.Parse (new string[] {
        "source"} );

    Assert.That (argSourceDir.Value, Is.EqualTo ("source"));
    Assert.That (argDestinationDir.Value, Is.EqualTo (null));
    Assert.That (argCopyBinary.Value, Is.EqualTo (true));
    Assert.That (argEnumOption.HasValue, Is.EqualTo (false));
  }

  [Test]
  public void TestParsingLeaveOutRequired ()
  {
    CommandLineStringArgument argSourceDir;
    CommandLineStringArgument argDestinationDir;
    CommandLineFlagArgument argCopyBinary;
    CommandLineEnumArgument argEnumOption;
    CommandLineParser parser = CreateParser (out argSourceDir, out argDestinationDir, out argCopyBinary, out argEnumOption);
    argEnumOption.IsOptional = false;
    Assert.That (
        () => parser.Parse (new string[] {
        "source"} ),
        Throws.InstanceOf<MissingRequiredCommandLineParameterException>());
  }

  [Test]
  public void TestParsingCaseSensitiveFail ()
  {
    CommandLineParser parser = CreateParser ();
    parser.IsCaseSensitive = true;
    Assert.That (
        () => parser.Parse (new string[] {
        "source", 
        "dest", 
        "/B-",
        "/Re:y" }),
        Throws.InstanceOf<InvalidCommandLineArgumentNameException>());
  }

  [Test]
  public void TestParsingNotIncrementalFail ()
  {
    CommandLineParser parser = CreateParser ();
    parser.IncrementalNameValidation = false;
    Assert.That (
        () => parser.Parse (new string[] {
        "source", 
        "dest", 
        "/b-",
        "/re:y" }),
        Throws.InstanceOf<InvalidCommandLineArgumentNameException>());
  }

  [Test]
  public void TestParsingNotIncrementalSucceed ()
  {
    CommandLineStringArgument argSourceDir;
    CommandLineStringArgument argDestinationDir;
    CommandLineFlagArgument argCopyBinary;
    CommandLineEnumArgument argEnumOption;
    CommandLineParser parser = CreateParser (out argSourceDir, out argDestinationDir, out argCopyBinary, out argEnumOption);

    parser.Parse (new string[] {
        "source", 
        "dest", 
        "/B-",
        "/Rep:y" });

    Assert.That (argSourceDir.Value, Is.EqualTo ("source"));
    Assert.That (argDestinationDir.Value, Is.EqualTo ("dest"));
    Assert.That (argCopyBinary.Value, Is.EqualTo (false));
    Assert.That (argEnumOption.Value, Is.EqualTo (TestOption.yes));
  }

  [Test]
  public void TestParsingTooManyPositionalFail ()
  {
    CommandLineParser parser = CreateParser ();
    parser.IncrementalNameValidation = false;
    Assert.That (
        () => parser.Parse (new string[] {
        "source", 
        "dest", 
        "another"} ),
        Throws.InstanceOf<InvalidNumberOfCommandLineArgumentsException>());
  }

  [Test]
  public void TestSynopsis ()
  {
    CommandLineParser parser = CreateParser();
    string synopsis = parser.GetAsciiSynopsis ("app.exe", 80);
    
    string expectedResult = 
        "app.exe [source-directory [destination-directory]] [/b-] [/rep:{yes|no|almost}]" 
        + "\n[{/m1|/m2}]"
        + "\n"
        + "\n  source-directory       Directory to copy from" 
        + "\n  destination-directory  This is the directory to copy to. This is the directory" 
        + "\n                         to copy to. This is the directory to copy to. This is" 
        + "\n                         the directory to copy to. This is the directory to copy" 
        + "\n                         to. This is the directory to copy to. This is the" 
        + "\n                         directory to copy to. This is the directory to copy to." 
        + "\n  /b                     binary copy on (+, default) or off (-)" 
        + "\n  /rep                   replace target"
        + "\n  /m1                    Primary mode"
        + "\n  /m2                    Secondary mode";
    Assert.That (synopsis, Is.EqualTo (expectedResult));
  }

  [Test]
  public void TestEnumValues ()
  {
    CommandLineEnumArgument enumArg;
    
    enumArg = new CommandLineEnumArgument (false, typeof (TestOption));
    PrivateInvoke.InvokeNonPublicMethod (enumArg, "SetStringValue", "yes");
    Assert.That ((TestOption) enumArg.Value, Is.EqualTo (TestOption.yes));

    enumArg = new CommandLineEnumArgument (false, typeof (IncrementalTestOptions));
    PrivateInvoke.InvokeNonPublicMethod (enumArg, "SetStringValue", "no");
    Assert.That ((IncrementalTestOptions) enumArg.Value, Is.EqualTo (IncrementalTestOptions.no));
  }

  [Test]
  public void TestInt32Values ()
  {
    CommandLineInt32Argument intArg;

    intArg = new CommandLineInt32Argument (true);
    PrivateInvoke.InvokeNonPublicMethod (intArg, "SetStringValue", "32");
    Assert.That (intArg.Value, Is.EqualTo (32));

    intArg = new CommandLineInt32Argument (true);
    PrivateInvoke.InvokeNonPublicMethod (intArg, "SetStringValue", " ");
    Assert.That (intArg.Value, Is.EqualTo (null));
  }

  [Test]
  public void TestEnumAmbiguous ()
  {
    CommandLineEnumArgument enumArg = new CommandLineEnumArgument (false, typeof (IncrementalTestOptions));

    Assert.That (
        () => PrivateInvoke.InvokeNonPublicMethod (enumArg, "SetStringValue", "n"),
        Throws.InstanceOf<InvalidCommandLineArgumentValueException>()
            .With.Message.Contains("Ambiguous"));
    }

  [Test]
  public void TestEnumInvalid ()
  {
      CommandLineEnumArgument enumArg = new CommandLineEnumArgument (false, typeof (IncrementalTestOptions));

      Assert.That (
       () => PrivateInvoke.InvokeNonPublicMethod (enumArg, "SetStringValue", "invalidvalue"),
       Throws.InstanceOf<InvalidCommandLineArgumentValueException>()
           .With.Message.Contains("Use one of"));
  }
}

}
