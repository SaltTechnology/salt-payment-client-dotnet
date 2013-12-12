using System.Net;
using System.Text;
using System;
using System.IO;

namespace com.admeris.creditcard.api{
	interface ICreditCardService{

        CreditCardReceipt installmentPurchase(String orderId, CreditCard creditCard, long perInstallmentAmount, DateTime startDateTime,  int totalNumberInstallments, VerificationRequest verificationRequest);

        CreditCardReceipt recurringPurchase(String orderId, CreditCard creditCard, long perPaymentAmount, DateTime startDateTime, DateTime endDateTime, PeriodicPurchaseInfo.Schedule schedule, VerificationRequest verificationRequest);
        CreditCardReceipt recurringPurchase(String orderId, String storageTokenId, long perPaymentAmount, DateTime startDateTime, DateTime endDateTime, PeriodicPurchaseInfo.Schedule schedule, VerificationRequest verificationRequest);
		CreditCardReceipt recurringPurchase(PeriodicPurchaseInfo periodicPurchaseInfo, CreditCard creditCard, String storageTokenId, VerificationRequest verificationRequest);
		
		CreditCardReceipt holdRecurringPurchase(long recurringPurchaseId);
		 
		CreditCardReceipt resumeRecurringPurchase(long recurringPurchaseId);
		 
		CreditCardReceipt cancelRecurringPurchase(long recurringPurchaseId);
     
		CreditCardReceipt queryRecurringPurchase(long recurringPurchaseId);

		CreditCardReceipt updateRecurringPurchase(long recurringPurchaseId, CreditCard creditCard, long perPaymentAmount, VerificationRequest verificationRequest);
		CreditCardReceipt updateRecurringPurchase(long recurringPurchaseId, String storageTokenId, long perPaymentAmount, VerificationRequest verificationRequest);
		CreditCardReceipt updateRecurringPurchase(PeriodicPurchaseInfo periodicPurchaseInfo, CreditCard creditCard, String storageTokenId,  VerificationRequest verificationRequest);
		
        CreditCardReceipt refund(long purchaseId, String purchaseOrderId, String refundOrderId, long amount);

        CreditCardReceipt singlePurchase(String orderId, CreditCard creditCard, long amount, VerificationRequest verificationRequest);

		/// Storage token version
        CreditCardReceipt singlePurchase(String orderId, String storageTokenId, long amount, VerificationRequest verificationRequest);

        CreditCardReceipt singlePurchase(String orderId, CreditCard creditCard, long amount, VerificationRequest verificationRequest, bool addToStorage, String secureTokenId);

        CreditCardReceipt singlePurchase(String orderId, CreditCard creditCard, long amount, VerificationRequest verificationRequest, PurchaseCardRequest purchaseCard);

        CreditCardReceipt singlePurchase(String orderId, CreditCard creditCard, long amount, VerificationRequest verificationRequest, PurchaseCardRequest purchaseCard, String secureTokenId);

        CreditCardReceipt singlePurchase(String orderId, String storageTokenId, long amount, VerificationRequest verificationRequest, PurchaseCardRequest purchaseCard);

        CreditCardReceipt verifyCreditCard(CreditCard creditCard, VerificationRequest verificationRequest);

        CreditCardReceipt verifyCreditCard(CreditCard creditCard, VerificationRequest verificationRequest, String secureTokenId);

		/// Storage token version
		CreditCardReceipt verifyCreditCard(String storageTokenId, VerificationRequest verificationRequest);
        
		CreditCardReceipt verifyTransaction(long transactionId, String transactionOrderId);

        CreditCardReceipt voidTransaction(long transactionId, String transactionOrderId);

		StorageReceipt addToStorage(String storageTokenId, PaymentProfile paymentProfile);

		StorageReceipt deleteFromStorage(String storageTokenId);

		StorageReceipt queryStorage(String storageTokenId);

		StorageReceipt updateStorage(String storageTokenId, PaymentProfile paymentProfile);

        CreditCardReceipt executeRecurringPurchase(long recurringPurchaseId, String cvv2);

        CreditCardReceipt closeBatch();
	}//end interface
}//end namespace
