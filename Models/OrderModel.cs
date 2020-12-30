using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosToWebPosBridge.Models
{
    /// <summary>
    /// Receipt Model for post
    /// </summary>
    public class Orders
    {
        /// <summary>
        /// Pos Id
        /// </summary>
        public Int64? ClientPosId { get; set; }//This

        /// <summary>
        /// Cover
        /// </summary>
        public int? Cover { get; set; }//This

        /// <summary>
        /// Post Day
        /// </summary>
        public DateTime? Day { get; set; }//--

        /// <summary>
        /// Department
        /// </summary>
        public String DepartmentDescription { get; set; }//--

        /// <summary>
        /// Department Id
        /// </summary>
        public Int64? DepartmentId { get; set; }//--

        /// <summary>
        /// Discount
        /// </summary>
        public Decimal? Discount { get; set; }//--

        /// <summary>
        /// Remarks for discount
        /// </summary>
        public String DiscountRemark { get; set; }//--

        /// <summary>
        /// Is Printed
        /// </summary>
        public bool? IsPrinted { get; set; }//--

        /// <summary>
        /// Modify
        /// </summary>
        public Int64? ModifyOrderDetails { get; set; }//--0 Default

        /// <summary>
        /// Pda 
        /// </summary>
        public Int64? PdaModuleId { get; set; }//This

        /// <summary>
        /// Pos Info
        /// </summary>
        public Int64? PosInfoId { get; set; }//--

        /// <summary>
        /// Sender
        /// </summary>
        public String Sender { get; set; }//--NULL

        /// <summary>
        /// Staff
        /// </summary>
        public String Staff { get; set; }//--First ' ' Last

        /// <summary>
        /// Staff Id
        /// </summary>
        public Int64? StaffId { get; set; }//--

        /// <summary>
        /// Table code
        /// </summary>
        public String TableCode { get; set; }//--

        /// <summary>
        /// Table Id
        /// </summary>
        public Int64? TableId { get; set; }//This

        /// <summary>
        /// Table sum
        /// </summary>
        public String TableSum { get; set; }//--

        /// <summary>
        /// Receipt details
        /// </summary>
        public List<OrdertDetails> ReceiptDetails { get; set; } //--

        /// <summary>
        /// Payments Details
        /// </summary>
        public List<PaymentsDetails> ReceiptPayments { get; set; }//--

        /// <summary>
        /// Billing Address
        /// </summary>
        public String BillingAddress { get; set; }//--

        /// <summary>
        /// Billing Address Id
        /// </summary>
        public Int64? BillingAddressId { get; set; }//--

        /// <summary>
        /// Billing City
        /// </summary>
        public String BillingCity { get; set; }//--

        /// <summary>
        /// Billing zip code
        /// </summary>
        public String BillingZipCode { get; set; }//--

        /// <summary>
        /// Customer Name
        /// </summary>
        public String CustomerName { get; set; }//--

        /// <summary>
        /// Remarks
        /// </summary>
        public String CustomerRemarks { get; set; }//--

        /// <summary>
        /// Floor
        /// </summary>
        public String Floor { get; set; }//--

        /// <summary>
        /// Latitude
        /// </summary>
        public double? Latitude { get; set; }//--

        /// <summary>
        /// Longtitude
        /// </summary>
        public double? Longtitude { get; set; }//--

        /// <summary>
        /// Phone
        /// </summary>
        public String Phone { get; set; }//--

        /// <summary>
        /// Shipping Address
        /// </summary>
        public String ShippingAddress { get; set; }//--

        /// <summary>
        /// Shipping Address Id
        /// </summary>
        public Int64? ShippingAddressId { get; set; }//--

        /// <summary>
        /// Shipping City
        /// </summary>
        public String ShippingCity { get; set; }//--

        /// <summary>
        /// Shippinf zip code
        /// </summary>
        public String ShippingZipCode { get; set; }//--

        /// <summary>
        /// Remarks for store
        /// </summary>
        public String StoreRemarks { get; set; }

        /// <summary>
        /// Counter for invoice
        /// </summary>
        public Int64? Counter { get; set; } //-- SELECT MAX()+1 ,* FROM PosInfoDetail pid WHERE pid.GroupId = 2 --AND pid.PosInfoId = 1

        /// <summary>
        /// Receipt no
        /// </summary>
        public long? ReceiptNo { get; set; }//This

        /// <summary>
        /// Invoice type id
        /// </summary>
        public Int64? InvoiceTypeId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Int16? InvoiceIndex { get; set; }//This

        /// <summary>
        /// Pos info detail
        /// </summary>
        public Int64? PosInfoDetailId { get; set; }//--

        /// <summary>
        /// Invoice description
        /// </summary>
        public String InvoiceDescription { get; set; }//--

        /// <summary>
        /// Invoice type type
        /// </summary>
        public Int16? InvoiceTypeType { get; set; }//--

        /// <summary>
        /// Invoie Abbreviation
        /// </summary>
        public String Abbreviation { get; set; }//--

        /// <summary>
        /// Create new transaction
        /// </summary>
        public bool? CreateTransactions { get; set; }//--

        /// <summary>
        /// Order No
        /// </summary>
        public String OrderNo { get; set; }//--

        /// <summary>
        /// Total amount
        /// </summary>
        public Decimal? Total { get; set; }//--

        /// <summary>
        /// Total before discount
        /// </summary>
        public Decimal? TotalBeforeDiscount { get; set; }

        /// <summary>
        /// Vat amount
        /// </summary>
        public Decimal? Vat { get; set; }//--

        /// <summary>
        /// Net Amount
        /// </summary>
        public Decimal? Net { get; set; }//--

        /// <summary>
        /// External key
        /// </summary>
        public String ExtKey { get; set; }

        /// <summary>
        /// External Type
        /// </summary>
        public int? ExtType { get; set; }

        /// <summary>
        /// External Object
        /// </summary>
        public String ExtObj { get; set; }

        //New Fields
        /// <summary>
        /// 
        /// </summary>
        public Int64? Id { get; set; }//--NULL

        /// <summary>
        /// End Of Day
        /// </summary>
        public Int64? EndOfDayId { get; set; }//--NULL

        /// <summary>
        /// FO Day
        /// </summary>
        public DateTime? FODay { get; set; }//--NULL

        /// <summary>
        /// Pos info description
        /// </summary>
        public String PosInfoDescription { get; set; }//--Descr Of PosInfo

        /// <summary>
        /// Staff code
        /// </summary>
        public String StaffCode { get; set; }//--

        /// <summary>
        /// Staff Name
        /// </summary>
        public String StaffName { get; set; }//--First ' ' Last

        /// <summary>
        /// If is voided
        /// </summary>
        public bool? IsVoided { get; set; }//--False Defalut

        /// <summary>
        /// Room no
        /// </summary>
        public String Room { get; set; }//--NULL

        /// <summary>
        /// Payment description
        /// </summary>
        public String PaymentsDesc { get; set; }//--NULL

        /// <summary>
        /// If is paid
        /// </summary>
        public Int16? IsPaid { get; set; }//--0 Default

        /// <summary>
        /// Paid total
        /// </summary>
        public Decimal? PaidTotal { get; set; }//--0 Default

        /// <summary>
        /// Staff Last Name
        /// </summary>
        public String StaffLastName { get; set; }//--NULL

        /// <summary>
        /// Origin order no
        /// </summary>
        public int? OrderOrigin { get; set; }


        public string BillingName { get; set; }
        public string BillingVatNo { get; set; }
        public string BillingDOY { get; set; }
        public string BillingJob { get; set; }

        public long? CustomerID { get; set; }

        /// <summary>
        /// datetime to send order to customer (appointment)
        /// </summary>
        public DateTime? EstTakeoutDate { get; set; }

        /// <summary>
        /// set order as appointment order
        /// </summary>
        public bool? IsDelay { get; set; }
    }

    public class OrdersExternal:Orders
    {
        public string WebCode { get; set; }

        public short? WebType { get; set; }

        public short? AccountType { get; set; }

    }

    /// <summary>
    /// Receipt detaiol Model
    /// </summary>
    public class OrdertDetails
    {
        /// <summary>
        /// Item Description
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// Item discount
        /// </summary>
        public Double? Discount { get; set; } //This

        /// <summary>
        /// Item Guid
        /// </summary>
        public Guid? Guid { get; set; }

        /// <summary>
        /// Item is extra
        /// </summary>
        public bool? IsExtra { get; set; } //This

        /// <summary>
        /// Item is changed
        /// </summary>
        public bool? IsChangeItem { get; set; } //This

        /// <summary>
        /// Item remarks
        /// </summary>
        public String ItemRemark { get; set; } //This

        /// <summary>
        /// Item code
        /// </summary>
        public String ItemCode { get; set; }//This

        /// <summary>
        /// Item description
        /// </summary>
        public String ItemDescr { get; set; }

        /// <summary>
        /// Item discount
        /// </summary>
        public Double? ItemDiscount { get; set; } //This

        /// <summary>
        /// Item price
        /// </summary>
        public Double? ItemPrice { get; set; } //This

        /// <summary>
        /// Item Qty
        /// </summary>
        public Double? ItemQty { get; set; } //This

        /// <summary>
        /// Item region
        /// </summary>
        public String ItemRegion { get; set; }

        /// <summary>
        /// Item vat rate
        /// </summary>
        public Double? ItemVatRate { get; set; } //This

        /// <summary>
        /// Item kds id
        /// </summary>
        public long? KdsId { get; set; } //This

        /// <summary>
        /// Item kitchen code
        /// </summary>
        public String KitchenCode { get; set; }

        /// <summary>
        /// Item kitchen id
        /// </summary>
        public int? KitchenId { get; set; } //This

        /// <summary>
        /// Item Order Id
        /// </summary>
        public long? OrderId { get; set; } //This

        /// <summary>
        /// Pos info id
        /// </summary>
        public long? PosInfoId { get; set; } //This

        /// <summary>
        /// Order Prepartion time
        /// </summary>
        public Int16? PreparationTime { get; set; } //This

        /// <summary>
        /// Order Previous Status
        /// </summary>
        public int? PreviousStatus { get; set; }

        /// <summary>
        /// Item Total Price
        /// </summary>
        public Double? Price { get; set; } //This

        /// <summary>
        /// Item Price List Detail Id
        /// </summary>
        public long? PriceListDetailId { get; set; } //This

        /// <summary>
        /// Item Price List Id
        /// </summary>
        public long? PriceListId { get; set; } //This

        /// <summary>
        /// Item Product Category
        /// </summary>
        public long? ProductCategoryId { get; set; } //This

        /// <summary>
        /// Item Product Id
        /// </summary>
        public long? ProductId { get; set; } //This

        /// <summary>
        /// Item Qty
        /// </summary>
        public Double? Qty { get; set; }

        /// <summary>
        /// Receipt discount
        /// </summary>
        public Double? ReceiptSplitedDiscount { get; set; }//This

        /// <summary>
        /// Receipt region
        /// </summary>
        public long? RegionId { get; set; } //This

        /// <summary>
        /// Receipt region position
        /// </summary>
        public int? RegionPosition { get; set; }//This

        /// <summary>
        /// Sales type Descr
        /// </summary>
        public String SalesTypeExtDesc { get; set; }

        /// <summary>
        /// Sales type Id
        /// </summary>
        public long? SalesTypeId { get; set; } //This

        /// <summary>
        /// Receipt staff id
        /// </summary>
        public long? StaffId { get; set; } //This

        /// <summary>
        /// Receipt table code
        /// </summary>
        public String TableCode { get; set; }

        /// <summary>
        /// Receipt table id
        /// </summary>
        public long? TableId { get; set; } //This

        /// <summary>
        /// Receipt tax id
        /// </summary>
        public long? TaxId { get; set; } //This

        /// <summary>
        /// Item total after discount
        /// </summary>
        public Double? TotalAfterDiscount { get; set; } //This

        /// <summary>
        /// Item total before discount
        /// </summary>
        public Double? TotalBeforeDiscount { get; set; }

        /// <summary>
        /// Item vat code
        /// </summary>
        public long? VatCode { get; set; } //This

        /// <summary>
        /// Item vat descr
        /// </summary>
        public String VatDesc { get; set; }//This

        /// <summary>
        /// Item vat id 
        /// </summary>
        public long? VatId { get; set; } //This

        /// <summary>
        /// Receipt Order no
        /// </summary>
        public long? OrderNo { get; set; } //This

        /// <summary>
        /// Item short position
        /// </summary>
        public int? ItemSort { get; set; }

        /// <summary>
        /// Receipt Invoice Abbreviation
        /// </summary>
        public String Abbreviation { get; set; }

        /// <summary>
        /// Receipt Invoice Type
        /// </summary>
        public long? InvoiceType { get; set; } //This

        /// <summary>
        /// Receipt Pos Info detail
        /// </summary>
        public long? PosInfoDetailId { get; set; } //This

        /// <summary>
        /// Receipt Paid ststus
        /// </summary>
        public int? PaidStatus { get; set; } //This

        /// <summary>
        /// Receipt status
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// Item gross
        /// </summary>
        public Double? ItemGross { get; set; }//This

        /// <summary>
        /// Item vat amount
        /// </summary>
        public Double? ItemVatValue { get; set; }//This

        /// <summary>
        /// Item net amount
        /// </summary>
        public Double? ItemNet { get; set; } //This
        
        //New Fields
        /// <summary>
        /// Receipt detail id
        /// </summary>
        public int? OrderDetailId { get; set; }//This Product Or Igredients ID

        /// <summary>
        /// Receipt id
        /// </summary>
        public long? ReceiptId { get; set; }//This NULL

        /// <summary>
        /// Receipt No
        /// </summary>
        public long? ReceiptNo { get; set; }//This NULL

        /// <summary>
        /// 
        /// </summary>
        public long? Id { get; set; }//This ???

        /// <summary>
        /// Ingredients Id
        /// </summary>
        public long? OrderDetailIgredientsId { get; set; }//This Igredients ID else null
        
        //public int? ItemPosition { get; set; } //This 
        //public int? ItemBarcode { get; set; }//This

        /// <summary>
        /// Item Id for Master Product
        /// </summary>
        public long? ItemId { get; set; }

        /// <summary>
        /// cont_line from hitposorder. If 0 then extras to first master product
        /// </summary>
        public long? cont_line { get; set; }
    }

    /// <summary>
    /// Receipt Payments Detail
    /// </summary>
    public class PaymentsDetails
    {
        public Double? PreviousAmount { get; set; } //This
        public Double? Amount { get; set; } //This
        public String OrderNo { get; set; } //This
        public long? AccountId { get; set; } //This
        public String Description { get; set; } //--
        public String AccountDescription { get; set; }//--
        public Int16? AccountType { get; set; } //This
        public long? PosInfoId { get; set; } //This
        public long? StaffId { get; set; } //This
        public bool? SendsTransfer { get; set; }//--
        //public String CreditAccountId { get; set; }
        //public String CreditCodeId { get; set; }
        //public String CreditAccountDescription { get; set; }
        //public String NewCreditBalance { get; set; }
        public Double? Percentage { get; set; } //This

        //New Fields
        /// <summary>
        /// Gust Id
        /// </summary>
        public long? GuestId { get; set; } //This  Customer ID

        //public CustomerModel Guest { get; set; }
        /// <summary>
        /// Guest Name and Last Name
        /// </summary>
        public string Guest { get; set; }

        /// <summary>
        /// Hotel Id
        /// </summary>
        public long? HotelId { get; set; }//This NULL

        /// <summary>
        /// Is Default Guest
        /// </summary>
        public bool? isDefault { get; set; } //This false

        /// <summary>
        /// Room no
        /// </summary>
        public String Room { get; set; }//This NULL
    }
    
    /// <summary>
    /// Extenral Order Info for New Order
    /// </summary>
    public class ExtObj
    {
        /// <summary>
        /// Order No
        /// </summary>
        public String OrderNo { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// Invoice code
        /// </summary>
        public String InvoiceCode { get; set; }

        /// <summary>
        /// Invoice Type
        /// </summary>
        public Int16? InvoiceType { get; set; }

        /// <summary>
        /// Account Type
        /// </summary>
        public Int16? AccountType { get; set; }
    }

    /// <summary>
    /// Model for new Ingredients
    /// </summary>
    public class Ingredients
    {
        /// <summary>
        /// Description
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// External description for other systems
        /// </summary>
        public String ExtendedDescription { get; set; }

        /// <summary>
        /// Sales description
        /// </summary>
        public String SalesDescription { get; set; }

        /// <summary>
        /// Qty
        /// </summary>
        public float? Qty { get; set; }

        /// <summary>
        /// Item Id
        /// </summary>
        public Int64? ItemId { get; set; }

        /// <summary>
        /// Unit Id
        /// </summary>
        public Int64? UnitId { get; set; }

        /// <summary>
        /// Code
        /// </summary>
        public String Code { get; set; }

        /// <summary>
        /// Is deleted
        /// </summary>
        public byte? IsDeleted { get; set; }

        /// <summary>
        /// Background
        /// </summary>
        public String Background { get; set; }

        /// <summary>
        /// Color
        /// </summary>
        public String Color { get; set; }
    }

    /// <summary>
    /// Mater Product to use for Extras
    /// </summary>
    public class MasterProductProperties
    {
        /// <summary>
        /// Kds Is
        /// </summary>
        public Int64? KdsId { get; set; }

        /// <summary>
        /// Kitchen code
        /// </summary>
        public String KitchenCode { get; set; }

        /// <summary>
        /// Kitchen Id
        /// </summary>
        public int? KitchenId { get; set; }

        /// <summary>
        /// Preperation Time
        /// </summary>
        public Int16? PreparationTime { get; set; }

        /// <summary>
        /// Price List Detail Id
        /// </summary>
        public Int64? PriceListDetailId { get; set; }

        /// <summary>
        /// Price List Id
        /// </summary>
        public Int64? PriceListId { get; set; }

        /// <summary>
        /// Product Category Id
        /// </summary>
        public Int64? ProductCategoryId { get; set; }

        /// <summary>
        /// Qty
        /// </summary>
        public Double? Qty { get; set; }

        /// <summary>
        /// Item Qty
        /// </summary>
        public Double? ItemQty { get; set; }

        /// <summary>
        /// Item Id
        /// </summary>
        public long? ItemId { get; set; }
    }

    /// <summary>
    /// Association between HitPos Sales Id and WebPos Sales Id
    /// </summary>
    public class SalesTypeMap
    {
        /// <summary>
        /// Hit Sales Id
        /// </summary>
        public String HitSales { get; set; }

        /// <summary>
        /// WepPos Sales
        /// </summary>
        public Int64 WebSales { get; set; }

        /// <summary>
        /// Pos info to download the order
        /// </summary>
        public Int64 PosInfoId { get; set; }

        /// <summary>
        /// Price list per pos info
        /// </summary>
        public Int64 PricelistId { get; set; }

    }

    /// <summary>
    /// Association between HitPos Invoice Type and WepPos Invoice Type
    /// </summary>
    public class InvoiceTypesMap
    {
        /// <summary>
        /// HitPos Invoice Type Code
        /// </summary>
        public String HitCode { get; set; }

        /// <summary>
        /// WepPos Invoice Type Code
        /// </summary>
        public String WebCode { get; set; }

        /// <summary>
        /// WepPos Invoice Type
        /// </summary>
        public Int16 WebType { get; set; }
    }





    /// <summary>
    /// Customer Model For Receipt (Not In Use)
    /// </summary>
    public class CustomerModel
    {
        public long? Id { get; set; }
        public int? ReservationId { get; set; }
        public bool? IsSharer { get; set; }
        public DateTime? arrivalDT { get; set; }
        public DateTime? departureDT { get; set; }
        public DateTime? birthdayDT { get; set; }
        public String Room { get; set; }
        public int? RoomId { get; set; }
        public String Arrival { get; set; }
        public String Departure { get; set; }
        public String ReservationCode { get; set; }
        public int? ProfileNo { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Member { get; set; }
        public String Password { get; set; }
        public String Address { get; set; }
        public String City { get; set; }
        public String PostalCode { get; set; }
        public String Country { get; set; }
        public String Birthday { get; set; }
        public String Email { get; set; }
        public String Telephone { get; set; }
        public String VIP { get; set; }
        public String Benefits { get; set; }
        public String NationalityCode { get; set; }
        public String ConfirmationCode { get; set; }
        public int? Type { get; set; }
        public String Title { get; set; }
        public int? Adults { get; set; }
        public int? Children { get; set; }
        public String BoardCode { get; set; }
        public String BoardName { get; set; }
        public String Note1 { get; set; }
        public String Note2 { get; set; }
        public Double? AllowdDiscount { get; set; }
        public Double? AllowdDiscountChild { get; set; }
        public int? AllowedAdultMeals { get; set; }
        public int? AllowedChildMeals { get; set; }
        public int? ConsumedMeals { get; set; }
        public int? ConsumedMealsChild { get; set; }
        public int? ConsumedNow { get; set; }
        public int? ConsumedNowChild { get; set; }
        public long? HotelId { get; set; }
    }

}
