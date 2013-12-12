using System.Net;
using System.Text;
using System;
using System.IO;

namespace com.admeris.creditcard.api {
    public class ApprovalInfo 
    {
        private String approvalCode;
        private int referenceNumber = -1;
        private int traceNumber = -1;
        private long authorizedAmount = -1;

        public ApprovalInfo(long authorizedAmount, String approvalCode,
                int traceNumber, int referenceNumber) {
            this.authorizedAmount = authorizedAmount;
            this.approvalCode = approvalCode;
            this.referenceNumber = referenceNumber;
            this.traceNumber = traceNumber;
        }

        public String getApprovalCode() {
            return this.approvalCode;
        }

        public long getAuthorizedAmount() {
            return this.authorizedAmount;
        }

        public int getReferenceNumber() {
            return this.referenceNumber;
        }

        public int getTraceNumber() {
            return this.traceNumber;
        }

        public override String ToString() {
            StringBuilder str = new StringBuilder();
            str.Append("[");
            str.Append("approvalCode= ").Append(this.approvalCode).Append(", ");
            str.Append("referenceNumber= ").Append(this.referenceNumber).Append(", ");
            str.Append("traceNumber= ").Append(this.traceNumber).Append(", ");
            str.Append("authorizedAmount= ").Append(this.authorizedAmount);
            str.Append("]");
            return str.ToString();
        }
	}//end class
}//end namespace
