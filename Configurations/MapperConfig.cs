using AutoMapper;
using pbms_be.DTOs;
using pbms_be.Library;

namespace pbms_be.Configurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            //Account
            CreateMap<Data.Auth.Account, DTOs.AccountDTO>().ReverseMap();
            CreateMap<Data.Auth.Account, DTOs.AccountUpdateDTO>().ReverseMap();
            CreateMap<Data.Auth.Account, DTOs.Account_VM_DTO>().ReverseMap();
            CreateMap<Data.Auth.Account, DTOs.AccountInCollabFundDTO>().ReverseMap();
            CreateMap<Data.Auth.Account, DTOs.AccountDetailInCollabFundDTO>().ReverseMap();

            //Wallet

            CreateMap<Data.WalletF.Wallet, DTOs.WalletDTO>().ReverseMap();
            CreateMap<Data.WalletF.Wallet, DTOs.WalletCreateDTO>().ReverseMap();
            CreateMap<Data.WalletF.Wallet, DTOs.WalletDeleteDTO>().ReverseMap();
            CreateMap<Data.WalletF.Wallet, DTOs.Wallet_VM_DTO>()
                .ForMember(dest => dest.BalanceStr, opt => opt.MapFrom(src => LConvertVariable.ConvertToMoneyFormat(src.Balance)))
                .ForMember(dest => dest.CreateTimeStr, opt => opt.MapFrom(src => LConvertVariable.ConvertDateTimeToStringCustom(src.CreateTime, ConstantConfig.DEFAULT_DATE_FORMAT)))
                .ReverseMap();
            CreateMap<Data.WalletF.Wallet, DTOs.Wallet_Balance_VM_DTO>()
                .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => LConvertVariable.ConvertToMoneyFormat(src.Balance)))
                .ReverseMap();
            CreateMap<Data.WalletF.Wallet, DTOs.WalletDetail_VM_DTO>().ReverseMap();
            CreateMap<Data.WalletF.Wallet, DTOs.WalletInTransaction_VM_DTO>().ReverseMap();


            //Budget
            CreateMap<Data.Budget.Budget, DTOs.CreateBudgetDTO>().ReverseMap();
            CreateMap<Data.Budget.Budget, DTOs.UpdateBudgetDTO>().ReverseMap();
            CreateMap<Data.Budget.Budget, DTOs.DeleteBudgetDTO>().ReverseMap();
            CreateMap<Data.Budget.Budget, DTOs.BudgetWithCategoryDTO>()
                .ForMember(dest => dest.RemainAmount, otp => otp.MapFrom(src => src.TargetAmount - src.CurrentAmount))
                .ForMember(dest => dest.RemainAmountStr, opt => opt.MapFrom(src => LConvertVariable.ConvertToMoneyFormat(src.TargetAmount - src.CurrentAmount)))
                .ForMember(dest => dest.CurrentAmountStr, opt => opt.MapFrom(src => LConvertVariable.ConvertToMoneyFormat(src.CurrentAmount)))
                .ForMember(dest => dest.TargetAmountStr, opt => opt.MapFrom(src => LConvertVariable.ConvertToMoneyFormat(src.TargetAmount)))
                .ForMember(dest => dest.BeginDateStr, opt => opt.MapFrom(src => LConvertVariable.ConvertDateTimeToStringCustom(src.BeginDate, ConstantConfig.DEFAULT_DATE_FORMAT)))
                .ForMember(dest => dest.EndDateStr, opt => opt.MapFrom(src => LConvertVariable.ConvertDateTimeToStringCustom(src.EndDate, ConstantConfig.DEFAULT_DATE_FORMAT)))
                .ForMember(dest => dest.CreateTimeStr, opt => opt.MapFrom(src => LConvertVariable.ConvertDateTimeToString(src.CreateTime)))
                .ForMember(dest => dest.PercentProgress, opt => opt.MapFrom(src => LConvertVariable.CalculatePercentProgress(src.CurrentAmount, src.TargetAmount, 2)))
                .ReverseMap();

            // BalanceHisLog
            CreateMap<Data.Balance.BalanceHistoryLog, DTOs.BalanceHisLog_VM_DTO>()
                .ForMember(dest => dest.BalanceStr, opt => opt.MapFrom(src => LConvertVariable.ConvertToMoneyFormat(src.Balance)))
                .ForMember(dest => dest.HisLogDateStr, opt => opt.MapFrom(src => LConvertVariable.ConvertDateTimeToString(src.HisLogDate)))
                .ReverseMap();

            // Category
            CreateMap<Data.Filter.Category, DTOs.Category_VM_DTO>().ReverseMap();
            CreateMap<Data.Filter.Category, DTOs.CategoryDTO>().ReverseMap();
            CreateMap<Data.Filter.Category, DTOs.CategoryInTransaction_VM_DTO>().ReverseMap();
            CreateMap<Data.Filter.Category, DTOs.CategoryCreateDTO>().ReverseMap();
            CreateMap<Data.Filter.Category, DTOs.CategoryUpdateDTO>().ReverseMap();

            // CategoryTree_VM_DTO
            CreateMap<Data.Filter.Category, DTOs.CategoryTree_VM_DTO>().ReverseMap();


            // Transaction
            CreateMap<Data.Trans.Transaction, DTOs.Transaction_VM_DTO>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
                .ForMember(dest => dest.TotalAmountStr, opt => opt.MapFrom(src => LConvertVariable.ConvertToMoneyFormat(src.TotalAmount)))
                .ReverseMap();
            CreateMap<Data.Trans.Transaction, DTOs.TransactionInList_VM_DTO>()
                // convert transaction date from utc to local time
                .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => LConvertVariable.ConvertUtcToLocalTime(src.TransactionDate)))
                .ForMember(dest => dest.TotalAmountStr, opt => opt.MapFrom(src => LConvertVariable.ConvertToMoneyFormat(src.TotalAmount)))
                .ForMember(dest => dest.TransactionDateStr, opt => opt.MapFrom(src => LConvertVariable.ConvertDateTimeToString(LConvertVariable.ConvertUtcToLocalTime(src.TransactionDate))))
                .ForMember(dest => dest.TransactionDateMinus, opt => opt.MapFrom(src => LConvertVariable.ConvertMinusTimeNowMonthString(src.TransactionDate)))
                .ForMember(dest => dest.TimeStr, opt => opt.MapFrom(src => LConvertVariable.ConvertDateTimeToStringCustom(LConvertVariable.ConvertUtcToLocalTime(src.TransactionDate), ConstantConfig.DEFAULT_TIME_FORMAT)))
                .ForMember(dest => dest.DateSlashStr, opt => opt.MapFrom(src => LConvertVariable.ConvertDateTimeToStringCustom(LConvertVariable.ConvertUtcToLocalTime(src.TransactionDate), ConstantConfig.DEFAULT_DATE_FORMAT_SLASH)))
                .ForMember(dest => dest.DateShortStr, opt => opt.MapFrom(src => LConvertVariable.ConvertDateToShortStr(LConvertVariable.ConvertUtcToLocalTime(src.TransactionDate))))
                .ForMember(dest => dest.DayOfWeekStrShort, opt => opt.MapFrom(src => LConvertVariable.ConvertDateTimeToDayOfWeekShort(LConvertVariable.ConvertUtcToLocalTime(src.TransactionDate))))
                .ForMember(dest => dest.DayOfWeekStrMdl, opt => opt.MapFrom(src => LConvertVariable.ConvertDateTimeToDayOfWeekMdl(LConvertVariable.ConvertUtcToLocalTime(src.TransactionDate))))
                .ForMember(dest => dest.DayOfWeekStrLong, opt => opt.MapFrom(src => LConvertVariable.ConvertDateTimeToDayOfWeekLong(LConvertVariable.ConvertUtcToLocalTime(src.TransactionDate))))
                .ReverseMap();
            CreateMap<Data.Trans.Transaction, DTOs.TransactionDetail_VM_DTO>()
               .ForMember(dest => dest.TotalAmountStr, opt => opt.MapFrom(src => LConvertVariable.ConvertToMoneyFormat(src.TotalAmount)))
               .ForMember(dest => dest.TransactionDateStr, opt => opt.MapFrom(src => LConvertVariable.ConvertDateTimeToString(LConvertVariable.ConvertUtcToLocalTime(src.TransactionDate))))
               .ForMember(dest => dest.TransactionDateMinus, opt => opt.MapFrom(src => LConvertVariable.ConvertMinusTimeNowMonthString(src.TransactionDate)))
               .ForMember(dest => dest.TransactionTimeStr, opt => opt.MapFrom(src => LConvertVariable.ConvertDateTimeToStringCustom(LConvertVariable.ConvertUtcToLocalTime(src.TransactionDate), ConstantConfig.DEFAULT_TIME_FORMAT)))
               .ReverseMap();
            CreateMap<Data.Trans.Transaction, DTOs.TransactionCreateDTO>().ReverseMap();
            CreateMap<Data.Trans.Transaction, DTOs.TransactionWithoutInvoiceCreateDTO>().ReverseMap();

            // TransactionCreateWithImageDTO
            CreateMap<Data.Trans.Transaction, DTOs.TransactionCreateWithImageDTO>().ReverseMap();

            // Invoice
            CreateMap<Data.Invo.Invoice, DTOs.InvoiceCreateDTO>().ReverseMap();

            // ProductInInvoice
            CreateMap<Data.Invo.ProductInInvoice, DTOs.ProductInInvoiceCreateDTO>().ReverseMap();

            CreateMap<Data.WalletF.Wallet, DTOs.WalletUpdateDTO>().ReverseMap();
            CreateMap<Data.WalletF.Wallet, DTOs.ChangeWalletActiveStateDTO>().ReverseMap();

            //CollabFund
            CreateMap<Data.CollabFund.CollabFund, DTOs.CollabAccountDTO>().ReverseMap();
            CreateMap<Data.CollabFund.CollabFund, DTOs.CreateCollabFundDTO>().ReverseMap();
            CreateMap<Data.CollabFund.CollabFund, DTOs.UpdateCollabFundDTO>().ReverseMap();
            CreateMap<Data.CollabFund.CollabFund, DTOs.ChangeCollabFundActiveStateDTO>().ReverseMap();
            CreateMap<Data.CollabFund.CollabFund, DTOs.CollabFund_VM_DTO>()
                .ForMember(dest => dest.TotalAmountStr, opt => opt.MapFrom(src => LConvertVariable.ConvertToMoneyFormat(src.TotalAmount)))
                .ReverseMap();
            CreateMap<Data.CollabFund.CollabFund, DTOs.CollabFundDetail_VM_DTO>()
                .ForMember(dest => dest.TotalAmountStr, opt => opt.MapFrom(src => LConvertVariable.ConvertToMoneyFormat(src.TotalAmount)))
                .ForMember(dest => dest.CreateTimeStr, opt => opt.MapFrom(src => LConvertVariable.ConvertDateTimeToString(src.CreateTime)))
                .ReverseMap();

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

            // Custom Data
            // TransactionInDayCalendar with its properties are long number, convert to money format
            CreateMap<Data.Custom.TransactionInDayCalendar, Data.Custom.TransactionInDayCalendar>()
                // total amount = total amount in - total amount out
                .ForMember(dest => dest.TotalAmount, otp => otp.MapFrom(src => src.TotalAmountIn - src.TotalAmountOut))
                .ForMember(dest => dest.TotalAmountStr, opt => opt.MapFrom(src => LConvertVariable.ConvertToMoneyFormat(src.TotalAmount)))
                .ForMember(dest => dest.TotalAmountInStr, opt => opt.MapFrom(src => LConvertVariable.ConvertToMoneyFormat(src.TotalAmountIn)))
                .ForMember(dest => dest.TotalAmountOutStr, opt => opt.MapFrom(src => LConvertVariable.ConvertToMoneyFormat(src.TotalAmountOut)))
                .ReverseMap();

        }

        // function to convert long number to money format, ex: 1000000 -> 1.000.000

    }
}
