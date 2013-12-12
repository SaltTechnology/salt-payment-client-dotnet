using System.Net;
using System.Text;
using System;
using System.IO;

namespace com.admeris.creditcard.api{
	public class HttpsCreditCardService : ICreditCardService {
		// error codes
		public const String REQ_MALFORMED_URL="-1";
		public const String REQ_POST_ERROR="-2";
		public const String REQ_RESPONSE_ERROR="-4";
		public const String REQ_CONNECTION_FAILED="-5";
		public const String REQ_INVALID_REQUEST="-6";

		/// Helper class used in receiving the server response
		private class SendResult {
			public String response;
			public String errorCode;
			public String errorMessage;
			public String debugMessage;
			public SendResult(String response, String errorCode, String errorMessage, String debugMessage) {
				this.response = response;
				this.errorCode = errorCode;
				this.errorMessage = errorMessage;
				this.debugMessage = debugMessage;
			}
		}

        //---------------------------------------------------------------------
        // Constants
        //---------------------------------------------------------------------
        /// <summary>
        /// The number of milliseconds before the connection will timeout. 
        /// This is to ensure that there is enough time for the request to 
        /// complete processing before timing out the connection.
        /// </summary>
        public const int CONNECTION_TIMEOUT_MS_BUFFER = 60000;

        /// <summary>
        /// The format to use for sending Dates.
        /// </summary> 
        public const String DATE_FORMAT = "yyyy-MM-dd";
		
		private const String DEFAULT_MARKET_SEGMENT = MarketSegment.INTERNET;

        //---------------------------------------------------------------------
        // Fields
        //---------------------------------------------------------------------
		private Merchant merchant;
		private String url;
		
        //---------------------------------------------------------------------
        // Constructors
        //---------------------------------------------------------------------

		public HttpsCreditCardService(Merchant merchant, String url) 
		{
			if (url == null)
            {
                throw new ArgumentNullException("url is required");
            }
			this.url = url;
			this.merchant = merchant;
		}
		
        public HttpsCreditCardService(int merchantId, String apiToken, String url) : this(merchantId, apiToken, DEFAULT_MARKET_SEGMENT, url)
        {			
        }

        public HttpsCreditCardService(int merchantId, String apiToken, String marketSegment, String url) : this(new Merchant(merchantId, apiToken), url)
        {            
        }

        //---------------------------------------------------------------------
        // Service Methods
        //---------------------------------------------------------------------

        public CreditCardReceipt installmentPurchase(String orderId,
            CreditCard creditCard, long perInstallmentAmount, DateTime startDate,
            int totalNumberInstallments,
            VerificationRequest verificationRequest)
        {
            if (orderId == null) {
                throw new Exception("orderId is required");
            }
            if (creditCard == null) {
                throw new Exception("creditCard is required");
            }
            // create the request string
            StringBuilder req = new StringBuilder();
            this.appendHeader(req, "installmentPurchase");
            this.appendOrderId(req, orderId);
            this.appendCreditCard(req, creditCard);
            this.appendAmount(req, perInstallmentAmount);
            this.appendStartDate(req, startDate);
            this.appendTotalNumberInstallments(req, totalNumberInstallments);
            this.appendVerificationRequest(req, verificationRequest);
            return this.send(req);
        }

		// recurring purchase, CC ver
		public CreditCardReceipt recurringPurchase(String orderId,
            CreditCard creditCard, long perPaymentAmount, DateTime startDate,
            DateTime endDate, PeriodicPurchaseInfo.Schedule schedule, 
			VerificationRequest verificationRequest)
            {
				if (creditCard == null) {
					return new CreditCardReceipt(REQ_INVALID_REQUEST,
						"creditCard is required", null);
				}
				return this.recurringPurchaseHelper(
					new PeriodicPurchaseInfo(0L, null, schedule, perPaymentAmount, orderId, null, startDate, endDate, DateTime.MinValue),
					creditCard, verificationRequest, true);
		}
		// recurring purchase, storage token ID ver
		public CreditCardReceipt recurringPurchase(String orderId,
            String storageTokenId, long perPaymentAmount, DateTime startDate,
            DateTime endDate, PeriodicPurchaseInfo.Schedule schedule, 
			VerificationRequest verificationRequest)
            {
				if (storageTokenId == null) {
					return new CreditCardReceipt(REQ_INVALID_REQUEST,
						"storageTokenId is required", null);
				}
				return this.recurringPurchaseHelper(
					new PeriodicPurchaseInfo(0L, null, schedule, perPaymentAmount, orderId, null, startDate, endDate, DateTime.MinValue),
					storageTokenId, verificationRequest, false);
		}
		
