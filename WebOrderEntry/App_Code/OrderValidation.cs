using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Web.UI;
using WebOrderEntry;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using LumenWorks.Framework.IO.Csv;
using System.Diagnostics;

namespace App_Code
{
  public class OrderValidation
  {
    warehouse_shipping_ordersWarehouse_shipping_order order =
        new warehouse_shipping_ordersWarehouse_shipping_order();

    ArrayList alOrderDetail = new ArrayList();
    ArrayList alOrderHeaderReference = new ArrayList();
    ArrayList alOrderNotes = new ArrayList();
    ValidatorCollection uploadWebValidators = null;

    const decimal MAX_VOLUME = 9999999.99m;
    const decimal MAX_WEIGHT = 99999999.99m;

    private string accountId;

    public string AccountId
    {
      get { return accountId; }
      set { accountId = value; }
    }
    private string accountNumber;

    public string AccountNumber
    {
      get { return accountNumber; }
      set { accountNumber = value; }
    }

    //disable default constructor
    private OrderValidation() { }

    public OrderValidation(ValidatorCollection validators)
    {
      uploadWebValidators = validators;
      order.total_weight = new total_weight();
      address_type[] addresses = new address_type[2];
      order.address = addresses;
    }

    //number of fields has already been validated
    //        public bool validateHeader(DbDataReader dr, string sCurrentRow)
    public List<CustomError> validateHeader(CsvReader csv, string sCurrentRow, out bool bValid)
    {
      List<CustomError> alWarningErrors = new List<CustomError>();
      bValid = true;
      //validate status
      if (csv[(int)OrderMapper.HeaderCells.STATUS].Length > 0)
      {
        if (csv[(int)OrderMapper.HeaderCells.STATUS].Equals("N"))
          order.status = status.N;
        else if (csv[(int)OrderMapper.HeaderCells.STATUS].Equals("R"))
          order.status = status.R;
        else if (csv[(int)OrderMapper.HeaderCells.STATUS].Equals("F"))
          order.status = status.F;
        else
        {
          bValid = false;
          uploadWebValidators.Add(
              new CustomError("Invalid Status. row=" + sCurrentRow + " column=" +
                  OrderMapper.COLUMN_MAPPER[(int)OrderMapper.HeaderCells.STATUS]));
        }
      }
      else
      {
        bValid = false;
        uploadWebValidators.Add(
            new CustomError("Status required. row=" + sCurrentRow + " column=" +
                OrderMapper.COLUMN_MAPPER[(int)OrderMapper.HeaderCells.STATUS]));
      }

      if (csv[(int)OrderMapper.HeaderCells.CUSTOMER_REF_NUMBER].Length > 0)
      {
        order.customer_reference_number =
            csv[(int)OrderMapper.HeaderCells.CUSTOMER_REF_NUMBER].ToString();
        if (order.customer_reference_number.Length > 22)
        {
          bValid = false;
          uploadWebValidators.Add(
              new CustomError("Customer Ref Number invalid length. row=" + sCurrentRow + " column=" +
                  OrderMapper.COLUMN_MAPPER[(int)OrderMapper.HeaderCells.CUSTOMER_REF_NUMBER]));
        }
      }
      else
      {
        bValid = false;
        uploadWebValidators.Add(
            new CustomError("Customer Ref Number required. row=" + sCurrentRow + " column=" +
                OrderMapper.COLUMN_MAPPER[(int)OrderMapper.HeaderCells.CUSTOMER_REF_NUMBER]));
      }

      if (csv[(int)OrderMapper.HeaderCells.CONSIGNEE_PO].Length > 0)
      {
        order.consignee_po = csv[(int)OrderMapper.HeaderCells.CONSIGNEE_PO].ToString();
        if (order.consignee_po.Length > 22)
        {
          bValid = false;
          uploadWebValidators.Add(
              new CustomError("Consignee invalid length. row=" + sCurrentRow + " column=" +
                  OrderMapper.COLUMN_MAPPER[(int)OrderMapper.HeaderCells.CONSIGNEE_PO]));
        }
      }
      else
      {
        bValid = false;
        uploadWebValidators.Add(
            new CustomError("Consignee required. row=" + sCurrentRow + " column=" +
                OrderMapper.COLUMN_MAPPER[(int)OrderMapper.HeaderCells.CONSIGNEE_PO]));
      }

      //export
      if (csv[(int)OrderMapper.HeaderCells.EXPORT].Length > 0)
      {
        if (csv[(int)OrderMapper.HeaderCells.EXPORT].Equals("N"))
          order.export = export.N;
        else if (csv[(int)OrderMapper.HeaderCells.EXPORT].Equals("Y"))
          order.export = export.Y;
        else
        {
          bValid = false;
          uploadWebValidators.Add(
              new CustomError("Invalid Export row=" + sCurrentRow + " column=" +
                  OrderMapper.COLUMN_MAPPER[(int)OrderMapper.HeaderCells.EXPORT]));

        }
      }
      else
      {
        bValid = false;
        uploadWebValidators.Add(
            new CustomError("Export required. row=" + sCurrentRow + " column=" +
                OrderMapper.COLUMN_MAPPER[(int)OrderMapper.HeaderCells.EXPORT]));
      }

      //temp control
      if (csv[(int)OrderMapper.HeaderCells.TEMP_CONTROL].Length > 0)
      {
        if (csv[(int)OrderMapper.HeaderCells.TEMP_CONTROL].Equals("N"))
          order.temperature_controlled = temperature_controlled.N;
        else if (csv[(int)OrderMapper.HeaderCells.TEMP_CONTROL].Equals("Y"))
          order.temperature_controlled = temperature_controlled.Y;
        else
        {
          bValid = false;
          uploadWebValidators.Add(
              new CustomError("Invalid Temperature Control row=" + sCurrentRow + " column=" +
                  OrderMapper.COLUMN_MAPPER[(int)OrderMapper.HeaderCells.TEMP_CONTROL]));
        }
      }
      else
      {
        bValid = false;
        uploadWebValidators.Add(
            new CustomError("Temperature Control required. row=" + sCurrentRow + " column=" +
                OrderMapper.COLUMN_MAPPER[(int)OrderMapper.HeaderCells.TEMP_CONTROL]));
      }

      //hazmat
      if (csv[(int)OrderMapper.HeaderCells.HAZMAT].Length > 0)
      {
        if (csv[(int)OrderMapper.HeaderCells.HAZMAT].Equals("N"))
          order.hazmat = hazmat.N;
        else if (csv[(int)OrderMapper.HeaderCells.HAZMAT].Equals("Y"))
          order.hazmat = hazmat.Y;
        else
        {
          bValid = false;
          uploadWebValidators.Add(
              new CustomError("Invalid Hazmat row=" + sCurrentRow + " column=" +
                  OrderMapper.COLUMN_MAPPER[(int)OrderMapper.HeaderCells.HAZMAT]));
        }
      }
      else
      {
        bValid = false;
        uploadWebValidators.Add(
            new CustomError("Hazmat required. row=" + sCurrentRow + " column=" +
                OrderMapper.COLUMN_MAPPER[(int)OrderMapper.HeaderCells.HAZMAT]));
      }

      if (csv[(int)OrderMapper.HeaderCells.PAYMENT_METHOD].Length > 0)
      {
        if (csv[(int)OrderMapper.HeaderCells.PAYMENT_METHOD].Equals("PP"))
          order.payment_method = payment_method.PP;
        else if (csv[(int)OrderMapper.HeaderCells.PAYMENT_METHOD].Equals("CC"))
          order.payment_method = payment_method.PP;
        else if (csv[(int)OrderMapper.HeaderCells.PAYMENT_METHOD].Equals("PC"))
          order.payment_method = payment_method.PC;
        else
        {
          bValid = false;
          uploadWebValidators.Add(
              new CustomError("Invalid Payment Method row=" + sCurrentRow + " column=" +
                  OrderMapper.COLUMN_MAPPER[(int)OrderMapper.HeaderCells.PAYMENT_METHOD]));
        }
      }
      else
      {
        bValid = false;
        uploadWebValidators.Add(
            new CustomError("Payment Method required. row=" + sCurrentRow + " column=" +
                OrderMapper.COLUMN_MAPPER[(int)OrderMapper.HeaderCells.PAYMENT_METHOD]));
      }

      //Transportation Method
      string transpMethod = csv[(int)OrderMapper.HeaderCells.TRANSPORTATION_METHOD].Trim();
      if (transpMethod.Length > 0)
      {
          switch (transpMethod)
          {
              case "M": order.transport_method = transport_method.M;
                  break;
              case "U": order.transport_method = transport_method.U;
                  break;
              case "H": order.transport_method = transport_method.H;
                  break;
              default:
                  bValid = false;
                  uploadWebValidators.Add(
                      new CustomError("Transportation Method '" + transpMethod + "' is Invalid. Row=" + sCurrentRow + " Column=" +
                          OrderMapper.COLUMN_MAPPER[(int)OrderMapper.HeaderCells.TRANSPORTATION_METHOD]));
                  break;
          }
      }
      else
      {
        bValid = false;
        uploadWebValidators.Add(
            new CustomError("Transportation Method is required. row=" + sCurrentRow + " column=" +
                OrderMapper.COLUMN_MAPPER[(int)OrderMapper.HeaderCells.TRANSPORTATION_METHOD]));
      }

      //SCAC
      if (csv[(int)OrderMapper.HeaderCells.SCAC].Length > 0)
      {
        order.scac = csv[(int)OrderMapper.HeaderCells.SCAC].ToString();
        if (!dbValidateSCAC(order.scac, sCurrentRow))
        {
          bValid = false;
        }
      }
      else
      {
        bValid = false;
        uploadWebValidators.Add(
            new CustomError("SCAC required. row=" + sCurrentRow + " column=" +
                OrderMapper.COLUMN_MAPPER[(int)OrderMapper.HeaderCells.SCAC]));
      }

      //total weight and weight uom
      if (csv[(int)OrderMapper.HeaderCells.TOTAL_WEIGHT].Length > 0)
      {
        try
        {
          order.total_weight.weight =
              Convert.ToDecimal(csv[(int)OrderMapper.HeaderCells.TOTAL_WEIGHT].ToString());
          order.total_weight.weight_unit_of_measure = weight_unit_of_measure.LB;
          if (order.total_weight.weight > MAX_WEIGHT)
          {
            bValid = false;
            uploadWebValidators.Add(
                new CustomError("Total Weight exceeds maximum of 99999999.99 row=" + sCurrentRow + " column=" +
                    OrderMapper.COLUMN_MAPPER[(int)OrderMapper.HeaderCells.TOTAL_WEIGHT]));
          }
        }
        catch (Exception e)
        {
          bValid = false;
          uploadWebValidators.Add(
              new CustomError("Total Weight field invalid. row=" + sCurrentRow + " column=" +
                  OrderMapper.COLUMN_MAPPER[(int)OrderMapper.HeaderCells.TOTAL_WEIGHT] +
                  " " + e.Message));
        }
      }
      else
      {
        bValid = false;
        uploadWebValidators.Add(
            new CustomError("Total Weight required row=" + sCurrentRow + " column=" +
                OrderMapper.COLUMN_MAPPER[(int)OrderMapper.HeaderCells.TOTAL_WEIGHT]));
      }

      //total volume and volume uom (optional)
      if (csv[(int)OrderMapper.HeaderCells.TOTAL_VOLUME].Length > 0)
      {
        try
        {
          order.total_volume = new total_volume();
          order.total_volume.volume =
              Convert.ToDecimal(csv[(int)OrderMapper.HeaderCells.TOTAL_VOLUME].ToString());
          order.total_volume.volume_unit_of_measure = volume_unit_of_measure.CF;
          if (order.total_volume.volume > MAX_VOLUME)
          {
            bValid = false;
            uploadWebValidators.Add(
                new CustomError("Total Volume exceeds maximum of 9999999.99  row=" + sCurrentRow + " column=" +
                    OrderMapper.COLUMN_MAPPER[(int)OrderMapper.HeaderCells.TOTAL_VOLUME]));
          }
        }
        catch (Exception e)
        {
          bValid = false;
          uploadWebValidators.Add(
              new CustomError("Invalid Total Volume.  row=" + sCurrentRow + " column=" +
                  OrderMapper.COLUMN_MAPPER[(int)OrderMapper.HeaderCells.TOTAL_VOLUME] +
                  " " + e.Message));
        }
      }

      //ship date
      DateTime dtShipDate = new DateTime();
      bool bValidDate = false;
      if (csv[(int)OrderMapper.HeaderCells.SHIP_DATE].Length > 0)
      {
        string sShipDate = csv[(int)OrderMapper.HeaderCells.SHIP_DATE].ToString();

        dtShipDate = new DateTime();
        try
        {
          dtShipDate = DateTime.ParseExact(sShipDate, "yyyyMMdd", null);
          if (dtShipDate.DayOfWeek == DayOfWeek.Saturday ||
                dtShipDate.DayOfWeek == DayOfWeek.Sunday)
          {
            alWarningErrors.Add(
                new CustomError("Warning: Ship Date is a Saturday or Sunday.  Extra charges may be applied. row=" + sCurrentRow + " column=" +
                    OrderMapper.COLUMN_MAPPER[(int)OrderMapper.HeaderCells.SHIP_DATE]));
          }
          if (dtShipDate.CompareTo(DateTime.Today) < 0)
          {
            bValid = false;
            uploadWebValidators.Add(
                new CustomError("Invalid Ship Date.  Cannot be earlier than today's date. row=" + sCurrentRow + " column=" +
                    OrderMapper.COLUMN_MAPPER[(int)OrderMapper.HeaderCells.SHIP_DATE]));
          }

          if (dtShipDate.CompareTo(DateTime.Today) == 0)
          {
            alWarningErrors.Add(
                new CustomError("Warning: Ship Date is today's date.  Extra charges may be applied. row=" + sCurrentRow + " column=" +
                    OrderMapper.COLUMN_MAPPER[(int)OrderMapper.HeaderCells.SHIP_DATE]));
          }

          if (bValid == true)
          {
            order.shipment_date = sShipDate;
            bValidDate = true;
          }

        }
        catch (Exception e)
        {
          bValid = false;
          uploadWebValidators.Add(
              new CustomError("Ship Date invalid row=" + sCurrentRow + " column=" +
                  OrderMapper.COLUMN_MAPPER[(int)OrderMapper.HeaderCells.SHIP_DATE] +
                  ". " + e.Message));
        }
      }
      else
      {
        bValid = false;
        uploadWebValidators.Add(
            new CustomError("Ship Date required row=" + sCurrentRow + " column=" +
                OrderMapper.COLUMN_MAPPER[(int)OrderMapper.HeaderCells.SHIP_DATE]));
      }

      //delivery date
      if (csv[(int)OrderMapper.HeaderCells.DELIVERY_DATE].Length > 0)
      {
        string sDeliverDate = csv[(int)OrderMapper.HeaderCells.DELIVERY_DATE].ToString();
        DateTime dtDeliverDate;
        dtDeliverDate = new DateTime();
        try
        {
          dtDeliverDate = DateTime.ParseExact(sDeliverDate, "yyyyMMdd", null);
          if (bValidDate)
          {
            int iCompareTest = dtShipDate.CompareTo(dtDeliverDate);
            //if they are the same
            if (iCompareTest == 0)
            {
              bValid = false;
              uploadWebValidators.Add(
                  new CustomError("Invalid Ship Date and Delivery Date combination.  They cannot be the same. row=" + sCurrentRow + " columns=" +
                      OrderMapper.COLUMN_MAPPER[(int)OrderMapper.HeaderCells.SHIP_DATE] +
                      " and " + OrderMapper.COLUMN_MAPPER[(int)OrderMapper.HeaderCells.DELIVERY_DATE]));
            }
            if (iCompareTest > 0)
            {
              bValid = false;
              uploadWebValidators.Add(
                  new CustomError("Invalid Ship Date and Delivery Date combination.  Ship Date cannot be greater. row=" + sCurrentRow + " columns=" +
                      OrderMapper.COLUMN_MAPPER[(int)OrderMapper.HeaderCells.SHIP_DATE] +
                      " and " + OrderMapper.COLUMN_MAPPER[(int)OrderMapper.HeaderCells.DELIVERY_DATE]));
            }
          }

          if (bValid == true)
            order.delivery_date = sDeliverDate;
        }
        catch (Exception e)
        {
          bValid = false;
          uploadWebValidators.Add(
              new CustomError("Delivery Date invalid row=" + sCurrentRow + " column=" +
                  OrderMapper.COLUMN_MAPPER[(int)OrderMapper.HeaderCells.DELIVERY_DATE] +
                  ". " + e.Message));
        }


      }
      else
      {
        bValid = false;
        uploadWebValidators.Add(
            new CustomError("Delivery Date required row=" + sCurrentRow + " column=" +
                OrderMapper.COLUMN_MAPPER[(int)OrderMapper.HeaderCells.DELIVERY_DATE]));
      }

      //ucc128
      if (csv[(int)OrderMapper.HeaderCells.UCC128].Length > 0)
      {
        order.ucc128 = csv[(int)OrderMapper.HeaderCells.UCC128];
        if (!order.ucc128.Equals("Y") &&
           !order.ucc128.Equals("N"))
        {
          bValid = false;
          uploadWebValidators.Add(
              new CustomError("UCC128 invalid. Must be Y or N. row=" + sCurrentRow + " column=" +
                  OrderMapper.COLUMN_MAPPER[(int)OrderMapper.HeaderCells.UCC128]));
        }
      }
      else
      {
        bValid = false;
        uploadWebValidators.Add(
            new CustomError("UCC128 required row=" + sCurrentRow + " column=" +
                OrderMapper.COLUMN_MAPPER[(int)OrderMapper.HeaderCells.UCC128]));
      }
      return alWarningErrors;
    }

