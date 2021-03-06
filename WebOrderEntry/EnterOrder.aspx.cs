﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Web.UI.HtmlControls;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Data;
using System.Configuration;
using System.Diagnostics;
using System.Collections;


namespace WebOrderEntry
{
    // There are three dynamic tables that are used on this page.
    // Each dynamic table must be rebuilt after every postback.
    // If a record in a dynamic table is deleted, that row's visibility attribute
    // is set to false.  The row is not actually deleted.  
    // Logic in this module tests the row visiblity to determine if a row is valid
    // and should be shown/processed.

    public partial class EnterOrder : System.Web.UI.Page
    {
        const string TB_DETAIL_PRODUCT_NUMBER_PREFIX = "tbProductNumber";
        const string TB_DETAIL_LOT_PREFIX = "tbLot";
        const string DDL_DETAIL_REF_TYPE_PREFIX = "ddlDetailReferenceType";
        const string TB_DETAIL_REF_NUMBER_PREFIX = "tbDetailReferenceNumber";
        const string TB_DETAIL_REF_DESC_PREFIX = "tbDetailReferenceDescription";
        const string TB_DETAIL_QUANTITY_PREFIX = "tbQuantityNumber";
        const string TB_DETAIL_CUBE_PREFIX = "tbCube";
        const string TB_DETAIL_WEIGHT_PREFIX = "tbWeight";

        const string DDL_HEADER_REF_QUALIFIER_PREFIX = "ddlReferenceQualifer";
        const string TB_HEADER_REF_DESC_PREFIX = "tbReferenceDescription";

        const string DDL_NOTE_QUALIFIER_PREFIX = "ddlNoteQualifer";
        const string TB_NOTE_DESC_PREFIX = "tbNotesDescription";

        //enumerators for dynamic table rows
        enum DetailCells
        {
            LINE_NUMBER, PRODUCT_NUMBER, QUANTITY, LOT_NUMBER, DETAIL_CUBE, DETAIL_WEIGHT,
            DETAIL_REF_QUALIFIER, DETAIL_REF_NUMBER, DETAIL_REF_DESCRIPTION
        };

        enum HeaderReferenceCells { QUALIFIER, DESCRIPTION };
        enum NoteCells { QUALIFIER, DESCRIPTION };

        protected void Page_PreInit(object sender, EventArgs e)
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (!IsPostBack)
            {
                //initialize all the dynamic tables and row count
                Session["detailCount"] = "1";
                Session["referenceCount"] = "1";
                Session["notesCount"] = "1";
                addDetailTable();
                addRowToDetailTable(1);
                addReferenceTable();
                addRowToReferenceTable(1);
                addNotesTable();
                addRowToNotesTable(1);
            }
            else
            {
                //add a new row to the correct dynamic table and
                //rebuild the other dynamic tables

                //check to see which button was clicked
                Control myControl = GetPostBackControl(this.Page.Master);

                if ((myControl != null))
                {
                    //detail lines
                    if ((myControl.ClientID.ToString() == btnAddDetail.ClientID))
                    {
                        string sOldDetailCount = (string)Session["detailCount"];
                        int iOldDetailCount = Convert.ToInt32(sOldDetailCount);
                        int iNewDetailCount = iOldDetailCount + 1;
                        string sNewDetailCount = Convert.ToString(iNewDetailCount);

                        //intial index is set to one so that the new
                        //rows will be added after the table header
                        addDetailTable();

                        for (int x = 1; x <= iNewDetailCount; x++)
                        {
                            addRowToDetailTable(x);
                        }

                        //update detail count
                        Session["detailCount"] = Convert.ToString(sNewDetailCount);
                        if (phDetail.Controls.Count > 0)
                        {
                            Table detailTable = (Table)phDetail.Controls[0];
                            Page.SetFocus(detailTable.Rows[iNewDetailCount].Cells[1]);
                        }
                        createRefItemsForPostback();
                        createNoteItemsForPostback();
                    }

                    //reference items
                    if ((myControl.ClientID.ToString() == btnaddReferenceItem.ClientID))
                    {
                        string sOldDetailCount = (string)Session["referenceCount"];
                        int iOldDetailCount = Convert.ToInt32(sOldDetailCount);
                        int iNewDetailCount = iOldDetailCount + 1;
                        string sNewDetailCount = Convert.ToString(iNewDetailCount);

                        //intial index is set to one so that the new
                        //rows will be added after the table header
                        addReferenceTable();

                        for (int x = 1; x <= iNewDetailCount; x++)
                        {
                            addRowToReferenceTable(x);
                        }

                        //update detail count
                        Session["referenceCount"] = Convert.ToString(sNewDetailCount);
                        if (phRef.Controls.Count > 0)
                        {
                            Table refTable = (Table)phRef.Controls[0];
                            Page.SetFocus(refTable.Rows[iNewDetailCount].Cells[0]);
                        }
                        createDetailForPostback();
                        createNoteItemsForPostback();
                    }

                    //note items (called Comments on the web page)
                    if ((myControl.ClientID.ToString() == btnAddNote.ClientID))
                    {
                        string sOldDetailCount = (string)Session["notesCount"];
                        int iOldDetailCount = Convert.ToInt32(sOldDetailCount);
                        int iNewDetailCount = iOldDetailCount + 1;
                        string sNewDetailCount = Convert.ToString(iNewDetailCount);

                        //intial index is set to one so that the new
                        //rows will be added after the table header
                        addNotesTable();

                        for (int x = 1; x <= iNewDetailCount; x++)
                        {
                            addRowToNotesTable(x);
                        }

                        //update detail count
                        Session["notesCount"] = Convert.ToString(sNewDetailCount);
                        if (phNotes.Controls.Count > 0)
                        {
                            Table notesTable = (Table)phNotes.Controls[0];
                            Page.SetFocus(notesTable.Rows[iNewDetailCount].Cells[0]);
                        }
                        createDetailForPostback();
                        createRefItemsForPostback();
                    }


                    if ((myControl.ClientID.ToString() == Submit.ClientID))
                    {
                        createDetailForPostback();
                        createRefItemsForPostback();
                        createNoteItemsForPostback();
                    }

                }
            }
        }

        //dynamic cotrols must be recreated after every postback
        private void createDetailForPostback()
        {
            string sCount = (string)Session["detailCount"];
            int iDetailCount = Convert.ToInt32(sCount);

            //intial index is set to one so that the new
            //rows will be added after the table header
            addDetailTable();

            for (int x = 1; x <= iDetailCount; x++)
            {
                addRowToDetailTable(x);
            }
        }

        //dynamic cotrols must be recreated after every postback
        private void createRefItemsForPostback()
        {
            string sRefCount = (string)Session["referenceCount"];
            int iRefCount = Convert.ToInt32(sRefCount);

            //create table and header
            addReferenceTable();
            //create ref items
            for (int x = 1; x <= iRefCount; x++)
            {
                addRowToReferenceTable(x);
            }
        }

        //dynamic cotrols must be recreated after every postback
        private void createNoteItemsForPostback()
        {
            string sNotesCount = (string)Session["notesCount"];
            int iNotesCount = Convert.ToInt32(sNotesCount);

            //create table and header
            addNotesTable();
            //create ref items
            for (int x = 1; x <= iNotesCount; x++)
            {
                addRowToNotesTable(x);
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                DropDownListStatus.Focus();
                // ----------------  Change done by FD on 04/30/2012 -------
                // TO Disable the logoff URL and let the menu item close the broser page instead ----

                //DSC system passed in the logoff url
                //must set the menu item up on each page
                //if (Session["LogoffUrl"] != null)
                //{
                //    Menu masterPageMenu = (Menu)this.Master.FindControl("Menu1");
                //    MenuItem logoffMenuItem = masterPageMenu.Items[2];
                //    logoffMenuItem.NavigateUrl = (string)Session["LogoffUrl"];
                //}                
            }
            
            //_____________________________________________________________________________________________
            // Verify That user is still logged on. Otherwise send them back to the sigon screen
            try
            {
                if (Session["AccountId"].Equals(null))
                { Response.Redirect("https://www.mydsconline.com/"); }
                else
                { lblMsg.Text = "Logged-on under Account: " + Session["AccountId"]; }
            }
            catch
            {
                Response.Redirect("https://www.mydsconline.com/");
            }//____________________________________________________________________________________________

            //display successful alert box
            if (Session["lblStatus"] != null)
            {
                //added location = 'EnterOrder.aspx'; to script so that the page would render properly
                //without it, the controls would be the wrong size
                Response.Write("<script language = 'javascript'>window.alert('Order submitted successfully.'); " + 
                    "location = 'EnterOrder.aspx';" +
                    "</script>");
                //lblStatus.Text = (string)Session["lblStatus"];
                Session["lblStatus"] = null;
            }

            //detail item delete
            //check to see if a delete img button was clicked
            if (hdnDeleteIndex.Value.Length > 0 &&
                !hdnDeleteIndex.Value.Equals("0"))
            {
                int iDeleteThisRow = Convert.ToInt32(hdnDeleteIndex.Value);
                if (iDeleteThisRow > 0)
                {
                    DeleteRow(iDeleteThisRow);
                    hdnDeleteIndex.Value = "0";
                }
                createRefItemsForPostback();
                createNoteItemsForPostback();
            }

            //reference item delete
            //check to see if a delete img button was clicked
            if (hdnRefDeleteIndex.Value.Length > 0 &&
                !hdnRefDeleteIndex.Value.Equals("0"))
            {
                int iDeleteThisRow = Convert.ToInt32(hdnRefDeleteIndex.Value);
                if (iDeleteThisRow > 0)
                {
                    DeleteRefItemRow(iDeleteThisRow);
                    hdnRefDeleteIndex.Value = "0";
                }
                createDetailForPostback();
                createNoteItemsForPostback();
            }
            //notes item delete
            //check to see if a delete img button was clicked
            if (hdnDeleteNotesIndex.Value.Length > 0 &&
                !hdnDeleteNotesIndex.Value.Equals("0"))
            {
                int iDeleteThisRow = Convert.ToInt32(hdnDeleteNotesIndex.Value);
                if (iDeleteThisRow > 0)
                {
                    DeleteNotesItemRow(iDeleteThisRow);
                    hdnDeleteNotesIndex.Value = "0";
                }
                createDetailForPostback();
                createRefItemsForPostback();
            }

            //recreate dynamic tables for postback
            if (phDetail.Controls.Count == 0)
            {
                createDetailForPostback();
            }
            if (phRef.Controls.Count == 0)
            {
                createRefItemsForPostback();
            }
            if (phNotes.Controls.Count == 0)
            {
                createNoteItemsForPostback();
            }

            //set auto line number fields
            string sOldDetailCount = (string)Session["detailCount"];
            int iOldDetailCount = Convert.ToInt32(sOldDetailCount);
            //get the table
            Table detailTable = (Table)phDetail.Controls[0];
            //set the line numbers
            int iLineNumber = 1;
            for (int x = 1; x <= iOldDetailCount; x++)
            {
                if (!detailTable.Rows[x].Cells[0].Text.Equals("D"))
                {
                    detailTable.Rows[x].Cells[0].Text = Convert.ToString(iLineNumber++);
                }
            }
        }

