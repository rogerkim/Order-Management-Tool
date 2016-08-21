using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangoOrderImporter
{
    public class SQL
    {
        public static readonly string CONSTRING = "Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;";

        public static readonly string INSERT_ORDERS = "insert into [dbo].[orders]" +
            "(orderkey, StorerKey, Facility, ExternOrderKey, M_Company, OrderDate, Type, Userdefine03, Salesman, PmtTerm" +
            ",ShipperKey, C_Company, C_Contact1, C_Zip, C_Phone1, C_Address2, C_Address3, C_Address4, Notes)" +
            "values(@OrderId, 'MANGOKR', 'Mango', @MangoOrderId, @MangoOrderId, @OrderDate, '2' , 'Mango', 'Mango.com', 'CC'" +
            ",'CJKE', @MangoOrderId, @Name, @Zipcode, @Mobile, @Address1, @Address2, @Address3, N'망고(의류)');";

        public static readonly string CHECK_ORDER = "SELECT COUNT(ExternOrderKey) FROM [dbo].[orders] WHERE ExternOrderKey = @MangoOrderId;";

        public static readonly string INESRT_CARTON = "INSERT INTO [dbo].[CartonShipmentDetail]" +
            "(storerkey, ExternOrderKey, CarrierCode, UCCLabelNo, TrackingNumber)" +
            "values('MANGOKR', @MangoOrderId,'CJKE' , @LineNo', @TrackingNumber);";
    }
}
