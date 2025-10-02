using AutoMapper;
using PGB.BuildingBlocks.Application.Models;
using PGB.BuildingBlocks.Application.Queries;
using PGB.Auth.Application.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PGB.Auth.Application.Queries.Handlers
{
    public class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, PagedResult<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var pagedResult = await _userRepository.GetPagedAsync(request, cancellationToken);

            var userDtos = _mapper.Map<List<UserDto>>(pagedResult.Items);

            return new PagedResult<UserDto>(userDtos, pagedResult.TotalCount, pagedResult.Page, pagedResult.PageSize);
        }
    }
}