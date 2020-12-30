using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosToWebPosBridge.Models
{
    public class GuestModel
    {
        /// <summary>
        /// Id Auto Incremet
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Arrival
        /// </summary>
        public Nullable<System.DateTime> arrivalDT { get; set; }

        /// <summary>
        /// Departure
        /// </summary>
        public Nullable<System.DateTime> departureDT { get; set; }

        /// <summary>
        /// Birthday
        /// </summary>
        public Nullable<System.DateTime> birthdayDT { get; set; }

        /// <summary>
        /// Room No
        /// </summary>
        public string Room { get; set; }

        /// <summary>
        /// Room Id
        /// </summary>
        public Nullable<int> RoomId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Arrival { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Departure { get; set; }

        /// <summary>
        /// Reservation Id
        /// </summary>
        public string ReservationCode { get; set; }

        /// <summary>
        /// Profile no (Assocs with DeliveryCustomer Model)
        /// </summary>
        public Nullable<int> ProfileNo { get; set; }

        /// <summary>
        /// First Name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last Name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Member
        /// </summary>
        public string Member { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// City
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Zip Code
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Country
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Birthday { get; set; }

        /// <summary>
        /// email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Phone
        /// </summary>
        public string Telephone { get; set; }

        /// <summary>
        /// Vip Code
        /// </summary>
        public string VIP { get; set; }

        /// <summary>
        /// Benefits
        /// </summary>
        public string Benefits { get; set; }

        /// <summary>
        /// Nationality
        /// </summary>
        public string NationalityCode { get; set; }

        /// <summary>
        /// Voucher No
        /// </summary>
        public string ConfirmationCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Nullable<int> Type { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Adults no
        /// </summary>
        public Nullable<int> Adults { get; set; }

        /// <summary>
        /// children no
        /// </summary>
        public Nullable<int> Children { get; set; }

        /// <summary>
        /// Board Code
        /// </summary>
        public string BoardCode { get; set; }

        /// <summary>
        /// Board Name
        /// </summary>
        public string BoardName { get; set; }

        /// <summary>
        /// Remarks
        /// </summary>
        public string Note1 { get; set; }

        /// <summary>
        /// Remarks
        /// </summary>
        public string Note2 { get; set; }

        /// <summary>
        /// Reservation Id
        /// </summary>
        public Nullable<int> ReservationId { get; set; }

        /// <summary>
        /// If customer shares room
        /// </summary>
        public Nullable<bool> IsSharer { get; set; }

        /// <summary>
        /// Hotel Id
        /// </summary>
        public Nullable<long> HotelId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Nullable<int> ClassId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Loyalty Points
        /// </summary>
        public Nullable<int> AvailablePoints { get; set; }

        /// <summary>
        /// Discount for loyalty
        /// </summary>
        public Nullable<int> fnbdiscount { get; set; }

        /// <summary>
        /// Change code to monay for loyalty
        /// </summary>
        public Nullable<int> ratebuy { get; set; }
    }
}
