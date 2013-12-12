using System.Net;
using System.Text;
using System;
using System.IO;

namespace com.admeris.creditcard.api{
    public class VerificationRequest {
        private AvsRequest avsRequest;
        private Cvv2Request cvv2Request;

        public VerificationRequest(AvsRequest avsRequest, Cvv2Request cvv2Request) {
            this.avsRequest = avsRequest;
            this.cvv2Request = cvv2Request;            
        }

        public AvsRequest getAvsRequest() {
            return this.avsRequest;
        }

        public Cvv2Request getCvv2Request() {
            return this.cvv2Request;
        }

        public bool isAvsEnabled() {
            return this.avsRequest != null;
        }    

        public bool isCvv2Enabled() {
            return this.cvv2Request != null;
        }
	}//end class
}//end namespace
