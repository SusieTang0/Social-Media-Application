//Shawnelle
using Firebase.Auth;
using Firebase.Storage;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.Extensions.ObjectPool;
using SocialMediaApplication.Models;

public class FirebaseService2
{
    private readonly FirebaseAuthProvider _authProvider;
    private readonly IFirebaseClient _firebaseClient;
    private readonly FirebaseStorage _firebaseStorage;

    public FirebaseService2(IConfiguration configuration)
    {
        _authProvider = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(configuration["Firebase:ApiKey"]));

        IFirebaseConfig config = new FireSharp.Config.FirebaseConfig
        {
            AuthSecret = configuration["Firebase:ApiKey"],
            BasePath = configuration["Firebase:DatabaseURL"]
        };
        _firebaseClient = new FireSharp.FirebaseClient(config);
        _firebaseStorage = new FirebaseStorage(configuration["Firebase:StorageBucket"]);
    }
    public async Task<SocialMediaApplication.Models.User> GetUserProfileAsync(string userId)
    {
        FirebaseResponse response = await _firebaseClient.GetAsync($"users/{userId}/profile");
        return response.ResultAs<SocialMediaApplication.Models.User>();
    }
    /*_______________Likes_______________________*/
    public async Task LikePost(string postId, string userId, string userName)
    {
        var like = new { UserId = userId, UserName = userName, LikedAt = DateTime.UtcNow };
        var response = await _firebaseClient.PushAsync($"posts/{postId}/likes/", like);

    }

    public async Task UnlikePost(string postId, string userId)
    {
        var likeNode = await _firebaseClient.GetAsync($"posts/{postId}/likes");
        var likes = likeNode.ResultAs<Dictionary<string,Like>>();

        var likeId = likes?.FirstOrDefault(x => x.Value.UserId == userId).Key;
        if(likeId != null){
            await _firebaseClient.DeleteAsync($"posts/{postId}/likes/{likeId}");
        }

    }
    public async Task<List<Like>> ShowPostLikes(string postId)
    {
        var response = await _firebaseClient.GetAsync($"posts/{postId}/likes");
        var likes = response.ResultAs<Dictionary<string,Like>>();

        return likes?.Values.ToList() ?? new List<Like>();
    }

    public async Task<int> ShowPostLikesCount(string postId)
    {
        var response = await _firebaseClient.GetAsync($"posts/{postId}/likes");
        var likes = response.ResultAs<Dictionary<string,Like>>();

        return likes?.Count ?? 0;

    }


    /*_______________Comments______________________*/
    public async Task AddComment(string postId, string authorId, string authorName, string content)
    {
        var comment = new Comment
        {
            AuthorId = authorId,
            AuthorName = authorName,
            PostId = postId,
            Content = content,
            CreatedTime = DateTime.UtcNow
        };
        var response = await _firebaseClient.PushAsync($"posts/{postId}/comments", comment);
    }

    public async Task EditComment(string postId, string content, string commentId)
    {
        var comment = new {
            Content = content,
            UpdatedTime = DateTime.UtcNow
        };
        var response = await _firebaseClient.PushAsync($"posts/{postId}/comments/{commentId}",comment);
    }

    public async Task DeleteComment(string postId,string userId)
    {
        var commentNode = await _firebaseClient.GetAsync($"posts/{postId}/comments");
        var comments = commentNode.ResultAs<Dictionary<string,Comment>>();

        var commentId = comments?.FirstOrDefault(x => x.Value.AuthorId == userId).Key;
        if(commentId != null){
            await _firebaseClient.DeleteAsync($"posts/{postId}/comments/{commentId}");
        }
    }
    public async Task<List<dynamic>> GetComments(string postId)
    {
        var response = await _firebaseClient.GetAsync($"posts/{postId}/comments");
        return response.ResultAs<List<dynamic>>();
    }

    public async Task LikeComment(string postId, string commentId, string userId, string userName)
    {
        var like = new { UserId = userId, UserName = userName, LikedAt = DateTime.UtcNow };
        var response = await _firebaseClient.PushAsync($"posts/{postId}/comments/{commentId}/likes", like);

    }

    public async Task UnlikeComment( string postId, string commentId, string userId)
    {
        var likeNode = await _firebaseClient.GetAsync($"posts/{postId}/comments/{commentId}/likes");
        var likes = likeNode.ResultAs<Dictionary<string,Like>>();

        var likeId = likes?.FirstOrDefault(x => x.Value.UserId == userId).Key;
        if(likeId != null){
            await _firebaseClient.DeleteAsync($"posts/{postId}/comments/{commentId}/likes/{likeId}");
        }

    }
    public async Task<List<Like>> ShowCommentLikes(string postId, string commentId)
    {
        var response = await _firebaseClient.GetAsync($"posts/{postId}/comments/{commentId}/likes");
        var likes = response.ResultAs<Dictionary<string,Like>>();

        return likes?.Values.ToList() ?? new List<Like>();
    }

    /*public async Task ShowCommentLikesCount(string postId, string commentId)
    {
        var response = await _firebaseClient.GetAsync($"posts/{postId}/comments/{commentId}/likes");
        var likes = response.ResultAs<Dictionary<string,Like>>();

        return likes?.Count ?? 0;

    }*/
}