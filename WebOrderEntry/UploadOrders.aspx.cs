using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using Subgurim.Controles;
using System.IO;
using System.Data.OleDb;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using App_Code;
using System.Collections;
using System.Xml.Serialization;
using Microsoft.Office.Interop.Excel;
using LumenWorks.Framework.IO.Csv;
using System.Configuration;

namespace WebOrderEntry
{
    public partial class UploadOrders1 : System.Web.UI.Page
    {
        const int HDR_FIELDCOUNT = 17;
        const string HDR = "HDR";
        const int LOC_FIELDCOUNT = 8;
        const string LOC = "LOC";
        const int DET_FIELDCOUNT = 8;
        const string DET = "DET";
        const int ASN_FIELDCOUNT = 5;
        const string ASN = "ASN";
        const int REF_FIELDCOUNT = 3;
        const string REF = "REF";
        const int NTE_FIELDCOUNT = 3;
        const string NTE = "NTE";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            bool bSubmitWithWarning = false;
            if (IsPostBack)
            {
                Control myControl = GetPostBackControl(this.Page.Master);
                if ((myControl.ClientID.ToString() == btnUpload.ClientID))
                {
                    bSubmitWithWarning = false;
                }

                if ((myControl.ClientID.ToString() == btnWarningSubmit.ClientID))
                {
                    bSubmitWithWarning = true;
                }
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

            if (!IsPostBack)
            {
                //DSC system passed in the logoff url
                //must set the menu item up on each page
                if (Session["LogoffUrl"] != null)
                {
                    System.Web.UI.WebControls.Menu masterPageMenu = (System.Web.UI.WebControls.Menu)this.Master.FindControl("Menu1");
                    System.Web.UI.WebControls.MenuItem logoffMenuItem = masterPageMenu.Items[1];
                    logoffMenuItem.NavigateUrl = (string)Session["LogoffUrl"];
                }
            }

            if (FileUpload1.HasFile)
            {
                ArrayList alOrders = null;
                string filePath = null;
                try
                {
                    //verify file extension
                    //must be .xls or .csv (not .xlsx)
                    if (!FileUpload1.FileName.Contains(".xls") &&
                        !FileUpload1.FileName.Contains(".csv")) //modify this test to enable
                    {                                           //csv.
                        lblStatus.CssClass = "errorColor";
                        lblStatus.Text = "Invalid file extension for file " + FileUpload1.PostedFile.FileName;
                        Validators.Add(new CustomError("Invalid file must be .xls or .csv"));
                        return;
                    }
                    else if (FileUpload1.FileName.Contains(".xlsx"))
                    {
                        lblStatus.CssClass = "errorColor";
                        lblStatus.Text = "Invalid file extension for file " + FileUpload1.PostedFile.FileName;
                        Validators.Add(new CustomError("Invalid file must be .xls or .csv"));
                        return;
                    }

                    filePath = Server.MapPath("~/temp/" + FileUpload1.FileName);
                    string sUniqueName = GetUniqueFilename(filePath);
                    FileUpload1.SaveAs(filePath);

                    //process batch order file
                    btnWarningSubmit.Visible = false;
                    List<CustomError> alWarningErrors;
                    alOrders = processFile(filePath, out alWarningErrors);

                    //if there were no errors 
                    if (alOrders != null)
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
                                Validators.Add(new CustomError("One or more Warnings exist.  Select the file and click 'Submit with Warnings' to continue processing order."));
                                alWarningErrors.Clear();
                                lblStatus.CssClass = "errorColor";
                                lblStatus.Text = "Warnings for file " + FileUpload1.PostedFile.FileName;
                                return;
                            }
                        }
                    }
                    else //return so that the user can correct the errors
                    {
                        return;
                    }

                    if (alOrders == null)
                    {
                        lblStatus.CssClass = "errorColor";
                        lblStatus.Text = "Validation failed for file " + FileUpload1.PostedFile.FileName;
                    }
                    else
                    {
                        lblStatus.CssClass = "successColor";
                        lblStatus.Text = "Upload and processing complete for file " + FileUpload1.PostedFile.FileName +
                            ". " + alOrders.Count.ToString() + " orders processed.";
                    }
                }
                catch (Exception ex)
                {
                    Validators.Add(new CustomError("Please contact customer support. Error code 10"));
                    EventLog woeLog = new EventLog("WebOrderEntry");
                    woeLog.Source = "WebOrderEntryApp";
                    string errorMessage = "Message\r\n" +
                        ex.Message.ToString() + "\r\n\r\n";
                    errorMessage += "Source\r\n" +
                        ex.Source + "\r\n\r\n";
                    errorMessage += "Target site\r\n" +
                        ex.TargetSite.ToString() + "\r\n\r\n";
                    errorMessage += "Stack trace\r\n" +
                        ex.StackTrace + "\r\n\r\n";
                    errorMessage += "ToString()\r\n\r\n" +
                        ex.ToString();
                    woeLog.WriteEntry(errorMessage, EventLogEntryType.Error, 10);
                }
                finally
                {
                    //delete the file from the server when done
                    File.Delete(filePath);
                }