    //number of fields has already been validated
    public bool validateLoc(CsvReader csv, string sCurrentRow)
        {
            bool bValid = true;

            //instantiating a new type so that even if the 
            //type value is not valid, the rest of the row can
            //be validated
            address_type address = new address_type();

            //validate type
            if (csv[(int)OrderMapper.LocationCells.TYPE].Equals("SF"))
            {
                address.type = type.SHIPFROM;
                if (order.address[0] == null)
                    order.address[0] = address;
                else
                {
                    bValid = false;
                    uploadWebValidators.Add(
                        new CustomError("Ship From LOC already exists for this order. row=" + sCurrentRow + " column=" +
                            OrderMapper.COLUMN_MAPPER[(int)OrderMapper.LocationCells.TYPE]));
                }
            }
            else if (csv[ (int)OrderMapper.LocationCells.TYPE].Equals("ST"))
            {
                address.type = type.SHIPTO;
                if (order.address[1] == null)
                    order.address[1] = address;
                else
                {
                    bValid = false;
                    uploadWebValidators.Add(
                        new CustomError("Ship To LOC already exists for this order. row=" + sCurrentRow + " column=" +
                            OrderMapper.COLUMN_MAPPER[(int)OrderMapper.LocationCells.TYPE]));
                }
            }
            else
            {
                bValid = false;
                uploadWebValidators.Add(
                    new CustomError("Invalid Type row=" + sCurrentRow + " column=" +
                        OrderMapper.COLUMN_MAPPER[(int)OrderMapper.LocationCells.TYPE]));
            }
            
            //validate location id (only SHIPFROM validated or used)
            //this field added release 1.0.0.2 02-16-2009 map
            //validate name
            bool bValidShipFromLocationID = false;
            if (csv[(int)OrderMapper.LocationCells.TYPE].Equals("SF"))
            {
                  string sLocationId = 
                    csv[(int)OrderMapper.LocationCells.LOCATION_ID].ToString();
                  if (sLocationId.Length > 12)
                  {
                      bValid = false;
                      uploadWebValidators.Add(
                          new CustomError("LOCATION_ID invalid length. row=" + sCurrentRow + " column=" +
                              OrderMapper.COLUMN_MAPPER[(int)OrderMapper.LocationCells.LOCATION_ID]));
                  }
                  if (bValid)
                  {
                    dbGetAccountNumber(this.AccountId, sLocationId);
                  }

                  if (sLocationId.Length > 0)
                  {
                    LocationDataContext db = new LocationDataContext();

                    var query = from l in db.LOCATION_MASTERs
                                where l.LOCATION_ID == sLocationId
                                select l;
                    var list = query.ToList();
                    if (list.Count == 1)
                    {
                      address.address1 = list[0].ADDRESS_LINE1;
                      address.address2 = list[0].ADDRESS_LINE2;
                      address.name = list[0].LOCATION_ID;
                      address.state_province = list[0].STATE;
                      address.postal_code = list[0].ZIP;
                      address.city = list[0].CITY;
                      bValidShipFromLocationID = true;
                    }

                  }

             }

            if (bValidShipFromLocationID == false)
            {
              //validate name
              if (csv[(int)OrderMapper.LocationCells.NAME].ToString().Length > 0)
              {
                address.name = csv[(int)OrderMapper.LocationCells.NAME].ToString();
                if (address.name.Length > 35)
                {
                  bValid = false;
                  uploadWebValidators.Add(
                      new CustomError("Name invalid length. row=" + sCurrentRow + " column=" +
                          OrderMapper.COLUMN_MAPPER[(int)OrderMapper.LocationCells.NAME]));
                }
              }
              else
              {
                bValid = false;
                uploadWebValidators.Add(
                    new CustomError("Name required row=" + sCurrentRow + " column=" +
                        OrderMapper.COLUMN_MAPPER[(int)OrderMapper.LocationCells.NAME]));
              }

              //validate address1
              if (csv[(int)OrderMapper.LocationCells.ADDRESS1].ToString().Length > 0)
              {
                address.address1 = csv[(int)OrderMapper.LocationCells.ADDRESS1].ToString();
                if (address.address1.Length > 35)
                {
                  bValid = false;
                  uploadWebValidators.Add(
                      new CustomError("Address1 invalid length row=" + sCurrentRow + " column=" +
                          OrderMapper.COLUMN_MAPPER[(int)OrderMapper.LocationCells.ADDRESS1]));
                }
              }
              else
              {
                bValid = false;
                uploadWebValidators.Add(
                    new CustomError("Address1 required row=" + sCurrentRow + " column=" +
                        OrderMapper.COLUMN_MAPPER[(int)OrderMapper.LocationCells.ADDRESS1]));
              }

              //set address2
              if (csv[(int)OrderMapper.LocationCells.ADDRESS2].ToString().Length > 0)
              {
                address.address2 = csv[(int)OrderMapper.LocationCells.ADDRESS2].ToString();
                if (address.address2.Length > 35)
                {
                  bValid = false;
                  uploadWebValidators.Add(
                      new CustomError("Address2 invalid length. row=" + sCurrentRow + " column=" +
                          OrderMapper.COLUMN_MAPPER[(int)OrderMapper.LocationCells.ADDRESS2]));
                }
              }

              //validate address1
              if (csv[(int)OrderMapper.LocationCells.ADDRESS1].ToString().Length > 0)
              {
                address.address1 = csv[(int)OrderMapper.LocationCells.ADDRESS1].ToString();
              }
              else
              {
                bValid = false;
                uploadWebValidators.Add(
                    new CustomError("Address1 required row=" + sCurrentRow + " column=" +
                        OrderMapper.COLUMN_MAPPER[(int)OrderMapper.LocationCells.ADDRESS1]));
              }

              //validate city
              if (csv[(int)OrderMapper.LocationCells.CITY].ToString().Length > 0)
              {
                address.city = csv[(int)OrderMapper.LocationCells.CITY].ToString();
              }
              else
              {
                bValid = false;
                uploadWebValidators.Add(
                    new CustomError("City required row=" + sCurrentRow + " column=" +
                        OrderMapper.COLUMN_MAPPER[(int)OrderMapper.LocationCells.CITY]));
              }

              //validate state
              if (csv[(int)OrderMapper.LocationCells.STATE_PROVINCE].ToString().Length > 0)
              {
                address.state_province =
                    csv[(int)OrderMapper.LocationCells.STATE_PROVINCE].ToString();
              }
              else
              {
                bValid = false;
                uploadWebValidators.Add(
                    new CustomError("State/Province required row=" + sCurrentRow + " column=" +
                        OrderMapper.COLUMN_MAPPER[(int)OrderMapper.LocationCells.STATE_PROVINCE]));
              }

              //validate postal code
              if (csv[(int)OrderMapper.LocationCells.POSTAL_CODE].ToString().Length > 0)
              {
                object postal = csv[(int)OrderMapper.LocationCells.POSTAL_CODE];
                address.postal_code =
                    csv[(int)OrderMapper.LocationCells.POSTAL_CODE].ToString();
              }
              else
              {
                bValid = false;
                uploadWebValidators.Add(
                    new CustomError("Postal Code required row=" + sCurrentRow + " column=" +
                        OrderMapper.COLUMN_MAPPER[(int)OrderMapper.LocationCells.POSTAL_CODE]));
              }
            }
            if (bValid == true)
            {
                //validate city state zip against the database
                if (!dbValidateCityStateZip(address, sCurrentRow))
                {
                    bValid = false;
                }
            }
            return bValid;
        }

