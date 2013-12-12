using System.Net;
using System.Text;
using System;
using System.IO;

namespace com.admeris.creditcard.api{
	public class AvsResponse 
    {
		private String avsResponseCode;
        private Boolean streetMatched;
        private Boolean zipMatched;
        private String zipType;
        private String avsErrorCode;
        private String avsErrorMessage;

        public AvsResponse(String avsResponseCode, Boolean streetMatched, Boolean zipMatched,
                String zipType, String avsErrorCode, String avsErrorMessage) {
			this.avsResponseCode = avsResponseCode;
            this.streetMatched = streetMatched;
            this.zipMatched = zipMatched;
            this.zipType = zipType;
            this.avsErrorCode = avsErrorCode;
            this.avsErrorMessage = avsErrorMessage;
        }

		public String getAvsResponseCode() {
			return this.avsResponseCode;
		}

        public String getAvsErrorCode() {
            return this.avsErrorCode;
        }

		public String getAvsErrorMessage() {
			return this.avsErrorMessage;
		}

		public String getZipType() {
			return this.zipType;
		}

		public Boolean isAvsPerformed() {
			return this.avsErrorCode == null && this.avsErrorMessage == null;
		}
		
		public Boolean isStreetFormat(){
			return this.streetMatched;
		}
		
		public Boolean isZipFormatValid(){
			return this.zipMatched;
		}

		public override String ToString() {
			StringBuilder str = new StringBuilder();
			str.Append("[");
			str.Append("avsResponseCode=").Append(this.avsResponseCode).Append(",");
			str.Append("streetMatched=").Append(this.streetMatched).Append(",");
			str.Append("zipMatched=").Append(this.zipMatched).Append(",");
			str.Append("zipType=").Append(this.zipType).Append(",");
			str.Append("avsErrorCode=").Append(this.avsErrorCode).Append(",");
			str.Append("avsErrorMessage=").Append(this.avsErrorMessage).Append("");
			str.Append("]");
			return str.ToString();
		}
	}//end class
}//end namespace
