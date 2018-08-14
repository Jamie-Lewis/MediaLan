using System;
using System.Collections.Generic;
using System.Web.Security;

namespace MediaLan
{
    public partial class Login : System.Web.UI.Page
    {
        public Dictionary<string, string> Users = new Dictionary<string, string>();

        public Login()
        {
            Users.Add("jamie", "phenom12");
            Users.Add("mya", "magicunicorn");
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (User.Identity != null && Request.Url.AbsolutePath != "/Logon") Response.Redirect("/Library");
        }

        protected void Logon_Click(object sender, EventArgs e)
        {
            var uname = UserName.Text.ToLower();
            if (Users.ContainsKey(uname))
            {
                if (Users[uname] == UserPass.Text)
                {
                    FormsAuthentication.RedirectFromLoginPage(UserName.Text, Persist.Checked);
                } else
                {
                    Msg.Text = "Invalid credentials. Please try again.";
                }
            } else
            {
                Msg.Text = "Invalid credentials. Please try again.";
            }
        }
    }
}