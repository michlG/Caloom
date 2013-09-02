﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebInterface
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string hostName = Request.Url.DnsSafeHost;
            if (hostName == "www.protonit.net" || hostName == "www.caloom.com" || hostName == "demowww.norssi.protonit.net")
            {
                Response.Redirect("/www-public/index.html", true);
                return;
            }
            if (hostName.StartsWith("oip.msunit.citrus.fi"))
                Response.Redirect("/public/grp/default/publicsite/oip-public/oip-layout-landing.phtml", true);
            else if (hostName.StartsWith("publicoip.") || hostName.StartsWith("demopublicoip."))
                Response.Redirect("/grp/default/publicsite/oip-public/oip-layout-landing.phtml", true);
            else if(hostName.StartsWith("oip.") || hostName.StartsWith("demooip.") || hostName == "localhost" || hostName == "theball.protonit.net" )
                Response.Redirect("/about/oip-public/oip-layout-register.phtml", true);
            else if (hostName.StartsWith("www.") || hostName.StartsWith("demowww") 
                || hostName.StartsWith("globalimpact.aalto.fi") || hostName.StartsWith("weconomy.aaltoglobalimpact.org"))
                Response.Redirect("/www-public/oip-layout-landing.phtml", true);
            else if(hostName == "weconomy.fi")
                Response.Redirect("http://www.weconomy.fi", true);
            else if(hostName == "aaltoglobalimpact.org")
                Response.Redirect("http://www.aaltoglobalimpact.org", true);
            else if(hostName == "hacktheball.protonit.net")
                Response.Redirect("/www-public/index.html", true);
        }
    }
}