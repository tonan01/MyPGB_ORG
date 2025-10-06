using PGB.BuildingBlocks.Application.Queries;
using PGB.BuildingBlocks.Application.Models;
using PGB.Auth.Application.Dtos;

namespace PGB.Auth.Application.Queries
{
    public class GetUsersQuery : PagedQuery<UserDto>
    {
        #region Filter Properties
        public bool? IsActive { get; set; }
        public bool? IsEmailVerified { get; set; }
        public bool? IsLocked { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; } 
        #endregion
    }
}