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
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OnLoadEventTest.aspx.cs"
  Inherits="Remotion.Web.Test.OnLoadEventTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title></title>
  <script type="text/javascript">
    function Page_OnLoad()
    {
      window.alert('On Load');
    }
  </script>
</head>
<body>
  <form id="form1" runat="server">
  <asp:ScriptManager runat="server" EnablePartialRendering="true" />
  <asp:UpdatePanel runat="server">
    <ContentTemplate>
      <asp:Button ID="AsyncButton" Text="Async PostBack" runat="server" />
    </ContentTemplate>
  </asp:UpdatePanel>
  <asp:Button ID="SyncButton" Text="Sync PostBack" runat="server" />
  </form>
</body>
</html>