        protected void btnaddReferenceItem_Click(object sender, EventArgs e)
        {
            //handled in OnInit
        }

        protected void hdnDeleteIndex_ValueChanged(object sender, EventArgs e)
        {
            //handled in Load_Page
        }

        protected void hdnDeleteNotesIndex_ValueChanged(object sender, EventArgs e)
        {
            //handled in Load_Page
        }

        protected void btnAddDetail_Click(object sender, EventArgs e)
        {
            //handled in OnInit
        }

        protected void btnAddNote_Click(object sender, EventArgs e)
        {
            //handled in OnInit
        }

        protected void btnWarningSubmit_Click(object sender, EventArgs e)
        {
            processSubmit(sender, e, true);
        }

        protected void Submit_Click(object sender, EventArgs e)
        {
            processSubmit(sender, e, false);
        }

        private void processSubmit(object sender, EventArgs e, bool bSubmitWithWarning)
        {
            bool isValid;
            List<CustomError> alWarningErrors = this.ValidatePage(out isValid);
            btnWarningSubmit.Visible = false;

            //if there were no errors 
            if (isValid)
            {
                //check for warnings
                if (alWarningErrors.Count > 0)
                {
                    //if Submit with Warnings was not clicked
                    if (bSubmitWithWarning == false)
                    {
                        //fill the error control list with the warnings
                        foreach (CustomError warning in alWarningErrors)
                        {
                            Validators.Add(warning);
                            btnWarningSubmit.Visible = true;
                        }
                        Validators.Add(new CustomError("One or more Warnings exist.  Click 'Submit with Warnings' to continue processing order."));
                        alWarningErrors.Clear();
                        return;
                    }
                }
            }
            else //return so that the user can correct the errors
            {
                return;
            }

            //count valid detail records
            Table detailTable = (Table)phDetail.Controls[0];
            int iValidNumberOfDetailRecords = 0;
            string sCount = (string)Session["detailCount"];
            int iDetailCount = Convert.ToInt32(sCount);
            for (int x = 1; x <= iDetailCount; x++)
            {
                if (detailTable.Rows[x].Visible == false)
                {
                    continue;
                }
                iValidNumberOfDetailRecords++;
            }

            //count valid note records (changed from note to comments mid-project)
            Table notesTable = (Table)phNotes.Controls[0];
            int iValidNumberOfNoteRecords = 0;
            string sNotesCount = (string)Session["notesCount"];
            int iNotesCount = Convert.ToInt32(sNotesCount);
            for (int x = 1; x <= iNotesCount; x++)
            {
                if (notesTable.Rows[x].Visible == false)
                {
                    continue;
                }
                iValidNumberOfNoteRecords++;
            }

            //count valid header reference records
            Table refTable = (Table)phRef.Controls[0];
            int iValidNumberOfHeaderReferenceRecords = 0;
            string sRefCount = (string)Session["referenceCount"];
            int iRefCount = Convert.ToInt32(sRefCount);
            for (int x = 1; x <= iRefCount; x++)
            {
                if (refTable.Rows[x].Visible == false)
                {
                    continue;
                }
                iValidNumberOfHeaderReferenceRecords++;
            }

            //instantiate xsd.exe generated objects for xml creation from GUI form
            warehouse_shipping_orders ordersRootElement = new warehouse_shipping_orders();
            warehouse_shipping_ordersWarehouse_shipping_order order = new warehouse_shipping_ordersWarehouse_shipping_order();
            warehouse_shipping_ordersWarehouse_shipping_order[] orderArray = ordersRootElement.warehouse_shipping_order;
            orderArray = new warehouse_shipping_ordersWarehouse_shipping_order[1];
            order.order_detail = new detail_type[iValidNumberOfDetailRecords];
            order.header_reference = new header_reference[iValidNumberOfHeaderReferenceRecords];
            order.note = new note[iValidNumberOfNoteRecords];

            //set account information from session
            if (Session["AccountId"] != null)
                order.account_id = (string)Session["AccountId"];
            if (Session["AccountNumber"] != null)
                order.account_number = (string)Session["AccountNumber"];

            orderArray[0] = order;
            ordersRootElement.warehouse_shipping_order = orderArray;
            address_type shipFrom = new address_type();
            address_type shipTo = new address_type();
            //weight is require. It will be filled in.
            total_weight totalWeight = new total_weight();
            ordersRootElement.warehouse_shipping_order[0].total_weight = totalWeight;
            //volume is optional test before creating the container
            //object
            if (tbTotalVolume.Text.Length > 0)
            {
                total_volume totalVolume = new total_volume();
                ordersRootElement.warehouse_shipping_order[0].total_volume = totalVolume;
            }
            address_type[] addresses = order.address;
            addresses = new address_type[2];
            addresses[0] = shipFrom;
            addresses[1] = shipTo;
            shipFrom.type = type.SHIPFROM;
            shipTo.type = type.SHIPTO;
            order.address = addresses;

            set_ddlStatus(order);
            order.customer_reference_number = tbCustomerRef.Text;
            order.consignee_po = tbConsigneePO.Text;
            set_ddlExport(order);
            set_ddlTempControl(order);
            set_ddlHazmat(order);
            set_ddlPaymentMethod(order);
            set_ddlTransportMethod(order);
            order.scac = ddlSCAC.SelectedValue;
            set_totalWeightFields(order);
            set_totalVolumeFields(order);
            //ship from
            shipFrom.name = tbShipFromName.Text;
            shipFrom.address1 = tbShipFromAddress1.Text;
            if (tbShipFromAddress2.Text.Length > 0)
                shipFrom.address2 = tbShipFromAddress2.Text;
            shipFrom.city = tbCity.Text;
            shipFrom.state_province = tbShipFromState.Text;
            shipFrom.postal_code = tbPostalCode.Text;

            //ship date
            DateTime dtShipDate = new DateTime();
            dtShipDate = Convert.ToDateTime(tbShipDate.Text);
            order.shipment_date = dtShipDate.ToString("yyyyMMdd");

            //ship to
            shipTo.name = tbShipToName.Text;
            shipTo.address1 = tbShipToAddress1.Text;
            if (tbShipToAddress2.Text.Length > 0)
                shipTo.address2 = tbShipToAddress2.Text;
            shipTo.city = tbShipToCity.Text;
            shipTo.state_province = tbShipToState.Text;
            shipTo.postal_code = tbShipToPostalCode.Text;

            //delivery date
            DateTime dtDeliveryDate = new DateTime();
            dtDeliveryDate = Convert.ToDateTime(tbDeliveryDate.Text);
            order.delivery_date = dtDeliveryDate.ToString("yyyyMMdd");

            //asn
            if (ddlAsn.SelectedValue.Equals("Y"))
            {
                order.asn = new asn();
                order.asn.id = tbAsnId.Text;
                order.asn.lu = tbAsnLu.Text;
                order.asn.mr = tbAsnMr.Text;
                order.asn.dp = tbAsnDp.Text;
            }

            order.ucc128 = ddlUCC128.SelectedValue;

            //detail
            detail_type eachDetail = null;
            TableCellCollection eachDetailTableCells = null;
            int iOrderDetailIndex = 0;
            for (int x = 1; x <= iDetailCount; x++)
            {
                if (detailTable.Rows[x].Visible == false)
                {
                    continue;
                }
                eachDetail = new detail_type();
                eachDetailTableCells = detailTable.Rows[x].Cells;
                eachDetail.line_number = eachDetailTableCells[(int)DetailCells.LINE_NUMBER].Text;

                TextBox tbProdNumber = (TextBox)eachDetailTableCells[(int)DetailCells.PRODUCT_NUMBER].Controls[0];
                eachDetail.product_number_or_nmfc = tbProdNumber.Text;

                TextBox tbLot = (TextBox)eachDetailTableCells[(int)DetailCells.LOT_NUMBER].Controls[0];
                if (tbLot.Text.Length > 0)
                {
                    eachDetail.lot_number = tbLot.Text;
                }

                TextBox tbQuantity = (TextBox)eachDetailTableCells[(int)DetailCells.QUANTITY].Controls[0];
                eachDetail.quantity = tbQuantity.Text;

                TextBox tbCube = (TextBox)eachDetailTableCells[(int)DetailCells.DETAIL_CUBE].Controls[0];
                if (tbCube.Text.Length > 0)
                {
                    eachDetail.detail_cube = Convert.ToDecimal(tbCube.Text);
                    eachDetail.detail_cubeSpecified = true;
                }

                TextBox tbDetWeight = (TextBox)eachDetailTableCells[(int)DetailCells.DETAIL_WEIGHT].Controls[0];
                if (tbDetWeight.Text.Length > 0)
                {
                    eachDetail.detail_weight = Convert.ToDecimal(tbDetWeight.Text);
                    eachDetail.detail_weightSpecified = true;
                }

                DropDownList ddlDetailRefQualifier =
                    (DropDownList)(eachDetailTableCells[(int)DetailCells.DETAIL_REF_QUALIFIER].Controls[0]);
                if (ddlDetailRefQualifier.SelectedValue.Length > 0)
                {
                    eachDetail.reference = new reference();
                    eachDetail.reference.qualifier = ddlDetailRefQualifier.SelectedValue;
                    TextBox ref_number = (TextBox)eachDetailTableCells[(int)DetailCells.DETAIL_REF_NUMBER].Controls[0];
                    eachDetail.reference.number = ref_number.Text;
                    TextBox ref_desc = (TextBox)eachDetailTableCells[(int)DetailCells.DETAIL_REF_DESCRIPTION].Controls[0];
                    eachDetail.reference.description = ref_desc.Text;
                }

                order.order_detail[iOrderDetailIndex++] = eachDetail;
            }

            //header reference
            header_reference eachHeaderReference = null;
            TableCellCollection eachHeaderRefTableCells = null;
            int iOrderHeaderRefIndex = 0;
            for (int x = 1; x <= iRefCount; x++)
            {
                if (refTable.Rows[x].Visible == false)
                {
                    continue;
                }
                eachHeaderRefTableCells = refTable.Rows[x].Cells;
                DropDownList ddlHeaderRefQualifier =
                    (DropDownList)(eachHeaderRefTableCells[(int)HeaderReferenceCells.QUALIFIER].Controls[0]);
                if (ddlHeaderRefQualifier.SelectedValue.Length > 0)
                {
                    eachHeaderReference = new header_reference();
                    eachHeaderReference.qualifier = ddlHeaderRefQualifier.SelectedValue;
                    TextBox headerRefDesc = (TextBox)eachHeaderRefTableCells[(int)HeaderReferenceCells.DESCRIPTION].Controls[0];
                    eachHeaderReference.description = headerRefDesc.Text;
                }
                order.header_reference[iOrderHeaderRefIndex++] = eachHeaderReference;
            }

            //check for duplicate header_reference qualifiers
            int y = 0;
            for (int x = 0; x < order.header_reference.Length; x++)
            {   
                y = x+1;
                for (; y < order.header_reference.Length; y++)
                {
                    if (order.header_reference[x].qualifier.Equals(
                        order.header_reference[y].qualifier))
                    {
                        Validators.Add(new CustomError("Duplicate Reference Type found. Only one of each type allowed."));
                        return;
                    }
                }
            }

            //notes
            note eachNote = null;
            TableCellCollection eachNoteTableCells = null;
            int iOrderNoteIndex = 0;
            for (int x = 1; x <= iNotesCount; x++)
            {
                if (notesTable.Rows[x].Visible == false)
                {
                    continue;
                }
                eachNoteTableCells = notesTable.Rows[x].Cells;
                DropDownList ddlNotesQualifier =
                    (DropDownList)(eachNoteTableCells[(int)NoteCells.QUALIFIER].Controls[0]);
                if (ddlNotesQualifier.SelectedValue.Length > 0)
                {
                    eachNote = new note();
                    eachNote.qualifier = ddlNotesQualifier.SelectedValue;
                    TextBox noteDesc = (TextBox)eachNoteTableCells[(int)NoteCells.DESCRIPTION].Controls[0];
                    eachNote.description = noteDesc.Text;
                }
                order.note[iOrderNoteIndex++] = eachNote;
            }

            //get path for writing generated xml documents
            string sXmlDocumentFolder =
            (string)ConfigurationManager.AppSettings["xml_document_folder"];
            string sXmlDocumentPath = Server.MapPath(sXmlDocumentFolder);
            try
            {
                DirectoryInfo dir = new DirectoryInfo(sXmlDocumentPath);
                if (!dir.Exists)
                {
                    Validators.Add(new CustomError("Invalid xml_document_folder entry in web.config"));
                    return;
                }
            }
            catch (Exception direx)
            {
                Validators.Add(new CustomError("Please contact customer support. Error code 9"));
                EventLog woeLog = new EventLog("WebOrderEntry");
                woeLog.Source = "WebOrderEntryApp";
                string errorMessage = "Message\r\n" +
                    direx.Message.ToString() + "\r\n\r\n";
                errorMessage += "Source\r\n" +
                    direx.Source + "\r\n\r\n";
                errorMessage += "Target site\r\n" +
                    direx.TargetSite.ToString() + "\r\n\r\n";
                errorMessage += "Stack trace\r\n" +
                    direx.StackTrace + "\r\n\r\n";
                errorMessage += "ToString()\r\n\r\n" +
                    direx.ToString();
                woeLog.WriteEntry(errorMessage, EventLogEntryType.Error, 9);
                return;
            }

            string sGeneratedXmlDocument = sXmlDocumentPath + "\\" +
                "EnterOrder.xml";
            string sUniqueGeneratedXmlDocument =
                GetUniqueFilename(sGeneratedXmlDocument);

            XmlSerializer s = new XmlSerializer(ordersRootElement.GetType());
            try
            {
                using (TextWriter writer = File.CreateText(sUniqueGeneratedXmlDocument))
                {
                    s.Serialize(writer, ordersRootElement);
                    //writer.Close();    //Autodisposed by the "using" statement
                    Session["lblStatus"] = "Order success";
                    Response.Redirect("EnterOrder.aspx");
                    //String xyz = writer.ToString();
                    //Session["xmlForProduction"] = xyz;
                }
            }
            catch (Exception txtWriterEx)
            {
                Validators.Add(new CustomError("Please contact customer support. Error code 9"));
                EventLog woeLog = new EventLog("WebOrderEntry");
                woeLog.Source = "WebOrderEntryApp";
                string errorMessage = "Message\r\n" +
                    txtWriterEx.Message.ToString() + "\r\n\r\n";
                errorMessage += "Source\r\n" +
                    txtWriterEx.Source + "\r\n\r\n";
                errorMessage += "Target site\r\n" +
                    txtWriterEx.TargetSite.ToString() + "\r\n\r\n";
                errorMessage += "Stack trace\r\n" +
                    txtWriterEx.StackTrace + "\r\n\r\n";
                errorMessage += "ToString()\r\n\r\n" +
                    txtWriterEx.ToString();
                woeLog.WriteEntry(errorMessage, EventLogEntryType.Error, 9);
                return;
            }
            //create xml from xsd object
            //XmlSerializer s = new XmlSerializer(ordersRootElement.GetType());
            //using (StringWriter writer = new StringWriter())
            //{
            //    s.Serialize(writer, ordersRootElement);
            //    String xyz = writer.ToString();
            //    Session["xmlForProduction"] = xyz;
            //}

            //Response.Redirect("XmlDisplay.aspx", true);

        }