		// recurring purchase, with PeriodicPurchaseInfo paramters
		public CreditCardReceipt recurringPurchase(
			PeriodicPurchaseInfo periodicPurchaseInfo, CreditCard creditCard, 
			String storageTokenId, VerificationRequest verificationRequest)
			{
				if (storageTokenId == null && creditCard == null){
					return new CreditCardReceipt(REQ_INVALID_REQUEST,
						"creditCard or storageTokenId is required", null);
				}else if (storageTokenId != null){
					return this.recurringPurchaseHelper(periodicPurchaseInfo, storageTokenId,
						verificationRequest, false);
				}else
					return this.recurringPurchaseHelper(periodicPurchaseInfo, creditCard,
						verificationRequest, true);
		}

        public CreditCardReceipt recurringPurchaseHelper (
			PeriodicPurchaseInfo periodicPurchaseInfo, Object creditCardSpecifier,
			VerificationRequest verificationRequest, bool isActualCreditCard)
        {
            if (periodicPurchaseInfo.getOrderId() == null) {
                return new CreditCardReceipt(REQ_INVALID_REQUEST, "orderId is required", null);
            }
            // create the request string
			StringBuilder req = new StringBuilder();
			try {
				this.appendHeader(req, "recurringPurchase");
				this.appendOperationType(req, "create");
				if (isActualCreditCard) {
					this.appendCreditCard(req, (CreditCard) creditCardSpecifier);
				} else {
					this.appendStorageTokenId(req, creditCardSpecifier.ToString());
				}
				this.appendPeriodicPurchaseInfo(req, periodicPurchaseInfo);
				this.appendVerificationRequest(req, verificationRequest);
			} catch (Exception e) {
				return new CreditCardReceipt(REQ_INVALID_REQUEST, e.ToString(), null);
			}
            return this.send(req);
        }

		public CreditCardReceipt holdRecurringPurchase(long recurringPurchaseId) {
			return this.updateRecurringPurchaseHelper(
				new PeriodicPurchaseInfo(recurringPurchaseId, PeriodicPurchaseInfo.State.ON_HOLD, -1), null, null, false);
		}
		public CreditCardReceipt resumeRecurringPurchase(long recurringPurchaseId) {
			return this.updateRecurringPurchaseHelper(
				new PeriodicPurchaseInfo(recurringPurchaseId, PeriodicPurchaseInfo.State.IN_PROGRESS, -1), null, null, false);
		}
		public CreditCardReceipt cancelRecurringPurchase(long recurringPurchaseId) {
			return this.updateRecurringPurchaseHelper(
				new PeriodicPurchaseInfo(recurringPurchaseId, PeriodicPurchaseInfo.State.CANCELLED, -1), null, null, false);
		}

		public CreditCardReceipt queryRecurringPurchase(long recurringPurchaseId) {
			if (recurringPurchaseId < 0) {
				return new CreditCardReceipt(REQ_INVALID_REQUEST,
					"recurringPurchaseId is required", null);
			}
			// create the request string
			StringBuilder req = new StringBuilder();
			try {
				this.appendHeader(req, "recurringPurchase");
				this.appendOperationType(req, "query");
				this.appendTransactionId(req, recurringPurchaseId);
			} catch (Exception e) {
				return new CreditCardReceipt(REQ_INVALID_REQUEST, e.ToString(), null);
			}
			return this.send(req);
		}

		public CreditCardReceipt updateRecurringPurchase(long recurringPurchaseId,
				CreditCard creditCard, long perPaymentAmount, VerificationRequest verificationRequest) {
			return this.updateRecurringPurchaseHelper(
				new PeriodicPurchaseInfo(recurringPurchaseId, PeriodicPurchaseInfo.State.NULL, perPaymentAmount), creditCard, verificationRequest, true);
		}

