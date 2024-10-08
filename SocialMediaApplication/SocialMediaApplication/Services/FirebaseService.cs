

using Firebase.Auth;
using Firebase.Storage;
using FireSharp.Interfaces;
using FireSharp.Response;

public class FirebaseService
{
    private readonly FirebaseAuthProvider _authProvider;
    private readonly IFirebaseClient _firebaseClient;
    private readonly FirebaseStorage _firebaseStorage;

    public FirebaseService(IConfiguration configuration)
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

    public async Task<FirebaseAuthLink> RegisterUser(string email, string password)
    {
        try
        {
            return await _authProvider.CreateUserWithEmailAndPasswordAsync(email, password);
        }
        catch (FirebaseAuthException ex)
        {
            if (ex.Message.Contains("EMAIL_EXISTS"))
            {
                throw new Exception("This email is already registered.");
            }
            throw new Exception("This email is already registered. Please Login.");
        }

    }



    public async Task<FirebaseAuthLink> LoginUser(string email, string password)
    {
        try
        {
            return await _authProvider.SignInWithEmailAndPasswordAsync(email, password);
        }
        catch (FirebaseAuthException ex)
        {
            if (ex.Message.Contains("EMAIL_NOT_FOUND"))
            {
                throw new Exception("This email is not registered.");
            }
            else if (ex.Message.Contains("INVALID_PASSWORD"))
            {
                throw new Exception("The password is incorrect.");
            }
            throw new Exception("The password/email is incorrect or does not exist.");
        }
    }


    public async Task<string> UploadProfilePictureAsync(Stream fileStream, string fileName)
    {
        try
        {
            var task = await _firebaseStorage
                .Child("profile_pictures")
                .Child(fileName)
                .PutAsync(fileStream);
            return task;
        }
        catch (FirebaseStorageException ex)
        {

            throw new Exception("Failed to upload profile picture. Please try again.");
        }
    }

    public async Task<SocialMediaApplication.Models.User> GetUserProfileAsync(string userId)
    {
        FirebaseResponse response = await _firebaseClient.GetAsync($"users/{userId}/profile");
        return response.ResultAs<SocialMediaApplication.Models.User>();
    }

    public async Task<FirebaseResponse> SaveUserProfileAsync(string userId, SocialMediaApplication.Models.User userProfile)
    {


        return await _firebaseClient.SetAsync($"users/{userId}/profile", userProfile);
    }


    public async Task SendPasswordResetEmailAsync(string email)
    {
        try
        {
            await _authProvider.SendPasswordResetEmailAsync(email);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to send password reset email. Please try again.");
        }
    }

    public async Task<bool> CheckIfUserExists(string email)
    {
        try
        {

            await _authProvider.SignInWithEmailAndPasswordAsync(email, "dummyPassword");

            return true;
        }
        catch (FirebaseAuthException ex)
        {

            if (ex.Message.Contains("EMAIL_NOT_FOUND"))
            {
                return false;
            }

            if (ex.Message.Contains("INVALID_PASSWORD"))
            {
                return true;
            }

            throw new Exception("An error occurred while checking the email.");
        }
    }





}