        private void set_ddlStatus(warehouse_shipping_ordersWarehouse_shipping_order order)
        {
            if (DropDownListStatus.SelectedValue.Equals("N"))
            {
                order.status = status.N;
            }
            if (DropDownListStatus.SelectedValue.Equals("R"))
            {
                order.status = status.R;
            }
            if (DropDownListStatus.SelectedValue.Equals("F"))
            {
                order.status = status.F;
            }
        }

        private void set_ddlExport(warehouse_shipping_ordersWarehouse_shipping_order order)
        {
            if (ddlExport.SelectedValue.Equals("N"))
            {
                order.export = export.N;
            }
            if (ddlExport.SelectedValue.Equals("Y"))
            {
                order.export = export.Y;
            }
        }

        private void set_ddlTempControl(warehouse_shipping_ordersWarehouse_shipping_order order)
        {
            if (ddlTempControl.SelectedValue.Equals("N"))
            {
                order.temperature_controlled = temperature_controlled.N;
            }
            if (ddlTempControl.SelectedValue.Equals("Y"))
            {
                order.temperature_controlled = temperature_controlled.Y;
            }
        }

        private void set_ddlHazmat(warehouse_shipping_ordersWarehouse_shipping_order order)
        {
            if (ddlHazmat.SelectedValue.Equals("N"))
            {
                order.hazmat = hazmat.N;
            }
            if (ddlTempControl.SelectedValue.Equals("Y"))
            {
                order.hazmat = hazmat.Y;
            }
        }

