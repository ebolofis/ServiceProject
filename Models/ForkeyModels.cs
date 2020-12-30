using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosToWebPosBridge.Models
{

    public class ForkeyDeliveryOrder
    {
        public long id { get; set; }
        // Customer Fields 
        // Unique identification to match external system
        public string user_id { get; set; }
        public string user_name { get; set; }
        public string user_email { get; set; }
        public string user_tel { get; set; }

        // DeliveryCustomer Address Fields
        public ForkeyDeliveryAddress delivery_address { get; set; }

        // list of order items 
        public List<ForkeyDishes> dishes { get; set; }

        // Order Status 
        // downloaded,printed,assigned,delivered,cancelled
        public string status { get; set; }
        public string error_reason { get; set; }

        public Nullable<bool> vip { get; set; }
        public Nullable<long> version { get; set; }
        public Nullable<decimal> weight { get; set; }
        public string venue_id { get; set; }

        public string payment_method { get; set; }
        public string order_source { get; set; }
        public string invoice_type { get; set; }
        // Extra
        public string invoice_name { get; set; }
        // Extra
        public string invoice_profession { get; set; }
        // Extra
        public string invoice_address { get; set; }
        // Extra ΑΦΜ
        public string invoice_vat_num { get; set; }
        // Extra ΔΟΥ
        public string invoice_tax_office { get; set; }
        // Transaction details 
        public Nullable<decimal> transaction_money { get; set; }
        public Nullable<decimal> promo_money { get; set; }
        public Nullable<decimal> credit_money { get; set; }
        public Nullable<decimal> prepaid_money { get; set; }
        public Nullable<decimal> bundle_money { get; set; }
        public Nullable<decimal> online_payment_money { get; set; }
        public Nullable<decimal> preorder_money { get; set; }
        public Nullable<decimal> online_payment_fee { get; set; }
        public Nullable<decimal> transaction_money_with_promo { get; set; }
        public Nullable<decimal> amount_payable { get; set; }
        public Nullable<decimal> vat { get; set; }

        //Unused Vars to parse in model
        public Nullable<bool> bad_weather { get; set; }
        public Nullable<bool> closed_roads { get; set; }
        public Nullable<bool> with_couvert { get; set; }

        public Nullable<DateTime> delivered_at { get; set; }
        public Nullable<DateTime> active_at { get; set; }
        public Nullable<DateTime> started_at { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public Nullable<DateTime> updated_at { get; set; }

        public Nullable<DateTime> arrived_at { get; set; }
        public Nullable<DateTime> cancelled_at { get; set; }
        public Nullable<DateTime> printed_at { get; set; }
        public Nullable<DateTime> deleted_at { get; set; }
        public Nullable<DateTime> assigned_at { get; set; }
        public string cancel_reason { get; set; }
        public string time_of_day { get; set; }

        //"customer_comms_at": null,
        public string discount_string { get; set; }
        //": "50%",
        //"prepaid_money_source": null,
        //"eta": null,
        //"order_source": "WEB",
        //"timeslot": null,
        //"update_channel":"aab083be2a5485a10ea96399b3848631",
        //"days_since_last_order":0,
        //"user_watchlist":{
        //"id": null,
        //"in_watchlist": false,
        //"watchlist_note": null
        //},
        //"assigned_timeslot":{
        //	"id":28,
        //	"start_time":"21:00:00",
        //	"end_time":"21:20:00",
        //	"active":false,
        //	"time_of_day":"LUNCH"
        //},
        public ForkeyTimeslot assigned_timeslot { get; set; }
        public ForkeyTimeslot timeslot { get; set; }
        public ForkeyPosDependencies dependencies { get; set; }
    }

    /// <summary>
    /// Property of forkey order to define weather order is with randezvous 
    /// or not this information is binded to external object 
    /// to define start and end time of current order
    /// </summary>
    public class ForkeyTimeslot
    {
        long? id { get; set; }
        public Nullable<TimeSpan> start_time { get; set; }
        public Nullable<TimeSpan> end_time { get; set; }
        public Nullable<bool> active { get; set; }
        public string time_of_day { get; set; }
    }

    /// <summary>
    /// Data from config file
    /// </summary>
    public class ForkeyPosDependencies
    {
        public long PosInfoId { get; set; }
        public long PosInfoDetailId { get; set; }
        public long StaffId { get; set; }
        public long InvoiceTypeId { get; set; }
        public long PricelistId { get; set; }
        public long SalesTypeId { get; set; }
        public long AccountId { get; set; }
    }

    // Order Item Details and Information 
    // Has match Code with Product and description
    public class ForkeyRecipe
    {
        long id { get; set; }
        // WebPOS Product.Code
        public string external_id { get; set; }
        public string name { get; set; }
        public string code_name { get; set; }
        public string description { get; set; }
        // Not needed items 
        //public string picture { get; set; }
        //public List<object> tags { get; set; }
        //public Nullable<bool> default_is_discounted { get; set; }
        //public Nullable<long> calories { get; set; }
        //public string color_hex { get; set; }
    }
    // Forkey Order Item master Object 
    public class ForkeyDishes
    {
        long id { get; set; }
        public ForkeyDish dish { get; set; }
        public long portions { get; set; }
        public Nullable<decimal> preorder_cost { get; set; }

    }

    // Dish Details  Order Item details
    public class ForkeyDish
    {
        long id { get; set; }
        public string category { get; set; }
        public string venue_id { get; set; }

        // Not needed items 
        //public Nullable<DateTime> date { get; set; }
        //public Nullable<bool> is_discounted { get; set; }

        public ForkeyRecipe recipe { get; set; }
        public Nullable<decimal> cost { get; set; }

        public Nullable<long> max_portions_available { get; set; }
        public Nullable<bool> is_discounted { get; set; }
        public Nullable<decimal> preorder_price_1 { get; set; }
        public Nullable<decimal> preorder_price_2 { get; set; }
        public Nullable<decimal> preorder_price_3 { get; set; }
        public Nullable<decimal> preorder_price_5 { get; set; }

        //// Not needed items 
        //public Nullable<bool> premature { get; set; }
        //public Nullable<bool> expired { get; set; }
        public Nullable<bool> is_main { get; set; }

        //public string time_of_day { get; set; }
        //public string color_hex { get; set; }
    }

    // Forkey General Order Model 



    /// <summary>
    /// Forkey DEivery Address Model Contains info of Receipt 
    /// Contains Doc Type and refers to Point on map 
    /// </summary>
    public class ForkeyDeliveryAddress
    {
        public long id { get; set; }
        public string address { get; set; }
        public string addr_num { get; set; }
        public string town { get; set; }
        // Extra field
        public string specific_indication { get; set; }
        public string google_address { get; set; }
        public string zip_code { get; set; }
        public string bell { get; set; }
        public string flat { get; set; }
        // Lat long Sist provided by address and map
        // RECEIPT Delivery document type !?!? 
        public string document_type { get; set; }
        // Store Identification
        public string venue_id { get; set; }
        //Address Map fields 
        public ForkeyPoint point { get; set; }

        public string comments { get; set; }

        /// <summary>
        /// Shipping Company Name
        /// </summary>
        public string company_name { get; set; }
        // Not needed items 
        //public Nullable<bool> with_couvert { get; set; }
        //public Nullable<bool> is_default { get; set; }
        //public Nullable<bool> within_range { get; set; }
        //public Nullable<bool> within_range_LUNCH { get; set; }
        //public Nullable<bool> within_range_DINNER { get; set; }
        //"municipality": null,

        //"within_range": true,
        //"within_range_l_u_n_c_h": null,
        //"within_range_d_i_n_n_e_r": null,
        //"point_accuracy": null,
        //"within_range_LUNCH": true,
        //"within_range_DINNER": true
    }

    /// <summary>
    /// Point on MAp provideing lat long and distance from shop
    /// extend of ForkeyDeliveryAddress 
    /// </summary>
    public class ForkeyPoint
    {
        public Nullable<decimal> latitude { get; set; }
        public Nullable<decimal> longitude { get; set; }
        public Nullable<long> distance { get; set; }
    }

    /// <summary>
    /// Model send to the Patch Rest Call to Forky's server
    /// </summary>
    public class ForkeyPatchOrderModel
    {
        /// <summary>
        /// The new order's status
        /// </summary>
        public string status { get; set; }
    }
}
