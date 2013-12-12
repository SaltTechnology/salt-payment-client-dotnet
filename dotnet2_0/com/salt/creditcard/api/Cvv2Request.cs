using System.Net;
using System.Text;
using System;
using System.IO;

namespace com.admeris.creditcard.api{
    public class Cvv2Request {
		private int code;
		private Cvv2Request(int code) {
			this.code = code;
		}
        public static Cvv2Request CVV2_NOT_SUBMITTED = new Cvv2Request(0);
		public static Cvv2Request CVV2_PRESENT = new Cvv2Request(1);
		public static Cvv2Request CVV2_PRESENT_BUT_ILLEGIBLE = new Cvv2Request(2);
   		public static Cvv2Request CARD_HAS_NO_CVV2 = new Cvv2Request(9);

		public override String ToString() {
			return this.code.ToString();
		}
	}
}//end namespace
