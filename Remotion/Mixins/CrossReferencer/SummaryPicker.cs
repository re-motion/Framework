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
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using MixinXRef.Utility;

namespace MixinXRef
{
  public class SummaryPicker
  {
    private static readonly XElement s_noSummary = new XElement ("summary", "No summary found.");
    private static readonly Regex s_normalizeTrim = new Regex (@"\s+", RegexOptions.Compiled);

    public XElement GetSummary (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      // get path and filename of xml summary
      var documentationFileName = Path.ChangeExtension (type.Assembly.Location, ".xml");

      
      // check if xml document exists
      if (!File.Exists (documentationFileName))
        return s_noSummary;

      // open docu
      var xmlDocument = XDocument.Load (documentationFileName);

      // search for member
      var searchName = "T:" + type.FullName;
      var xpath = string.Format ("//member[@name = '{0}']/summary", searchName);
      var summary = xmlDocument.XPathSelectElement (xpath);

      // xpath expression returned no result
      if (summary == null)
        return s_noSummary;

      // normalize and trim summary content
      return NormalizeAndTrim (summary);
    }

    public XElement NormalizeAndTrim (XElement element)
    {
      ArgumentUtility.CheckNotNull ("element", element);

      var normalizedElement = s_normalizeTrim.Replace (element.ToString(), " ").Replace (" <", "<").Replace ("> ", ">");

      return XElement.Parse (normalizedElement);
    }
  }
}