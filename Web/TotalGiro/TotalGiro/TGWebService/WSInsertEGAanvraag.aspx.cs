using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using B4F.TotalGiro.Dal;

using B4F.TotalGiro.CRM;
using B4F.TotalGiro.StaticData;
using B4F.Web.TGWebService;

public partial class Test_TestWebService : System.Web.UI.Page
{
    protected ArrayList validationmessages = new ArrayList();

    private void Page_Load(object sender, EventArgs e)
    {
       this.Response.ContentType = "text/xml";

       WSResponse response = new WSResponse();

       response.MetaData = new WSMetaData();

       XmlSerializer s = null;
       try
       {
           // Read XML posted via HTTP
           s = new XmlSerializer(typeof(WSRequest));
           XmlReader reader = validateRequest(Request.InputStream);
           WSRequest req = (WSRequest)s.Deserialize(reader);
           reader.Close();

           if (validationmessages.Count > 0)
           {
               response.MetaData.AddMessage(new WSMessage(WSMessageSeverity.ERR, 1101, "There were validation errors."));
               foreach (string msg in validationmessages)
               {
                   response.MetaData.AddMessage(new WSMessage(WSMessageSeverity.MESSAGE, 104, msg));
               }
           }
           else
           {
               response.MetaData.AddMessage(new WSMessage(WSMessageSeverity.MESSAGE, 101, "Web request parsed successfully."));

               executeRequest(req);
           }
       }
       catch (Exception ex)
       {
           response.MetaData.AddMessage(new WSMessage(WSMessageSeverity.ERR, 1001, ex.Message + ", complete Exception: " + ex.ToString()));
       }

       s = new XmlSerializer(typeof(WSResponse));
       StringWriter sw = new StringWriter();
       s.Serialize(sw,response);

       string sresponse = sw.ToString();
       Response.Write(sresponse);


    }

    private XmlReader validateRequest(Stream inputStream)
    {

        //load instance into memory
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.Schemas.Add(null, Server.MapPath(".") + "\\wsrequest.xsd");

        //add resolver to pass security if accessing web site to recieve schemaXmlUrlResolver 
        XmlUrlResolver resolver = new XmlUrlResolver();
        resolver.Credentials = System.Net.CredentialCache.DefaultCredentials;
        settings.XmlResolver = resolver; 

        settings.ValidationType = ValidationType.Schema; //this line is neccessary for validation to 
        settings.ValidationEventHandler += new ValidationEventHandler(xvr_ValidationEventHandler);
        return XmlReader.Create(inputStream, settings);
    }

    private void xvr_ValidationEventHandler(object sender, ValidationEventArgs args)
    {
        string strTemp;
        strTemp = "Validation error: " + args.Message;

        this.validationmessages.Add(strTemp);

    }


    private void WriteToFile(string line)
    {
        TextWriter tw = new StreamWriter("c:\\temp\\wsrequest.log", true);

        tw.WriteLine(line);

        tw.Close();

    }

    void executeRequest(WSRequest request)
    {

        // Check credentials
        int LoginId = CheckLogin(request.Username, request.Password);
        // Write the data in Excel parsable form to a text file
        WriteToFile(request.UpdateRequestData.WriteToString());

        if (LoginId > 0)
            SaveToDB(LoginId, request.UpdateRequestData);
        else
            throw new ApplicationException("Unknown username or incorrect password.");


    }

    /// <summary>
    /// Checks the credentials and returns the login id for this account
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="password">Password</param>
    /// <returns>LoginId, 0 if login failed</returns>
    int CheckLogin(string username, string password)
    {
        IDalSession session = null;  
        SqlDataReader rdr = null;

        int loginid = 0;

        try {
            session = NHSessionFactory.CreateSession();

			SqlCommand cmd  = new SqlCommand(
                "select loginid from tblLogin where username = @username and password = dbo.ud_MakeSHA1(@password) and enabled = 1", (SqlConnection) session.Connection);

            cmd.Parameters.Add(new SqlParameter("@username", username));
            cmd.Parameters.Add(new SqlParameter("@password", password));

			// execute the command
			rdr = cmd.ExecuteReader();

			// iterate through results, printing each to console
			while (rdr.Read())
			{
                if (!rdr.IsDBNull(0))
				    loginid = rdr.GetInt32(0);
			}

            session.Close();
		}
		finally
		{
			if (rdr != null)
			{
				rdr.Close();
			}
            if (session != null)
                session.Close();
		}

        return loginid;

    }

