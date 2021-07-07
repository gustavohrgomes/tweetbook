using System;

namespace Tweetbook.Contracts.v1.Responses
{
    public class TagResponse
    {
        public string Name { get; set; }
        public string CreatorId { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