    public bool validateRef(CsvReader csv, string sCurrentRow)
    {
      bool bValid = true;

      header_reference headerReference = new header_reference();

      //validate type
      if (csv[(int)OrderMapper.ReferenceCells.TYPE].ToString().Length > 0)
      {
        headerReference.qualifier = csv[(int)OrderMapper.ReferenceCells.TYPE].ToString();
        if (csv[(int)OrderMapper.ReferenceCells.NAME].ToString().Length > 0)
        {
          headerReference.description =
              csv[(int)OrderMapper.ReferenceCells.NAME].ToString();
          if (headerReference.description.Length > 30)
          {
            bValid = false;
            uploadWebValidators.Add(
                new CustomError("Ref Name invalid length. row=" + sCurrentRow + " column=" +
                    OrderMapper.COLUMN_MAPPER[(int)OrderMapper.ReferenceCells.NAME]));
          }
        }
        else
        {
          bValid = false;
          uploadWebValidators.Add(
              new CustomError("Ref Name required row=" + sCurrentRow + " column=" +
                  OrderMapper.COLUMN_MAPPER[(int)OrderMapper.ReferenceCells.NAME]));
        }
        //check reference type against the database
        if (bValid)
        {
          bValid = dbValidate_REF_ReferenceType(headerReference, sCurrentRow);
        }
      }
      else
      {
        bValid = false;
        uploadWebValidators.Add(
            new CustomError("Ref Type required row=" + sCurrentRow + " column=" +
                OrderMapper.COLUMN_MAPPER[(int)OrderMapper.ReferenceCells.TYPE]));
      }

      //add header reference to the list
      if (bValid)
      {
        //check for duplicate types
        for (int x = 0; x < alOrderHeaderReference.Count; x++)
        {
          string previouslyAddedType = ((header_reference)alOrderHeaderReference[x]).qualifier;
          string newType = csv[(int)OrderMapper.ReferenceCells.TYPE].ToString();
          if (previouslyAddedType.Equals(newType))
          {
            bValid = false;
            uploadWebValidators.Add(
                new CustomError("Duplicate REF Type found.  Only one of each type allowed.  row=" + sCurrentRow + " column=" +
                OrderMapper.COLUMN_MAPPER[(int)OrderMapper.ReferenceCells.TYPE]));
          }
        }

        if (bValid)
          alOrderHeaderReference.Add(headerReference);
      }

      return bValid;
    }

