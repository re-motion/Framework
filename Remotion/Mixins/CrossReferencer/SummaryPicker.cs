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
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using Remotion.Utilities;

namespace Remotion.Mixins.CrossReferencer
{
  public class SummaryPicker
  {
    private static readonly XElement s_noSummary = new("summary", "No summary found.");
    private static readonly Regex s_normalizeTrim = new(@"\s+", RegexOptions.Compiled);

    public XElement GetSummary (Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);

      // get path and filename of xml summary
      var documentationFileName = Path.ChangeExtension(type.Assembly.Location, ".xml");


      // check if xml document exists
      if (!File.Exists(documentationFileName))
        return s_noSummary;

      // open docu
      var xmlDocument = XDocument.Load(documentationFileName);

      // search for member
      var searchName = "T:" + type.FullName;
      var xpath = string.Format("//member[@name = '{0}']/summary", searchName);
      var summary = xmlDocument.XPathSelectElement(xpath);

      // xpath expression returned no result
      if (summary == null)
        return s_noSummary;

      // normalize and trim summary content
      return NormalizeAndTrim(summary);
    }

    public XElement NormalizeAndTrim (XElement element)
    {
      ArgumentUtility.CheckNotNull("element", element);

      var normalizedElement = s_normalizeTrim.Replace(element.ToString(), " ").Replace(" <", "<").Replace("> ", ">");

      return XElement.Parse(normalizedElement);
    }
  }
}