        private void set_ddlPaymentMethod(warehouse_shipping_ordersWarehouse_shipping_order order)
        {
            if (DropDownListStatus.SelectedValue.Equals("PP"))
            {
                order.payment_method = payment_method.PP;
            }
            if (DropDownListStatus.SelectedValue.Equals("CC"))
            {
                order.payment_method = payment_method.CC;
            }
            if (DropDownListStatus.SelectedValue.Equals("PC"))
            {
                order.payment_method = payment_method.PC;
            }
        }

        private void set_ddlTransportMethod(warehouse_shipping_ordersWarehouse_shipping_order order)
        {
            if (ddlTransportMethod.SelectedValue.Equals("M"))
            {
                order.transport_method = transport_method.M;
            }
            if (ddlTransportMethod.SelectedValue.Equals("U"))
            {
                order.transport_method = transport_method.U;
            }
        }

        private void set_totalWeightFields(warehouse_shipping_ordersWarehouse_shipping_order order)
        {
            if (tbTotalWeight.Text.Length > 0)
            {
                order.total_weight.weight = Convert.ToDecimal(tbTotalWeight.Text);
                order.total_weight.weight_unit_of_measure = weight_unit_of_measure.LB;
            }
        }

        private void set_totalVolumeFields(warehouse_shipping_ordersWarehouse_shipping_order order)
        {
            if (tbTotalVolume.Text.Length > 0)
            {
                order.total_volume.volume = Convert.ToDecimal(tbTotalVolume.Text);
                order.total_volume.volume_unit_of_measure = volume_unit_of_measure.CF;
            }
        }

        private void addDetailTable()
        {
            Table tblDetailTable = new Table();
            tblDetailTable.ID = "tblDetailTable";
            tblDetailTable.CssClass = "dynamicTable";
            TableHeaderRow thrDetailTable = new TableHeaderRow();
            TableHeaderCell thcLine = new TableHeaderCell();
            thcLine.Text = "Line";
            thrDetailTable.Cells.Add(thcLine);

            TableHeaderCell thcProduct = new TableHeaderCell();
            thcProduct.Text = "Product#/NMFC";
            thrDetailTable.Cells.Add(thcProduct);

            TableHeaderCell thcQuantity = new TableHeaderCell();
            thcQuantity.Text = "Qty";
            thrDetailTable.Cells.Add(thcQuantity);

            TableHeaderCell thcLot = new TableHeaderCell();
            thcLot.Text = "Lot";
            thrDetailTable.Cells.Add(thcLot);

            TableHeaderCell thcCube = new TableHeaderCell();
            thcCube.Text = "Cube";
            thrDetailTable.Cells.Add(thcCube);

            TableHeaderCell thcWeight = new TableHeaderCell();
            thcWeight.Text = "Weight";
            thrDetailTable.Cells.Add(thcWeight);

            TableHeaderCell thcDetaiRefType = new TableHeaderCell();
            thcDetaiRefType.Text = "Ref Type";
            thrDetailTable.Cells.Add(thcDetaiRefType);

            TableHeaderCell thcDetaiRefNumber = new TableHeaderCell();
            thcDetaiRefNumber.Text = "Ref#";
            thrDetailTable.Cells.Add(thcDetaiRefNumber);

            TableHeaderCell thcDetaiRefDescription = new TableHeaderCell();
            thcDetaiRefDescription.Text = "Ref Description";
            thrDetailTable.Cells.Add(thcDetaiRefDescription);

            TableHeaderCell thcDetailDelete = new TableHeaderCell();
            thcDetailDelete.Text = "";
            thrDetailTable.Cells.Add(thcDetailDelete);
            tblDetailTable.Controls.Add(thrDetailTable);
            phDetail.Controls.Add(tblDetailTable);
        }

