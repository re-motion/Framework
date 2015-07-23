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
    private AclExpansionHtmlWriterImplementationBase _implementation;

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
      ArgumentUtility.CheckNotNull ("aclExpansion", aclExpansion);
      using (var textWriter = _textWriterFactory.CreateTextWriter (MasterFileName))
      {
        _implementation = new AclExpansionHtmlWriterImplementationBase (textWriter, _indentXml);

        _implementation.WritePageStart (AclToolsExpansion.PageTitle);
        _implementation.WriteTableStart ("remotion-user-table");
        WriteTableHeaders ();
        WriteTableBody (aclExpansion);
        _implementation.WriteTableEnd ();
        _implementation.WritePageEnd ();
      }
    }

    private void WriteTableHeaders ()
    {
      _implementation.HtmlTagWriter.Tags.tr ();
      _implementation.WriteHeaderCell (AclToolsExpansion.UserTableHeader);  
      _implementation.WriteHeaderCell (AclToolsExpansion.FirstNameTableHeader);
      _implementation.WriteHeaderCell (AclToolsExpansion.LastNameTableHeader);
      _implementation.WriteHeaderCell (AclToolsExpansion.AccessRightsNameTableHeader);
      _implementation.HtmlTagWriter.Tags.trEnd ();
    }




    private void WriteTableBody (List<AclExpansionEntry> aclExpansion)
    {
      var users = GetUsers (aclExpansion);

      foreach (var user in users)
      {
        // Note: Due to HTML-table-cells using rowspan attribute it is not safe to assume that we are already in a table row here
        // (i.e. that a <tr>-tag has already been written).
        _implementation.WriteTableRowBeginIfNotInTableRow (); 
        WriteTableBody_ProcessUser (user, aclExpansion);
        _implementation.WriteTableRowEnd ();
      }
    }

    // Note: Method name has been picked in analogy to method names in AclExpansionHtmlWriter.
    private void WriteTableBody_ProcessUser (User user, List<AclExpansionEntry> aclExpansion)
    {
      _implementation.WriteTableData (user.UserName);
      _implementation.WriteTableData (user.FirstName);
      _implementation.WriteTableData (user.LastName);

      string userDetailFileName = AclExpansionHtmlWriterImplementationBase.ToValidFileName (user.UserName); 
      using (var detailTextWriter = _textWriterFactory.CreateTextWriter (userDetailFileName))
      {

        var aclExpansionSingleUser = GetAccessControlEntriesForUser (aclExpansion, user);
        var detailAclExpansionHtmlWriter = new AclExpansionHtmlWriter (detailTextWriter, false, _detailHtmlWriterSettings);
        detailAclExpansionHtmlWriter.WriteAclExpansion (aclExpansionSingleUser);
      }

      string relativePath = _textWriterFactory.GetRelativePath (MasterFileName, userDetailFileName);
      _implementation.WriteTableRowBeginIfNotInTableRow (); 
      _implementation.HtmlTagWriter.Tags.td ();
      _implementation.HtmlTagWriter.Tags.a();
      _implementation.HtmlTagWriter.Attribute ("href", relativePath);
      _implementation.HtmlTagWriter.Attribute ("target", "_blank");
      _implementation.HtmlTagWriter.Value (relativePath);
      _implementation.HtmlTagWriter.Tags.aEnd ();
      _implementation.HtmlTagWriter.Tags.tdEnd ();
    }


    public static List<User> GetUsers (IEnumerable<AclExpansionEntry> aclExpansion)
    {
      return (from aee in aclExpansion
             let user = aee.User
             orderby user.LastName, user.FirstName, user.UserName
              select user).Distinct ().ToList ();
    }

    public static List<AclExpansionEntry> GetAccessControlEntriesForUser (IEnumerable<AclExpansionEntry> aclExpansion, User user)
    {
      return (from aee in aclExpansion
             where aee.User == user
             select aee).ToList();
    }
  }
}