    public bool validateNote(CsvReader csv, string sCurrentRow)
    {
      bool bValid = true;

      note headerNote = new note();

      //validate type
      if (csv[(int)OrderMapper.NoteCells.TYPE].ToString().Length > 0)
      {
        headerNote.qualifier = csv[(int)OrderMapper.NoteCells.TYPE].ToString();
        if (csv[(int)OrderMapper.NoteCells.NAME].ToString().Length > 0)
        {
          headerNote.description =
              csv[(int)OrderMapper.NoteCells.NAME].ToString();
          if (headerNote.description.Length > 60)
          {
            bValid = false;
            uploadWebValidators.Add(
                new CustomError("Note Name invalid length. row=" + sCurrentRow + " column=" +
                    OrderMapper.COLUMN_MAPPER[(int)OrderMapper.NoteCells.NAME]));
          }
        }
        else
        {
          bValid = false;
          uploadWebValidators.Add(
              new CustomError("Note Name required row=" + sCurrentRow + " column=" +
                  OrderMapper.COLUMN_MAPPER[(int)OrderMapper.NoteCells.NAME]));
        }
        if (bValid)
        {
          bValid = dbValidateInstCodes(headerNote.qualifier, sCurrentRow);
        }
      }
      else
      {
        bValid = false;
        uploadWebValidators.Add(
            new CustomError("Note Type required row=" + sCurrentRow + " column=" +
                OrderMapper.COLUMN_MAPPER[(int)OrderMapper.ReferenceCells.TYPE]));
      }

      //add header reference to the list
      if (bValid)
      {
        alOrderNotes.Add(headerNote);
      }

      return bValid;
    }

