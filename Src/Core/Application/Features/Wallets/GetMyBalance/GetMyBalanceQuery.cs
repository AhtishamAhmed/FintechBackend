using Application.Exceptions;
using Application.Interfaces;
using Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Wallets.GetMyBalance
{
    public class BalanceDto
    {
        public decimal Balance { get; set; }
        public string Currency { get; set; } = string.Empty;
    }

    public class GetMyBalanceQuery : IRequest<ApiResponse<BalanceDto>>
    {
    }

    public class GetMyBalanceQueryHandler : IRequestHandler<GetMyBalanceQuery, ApiResponse<BalanceDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetMyBalanceQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<BalanceDto>> Handle(GetMyBalanceQuery request, CancellationToken cancellationToken)
        {
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == _currentUserService.UserId, cancellationToken)
                ?? throw new ApiException("You do not have a wallet yet.");

            return new ApiResponse<BalanceDto>(new BalanceDto { Balance = wallet.Balance, Currency = wallet.Currency });
        }
    }
}
