// This file is part of the MixinXRef project
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.XRef
{
  public class XRefTransformer
  {
    private readonly string _xmlInputFile;
    private readonly string _outputDirectory;

    public XRefTransformer (string xmlInputFile, string outputDirectory)
    {
      ArgumentUtility.CheckNotNull("xmlInputFile", xmlInputFile);
      ArgumentUtility.CheckNotNull("outputDirectory", outputDirectory);

      _xmlInputFile = xmlInputFile;
      _outputDirectory = outputDirectory;
    }

    public int GenerateHtmlFromXml ()
    {
      var xRefPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

      // stylesheet path
      var xsltStyleSheetPath = Path.Combine(xRefPath!, @"xml_utilities\main.xslt");

      // xslt processor path      
      var xsltProcessorPath = Path.Combine(xRefPath!, @"xml_utilities\saxon\Transform.exe");

      // dummy output file - will not be created, just to trick saxon
      var mainOutputFile = Path.Combine(_outputDirectory, "dummy.html");
      var arguments = String.Format("-s:\"{0}\" -xsl:\"{1}\" -o:\"{2}\"", _xmlInputFile, xsltStyleSheetPath, mainOutputFile);

      var xsltProcessor = new Process();
      xsltProcessor.StartInfo.FileName = xsltProcessorPath;
      xsltProcessor.StartInfo.Arguments = arguments;
      xsltProcessor.StartInfo.RedirectStandardError = true;
      xsltProcessor.StartInfo.RedirectStandardOutput = true;
      xsltProcessor.StartInfo.UseShellExecute = false;

      xsltProcessor.Start();
      // XRef.Log.SendInfo(xsltProcessor.StandardOutput.ReadToEnd().TrimEnd(Environment.NewLine.ToCharArray()));
      // XRef.Log.SendError(xsltProcessor.StandardError.ReadToEnd().TrimEnd(Environment.NewLine.ToCharArray()));
      xsltProcessor.WaitForExit();

      return xsltProcessor.ExitCode;
    }
  }
}
