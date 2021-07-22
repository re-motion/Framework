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

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SutForm.aspx.cs" Inherits="Remotion.Web.Test.UpdatePanelTests.SutForm" MasterPageFile="UpdatePanelMasterPage.master" %>

<%@ Register Src="SutUserControl.ascx" TagName="SutUserControl" TagPrefix="rwt" %>
<asp:Content runat="server" ContentPlaceHolderID="FormContent">
  <asp:ScriptManager ID="ScriptManager" runat="server" />
  <table style="width: 100%; height: 100%;">
    <tr>
      <td style="vertical-align: top;">
        <rwt:SutUserControl ID="SutUserControl" runat="server" />
      </td>
    </tr>
  </table>
</asp:Content>
