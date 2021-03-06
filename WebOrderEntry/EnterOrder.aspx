<%@ Page Language="C#" AutoEventWireup="True" Inherits="WebOrderEntry.EnterOrder"
    MasterPageFile="~/Site1.Master" Title="Enter Order" Codebehind="EnterOrder.aspx.cs" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <%--<asp:ScriptManager ID="ScriptManager1" runat="server">     </asp:ScriptManager>--%>
    <cc1:ToolkitScriptManager ID="toolkitScriptManager1" runat="server"></cc1:ToolkitScriptManager>
    <%--<asp:ToolkitScriptManager ID="toolkitScriptMaster" runat="server"></asp:ToolkitScriptManager>--%>

    <table id="tblLayout1" style="font-size: small; margin-right: 3px;">
        <tr>
            <td style="vertical-align: top;">
                <table>
                    <tr>
                        <td align="right">
                            Status&nbsp;
                        </td>
                        <td>
                            <asp:DropDownList ID="DropDownListStatus" runat="server" 
                                Style="margin-bottom: 2px" TabIndex="1">
                                <asp:ListItem Selected="True" Value="N">Original</asp:ListItem>
                                <asp:ListItem Value="R">Revision</asp:ListItem>
                                <asp:ListItem Value="F">Cancellation</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Customer Ref#
                        </td>
                        <td style="white-space: nowrap">
                            <asp:TextBox ID="tbCustomerRef" runat="server" BackColor="#FF99CC" MaxLength="22"
                                Width="110px" TabIndex="1"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="tbCustomerRef"
                                ErrorMessage="Customer Ref# is required." ToolTip="Customer Ref# is required.">*</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Consignee PO
                        </td>
                        <td style="white-space: nowrap">
                            <asp:TextBox ID="tbConsigneePO" runat="server" BackColor="#FF99CC" MaxLength="22"
                                Width="110px" TabIndex="2"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="tbConsigneePO"
                                ErrorMessage="Consignee PO is required." ToolTip="Consignee PO is required.">*</asp:RequiredFieldValidator>

<%--                        FD CHANGE - 04/30/2012  No Longer Required to Validate Alphacharacters. Any valid Character will be valid    
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="tbConsigneePO"
                                ErrorMessage="Consignee PO must be alphanumeric." ValidationExpression="[a-zA-Z0-9]+"
                                ToolTip="Consignee PO must be alphanumeric.">
                                *
                            </asp:RegularExpressionValidator>
