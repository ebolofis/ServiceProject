using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosToWebPosBridge.Models
{
    /// <summary>
    /// Ingredients Model from hitpos db
    /// </summary>
    public class HitPosIngredientsModels
    {
        /// <summary>
        /// Code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Exctented Description
        /// </summary>
        public string ExtendedDescription { get; set; }

        /// <summary>
        /// Sales Description
        /// </summary>
        public string SalesDescription { get; set; }


    }

    /// <summary>
    /// Hit Pos Order Model
    /// </summary>
    public class HitPosOrderModel
    {
        public int? newAmount { get; set; }

        public int? id { get; set; }

        public int? orderno { get; set; }

        public int? pos { get; set; }

        public string shop_id { get; set; }

        public int? item_group { get; set; }

        public int? item_code { get; set; }

        public string item_descr { get; set; }

        public int? item_subgroup { get; set; }

        public int? item_vat { get; set; }

        public decimal? cont_line { get; set; }

        public string sp { get; set; }

        public DateTime? prep_time { get; set; }

        public DateTime? start_time { get; set; }

        public int? load_time { get; set; }

        public int? otd { get; set; }

        public int? qty { get; set; }

        public int? amount { get; set; }

        public int? total { get; set; }

        public int? waiter { get; set; }

        public int? ttable { get; set; }

        public int? listino { get; set; }

        public int? receipt { get; set; }

        public string member { get; set; }

        public int? priority { get; set; }

        public int? kdws { get; set; }

        public int? ready { get; set; }

        public int? rqty { get; set; }

        public int? nieko_flag { get; set; }

        public int? status { get; set; }

        public DateTime? status_time { get; set; }

        public int? rest_time { get; set; }

        public string room { get; set; }

        public string payment { get; set; }

        public string type { get; set; }

        public string comments { get; set; }

        public int? mqty { get; set; }

        public DateTime? rec_time_start { get; set; }

        public DateTime? status_time2 { get; set; }

        public DateTime? status_time3 { get; set; }

        public DateTime? status_time4 { get; set; }

        public DateTime? status_time5 { get; set; }

        public DateTime? fo_day { get; set; }

        public DateTime? delivery_time { get; set; }

        public int? agent { get; set; }

        public int? flag_up { get; set; }

        public int? sent { get; set; }

        public int? correct { get; set; }
    }

    /// <summary>
    /// hit Pos Customers Model
    /// </summary>
    public class HitPosCustomersModel
    {
        public string customerid { get; set; }

        public string name { get; set; }

        public string fname { get; set; }

        public string title { get; set; }

        public string profession { get; set; }

        public string tel1 { get; set; }

        public string tel2 { get; set; }

        public string fax { get; set; }

        public string mobile { get; set; }

        public string address1 { get; set; }

        public string address2 { get; set; }

        public string address_no { get; set; }

        public string orofos1 { get; set; }

        public string orofos2 { get; set; }

        public string city { get; set; }

        public string zipcode { get; set; }

        public string doy { get; set; }

        public string afm { get; set; }

        public string email { get; set; }

        public string contact { get; set; }

        public string vip { get; set; }

        public string member { get; set; }

        public string tomeas { get; set; }

        public string store { get; set; }

        public string sector { get; set; }

        public string diet { get; set; }

        public string entolh { get; set; }

        public string farsa { get; set; }

        public string remarks { get; set; }

        public float? amount { get; set; }

        public DateTime? expireddate { get; set; }

        public string order_comments { get; set; }

        public DateTime? first_order { get; set; }

        public DateTime? last_order { get; set; }

        public int? no_of_orders { get; set; }

        public decimal? tziros { get; set; }

        public int? bonus { get; set; }

        public int? epitages { get; set; }

        public int? zerobonus { get; set; }

        public int? domino_false { get; set; }

        public int? lates { get; set; }

        public decimal? credit { get; set; }

        public decimal? max_charge { get; set; }

        public string company_name { get; set; }

        public string bl_address { get; set; }

        public string bl_address_no { get; set; }

        public string bl_city { get; set; }

        public int? doycode { get; set; }
    }

    /// <summary>
    /// Uses to checks if LabCustomers is inserted to WebPos
    /// </summary>
    public class LabCustomer
    {
        public string CustomerId { get; set; }
        public int Insert { get; set; }
    }
}
