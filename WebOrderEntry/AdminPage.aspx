<%@ Page Language="C#" AutoEventWireup="True" Inherits="WebOrderEntry.AdminPage" CodeBehind="AdminPage.aspx.cs"  %>
<%--<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminPage.aspx.cs" Inherits="WebOrderEntry.WebForm2" %>--%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<script runat="server">

</script>


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <p>
    <br />
    </p>
    <form id="form1" runat="server">
    <p>
        <asp:Label ID="lblRefType" runat="server" Text="Ref. Type"></asp:Label>
         &nbsp;&nbsp;&nbsp;
        <asp:Label ID="lblRefId" runat="server" Text="Ref. Id"></asp:Label>
         &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Label ID="lblRefType1" runat="server" Text="Reference Description"></asp:Label>
        <br />
        <asp:DropDownList ID="ddlRefType" runat="server" OnSelectedIndexChanged="ddlRefType_SelectedIndexChanged">
            <asp:ListItem Value="H">Header</asp:ListItem>
            <asp:ListItem Value="D">Detail</asp:ListItem>
        </asp:DropDownList>
        &nbsp;
        <asp:TextBox ID="txtRefId" runat="server" MaxLength="4" Width="35px"></asp:TextBox>
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="txtRefDesc" runat="server" MaxLength="45" Width="200px" OnTextChanged="txtRefDesc_TextChanged"></asp:TextBox>
        <br />
        <asp:Label ID="lblErrMsg" runat="server" ForeColor="Red"></asp:Label>       
    </p>
    <p>
        &nbsp;
        <asp:Button ID="btnRefUpd" runat="server" Text="Create Ref" 
            onclick="Button1_Click" />
    </p>
    <div>
    
    </div>
    </form>
</body>
</html>
