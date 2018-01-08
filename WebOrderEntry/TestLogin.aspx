<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestLogin.aspx.cs" Inherits="WebOrderEntry.WebForm1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <br />
        <br />
        WebOrder Entry Account Login Page:<br />
        <br />
        Select Account ID to Login:<br />
        <asp:DropDownList ID="ddlAccountId" runat="server">
            <asp:ListItem>BAFTEST</asp:ListItem>
            <asp:ListItem>BRUSHPOINT</asp:ListItem>
            <asp:ListItem>FSD</asp:ListItem>
            <asp:ListItem>GLKFOODS</asp:ListItem>
            <asp:ListItem>KELLOGGALM</asp:ListItem>
            <asp:ListItem>KELLOGGAUB</asp:ListItem>
            <asp:ListItem>KELLOGGAUG</asp:ListItem>
            <asp:ListItem>KELLOGGCLA</asp:ListItem>
            <asp:ListItem>KELLOGGELK</asp:ListItem>
            <asp:ListItem>KELLOGGGON</asp:ListItem>
            <asp:ListItem>KELLOGGGRE</asp:ListItem>
            <asp:ListItem>KELLOGGHAG</asp:ListItem>
            <asp:ListItem>KELLOGGIND</asp:ListItem>
            <asp:ListItem>KELLOGGJON</asp:ListItem>
            <asp:ListItem>KELLOGGJOP</asp:ListItem>
            <asp:ListItem>KELLOGGLOU</asp:ListItem>
            <asp:ListItem>KELLOGGMEM</asp:ListItem>
            <asp:ListItem>KELLOGGNEE</asp:ListItem>
            <asp:ListItem>KELLOGGNOR</asp:ListItem>
            <asp:ListItem>KELLOGGOMA</asp:ListItem>
            <asp:ListItem>KELLOGGOSH</asp:ListItem>
            <asp:ListItem>KELLOGGPHI</asp:ListItem>
            <asp:ListItem>KELLOGGPHO</asp:ListItem>
            <asp:ListItem>KELLOGGPOR</asp:ListItem>
            <asp:ListItem>KELLOGGROC</asp:ListItem>
            <asp:ListItem>KELLOGGSAN</asp:ListItem>
            <asp:ListItem>KELLOGGSEA</asp:ListItem>
            <asp:ListItem>KELLOGGSPR</asp:ListItem>
            <asp:ListItem>KELLOGGTER</asp:ListItem>
            <asp:ListItem>KELLOGGTRA</asp:ListItem>
            <asp:ListItem>KELLOGGWIL</asp:ListItem>
            <asp:ListItem>KELLOGMNKA</asp:ListItem>
            <asp:ListItem>KELLOGPLNT</asp:ListItem>
            <asp:ListItem>KELLOGWKKI</asp:ListItem>
            <asp:ListItem>LANELIMITD</asp:ListItem>
            <asp:ListItem>PREMIER</asp:ListItem>
            <asp:ListItem>REDCACTUS</asp:ListItem>
            <asp:ListItem>RJREYNOLDS</asp:ListItem>
            <asp:ListItem>SOVENA</asp:ListItem>
            <asp:ListItem>WHITFIELD</asp:ListItem>
        </asp:DropDownList>
        &nbsp;<asp:Button ID="btnLogon" runat="server" Text="LOGON" 
            onclick="btnLogon_Click" />
        <br />
    </div>
    </form>
</body>
</html>
