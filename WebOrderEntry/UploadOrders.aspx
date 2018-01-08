<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="UploadOrders.aspx.cs" Inherits="WebOrderEntry.UploadOrders1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <p>Please select a .csv batch order file to upload.</p>
    <asp:FileUpload ID="FileUpload1" runat="server" />
    <br />
    <asp:Button ID="btnUpload" runat="server" OnClick="btnUpload_Click"
        Text="Upload" />
    <br />
    <asp:Button ID="btnWarningSubmit" runat="server"
        Text="Submit with Warnings" OnClick="btnWarningSubmit_Click" Visible="False" />

    <br />
    <asp:Label ID="lblStatus" runat="server"></asp:Label>
    <table style="width: 1014px;">
        <tr>
            <td>Errors
            </td>
            <td>Please fix problems and submit again
            </td>
            <td></td>
        </tr>
    </table>
    <table style="border: thin solid #000000; width: 911px; height: 100px;">
        <tr>
            <td style="vertical-align: top">&nbsp;
                <div style="overflow: auto">
                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" Height="100px" Width="901px" />
                </div>
            </td>
        </tr>
        <tr>
            <asp:Label ID="lblDebug" runat="server" Text=""></asp:Label>
        </tr>
    </table>

</asp:Content>
