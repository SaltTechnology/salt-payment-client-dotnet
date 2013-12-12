using System;

namespace com.admeris.creditcard.api{

	/// <summary>
	/// A threadsafe, immutable value object representing a Merchant.
	/// </summary>
    public class Merchant {
        private int merchantId;
		private String apiToken;
		private String storeId;
		
		public Merchant(int merchantId, String apiToken) : this(merchantId, apiToken, null) {
		}	
		
		public Merchant(int merchantId, String apiToken, String storeId) {
			if(apiToken == null){
				throw new ArgumentNullException("apiToken must not be null");
			}

			this.merchantId = merchantId;
			this.apiToken = apiToken;
			this.storeId = storeId;
		}
		
		public int getMerchantId(){
			return this.merchantId;
		}

		public String getApiToken(){
			return this.apiToken;
		}

		public String getStoreId(){
			return this.storeId;
		}
	}//end class
}//end namespace
