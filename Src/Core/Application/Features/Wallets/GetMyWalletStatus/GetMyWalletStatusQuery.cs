using Application.Exceptions;
using Application.Interfaces;
using Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Wallets.GetMyWalletStatus
{
    public class GetMyWalletStatusQuery : IRequest<ApiResponse<string>>
    {
    }

    public class GetMyWalletStatusQueryHandler : IRequestHandler<GetMyWalletStatusQuery, ApiResponse<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetMyWalletStatusQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<string>> Handle(GetMyWalletStatusQuery request, CancellationToken cancellationToken)
        {
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == _currentUserService.UserId, cancellationToken)
                ?? throw new ApiException("You do not have a wallet yet.");

            return new ApiResponse<string>(wallet.Status.ToString(), null);
        }
    }
}