    public bool validateDetail(CsvReader csv, string sCurrentRow)
    {
      bool bValid = true;
      detail_type detail = new detail_type();

      //validate line
      if (csv[(int)OrderMapper.DetailCells.LINE_NUMBER].ToString().Length > 0)
      {
        try
        {
          Convert.ToInt32(csv[(int)OrderMapper.DetailCells.LINE_NUMBER].ToString());
          detail.line_number = csv[(int)OrderMapper.DetailCells.LINE_NUMBER].ToString();
        }
        catch (FormatException)
        {
          bValid = false;
          uploadWebValidators.Add(
              new CustomError("Invalid Line Number row=" + sCurrentRow + " column=" +
                  OrderMapper.COLUMN_MAPPER[(int)OrderMapper.DetailCells.LINE_NUMBER]));
        }
      }
      else
      {
        bValid = false;
        uploadWebValidators.Add(
            new CustomError("Line Number required row=" + sCurrentRow + " column=" +
                OrderMapper.COLUMN_MAPPER[(int)OrderMapper.DetailCells.LINE_NUMBER]));
      }

      //validate Product#/NMFC
      if (csv[(int)OrderMapper.DetailCells.PRODUCT_NUMBER_NMFC].ToString().Length > 0)
      {
        detail.product_number_or_nmfc =
            csv[(int)OrderMapper.DetailCells.PRODUCT_NUMBER_NMFC].ToString();
      }
      else
      {
        bValid = false;
        uploadWebValidators.Add(
            new CustomError("Product#/NMFC required row=" + sCurrentRow + " column=" +
                OrderMapper.COLUMN_MAPPER[(int)OrderMapper.DetailCells.PRODUCT_NUMBER_NMFC]));
      }

      //validate Quantity
      if (csv[(int)OrderMapper.DetailCells.QUANTITY].ToString().Length > 0)
      {
        try
        {
          int test = Convert.ToInt32(csv[(int)OrderMapper.DetailCells.QUANTITY].ToString());
          if (test > 999999999)
          {
            bValid = false;
            uploadWebValidators.Add(
                new CustomError("Quantity greater than 999999999. row=" + sCurrentRow + " column=" +
                    OrderMapper.COLUMN_MAPPER[(int)OrderMapper.DetailCells.QUANTITY]));
          }
          detail.quantity =csv[(int)OrderMapper.DetailCells.QUANTITY].ToString();
        }
        catch (Exception e)
        {
          bValid = false;
          uploadWebValidators.Add(
              new CustomError("Quantity invalid. row=" + sCurrentRow + " column=" +
                  OrderMapper.COLUMN_MAPPER[(int)OrderMapper.DetailCells.QUANTITY] +
                  " " + e.Message));
        }
      }
      else
      {
        bValid = false;
        uploadWebValidators.Add(
            new CustomError("Quantity required row=" + sCurrentRow + " column=" +
                OrderMapper.COLUMN_MAPPER[(int)OrderMapper.DetailCells.QUANTITY]));
      }

      //validate Lot (optional)
      if (csv[(int)OrderMapper.DetailCells.LOT].ToString().Length > 0)
      {
        detail.lot_number =
            csv[(int)OrderMapper.DetailCells.LOT].ToString();
        if (detail.lot_number.Length > 12)
        {
          bValid = false;
          uploadWebValidators.Add(
              new CustomError("Lot number invalid length. row=" + sCurrentRow + " column=" +
                  OrderMapper.COLUMN_MAPPER[(int)OrderMapper.DetailCells.LOT]));
        }
      }

      //validate detail reference type, number, and description (optional)
      if (csv[(int)OrderMapper.DetailCells.REF_TYPE].ToString().Length > 0)
      {
        detail.reference = new reference();
        detail.reference.qualifier =
            csv[(int)OrderMapper.DetailCells.REF_TYPE].ToString();
        //ref number
        if (csv[(int)OrderMapper.DetailCells.REF_NUMBER].ToString().Length > 0)
        {
          detail.reference.number =
              csv[(int)OrderMapper.DetailCells.REF_NUMBER].ToString();
          if (detail.reference.number.Length > 30)
          {
            bValid = false;
            uploadWebValidators.Add(
                new CustomError("Reference Number invalid length. row=" + sCurrentRow + " column=" +
                    OrderMapper.COLUMN_MAPPER[(int)OrderMapper.DetailCells.REF_NUMBER]));
          }
        }
        else
        {
          bValid = false;
          uploadWebValidators.Add(
              new CustomError("Reference Number required row=" + sCurrentRow + " column=" +
                  OrderMapper.COLUMN_MAPPER[(int)OrderMapper.DetailCells.REF_NUMBER]));
        }
        //ref desc
        if (csv[(int)OrderMapper.DetailCells.REF_DESCRIPTION].ToString().Length > 0)
        {
          detail.reference.description =
              csv[(int)OrderMapper.DetailCells.REF_DESCRIPTION].ToString();
          if (detail.reference.description.Length > 45)
          {
            bValid = false;
            uploadWebValidators.Add(
                new CustomError("Reference Description invalid length. row=" + sCurrentRow + " column=" +
                    OrderMapper.COLUMN_MAPPER[(int)OrderMapper.DetailCells.REF_DESCRIPTION]));
          }
        }
        else
        {
          bValid = false;
          uploadWebValidators.Add(
              new CustomError("Reference Description required row=" + sCurrentRow + " column=" +
                  OrderMapper.COLUMN_MAPPER[(int)OrderMapper.DetailCells.REF_DESCRIPTION]));
        }

        if (bValid)
        {
          bValid = dbValidate_DET_ReferenceType(detail, sCurrentRow);
        }
      }

      //cube
      if (csv[(int)OrderMapper.DetailCells.CUBE].ToString().Length > 0)
      {
        try
        {
          decimal decCube = Convert.ToDecimal(csv[(int)OrderMapper.DetailCells.CUBE].ToString());
          if (decCube > MAX_VOLUME)
          {
            bValid = false;
            uploadWebValidators.Add(
                new CustomError("Cube greater than maximum. row=" + sCurrentRow + " column=" +
                    OrderMapper.COLUMN_MAPPER[(int)OrderMapper.DetailCells.CUBE]));
          }
          detail.detail_cube = decCube;
          detail.detail_cubeSpecified = true;
        }
        catch (FormatException)
        {
          bValid = false;
          uploadWebValidators.Add(
              new CustomError("Cube must be numeric row=" + sCurrentRow + " column=" +
                  OrderMapper.COLUMN_MAPPER[(int)OrderMapper.DetailCells.CUBE]));
        }
      }

      //weight
      if (csv[(int)OrderMapper.DetailCells.WEIGHT].ToString().Length > 0)
      {
        try
        {
          decimal decDetWeight = Convert.ToDecimal(csv[(int)OrderMapper.DetailCells.WEIGHT].ToString());
          if (detail.detail_weight > MAX_WEIGHT)
          {
            bValid = false;
            uploadWebValidators.Add(
                new CustomError("Weight must greater than maximum. row=" + sCurrentRow + " column=" +
                    OrderMapper.COLUMN_MAPPER[(int)OrderMapper.DetailCells.WEIGHT]));
          }
          detail.detail_weight = decDetWeight;
          detail.detail_weightSpecified = true;
        }
        catch (FormatException)
        {
          bValid = false;
          uploadWebValidators.Add(
              new CustomError("Weight must be numeric row=" + sCurrentRow + " column=" +
                  OrderMapper.COLUMN_MAPPER[(int)OrderMapper.DetailCells.WEIGHT]));
        }
      }
      if (bValid == true)
      {
        //validate Product#/NMFC and Lot combo against the database
        int iAccountNumber = Convert.ToInt32(this.AccountNumber);
        if (!dbValidateProduct_Lot_AccountNumber(iAccountNumber, detail, sCurrentRow))
        {
          bValid = false;
        }
      }

      if (bValid)
        alOrderDetail.Add(detail);

      return bValid;
    }