		public CreditCardReceipt updateRecurringPurchase(long recurringPurchaseId,
				String storageTokenId, long perPaymentAmount, VerificationRequest verificationRequest) {
			return this.updateRecurringPurchaseHelper(
				new PeriodicPurchaseInfo(recurringPurchaseId, PeriodicPurchaseInfo.State.NULL, perPaymentAmount),storageTokenId, verificationRequest, false);
		}
		
		public CreditCardReceipt updateRecurringPurchase(PeriodicPurchaseInfo periodicPurchaseInfo,
				CreditCard creditCard, String storageTokenId, VerificationRequest verificationRequest){			
			
			if (storageTokenId == null && creditCard == null){
				return new CreditCardReceipt(REQ_INVALID_REQUEST,
					"creditCard or storageTokenId is required", null);
			}else if (storageTokenId != null){
				return this.updateRecurringPurchaseHelper(periodicPurchaseInfo, storageTokenId,
					verificationRequest, false);
			}else
				return this.updateRecurringPurchaseHelper(periodicPurchaseInfo, creditCard,
					verificationRequest, true); 
		}
		// helper method for all update + change state recurring methods
		private CreditCardReceipt updateRecurringPurchaseHelper
				(PeriodicPurchaseInfo periodicPurchaseInfo, Object creditCardSpecifier, VerificationRequest verificationRequest,
				bool isActualCreditCard)
		{
			if (periodicPurchaseInfo.getPeriodicTransactionId() < 0) {
				return new CreditCardReceipt(REQ_INVALID_REQUEST,
					"recurringPurchaseId is required", null);
			}
			// create the request string
			StringBuilder req = new StringBuilder();
			try {
				this.appendHeader(req, "recurringPurchase");
				this.appendOperationType(req, "update");
				this.appendTransactionId(req, periodicPurchaseInfo.getPeriodicTransactionId());
				if (creditCardSpecifier != null) {
					if (isActualCreditCard) {
						this.appendCreditCard(req, (CreditCard) creditCardSpecifier);
					} else {
						this.appendStorageTokenId(req, creditCardSpecifier.ToString());
					}
				}
				if (periodicPurchaseInfo.getPerPaymentAmount() > 0) {
					this.appendAmount(req, periodicPurchaseInfo.getPerPaymentAmount());
				}
				if (verificationRequest != null) {
					this.appendVerificationRequest(req, verificationRequest);
				}
				if (periodicPurchaseInfo.getState() != PeriodicPurchaseInfo.State.NULL) {
					this.appendPeriodicPurchaseState(req, periodicPurchaseInfo.getState());
				}
			} catch (Exception e) {
				return new CreditCardReceipt(REQ_INVALID_REQUEST, e.ToString(), null);
			}
			return this.send(req);
		}

        public CreditCardReceipt refund(long purchaseId, String purchaseOrderId,
            String refundOrderId, long amount)
        {
            if (purchaseOrderId == null) {
                throw new Exception("purchaseOrderId is required");
            }
            // create the request string
            StringBuilder req = new StringBuilder();
            this.appendHeader(req, "refund");
            this.appendTransactionId(req, purchaseId);
            this.appendTransactionOrderId(req, purchaseOrderId);
            this.appendOrderId(req, refundOrderId);
            this.appendAmount(req, amount);
            return this.send(req);
        }

        public CreditCardReceipt singlePurchase(String orderId,
            CreditCard creditCard, long amount,
            VerificationRequest verificationRequest)
        {
            if (orderId == null) {
                throw new ArgumentNullException("orderId is required");
            }
            if (creditCard == null) {
                throw new ArgumentNullException("creditCard is required");
            }
            // create the request string
            StringBuilder req = new StringBuilder();
            this.appendHeader(req, "singlePurchase");
            this.appendOrderId(req, orderId);
            this.appendCreditCard(req, creditCard);
            this.appendAmount(req, amount);
            this.appendVerificationRequest(req, verificationRequest);
            return this.send(req);
        }
		public CreditCardReceipt singlePurchase(String orderId,
            String storageTokenId, long amount,
            VerificationRequest verificationRequest)
        {
            if (orderId == null) {
                throw new ArgumentNullException("orderId is required");
            }
            if (storageTokenId == null) {
                throw new ArgumentNullException("storageTokenId is required");
            }
            // create the request string
            StringBuilder req = new StringBuilder();
            this.appendHeader(req, "singlePurchase");
            this.appendOrderId(req, orderId);
            this.appendStorageTokenId(req, storageTokenId);
            this.appendAmount(req, amount);
            this.appendVerificationRequest(req, verificationRequest);
            return this.send(req);
        }

