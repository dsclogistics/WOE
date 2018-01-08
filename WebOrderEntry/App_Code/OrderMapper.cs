using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App_Code
{
    public static class OrderMapper
    {
        public enum HeaderCells
        {
            HDR_SEGMENT_NAME, STATUS, CUSTOMER_REF_NUMBER, CONSIGNEE_PO, EXPORT, TEMP_CONTROL,
            HAZMAT, PAYMENT_METHOD, TRANSPORTATION_METHOD, SCAC, TOTAL_WEIGHT, TOTAL_WEIGHT_UOM,
            TOTAL_VOLUME, TOTAL_VOLUME_UOM, SHIP_DATE, DELIVERY_DATE, UCC128
        };

        //added LOCATION_ID release 1.0.0.2 02-16-2009 map 
        public enum LocationCells
        {
            LOC_SEGMENT_NAME, LOCATION_ID, TYPE, NAME, ADDRESS1, ADDRESS2, CITY,
            STATE_PROVINCE, POSTAL_CODE
        };
        public enum DetailCells
        {
            DET_SEGMENT_NAME, LINE_NUMBER, PRODUCT_NUMBER_NMFC, QUANTITY, LOT, REF_TYPE,
            REF_NUMBER, REF_DESCRIPTION, CUBE, WEIGHT,
        };

        public enum AsnCells
        {
            ASN_SEGMENT_NAME, ASN_ID, LOCATION_NUMBER, MERCH_TYPE_CODE,
            DEPARTMENT_NUMBER
        };
        public enum ReferenceCells { REF_SEGMENT_NAME, TYPE, NAME };
        public enum NoteCells { NOT_SEGMENT_NAME, TYPE, NAME };

        static public string[] COLUMN_MAPPER = {"A","B","C","D","E","F","G","H","I","J",
                                              "K","L","M","N","O","P","Q","R","S","T",
                                              "U","V","W","X","Y","Z"};



    }
}
