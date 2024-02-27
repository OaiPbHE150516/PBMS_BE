using AutoMapper;

namespace pbms_be.Configurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig ()
        {
            CreateMap<Data.Auth.Account, DTOs.AccountDTO>().ReverseMap();
            CreateMap<Data.Auth.Account, DTOs.AccountUpdateDTO>().ReverseMap();

            CreateMap<Data.WalletF.Wallet, DTOs.WalletDTO>().ReverseMap();
            CreateMap<Data.WalletF.Wallet, DTOs.WalletCreateDTO>().ReverseMap();
        
            //CollabFund
            CreateMap<Data.CollabFund.CollabFund, DTOs.CreateCollabFundDTO>().ReverseMap();
            CreateMap<Data.CollabFund.CollabFund, DTOs.UpdateCollabFundDTO>().ReverseMap();
            CreateMap<Data.CollabFund.CollabFund, DTOs.ChangeCollabFundActiveStateDTO>().ReverseMap();

            //CollabFundActivity
            CreateMap<Data.CollabFund.CollabFundActivity, DTOs.CreateCfaNoTransactionDTO>().ReverseMap();

            // CF_DividingMoney_MV_DTO and CF_DividingMoney, convert long number to money format
            CreateMap<Data.CollabFund.CF_DividingMoney, DTOs.CF_DividingMoney_MV_DTO>()
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => ConvertToMoneyFormat(src.TotalAmount)))
                .ForMember(dest => dest.AverageAmount, opt => opt.MapFrom(src => ConvertToMoneyFormat(src.AverageAmount)))
                .ForMember(dest => dest.RemainAmount, opt => opt.MapFrom(src => ConvertToMoneyFormat(src.RemainAmount)))
                .ReverseMap();

            // CF_DividingMoneyDetail and CF_DividingMoneyDetail_MV_DTO, convert long number to money format
            CreateMap<Data.CollabFund.CF_DividingMoneyDetail, DTOs.CF_DividingMoneyDetail_MV_DTO>()
                .ForMember(dest => dest.DividingAmount, opt => opt.MapFrom(src => ConvertToMoneyFormat(src.DividingAmount)))
                .ReverseMap();
        }

        // function to convert long number to money format, ex: 1000000 -> 1.000.000
        public string ConvertToMoneyFormat(long number)
        {
            var result = number.ToString("N0");
            result = result.Replace(",", ".") + " đ";
            return result;
        }
    }
}
