using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace HRMS.Services
{
    public class AuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        private readonly AuthorizationOptions Authorization;

        public AuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
            Authorization = options.Value;
        }

        public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            var policy = await base.GetPolicyAsync(policyName);
            string[] claims = policyName.Split(":");

            if (policy == null)
            {
                policy = new AuthorizationPolicyBuilder().RequireClaim(claims[0], claims[1]).Build();
                Authorization.AddPolicy(policyName, policy);
            }
            return policy;
        }
    }
}
