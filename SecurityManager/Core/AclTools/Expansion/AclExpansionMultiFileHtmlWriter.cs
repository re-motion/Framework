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
using System.Collections.Generic;
using System.Linq;
using Remotion.SecurityManager.AclTools.Expansion.Infrastructure;
using Remotion.SecurityManager.AclTools.Expansion.TextWriterFactory;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.Globalization.AclTools.Expansion;
using Remotion.Utilities;


namespace Remotion.SecurityManager.AclTools.Expansion
{
  /// <summary>
  /// <see cref="IAclExpansionWriter"/> which outputs a <see cref="List{T}"/> of <see cref="AclExpansionEntry"/> as a master HTML table consisting
  /// of users linking to detail HTML tables conaining the access rights of the respective user. All HTML files are written
  /// into an automatically generated directory.
  /// </summary>
  public class AclExpansionMultiFileHtmlWriter : IAclExpansionWriter
  {
    public const string MasterFileName = "_AclExpansionMain_";

    private readonly ITextWriterFactory _textWriterFactory;
    private readonly bool _indentXml;
    private AclExpansionHtmlWriterSettings _detailHtmlWriterSettings = new AclExpansionHtmlWriterSettings();

    public AclExpansionMultiFileHtmlWriter (ITextWriterFactory textWriterFactory, bool indentXml)
    {
      _textWriterFactory = textWriterFactory;
      _indentXml = indentXml;
    }


    public AclExpansionHtmlWriterSettings DetailHtmlWriterSettings
    {
      get { return _detailHtmlWriterSettings; }
      set { _detailHtmlWriterSettings = value; }
    }


    public void WriteAclExpansion (List<AclExpansionEntry> aclExpansion)
    {
      ArgumentUtility.CheckNotNull("aclExpansion", aclExpansion);
      using (var textWriter = _textWriterFactory.CreateTextWriter(MasterFileName))
      {
        var writerImplementation = new AclExpansionHtmlWriterImplementationBase(textWriter, _indentXml);

        writerImplementation.WritePageStart(AclToolsExpansion.PageTitle);
        writerImplementation.WriteTableStart("remotion-user-table");
        WriteTableHeaders(writerImplementation);
        WriteTableBody(writerImplementation, aclExpansion);
        writerImplementation.WriteTableEnd();
        writerImplementation.WritePageEnd();
      }
    }

    private void WriteTableHeaders (AclExpansionHtmlWriterImplementationBase writerImplementation)
    {
      writerImplementation.HtmlTagWriter.Tags.tr();
      writerImplementation.WriteHeaderCell(AclToolsExpansion.UserTableHeader);
      writerImplementation.WriteHeaderCell(AclToolsExpansion.FirstNameTableHeader);
      writerImplementation.WriteHeaderCell(AclToolsExpansion.LastNameTableHeader);
      writerImplementation.WriteHeaderCell(AclToolsExpansion.AccessRightsNameTableHeader);
      writerImplementation.HtmlTagWriter.Tags.trEnd();
    }




    private void WriteTableBody (AclExpansionHtmlWriterImplementationBase writerImplementation, List<AclExpansionEntry> aclExpansion)
    {
      var users = GetUsers(aclExpansion);

      foreach (var user in users)
      {
        // Note: Due to HTML-table-cells using rowspan attribute it is not safe to assume that we are already in a table row here
        // (i.e. that a <tr>-tag has already been written).
        writerImplementation.WriteTableRowBeginIfNotInTableRow();
        WriteTableBody_ProcessUser(writerImplementation, user, aclExpansion);
        writerImplementation.WriteTableRowEnd();
      }
    }

    // Note: Method name has been picked in analogy to method names in AclExpansionHtmlWriter.
    private void WriteTableBody_ProcessUser (AclExpansionHtmlWriterImplementationBase writerImplementation, User user, List<AclExpansionEntry> aclExpansion)
    {
      writerImplementation.WriteTableData(user.UserName);
      writerImplementation.WriteTableData(user.FirstName);
      writerImplementation.WriteTableData(user.LastName);

      string userDetailFileName = AclExpansionHtmlWriterImplementationBase.ToValidFileName(user.UserName);
      using (var detailTextWriter = _textWriterFactory.CreateTextWriter(userDetailFileName))
      {

        var aclExpansionSingleUser = GetAccessControlEntriesForUser(aclExpansion, user);
        var detailAclExpansionHtmlWriter = new AclExpansionHtmlWriter(detailTextWriter, false, _detailHtmlWriterSettings);
        detailAclExpansionHtmlWriter.WriteAclExpansion(aclExpansionSingleUser);
      }

      string relativePath = _textWriterFactory.GetRelativePath(MasterFileName, userDetailFileName);
      writerImplementation.WriteTableRowBeginIfNotInTableRow();
      writerImplementation.HtmlTagWriter.Tags.td();
      writerImplementation.HtmlTagWriter.Tags.a();
      writerImplementation.HtmlTagWriter.Attribute("href", relativePath);
      writerImplementation.HtmlTagWriter.Attribute("target", "_blank");
      writerImplementation.HtmlTagWriter.Value(relativePath);
      writerImplementation.HtmlTagWriter.Tags.aEnd();
      writerImplementation.HtmlTagWriter.Tags.tdEnd();
    }


    public static List<User> GetUsers (IEnumerable<AclExpansionEntry> aclExpansion)
    {
      return (from aee in aclExpansion
             let user = aee.User
             orderby user.LastName, user.FirstName, user.UserName
              select user).Distinct().ToList();
    }

    public static List<AclExpansionEntry> GetAccessControlEntriesForUser (IEnumerable<AclExpansionEntry> aclExpansion, User user)
    {
      return (from aee in aclExpansion
             where aee.User == user
             select aee).ToList();
    }
  }
}
