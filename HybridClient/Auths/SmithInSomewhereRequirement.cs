using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HybridClient.Auths
{
    public class SmithInSomewhereRequirement:IAuthorizationRequirement
    {
        public SmithInSomewhereRequirement()
        {

        }
    }
}
