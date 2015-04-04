using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Neo4j;

//used for getting mail stuff done
using System.Net.Mail;
using System.Net;

namespace BCCApplication.Account
{
    public partial class SuggestTerm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                try { if (txtTermName.Text.Equals("")) throw new System.ArgumentException("Parameter cannot be null", "original"); }
                catch
                {
                    lblResult.Text = "Left 'Term Name parameter' blank!";
                    return;
                }
                try { if (txtParentString.Text.Equals("")) throw new System.ArgumentException("Parameter cannot be null", "original"); }
                catch
                {
                    lblResult.Text = "Left 'Parent String parameter' blank!";
                    return;
                }

                //Create the msg object to be sent
                MailMessage msg = new MailMessage();
                //Add your email address to the recipients
                msg.To.Add("bcclassification@gmail.com");
                //Configure the address we are sending the mail from
                MailAddress address = new MailAddress("bcclassification@gmail.com");
                msg.From = address;
                //Append their name in the beginning of the subject
                msg.Subject = "New Term Request";
                msg.Body = "There has been a request for the term: " + txtTermName.Text
                    + "\nAdd at location: " + txtParentString.Text
                    + "\nFor the reason: " + txtMessage.Text;

                //Configure an SmtpClient to send the mail.
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true; //only enable this if your provider requires it
                //Setup credentials to login to our sender email address ("UserName", "Password")
                NetworkCredential credentials = new NetworkCredential("bcclassification@gmail.com", "Classification2015");
                client.Credentials = credentials;

                //Send the msg
                client.Send(msg);

                //Display some feedback to the user to let them know it was sent
                lblResult.Text = "Your message was sent!";

                try
                {
                    // TODO: not hard code this!!!
                    var conn = new Neo4jDB();
                    conn.createNotification(
                        String.Format("{0}|{1}|{2}", txtTermName.Text, txtParentString.Text, txtMessage.Text), 
                        "bcclassification@gmail.com");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }

                //Clear the form
                txtTermName.Text = "";
                txtParentString.Text = "";
                txtMessage.Text = "";
            }
            catch
            {
                //If the message failed at some point, let the user know
                lblResult.Text = "Your message failed to send, please try again.";
            }
        }
    }
}