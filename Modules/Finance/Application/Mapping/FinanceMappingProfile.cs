using AutoMapper;
using FinanceApplication.financedtos;
using FinanceDomain.Entities;

namespace FinanceApplication.Mapping
{
    public sealed class FinanceMappingProfile : Profile
    {
        public FinanceMappingProfile()
        {
            // ── Wallet ─────────────────────────────────────────────────────────

            CreateMap<Wallet, WalletDto>()
                .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance.Amount))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Balance.Currency));

            // ── FinancialCategory ──────────────────────────────────────────────

            CreateMap<FinancialCategory, FinancialCategoryDto>();

            // ── FinancialTransaction ───────────────────────────────────────────
            // Navigation names resolved via Include() in repository queries

            CreateMap<FinancialTransaction, FinancialTransactionDto>()
                .ForMember(dest => dest.WalletName,
                    opt => opt.MapFrom(src => src.Wallet != null ? src.Wallet.Name : null))
                .ForMember(dest => dest.CategoryName,
                    opt => opt.MapFrom(src => src.FinancialCategory != null ? src.FinancialCategory.Name : null))
                // DestinationWalletName must be populated by the service (second wallet lookup)
                .ForMember(dest => dest.DestinationWalletName, opt => opt.Ignore());
        }
    }
}