        private void addRowToDetailTable(int addAtIndex)
        {
            int iAddAtTableIndex = addAtIndex;
            string sAddAtTableIndex = Convert.ToString(iAddAtTableIndex);

            TableRow eachPreviouslyAddedRow = new TableRow();

            //create the static auto fill line number
            TableCell cellLineNumber = new TableCell();
            cellLineNumber.Width = new Unit(30);
            if (!cellLineNumber.Text.Equals("D"))
            {
                cellLineNumber.Text = sAddAtTableIndex;
            }
            eachPreviouslyAddedRow.Cells.Add(cellLineNumber);

            //detail product number
            TableCell cellProductNumber = new TableCell();
            TextBox tbProductNumber = new TextBox();
            tbProductNumber.Width = new Unit(150);
            tbProductNumber.ID = TB_DETAIL_PRODUCT_NUMBER_PREFIX + sAddAtTableIndex;
            tbProductNumber.CssClass = "requiredField";
            tbProductNumber.TabIndex = 33;
            tbProductNumber.MaxLength = 30;

            cellProductNumber.Controls.Add(tbProductNumber);
            RequiredFieldValidator rfvDetailProduct = new RequiredFieldValidator();
            rfvDetailProduct.ID = "rfvDetailProduct" + sAddAtTableIndex;
            rfvDetailProduct.ControlToValidate = tbProductNumber.ID;
            rfvDetailProduct.ErrorMessage = "Product#/NMFC required";
            rfvDetailProduct.ToolTip = "Product#/NMFC required";
            rfvDetailProduct.Text = "*";
            cellProductNumber.Controls.Add(rfvDetailProduct);
            eachPreviouslyAddedRow.Cells.Add(cellProductNumber);

            //detail quantity
            TableCell cellQuantity = new TableCell();
            TextBox tbQuantityNumber = new TextBox();
            tbQuantityNumber.Width = new Unit(30);
            tbQuantityNumber.ID = TB_DETAIL_QUANTITY_PREFIX + sAddAtTableIndex;
            tbQuantityNumber.CssClass = "requiredField";
            tbQuantityNumber.TabIndex = 33;
            tbQuantityNumber.MaxLength = 9;
            cellQuantity.Controls.Add(tbQuantityNumber);
            RequiredFieldValidator rfvQuantity = new RequiredFieldValidator();
            rfvQuantity.ID = "rfvQuantity" + sAddAtTableIndex;
            rfvQuantity.ControlToValidate = tbQuantityNumber.ID;
            rfvQuantity.ErrorMessage = "Quantity required";
            rfvQuantity.ToolTip = "Quantity required";
            rfvQuantity.Text = "*";
            cellQuantity.Controls.Add(rfvQuantity);
            RegularExpressionValidator revQuantity = new RegularExpressionValidator();
            revQuantity.ControlToValidate = tbQuantityNumber.ID;
            revQuantity.ID = "revQuantity" + sAddAtTableIndex;
            revQuantity.ErrorMessage = "Detail quantity must be numeric greater than zero.";
            revQuantity.ToolTip = "Detail quantity must be numeric greater than zero.";
            revQuantity.ValidationExpression = "^[1-9]+[0-9]*$";
            revQuantity.Text = "*";
            cellQuantity.Controls.Add(revQuantity);
            eachPreviouslyAddedRow.Cells.Add(cellQuantity);

            //detail lot
            TableCell cellLot = new TableCell();
            TextBox tbLot = new TextBox();
            tbLot.Width = new Unit(75);
            tbLot.ID = TB_DETAIL_LOT_PREFIX + sAddAtTableIndex;
            tbLot.TabIndex = 33;
            tbLot.MaxLength = 12;
            //cellLot.Text = aryhdnLot[iHiddenArrayIndex];
            cellLot.Controls.Add(tbLot);
            eachPreviouslyAddedRow.Cells.Add(cellLot);

            //cube
            TableCell cellCube = new TableCell();
            TextBox tbCube = new TextBox();
            tbCube.Width = new Unit(45);
            tbCube.ID = TB_DETAIL_CUBE_PREFIX + sAddAtTableIndex;
            tbCube.TabIndex = 33;
            tbCube.MaxLength = 9;
            cellCube.Controls.Add(tbCube);
            RegularExpressionValidator revDetailCube = new RegularExpressionValidator();
            revDetailCube.ControlToValidate = tbCube.ID;
            revDetailCube.ID = "revDetailCube" + sAddAtTableIndex;
            revDetailCube.ErrorMessage = "Detail weight must be numeric.";
            revDetailCube.ToolTip = "Detail volume must be numeric decimal.";
            revDetailCube.ValidationExpression = "^[-+]?[0-9]*\\.?[0-9]+$";
            revDetailCube.Text = "*";
            cellCube.Controls.Add(revDetailCube);
            RangeValidator rvDetailCube = new RangeValidator();
            rvDetailCube.ControlToValidate = tbCube.ID;
            rvDetailCube.ID = "rvDetailCube" + sAddAtTableIndex;
            rvDetailCube.ErrorMessage = "Detail cube must be 0 - 9999999.99";
            rvDetailCube.ToolTip = "Detail cube must be 0 - 9999999.99";
            rvDetailCube.MaximumValue = "9999999.99";
            rvDetailCube.MinimumValue = "0";
            rvDetailCube.Text = "*";
            cellCube.Controls.Add(rvDetailCube);
            eachPreviouslyAddedRow.Cells.Add(cellCube);

            //detail weight
            TableCell cellWeight = new TableCell();
            TextBox tbWeight = new TextBox();
            tbWeight.Width = new Unit(45);
            tbWeight.ID = TB_DETAIL_WEIGHT_PREFIX + sAddAtTableIndex;
            tbWeight.TabIndex = 33;
            tbWeight.MaxLength = 10;
            cellWeight.Controls.Add(tbWeight);
            //add validators
            RegularExpressionValidator revDetailWeight = new RegularExpressionValidator();
            revDetailWeight.ControlToValidate = tbWeight.ID;
            revDetailWeight.ID = "revDetailWeight" + sAddAtTableIndex;
            revDetailWeight.ErrorMessage = "Detail weight must be numeric.";
            revDetailWeight.ToolTip = "Detail weight must be numeric.";
            revDetailWeight.ValidationExpression = "[0-9]+";
            revDetailWeight.Text = "*";
            cellWeight.Controls.Add(revDetailWeight);
            RangeValidator rvDetailWeight = new RangeValidator();
            rvDetailWeight.ControlToValidate = tbWeight.ID;
            rvDetailWeight.ID = "rvDetailWeight" + sAddAtTableIndex;
            rvDetailWeight.ErrorMessage = "Detail weight must 0-9999999999.";
            rvDetailWeight.ToolTip = "Detail weight must be 0-9999999999.";
            rvDetailWeight.MaximumValue = "9999999999";
            rvDetailWeight.MinimumValue = "0";
            rvDetailWeight.Text = "*";
            cellWeight.Controls.Add(rvDetailWeight);

            eachPreviouslyAddedRow.Cells.Add(cellWeight);

            //detail type
            //a javascript jsDetailRefMethodCall is added to this control
            //after it is inserted into the ContentPlaceholder (end of this method)
            //that is done so that the ClientIds for the controls the 
            //script references will be correct.
            TableCell cellDetailReferenceType = new TableCell();
            DropDownList ddlDetailReferenceType = new DropDownList();
            ddlDetailReferenceType.Width = new Unit(100);
            ddlDetailReferenceType.ID = DDL_DETAIL_REF_TYPE_PREFIX + sAddAtTableIndex;
            ddlDetailReferenceType.Items.Add(new ListItem("", "", true));
            ddlDetailReferenceType.AppendDataBoundItems = true;
            ddlDetailReferenceType.DataTextField = "REF_ID";
            ddlDetailReferenceType.DataValueField = "REF_ID";
            ddlDetailReferenceType.DataSourceID = "SqlDataSourceDetailRef";
            ddlDetailReferenceType.TabIndex = 33;
            cellDetailReferenceType.Controls.Add(ddlDetailReferenceType);
            eachPreviouslyAddedRow.Cells.Add(cellDetailReferenceType);

            //detail ref number
            TableCell cellDetailReferenceNumber = new TableCell();

            TextBox tbDetailReferenceNumber = new TextBox();
            //TextBox tbDetailReferenceNumber = new TextBox();
            tbDetailReferenceNumber.Width = new Unit(65);
            tbDetailReferenceNumber.ID = TB_DETAIL_REF_NUMBER_PREFIX + sAddAtTableIndex;
            tbDetailReferenceNumber.TabIndex = 33;
            tbDetailReferenceNumber.MaxLength = 30;
            cellDetailReferenceNumber.Controls.Add(tbDetailReferenceNumber);
            RequiredFieldValidator rfvRefNumber = new RequiredFieldValidator();
            rfvRefNumber.ControlToValidate = tbDetailReferenceNumber.ID;
            rfvRefNumber.ID = "rfvRefNumber" + sAddAtTableIndex;
            rfvRefNumber.ErrorMessage = "Reference Number required";
            rfvRefNumber.ToolTip = "Reference Number required";
            rfvRefNumber.Enabled = false;
            //rfvQuantity.ValidationGroup = "vgDetail";
            rfvRefNumber.Text = "*";
            cellDetailReferenceNumber.Controls.Add(rfvRefNumber);
            eachPreviouslyAddedRow.Cells.Add(cellDetailReferenceNumber);

            //detail ref desc
            TableCell cellDetailReferenceDescription = new TableCell();
            TextBox tbDetailReferenceDescription = new TextBox();
            tbDetailReferenceDescription.Width = new Unit(150);
            tbDetailReferenceDescription.ID = TB_DETAIL_REF_DESC_PREFIX + sAddAtTableIndex;
            tbDetailReferenceDescription.TabIndex = 33;
            tbDetailReferenceDescription.MaxLength = 45;
            cellDetailReferenceDescription.Controls.Add(tbDetailReferenceDescription);
            eachPreviouslyAddedRow.Cells.Add(cellDetailReferenceDescription);

            TableCell cellDetailDelete = new TableCell();
            ImageButton imgbtnDetailDelete = new ImageButton();
            imgbtnDetailDelete.ID = "imgbtnDetailDelete" + sAddAtTableIndex;
            imgbtnDetailDelete.TabIndex = 33;
            imgbtnDetailDelete.ImageUrl = "images/delete.png";
            imgbtnDetailDelete.CausesValidation = false;
            imgbtnDetailDelete.ToolTip = "Delete";

            //add client-side javascript control events
            //user clicks the X delete button, fires off this javascript
            //(which is located on Site1.Master page), the javascript
            //function sets the hdnDeleteIndex hidden input field,
            //that causes the hdnDeleteIndex onvaluechanged event to get
            //fired.  The onvaluechanged event is registered as an
            //AsyncPostBackTrigger in UpdatePanel7.  OnLoad in the
            //codebehind tests for a value in the delete index and 
            //the row visible attribute gets set to false
            String jsMethodCall = "setDetailDeleteIndex('" +
                sAddAtTableIndex + "','" + hdnDeleteIndex.ClientID + "');";
            imgbtnDetailDelete.Attributes.Add("OnClick", jsMethodCall);

            cellDetailDelete.Controls.Add(imgbtnDetailDelete);
            eachPreviouslyAddedRow.Cells.Add(cellDetailDelete);

            phDetail.Controls[0].Controls.Add(eachPreviouslyAddedRow);

            //none of the ClientIDs will be correct until all 
            //of the controls are aded to the placeholder, that is why 
            //this javascript method is setup here instead of above
            //when the control was actually created
            //String jsDetailRefMethodCall = "toggleEnableDetailRef(this,'" + 
            //  tbDetailReferenceNumber.ClientID + "','" +
            //  tbDetailReferenceDescription.ClientID + "','" + rfvRefNumber.ClientID +
            //  "');";
            //ddlDetailReferenceType.Attributes.Add("onchange", jsDetailRefMethodCall);
        }

        //creates the table and sets the deleted row's 
        //visible attribute to false and line number to "D"
        public void DeleteRefItemRow(int deleteRowIndex)
        {
            string sOldDetailCount = (string)Session["referenceCount"];
            int iOldDetailCount = Convert.ToInt32(sOldDetailCount);

            //create table and header row
            addReferenceTable();
            //intial index is set to one so that the new
            //rows will be added after the table header
            int x = 1;
            for (x = 1; x <= iOldDetailCount; x++)
            {
                addRowToReferenceTable(x);
            }

            Table refItemsTable = (Table)phRef.Controls[0];

            refItemsTable.Rows[deleteRowIndex].Visible = false;

        }

        //creates the table and sets the deleted row's 
        //visible attribute to false and line number to "D"
        public void DeleteNotesItemRow(int deleteRowIndex)
        {
            string sOldDetailCount = (string)Session["notesCount"];
            int iOldDetailCount = Convert.ToInt32(sOldDetailCount);

            //create table and header row
            addNotesTable();
            //intial index is set to one so that the new
            //rows will be added after the table header
            int x = 1;
            for (x = 1; x <= iOldDetailCount; x++)
            {
                addRowToNotesTable(x);
            }

            Table notesItemsTable = (Table)phNotes.Controls[0];

            notesItemsTable.Rows[deleteRowIndex].Visible = false;

        }

        //creates the table and sets the deleted row's 
        //visible attribute to false and line number to "D"
        public void DeleteRow(int deleteRowIndex)
        {
            string sOldDetailCount = (string)Session["detailCount"];
            int iOldDetailCount = Convert.ToInt32(sOldDetailCount);

            //create table and header row
            addDetailTable();
            //intial index is set to one so that the new
            //rows will be added after the table header
            int x = 1;
            for (x = 1; x <= iOldDetailCount; x++)
            {
                addRowToDetailTable(x);
            }

            Table detailTable = (Table)phDetail.Controls[0];

            detailTable.Rows[deleteRowIndex].Visible = false;
            detailTable.Rows[deleteRowIndex].Cells[0].Text = "D";

            //reassign the static line number fields
            int iLineNumber = 1;
            for (x = 1; x <= iOldDetailCount; x++)
            {
                if (!detailTable.Rows[x].Cells[0].Text.Equals("D"))
                {
                    detailTable.Rows[x].Cells[0].Text = Convert.ToString(iLineNumber++);
                }
            }
        }

