C:\windows\Microsoft.NET\Framework\v4.0.30319\csc /t:library /out:admeriscc2.dll com\salt\creditcard\api\AbstractReceipt.cs com\salt\creditcard\api\ApprovalInfo.cs  com\salt\creditcard\api\AvsRequest.cs com\salt\creditcard\api\AvsResponse.cs com\salt\creditcard\api\CreditCard.cs com\salt\creditcard\api\CreditCardReceipt.cs com\salt\creditcard\api\CustomerProfile.cs com\salt\creditcard\api\Cvv2Request.cs com\salt\creditcard\api\Merchant.cs com\salt\creditcard\api\Cvv2Response.cs com\salt\creditcard\api\MarketSegment.cs com\salt\creditcard\api\PaymentProfile.cs com\salt\creditcard\api\PeriodicPurchaseInfo.cs com\salt\creditcard\api\VerificationRequest.cs  com\salt\creditcard\api\StorageReceipt.cs com\salt\creditcard\api\ICreditCardService.cs com\salt\creditcard\api\HttpsCreditCardService.cs com\salt\creditcard\api\PurchaseCardRequest.cs com\salt\creditcard\api\CreditCardIndicator.cs

mt.exe -manifest admeriscc2.dll.manifest -outputresource:admeriscc2.dll;2

ildasm.exe admeriscc2.dll /out:admeriscc2.il

C:\windows\Microsoft.NET\Framework\v4.0.30319\ilasm.exe admeriscc2.il /dll /output=admeriscc2StrongNamed.dll /key=AdmerisSNKey.snk

