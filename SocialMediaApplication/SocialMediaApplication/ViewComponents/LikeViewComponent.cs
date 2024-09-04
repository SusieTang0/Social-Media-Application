using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SocialMediaApplication.Models;

namespace SocialMediaApplication.ViewComponents
{            
    public class LikeViewComponent : ViewComponent
    {
        private readonly FirebaseService2 _firebaseService;

         public LikeViewComponent(FirebaseService2 firebaseService)
        {
            _firebaseService = firebaseService;
        }
    }
}