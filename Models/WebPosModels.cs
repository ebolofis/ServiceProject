using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosToWebPosBridge.Models
{
    /// <summary>
    /// Model For Product Categories from Products
    /// </summary>
    public class WebPosProductsProductCategoriesModels
    {

        public Int32? ProductCategoryId { get; set; }

        public Int32? KdsId { get; set; }
    }

    /// <summary>
    /// Model For OrderStatuses for Orders
    /// </summary>
    public class OrdersOrderStatusModel
    {
        public int? MatchValue { get; set; }

        public string ExtKey { get; set; }

        public DateTime? TimeChanged { get; set; }

        public long? Id { get; set; }
    }

    //public class OrderDetailsModel
    //{
    //    public int? Id { get; set; }

    //    public string Description { get; set; }

    //    public double? Discount { get; set; }

    //    public Guid? Guid { get; set; }

    //    public bool? IsExtra { get; set; }

    //    public bool? IsChangeItem { get; set; }

    //    public string ItemCode { get; set; }

    //    public string ItemDescr { get; set; }

    //    public double? ItemDiscount { get; set; }

    //    public double? ItemPrice { get; set; }

    //    public double? ItemQty { get; set; }

    //    public string ItemRegion { get; set; }

    //    public double? ItemVatRate { get; set; }

    //    public Int64? KdsId { get; set; }

    //    public string KitchenCode { get; set; }

    //    public int? KitchenId { get; set; }

    //    public Int64? PosInfoId { get; set; }

    //    public short? PreparationTime { get; set; }

    //    public int? PreviousStatus { get; set; }

    //    public double? Price { get; set; }

    //    public Int64? PriceListDetailId { get; set; }

    //    public Int64? PriceListId { get; set; }

    //    public Int64? ProductCategoryId { get; set; }

    //    public Int64? ProductId { get; set; }

    //    public double? Qty { get; set; }

    //    public double? ReceiptSplitedDiscount { get; set; }

    //    public Int64? RegionId { get; set; }

    //    public int? RegionPosition { get; set; }

    //    public string SalesTypeExtDesc { get; set; }

    //    public Int64? SalesTypeId { get; set; }

    //    public Int64? StaffId { get; set; }

    //    public string TableCode { get; set; }

    //    public Int64? TableId { get; set; }

    //    public double? TotalAfterDiscount { get; set; }

    //    public double? TotalBeforeDiscount { get; set; }

    //    public Int64? VatCode { get; set; }

    //    public string VatDesc { get; set; }

    //    public Int64? VatId { get; set; }


    //}
}
