using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweetbook.Data;
using Tweetbook.Domain;

namespace Tweetbook.Services
{
    public class PostService : IPostService
    {
        private readonly DataContext _context;

        public PostService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Post>> GetPostsAsync()
        {
            return await _context.Posts
                .Include(posts => posts.Tags)
                .ToListAsync();
        }

        public async Task<Post> GetPostByIdAsync(Guid id)
        {
            return await _context.Posts
                .Include(posts => posts.Tags)
                .SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> CreatePostAsync(Post post)
        {
            post.Tags?.ForEach(postTag => postTag.TagName = postTag.TagName.ToLower());

            await AddNewTagsAsync(post);
            await _context.Posts.AddAsync(post);
            var created = await _context.SaveChangesAsync();
            
            return created > 0;
        }

        public async Task<bool> UpdatePostAsync(Post postToUpdate)
        {
            postToUpdate.Tags?.ForEach(postTag => postTag.TagName = postTag.TagName.ToLower());

            await AddNewTagsAsync(postToUpdate);
            _context.Posts.Update(postToUpdate);
            var updated = await _context.SaveChangesAsync();

            return updated > 0;
        }

        public async Task<bool> DeletePostAsync(Guid id)
        {
            var post = await GetPostByIdAsync(id);

            if (post == null)
                return false;

            _context.Posts.Remove(post);
            var deleted = await _context.SaveChangesAsync();
            return deleted > 0;
        }

        public async Task<List<Tag>> GetAllTagsAsync()
        {
            return await _context.Tags.AsNoTracking().ToListAsync();
        }

        public async Task<bool> UserOwnsPostAsync(Guid id, string userId)
        {
            var post = await _context.Posts.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);

            if (post == null)
            {
                return false;
            }

            if (post.UserId != userId)
            {
                return false;
            }

            return true;
        }

        private async Task AddNewTagsAsync(Post post)
        {
            foreach (var newTag in post.Tags)
            {
                var matchingTags = await _context.Tags.SingleOrDefaultAsync(existingTag => existingTag.Name == newTag.TagName);

                if (matchingTags != null)
                {
                    continue;
                }

                _context.Tags.Add(new Tag
                {
                    Name = newTag.TagName,
                    CreatorId = post.UserId,
                    CreatedOn = DateTime.UtcNow
                });
            }
        }
    }
}
