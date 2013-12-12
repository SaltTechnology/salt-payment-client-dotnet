using System.Net;
using System.Text;
using System;
using System.IO;

namespace com.admeris.creditcard.api{
    public class AvsRequest {
		private int code;
		private AvsRequest(int code) {
			this.code = code;
		}
		/** Full AVS by verifying both street and zip. */
		public static AvsRequest VERIFY_STREET_AND_ZIP = new AvsRequest(0);

		/** Verify the zip only. */
		public static AvsRequest VERIFY_ZIP_ONLY = new AvsRequest(1);

		public override String ToString() {
			return this.code.ToString();
		}
	}
}//end namespace