        public Control GetPostBackControl(MasterPage page)
        {
            Control control = null;
            string ctrlName = page.Page.Request.Params.Get("__EVENTTARGET");
            if (((ctrlName != null) && (ctrlName != string.Empty)))
            {
                control = page.Page.FindControl(ctrlName);
            }
            else
            {
                foreach (string Item in page.Page.Request.Form)
                {
                    Control cl = page.Page.FindControl(Item);

                    if (((cl) is System.Web.UI.WebControls.Button))
                    {
                        control = cl;
                        break;
                    }
                }
            }

            return control;
        }

        protected void hdnRefDeleteIndex_ValueChanged(object sender, EventArgs e)
        {
        }

        private void addReferenceTable()
        {
            Table tblDetailTable = new Table();
            tblDetailTable.ID = "tblReferenceTable";
            tblDetailTable.CssClass = "dynamicTable";
            TableHeaderRow thrDetailTable = new TableHeaderRow();
            TableHeaderCell thcReferenceQualifier = new TableHeaderCell();
            thcReferenceQualifier.Text = "Ref";
            thrDetailTable.Cells.Add(thcReferenceQualifier);

            TableHeaderCell thcRefDescription = new TableHeaderCell();
            thcRefDescription.Text = "Description";
            thrDetailTable.Cells.Add(thcRefDescription);

            TableHeaderCell thcDetailDelete = new TableHeaderCell();
            thcDetailDelete.Text = "";
            thrDetailTable.Cells.Add(thcDetailDelete);
            tblDetailTable.Controls.Add(thrDetailTable);
            phRef.Controls.Add(tblDetailTable);
        }

        private void addRowToReferenceTable(int addAtIndex)
        {
            int iAddAtTableIndex = addAtIndex;
            string sAddAtTableIndex = Convert.ToString(iAddAtTableIndex);

            TableRow eachPreviouslyAddedRow = new TableRow();

            //reference qualifer
            TableCell cellReferenceQualifer = new TableCell();
            DropDownList ddlReferenceQualifer = new DropDownList();
            ddlReferenceQualifer.Width = new Unit(235);
            ddlReferenceQualifer.ID = DDL_HEADER_REF_QUALIFIER_PREFIX + sAddAtTableIndex;
            //add an empty item so that nothing is selected on init
            ddlReferenceQualifer.Items.Add(new ListItem("", "", true));
            //append the db items to the empty one
            ddlReferenceQualifer.AppendDataBoundItems = true;
            ddlReferenceQualifer.DataTextField = "REF_DESC";
            ddlReferenceQualifer.DataValueField = "REF_ID";
            ddlReferenceQualifer.DataSourceID = "SqlDataSourceRef";
            ddlReferenceQualifer.TabIndex = 26;

            ddlReferenceQualifer.CausesValidation = false;
            cellReferenceQualifer.Controls.Add(ddlReferenceQualifer);
            eachPreviouslyAddedRow.Cells.Add(cellReferenceQualifer);
            //<asp:DropDownList ID="ddlHeaderReference" runat="server" Style="margin-left: 0px"
            //    Width="138px" Height="22px" DataSourceID="SqlDataSourceRef" 
            //    DataTextField="VALUE" DataValueField="VALUE" CausesValidation="false" >
            //    <asp:ListItem Selected="True"></asp:ListItem>
            //</asp:DropDownList>

            //reference description
            TableCell cellReferenceDescription = new TableCell();
            TextBox tbReferenceDescription = new TextBox();
            tbReferenceDescription.Width = new Unit(90);
            tbReferenceDescription.ID = TB_HEADER_REF_DESC_PREFIX + sAddAtTableIndex;
            tbReferenceDescription.TabIndex = 26;
            tbReferenceDescription.MaxLength = 30;
            cellReferenceDescription.Controls.Add(tbReferenceDescription);
            eachPreviouslyAddedRow.Cells.Add(cellReferenceDescription);


            TableCell cellRefDelete = new TableCell();
            cellRefDelete.HorizontalAlign = HorizontalAlign.Left;
            ImageButton imgbtnDetailDelete = new ImageButton();

            imgbtnDetailDelete.ID = "imgbtnRefItemDelete" + sAddAtTableIndex;
            imgbtnDetailDelete.ImageUrl = "images/delete.png";
            imgbtnDetailDelete.CausesValidation = false;
            imgbtnDetailDelete.ToolTip = "Delete";
            imgbtnDetailDelete.TabIndex = 26;
            //imgbtnDetailDelete.ValidationGroup = "vgDetail";

            //add client-side javascript control events
            //user clicks the X delete button, fires off this javascript
            //(which is located on Site1.Master page), the javascript
            //function sets the hdnDeleteIndex hidden input field,
            //that causes the hdnDeleteIndex onvaluechanged event to get
            //fired.  The onvaluechanged event is registered as an
            //AsyncPostBackTrigger in UpdatePanel7.  OnLoad in the
            //codebehind tests for a value in the delete index and 
            //the row visible attribute gets set to false
            String jsMethodCall = "setDetailDeleteIndex('" +
                sAddAtTableIndex + "','" + hdnRefDeleteIndex.ClientID + "');";
            imgbtnDetailDelete.Attributes.Add("OnClick", jsMethodCall);

            cellRefDelete.Controls.Add(imgbtnDetailDelete);
            eachPreviouslyAddedRow.Cells.Add(cellRefDelete);

            phRef.Controls[0].Controls.Add(eachPreviouslyAddedRow);
        }

        private void addNotesTable()
        {
            Table tblDetailTable = new Table();
            tblDetailTable.ID = "tblNotesTable";
            tblDetailTable.CssClass = "dynamicTable";
            TableHeaderRow thrDetailTable = new TableHeaderRow();
            TableHeaderCell thcNoteQualifier = new TableHeaderCell();
            thcNoteQualifier.Text = "Comment Type";
            thrDetailTable.Cells.Add(thcNoteQualifier);

            TableHeaderCell thcNoteDescription = new TableHeaderCell();
            thcNoteDescription.Text = "Comment";
            thrDetailTable.Cells.Add(thcNoteDescription);

            TableHeaderCell thcDetailDelete = new TableHeaderCell();
            thcDetailDelete.Text = "";
            thrDetailTable.Cells.Add(thcDetailDelete);
            tblDetailTable.Controls.Add(thrDetailTable);
            phNotes.Controls.Add(tblDetailTable);
        }

        private void addRowToNotesTable(int addAtIndex)
        {
            int iAddAtTableIndex = addAtIndex;
            string sAddAtTableIndex = Convert.ToString(iAddAtTableIndex);

            TableRow eachPreviouslyAddedRow = new TableRow();

            //note qualifer
            TableCell cellNoteQualifier = new TableCell();
            DropDownList ddlNoteQualifer = new DropDownList();
            ddlNoteQualifer.Width = new Unit(85);
            ddlNoteQualifer.ID = DDL_NOTE_QUALIFIER_PREFIX + sAddAtTableIndex;
            ddlNoteQualifer.DataTextField = "INST_CODE";
            ddlNoteQualifer.DataValueField = "INST_CODE";
            ddlNoteQualifer.DataSourceID = "SqlDataSourceNotes";
            ddlNoteQualifer.TabIndex = 32;
            //add an empty item so that nothing is selected on init
            ddlNoteQualifer.Items.Add(new ListItem("", "", true));
            //append the db items to the empty one
            ddlNoteQualifer.AppendDataBoundItems = true;
            ddlNoteQualifer.CausesValidation = false;
            cellNoteQualifier.Controls.Add(ddlNoteQualifer);
            eachPreviouslyAddedRow.Cells.Add(cellNoteQualifier);

            //reference description
            TableCell cellNotesDescription = new TableCell();
            TextBox tbNotesDescription = new TextBox();
            tbNotesDescription.Width = new Unit(90);
            tbNotesDescription.ID = "tbNotesDescription" + sAddAtTableIndex;
            tbNotesDescription.TabIndex = 32;
            tbNotesDescription.MaxLength = 60;
            cellNotesDescription.Controls.Add(tbNotesDescription);
            eachPreviouslyAddedRow.Cells.Add(cellNotesDescription);

            TableCell cellNoteDelete = new TableCell();
            cellNoteDelete.HorizontalAlign = HorizontalAlign.Left;
            ImageButton imgbtnNotesDelete = new ImageButton();

            imgbtnNotesDelete.ID = "imgbtnNoteItemDelete" + sAddAtTableIndex;
            imgbtnNotesDelete.ImageUrl = "images/delete.png";
            imgbtnNotesDelete.CausesValidation = false;
            imgbtnNotesDelete.ToolTip = "Delete";
            imgbtnNotesDelete.TabIndex = 32;
            //imgbtnNotesDelete.ValidationGroup = "vgDetail";

            //add client-side javascript control events
            //user clicks the X delete button, fires off this javascript
            //(which is located on Site1.Master page), the javascript
            //function sets the hdnNotesDeleteIndex hidden input field,
            //that causes the hdnDeleteIndex onvaluechanged event to get
            //fired.  The onvaluechanged event is registered as an
            //AsyncPostBackTrigger in UpdatePaneNotes.  OnLoad in the
            //codebehind tests for a value in the delete index and 
            //the row visible attribute gets set to false
            String jsMethodCall = "setDetailDeleteIndex('" +
                sAddAtTableIndex + "','" + hdnDeleteNotesIndex.ClientID + "');";
            imgbtnNotesDelete.Attributes.Add("OnClick", jsMethodCall);

            cellNoteDelete.Controls.Add(imgbtnNotesDelete);
            eachPreviouslyAddedRow.Cells.Add(cellNoteDelete);

            phNotes.Controls[0].Controls.Add(eachPreviouslyAddedRow);

        }

