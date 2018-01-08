<%@ Page Title="DSC Web Order Entry Landing Page" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" Inherits="WebOrderEntry.LandingPage" Codebehind="LandingPage.aspx.cs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    Welcome back&nbsp;
    <asp:Label ID="lblUserId" runat="server"></asp:Label><br /><br />
    <asp:HyperLink ID="hlEnterOrder" runat="server" NavigateUrl="~/EnterOrder.aspx" 
        ToolTip="Enter Order">Enter Order</asp:HyperLink>
    <br />
    <br />
    <asp:HyperLink ID="hlBatchUpload" runat="server" NavigateUrl="~/UploadOrders.aspx" ToolTip="Upload Orders">
        Upload Orders
    </asp:HyperLink>
    <br />
    <br />
    <asp:Button ID="btnAdmin" runat="server" PostBackUrl="~/AdminPage.aspx" 
        Text="Admin Functions" Visible="False" />
    <br />
    <br />
</asp:Content>
