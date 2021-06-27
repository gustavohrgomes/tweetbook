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
        public async Task<IActionResult> GetAll()
        {
            var posts = await _postService.GetPostsAsync();
            return Ok(posts);
        }

        [HttpGet(ApiRoutes.Posts.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var post = await _postService.GetPostByIdAsync(id);

            if (post == null)
                return NotFound();

            return Ok(post);
        }

        [HttpPost(ApiRoutes.Posts.Create)]
        public async Task<IActionResult> Create([FromBody] CreatePostRequest postRequest)
        {
            var post = new Post { Name = postRequest.Name };

            await _postService.CreatePostAsync(post);

            var baseURL = $"{ HttpContext.Request.Scheme }://{HttpContext.Request.Host.ToUriComponent()}";
            var locationURI = baseURL + "/" + ApiRoutes.Posts.Get.Replace("{id}", post.Id.ToString());

            var response = new PostResponse { Id = post.Id };

            return Created(locationURI, response);
        }
        
        [HttpPut(ApiRoutes.Posts.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdatePostRequest postRequest)
        {
            var post = new Post
            {
                Id = id,
                Name = postRequest.Name
            };

            var updated = await _postService.UpdatePostAsync(post);

            if (updated)
                return Ok(post);

            return NotFound();
        }

        [HttpDelete(ApiRoutes.Posts.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var deletedPost = await _postService.DeletePostAsync(id);

            if (deletedPost)
                return NoContent();

            return NotFound();
        }
    } 
}
