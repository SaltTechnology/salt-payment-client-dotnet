using System.Net;
using System.Text;
using System;
using System.IO;

namespace com.admeris.creditcard.api{
    public class PaymentProfile {
		private CreditCard creditCard;
		private CustomerProfile customerProfile;

		public PaymentProfile(CreditCard creditCard, CustomerProfile customerProfile) {
			this.creditCard = creditCard;
			this.customerProfile = customerProfile;
		}

		public CreditCard getCreditCard() {
			return this.creditCard;
		}
		public CustomerProfile getCustomerProfile() {
			return this.customerProfile;
		}

		public void setCreditCard(CreditCard newCreditCard) {
			this.creditCard = newCreditCard;
		}
		public void setCustomerProfile(CustomerProfile newCustomerProfile) {
			this.customerProfile = newCustomerProfile;
		}

		public override String ToString() {
			StringBuilder str = new StringBuilder();
			str.Append("[CreditCard]").Append(Environment.NewLine);
			str.Append(this.creditCard).Append(Environment.NewLine);
			str.Append("[CustomerProfile]").Append(Environment.NewLine);
			str.Append(this.customerProfile).Append(Environment.NewLine);
			return str.ToString();
		}
    }//end class
}//end namespace
