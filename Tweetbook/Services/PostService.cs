using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweetbook.Domain;

namespace Tweetbook.Services
{
    public class PostService : IPostService
    {
        private readonly List<Post> _posts;

        public PostService()
        {
            _posts = new List<Post>();
            for (int i = 0; i < 5; i++)
            {
                _posts.Add(new Post { Id = Guid.NewGuid(), Name = $"Post Number {i}" });
            }
        }
        public List<Post> GetPosts()
        {
            return _posts;
        }

        public Post GetPostById(Guid id)
        {
            return _posts.SingleOrDefault(p => p.Id == id);
        }

        
    }
}