        protected void ServerValidateEventHandler(object source, ServerValidateEventArgs args)
        { }

        //Validates the Enter Order form
        //if there are errors, the error control list will be propagated and
        //the bIsValid will be set to false
        //if there are warnings, a list list of warning messages 
        //will be returned
        private List<CustomError> ValidatePage(out bool bIsValid)
        {
            List<CustomError> alWarningErrors = new List<CustomError>();
            bIsValid = true;

            //get the account number
            if (!dbGetAccountNumber((string)Session["AccountId"], ddlLocationId.SelectedValue))
            {
                bIsValid = false;
                Validators.Add(new CustomError("Invalid AccountId and LocationID combination."));
                return alWarningErrors;
            }
            
            string sAccountNumber = (string)Session["AccountNumber"];
            int iAccountNumber = Convert.ToInt32(sAccountNumber);

            string connString = 
                ConfigurationManager.ConnectionStrings["WebOrderEntryConnectionString"].ConnectionString;

            try
            {
                using (SqlConnection myConnection = new SqlConnection(connString.ToString()))
                {
                    myConnection.Open();

                    //Client side validation check for required fields
                    //and valid ranges
                    string sDetailCount = (string)Session["detailCount"];
                    int iDetailCount = Convert.ToInt32(sDetailCount);
                    Table tblDetail = (Table)phDetail.Controls[0];
                    TableRowCollection trcRows = tblDetail.Rows;
                    SqlDataReader dr = null;

                    int iErrorDetailLineNumber = 1;
                    int iValidNumberOfDetailRecords = 0;
                    for (int x = 1; x <= iDetailCount; x++)
                    {
                        //if row is not visible that means it 
                        //has been deleted on the web page
                        if (trcRows[x].Visible == false)
                        {
                            continue;
                        }
                        iValidNumberOfDetailRecords++;
                        //validate ProductNumber(item number in db),Lot number, AcccountNumber
                        //using stored procedure
                        TextBox eachItemNumber = (TextBox)trcRows[x].FindControl(TB_DETAIL_PRODUCT_NUMBER_PREFIX + Convert.ToString(x));
                        TextBox eachLot = (TextBox)trcRows[x].FindControl(TB_DETAIL_LOT_PREFIX + Convert.ToString(x));
                        SqlCommand myCommand = new SqlCommand();
                        myCommand.Connection = myConnection;
                        myCommand.CommandText = "sp_ItemValidation";// sItemSelect;
                        myCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        myCommand.Parameters.Add("@ItemNumber", SqlDbType.VarChar, 22);
                        myCommand.Parameters.Add("@LotNumber", SqlDbType.VarChar, 12);
                        myCommand.Parameters.Add("@AccountNumber", SqlDbType.Int);

                        myCommand.Parameters["@ItemNumber"].Value = eachItemNumber.Text;
                        myCommand.Parameters["@LotNumber"].Value = eachLot.Text;
                        myCommand.Parameters["@AccountNumber"].Value = iAccountNumber;

                        dr = myCommand.ExecuteReader();
                        if (!dr.HasRows)
                        {
                            bIsValid = false;
                            string sValidatorMessage = "Invalid Product#/NMFC, Lot, AccountNumber combination - detail line " + 
                              Convert.ToString(iErrorDetailLineNumber) + ". Error code 10.";
                            Validators.Add(new CustomError(sValidatorMessage));
                            EventLog woeLog = new EventLog("WebOrderEntry");
                            woeLog.Source = "WebOrderEntryApp";
                            string errorMessage = "Message\r\n" +
                                sValidatorMessage + "\r\n";
                            errorMessage += "Product#/NMFC =" +
                                eachItemNumber.Text + "\r\n";
                            errorMessage += "Lot =" +
                                eachLot.Text + "\r\n";
                            errorMessage += "Account Number =" +
                                sAccountNumber + "\r\n\r\n";
                            woeLog.WriteEntry(errorMessage, EventLogEntryType.Error, 10);
                        }
                        dr.Close();

                        //validate detail ref type, number, desc
                        DropDownList eachDetailRefType = (DropDownList)trcRows[x].FindControl(DDL_DETAIL_REF_TYPE_PREFIX + Convert.ToString(x));
                        TextBox eachDetailRefNumber = (TextBox)trcRows[x].FindControl(TB_DETAIL_REF_NUMBER_PREFIX + Convert.ToString(x));
                        TextBox eachDetailRefDesc = (TextBox)trcRows[x].FindControl(TB_DETAIL_REF_DESC_PREFIX + Convert.ToString(x));
                        if (eachDetailRefType.SelectedValue.Length > 0)
                        {
                            if (eachDetailRefNumber.Text.Length == 0)
                            {
                                bIsValid = false;
                                Validators.Add(new CustomError("Reference Number required - detail line " + Convert.ToString(iErrorDetailLineNumber)));
                            }
                            if (eachDetailRefDesc.Text.Length == 0)
                            {
                                bIsValid = false;
                                Validators.Add(new CustomError("Reference Description required - detail line " + Convert.ToString(iErrorDetailLineNumber)));
                            }
                        }
                        //will only be incremented for a valid detail line
                        iErrorDetailLineNumber++;
                    }
                    if (iValidNumberOfDetailRecords == 0)
                    {
                        bIsValid = false;
                        Validators.Add(new CustomError("At least one detail line is required"));
                    }
                }
                //header reference validation
                string sHeaderRefDetailCount = (string)Session["referenceCount"];
                int iHeaderRefDetailCount = Convert.ToInt32(sHeaderRefDetailCount);
                Table tblHeaderRefDetail = (Table)phRef.Controls[0];
                TableRowCollection trcHeaderRefDetailRows = tblHeaderRefDetail.Rows;

                int iErrorHeaderRefDetailLineNumber = 1;
                for (int x = 1; x <= iHeaderRefDetailCount; x++)
                {
                    //if row is not visible that means it 
                    //has been deleted on the web page
                    if (trcHeaderRefDetailRows[x].Visible == false)
                    {
                        continue;
                    }
                    DropDownList eachHeaderRefQualifier = (DropDownList)trcHeaderRefDetailRows[x].FindControl(DDL_HEADER_REF_QUALIFIER_PREFIX + Convert.ToString(x));
                    TextBox eachHeaderRefDesc = (TextBox)trcHeaderRefDetailRows[x].FindControl(TB_HEADER_REF_DESC_PREFIX + Convert.ToString(x));
                    if (eachHeaderRefQualifier.SelectedValue.Length > 0)
                    {
                        if (eachHeaderRefDesc.Text.Length == 0)
                        {
                            bIsValid = false;
                            Validators.Add(new CustomError("Header Reference Description required - line " + Convert.ToString(iErrorHeaderRefDetailLineNumber)));
                        }
                    }
                    iErrorHeaderRefDetailLineNumber++;
                }

                //note validation
                string sNoteDetailCount = (string)Session["notesCount"];
                int iNoteDetailCount = Convert.ToInt32(sNoteDetailCount);
                Table tblNoteDetail = (Table)phNotes.Controls[0];
                TableRowCollection trcNoteDetailRows = tblNoteDetail.Rows;

                int iErrorNoteDetailLineNumber = 1;
                for (int x = 1; x <= iNoteDetailCount; x++)
                {
                    //if row is not visible that means it 
                    //has been deleted on the web page
                    if (trcNoteDetailRows[x].Visible == false)
                    {
                        continue;
                    }
                    DropDownList eachNoteQualifier = (DropDownList)trcNoteDetailRows[x].FindControl(DDL_NOTE_QUALIFIER_PREFIX + Convert.ToString(x));
                    TextBox eachNoteDesc = (TextBox)trcNoteDetailRows[x].FindControl(TB_NOTE_DESC_PREFIX + Convert.ToString(x));
                    if (eachNoteQualifier.SelectedValue.Length > 0)
                    {
                        if (eachNoteDesc.Text.Length == 0)
                        {
                            bIsValid = false;
                            Validators.Add(new CustomError("Comment required - line " + Convert.ToString(iErrorNoteDetailLineNumber)));
                        }
                    }
                    iErrorNoteDetailLineNumber++;
                }

                //ship to and ship from city, state, zip lookup
                using (SqlConnection myConnection = new SqlConnection(connString.ToString()))
                {
                    myConnection.Open();

                    SqlDataReader dr = null;
                    SqlCommand myCommand = new SqlCommand();
                    myCommand.Connection = myConnection;
                    myCommand.CommandText = "sp_CityStateZipValidation";// sItemSelect;
                    myCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    myCommand.Parameters.Add("@City", SqlDbType.VarChar, 30);
                    myCommand.Parameters.Add("@StateProvince", SqlDbType.VarChar, 2);
                    myCommand.Parameters.Add("@Zip", SqlDbType.VarChar, 10);

                    //validate ship from
                    myCommand.Parameters["@City"].Value = tbCity.Text.ToUpper();
                    myCommand.Parameters["@StateProvince"].Value =
                        tbShipFromState.Text.ToUpper();
                    myCommand.Parameters["@Zip"].Value = tbPostalCode.Text.ToUpper();


                    dr = myCommand.ExecuteReader();
                    if (!dr.HasRows)
                    {
                        bIsValid = false;
                        Validators.Add(new CustomError("Invalid ShipFrom City, State, PostalCode combination"));
                    }
                    dr.Close();

                    //validate ship to
                    myCommand.Parameters["@City"].Value = tbShipToCity.Text.ToUpper();
                    myCommand.Parameters["@StateProvince"].Value =
                        tbShipToState.Text.ToUpper();
                    myCommand.Parameters["@Zip"].Value = tbShipToPostalCode.Text.ToUpper();


                    dr = myCommand.ExecuteReader();
                    if (!dr.HasRows)
                    {
                        bIsValid = false;
                        Validators.Add(new CustomError("Invalid ShipTo City, State, PostalCode combination"));
                    }
                    dr.Close();

                }

                //validate ship date
                bool bValidDate = true;
                DateTime dtShipDate = new DateTime();
                try
                {
                    dtShipDate = Convert.ToDateTime(tbShipDate.Text);

                    if (dtShipDate.CompareTo(DateTime.Today) < 0)
                    {
                        bValidDate = false;
                        bIsValid = false;
                        Validators.Add(new CustomError("Invalid Ship Date.  Cannot be earlier than today's date."));
                    }

                    if (dtShipDate.DayOfWeek == DayOfWeek.Saturday ||
                        dtShipDate.DayOfWeek == DayOfWeek.Sunday)
                    {
                        alWarningErrors.Add(new CustomError("Warning: Ship Date is a Saturday or Sunday.  Extra charges may be applied."));
                    }
                    
                    if (dtShipDate.CompareTo(DateTime.Today) == 0)
                    {
                        alWarningErrors.Add(new CustomError("Warning: Ship Date is today's date.  Extra charges may be applied."));
                    }
                    
                }
                catch (FormatException fe)
                {
                    FormatException x = fe;
                    bValidDate = false;
                    bIsValid = false;
                    Validators.Add(new CustomError("Invalid Ship Date"));
                }

                //validate deliver date
                DateTime dtDeliverDate = new DateTime();
                try
                {
                    dtDeliverDate = Convert.ToDateTime(tbDeliveryDate.Text);
                }
                catch (FormatException fe)
                {
                    FormatException x = fe;
                    bValidDate = false;
                    bIsValid = false;
                    Validators.Add(new CustomError("Invalid Delivery Date"));
                }
                //validate ship date and deliver date
                if (bValidDate)
                {
                    int iCompareTest = dtShipDate.CompareTo(dtDeliverDate);
                    //if they are the same
                    if (iCompareTest == 0)
                    {
                        bIsValid = false;
                        Validators.Add(new CustomError("Invalid Ship Date and Delivery Date combination.  They cannot be the same."));
                    }
                    if (iCompareTest > 0)
                    {
                        bIsValid = false;
                        Validators.Add(new CustomError("Invalid Ship Date and Delivery Date combination.  Ship Date cannot be greater."));
                    }
                }

                //validate asn fields
                if (ddlAsn.SelectedValue.Equals("Y"))
                {
                    if (tbAsnId.Text.Length == 0)
                    {
                        bIsValid = false;
                        Validators.Add(new CustomError("ASN ID is required when ASN? = Yes."));
                    }
                    if (tbAsnLu.Text.Length == 0)
                    {
                        bIsValid = false;
                        Validators.Add(new CustomError("Location Number is required when ASN? = Yes."));
                    }
                    if (tbAsnMr.Text.Length == 0)
                    {
                        bIsValid = false;
                        Validators.Add(new CustomError("Merchadise Type Code is required when ASN? = Yes."));
                    }
                    if (tbAsnDp.Text.Length == 0)
                    {
                        bIsValid = false;
                        Validators.Add(new CustomError("Department Number is required when ASN? = Yes."));
                    }
                }

                if (ddlSCAC.SelectedValue.Length == 0)
                {
                    bIsValid = false;
                    Validators.Add(new CustomError("SCAC is required."));
                }



            }
            catch (SqlException e)
            {
                bIsValid = false;
                Validators.Add(new CustomError("Error validating data. Contact customer support. Error code 7"));
                EventLog woeLog = new EventLog("WebOrderEntry");
                woeLog.Source = "WebOrderEntryApp";
                string errorMessage = "Message\r\n" +
                    e.Message.ToString() + "\r\n\r\n";
                errorMessage += "Source\r\n" +
                    e.Source + "\r\n\r\n";
                errorMessage += "Target site\r\n" +
                    e.TargetSite.ToString() + "\r\n\r\n";
                errorMessage += "Stack trace\r\n" +
                    e.StackTrace + "\r\n\r\n";
                errorMessage += "ToString()\r\n\r\n" +
                    e.ToString();
                woeLog.WriteEntry(errorMessage, EventLogEntryType.Error, 7);
            }
            return alWarningErrors;
        }