--%>                        </td>
                    </tr>
                    <tr>
                        <td align="right" class="style2">
                            Export?
                        </td>
                        <td align="left" class="style2">
                            <asp:DropDownList ID="ddlExport" runat="server" Width="48px" TabIndex="3">
                                <asp:ListItem Selected="True" Value="N">No</asp:ListItem>
                                <asp:ListItem Value="Y">Yes</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Temp Control?
                        </td>
                        <td align="left">
                            <asp:DropDownList ID="ddlTempControl" runat="server" Width="48px" TabIndex="4">
                                <asp:ListItem Selected="True" Value="N">No</asp:ListItem>
                                <asp:ListItem Value="Y">Yes</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Hazmat?
                        </td>
                        <td align="left">
                            <asp:DropDownList ID="ddlHazmat" runat="server" Width="48px" TabIndex="5">
                                <asp:ListItem Selected="True" Value="N">No</asp:ListItem>
                                <asp:ListItem Value="Y">Yes</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Payment Method
                        </td>
                        <td align="left">
                            <asp:DropDownList ID="ddlPaymentMethod" runat="server" Width="126px" 
                                TabIndex="6">
                                <asp:ListItem Selected="True" Value="PP">Prepaid</asp:ListItem>
                                <asp:ListItem Value="CC">Collect</asp:ListItem>
                                <asp:ListItem Value="PC">Prepaid/Charge</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Transport Method
                        </td>
                        <td align="left">
                            <asp:DropDownList ID="ddlTransportMethod" runat="server" Width="126px" 
                                TabIndex="7">
                                <asp:ListItem Selected="True" Value="M">Motor (M)</asp:ListItem>
                                <asp:ListItem Value="U">Package (U)</asp:ListItem>
                                <asp:ListItem Value="H">Cust Pickup (H)</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Carrier SCAC
                        </td>
                        <td align="left">
                            <asp:DropDownList ID="ddlSCAC" runat="server" DataSourceID="SqlDataSourceSCAC" DataTextField="SCAC_CODE"
                                DataValueField="SCAC_CODE" Width="126px" AppendDataBoundItems="True" 
                                BackColor="#FF99CC" TabIndex="8">
                                <asp:ListItem Selected="True"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator16" 
                                runat="server" 
                                ErrorMessage="Carrier SCAC required" 
                                ControlToValidate="ddlSCAC"
                                Text="*"
                                ToolTip="Carrier SCAC required" />
                        </td>
                    </tr>
                    <tr>
                        <td align="right" >
                            Total Weight
                        </td>
                        <td align="left" style="white-space: nowrap" >
                            <asp:TextBox ID="tbTotalWeight" runat="server" Width="92px" BackColor="#FF99CC" 
                                TabIndex="9" MaxLength="10"></asp:TextBox>
                            LB<asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server"
                                ControlToValidate="tbTotalWeight" ErrorMessage="Total Weight must be numeric."
                                ValidationExpression="[0-9]+" ToolTip="Total Weight must be numeric.">*</asp:RegularExpressionValidator>
                            <asp:RangeValidator ID="RangeValidator1" runat="server" ControlToValidate="tbTotalWeight"
                                ErrorMessage="Total Weight must be 0-9999999999" MaximumValue="9999999999" MinimumValue="0"
                                ToolTip="Total Weight must be 0-9999999999">*</asp:RangeValidator>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator15" runat="server" ControlToValidate="tbTotalWeight"
                                ErrorMessage="Total Weight is required" ToolTip="Total Weight is required">*</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td align="right" >
                            Total Volume
                        </td>
                        <td align="left" style="white-space: nowrap" >
                            <asp:TextBox ID="tbTotalVolume" runat="server" Width="92px" 
                                Style="margin-bottom: 0px" TabIndex="10" MaxLength="10"></asp:TextBox>
                            CF
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator5" runat="server"
                                ControlToValidate="tbTotalVolume" ErrorMessage="Total Volume must be numeric."
                                ValidationExpression="^[-+]?[0-9]*\.?[0-9]+$" ToolTip="Total Volume must be numeric.">*</asp:RegularExpressionValidator>
                            <asp:RangeValidator ID="RangeValidator2" runat="server" ControlToValidate="tbTotalVolume"
                                ErrorMessage="Total Volume must be 0 - 9999999.99" MaximumValue="9999999.99"
                                MinimumValue="0" ToolTip="Total Volume must be 0 - 9999999.99">*</asp:RangeValidator>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                           No.of Pallets
                        </td>
                        <td align="left" style="white-space: nowrap" >
                           <asp:TextBox ID="tbPalletCount" runat="server" Width="92px"
                           Style="margin-bottom: 0px" MaxLength="3" ></asp:TextBox>
                           <asp:RegularExpressionValidator ID="RegularExpressionValidator6" runat="server"
                                ControlToValidate="tbPalletCount" ErrorMessage="Number of Pallets must be numeric."
                                ValidationExpression="^[-+]?[0-9]*\.?[0-9]+$" 
                                ToolTip="Number of Pallets must be numeric.">
                           *
                           </asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            &nbsp;
                        </td>
                        <td align="left">
                            <asp:SqlDataSource ID="SqlDataSourceRef" runat="server" ConnectionString="<%$ ConnectionStrings:WebOrderEntryConnectionString %>"
                                SelectCommand="SELECT [REF_ID], [REF_DESC] FROM [REFERENCE_QUALIFIER] WHERE ([REF_TYPE] = @REF_TYPE) ORDER BY [REF_DESC]">
                                <SelectParameters>
                                    <asp:Parameter DefaultValue="H" Name="REF_TYPE" Type="String" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                            <asp:SqlDataSource ID="SqlDataSourceNotes" runat="server" ConnectionString="<%$ ConnectionStrings:WebOrderEntryConnectionString %>"
                                SelectCommand="SELECT DISTINCT [INST_CODE] FROM [INSTRUMENT_CODES] ORDER BY [INST_CODE]">
                            </asp:SqlDataSource>

                            <asp:SqlDataSource ID="SqlDataSourceLocationId" runat="server" ConnectionString="<%$ ConnectionStrings:WebOrderEntryConnectionString %>"
                                SelectCommand="SELECT DISTINCT [LOCATION_ID] FROM [ACCOUNT_MASTER] WHERE ([ACCOUNT_ID] = @ACCOUNT_ID) ORDER BY [LOCATION_ID]">
                                <SelectParameters>
                                    <asp:SessionParameter Name="ACCOUNT_ID" SessionField="AccountId" Type="String" />
                                </SelectParameters>
                            </asp:SqlDataSource>

                            <asp:SqlDataSource ID="SqlDataSourceLocOrigId" runat="server" ConnectionString="<%$ ConnectionStrings:WebOrderEntryConnectionString %>"
                                SelectCommand="SELECT DISTINCT [LOCATION_ID] FROM [ACCOUNT_MASTER] WHERE ([ACCOUNT_ID] = @ACCOUNT_ID AND [LOCATION_ID] IN (SELECT DISTINCT [LOCATION_ID] FROM [TestLocation_MASTER] WHERE ([ELSCATR] IN ('O', 'B')) ) ) ORDER BY [LOCATION_ID]">
                                <SelectParameters>
                                    <asp:SessionParameter Name="ACCOUNT_ID" SessionField="AccountId" Type="String" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                            <asp:SqlDataSource ID="SqlDataSourceLocDestId" runat="server" ConnectionString="<%$ ConnectionStrings:WebOrderEntryConnectionString %>"
                                SelectCommand="SELECT DISTINCT [LOCATION_ID] FROM [ACCOUNT_MASTER] WHERE ([ACCOUNT_ID] = @ACCOUNT_ID AND [LOCATION_ID] IN (SELECT DISTINCT [LOCATION_ID] FROM [TestLocation_MASTER] WHERE ( [ELSCATR] IN ('D', 'B')) ) ) ORDER BY [LOCATION_ID]">
                                <SelectParameters>
                                    <asp:SessionParameter Name="ACCOUNT_ID" SessionField="AccountId" Type="String" />
                                </SelectParameters>
                            </asp:SqlDataSource>

                            <asp:SqlDataSource ID="SqlDataSourceDetailRef" runat="server" ConnectionString="<%$ ConnectionStrings:WebOrderEntryConnectionString %>"
                                SelectCommand="SELECT [REF_ID] FROM [REFERENCE_QUALIFIER] WHERE ([REF_TYPE] = @REF_TYPE) ORDER BY [REF_ID]">
                                <SelectParameters>
                                    <asp:Parameter DefaultValue="D" Name="REF_TYPE" Type="String" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                            <asp:SqlDataSource ID="SqlDataSourceSCAC" runat="server" ConnectionString="<%$ ConnectionStrings:WebOrderEntryConnectionString %>"
                                ProviderName="<%$ ConnectionStrings:WebOrderEntryConnectionString.ProviderName %>"
                                SelectCommand="SELECT [SCAC_CODE] FROM [SCAC]"></asp:SqlDataSource>
                        </td>
                    </tr>
                </table>
            </td>
            <td valign="top" style="vertical-align: top;">
                <asp:UpdatePanel ID="UpdatePanel8" runat="server" ChildrenAsTriggers="true">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlLocationId" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlShipToId" EventName="SelectedIndexChanged" />
                    </Triggers>
                    <ContentTemplate>
                        <table>
                            <tr>
                                <td align="center" colspan="2">
                                    Ship From
                                </td>
                                <td align="center" colspan="2">
                                    Ship To
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    Location
                                </td>
                                <td style="white-space: nowrap">
                                    <asp:DropDownList ID="ddlLocationId" runat="server" AutoPostBack="True" DataSourceID="SqlDataSourceLocOrigId"
                                        DataTextField="LOCATION_ID" DataValueField="LOCATION_ID" OnSelectedIndexChanged="ddlLocationId_SelectedIndexChanged"
                                        AppendDataBoundItems="True" TabIndex="11">
                                        <asp:ListItem Selected="True"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td align="right">
                                    Location
                                </td>
                                <td style="white-space: nowrap">
                                    <asp:DropDownList ID="ddlShipToId" runat="server" AutoPostBack="True" DataSourceID="SqlDataSourceLocDestId"
                                        DataTextField="LOCATION_ID" DataValueField="LOCATION_ID" OnSelectedIndexChanged="ddlShipToId_SelectedIndexChanged"
                                        AppendDataBoundItems="True">
                                        <asp:ListItem Selected="True"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>

                            </tr>
                            <tr>
                                <td align="right">
                                    Name
                                </td>
                                <td style="white-space: nowrap">
                                    <asp:TextBox ID="tbShipFromName" runat="server" BackColor="#FF99CC" MaxLength="35"
                                        Width="110px" TabIndex="12"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="tbShipFromName"
                                        ErrorMessage="Ship From Name is required." ToolTip="Ship From Name is required.">*</asp:RequiredFieldValidator>
                                </td>
                                <td align="right">
                                    Name
                                </td>
                                <td style="white-space: nowrap">
                                    <asp:TextBox ID="tbShipToName" runat="server" BackColor="#FF99CC" MaxLength="35"
                                        Width="110px" TabIndex="19"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ControlToValidate="tbShipToName"
                                        ErrorMessage="Ship To Name is required." ToolTip="Ship To Name is required.">*</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    Address1
                                </td>
                                <td style="white-space: nowrap">
                                    <asp:TextBox ID="tbShipFromAddress1" runat="server" BackColor="#FF99CC" MaxLength="35"
                                        Width="110px" TabIndex="13"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="tbShipFromAddress1"
                                        ErrorMessage="Ship From Address1 is required." ToolTip="Ship From Address1 is required.">*</asp:RequiredFieldValidator>
                                </td>
                                <td align="right">
                                    Address1
                                </td>
                                <td style="white-space: nowrap">
                                    <asp:TextBox ID="tbShipToAddress1" runat="server" BackColor="#FF99CC" MaxLength="35"
                                        Width="110px" TabIndex="20"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" ControlToValidate="tbShipToAddress1"
                                        ErrorMessage="Ship To Address1 is required." 
                                        ToolTip="Ship To Address1 is required.">*</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    Address2
                                </td>
                                <td>
                                    <asp:TextBox ID="tbShipFromAddress2" runat="server" MaxLength="35" 
                                        Width="110px" TabIndex="14"></asp:TextBox>
                                </td>
                                <td align="right">
                                    Address2
                                </td>
                                <td>
                                    <asp:TextBox ID="tbShipToAddress2" runat="server" MaxLength="35" Width="110px" 
                                        TabIndex="21"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    City
                                </td>
                                <td style="white-space: nowrap">
                                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                        <ContentTemplate>
                                            <asp:TextBox ID="tbCity" runat="server" MaxLength="30" BackColor="#FF99CC" 
                                                Width="110px" TabIndex="15"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="tbCity"
                                                ErrorMessage="Ship From City is required." ToolTip="Ship From City is required.">*</asp:RequiredFieldValidator>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                                <td align="right">
                                    City
                                </td>
                                <td style="white-space: nowrap">
                                    <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                                        <ContentTemplate>
                                            <asp:TextBox ID="tbShipToCity" runat="server" MaxLength="30" BackColor="#FF99CC"
                                                Width="110px" TabIndex="22"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" ControlToValidate="tbShipToCity"
                                                ErrorMessage="Ship To City is required." ToolTip="Ship To City is required.">*</asp:RequiredFieldValidator>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    State/Province
                                </td>
                                <td style="white-space: nowrap">
                                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                        <ContentTemplate>
                                            <asp:TextBox ID="tbShipFromState" runat="server" BackColor="#FF99CC" 
                                                Width="110px" TabIndex="16" MaxLength="2"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="tbShipFromState"
                                                ErrorMessage="Ship From State/Province is required." ToolTip="Ship From State/Province is required.">*</asp:RequiredFieldValidator>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                                <td align="right">
                                    State/Province
                                </td>
                                <td style="white-space: nowrap">
                                    <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                                        <ContentTemplate>
                                            <asp:TextBox ID="tbShipToState" runat="server" BackColor="#FF99CC" 
                                                Width="110px" TabIndex="23" MaxLength="2"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator12" runat="server" ControlToValidate="tbShipToState"
                                                ErrorMessage="Ship To State/Province is required." ToolTip="Ship To State/Province is required.">*</asp:RequiredFieldValidator>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    Postal Code
                                </td>
                                <td style="white-space: nowrap">
                                    <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                        <ContentTemplate>
                                            <asp:TextBox ID="tbPostalCode" runat="server" BackColor="#FF99CC" Width="110px" 
                                                TabIndex="17" MaxLength="10"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ControlToValidate="tbPostalCode"
                                                ErrorMessage="Ship From Postal Code is required." ToolTip="Ship From Postal Code is required.">*</asp:RequiredFieldValidator>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                                <td align="right">
                                    Postal Code
                                </td>
                                <td style="white-space: nowrap">
                                    <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                                        <ContentTemplate>
                                            <asp:TextBox ID="tbShipToPostalCode" runat="server" BackColor="#FF99CC" 
                                                Width="110px" TabIndex="24" MaxLength="10"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator13" runat="server" ControlToValidate="tbShipToPostalCode"
                                                ErrorMessage="Ship To Postal Code is required." ToolTip="Ship To Postal Code is required.">*</asp:RequiredFieldValidator>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    Ship Date
                                </td>
                                <td style="white-space: nowrap">
                                    <asp:TextBox ID="tbShipDate" runat="server" BackColor="#FF99CC" Width="110px" 
                                        TabIndex="18" MaxLength="10"></asp:TextBox>
                                    <cc1:CalendarExtender ID="tbShipDate_CalendarExtender" runat="server" TargetControlID="tbShipDate"
                                        PopupButtonID="imageCalendar">
                                    </cc1:CalendarExtender>
                                    <cc1:TextBoxWatermarkExtender ID="tbwmShipDate" runat="server" TargetControlID="tbShipDate"
                                        WatermarkText="dd/mm/ccyy" WatermarkCssClass="watermarked" />
                                    <asp:Image ID="imageCalendar" runat="server" Height="16px" ImageUrl="images/Calendar.png" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ControlToValidate="tbShipDate"
                                        ToolTip="Ship Date is required." ErrorMessage="Ship Date is required." Text="*" />
                                </td>
                                <td align="right">
                                    Delivery Date
                                </td>
                                <td style="white-space: nowrap">
                                    <asp:TextBox ID="tbDeliveryDate" runat="server" BackColor="#FF99CC" 
                                        Width="110px" TabIndex="25" MaxLength="10"></asp:TextBox>
                                    <cc1:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" runat="server" TargetControlID="tbDeliveryDate"
                                        WatermarkText="dd/mm/ccyy" WatermarkCssClass="watermarked" />
                                    <cc1:CalendarExtender ID="tbDeliveryDate_CalendarExtender" runat="server" TargetControlID="tbDeliveryDate"
                                        PopupButtonID="imageCalendar0">
                                    </cc1:CalendarExtender>
                                    <asp:Image ID="imageCalendar0" runat="server" Height="16px" ImageUrl="images/Calendar.png" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator14" runat="server" ControlToValidate="tbDeliveryDate"
                                        ErrorMessage="Delivery Date is required." ToolTip="Delivery Date is required.">*</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr >
                                <td colspan="4">
                                    <asp:UpdatePanel ID="UpdatePanelRef" runat="server" ChildrenAsTriggers="true">
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="btnaddReferenceItem" EventName="Click" />
                                            <asp:AsyncPostBackTrigger ControlID="hdnRefDeleteIndex" EventName="valuechanged" />
                                        </Triggers>
                                        <ContentTemplate>
                                            <asp:PlaceHolder ID="phRef" runat="server"></asp:PlaceHolder>
                                            <asp:HiddenField ID="hdnRefDeleteIndex" runat="server" OnValueChanged="hdnRefDeleteIndex_ValueChanged" />
                                            <asp:Button ID="btnaddReferenceItem" runat="server" 
                                                CausesValidation="False" Height="26px"
                                                OnClick="btnaddReferenceItem_Click" 
                                               Style="margin-left: 0px" Text="Add Ref" Width="64px" TabIndex="26" />                                                
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td style="vertical-align: top; text-align: left; font-size: small;">
                <table>
                    <tr>
                        <td colspan="2" style="text-align: center">
                            ASN and Label
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right">
                            ASN?
                        </td>
                        <td style="text-align: left">
                            <asp:DropDownList ID="ddlAsn" runat="server" Width="48px" TabIndex="26">
                                <asp:ListItem Selected="True" Value="N">No</asp:ListItem>
                                <asp:ListItem Value="Y">Yes</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right">
                            ASN ID
                        </td>
                        <td>
                            <asp:TextBox ID="tbAsnId" runat="server" Width="110px" TabIndex="27" 
                                MaxLength="17"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right">
                            Location Number
                        </td>
                        <td>
                            <asp:TextBox ID="tbAsnLu" runat="server" Width="110px" TabIndex="28" 
                                MaxLength="30"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right">
                            <asp:Label ID="Label1" runat="server" Text="Merchadise Code" ToolTip="Merchadise Type Code"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="tbAsnMr" runat="server" Width="110px" TabIndex="29" 
                                MaxLength="30"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">
                            <asp:Label ID="Label2" runat="server" Text="Dept Number" ToolTip="Department Number "></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="tbAsnDp" runat="server" Width="110px" TabIndex="30" 
                                MaxLength="30"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right">
                            UCC128?
                        </td>
                        <td style="text-align: left; vertical-align: top;">
                            <asp:DropDownList ID="ddlUCC128" runat="server" Width="48px" TabIndex="31">
                                <asp:ListItem Selected="True" Value="N">No</asp:ListItem>
                                <asp:ListItem Value="Y">Yes</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="left">
                            <asp:UpdatePanel ID="UpdatePaneNotes" runat="server">
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="btnAddNote" EventName="Click" />
                                    <asp:AsyncPostBackTrigger ControlID="hdnDeleteNotesIndex" EventName="valuechanged" />
                                </Triggers>
                                <ContentTemplate>
                                    <asp:PlaceHolder ID="phNotes" runat="server"></asp:PlaceHolder>
                                    <asp:HiddenField ID="hdnDeleteNotesIndex" runat="server" OnValueChanged="hdnDeleteNotesIndex_ValueChanged" />
                                    <asp:Button ID="btnAddNote" runat="server" CausesValidation="False" Height="26px"
                                        OnClick="btnAddNote_Click" Text="Add Comment" Width="100px" TabIndex="32" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:UpdatePanel ID="UpdatePanel7" runat="server" ChildrenAsTriggers="true">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnAddDetail" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="hdnDeleteIndex" EventName="valuechanged" />
        </Triggers>
        <ContentTemplate>
            <asp:PlaceHolder ID="phDetail" runat="server"></asp:PlaceHolder>
            <asp:HiddenField ID="hdnDeleteIndex" runat="server" OnValueChanged="hdnDeleteIndex_ValueChanged" />
            &nbsp;<asp:Button ID="btnAddDetail" runat="server" OnClick="btnAddDetail_Click" Text="Add Detail"
                ToolTip="Add Detail" CausesValidation="False" TabIndex="33" />
            <asp:CustomValidator ID="CustomValidator1" runat="server" OnServerValidate="ServerValidateEventHandler" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <table style="width: 1014px;">
        <tr>
            <td>
                Errors
            </td>
            <td>
                Please fix problems and submit again
            </td>
            <td>
            </td>
        </tr>
    </table>
    <table style="border: thin solid #000000; width: 911px; height: 100px;">
        <tr>
            <td style="vertical-align: top">
                &nbsp;
                <div style="overflow: auto">
                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" Height="100px" Width="901px" />
                </div>
            </td>
        </tr>
    </table>
    <asp:Button ID="Submit" runat="server" PostBackUrl="~/EnterOrder.aspx" Text="Submit"
        OnClick="Submit_Click" TabIndex="34" />
    <br />
    <asp:Button ID="btnWarningSubmit" runat="server" Text="Submit with Warnings" 
        onclick="btnWarningSubmit_Click" Visible="False" />
    <br />
    <asp:Label ID="lblMsg" runat="server"></asp:Label>
</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="head">
    <style type="text/css">
        .style2
        {
            height: 20px;
        }
    </style>
</asp:Content>
