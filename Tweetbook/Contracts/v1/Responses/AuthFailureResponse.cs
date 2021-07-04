using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tweetbook.Contracts.v1.Responses
{
    public class AuthFailureResponse
    {
        public IEnumerable<string> Errors { get; set; }
    }
}
