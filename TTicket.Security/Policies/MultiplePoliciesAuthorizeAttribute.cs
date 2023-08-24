using Microsoft.AspNetCore.Mvc;

namespace TTicket.Security.Policies
{
    public class MultiplePoliciesAuthorizeAttribute : TypeFilterAttribute
    {
        public MultiplePoliciesAuthorizeAttribute(string policys, bool isAnd = false) : base(typeof(MultiplePoliciesAuthorizeFilter))
        {
            Arguments = new object[] { policys, isAnd };
        }
    }
}
