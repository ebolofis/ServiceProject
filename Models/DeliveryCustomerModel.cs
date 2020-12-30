using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosToWebPosBridge.Models
{
    /// <summary>
    /// Main data for Customer
    /// </summary>
    public class DeliveryCustomerDS
    {
        /// <summary>
        /// Id Auto Increment
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// Customer Last Name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Customer First Name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Customer Vat No (AFM)
        /// </summary>
        public string VatNo { get; set; }

        /// <summary>
        /// Customer DOY
        /// </summary>
        public string DOY { get; set; }

        /// <summary>
        /// Customer Floor
        /// </summary>
        public string Floor { get; set; }

        /// <summary>
        /// Customer email
        /// </summary>
        public string email { get; set; }

        /// <summary>
        /// Customer Comments
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Id from hitPos
        /// </summary>
        public string ExtCustId { get; set; }

        /// <summary>
        /// Billing Customer Name
        /// </summary>
        public string BillingName { get; set; }

        /// <summary>
        /// Billing Customer Vat No (AFM)
        /// </summary>
        public string BillingVatNo { get; set; }

        /// <summary>
        /// Billing Customer DOY
        /// </summary>
        public string BillingDOY { get; set; }

        /// <summary>
        /// Billing Customer Job
        /// </summary>
        public string BillingJob { get; set; }

        /// <summary>
        /// Customer Type (First Found)
        /// </summary>
        public Nullable<int> CustomerType { get; set; } //Take id of entity and manage table  and assign to model CustomerType.Id

        /// <summary>
        /// Customer Price List
        /// </summary>
        public Nullable<long> DefaultPricelistId { get; set; }

        /// <summary>
        /// Customer Is Deleted
        /// </summary>
        public Nullable<bool> IsDeleted { get; set; }

        /// <summary>
        /// Guest Id for receipt model
        /// </summary>
        public Nullable<Int64> GuestId { get; set; }

    }

    /// <summary>
    /// Main Data From SQL Query
    /// </summary>
    public class DeliveryCustomerExtendedModel : DeliveryCustomerModel
    {
        public string tel1 { get; set; }

        public string tel2 { get; set; }

        public string fax { get; set; }

        public string mobile { get; set; }

        public string address1 { get; set; }

        public string address2 { get; set; }

        public string orofos2 { get; set; }

        public string address_no { get; set; }

        public string city { get; set; }

        public string zipcode { get; set; }

        public float? amount { get; set; }

        public string bl_address { get; set; }

        public string bl_address_no { get; set; }

        public string bl_city { get; set; }

        public string doycode { get; set; }
    }

    /// <summary>
    /// Detail Data as Shipping and billing addresses
    /// </summary>
    public class DeliveryCustomerModel : DeliveryCustomerDS
    {
        /// <summary>
        /// List of Customer's Phones
        /// </summary>
        public List<DeliveryCustomersPhonesModel> Phones { get; set; }

        /// <summary>
        /// List of Shiping Addresses
        /// </summary>
        public List<DeliveryCustomersShippingAddressModel> ShippingAddresses { get; set; }

        /// <summary>
        /// List of Billing Addresses
        /// </summary>
        public List<DeliveryCustomersBillingAddressModel> BillingAddresses { get; set; }

        /// <summary>
        /// List of associations addressed and phones per customer
        /// </summary>
        public List<DeliveryCustomersPhonesAndAddressModel> Assocs { get; set; }
    }

    /// <summary>
    /// Customer Phones Model
    /// </summary>
    public class DeliveryCustomersPhonesModel
    {
        /// <summary>
        /// Id Auto Increment
        /// </summary>
        public long ID { get; set; }
        
        /// <summary>
        /// Customer Id
        /// </summary>
        public long CustomerID { get; set; }
        
        /// <summary>
        /// Customer's Phone
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Customer's current selected phone
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// If Record is deleted
        /// </summary>
        public bool IsDeleted { get; set; }
        
        /// <summary>
        /// First found from DB
        /// </summary>
        public Nullable<int> PhoneType { get; set; }
    }

    /// <summary>
    /// Shiping Address Model
    /// </summary>
    public class DeliveryCustomersShippingAddressModel
    {
        /// <summary>
        /// Id auto Increment
        /// </summary>
        public long ID { get; set; }
        
        /// <summary>
        /// Custimer Id (0)
        /// </summary>
        public long CustomerID { get; set; }
        
        /// <summary>
        /// Address Street (Unkonwn if empty)
        /// </summary>
        public string AddressStreet { get; set; }

        /// <summary>
        /// Address No (- if emtpy)
        /// </summary>
        public string AddressNo { get; set; }

        /// <summary>
        /// Customer's City
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Customer's Zip Code
        /// </summary>
        public string Zipcode { get; set; }

        /// <summary>
        /// Street Latitude
        /// </summary>
        public string Latitude { get; set; }

        /// <summary>
        /// Street Longtitude
        /// </summary>
        public string Longtitude { get; set; }

        /// <summary>
        /// Customer's Floor
        /// </summary>
        public string Floor { get; set; }

        /// <summary>
        /// Current street is selected
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// If Record is deleted
        /// </summary>
        public Nullable<bool> IsDeleted { get; set; }

        /// <summary>
        /// First found in DB
        /// </summary>
        public Nullable<int> Type { get; set; }
    }

    /// <summary>
    /// Billing Address Model
    /// </summary>
    public class DeliveryCustomersBillingAddressModel
    {
        /// <summary>
        /// Id Auto Increment
        /// </summary>
        public long ID { get; set; }
        
        /// <summary>
        /// Customer Id
        /// </summary>
        public long CustomerID { get; set; }

        /// <summary>
        /// Address Street (Unkonwn if empty)
        /// </summary>
        public string AddressStreet { get; set; }

        /// <summary>
        /// Address No (- if emtpy)
        /// </summary>
        public string AddressNo { get; set; }

        /// <summary>
        /// Customer's City
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Customer's Zip Code
        /// </summary>
        public string Zipcode { get; set; }

        /// <summary>
        /// Street Longtitude
        /// </summary>
        public string Latitude { get; set; }

        /// <summary>
        /// Street Longtitude
        /// </summary>
        public string Longtitude { get; set; }

        /// <summary>
        /// Customer's Floor
        /// </summary>
        public string Floor { get; set; }

        /// <summary>
        /// Current street is selected
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// If Record is deleted
        /// </summary>
        public Nullable<bool> IsDeleted { get; set; }

        /// <summary>
        /// First found in DB
        /// </summary>
        public Nullable<int> Type { get; set; }
    }

    /// <summary>
    /// Accosiation for shipping and billing and phones
    /// Null for me
    /// </summary>
    public class DeliveryCustomersPhonesAndAddressModel
    {
        /// <summary>
        /// Id Auto Increment
        /// </summary>
        public Nullable<long> ID { get; set; }

        /// <summary>
        /// Customer Id
        /// </summary>
        public Nullable<long> CustomerID { get; set; }

        /// <summary>
        /// Phone Id
        /// </summary>
        public Nullable<long> PhoneID { get; set; }
        
        /// <summary>
        /// Address Id
        /// </summary>
        public Nullable<long> AddressID { get; set; }
        
        /// <summary>
        /// If Address is shipping or billing
        /// </summary>
        public Nullable<short> IsShipping { get; set; }
    }
}
