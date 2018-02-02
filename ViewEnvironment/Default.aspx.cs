using System;
using System.Collections;
using System.Web.UI;

public partial class _Default : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Session["RedisData"] = "This is stored in Redis Server , time - " + DateTime.Now.ToLongTimeString();
        // Get environment variables and dump them
        IDictionary vars = Environment.GetEnvironmentVariables();
        Environment.GetEnvironmentVariables();
        foreach (DictionaryEntry entry in vars)
        {
            // add to querystring all to dump all environment variables
            if (Request.QueryString["all"]!=null)
                Response.Write(entry.Key + " = " + entry.Value + "<br>");
        }
        //Storing CurrentTime in Session
        lblTime.Text = DateTime.Now.ToString();
        Session["CurrentTime"] = lblTime.Text;
        lblDotNetVersion.Text = Environment.Version.ToString();
        lblPort.Text = Environment.GetEnvironmentVariable("PORT");
        lblInstanceID.Text = Environment.GetEnvironmentVariable("INSTANCE_GUID");
        lblInstanceIndex.Text = Environment.GetEnvironmentVariable("INSTANCE_INDEX");
        lblInstanceStart.Text = DateTime.Now.Subtract(TimeSpan.FromMilliseconds(Environment.TickCount)).ToString();
        lblBoundServices.Text = Environment.GetEnvironmentVariable("VCAP_SERVICES");
        
    }
    protected void btnKill_Click(object sender, EventArgs e)
    {
        var connection = Dal.Startup.GetRedisConnectionString();
        log("Test.");
        //Environment.Exit(-1);
        lblTime.Text = Session["CurrentTime"].ToString();
        lblRedisData.Text = Session["RedisData"].ToString();
    }

    private void log(string message)
    {
        Console.WriteLine(message);
    }
}