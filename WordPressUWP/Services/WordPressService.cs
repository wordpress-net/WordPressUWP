﻿using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WordPressPCL;
using WordPressPCL.Models;
using WordPressPCL.Utility;
using WordPressUWP.Helpers;
using WordPressUWP.Interfaces;

namespace WordPressUWP.Services
{
    public class WordPressService : ViewModelBase, IWordPressService 
    {
        private WordPressClient _client;

        private bool _isLoggedIn;
        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            set { Set(ref _isLoggedIn, value); }
        }

        public WordPressService()
        {
            _client = new WordPressClient(ApiCredentials.WordPressUri);
            IsLoggedIn = false;
        }

        public async Task<bool> AuthenticateUser(string username, string password)
        {
            _client.AuthMethod = AuthMethod.JWT;
            await _client.RequestJWToken(username, password);
            return await IsUserAuthenticated();
        }

        public async Task<List<CommentThreaded>> GetCommentsForPost(int postid)
        {
            var comments = await _client.Comments.Query(new CommentsQueryBuilder()
            {
                Posts = new int[] { postid },
                Page = 1,
                PerPage = 100
            });

            return ThreadedCommentsHelper.GetThreadedComments(comments);
        }

        public async Task<IEnumerable<Post>> GetLatestPosts(int page = 0, int perPage = 20)
        {
            return await _client.Posts.Query(new PostsQueryBuilder()
            {
                Page = page,
                PerPage = perPage,
                Embed = true
            });
        }

        public async Task<User> GetUser()
        {
            return await _client.Users.GetCurrentUser();
        }

        public async Task<bool> IsUserAuthenticated()
        {
            IsLoggedIn = await _client.IsValidJWToken();
            return IsLoggedIn;
        }

        public async Task<Comment> PostComment(int postId, string text)
        {
            return await _client.Comments.Create(new Comment(postId, text));
        }
    }
}