    public bool validateAsn(CsvReader csv, string sCurrentRow)
    {
      bool bValid = true;
      asn headerAsn = new asn();

      //validate id
      if (csv[(int)OrderMapper.AsnCells.ASN_ID].ToString().Length > 0)
      {
        headerAsn.id = csv[(int)OrderMapper.AsnCells.ASN_ID].ToString();
        if (headerAsn.id.Length > 17)
        {
          bValid = false;
          uploadWebValidators.Add(
              new CustomError("Asn Id invalid length =" + sCurrentRow + " column=" +
                  OrderMapper.COLUMN_MAPPER[(int)OrderMapper.AsnCells.ASN_ID]));
        }
      }
      else
      {
        bValid = false;
        uploadWebValidators.Add(
            new CustomError("Asn Id required row=" + sCurrentRow + " column=" +
                OrderMapper.COLUMN_MAPPER[(int)OrderMapper.AsnCells.ASN_ID]));
      }

      //validate location number
      if (csv[(int)OrderMapper.AsnCells.LOCATION_NUMBER].ToString().Length > 0)
      {
        headerAsn.lu = csv[(int)OrderMapper.AsnCells.LOCATION_NUMBER].ToString();
        if (headerAsn.lu.Length > 30)
        {
          bValid = false;
          uploadWebValidators.Add(
              new CustomError("Location Number invalid length row=" + sCurrentRow + " column=" +
                  OrderMapper.COLUMN_MAPPER[(int)OrderMapper.AsnCells.LOCATION_NUMBER]));
        }
      }
      else
      {
        bValid = false;
        uploadWebValidators.Add(
            new CustomError("Location Number required row=" + sCurrentRow + " column=" +
                OrderMapper.COLUMN_MAPPER[(int)OrderMapper.AsnCells.LOCATION_NUMBER]));
      }

      //validate merchandise type code
      if (csv[(int)OrderMapper.AsnCells.MERCH_TYPE_CODE].ToString().Length > 0)
      {
        headerAsn.mr = csv[(int)OrderMapper.AsnCells.MERCH_TYPE_CODE].ToString();
        if (headerAsn.mr.Length > 30)
        {
          bValid = false;
          uploadWebValidators.Add(
              new CustomError("Merchandise Type Code invalid length row=" + sCurrentRow + " column=" +
                  OrderMapper.COLUMN_MAPPER[(int)OrderMapper.AsnCells.MERCH_TYPE_CODE]));
        }
      }
      else
      {
        bValid = false;
        uploadWebValidators.Add(
            new CustomError("Merchandise Type Code required row=" + sCurrentRow + " column=" +
                OrderMapper.COLUMN_MAPPER[(int)OrderMapper.AsnCells.MERCH_TYPE_CODE]));
      }

      //validate department number code
      if (csv[(int)OrderMapper.AsnCells.DEPARTMENT_NUMBER].ToString().Length > 0)
      {
        headerAsn.dp = csv[(int)OrderMapper.AsnCells.DEPARTMENT_NUMBER].ToString();
        if (headerAsn.dp.Length > 30)
        {
          bValid = false;
          uploadWebValidators.Add(
              new CustomError("Department number invalid length row=" + sCurrentRow + " column=" +
                  OrderMapper.COLUMN_MAPPER[(int)OrderMapper.AsnCells.DEPARTMENT_NUMBER]));
        }
      }
      else
      {
        bValid = false;
        uploadWebValidators.Add(
            new CustomError("Department number required row=" + sCurrentRow + " column=" +
                OrderMapper.COLUMN_MAPPER[(int)OrderMapper.AsnCells.DEPARTMENT_NUMBER]));
      }

      if (bValid)
      {
        if (order.asn == null)
          order.asn = headerAsn;
        else
        {
          bValid = false;
          uploadWebValidators.Add(
              new CustomError("Too many ASN records.  Only one ASN record allowed. row=" + sCurrentRow));
        }
      }

      return bValid;
    }

