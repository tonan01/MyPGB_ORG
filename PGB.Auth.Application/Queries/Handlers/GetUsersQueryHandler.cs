using AutoMapper;
using PGB.BuildingBlocks.Application.Models;
using PGB.BuildingBlocks.Application.Queries;
using PGB.Auth.Application.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PGB.Auth.Application.Dtos;

namespace PGB.Auth.Application.Queries.Handlers
{
    public class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, PagedResult<UserDto>>
    {
        #region Fields
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        public GetUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        #endregion

        #region Handle Method
        public async Task<PagedResult<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var pagedResult = await _userRepository.GetPagedAsync(request, cancellationToken);

            var userDtos = _mapper.Map<List<UserDto>>(pagedResult.Items);

            return new PagedResult<UserDto>(userDtos, pagedResult.TotalCount, pagedResult.Page, pagedResult.PageSize);
        } 
        #endregion
    }
}