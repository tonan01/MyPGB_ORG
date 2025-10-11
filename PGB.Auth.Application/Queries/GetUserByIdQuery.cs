using PGB.Auth.Application.Dtos;
using PGB.BuildingBlocks.Application.Queries;

namespace PGB.Auth.Application.Queries
{
    public class GetUserByIdQuery : BaseQuery<UserDto>
    {
        #region Properties
        public Guid UserId { get; set; }
        #endregion

        #region Constructors
        public GetUserByIdQuery(Guid userId)
        {
            UserId = userId;
        }
        #endregion
    }
}