    //validate ProductNumber(item number in db),Lot number, AcccountNumber
    private bool dbValidateProduct_Lot_AccountNumber(int iAccountNumber, detail_type detail, string sCurrentRow)
    {
      bool isValid = true;

      string connString = ConfigurationManager.ConnectionStrings["WebOrderEntryConnectionString"].ConnectionString;
      try
      {
        using (SqlConnection myConnection = new SqlConnection(connString.ToString()))
        {
          myConnection.Open();

          SqlDataReader dr = null;

          SqlCommand myCommand = new SqlCommand();
          myCommand.Connection = myConnection;
          myCommand.CommandText = "sp_ItemValidation";// sItemSelect;
          myCommand.CommandType = System.Data.CommandType.StoredProcedure;
          myCommand.Parameters.Add("@ItemNumber", SqlDbType.VarChar, 22);
          myCommand.Parameters.Add("@LotNumber", SqlDbType.VarChar, 12);
          myCommand.Parameters.Add("@AccountNumber", SqlDbType.Int);

          myCommand.Parameters["@ItemNumber"].Value = detail.product_number_or_nmfc;
          myCommand.Parameters["@LotNumber"].Value = detail.lot_number;
          myCommand.Parameters["@AccountNumber"].Value = iAccountNumber;
          dr = myCommand.ExecuteReader();
          if (!dr.HasRows)
          {
            isValid = false;
            string sValidatorMessage = "Invalid Product#/NMFC, Lot, Account Number combination. Row=" + sCurrentRow +
                    " columns=" + OrderMapper.COLUMN_MAPPER[(int)OrderMapper.DetailCells.PRODUCT_NUMBER_NMFC] +
                    " and " + OrderMapper.COLUMN_MAPPER[(int)OrderMapper.DetailCells.LOT] +
                    "[ " + detail.product_number_or_nmfc + ", " + detail.lot_number + ", " + iAccountNumber + " ]" +
                    ". Error Code 10.";
            uploadWebValidators.Add(new CustomError(sValidatorMessage));
            string sAccountNumber = Convert.ToString(iAccountNumber);
            EventLog woeLog = new EventLog("WebOrderEntry");
            woeLog.Source = "WebOrderEntryApp";
            string errorMessage = "Message\r\n" +
                sValidatorMessage + "\r\n";
            errorMessage += "Product#/NMFC =" +
                detail.product_number_or_nmfc + "\r\n";
            errorMessage += "Lot =" +
                detail.lot_number + "\r\n";
            errorMessage += "Account Number =" +
                sAccountNumber + "\r\n\r\n";
            woeLog.WriteEntry(errorMessage, EventLogEntryType.Error, 10);

          }
          dr.Close();
        }
      }
      catch (SqlException e)
      {
        isValid = false;
        uploadWebValidators.Add(new CustomError("Error validating data. Contact customer support. Error code 2"));
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
        woeLog.WriteEntry(errorMessage, EventLogEntryType.Error, 2);
      }
      return isValid;
    }
    private bool dbValidateCityStateZip(address_type address, string sCurrentRow)
    {
      bool isValid = true;

      string connString = ConfigurationManager.ConnectionStrings["WebOrderEntryConnectionString"].ConnectionString;
      try
      {
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
          myCommand.Parameters["@City"].Value = address.city.ToUpper();
          myCommand.Parameters["@StateProvince"].Value =
              address.state_province.ToUpper();
          myCommand.Parameters["@Zip"].Value = address.postal_code.ToUpper();

          dr = myCommand.ExecuteReader();
          if (!dr.HasRows)
          {
            isValid = false;
            uploadWebValidators.Add(new CustomError("Invalid ShipFrom City, State, PostalCode combination.  Row=" + sCurrentRow +
                    " columns=" + OrderMapper.COLUMN_MAPPER[(int)OrderMapper.LocationCells.CITY] +
                    "," + OrderMapper.COLUMN_MAPPER[(int)OrderMapper.LocationCells.STATE_PROVINCE] +
                    ",and " + OrderMapper.COLUMN_MAPPER[(int)OrderMapper.LocationCells.POSTAL_CODE]));
          }
          dr.Close();
        }
      }
      catch (SqlException e)
      {
        isValid = false;
        uploadWebValidators.Add(new CustomError("Error validating data. Contact customer support. Error code 3"));
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
        woeLog.WriteEntry(errorMessage, EventLogEntryType.Error, 3);

      }


      return isValid;
    }

    private bool dbValidateSCAC(string ScacCode, string sCurrentRow)
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
          myCommand.CommandText = "sp_SCACValidation";
          myCommand.CommandType = System.Data.CommandType.StoredProcedure;
          myCommand.Parameters.Add("@ScacCode", SqlDbType.VarChar, 4);

          myCommand.Parameters["@ScacCode"].Value = ScacCode;

