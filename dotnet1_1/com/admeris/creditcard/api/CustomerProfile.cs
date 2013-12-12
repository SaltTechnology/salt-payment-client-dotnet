using System.Net;
using System.Text;
using System;
using System.IO;

namespace com.admeris.creditcard.api{
    public class CustomerProfile {
    private String legalName;
    private String tradeName;
    private String website;
    private String firstName;
    private String lastName;
    private String phoneNumber;
    private String faxNumber;
    private String address1;
    private String address2;
    private String city;
    private String province;
    private String postal;
    private String country;

    /**
     * Create a new blank CustomerProfile.
     */
    public CustomerProfile() {
    }

    public String getLegalName() {
        return this.legalName;
    }
    public String getTradeName() {
        return this.tradeName;
    }
    public String getWebsite() {
        return this.website;
    }
    public String getFirstName() {
        return this.firstName;
    }
    public String getLastName() {
        return this.lastName;
    }
    public String getPhoneNumber() {
        return this.phoneNumber;
}
    public String getFaxNumber() {
        return this.faxNumber;
    }
    public String getAddress1() {
        return this.address1;
    }
    public String getAddress2() {
        return this.address2;
    }
    public String getCity() {
        return this.city;
    }
    public String getProvince() {
        return this.province;
    }
    public String getPostal() {
        return this.postal;
    }
    public String getCountry() {
        return this.country;
    }

    public void setLegalName(String legalName) {
        this.legalName = legalName;
    }
    public void setTradeName(String tradeName) {
        this.tradeName = tradeName;
    }
    public void setWebsite(String website) {
        this.website = website;
    }
    public void setFirstName(String firstName) {
        this.firstName = firstName;
    }
    public void setLastName(String lastName) {
        this.lastName = lastName;
    }
    public void setPhoneNumber(String phoneNumber) {
        this.phoneNumber = phoneNumber;
    }
    public void setFaxNumber(String faxNumber) {
        this.faxNumber = faxNumber;
    }
    public void setAddress1(String address1) {
		 this.address1 = address1;
    }
    public void setAddress2(String address2) {
        this.address2 = address2;
    }
    public void setCity(String city) {
        this.city = city;
    }
    public void setProvince(String province) {
        this.province = province;
    }
    public void setPostal(String postal) {
        this.postal = postal;
    }
    public void setCountry(String country) {
        this.country = country;
    }

    public bool isBlank() {
        return(
            !((firstName != null && firstName.Length > 0)
            || (lastName != null && lastName.Length > 0)
            || (legalName != null && legalName.Length > 0)
            || (tradeName != null && tradeName.Length > 0)
            || (address1 != null && address1.Length > 0)
            || (address2 != null && address2.Length > 0)
            || (city != null && city.Length > 0)
            || (province != null && province.Length > 0)
            || (postal != null && postal.Length > 0)
            || (country != null && country.Length > 0)
            || (website != null && website.Length > 0)
            || (phoneNumber != null && phoneNumber.Length > 0)
            || (faxNumber != null && faxNumber.Length > 0)
            )
        );
    }

	public override String ToString() {
        StringBuilder req = new StringBuilder();
        req.Append("profileLegalName=").Append(this.getLegalName()).Append(Environment.NewLine);
        req.Append("profileLegalName=").Append(this.getTradeName()).Append(Environment.NewLine);
        req.Append("profileTradeName=").Append(this.getWebsite()).Append(Environment.NewLine);
        req.Append("profileFirstName=").Append(this.getFirstName()).Append(Environment.NewLine);
        req.Append("profileLastName=").Append(this.getLastName()).Append(Environment.NewLine);
        req.Append("profilePhoneNumber=").Append(this.getPhoneNumber()).Append(Environment.NewLine);
        req.Append("profileFaxNumber=").Append(this.getFaxNumber()).Append(Environment.NewLine);
        req.Append("profileAddress1=").Append(this.getAddress1()).Append(Environment.NewLine);
        req.Append("profileAddress2=").Append(this.getAddress2()).Append(Environment.NewLine);
        req.Append("profileCity=").Append(this.getCity()).Append(Environment.NewLine);
        req.Append("profileProvince=").Append(this.getProvince()).Append(Environment.NewLine);
        req.Append("profilePostal=").Append(this.getPostal()).Append(Environment.NewLine);
        req.Append("profileCountry=").Append(this.getCountry()).Append(Environment.NewLine);
        return req.ToString();
    }
    }//end class
}//end namespace
