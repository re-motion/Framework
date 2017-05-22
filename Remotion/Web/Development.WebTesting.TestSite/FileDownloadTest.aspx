<%-- This file is part of the re-motion Core Framework (www.re-motion.org)
 % Copyright (c) rubicon IT GmbH, www.rubicon.eu
 %
 % The re-motion Core Framework is free software; you can redistribute it 
 % and/or modify it under the terms of the GNU Lesser General Public License 
 % as published by the Free Software Foundation; either version 2.1 of the 
 % License, or (at your option) any later version.
 %
 % re-motion is distributed in the hope that it will be useful, 
 % but WITHOUT ANY WARRANTY; without even the implied warranty of 
 % MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
 % GNU Lesser General Public License for more details.
 %
 % You should have received a copy of the GNU Lesser General Public License
 % along with re-motion; if not, see http://www.gnu.org/licenses.
--%>
<%@ Page Language="C#" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="FileDownloadTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.FileDownloadTest" %>
<asp:Content ContentPlaceHolderID="body" runat="server">

  <h3>File Download</h3>
  <h4>Typical download use cases</h4>
  
  <p>
    <asp:Button ID="DownloadPostbackButton" Text="Start download via postback" runat="server"/>
  </p>
  <p>
    <a id="TargetBlankAnchor" target="_blank" href="FileDownloadHandler.ashx?testMode=xml" runat="server">Download via anchor with target='_blank'</a>
  </p>
  <p>
    <a id="DownloadXmlFile" target="_blank" href="FileDownloadHandler.ashx?testMode=xml" runat="server">Download Xml file</a>
  </p>
  
  <h4>Download use cases which replace the current page</h4>
  <p>
    <asp:Button ID="DownloadTxtReplaceSiteButton" Text="Start download which replaces current page" runat="server"/>
  </p>
  <p>
    <a id="TargetSelfAnchor" target="_self" href="FileDownloadHandler.ashx?testMode=txt" runat="server">Download via anchor with target='_self'</a>
  </p>

  <h4>Error use case</h4>
  <p>
    <a id="DownloadWith5SecondTimeout" href="FileDownloadHandler.ashx?testMode=longRunning" target="_blank" runat="server">Download which does not update the file for 5 seconds</a>
  </p>
  
  <h4>Special use case</h4>
  <p>
    <a id="DownloadFileWithoutFileExtension" target="_blank" href="FileDownloadHandler.ashx?testMode=withoutExtension" runat="server">Download file without file extension</a>
  </p>

</asp:Content>