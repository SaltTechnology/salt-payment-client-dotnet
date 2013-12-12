using System.Net;
using System.Text;
using System;
using System.IO;
using System.Collections;

namespace com.admeris.creditcard.api{
    public class CreditCardReceipt : AbstractReceipt {
        private ApprovalInfo approvalInfo;
        private AvsResponse avsResponse;
        private Cvv2Response cvv2Response;
		private PeriodicPurchaseInfo periodicPurchaseInfo = null;

        //---------------------------------------------------------------------
        // Constructors
        //---------------------------------------------------------------------
		public CreditCardReceipt(String errorCode, String errorMessage, String debugMessage) : base(errorCode, errorMessage, debugMessage) {
	    }
	
        public CreditCardReceipt(String response) : base(response) {
            // parse the approval info
            if (this.isApproved()) {
                this.approvalInfo = new ApprovalInfo(
                    this.parseLong("AUTHORIZED_AMOUNT"),
                    (String) this.responseParams["APPROVAL_CODE"], 
                    this.parseInteger("TRACE_NUMBER"),
                    this.parseInteger("REFERENCE_NUMBER"));
            } else {
                this.approvalInfo = null;
            }
            // parse the AVS response
            bool avsResponseAvailable = this.parseBoolean("AVS_RESPONSE_AVAILABLE");
            if (avsResponseAvailable) {
                this.avsResponse = new AvsResponse(
					(String) this.responseParams["AVS_RESPONSE_CODE"],
                    this.parseBoolean("STREET_MATCHED"), 
                    this.parseBoolean("ZIP_MATCHED"), 
                    (String) this.responseParams["ZIP_TYPE"],
                    (String) this.responseParams["AVS_ERROR_CODE"], 
                    (String) this.responseParams["AVS_ERROR_MESSAGE"]);
            } else {
                this.avsResponse = null;
            }
            // parse the CVV2 response
            bool cvv2ResponseAvailable = this
                .parseBoolean("CVV2_RESPONSE_AVAILABLE");
            if (cvv2ResponseAvailable) {
                this.cvv2Response = new Cvv2Response(
                    (String) this.responseParams["CVV2_RESPONSE_CODE"],
                    (String)this.responseParams["CVV2_RESPONSE_MESSAGE"]);
            } else {
                this.cvv2Response = null;
            }

			// parse periodic purchase info
			long periodicPurchaseId = this.parseLong("PERIODIC_TRANSACTION_ID");
			if (periodicPurchaseId >= 0) {
				PeriodicPurchaseInfo.State periodicPurchaseState = 
					PeriodicPurchaseInfo.State.fromCode(this.parseShort("PERIODIC_TRANSACTION_STATE"));
				DateTime nextPaymentDate = this.parseDateTime("PERIODIC_NEXT_PAYMENT_DATE");				
				long lastPaymentId = this.parseLong("PERIODIC_LAST_PAYMENT_ID");
				
				this.periodicPurchaseInfo = new PeriodicPurchaseInfo(periodicPurchaseId, periodicPurchaseState, nextPaymentDate, lastPaymentId);	
			}else {
				this.periodicPurchaseInfo = null;
			}
        }

        //---------------------------------------------------------------------
        // Public Accessors
        //---------------------------------------------------------------------

        public ApprovalInfo getApprovalInfo()
        {
            return this.approvalInfo;
        }

        public AvsResponse getAvsResponse()
        {
            return this.avsResponse;
        }

        public Cvv2Response getCvv2Response()
        {
            return this.cvv2Response;
        }

		public PeriodicPurchaseInfo getPeriodicPurchaseInfo() {
			return this.periodicPurchaseInfo;
		}

    }//end class
}//end namespace