        public CreditCardReceipt verifyCreditCard(CreditCard creditCard,
            VerificationRequest verificationRequest)
        {
            if (creditCard == null) {
                throw new Exception("creditCard is required");
            }
            if (verificationRequest == null) {
                throw new Exception("verificationRequest is required");
            }
            // create the request string
            StringBuilder req = new StringBuilder();
            this.appendHeader(req, "verifyCreditCard");
            this.appendCreditCard(req, creditCard);
            this.appendVerificationRequest(req, verificationRequest);
            return this.send(req);
        }
		public CreditCardReceipt verifyCreditCard(String storageTokenId,
            VerificationRequest verificationRequest)
        {
            if (storageTokenId == null) {
                throw new Exception("storageTokenId is required");
            }
            if (verificationRequest == null) {
                throw new Exception("verificationRequest is required");
            }
            // create the request string
            StringBuilder req = new StringBuilder();
            this.appendHeader(req, "verifyCreditCard");
            this.appendStorageTokenId(req, storageTokenId);
            this.appendVerificationRequest(req, verificationRequest);
            return this.send(req);
        }
		
		public CreditCardReceipt verifyTransaction(long transactionId,
            String transactionOrderId)
        {

            if (transactionOrderId == null) {
                throw new Exception("transactionOrderId is required");
            }
            // create the request string
            StringBuilder req = new StringBuilder();
            this.appendHeader(req, "verifyTransaction");
            this.appendTransactionId(req, transactionId);
            this.appendTransactionOrderId(req, transactionOrderId);
            return this.send(req);
        }
		
		public CreditCardReceipt verifyTransaction (long transactionId){
			StringBuilder req = new StringBuilder();
			this.appendHeader(req, "verifyTransaction");
			this.appendTransactionId(req, transactionId);
			return this.send(req);
		}
		
		public CreditCardReceipt verifyTransaction(String transactionOrderId){
			if (transactionOrderId == null){
				throw new ArgumentNullException("transactionOrderId is required");
			}
			StringBuilder req = new StringBuilder();
			this.appendHeader(req, "verifyTransaction");
			this.appendTransactionOrderId(req, transactionOrderId);
			return this.send(req);
		}

		

        public CreditCardReceipt voidTransaction(long transactionId,
            String transactionOrderId)
        {
            if (transactionOrderId == null) {
                throw new ArgumentNullException("transactionOrderId is required");
            }
            // create the request string
            StringBuilder req = new StringBuilder();
            this.appendHeader(req, "void");
            this.appendTransactionId(req, transactionId);
            this.appendTransactionOrderId(req, transactionOrderId);
            return this.send(req);
        }

