Note to Developers

The WebOrderEntry web application contains two general business processes: GUI order entry and batch order entry(.csv or .xls).  This application validates the user’s entered data and then produces an XML document that is written to a common folder (ex: C:\Inetpub\wwwroot\WebOrderEntry\xml_document_folder). Another DSC program will pick these files up and import them into their EDI order system.

DSC Logistic’s web application will handle user authentication, then call LandingPage.aspx with these two hidden fields(see Default.htm for an example):
    <input name="username" type="hidden" value="KC" />
    <input name="LOGOFF_URL" type="hidden" value="TestLogoff.aspx" />
These two field values would be different based on user credentials.

Module description:
CustomError.cs – class that is used in reporting errors to the error control on a web page.
Default.htm – a way to test WebOrderEntry without the application that DSC uses for validation.
EnterOrder.aspx – contains the GUI form for entering one order at a time.
Error.aspx – page that the user is forwarded to if there is an unhandled error.
Global.asax – Contains the code that logs any unhandled exceptions to the WebOrderEntry Event Log and forwards the user to the error.aspx page.
LandingPage.aspx – The true entry point of the web application.  Contains links to the Enter Order and Upload Order pages.  Session variables are set of the account name and the log off URL here.
Location.dbml – LINQ classes to lookup the selection location id and propagate the ShipTo fields if a record is found.
Site1.Master – contains the header and footer information included on all the pages throughout the website.
UploadOrders.aspx – handles batch uploads of orders in .csv or .xls format.
Web.config – configuration information for the website, including the database connection string and xml document folder location.
XmlDisplay.aspx – a page used for initial developer testing.  Not being used currently.
App_Code/OrderMapper.cs – helper class for mapping the different record segments for the file upload process
App_Code/OrderValidation.cs – validates uploaded file data and sets some of the fields in the xsd.exe generated warehouse_shipping_ordersWarehouse_shipping_order class.
App_Code/WarehouseShippingOrder.cs – xsd.exe generated class using the WarehouseShippingOrder.xsd schema.  This class creates the xml document.
xml_document_folder/Browse0070.aspx - helper class that displays the contents of the xml_document folder in the browser.  There is a link to it on the Defaults.htm test page.

There is a class library project CustomAction in this solution and a setup project.
The CustomAction SetupAction.cs class is used by the setup project to create the 
WebOrderEntry Event Log source during installation.

