using System;

namespace com.admeris.creditcard.api
{
    public class PurchaseCardRequest
    {
        private String customerCode;
        private long salesTax;
        private String invoice;
        private String tranCode;
        private String skuNumber;

        public PurchaseCardRequest(String customerCode, long salesTax)
        {
            this.customerCode = customerCode;
            this.salesTax = salesTax;
        }

        /**
         * @return the invoice
         */
        public String getInvoice()
        {
            return invoice;
        }

        /**
         * @param invoice the invoice to set
         */
        public void setInvoice(String invoice)
        {
            this.invoice = invoice;
        }

        /**
         * @return the tranCode
         */
        public String getTranCode()
        {
            return tranCode;
        }

        /**
         * @param tranCode the tranCode to set
         */
        public void setTranCode(String tranCode)
        {
            this.tranCode = tranCode;
        }

        /**
         * @return the skuNumber
         */
        public String getSkuNumber()
        {
            return skuNumber;
        }

        /**
         * @param skuNumber
         *            the skuNumber to set
         */
        public void setSkuNumber(String skuNumber)
        {
            this.skuNumber = skuNumber;
        }

        /**
         * @return the customerCode
         */
        public String getCustomerCode()
        {
            return customerCode;
        }

        /**
         * @return the salesTax
         */
        public long getSalesTax()
        {
            return salesTax;
        }
    }
}
