using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosToWebPosBridge.Models
{
    public class SettingsModel
    {
        public Int64 InsertIngr { get; set; }

        public string WebApiURL { get; set; }

        public string URLUserName { get; set; }

        public string URLPass { get; set; }

        public string ShopId { get; set; }

        public string ShopInfoId { get; set; }

        public string ShopAddress { get; set; }

        public string ShopCity { get; set; }

        public double ShopLatitude { get; set; }

        public double ShopLongtitude { get; set; }

        public Int64 PosInfoId { get; set; }

        public Int64 PriceListId { get; set; }

        public Int64 StaffId { get; set; }

        public Int64 InvoiceType { get; set; }

        public Int64 CashId { get; set; }

        public Int64 CreditCardId { get; set; }

        public int ExtType { get; set; }

        public int CheckCount { get; set; }


        public string WebPosServer { get; set; }

        public string WebPosUser { get; set; }

        public string WebPosPass { get; set; }

        public string WebPosDB { get; set; }

        public string HitPosServer { get; set; }

        public string HitPosUser { get; set; }

        public string HitPosPass { get; set; }

        public string HitPosDB { get; set; }

        public List<SalesTypeMap> SalesTypeMapped { get; set; }

        public List<InvoiceTypesMap> InvoiceTypesMapped { get; set; }
        

        public string ForkeyURL { get; set; }

        public string ForkeyUserHeader { get; set; }

        /// <summary>
        /// Type Id for Invoice
        /// </summary>
        public Nullable<Int64> ForkeyInvoiceType { get; set; }

        /// <summary>
        /// Type Id for Receipt
        /// </summary>
        public Nullable<Int64> ForkeyReceiptType { get; set; }

        /// <summary>
        /// Type Id for Sales Type
        /// </summary>
        public Nullable<Int64> ForkeySalesTypeId { get; set; }

        /// <summary>
        /// Id for PosInfoDetail
        /// </summary>
        public Nullable<Int64> ForkyPosInfoCaptensOrder { get; set; }

        /// <summary>
        /// Set an order as delay after minute
        /// </summary>
        public int SetDelayAfterMinutes { get; set; }

    }
}
