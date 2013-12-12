using System.Net;
using System.Text;
using System;
using System.IO;

namespace com.admeris.creditcard.api{
    public class CreditCard {
        private long creditCardNumber;
        private short expiryDate;
        private String magneticData;
        private String cvv2;
        private String street;
        private String zip;
        private String secureCode;

        public CreditCard(long creditCardNumber, short expiryDate) 
            : this(creditCardNumber, expiryDate, null, null, null, null) 
        {
        }

        public CreditCard(long creditCardNumber, short expiryDate, String cvv2, String street, String zip)
            : this(creditCardNumber, expiryDate, cvv2, street, zip, null)
        {
        }

        private CreditCard(long creditCardNumber, short expiryDate, String cvv2,
                String street, String zip, String secureCode) {
            this.creditCardNumber = creditCardNumber;
            this.expiryDate = expiryDate;
            this.cvv2 = cvv2;
            this.street = street;
            this.zip = zip;
            this.magneticData = null;
            this.secureCode = secureCode;
        }

        public CreditCard(String magneticData) 
            : this(magneticData, null, null, null, null)
        {
        }

        public CreditCard(String magneticData, String cvv2, String street, String zip) 
            : this(magneticData, cvv2, street, zip, null)
        {
        }

        private CreditCard(String magneticData, String cvv2, String street,
                String zip, String secureCode) {
            if (magneticData == null) {
                throw new Exception("magneticData must not be null");
            }
            this.magneticData = magneticData;
            this.cvv2 = cvv2;
            this.street = street;
            this.zip = zip;
            this.creditCardNumber = -1;
            this.expiryDate = -1;
            this.secureCode = secureCode;
        }

        public long getCreditCardNumber() {
            return this.creditCardNumber;
        }

        public String getCvv2() {
            return this.cvv2;
        }

        public short getExpiryDate() {
            return this.expiryDate;
        }

        public String getMagneticData() {
            return this.magneticData;
        }

        public String getSecureCode() {
            return this.secureCode;
        }

        public String getStreet() {
            return this.street;
        }

        public String getZip() {
            return this.zip;
        }

        public bool isSwiped() {
            return this.magneticData != null;
        }

		public override String ToString() {
			StringBuilder req = new StringBuilder();
			req.Append("creditCardNumber=").Append(this.getCreditCardNumber()).Append(Environment.NewLine);
			req.Append("expiryDate=").Append(this.getExpiryDate()).Append(Environment.NewLine);
			req.Append("street=").Append(this.getStreet()).Append(Environment.NewLine);
			req.Append("zip=").Append(this.getZip()).Append(Environment.NewLine);
			return req.ToString();
		}
    }//end class
}//end namespace
