using com.admeris.creditcard.api;
using System;

using System.Net;
using System.Security.Cryptography.X509Certificates;
public class CCGatewayTest{

	public static void Main (string [] args){
		int merchantId=0;
		string apiToken="";
		long pan = 0;
		short expiry = 0;
		String cvv2 = null;
		string avsPostal = "";
		string avsAddress = "";
		string purchaseInvoice = "";
		string invoice="";
		long amount=0;
		string url="";
		string method="";
		long transactionId=0;
		string customerId="";
		
		DateTime startDate = DateTime.Now;
		DateTime endDate = DateTime.Now;
		DateTime nextPaymentDate = DateTime.Now;

		if(args.Length >=6){
			method=args[0];
			url=args[1];
			merchantId=Convert.ToInt32(args[2]);
			apiToken=args[3];
			if(method.Equals("purchase")){
				pan = Convert.ToInt64(args[4]);
                expiry = Convert.ToInt16(args[5]);
				invoice = args[6];
				amount=Convert.ToInt64(args[7]);
			} 
            else if(method.Equals("refund")) {
				transactionId=Convert.ToInt64(args[4]);
                purchaseInvoice = args[5];
                invoice = args[6];
				amount = Convert.ToInt64(args[7]);
            }
            else if (method.Equals("void"))
            {
                transactionId = Convert.ToInt64(args[4]);
                invoice = args[5];
            }
            else if (method.Equals("verify"))
            {
                pan = Convert.ToInt64(args[4]);
                expiry = Convert.ToInt16(args[5]);
                cvv2 = args[6];
                avsAddress = args[7];
                avsPostal = args[8];
            } 
			else if (method.Equals("recurring"))
            {
				pan = Convert.ToInt64(args[4]);
                expiry = Convert.ToInt16(args[5]);
				invoice = args[6];
				amount=Convert.ToInt64(args[7]);
				startDate = DateTime.ParseExact(args[8], "yyyyMMdd", null);
				endDate = DateTime.ParseExact(args[9], "yyyyMMdd", null);
				customerId = args[10];
            }
			else if (method.Equals("queryRecurring"))
			{
				pan = Convert.ToInt64(args[4]);
				expiry = Convert.ToInt16(args[5]);
				transactionId = Convert.ToInt64(args[6]);
			}
			
			System.Net.ServicePointManager.CertificatePolicy = null;

            // Service
            HttpsCreditCardService ccService = new HttpsCreditCardService(merchantId, apiToken, url);
			CreditCardReceipt resp=null;
            // invoke txn method
			if(method.Equals("purchase")){
                CreditCard creditCard = new CreditCard(pan, expiry);
				resp = ccService.singlePurchase(invoice, creditCard, amount, null);
			} else if (method.Equals("refund")){
                resp = ccService.refund(transactionId, purchaseInvoice, invoice, amount);
            } else if (method.Equals("void")) {
                resp = ccService.voidTransaction(transactionId, invoice);
            } else if (method.Equals("verify")){
                CreditCard creditCard = new CreditCard(pan, expiry, cvv2, avsAddress, avsPostal);
                VerificationRequest vr = new VerificationRequest(AvsRequest.VERIFY_STREET_AND_ZIP, Cvv2Request.CVV2_PRESENT);
                resp = ccService.verifyCreditCard(creditCard, vr);
			} else if (method.Equals("recurring")) {
                CreditCard creditCard = new CreditCard(pan, expiry);
				PeriodicPurchaseInfo.Schedule schedule = new PeriodicPurchaseInfo.Schedule(PeriodicPurchaseInfo.ScheduleType.MONTH, 1);
				resp = ccService.recurringPurchase(invoice, creditCard, amount, startDate, endDate, schedule, null);
			} else if (method.Equals("queryRecurring")){
				CreditCard creditCard = new CreditCard (pan, expiry);
				resp = ccService.queryRecurringPurchase(transactionId);
			} else  {
				Console.WriteLine("args[0] must be purchase, refund or verify");
			}
			if(resp.isApproved()){
                Console.WriteLine("isApproved: {0}", resp.isApproved());
				Console.WriteLine("getTransactionId: {0}",resp.getTransactionId());                
                if (resp.getApprovalInfo() != null && !method.Equals("void"))
                {
                    // void does not have approval, it just cancels a pending txn
                    Console.WriteLine("getApprovalInfo: {0}", resp.getApprovalInfo().ToString());
                }
				if (resp.getCvv2Response() != null)
				{
                    Console.WriteLine("getCvv2Response: {0}", resp.getCvv2Response().ToString());
				}
				if (resp.getAvsResponse() != null)
				{
                    Console.WriteLine("getAvsResponse: {0}", resp.getAvsResponse().ToString());
				}
				if (resp.getPeriodicPurchaseInfo() !=null)
				{
					Console.WriteLine("getPeriodicPurchaseInfo: \n {0}", resp.getPeriodicPurchaseInfo().ToString());
				}
			} else {
				//display error
                Console.WriteLine("isApproved: {0}", resp.isApproved());
				Console.WriteLine("Error Code: {0} Message: {1}",resp.getErrorCode(),resp.getErrorMessage());
				Console.WriteLine("Debug Mesg: Message: {0}",resp.getDebugMessage());
			}
		} else {
			Console.WriteLine("[Invalid Command]");
            Console.WriteLine("");
            Console.WriteLine("Usage:");
            Console.WriteLine("");
			Console.WriteLine("Purchase: purchase url(string) merchantId(int) apiToken(string) pan(long) expiry(short) invoice(string) amount(long)");
            Console.WriteLine("      CCGatewayTest purchase https://test.admeris.com/ccgateway/cc/processor.do 6 abcdefgh 4242424242424242 1210 order-123 1200");
			Console.WriteLine("------------------------------------*---------------------------------------");
			Console.WriteLine("Refund: refund url(string) merchantId(int) apiToken(string) purchaseTxnId(long) purchaseInvoice(string), invoice(string) amount(long)");
            Console.WriteLine("      CCGatewayTest refund https://test.admeris.com/ccgateway/cc/processor.do 6 abcdefgh 123456789 order-123 refund-456 1200");
            Console.WriteLine("------------------------------------*---------------------------------------");
            Console.WriteLine("Void: void url(string) merchantId(int) apiToken(string) transactionId(long) originalInvoice(string)");
            Console.WriteLine("      CCGatewayTest void https://test.admeris.com/ccgateway/cc/processor.do 6 abcdefgh 123456789 order-123");
            Console.WriteLine("------------------------------------*---------------------------------------");
            Console.WriteLine("Verify: verify url(string) merchantId(int) apiToken(string) pan(long) expiry(short) cvv2(string) avsAddress(string) avsPostal(string)");
            Console.WriteLine("      CCGatewayTest verify https://test.admeris.com/ccgateway/cc/processor.do 6 abcdefgh 4242424242424242 1210 456 \"100 Easy Street\" A1B2C3");
            Console.WriteLine("------------------------------------*---------------------------------------");
            Console.WriteLine("Recurring: recurring url(string) merchantId(int) apiToken(string) pan(long) expiry(short) invoice(string) amount(long) start(yyyyMMdd) end(yyyyMMdd) customerId(string)");
            Console.WriteLine("      CCGatewayTest recurring https://test.admeris.com/ccgateway/cc/processor.do 1 abc 4242424242424242 1212 rec-123 1000 20090505 20100505 customer");
			Console.WriteLine("------------------------------------*---------------------------------------");
			Console.WriteLine("Query Recurring: queryRecurring url(string) merchantId(int) apiToken(string) pan(long) expiry(short) recurringPurchaseId(long)");
			Console.WriteLine("      CCGatewayTest queryRecurring https://test.admeris.com/ccgateway/cc/processor.do 1 abc 4242424242424242 1212 670");
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
