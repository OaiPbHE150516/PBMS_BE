using AutoMapper;
using pbms_be.Library;

namespace pbms_be.Configurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig ()
        {
            //Account
            CreateMap<Data.Auth.Account, DTOs.AccountDTO>().ReverseMap();
            CreateMap<Data.Auth.Account, DTOs.AccountUpdateDTO>().ReverseMap();
            CreateMap<Data.Auth.Account, DTOs.Account_VM_DTO>().ReverseMap();

            //Wallet

            CreateMap<Data.WalletF.Wallet, DTOs.WalletDTO>().ReverseMap();
            CreateMap<Data.WalletF.Wallet, DTOs.WalletCreateDTO>().ReverseMap();
            CreateMap<Data.WalletF.Wallet, DTOs.WalletDeleteDTO>().ReverseMap();
            CreateMap<Data.WalletF.Wallet, DTOs.Wallet_VM_DTO>()
                .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => LConvertVariable.ConvertToMoneyFormat(src.Balance)))
                .ReverseMap();


            //Budget
            CreateMap<Data.Budget.Budget, DTOs.CreateBudgetDTO>().ReverseMap();
            CreateMap<Data.Budget.Budget, DTOs.UpdateBudgetDTO>().ReverseMap();
            CreateMap<Data.Budget.Budget, DTOs.BudgetWithCategoryDTO>().ReverseMap();

            // Category
            CreateMap<Data.Filter.Category, DTOs.Category_VM_DTO>().ReverseMap();
            CreateMap<Data.Filter.Category, DTOs.CategoryDTO>().ReverseMap();

            // Transaction
            CreateMap<Data.Trans.Transaction, DTOs.Transaction_VM_DTO>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
                .ReverseMap();
        
            CreateMap<Data.WalletF.Wallet, DTOs.WalletUpdateDTO>().ReverseMap();
            CreateMap<Data.WalletF.Wallet, DTOs.ChangeWalletActiveStateDTO>().ReverseMap();

            //CollabFund
            CreateMap<Data.CollabFund.CollabFund, DTOs.CollabAccountDTO>().ReverseMap();
            CreateMap<Data.CollabFund.CollabFund, DTOs.CreateCollabFundDTO>().ReverseMap();
            CreateMap<Data.CollabFund.CollabFund, DTOs.UpdateCollabFundDTO>().ReverseMap();
            CreateMap<Data.CollabFund.CollabFund, DTOs.ChangeCollabFundActiveStateDTO>().ReverseMap();

            //CollabFundActivity
            CreateMap<Data.CollabFund.CollabFundActivity, DTOs.CreateCfaNoTransactionDTO>().ReverseMap();

            // CF_DividingMoney_MV_DTO and CF_DividingMoney, convert long number to money format
            CreateMap<Data.CollabFund.CF_DividingMoney, DTOs.CF_DividingMoney_MV_DTO>()
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => LConvertVariable.ConvertToMoneyFormat(src.TotalAmount)))
                .ForMember(dest => dest.AverageAmount, opt => opt.MapFrom(src => LConvertVariable.ConvertToMoneyFormat(src.AverageAmount)))
                .ForMember(dest => dest.RemainAmount, opt => opt.MapFrom(src => LConvertVariable.ConvertToMoneyFormat(src.RemainAmount)))
                .ReverseMap();

            // CF_DividingMoneyDetail and CF_DividingMoneyDetail_MV_DTO, convert long number to money format
            CreateMap<Data.CollabFund.CF_DividingMoneyDetail, DTOs.CF_DividingMoneyDetail_MV_DTO>()
                .ForMember(dest => dest.DividingAmount, opt => opt.MapFrom(src => LConvertVariable.ConvertToMoneyFormat(src.DividingAmount)))
                .ReverseMap();

            // CollabFundActivity_MV_DTO and CollabFundActivity, convert datetime to string, map Account to Account_VM_DTO
            CreateMap<Data.CollabFund.CollabFundActivity, DTOs.CollabFundActivity_MV_DTO>()
                .ForMember(dest => dest.MinusTimeNowString, opt => opt.MapFrom(src => LConvertVariable.ConvertMinusTimeNowString(src.CreateTime)))
                .ForMember(dest => dest.CreateTimeString, opt => opt.MapFrom(src => LConvertVariable.ConvertDateTimeToString(src.CreateTime)))
                .ReverseMap();

        }

        // function to convert long number to money format, ex: 1000000 -> 1.000.000

    }
}