          dr = myCommand.ExecuteReader();
          if (!dr.HasRows)
          {
            isValid = false;    
            uploadWebValidators.Add(
                new CustomError("Invalid SCAC code. row=" + sCurrentRow +
                    " column=" + OrderMapper.COLUMN_MAPPER[(int)OrderMapper.HeaderCells.SCAC]));
          }
          dr.Close();
        }
      }
      catch (SqlException e)
      {
        isValid = false;
        uploadWebValidators.Add(new CustomError("Error validating data. Contact customer support. Error code 4"));
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
        woeLog.WriteEntry(errorMessage, EventLogEntryType.Error, 4);
      }
      return isValid;
    }

    //DET reference type and REF reference type share the same
    //datatbase table REFERENCE_QUALIFIER.  REF_TYPE field "D"
    //means it's a DET type
    private bool dbValidate_DET_ReferenceType(detail_type detail, string sCurrentRow)
    {
      bool bValid = false;
      bValid = dbValidateReferenceIdAndType(detail.reference.qualifier, "D", sCurrentRow);
      if (!bValid)
      {
        uploadWebValidators.Add(new CustomError("Invalid Reference Type. row=" + sCurrentRow +
                " column=" + OrderMapper.COLUMN_MAPPER[(int)OrderMapper.DetailCells.REF_TYPE]));
      }
      return bValid;
    }
    //DET reference type and REF reference type share the same
    //datatbase table REFERENCE_QUALIFIER. REF_TYPE field "H"
    //means it's a REF type
    private bool dbValidate_REF_ReferenceType(header_reference hdrRef, string sCurrentRow)
    {
      bool bValid = false;
      bValid = dbValidateReferenceIdAndType(hdrRef.qualifier, "H", sCurrentRow);
      if (!bValid)
      {
        uploadWebValidators.Add(new CustomError("Invalid Reference Type. row=" + sCurrentRow +
                " column=" + OrderMapper.COLUMN_MAPPER[(int)OrderMapper.ReferenceCells.TYPE]));
      }
      return bValid;
    }
    private bool dbValidateReferenceIdAndType(String sRefId, string typeHorD, string sCurrentRow)
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
          myCommand.CommandText = "sp_RefIdandTypeValidation";
          myCommand.CommandType = System.Data.CommandType.StoredProcedure;
          myCommand.Parameters.Add("@RefId", SqlDbType.VarChar, 22);
          myCommand.Parameters.Add("@RefType", SqlDbType.VarChar, 12);

          myCommand.Parameters["@RefId"].Value = sRefId;
          myCommand.Parameters["@RefType"].Value = typeHorD;

          dr = myCommand.ExecuteReader();
          if (!dr.HasRows)
          {
            isValid = false;
            //uploadWebValidators.Add(new CustomError("Invalid Reference Type. row=" + sCurrentRow +
            //        " column=" + OrderMapper.COLUMN_MAPPER[(int)OrderMapper.DetailCells.REF_TYPE]));
          }
          dr.Close();
        }
      }
      catch (SqlException e)
      {
        isValid = false;
        uploadWebValidators.Add(new CustomError("Error validating data. Contact customer support. Error code 5"));
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
        woeLog.WriteEntry(errorMessage, EventLogEntryType.Error, 5);

      }
      return isValid;
    }

    private bool dbValidateInstCodes(string InstCode, string sCurrentRow)
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
          myCommand.CommandText = "sp_InstCodesValidation";
          myCommand.CommandType = System.Data.CommandType.StoredProcedure;
          myCommand.Parameters.Add("@InstCode", SqlDbType.VarChar, 4);

          myCommand.Parameters["@InstCode"].Value = InstCode;

          dr = myCommand.ExecuteReader();
          if (!dr.HasRows)
          {
            isValid = false;
            uploadWebValidators.Add(
                new CustomError("Invalid Type code. row=" + sCurrentRow +
                    " column=" + OrderMapper.COLUMN_MAPPER[(int)OrderMapper.NoteCells.TYPE]));
          }
          dr.Close();
        }
      }
      catch (SqlException e)
      {
        isValid = false;
        uploadWebValidators.Add(new CustomError("Error validating data. Contact customer support. Error code 6"));
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

    public warehouse_shipping_ordersWarehouse_shipping_order getOrder(string sCurrentRow)
    {
      bool bValid = true;
      warehouse_shipping_ordersWarehouse_shipping_order builtOrder = null;
      string sFinalRow = (Convert.ToInt32(sCurrentRow) - 1).ToString();

      //check detail requirement
      if (alOrderDetail.Count > 0)
      {
        order.order_detail = new detail_type[alOrderDetail.Count];
        for (int x = 0; x < alOrderDetail.Count; x++)
        {
          order.order_detail[x] = (detail_type)alOrderDetail[x];
        }
        //clear out the detail array list
        alOrderDetail.Clear();

        for (int x = 0; x < order.order_detail.Length; x++)
        {
          //lline number was already validated as a numeric
          //so the conversion does not need to be surrounded by 
          //try/catch
          int iLineNumber =
              Convert.ToInt32(order.order_detail[x].line_number);
          if (iLineNumber != (x + 1))
          {
            bValid = false;
            uploadWebValidators.Add(
                new CustomError("Detail Line number invalid. Final row=" + sFinalRow));
          }
        }
      }
      else
      {
        bValid = false;
        uploadWebValidators.Add(
            new CustomError("No detail lines in this order. Final row=" + sFinalRow));
      }

      //load any header references into this order (optional)
      if (alOrderHeaderReference.Count > 0)
      {
        order.header_reference = new header_reference[alOrderHeaderReference.Count];
        for (int x = 0; x < alOrderHeaderReference.Count; x++)
        {
          order.header_reference[x] =
              (header_reference)alOrderHeaderReference[x];
        }
        //clear out the header reference array list
        alOrderHeaderReference.Clear();

      }

      //load any notes into this order (optional)
      if (alOrderNotes.Count > 0)
      {
        order.note = new note[alOrderNotes.Count];
        for (int x = 0; x < alOrderNotes.Count; x++)
        {
          order.note[x] = (note)alOrderNotes[x];
        }
        //clear out the notes array list
        alOrderNotes.Clear();

      }

      //check address requirements
      if (order.address[0] == null)
      {
        bValid = false;
        uploadWebValidators.Add(
            new CustomError("Order missing Ship From LOC record. Final row=" +
                sFinalRow));
      }

      if (order.address[1] == null)
      {
        bValid = false;
        uploadWebValidators.Add(
            new CustomError("Order missing Ship To LOC record. Final row=" +
                sFinalRow));
      }

      //add account information
      order.account_id = this.AccountId;
      order.account_number = this.AccountNumber;

      if (bValid)
        builtOrder = order;

      return builtOrder;

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
            this.AccountNumber = sAccountNumber;
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

  }
}