        public string GetUniqueFilename(string FileName)
        {
            int count = 0;
            string Name = "";

            lock (this)
            {

                if (System.IO.File.Exists(FileName))
                {
                    System.IO.FileInfo f = new System.IO.FileInfo(FileName);

                    if (!string.IsNullOrEmpty(f.Extension))
                    {
                        Name = f.FullName.Substring(0, f.FullName.LastIndexOf('.'));
                    }
                    else
                    {
                        Name = f.FullName;
                    }

                    while (File.Exists(FileName))
                    {
                        count++;
                        FileName = Name + count.ToString() + f.Extension;
                    }
                }
            }
            return FileName;
        }

        //moved here release 1.0.0.2 02-16-2009 map 
        private bool dbGetAccountNumber(string sAccountId, string sLocationId)
        {
          bool isValid = true;

          string connString =
          ConfigurationManager.ConnectionStrings["WebOrderEntryConnectionString"].ConnectionString;
          try
          {
            using (SqlConnection myConnection = new SqlConnection(connString.ToString()))
            {
              myConnection.Open();

              SqlDataReader dr = null;

              SqlCommand myCommand = new SqlCommand();
              myCommand.Connection = myConnection;
              myCommand.CommandText = "sp_GetAccountNumber";// sItemSelect;
              myCommand.CommandType = System.Data.CommandType.StoredProcedure;
              myCommand.Parameters.Add("@account_id", SqlDbType.VarChar, 30);
              myCommand.Parameters.Add("@location_id", SqlDbType.VarChar, 30);

              myCommand.Parameters["@account_id"].Value = sAccountId;
              myCommand.Parameters["@location_id"].Value = sLocationId;

              dr = myCommand.ExecuteReader();
              if (!dr.HasRows)
              {
                isValid = false;
              }
              else
              {
                //set the session variable
                dr.Read();
                int iAccountNumber = (int)dr[0];
                string sAccountNumber = Convert.ToString(iAccountNumber);
                Session["AccountNumber"] = sAccountNumber;
              }
              dr.Close();
            }
          }
          catch (SqlException e)
          {
            isValid = false;
            EventLog woeLog = new EventLog("WebOrderEntry");
            woeLog.Source = "WebOrderEntryApp";
            string errorMessage = "Message\r\n" +
                e.Message.ToString() + "\r\n\r\n";
            errorMessage += "Source\r\n" +
                e.Source + "\r\n\r\n";
            errorMessage += "Target site\r\n" +
                e.TargetSite.ToString() + "\r\n\r\n";
            errorMessage += "Stack trace\r\n" +
                e.StackTrace + "\r\n\r\n";
            errorMessage += "ToString()\r\n\r\n" +
                e.ToString();
            woeLog.WriteEntry(errorMessage, EventLogEntryType.Error, 6);
          }
          return isValid;
        }

        protected void ddlLocationId_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlLocationId.SelectedValue.Length > 0)
            {
                LocationDataContext db = new LocationDataContext();

                var query = from l in db.LOCATION_MASTERs
                            where l.LOCATION_ID == ddlLocationId.SelectedValue
                            select l;
                var list = query.ToList();
                if (list.Count == 1)
                {
                    tbShipFromAddress1.Text = list[0].ADDRESS_LINE1;
                    tbShipFromAddress2.Text = list[0].ADDRESS_LINE2;
                    tbShipFromName.Text = list[0].LOCATION_ID;
                    tbShipFromState.Text = list[0].STATE;
                    tbPostalCode.Text = list[0].ZIP;
                    tbCity.Text = list[0].CITY;
                    lblMsg.Text = "Action Completed.";
                }
                else {
                    lblMsg.Text = "Several Matches were found.";
                }
            }
            else
            {
                lblMsg.Text = "No Value Selected";
            }
        }

        protected void ddlShipToId_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlShipToId.SelectedValue.Length > 0)
            {
                LocationDataContext db = new LocationDataContext();

                var query = from l in db.LOCATION_MASTERs
                            where l.LOCATION_ID == ddlShipToId.SelectedValue
                            select l;
                var list = query.ToList();
                if (list.Count == 1)
                {
                    tbShipToAddress1.Text = list[0].ADDRESS_LINE1;
                    tbShipToAddress2.Text = list[0].ADDRESS_LINE2;
                    tbShipToName.Text = list[0].LOCATION_ID;
                    tbShipToState.Text = list[0].STATE;
                    tbShipToPostalCode.Text = list[0].ZIP;
                    tbShipToCity.Text = list[0].CITY;
                }
            }

        }

    }
}