                //write xml to production server folder
                warehouse_shipping_orders ordersRootElement = null;
                if (alOrders != null)
                {
                    ordersRootElement = new warehouse_shipping_orders();
                    ordersRootElement.warehouse_shipping_order =
                        new warehouse_shipping_ordersWarehouse_shipping_order[alOrders.Count];
                    for (int x = 0; x < alOrders.Count; x++)
                    {
                        ordersRootElement.warehouse_shipping_order[x] =
                            (warehouse_shipping_ordersWarehouse_shipping_order)alOrders[x];
                    }

                    string sGeneratedXmlDocument = sXmlDocumentPath + "\\" +
                        FileUpload1.FileName + ".xml";
                    string sUniqueGeneratedXmlDocument =
                        GetUniqueFilename(sGeneratedXmlDocument);

                    XmlSerializer s = new XmlSerializer(ordersRootElement.GetType());
                    try
                    {
                        using (TextWriter writer = File.CreateText(sUniqueGeneratedXmlDocument))
                        {
                            s.Serialize(writer, ordersRootElement);
                            writer.Flush();
                            //writer.Close();
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
                }
            }
            else
            {
                if (IsPostBack)
                {
                    lblStatus.CssClass = "errorColor";
                    lblStatus.Text = "File not found " + FileUpload1.PostedFile.FileName;
                }
            }
        }

