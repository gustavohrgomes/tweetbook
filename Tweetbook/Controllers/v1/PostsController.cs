using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweetbook.Contracts;
using Tweetbook.Contracts.v1.Requests;
using Tweetbook.Contracts.v1.Responses;
using Tweetbook.Domain;
using Tweetbook.Services;

namespace Tweetbook.Controllers
{
    public class PostsController : Controller
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet(ApiRoutes.Posts.GetAll)]
        public IActionResult GetAll()
        {
            return Ok(_postService.GetPosts());
        }

        [HttpGet(ApiRoutes.Posts.Get)]
        public IActionResult Get([FromRoute] Guid id)
        {
            var post = _postService.GetPostById(id);

            if (post == null)
                return NotFound();

            return Ok(post);
        }

        [HttpPost(ApiRoutes.Posts.Create)]
        public IActionResult Create([FromBody] CreatePostRequest postRequest)
        {
            var post = new Post { Id = postRequest.Id };

            if (post.Id != Guid.Empty)
                post.Id = Guid.NewGuid();

            var baseURL = $"{ HttpContext.Request.Scheme }://{HttpContext.Request.Host.ToUriComponent()}";
            var locationURI = baseURL + "/" + ApiRoutes.Posts.Get.Replace("{id}", post.Id.ToString());

            var response = new PostResponse { Id = post.Id };

            return Created(locationURI, response);
        }
        
        [HttpPut(ApiRoutes.Posts.Update)]
        public IActionResult Update([FromRoute] Guid id, [FromBody] UpdatePostRequest postRequest)
        {
            var post = new Post
            {
                Id = id,
                Name = postRequest.Name
            };

            var updated = _postService.UpdatePost(post);

            if (updated)
                return Ok(post);

            return NotFound();
        }
    }
}