		public StorageReceipt addToStorage(String storageTokenId, PaymentProfile paymentProfile) {
			if (paymentProfile == null) {
				return new StorageReceipt(REQ_INVALID_REQUEST,
					"paymentProfile is required", null);
			}
			// create the request string
			StringBuilder req = new StringBuilder();
			try {
				this.appendHeader(req, "secureStorage");
				this.appendOperationType(req, "create");
				this.appendStorageTokenId(req, storageTokenId);
				this.appendPaymentProfile(req, paymentProfile);
			} catch (Exception e) {
				return new StorageReceipt(REQ_INVALID_REQUEST, e.ToString(), null);
			}
			return this.sendStorageRequest(req);
		}
		public StorageReceipt deleteFromStorage(String storageTokenId) {
			if (storageTokenId == null) {
				return new StorageReceipt(REQ_INVALID_REQUEST,
					"storageTokenId is required", null);
			}
			// create the request string
			StringBuilder req = new StringBuilder();
			try {
				this.appendHeader(req, "secureStorage");
				this.appendOperationType(req, "delete");
				this.appendStorageTokenId(req, storageTokenId);
			} catch (Exception e) {
				return new StorageReceipt(REQ_INVALID_REQUEST, e.ToString(), null);
			}
			return this.sendStorageRequest(req);
		}
		public StorageReceipt queryStorage(String storageTokenId) {
			if (storageTokenId == null) {
				return new StorageReceipt(REQ_INVALID_REQUEST,
					"storageTokenId is required", null);
			}
			// create the request string
			StringBuilder req = new StringBuilder();
			try {
				this.appendHeader(req, "secureStorage");
				this.appendOperationType(req, "query");
				this.appendStorageTokenId(req, storageTokenId);
			} catch (Exception e) {
				return new StorageReceipt(REQ_INVALID_REQUEST, e.ToString(), null);
			}
			return this.sendStorageRequest(req);
		}
		public StorageReceipt updateStorage(String storageTokenId, PaymentProfile paymentProfile) {
			if (storageTokenId == null) {
				return new StorageReceipt(REQ_INVALID_REQUEST,
					"storageTokenId is required", null);
			}
			if (paymentProfile == null) {
				return new StorageReceipt(REQ_INVALID_REQUEST,
					"paymentProfile is required", null);
			}
			// create the request string
			StringBuilder req = new StringBuilder();
			try {
				this.appendHeader(req, "secureStorage");
				this.appendOperationType(req, "update");
				this.appendStorageTokenId(req, storageTokenId);
				this.appendPaymentProfile(req, paymentProfile);
			} catch (Exception e) {
				return new StorageReceipt(REQ_INVALID_REQUEST, e.ToString(), null);
			}
			return this.sendStorageRequest(req);
		}
        //---------------------------------------------------------------------
        // Public Accessor Methods
        //---------------------------------------------------------------------

        public String getApiToken()
        {
			return this.merchant.getApiToken();
        }
        public String getMarketSegment()
        {
            return DEFAULT_MARKET_SEGMENT;
        }
        public int getMerchantId()
        {
            return this.merchant.getMerchantId();
        }
        public String getUrl()
        {
            return this.url;
        }

        //---------------------------------------------------------------------
        // Private Methods
        //---------------------------------------------------------------------

        private void appendAmount(StringBuilder req, long amount)
        {
            this.appendParam(req, "amount", amount);
        }

        private void appendCreditCard(StringBuilder req, CreditCard creditCard)
        {
            if (creditCard != null)
            {
                this.appendParam(req, "creditCardNumber", creditCard
                        .getCreditCardNumber());
                this.appendParam(req, "expiryDate", creditCard.getExpiryDate());
                this.appendParam(req, "magneticData", creditCard.getMagneticData());
                if (creditCard.getCvv2() != null)
                {
                    this.appendParam(req, "cvv2", creditCard.getCvv2());
                }
                this.appendParam(req, "street", creditCard.getStreet());
                this.appendParam(req, "zip", creditCard.getZip());
                this.appendParam(req, "secureCode", creditCard.getSecureCode());
            }
        }
		
        private void appendHeader(StringBuilder req, String requestCode)
        {
            if (requestCode == null)
            {
                throw new Exception("requestCode is required");
            }
            this.appendParam(req, "requestCode", requestCode);
            this.appendParam(req, "merchantId", this.merchant.getMerchantId());
            this.appendParam(req, "apiToken", this.merchant.getApiToken());
            this.appendParam(req, "marketSegmentCode", DEFAULT_MARKET_SEGMENT);
			if(this.merchant.getStoreId() != null){
				this.appendParam(req, "storeId", this.merchant.getStoreId());
			}
        }

		protected void appendOperationType(StringBuilder req, String type) {
			if (type == null) {
				throw new Exception("type is required");
			}
			this.appendParam(req, "operationCode", type);
		}

        private void appendOrderId(StringBuilder req, String orderId)
        {
            this.appendParam(req, "orderId", orderId);
        }