        //reads the batch order file and returns
        //an array list of orders 
        private ArrayList processFile(string fileName, out List<CustomError> alWarnings)
        {
            alWarnings = new List<CustomError>();
            bool bValid = true;

            ArrayList alOrders = null;
            string csvFileName = null;

            //no conversion needed if .csv
            if (fileName.Contains(".csv"))
            {
                csvFileName = fileName;
            }
            else
            {
                //convert to .csv
                ApplicationClass ExcelApp = null;
                try
                {
                    ExcelApp = new ApplicationClass();
                    Workbook uploadedWorkBook = ExcelApp.Workbooks.Open(fileName,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing);

                    csvFileName = fileName + ".csv";
                    File.Delete(csvFileName);
                    uploadedWorkBook.SaveAs(csvFileName, XlFileFormat.xlCSV,
                        Type.Missing, Type.Missing,
                        Type.Missing, Type.Missing,
                        XlSaveAsAccessMode.xlExclusive, Type.Missing,
                        Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    uploadedWorkBook.Close(false, csvFileName, false);

                }
                catch (Exception e)
                {
                    bValid = false;
                    Validators.Add(new CustomError("Error processing .xls file. Contact customer support. Error code 1"));
                    EventLog woeLog = new EventLog("WebOrderEntry");
                    woeLog.Source = "WebOrderEntryApp";
                    string errorMessage = "Message\r\n" + e.Message.ToString() + "\r\n\r\n";
                    errorMessage += "Source\r\n" + e.Source + "\r\n\r\n";
                    errorMessage += "Target site\r\n" + e.TargetSite.ToString() + "\r\n\r\n";
                    errorMessage += "Stack trace\r\n" + e.StackTrace + "\r\n\r\n";
                    errorMessage += "ToString()\r\n\r\n" + e.ToString();
                    woeLog.WriteEntry(errorMessage, EventLogEntryType.Error, 1);
                    return null;
                }
                finally
                {
                    if (ExcelApp != null)
                    {
                        ExcelApp.Workbooks.Close();
                        ExcelApp.Quit();
                    }
                }
            }

            alOrders = new ArrayList();
            warehouse_shipping_ordersWarehouse_shipping_order eachOrder = null;
            OrderValidation orderValidation = null;
            int iCurrentRow = 1;
            string sCurrentRow = "1";

            // open the file which is a CSV file with no headers
            using (CsvReader csv = new CsvReader(new StreamReader(csvFileName), false))
            {
                int fieldCount = csv.FieldCount;
                //string[] headers = csv.GetFieldHeaders();

                while (csv.ReadNextRecord())
                {
                    string sEachSegment = csv[0].Trim();

                    switch (sEachSegment)
                    {
                        case "HDR":
                            #region validateHeader
                            // New Order
                            if (orderValidation != null)
                            {
                                eachOrder = orderValidation.getOrder(sCurrentRow);
                                if (eachOrder != null)
                                    alOrders.Add(eachOrder);
                                else
                                {
                                    bValid = false;
                                    //Validators.Add(new CustomError("Invalid " + HDR + " field count=" + dr.FieldCount.ToString() + " should be " + HDR_FIELDCOUNT.ToString() + " row " + iCurrentRow + " column A."));
                                    break;
                                }
                            }
                            orderValidation = new OrderValidation(Validators);

                            //set account id from session
                            if (Session["AccountId"] != null)
                                orderValidation.AccountId = (string)Session["AccountId"];

                            //process header record
                            //check field count
                            if (csv.FieldCount < HDR_FIELDCOUNT)
                            {
                                bValid = false;
                                Validators.Add(new CustomError("Invalid " + HDR + " field count=" + csv.FieldCount.ToString() +
                                    " should be " + HDR_FIELDCOUNT.ToString() + " row " + iCurrentRow + " column A."));
                                break;
                            }

                            List<CustomError> eachOrderWarningList = orderValidation.validateHeader(csv, sCurrentRow, out bValid);

                            if (eachOrderWarningList.Count > 0)
                            {
                                foreach (CustomError warning in eachOrderWarningList)
                                {
                                    alWarnings.Add(warning);
                                }
                            }
                            if (bValid == false) { break; }
                            
                            #endregion
                            break;
                        case "LOC":
                            #region validateLoc
                            //process address record
                            //check field count
                            if (csv.FieldCount < LOC_FIELDCOUNT)
                            {
                                bValid = false;
                                Validators.Add(new CustomError("Invalid " + LOC + " field count=" + csv.FieldCount.ToString() + " should be " + LOC_FIELDCOUNT.ToString() + " row " + iCurrentRow + " column A."));
                                break;
                            }
                            if (!orderValidation.validateLoc(csv, sCurrentRow))
                            {
                                bValid = false;
                                break;
                            }
                            #endregion
                            break;
                        case "DET":
                            #region validateDetail
                            //process detail record
                            //check field count
                            if (csv.FieldCount < DET_FIELDCOUNT)
                            {
                                bValid = false;
                                Validators.Add(new CustomError("Invalid " + DET + " field count=" + csv.FieldCount.ToString() + " should be " + DET_FIELDCOUNT.ToString() + " row " + iCurrentRow + " column A."));
                                break;
                            }
                            if (!orderValidation.validateDetail(csv, sCurrentRow))
                            {
                                bValid = false;
                                break;
                            }
                            #endregion
                            break;
                        case "ASN":
                            #region validateASN
                            //process asn record
                            //check field count
                            if (csv.FieldCount < ASN_FIELDCOUNT)
                            {
                                bValid = false;
                                Validators.Add(new CustomError("Invalid " + ASN + " field count=" + csv.FieldCount.ToString() + " should be " + ASN_FIELDCOUNT.ToString() + " row " + iCurrentRow + " column A."));
                                break;
                            }
                            if (!orderValidation.validateAsn(csv, sCurrentRow))
                            {
                                bValid = false;
                                break;
                            }
                            #endregion
                            break;
                        case "REF":
                            #region validateRefNbr
                            //process header reference record
                            //check field count
                            if (csv.FieldCount < REF_FIELDCOUNT)
                            {
                                bValid = false;
                                Validators.Add(new CustomError("Invalid " + REF + " field count=" + csv.FieldCount.ToString() + " should be " + REF_FIELDCOUNT.ToString() + " row " + iCurrentRow + " column A."));
                                break;
                            }
                            if (!orderValidation.validateRef(csv, sCurrentRow))
                            {
                                bValid = false;
                                break;
                            }
                            #endregion
                            break;
                        case "NTE":
                            #region validateRefNbr
                            //process note record
                            //check field count
                            if (csv.FieldCount < NTE_FIELDCOUNT)
                            {
                                bValid = false;
                                Validators.Add(new CustomError("Invalid " + NTE + " field count=" + csv.FieldCount.ToString() + " should be " + NTE_FIELDCOUNT.ToString() + " row " + iCurrentRow + " column A."));
                                break;
                            }
                            if (!orderValidation.validateNote(csv, sCurrentRow))
                            {
                                bValid = false;
                                break;
                            }
                            #endregion
                            break;
                        default:
                            bValid = false;
                            Validators.Add(new CustomError("Invalid Segment Name" + sEachSegment + " - row " + iCurrentRow + " column A."));
                            break;
                    }


                    iCurrentRow++;
                    sCurrentRow = Convert.ToString(iCurrentRow);
                }
            }

            //done with csv file, delete it
            File.Delete(csvFileName);

            //}
            //check/add the last order
            if (bValid)
            {
                eachOrder = orderValidation.getOrder(sCurrentRow);
                if (eachOrder != null)
                    alOrders.Add(eachOrder);
                else
                    bValid = false;
            }

            //return the validated orders arraylist
            if (bValid)
            {
                return alOrders;
            }
            else
            {
                return null;

            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            //handled in Page_Load
        }
        protected void btnWarningSubmit_Click(object sender, EventArgs e)
        {
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

    }
}
