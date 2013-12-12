using System.Net;
using System.Text;
using System;
using System.IO;
using System.Collections;

namespace com.admeris.creditcard.api{
    public abstract class AbstractReceipt {

        protected Hashtable responseParams;
        protected bool approved = false;
        protected long transactionId;
        protected String orderId;
        protected DateTime processedDateTime;
        protected String errorCode;
        protected String errorMessage;
        protected String debugMessage;
		protected String response;

		protected AbstractReceipt() {}

        //---------------------------------------------------------------------
        // Constructors
        //---------------------------------------------------------------------

		///
		/// <summary>Caller-side error constructor.</summary>
		///
		/// <param name="errorCode">error Code</param>
		/// <param name="errorMessage">error Message</param>
		/// <param name="debugMessage">debug Message, if applicable</param>
		///
		public AbstractReceipt(String errorCode, String errorMessage, String debugMessage) {
			// just set the error and date info, the rest is null
			this.approved = false;
			this.errorCode = errorCode;
			this.errorMessage = errorMessage;
			this.debugMessage = debugMessage;
			this.processedDateTime = DateTime.Now;

			// use this data to create an equivalent response
			StringBuilder str = new StringBuilder();
			str.Append("APPROVED= ").Append(this.approved).Append(", ");
			str.Append("ERROR_CODE= ").Append(this.errorCode != null ? this.errorCode : "").Append(", ");
			str.Append("ERROR_MESSAGE= ").Append(this.errorMessage != null ? this.errorMessage : "").Append(", ");
			str.Append("DEBUG_MESSAGE= ").Append(this.debugMessage != null ? this.debugMessage : "").Append(", ");
			str.Append("PROCESSED_DATE= ").Append(this.processedDateTime.ToShortDateString()).Append(", ");
			str.Append("PROCESSED_TIME= ").Append(this.processedDateTime.ToShortTimeString());
			this.response = str.ToString();
		}

		///
		/// <summary>Creates an instance by parsing the response from the gateway.</summary>
		///
		/// <param name="response">the response from the gateway to parse. Not null.</param>
		///
        public AbstractReceipt(String response) {
            if (response == null) {
                throw new Exception("response is required");
            }
			this.response = response;
            responseParams = new Hashtable();

            // split \n delimited params into the param hashtable
            String[] param = response.Trim().Split('\n');
            for (int i = 0; i < param.Length; i++)
            {
                String[] key = param[i].Split('=');
                if (key.Length == 2)
                {
                    responseParams.Add(key[0], key[1]);
                }
            }//end compute hashtable

            // parse the parameters
            this.approved = this.parseBoolean("APPROVED");
            this.transactionId = this.parseLong("TRANSACTION_ID");
            this.orderId = (String) this.responseParams["ORDER_ID"];
            Object processedDate = this.responseParams["PROCESSED_DATE"];
            Object processedTime = this.responseParams["PROCESSED_TIME"];
            if (processedDate != null && processedTime != null) {
                try {
                    this.processedDateTime = DateTime.ParseExact((processedDate.ToString() + processedTime.ToString()), "yyMMddHHmmss", null);
                } catch (Exception e) {
                    throw e;
                }
            } else {
                this.processedDateTime = new DateTime();
            }
            this.errorCode = (String) this.responseParams["ERROR_CODE"];
            this.errorMessage = (String) this.responseParams["ERROR_MESSAGE"];
            this.debugMessage = (String) this.responseParams["DEBUG_MESSAGE"];
        }

        //---------------------------------------------------------------------
        // Public Accessors
        //---------------------------------------------------------------------

        public String getDebugMessage()
        {
            return this.debugMessage;
        }

        public String getErrorCode()
        {
            return this.errorCode;
        }

        public String getErrorMessage()
        {
            return this.errorMessage;
        }

        public String getOrderId()
        {
            return this.orderId;
        }
        public Hashtable getParams() {
            return this.responseParams;
        }

        public DateTime getProcessedDateTime()
        {
            return this.processedDateTime;
        }

        public long getTransactionId()
        {
            return this.transactionId;
        }

        public bool isApproved()
        {
            return this.approved;
        }

        protected bool parseBoolean(String paramName) {
            Object value = this.responseParams[paramName];            
            return value != null ? Boolean.Parse(value.ToString()) : false;
        }
        protected int parseInteger(String paramName) {
            Object value = this.responseParams[paramName];
            return value != null ? int.Parse(value.ToString()) : -1;
        }
        protected long parseLong(String paramName) {
            Object value = this.responseParams[paramName];
            return value != null ? long.Parse(value.ToString()) : -1;
        }

		protected short parseShort(String paramName) {
	        Object value = this.responseParams[paramName];
			return value != null ?  short.Parse(value.ToString()) : (short) -1;
	    }
		
		protected DateTime parseDateTime(String paramName){
			Object value = this.responseParams[paramName];
			try{
				return value!=null ? DateTime.ParseExact(value.ToString(), HttpsCreditCardService.DATE_FORMAT, null): DateTime.MinValue;
			}catch (Exception e){
				throw e;
			}
		}

        public override String ToString() {
            return this.response;
        }

    }//end class
}//end namespace
