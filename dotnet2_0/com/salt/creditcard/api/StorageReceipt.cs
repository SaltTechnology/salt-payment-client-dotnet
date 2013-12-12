using System.Net;
using System.Text;
using System;
using System.IO;
using System.Collections;

namespace com.admeris.creditcard.api{
    public class StorageReceipt : AbstractReceipt {
		private PaymentProfile paymentProfile= null;
		private String storageTokenId = null;

        //---------------------------------------------------------------------
        // Constructors
        //---------------------------------------------------------------------
		public StorageReceipt(String errorCode, String errorMessage, String debugMessage) : base(errorCode, errorMessage, debugMessage) {
	    }
	
        public StorageReceipt(String response) : base(response) {
			// storage token ID
            this.storageTokenId = (String) this.responseParams["STORAGE_TOKEN_ID"];
			// make sure profile available
            bool paymentProfileAvailable = this
                .parseBoolean("PAYMENT_PROFILE_AVAILABLE");
            if (paymentProfileAvailable) {
				// parse the CreditCard
				CreditCard creditCard = null;
				bool creditCardAvailable = this.parseBoolean("CREDIT_CARD_AVAILABLE");
				if (creditCardAvailable) {
					String sanitized = (String) this.responseParams["CREDIT_CARD_NUMBER"];
					sanitized = sanitized.Replace("*", "");
					creditCard = new CreditCard(long.Parse(sanitized), this.parseShort("EXPIRY_DATE"));
				}
				// parse the Customer Profile
				CustomerProfile profile = null;
				bool customerProfileAvailable = this.parseBoolean("CUSTOMER_PROFILE_AVAILABLE");
				if (customerProfileAvailable) {
					profile = new CustomerProfile();
					profile.setLegalName((String)this.responseParams["CUSTOMER_PROFILE_LEGAL_NAME"]);
					profile.setTradeName((String)this.responseParams["CUSTOMER_PROFILE_TRADE_NAME"]);
					profile.setWebsite((String)this.responseParams["CUSTOMER_PROFILE_WEBSITE"]);
					profile.setFirstName((String)this.responseParams["CUSTOMER_PROFILE_FIRST_NAME"]);
					profile.setLastName((String)this.responseParams["CUSTOMER_PROFILE_LAST_NAME"]);
					profile.setPhoneNumber((String)this.responseParams["CUSTOMER_PROFILE_PHONE_NUMBER"]);
					profile.setFaxNumber((String)this.responseParams["CUSTOMER_PROFILE_FAX_NUMBER"]);
					profile.setAddress1((String)this.responseParams["CUSTOMER_PROFILE_ADDRESS1"]);
					profile.setAddress2((String)this.responseParams["CUSTOMER_PROFILE_ADDRESS2"]);
					profile.setCity((String)this.responseParams["CUSTOMER_PROFILE_CITY"]);
					profile.setProvince((String)this.responseParams["CUSTOMER_PROFILE_PROVINCE"]);
					profile.setPostal((String)this.responseParams["CUSTOMER_PROFILE_POSTAL"]);
					profile.setCountry((String)this.responseParams["CUSTOMER_PROFILE_COUNTRY"]);
				}
				this.paymentProfile = new PaymentProfile(creditCard, profile);
            } else {
                this.paymentProfile = null;
            }            
        }

        //---------------------------------------------------------------------
        // Public Accessors
        //---------------------------------------------------------------------

        public String getStorageTokenId()
        {
            return this.storageTokenId;
        }

        public PaymentProfile getPaymentProfile()
        {
            return this.paymentProfile;
        }
    }//end class
}//end namespace
