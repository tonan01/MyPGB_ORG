using AutoMapper;
using PGB.Auth.Application.Dtos;
using PGB.Auth.Application.Repositories;
using PGB.BuildingBlocks.Application.Exceptions;
using PGB.BuildingBlocks.Application.Queries;

namespace PGB.Auth.Application.Queries.Handlers
{
    public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserDto>
    {
        #region Dependencies
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        public GetUserByIdQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        #endregion

        #region Handle Method
        public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user == null)
                throw new NotFoundException($"User with ID {request.UserId} not found");

            return _mapper.Map<UserDto>(user);
        }
        #endregion
    }
}