		private void appendPaymentProfile(StringBuilder req, PaymentProfile paymentProfile) {
			if (paymentProfile == null) {
				return;
			} else {
				if (paymentProfile.getCreditCard() != null) {
					this.appendCreditCard(req, paymentProfile.getCreditCard());
				}
				if (paymentProfile.getCustomerProfile() != null) {
					this.appendParam(req, "profileLegalName", paymentProfile.getCustomerProfile().getLegalName());
					this.appendParam(req, "profileTradeName", paymentProfile.getCustomerProfile().getTradeName());
					this.appendParam(req, "profileWebsite", paymentProfile.getCustomerProfile().getWebsite());
					this.appendParam(req, "profileFirstName", paymentProfile.getCustomerProfile().getFirstName());
					this.appendParam(req, "profileLastName", paymentProfile.getCustomerProfile().getLastName());
					this.appendParam(req, "profilePhoneNumber", paymentProfile.getCustomerProfile().getPhoneNumber());
					this.appendParam(req, "profileFaxNumber", paymentProfile.getCustomerProfile().getFaxNumber());
					this.appendParam(req, "profileAddress1", paymentProfile.getCustomerProfile().getAddress1());
					this.appendParam(req, "profileAddress2", paymentProfile.getCustomerProfile().getAddress2());
					this.appendParam(req, "profileCity", paymentProfile.getCustomerProfile().getCity());
					this.appendParam(req, "profileProvince", paymentProfile.getCustomerProfile().getProvince());
					this.appendParam(req, "profilePostal", paymentProfile.getCustomerProfile().getPostal());
					this.appendParam(req, "profileCountry", paymentProfile.getCustomerProfile().getCountry());
				}
			}
		}

		private void appendPeriodicPurchaseState(StringBuilder req, PeriodicPurchaseInfo.State state) {
			if (state != PeriodicPurchaseInfo.State.NULL) {
				this.appendParam(req, "periodicPurchaseStateCode", (int)state.toCode());
			}
		}

		private void appendPeriodicPurchaseSchedule(StringBuilder req, PeriodicPurchaseInfo.Schedule schedule) {
			if (schedule == null) {
				throw new Exception("a non-null schedule is required");
			}
			this.appendParam(req, "periodicPurchaseScheduleTypeCode", (int) schedule.getScheduleType());
			this.appendParam(req, "periodicPurchaseIntervalLength", schedule.getIntervalLength());
		}

        private void appendStartDate(StringBuilder req, DateTime startDate)
        {
            this.appendParam(req, "startDate", startDate.ToString(DATE_FORMAT));
        }
		
        private void appendEndDate(StringBuilder req, DateTime endDate)
        {
            this.appendParam(req, "endDate", endDate.ToString(DATE_FORMAT));
        }
		
		private void appendNextPaymentDate (StringBuilder req, DateTime nextPaymentDate)
		{
			this.appendParam (req, "nextPaymentDate", nextPaymentDate.ToString(DATE_FORMAT));
		}
		
		
		private void appendPeriodicPurchaseInfo(StringBuilder req, PeriodicPurchaseInfo periodicPurchaseInfo) {
			if (periodicPurchaseInfo.getPerPaymentAmount() >= 0) {
				this.appendAmount(req,periodicPurchaseInfo.getPerPaymentAmount());
			}
			if (periodicPurchaseInfo.getState() != null) {
				this.appendPeriodicPurchaseState(req, periodicPurchaseInfo.getState());
			}
			if (periodicPurchaseInfo.getSchedule() != null) {
				this.appendPeriodicPurchaseSchedule(req, periodicPurchaseInfo.getSchedule());
			}
			if (periodicPurchaseInfo.getOrderId() != null) {
				this.appendOrderId(req, periodicPurchaseInfo.getOrderId());
			}
			if (periodicPurchaseInfo.getCustomerId() != null) {
				this.appendParam(req, "customerId", periodicPurchaseInfo.getCustomerId());
			}
			if (periodicPurchaseInfo.getStartDate() != DateTime.MinValue) {
				this.appendStartDate(req, periodicPurchaseInfo.getStartDate());
			}
			if (periodicPurchaseInfo.getEndDate() != DateTime.MinValue) {
				this.appendEndDate(req, periodicPurchaseInfo.getEndDate());
			}
			if (periodicPurchaseInfo.getNextPaymentDate() != DateTime.MinValue) {
				this.appendNextPaymentDate(req, periodicPurchaseInfo.getNextPaymentDate());
			}
		
		}
		

