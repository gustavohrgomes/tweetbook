using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Tweetbook.Contracts;
using Tweetbook.Contracts.v1.Requests;
using Tweetbook.Contracts.v1.Responses;
using Tweetbook.Domain;
using Tweetbook.Extensions;
using Tweetbook.Services;

namespace Tweetbook.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
            return Ok(posts.Select(post => new PostResponse
            {
                Id = post.Id,
                Name = post.Name,
                Tags = post.Tags
            }));
        }

        [HttpGet(ApiRoutes.Posts.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var post = await _postService.GetPostByIdAsync(id);

            if (post == null)
                return NotFound();

            return Ok(new PostResponse
            {
                Id = post.Id,
                Name = post.Name,
                Tags = post.Tags
            });
        }

        [HttpPost(ApiRoutes.Posts.Create)]
        public async Task<IActionResult> Create([FromBody] CreatePostRequest postRequest)
        {
            var newPostId = Guid.NewGuid();
            var post = new Post
            {
                Id = newPostId,
                Name = postRequest.Name,
                UserId = HttpContext.GetUserId(),
                Tags = postRequest.Tags.Select(tagName => new PostTag { TagName = tagName, PostId = newPostId }).ToList()
            };

            await _postService.CreatePostAsync(post);

            var baseURL = $"{ HttpContext.Request.Scheme }://{HttpContext.Request.Host.ToUriComponent()}";
            var locationURI = baseURL + "/" + ApiRoutes.Posts.Get.Replace("{id}", post.Id.ToString());

            var response = new PostResponse
            {
                Id = post.Id,
                Name = post.Name,
                Tags = post.Tags
            };

            return Created(locationURI, response);
        }
        
        [HttpPut(ApiRoutes.Posts.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdatePostRequest postRequest)
        {
            var userOwnsPost = await _postService.UserOwnsPostAsync(id, HttpContext.GetUserId());

            if (!userOwnsPost)
            {
                return BadRequest(new { error = "You do not own this post" });
            }

            var post = await _postService.GetPostByIdAsync(id);
            post.Name = postRequest.Name;
            post.Tags = postRequest.Tags.Select(tagName => new PostTag { TagName = tagName, PostId = post.Id }).ToList();

            var updated = await _postService.UpdatePostAsync(post);

            if (updated)
            {
                return Ok(new PostResponse
                {
                    Id = post.Id,
                    Name = post.Name,
                    Tags = post.Tags
                });
            }

            return NotFound();
        }

        [HttpDelete(ApiRoutes.Posts.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var userOwnsPost = await _postService.UserOwnsPostAsync(id, HttpContext.GetUserId());

            if (!userOwnsPost)
            {
                return BadRequest(new { error = "You do not own this post" });
            }

            var deletedPost = await _postService.DeletePostAsync(id);

            if (deletedPost)
                return NoContent();

            return NotFound();
        }
    } 
}
