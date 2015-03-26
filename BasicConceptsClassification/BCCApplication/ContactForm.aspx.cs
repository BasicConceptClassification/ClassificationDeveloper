using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

//used for getting mail stuff done
using System.Net.Mail;
using System.Net;

namespace BCCApplication
{
    public partial class ContactForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                //Create the msg object to be sent
                MailMessage msg = new MailMessage();
                //Add your email address to the recipients
                msg.To.Add("kimfkramer@gmail.com");
                //Configure the address we are sending the mail from
                MailAddress address = new MailAddress("kimfkramer@gmail.com");
                msg.From = address;
                //Append their name in the beginning of the subject
                msg.Subject = txtName.Text + " :  " + ddlSubject.Text;
                msg.Body = txtEmail.Text + " :  " + txtMessage.Text;

                //Configure an SmtpClient to send the mail.
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true; //only enable this if your provider requires it
                //Setup credentials to login to our sender email address ("UserName", "Password")
                NetworkCredential credentials = new NetworkCredential("kimfkramer@gmail.com", "Sesshomaru101");
                client.Credentials = credentials;

                //Send the msg
                client.Send(msg);

                //Display some feedback to the user to let them know it was sent
                lblResult.Text = "Your message was sent!";

                //Clear the form
                txtName.Text = "";
                txtEmail.Text = "";
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