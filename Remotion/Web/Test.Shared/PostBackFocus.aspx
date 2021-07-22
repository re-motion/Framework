<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PostBackFocus.aspx.cs" Inherits="Remotion.Web.Test.PostBackFocus"  %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server" EnablePartialRendering="true" />
    <div>
    <asp:HyperLink ID="SelfLink" runat="server" NavigateUrl="~/PostBackFocus.aspx">Self</asp:HyperLink>
    <br />
<asp:TextBox ID="Field1" runat="server" AutoPostBack="true" />
<asp:TextBox ID="Field2" runat="server" AutoPostBack="true" />
<asp:TextBox ID="Field3" runat="server" AutoPostBack="true" />
<asp:TextBox ID="Field4" runat="server" AutoPostBack="true" />
<asp:TextBox ID="Field5" runat="server" AutoPostBack="true" />
<p></p>
        Update Panel:
      <asp:UpdatePanel runat="server">
        <ContentTemplate>
<asp:TextBox ID="UpdatePanelField1" runat="server" AutoPostBack="true" />
<asp:TextBox ID="UpdatePanelField2" runat="server" AutoPostBack="true" />
<asp:TextBox ID="UpdatePanelField3" runat="server" AutoPostBack="true" />
<asp:TextBox ID="UpdatePanelField4" runat="server" AutoPostBack="true" />
<asp:TextBox ID="UpdatePanelField5" runat="server" AutoPostBack="true" />
        </ContentTemplate>
      </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
