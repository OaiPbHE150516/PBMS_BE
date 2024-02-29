using AutoMapper;
using pbms_be.Data.Budget;

namespace pbms_be.Configurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig ()
        {
            //Account
            CreateMap<Data.Auth.Account, DTOs.AccountDTO>().ReverseMap();
            CreateMap<Data.Auth.Account, DTOs.AccountUpdateDTO>().ReverseMap();

            //Wallet

            CreateMap<Data.WalletF.Wallet, DTOs.WalletDTO>().ReverseMap();
            CreateMap<Data.WalletF.Wallet, DTOs.WalletCreateDTO>().ReverseMap();

           //Budget
            CreateMap<Data.Budget.Budget, DTOs.CreateBudgetDTO>().ReverseMap();
            CreateMap<Data.Budget.Budget, DTOs.UpdateBudgetDTO>().ReverseMap();
        
            //CollabFund
            CreateMap<Data.CollabFund.CollabFund, DTOs.CreateCollabFundDTO>().ReverseMap();
            CreateMap<Data.CollabFund.CollabFund, DTOs.UpdateCollabFundDTO>().ReverseMap();
            CreateMap<Data.CollabFund.CollabFund, DTOs.ChangeCollabFundActiveStateDTO>().ReverseMap();

            //CollabFundActivity
            CreateMap<Data.CollabFund.CollabFundActivity, DTOs.CreateCfaNoTransactionDTO>().ReverseMap();

        }
    }
}