    /// <summary>
    /// Calls a SP to insert an new request in TotalGiro
    /// </summary>
    void SaveToDB(int LoginId, WSAccountRequestData requestdata)
    {
        IDalSession session = NHSessionFactory.CreateSession();

        ArrayList parameters = new ArrayList();

        parameters.Add(new NHSession.ParamInfo("LoginId", LoginId, NHibernate.NHibernateUtil.Int32)); // int,
        parameters.Add(new NHSession.ParamInfo("Titel", requestdata.Applicant.Title, NHibernate.NHibernateUtil.String));// varchar(10) = '', 
        parameters.Add(new NHSession.ParamInfo("Naam", requestdata.Applicant.LastName, NHibernate.NHibernateUtil.String));// varchar(30) = '', 
        parameters.Add(new NHSession.ParamInfo("Tussenvoegsels", requestdata.Applicant.MiddleName, NHibernate.NHibernateUtil.String));// varchar(10) = '', 
        parameters.Add(new NHSession.ParamInfo("Voorletters", requestdata.Applicant.Initials, NHibernate.NHibernateUtil.String));// varchar(10) = '', 
        parameters.Add(new NHSession.ParamInfo("Geslacht", requestdata.Applicant.Sex, NHibernate.NHibernateUtil.String));// varchar(1) = '',
        parameters.Add(new NHSession.ParamInfo("Nationaliteit", requestdata.Applicant.Nationality, NHibernate.NHibernateUtil.String));// varchar(30) = '',

        //ModelPortfolio

        if (requestdata.Applicant.ContactDetails != null && requestdata.Applicant.ContactDetails.ResidentialAddress != null)
        {
            parameters.Add(new NHSession.ParamInfo("Straat", requestdata.Applicant.ContactDetails.ResidentialAddress.Street, NHibernate.NHibernateUtil.String));// varchar(30) = '', 
            parameters.Add(new NHSession.ParamInfo("Huisnummer", requestdata.Applicant.ContactDetails.ResidentialAddress.HouseNumber, NHibernate.NHibernateUtil.String));// varchar(10) = '', 
            parameters.Add(new NHSession.ParamInfo("HuisnummerToevoeging", requestdata.Applicant.ContactDetails.ResidentialAddress.HouseNumberSuffix, NHibernate.NHibernateUtil.String));// varchar(7) = '', 
            parameters.Add(new NHSession.ParamInfo("Postcode", requestdata.Applicant.ContactDetails.ResidentialAddress.PostalCode, NHibernate.NHibernateUtil.String));// varchar(7) = '', 
            parameters.Add(new NHSession.ParamInfo("Plaats", requestdata.Applicant.ContactDetails.ResidentialAddress.City, NHibernate.NHibernateUtil.String));// varchar(30) = '',
            parameters.Add(new NHSession.ParamInfo("Land", requestdata.Applicant.ContactDetails.ResidentialAddress.Country, NHibernate.NHibernateUtil.String));// varchar(30) = '',
        }
        else
        {
            parameters.Add(new NHSession.ParamInfo("Straat", "", NHibernate.NHibernateUtil.String));// varchar(30) = '', 
            parameters.Add(new NHSession.ParamInfo("Huisnummer", "", NHibernate.NHibernateUtil.String));// varchar(10) = '', 
            parameters.Add(new NHSession.ParamInfo("HuisnummerToevoeging", "", NHibernate.NHibernateUtil.String));// varchar(7) = '', 
            parameters.Add(new NHSession.ParamInfo("Postcode", "", NHibernate.NHibernateUtil.String));// varchar(7) = '', 
            parameters.Add(new NHSession.ParamInfo("Plaats", "", NHibernate.NHibernateUtil.String));// varchar(30) = '',
            parameters.Add(new NHSession.ParamInfo("Land", "", NHibernate.NHibernateUtil.String));// varchar(30) = '',
        }

        if (requestdata.Applicant.ContactDetails != null && requestdata.Applicant.ContactDetails.PostAddress != null)
        {
            parameters.Add(new NHSession.ParamInfo("PostStraat", requestdata.Applicant.ContactDetails.PostAddress.Street, NHibernate.NHibernateUtil.String));// varchar(30) = '',
            parameters.Add(new NHSession.ParamInfo("PostHuisnummer", requestdata.Applicant.ContactDetails.PostAddress.HouseNumber, NHibernate.NHibernateUtil.String));// varchar(10) = '',
            parameters.Add(new NHSession.ParamInfo("PostHuisnummerToevoeging", requestdata.Applicant.ContactDetails.PostAddress.HouseNumberSuffix, NHibernate.NHibernateUtil.String));// varchar(7) = '',
            parameters.Add(new NHSession.ParamInfo("PostPostcode", requestdata.Applicant.ContactDetails.PostAddress.PostalCode, NHibernate.NHibernateUtil.String));// varchar(7) = '',
            parameters.Add(new NHSession.ParamInfo("PostPlaats", requestdata.Applicant.ContactDetails.PostAddress.City, NHibernate.NHibernateUtil.String));// varchar(30) = '',
            parameters.Add(new NHSession.ParamInfo("PostLand", requestdata.Applicant.ContactDetails.PostAddress.Country, NHibernate.NHibernateUtil.String));// varchar(30) = '',	
        }
        else
        {
            parameters.Add(new NHSession.ParamInfo("PostStraat", "", NHibernate.NHibernateUtil.String));// varchar(30) = '',
            parameters.Add(new NHSession.ParamInfo("PostHuisnummer", "", NHibernate.NHibernateUtil.String));// varchar(10) = '',
            parameters.Add(new NHSession.ParamInfo("PostHuisnummerToevoeging", "", NHibernate.NHibernateUtil.String));// varchar(7) = '',
            parameters.Add(new NHSession.ParamInfo("PostPostcode", "", NHibernate.NHibernateUtil.String));// varchar(7) = '',
            parameters.Add(new NHSession.ParamInfo("PostPlaats", "", NHibernate.NHibernateUtil.String));// varchar(30) = '',
            parameters.Add(new NHSession.ParamInfo("PostLand", "", NHibernate.NHibernateUtil.String));// varchar(30) = '',	
        }

        parameters.Add(new NHSession.ParamInfo("Geboortedatum", requestdata.Applicant.BirthDate, NHibernate.NHibernateUtil.DateTime));// datetime = null, 
        parameters.Add(new NHSession.ParamInfo("SOFI", requestdata.Applicant.SOFINumber, NHibernate.NHibernateUtil.String));// varchar(12) = '', 
        parameters.Add(new NHSession.ParamInfo("Email", requestdata.Applicant.ContactDetails.EMail, NHibernate.NHibernateUtil.String));// varchar(50) = '', 
        parameters.Add(new NHSession.ParamInfo("Telefoon", requestdata.Applicant.ContactDetails.PhoneWork, NHibernate.NHibernateUtil.String));// varchar(12) = '', 
        parameters.Add(new NHSession.ParamInfo("TelefoonAvond", requestdata.Applicant.ContactDetails.PhonePrivate, NHibernate.NHibernateUtil.String));// varchar(12) = '', 
        parameters.Add(new NHSession.ParamInfo("Mobiel", requestdata.Applicant.ContactDetails.PhoneMobile, NHibernate.NHibernateUtil.String));// varchar(12) = '', 
        parameters.Add(new NHSession.ParamInfo("Fax", requestdata.Applicant.ContactDetails.PhoneFax, NHibernate.NHibernateUtil.String));// varchar(12) = '', 
        parameters.Add(new NHSession.ParamInfo("LegitimatieSoort", requestdata.Applicant.Identification.Type, NHibernate.NHibernateUtil.String));// varchar(20) = '', 
        parameters.Add(new NHSession.ParamInfo("LegitimatieNummer", requestdata.Applicant.Identification.Number, NHibernate.NHibernateUtil.String));// varchar(20) = '', 
        parameters.Add(new NHSession.ParamInfo("LegitimatieGeldigTot", requestdata.Applicant.Identification.ValidityPeriod, NHibernate.NHibernateUtil.DateTime));// datetime = null,

        if (requestdata.SecondApplicant != null)
        {
            parameters.Add(new NHSession.ParamInfo("PTitel", requestdata.SecondApplicant.Title, NHibernate.NHibernateUtil.String));// varchar(10) = '', 
            parameters.Add(new NHSession.ParamInfo("PNaam", requestdata.SecondApplicant.LastName, NHibernate.NHibernateUtil.String));// varchar(30) = '', 
            parameters.Add(new NHSession.ParamInfo("PTussenvoegsels", requestdata.SecondApplicant.MiddleName, NHibernate.NHibernateUtil.String));// varchar(10) = '', 
            parameters.Add(new NHSession.ParamInfo("PVoorletters", requestdata.SecondApplicant.Initials, NHibernate.NHibernateUtil.String));// varchar(10) = '', 
            parameters.Add(new NHSession.ParamInfo("PGeslacht", requestdata.SecondApplicant.Sex, NHibernate.NHibernateUtil.String));// varchar(1) = '', 
        }
        else
        {
            parameters.Add(new NHSession.ParamInfo("PTitel", "", NHibernate.NHibernateUtil.String));// varchar(10) = '', 
            parameters.Add(new NHSession.ParamInfo("PNaam", "", NHibernate.NHibernateUtil.String));// varchar(30) = '', 
            parameters.Add(new NHSession.ParamInfo("PTussenvoegsels", "", NHibernate.NHibernateUtil.String));// varchar(10) = '', 
            parameters.Add(new NHSession.ParamInfo("PVoorletters", "", NHibernate.NHibernateUtil.String));// varchar(10) = '', 
            parameters.Add(new NHSession.ParamInfo("PGeslacht", "", NHibernate.NHibernateUtil.String));// varchar(1) = '', 
        }

        if (requestdata.SecondApplicant != null && requestdata.SecondApplicant.ContactDetails != null && requestdata.SecondApplicant.ContactDetails.ResidentialAddress != null)
        {
            parameters.Add(new NHSession.ParamInfo("PStraat", requestdata.SecondApplicant.ContactDetails.ResidentialAddress.Street, NHibernate.NHibernateUtil.String));// varchar(30) = '', 
            parameters.Add(new NHSession.ParamInfo("PHuisnummer", requestdata.SecondApplicant.ContactDetails.ResidentialAddress.HouseNumber, NHibernate.NHibernateUtil.String));// varchar(10) = '', 
            parameters.Add(new NHSession.ParamInfo("PHuisnummerToevoeging", requestdata.SecondApplicant.ContactDetails.ResidentialAddress.HouseNumberSuffix, NHibernate.NHibernateUtil.String));// varchar(7) = '', 
            parameters.Add(new NHSession.ParamInfo("PPostcode", requestdata.SecondApplicant.ContactDetails.ResidentialAddress.PostalCode, NHibernate.NHibernateUtil.String));// varchar(7) = '', 
            parameters.Add(new NHSession.ParamInfo("PPlaats", requestdata.SecondApplicant.ContactDetails.ResidentialAddress.City, NHibernate.NHibernateUtil.String));// varchar(30) = '', 
        }
        else
        {
            parameters.Add(new NHSession.ParamInfo("PStraat", "", NHibernate.NHibernateUtil.String));// varchar(30) = '', 
            parameters.Add(new NHSession.ParamInfo("PHuisnummer", "", NHibernate.NHibernateUtil.String));// varchar(10) = '', 
            parameters.Add(new NHSession.ParamInfo("PHuisnummerToevoeging", "", NHibernate.NHibernateUtil.String));// varchar(7) = '', 
            parameters.Add(new NHSession.ParamInfo("PPostcode", "", NHibernate.NHibernateUtil.String));// varchar(7) = '', 
            parameters.Add(new NHSession.ParamInfo("PPlaats", "", NHibernate.NHibernateUtil.String));// varchar(30) = '', 
        }

        if (requestdata.SecondApplicant != null)
        {
            parameters.Add(new NHSession.ParamInfo("PGeboortedatum", requestdata.SecondApplicant.BirthDate, NHibernate.NHibernateUtil.DateTime));// datetime = null, 
            parameters.Add(new NHSession.ParamInfo("PSOFI", requestdata.SecondApplicant.SOFINumber, NHibernate.NHibernateUtil.String));// varchar(12) = '', 
            if (requestdata.SecondApplicant.ContactDetails != null)
            {
                parameters.Add(new NHSession.ParamInfo("PEmail", requestdata.SecondApplicant.ContactDetails.EMail, NHibernate.NHibernateUtil.String));// varchar(50) = '', 
                parameters.Add(new NHSession.ParamInfo("PTelefoon", requestdata.SecondApplicant.ContactDetails.PhoneWork, NHibernate.NHibernateUtil.String));// varchar(12) = '', 
                parameters.Add(new NHSession.ParamInfo("PTelefoonAvond", requestdata.SecondApplicant.ContactDetails.PhonePrivate, NHibernate.NHibernateUtil.String));// varchar(12) = '', 
                parameters.Add(new NHSession.ParamInfo("PMobiel", requestdata.SecondApplicant.ContactDetails.PhoneMobile, NHibernate.NHibernateUtil.String));// varchar(12) = '', 
                parameters.Add(new NHSession.ParamInfo("PFax", requestdata.SecondApplicant.ContactDetails.PhoneFax, NHibernate.NHibernateUtil.String));// varchar(12) = '', 
            }
            else
            {
                parameters.Add(new NHSession.ParamInfo("PEmail", "", NHibernate.NHibernateUtil.String));// varchar(50) = '', 
                parameters.Add(new NHSession.ParamInfo("PTelefoon", "", NHibernate.NHibernateUtil.String));// varchar(12) = '', 
                parameters.Add(new NHSession.ParamInfo("PTelefoonAvond", "", NHibernate.NHibernateUtil.String));// varchar(12) = '', 
                parameters.Add(new NHSession.ParamInfo("PMobiel", "", NHibernate.NHibernateUtil.String));// varchar(12) = '', 
                parameters.Add(new NHSession.ParamInfo("PFax", "", NHibernate.NHibernateUtil.String));// varchar(12) = '', 
            }
        }
        else
        {
            parameters.Add(new NHSession.ParamInfo("PGeboortedatum", null, NHibernate.NHibernateUtil.DateTime));// datetime = null, 
            parameters.Add(new NHSession.ParamInfo("PSOFI", "", NHibernate.NHibernateUtil.String));// varchar(12) = '', 
            parameters.Add(new NHSession.ParamInfo("PEmail", "", NHibernate.NHibernateUtil.String));// varchar(50) = '', 
            parameters.Add(new NHSession.ParamInfo("PTelefoon", "", NHibernate.NHibernateUtil.String));// varchar(12) = '', 
            parameters.Add(new NHSession.ParamInfo("PTelefoonAvond", "", NHibernate.NHibernateUtil.String));// varchar(12) = '', 
            parameters.Add(new NHSession.ParamInfo("PMobiel", "", NHibernate.NHibernateUtil.String));// varchar(12) = '', 
            parameters.Add(new NHSession.ParamInfo("PFax", "", NHibernate.NHibernateUtil.String));// varchar(12) = '', 
        }

        if (requestdata.SecondApplicant != null && requestdata.SecondApplicant.Identification != null)
        {
            parameters.Add(new NHSession.ParamInfo("PLegitimatieSoort", requestdata.SecondApplicant.Identification.Type, NHibernate.NHibernateUtil.String));// varchar(20) = '', 
            parameters.Add(new NHSession.ParamInfo("PLegitimatieNummer", requestdata.SecondApplicant.Identification.Number, NHibernate.NHibernateUtil.String));// varchar(20) = '',
            parameters.Add(new NHSession.ParamInfo("PLegitimatieGeldigTot", requestdata.SecondApplicant.Identification.ValidityPeriod, NHibernate.NHibernateUtil.DateTime));// datetime = null,
        }
        else
        {
            parameters.Add(new NHSession.ParamInfo("PLegitimatieSoort", "", NHibernate.NHibernateUtil.String));// varchar(20) = '', 
            parameters.Add(new NHSession.ParamInfo("PLegitimatieNummer", "", NHibernate.NHibernateUtil.String));// varchar(20) = '',
            parameters.Add(new NHSession.ParamInfo("PLegitimatieGeldigTot", null, NHibernate.NHibernateUtil.String));// datetime = null,
        }

        if (requestdata.SecondApplicant != null)
        {
            parameters.Add(new NHSession.ParamInfo("PNationaliteit", requestdata.SecondApplicant.Nationality, NHibernate.NHibernateUtil.String));// varchar(30) = '',
        }
        else
        {
            parameters.Add(new NHSession.ParamInfo("PNationaliteit", "", NHibernate.NHibernateUtil.String));// varchar(30) = '',
        }

        parameters.Add(new NHSession.ParamInfo("TegenRekening", requestdata.MoneyAccount, NHibernate.NHibernateUtil.String));// varchar(12) = '', 
        parameters.Add(new NHSession.ParamInfo("Onttrekking", requestdata.PeriodicWithdrawal == true ? "Y" : "N", NHibernate.NHibernateUtil.String));// varchar(1) = '',
        parameters.Add(new NHSession.ParamInfo("OnttrekkingBedrag", requestdata.PeriodicWithdrawalAmount.ToString(), NHibernate.NHibernateUtil.String));// varchar(12) = '',
        parameters.Add(new NHSession.ParamInfo("TegenRekeningTNV", requestdata.MoneyAccountHolder, NHibernate.NHibernateUtil.String));// varchar(30) = '', 
        parameters.Add(new NHSession.ParamInfo("TegenRekeningPlaats", requestdata.MoneyAccountBankCity, NHibernate.NHibernateUtil.String));// varchar(30) = '',
        parameters.Add(new NHSession.ParamInfo("TegenRekeningBank", requestdata.MoneyAccountBank, NHibernate.NHibernateUtil.String));// varchar(30) = '',
        parameters.Add(new NHSession.ParamInfo("VerpandSoort", "", NHibernate.NHibernateUtil.String));// varchar(50) = '',
        parameters.Add(new NHSession.ParamInfo("Pandhouder", requestdata.Ledger, NHibernate.NHibernateUtil.String));// varchar(30) = '',
        parameters.Add(new NHSession.ParamInfo("EersteInleg", requestdata.FirstDeposit.ToString(), NHibernate.NHibernateUtil.String));//  varchar(30) = '',
        parameters.Add(new NHSession.ParamInfo("PeriodeInleg", "", NHibernate.NHibernateUtil.String));//  varchar(1) = '',
        parameters.Add(new NHSession.ParamInfo("PeriodiekeInleg", "", NHibernate.NHibernateUtil.String));//  varchar(30) = '',
        parameters.Add(new NHSession.ParamInfo("BNaam", "", NHibernate.NHibernateUtil.String));// varchar(30) = '',
        parameters.Add(new NHSession.ParamInfo("BRechtsvorm", "", NHibernate.NHibernateUtil.String));// varchar(50) = '',
        parameters.Add(new NHSession.ParamInfo("BStraat", "", NHibernate.NHibernateUtil.String));// varchar(30) = '', 
        parameters.Add(new NHSession.ParamInfo("BHuisnummer", "", NHibernate.NHibernateUtil.String));// varchar(10) = '', 
        parameters.Add(new NHSession.ParamInfo("BHuisnummerToevoeging", "", NHibernate.NHibernateUtil.String));// varchar(7) = '', 
        parameters.Add(new NHSession.ParamInfo("BPostcode", "", NHibernate.NHibernateUtil.String));// varchar(7) = '', 
        parameters.Add(new NHSession.ParamInfo("BPlaats", "", NHibernate.NHibernateUtil.String));// varchar(30) = '',
        parameters.Add(new NHSession.ParamInfo("BLand", "", NHibernate.NHibernateUtil.String));// varchar(30) = '',
        parameters.Add(new NHSession.ParamInfo("DatumOprichting", null, NHibernate.NHibernateUtil.DateTime));// datetime = null, 
        parameters.Add(new NHSession.ParamInfo("KVK", "", NHibernate.NHibernateUtil.String));// varchar(12) = '', 
        parameters.Add(new NHSession.ParamInfo("BEmail", "", NHibernate.NHibernateUtil.String));// varchar(50) = '', 
        parameters.Add(new NHSession.ParamInfo("BTelefoon", "", NHibernate.NHibernateUtil.String));// varchar(12) = '', 
        parameters.Add(new NHSession.ParamInfo("BTelefoonAvond", "", NHibernate.NHibernateUtil.String));// varchar(12)='', 
        parameters.Add(new NHSession.ParamInfo("BMobiel", "", NHibernate.NHibernateUtil.String));// varchar(12) = '', 
        parameters.Add(new NHSession.ParamInfo("BFax", "", NHibernate.NHibernateUtil.String));// varchar(12) = '',
        parameters.Add(new NHSession.ParamInfo("BRekHouderTitel1", "", NHibernate.NHibernateUtil.String));// varchar(10) = '', 
        parameters.Add(new NHSession.ParamInfo("BRekHouder1", "", NHibernate.NHibernateUtil.String));// varchar(30) = '', 
        parameters.Add(new NHSession.ParamInfo("BRekHouderTussenv1", "", NHibernate.NHibernateUtil.String));// varchar(10) = '', 
        parameters.Add(new NHSession.ParamInfo("BRekHouderVoorl1", "", NHibernate.NHibernateUtil.String));// varchar(20) = '',
        parameters.Add(new NHSession.ParamInfo("BRekHouderGeslacht1", "M", NHibernate.NHibernateUtil.String));// varchar(1) = '', 
        parameters.Add(new NHSession.ParamInfo("BBevoegdheid1", "", NHibernate.NHibernateUtil.String));// varchar(30) = '', 
        parameters.Add(new NHSession.ParamInfo("BRekHouderTitel2", "", NHibernate.NHibernateUtil.String));// varchar(10) = '', 
        parameters.Add(new NHSession.ParamInfo("BRekHouder2", "", NHibernate.NHibernateUtil.String));// varchar(30) = '', 
        parameters.Add(new NHSession.ParamInfo("BRekHouderTussenv2", "", NHibernate.NHibernateUtil.String));// varchar(10) = '', 
        parameters.Add(new NHSession.ParamInfo("BRekHouderVoorl2", "", NHibernate.NHibernateUtil.String));// varchar(20) = '',
        parameters.Add(new NHSession.ParamInfo("BRekHouderGeslacht2", "", NHibernate.NHibernateUtil.String));// varchar(1) = '', 
        parameters.Add(new NHSession.ParamInfo("BBevoegdheid2", "", NHibernate.NHibernateUtil.String));// varchar(30) = '',
        /* Hardcoded to Vierlander */
        parameters.Add(new NHSession.ParamInfo("Bedrijfafkorting", "VL", NHibernate.NHibernateUtil.String));// char(2) = '' 
        parameters.Add(new NHSession.ParamInfo("ModelPortfolio", requestdata.ModelPortfolio, NHibernate.NHibernateUtil.String));// char(50) = ''
        parameters.Add(new NHSession.ParamInfo("IncludeOPALData", requestdata.IncludeOPALData, NHibernate.NHibernateUtil.Int16));// bit = 0

        if (requestdata.Questionnaire != null)
        {
            // Write xml questionnaire to userprofle
            XmlSerializer s = new XmlSerializer(typeof(WSQuestionnaire));
            StringWriter sw = new StringWriter();
            s.Serialize(sw,requestdata.Questionnaire);
            parameters.Add(new NHSession.ParamInfo("UserProfile", sw.ToString(), NHibernate.NHibernateUtil.StringClob));// text
        }
        else
            parameters.Add(new NHSession.ParamInfo("UserProfile", "", NHibernate.NHibernateUtil.StringClob));// text

        if (requestdata.OPALData != null)
        {
            // Write xml questionnaire to userprofle
            XmlSerializer s = new XmlSerializer(typeof(WSOPALData));
            StringWriter sw = new StringWriter();
            s.Serialize(sw, requestdata.OPALData);
            parameters.Add(new NHSession.ParamInfo("OPALData", sw.ToString(), NHibernate.NHibernateUtil.StringClob));// text
        }
        else
            parameters.Add(new NHSession.ParamInfo("OPALData", "", NHibernate.NHibernateUtil.StringClob));// text


        session.ExecuteStoredProcedureParam(@"EXEC dbo.insertEGAanvraag_Finix
	                                                    @LoginId = :LoginId,
	                                                    @Titel = :Titel, 
	                                                    @Naam = :Naam, 
	                                                    @Tussenvoegsels = :Tussenvoegsels, 
	                                                    @Voorletters = :Voorletters, 
	                                                    @Geslacht = :Geslacht,
	                                                    @Nationaliteit = :Nationaliteit,
	                                                    @Straat = :Straat, 
                                                        @Huisnummer = :Huisnummer,
	                                                    @HuisnummerToevoeging = :HuisnummerToevoeging, 
	                                                    @Postcode = :Postcode, 
	                                                    @Plaats = :Plaats,
	                                                    @Land = :Land,
	                                                    @PostStraat = :PostStraat,
	                                                    @PostHuisnummer = :PostHuisnummer,
	                                                    @PostHuisnummerToevoeging = :PostHuisnummerToevoeging,
	                                                    @PostPostcode = :PostPostcode,
	                                                    @PostPlaats = :PostPlaats,
	                                                    @PostLand = :PostLand,	
	                                                    @Geboortedatum = :Geboortedatum, 
	                                                    @SOFI = :SOFI, 
	                                                    @Email = :Email, 
	                                                    @Telefoon = :Telefoon, 
	                                                    @TelefoonAvond =:TelefoonAvond, 
	                                                    @Mobiel = :Mobiel, 
	                                                    @Fax = :Fax, 
	                                                    @LegitimatieSoort = :LegitimatieSoort, 
	                                                    @LegitimatieNummer = :LegitimatieNummer, 
	                                                    @LegitimatieGeldigTot = :LegitimatieGeldigTot,
	                                                    @PTitel = :PTitel, 
	                                                    @PNaam = :PNaam, 
	                                                    @PTussenvoegsels = :PTussenvoegsels, 
	                                                    @PVoorletters = :PVoorletters, 
	                                                    @PGeslacht = :PGeslacht, 
	                                                    @PStraat = :PStraat, 
	                                                    @PHuisnummer = :PHuisnummer, 
	                                                    @PHuisnummerToevoeging = :PHuisnummerToevoeging, 
	                                                    @PPostcode = :PPostcode, 
	                                                    @PPlaats = :PPlaats, 
	                                                    @PGeboortedatum = :PGeboortedatum, 
	                                                    @PSOFI = :PSOFI, 
	                                                    @PEmail = :PEmail, 
	                                                    @PTelefoon = :PTelefoon, 
	                                                    @PTelefoonAvond = :PTelefoonAvond, 
	                                                    @PMobiel = :PMobiel, 
	                                                    @PFax = :PFax, 
	                                                    @PLegitimatieSoort = :PLegitimatieSoort, 
	                                                    @PLegitimatieNummer = :PLegitimatieNummer,
	                                                    @PLegitimatieGeldigTot = :PLegitimatieGeldigTot,
	                                                    @PNationaliteit = :PNationaliteit,
	                                                    @TegenRekening = :TegenRekening, 
	                                                    @Onttrekking = :Onttrekking,
	                                                    @OnttrekkingBedrag = :OnttrekkingBedrag,
	                                                    @TegenRekeningTNV = :TegenRekeningTNV, 
	                                                    @TegenRekeningPlaats = :TegenRekeningPlaats,
	                                                    @TegenRekeningBank = :TegenRekeningBank,
	                                                    @VerpandSoort = :VerpandSoort,
	                                                    @Pandhouder = :Pandhouder,
	                                                    @EersteInleg = :EersteInleg,
	                                                    @PeriodeInleg = :PeriodeInleg,
	                                                    @PeriodiekeInleg = :PeriodiekeInleg,
	                                                    @BNaam = :BNaam,
	                                                    @BRechtsvorm = :BRechtsvorm,
	                                                    @BStraat = :BStraat, 
	                                                    @BHuisnummer = :BHuisnummer, 
	                                                    @BHuisnummerToevoeging = :BHuisnummerToevoeging, 
	                                                    @BPostcode = :BPostcode, 
	                                                    @BPlaats = :BPlaats,
	                                                    @BLand = :BLand,
	                                                    @DatumOprichting = :DatumOprichting, 
	                                                    @KVK = :KVK, 
	                                                    @BEmail = :BEmail, 
	                                                    @BTelefoon = :BTelefoon, 
	                                                    @BTelefoonAvond = :BTelefoonAvond, 
	                                                    @BMobiel = :BMobiel, 
	                                                    @BFax = :BFax,
	                                                    @BRekHouderTitel1 = :BRekHouderTitel1, 
	                                                    @BRekHouder1 = :BRekHouder1, 
	                                                    @BRekHouderTussenv1 = :BRekHouderTussenv1, 
	                                                    @BRekHouderVoorl1 = :BRekHouderVoorl1,
	                                                    @BRekHouderGeslacht1 = :BRekHouderGeslacht1, 
	                                                    @BBevoegdheid1 = :BBevoegdheid1, 
	                                                    @BRekHouderTitel2 = :BRekHouderTitel2, 
	                                                    @BRekHouder2 = :BRekHouder2, 
	                                                    @BRekHouderTussenv2 = :BRekHouderTussenv2, 
	                                                    @BRekHouderVoorl2 = :BRekHouderVoorl2,
	                                                    @BRekHouderGeslacht2 = :BRekHouderGeslacht2, 
	                                                    @BBevoegdheid2 = :BBevoegdheid2,
	                                                    @Bedrijfafkorting = :Bedrijfafkorting,
	                                                    @ModelPortfolio = :ModelPortfolio,
                                                        @IncludeOPALData = :IncludeOPALData,
                                                        @UserProfile = :UserProfile,
                                                        @OPALData = :OPALData
	            ", parameters);
        session.Close();

    }
}
