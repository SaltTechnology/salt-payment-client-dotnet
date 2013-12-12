using System.Net;
using System.Text;
using System;
using System.IO;

namespace com.admeris.creditcard.api {
    public class Cvv2Response 
    {
        private String code;
        private String message;

        public Cvv2Response(String code, String message) {
            this.code = code;
            this.message = message;
        }

        public String getCode() {
            return this.code;
        }

        public String getMessage() {
            return this.message;
        }

        public override String ToString() {
            StringBuilder str = new StringBuilder();
            str.Append("[");
            str.Append("code=").Append(this.code).Append(",");
            str.Append("message=").Append(this.message).Append("");
            str.Append("]");
            return str.ToString();
        }

	}//end class
}//end namespace