		private void appendStorageTokenId(StringBuilder req, String storageTokenId) {
			if (storageTokenId != null) {
				this.appendParam(req, "storageTokenId", storageTokenId);
			}
		}

        private void appendTotalNumberInstallments(StringBuilder req, int totalNumberInstallments)
        {
            this.appendParam(req, "totalNumberInstallments",
                    totalNumberInstallments);
        }

        private void appendTransactionId(StringBuilder req, long transactionId)
        {
            this.appendParam(req, "transactionId", transactionId);
        }

        private void appendTransactionOrderId(StringBuilder req, String transactionOrderId)
        {
            this.appendParam(req, "transactionOrderId", transactionOrderId);
        }

        private void appendVerificationRequest(StringBuilder req, VerificationRequest vr)
        {
            if (vr != null)
            {
                this.appendParam(req, "avsRequestCode", vr.isAvsEnabled() ? 
                    vr.getAvsRequest().ToString() : null);
                this.appendParam(req, "cvv2RequestCode", vr.isCvv2Enabled() ? 
                    vr.getCvv2Request().ToString() : null);
            }
        }

        private void appendParam(StringBuilder str, String name, Object value)
        {
            if (str == null)
            {
                throw new Exception("str is required");
            }
            if (name == null)
            {
                throw new Exception("name is required");
            }
            if (value != null)
            {
                if (str.Length != 0)
                {
                    str.Append("&");
                }
                str.Append(name).Append("=").Append(value);                
            }
        }

		protected CreditCardReceipt send(StringBuilder request) {
			CreditCardReceipt receipt = null;
			SendResult result = this.doSend(request);
			if (result != null) {
				if (result.errorCode != null) {
					receipt = new CreditCardReceipt(result.errorCode, result.errorMessage, result.debugMessage);
				} else {
					receipt = new CreditCardReceipt(result.response);
				}
			}
			return receipt;
		}

		protected StorageReceipt sendStorageRequest(StringBuilder request) {
			StorageReceipt receipt = null;
			SendResult result = this.doSend(request);
			if (result != null) {
				if (result.errorCode != null) {
					receipt = new StorageReceipt(result.errorCode, result.errorMessage, result.debugMessage);
				} else {
					receipt = new StorageReceipt(result.response);
				}
			}
			return receipt;
		}
        // Sends the txn request to the server
        private SendResult doSend(StringBuilder request)
        {
            HttpWebRequest connection = null;
            //try establishing connection with gateway url
            try
            {
                connection = (HttpWebRequest)WebRequest.Create(this.url);
                connection.Timeout = CONNECTION_TIMEOUT_MS_BUFFER;
                connection.Method = "POST";
                UTF8Encoding encoding = new UTF8Encoding();
                byte[] byteData = encoding.GetBytes(request.ToString());
                connection.ContentLength = byteData.Length;
                connection.ContentType = "application/x-www-form-urlencoded";
                //writing to stream
                try
                {
                    Stream requestStream = connection.GetRequestStream();
                    requestStream.Write(byteData, 0, byteData.Length);
                    requestStream.Close();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error Sending Request to Gateway: " + ex.ToString());
                }
                //get response code
                try
                {
                    HttpWebResponse response = (HttpWebResponse)connection.GetResponse();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Stream respStream = response.GetResponseStream();
                        StreamReader readStream = new StreamReader(respStream, Encoding.UTF8);
                        //read
                        String respStr = readStream.ReadToEnd();
                        readStream.Close();
                        response.Close();
                        return new SendResult(respStr, null, null, null);
                    }
                    else
                    {
                        String errorMsg = "HTTP Error [" + response.StatusCode + "]: response isn't 200 - OK";                        
                        response.Close();
                        throw new Exception(errorMsg);
                    }
                }
                catch (Exception ex)
                {
                    String errorMsg = "Could not Receive response from Gateway: " + ex.ToString();
                    throw new Exception(errorMsg);
                }
            }
            catch (UriFormatException ufe)
            {
                String errorMsg = "the specified url is invalid/malformed: " + ufe.ToString();
                throw new Exception(errorMsg);
            }
            catch (Exception ex)
            {
               String errorMsg = ex.ToString();
               throw new Exception(errorMsg);                
            }
        }//end send method
	}//end class
}//end namespace
