using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.WsFederation;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSFedAAD
{
	public partial class Startup
	{
		public void ConfigAuth(IAppBuilder app)
        {
			app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

			app.UseCookieAuthentication(new CookieAuthenticationOptions()
			{
				CookieHttpOnly = true,
			});

			app.UseWsFederationAuthentication(new WsFederationAuthenticationOptions()
			{
				MetadataAddress = "https://adfs.contoso.com/federationmetadata/2007-06/federationmetadata.xml",
				Wtrealm = "https://localhost:44391/"
			});
        }
	}
}