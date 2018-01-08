<%@ Page Title="Web Order Entry: Error Page" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="error.aspx.cs" Inherits="WebOrderEntry.error" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <p>An unexpected error occurred in the application.  Error Code: 19</p>
    <p>
        Please contact customer service. Or, you can click <a href="/LandingPage.aspx">here</a> 
        to go back to the Web Order Entry landing page.</p>
    <p>
        Thank you for your patience.</p>
    <p>
        &nbsp;</p>
</asp:Content>
