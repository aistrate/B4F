using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Net;
using System.IO;
using System.Xml.Serialization;
using B4F.TotalGiro.Dal;

using B4F.TotalGiro.CRM;
using B4F.TotalGiro.StaticData;
using B4F.Web.TGWebService;

public partial class Test_TestWebServiceRequest : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void createTestRequest()
    {
        WSRequest request = new WSRequest();
        request.Version = "1.0";
        request.Username = "test_1119";
        request.Password = "test";

        WSAccountRequestData ard = new WSAccountRequestData();

        ard.IncludeOPALData = 1;
        ard.AccountNumber = "EGVL999999";
        ard.FirstDeposit = 123456.34f;
        ard.ModelPortfolio = "J";
        ard.MoneyAccount = "P12345678";
        ard.MoneyAccountHolder = "Kribbe";
        ard.PeriodicWithdrawal = true;
        ard.PeriodWithdrawal = "year";
        ard.PeriodicWithdrawalAmount = 50f;
        ard.Remisier = "Finix";

        WSPerson person = new WSPerson();
        person.BirthDate = new DateTime(1970, 6, 24);
        person.LastName = "Kribbe";
        person.Initials = "S";
        person.Sex = "M";
        person.Title = "ing";
        person.SOFINumber = "179218773";

        WSIdentification id = new WSIdentification();
        id.Number = "999888777";
        id.Type = "Paspoort";
        id.ValidityPeriod = new DateTime(2010, 10, 10);

        person.Identification = id;

        WSAddress postaddress = new WSAddress("Herengracht", "199", "b", "1017EL", "Amsterdam", "Netherlands");

        WSContactDetails contactdetails = new WSContactDetails();
        contactdetails.PhoneMobile = "+31628785883";
        contactdetails.PostAddress = postaddress;

        person.ContactDetails = contactdetails;

        ard.Applicant = person;

        WSPerson person2 = new WSPerson();
        person2.BirthDate = new DateTime(1968, 9, 24);
        person2.LastName = "Partner";
        person2.Initials = "B";
        person2.Sex = "M";
        person2.Title = "ir";
        person2.SOFINumber = "782387678";

        WSIdentification id2 = new WSIdentification();
        id2.Number = "111222333";
        id2.Type = "Rijbewijs";
        id2.ValidityPeriod = new DateTime(2010, 10, 10);

        person2.Identification = id2;

        WSAddress postaddress2 = new WSAddress("Keizersgracht", "222", "", "1017AB", "Amsterdam", "Netherlands");

        WSContactDetails contactdetails2 = new WSContactDetails();
        contactdetails2.PhoneMobile = "+3162347802";
        contactdetails2.PostAddress = postaddress2;

        person2.ContactDetails = contactdetails2;

        ard.SecondApplicant = person2;

        request.UpdateRequestData = ard;

        WSOPALData opaldata = new WSOPALData();
        WSOPALInput opalinput = new WSOPALInput();
        Results results  = new Results();

        results.SchemaName = "steven.xsd";
        //results.Version = "1.0";

        opalinput.outputresults = results;

        opaldata.OpalInput = opalinput;

        WSOPALOutput opaloutput = new WSOPALOutput();
        Project project = new Project();
        project.ProjectName = "project1";

        opaloutput.OutputProject = project;

        opaldata.OpalOutput = opaloutput;

        request.UpdateRequestData.OPALData = opaldata;

        // Serialize the request
        XmlSerializer s = new XmlSerializer(typeof(WSRequest));
        FileStream w = new FileStream("c:\\temp\\wsrequest.xml", FileMode.Create);
        s.Serialize(w, request);
        w.Close();
    }

    // readStream is the stream you need to read
    // writeStream is the stream you want to write to
    private void ReadWriteStream(Stream readStream, Stream writeStream)
    {
        int Length = 256;
        Byte[] buffer = new Byte[Length];
        int bytesRead = readStream.Read(buffer, 0, Length);
        // write the required bytes
        while (bytesRead > 0)
        {
            writeStream.Write(buffer, 0, bytesRead);
            bytesRead = readStream.Read(buffer, 0, Length);
        }
        readStream.Close();
        writeStream.Close();
    }

    // Post a testrequest to the TotalGiro Web service
    protected void postRequest()
    {
        WebRequest req = null;
        WebResponse rsp = null;
        WSResponse resp = null;
        try
        {
            string fileName = Server.MapPath(".") + "\\wsrequest.xml";
            //string fileName = "c:\\temp\\wsrequest.xml";

            string localPath = Request.Url.LocalPath.Remove(Request.Url.LocalPath.IndexOf("/test/", StringComparison.InvariantCultureIgnoreCase));
            string surl = string.Format("http://{0}{1}/TGWebService/WSInsertEGAanvraag.aspx", Request.Url.Authority, localPath);
            //string surl = string.Format("http://{0}/TGWebService/WSInsertEGAanvraag.aspx", "145.7.20.130/tg");
            req = WebRequest.Create(surl);
            req.Method = "POST";        // Post method
            req.ContentType = "text/xml";     // content type

            // Wrap the request stream with a text-based writer
            StreamWriter writer = new StreamWriter(req.GetRequestStream());
            // Write the xml text into the stream
            writer.WriteLine(this.GetTextFromXMLFile(fileName));
            writer.Close();
            // Send the data to the webserver
            rsp = req.GetResponse();

            //string saveTo = "c:\\wsresponse.xml";
            //// create a write stream
            //FileStream writeStream = new FileStream(saveTo, FileMode.Create, FileAccess.Write);
            //// write to the stream
            //ReadWriteStream(rsp.GetResponseStream(),writeStream);
             

            XmlSerializer s = new XmlSerializer(typeof(WSResponse));
            StreamReader reader = new StreamReader(rsp.GetResponseStream());
            resp = (WSResponse)s.Deserialize(reader);
            reader.Close();

        }
        catch (WebException webEx)
        {
            lblError.Text = webEx.Message;

        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
        }
        finally
        {
            //if (req != null) req.GetRequestStream().Close();
            if (rsp != null) rsp.GetResponseStream().Close();
        }

        if (resp != null)
        {
            foreach (WSMessage msg in resp.MetaData.Messages)
                lblResponse.Text = msg.ToString() + "<br/>";

           
        }
    }

    // reads the content of a stream 
    private string ReadTextFromStream(StreamReader reader)
    {
        string ret = reader.ReadToEnd();
        reader.Close();
        return ret;
    }

    // reads the contents of a file
    private string GetTextFromXMLFile(string file)
    {
        StreamReader reader = new StreamReader(file);
        return ReadTextFromStream(reader);
    }

    protected void btnSendRequest_Click(object sender, EventArgs e)
    {
        postRequest();
    }

    protected void btnCreateTestFile_Click(object sender, EventArgs e)
    {
        createTestRequest();
    }
    protected void btnTestText_Click(object sender, EventArgs e)
    {
        IDalSession session = null;
        SqlDataReader rdr = null;

        try
        {
            session = NHSessionFactory.CreateSession();

            SqlCommand cmd = new SqlCommand(
                "select opaldata from tblegaanvraag where id in (select max(id) from tblegaanvraag)",(SqlConnection) session.Connection);

            // execute the command
            rdr = cmd.ExecuteReader();

            // iterate through results, printing each to console
            while (rdr.Read())
            {
                if (!rdr.IsDBNull(0))
                    lblResponse.Text = rdr.GetString(0);
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

    }
}
