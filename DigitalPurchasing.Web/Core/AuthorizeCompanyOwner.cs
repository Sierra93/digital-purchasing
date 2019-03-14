using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core;
using Microsoft.AspNetCore.Authorization;

namespace DigitalPurchasing.Web.Core
{
    public class AuthorizeCompanyOwner: AuthorizeAttribute
    {
        public AuthorizeCompanyOwner() => Roles = Consts.Roles.CompanyOwner;
    }

    public class AuthorizeAdmin: AuthorizeAttribute
    {
        public AuthorizeAdmin() => Roles = Consts.Roles.Admin;
    }
}
