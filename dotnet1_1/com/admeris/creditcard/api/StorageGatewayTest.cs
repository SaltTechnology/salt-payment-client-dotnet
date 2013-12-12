using com.admeris.creditcard.api;
using System;

using System.Net;
using System.Security.Cryptography.X509Certificates;
public class StorageGatewayTest{

	public static void Main (string [] args){
		int merchantId=0;
		string apiToken="";
		string url="";

		long pan = 0;
        short expiry = 0;

		String firstName = null;
		String lastName = null;

        string storageTokenId = "";
		string orderId = "";
		long amount = 0;

		string method="";

		if(args.Length >=5){
			method=args[0];
			url=args[1];
			merchantId = Convert.ToInt32(args[2]);
			apiToken=args[3];
			storageTokenId = args[4];
			if(method.Equals("add")){
				pan = Convert.ToInt64(args[5]);
                expiry = Convert.ToInt16(args[6]);
                firstName = args[7];
                lastName = args[8];
			} 
            else if(method.Equals("delete")) {
            }
            else if (method.Equals("query")) {
            }
            else if (method.Equals("update"))
            {
                pan = Convert.ToInt64(args[5]);
                expiry = Convert.ToInt16(args[6]);
                firstName = args[7];
                lastName = args[8];
            } else if (method.Equals("purchase")) {
				orderId = args[5];
				amount = Convert.ToInt64(args[6]);
			}
			System.Net.ServicePointManager.CertificatePolicy = new MyPolicy();

            // Service
            HttpsCreditCardService ccService = new HttpsCreditCardService(merchantId, apiToken, url);
			AbstractReceipt resp=null;
            // invoke txn method
			if(method.Equals("add")){
                CreditCard creditCard = new CreditCard(pan, expiry);
				CustomerProfile customerProfile = new CustomerProfile();
				customerProfile.setFirstName(firstName);
				customerProfile.setLastName(lastName);
				PaymentProfile paymentProfile = new PaymentProfile(creditCard, customerProfile);
				Console.WriteLine("made profile");
				resp = ccService.addToStorage(storageTokenId, paymentProfile);
				Console.WriteLine("got resp");
			} else if (method.Equals("delete")){
                resp = ccService.deleteFromStorage(storageTokenId);
            } else if (method.Equals("query")) {
                resp = ccService.queryStorage(storageTokenId);
            } else if (method.Equals("update")){
                CreditCard creditCard = new CreditCard(pan, expiry);
				CustomerProfile customerProfile = new CustomerProfile();
				customerProfile.setFirstName(firstName);
				customerProfile.setLastName(lastName);
				PaymentProfile paymentProfile = new PaymentProfile(creditCard, customerProfile);
				resp = ccService.updateStorage(storageTokenId, paymentProfile);
			} else if (method.Equals("purchase")){
				resp = ccService.singlePurchase(orderId, storageTokenId, amount, null);
			} else  {
				Console.WriteLine("args[0] must be add, delete, query, update, or purchase");
			}
			if(resp.isApproved()){
				Console.WriteLine("Response: {0}", resp.ToString());                
			} else {
				//display error
                Console.WriteLine("isApproved: {0}", resp.isApproved());
				Console.WriteLine("Error Code: {0} Message: {1}",resp.getErrorCode(),resp.getErrorMessage());
			}
		} else {
			Console.WriteLine("[Invalid Command]");
            Console.WriteLine("");
            Console.WriteLine("Usage:");
            Console.WriteLine("");
			Console.WriteLine("Add: add url(string) merchantId(int) apiToken(string) storageTokenId(string) pan(long) expiry(short) firstName(string) lastName(string)");
            Console.WriteLine("      StorageGatewayTest add https://test.admeris.com/ccgateway/cc/processor.do 6 abcdefgh myStorageToken 4242424242424242 1210 John Smith");
			Console.WriteLine("------------------------------------*---------------------------------------");
			Console.WriteLine("Delete: delete url(string) merchantId(int) apiToken(string) storageTokenId(string)");
            Console.WriteLine("      StorageGatewayTest delete https://test.admeris.com/ccgateway/cc/processor.do 6 abcdefgh myStorageToken");
            Console.WriteLine("------------------------------------*---------------------------------------");
            Console.WriteLine("Query: query url(string) merchantId(int) apiToken(string) storageTokenId(string)");
            Console.WriteLine("      StorageGatewayTest query https://test.admeris.com/ccgateway/cc/processor.do 6 abcdefgh myStorageToken");
            Console.WriteLine("------------------------------------*---------------------------------------");
            Console.WriteLine("Update: update url(string) merchantId(int) apiToken(string) storageTokenId(string) expiry(short) firstName(string) lastName(string)");
            Console.WriteLine("      StorageGatewayTest update https://test.admeris.com/ccgateway/cc/processor.do 6 abcdefgh myStorageToken 4111111111111111 1111 Jane Doe");
            Console.WriteLine("------------------------------------*---------------------------------------");
			Console.WriteLine("Purchase: purchase url(string) merchantId(int) apiToken(string) storageTokenId(string) order_id(string) amount_in_cents(long) ");
            Console.WriteLine("      StorageGatewayTest purchase https://test.admeris.com/ccgateway/cc/processor.do 6 abcdefgh myStorageToken order-001 100");
		}
		
	}
}

public class MyPolicy : ICertificatePolicy {
    public bool CheckValidationResult(
          ServicePoint srvPoint
        , X509Certificate certificate
        , WebRequest request
        , int certificateProblem) {

        //Return True to force the certificate to be accepted.
        return true;

    } // end CheckValidationResult
} // class MyPolicy
