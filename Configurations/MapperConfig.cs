using AutoMapper;

namespace pbms_be.Configurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig ()
        {
            //CreateMap<Data.Auth.Account, DTOs.AccountDTO>().ReverseMap();
            // create map from Account to AccountDTO, convert CreateTime from unix timestamp to datetime
            //CreateMap<Data.Auth.Account, DTOs.AccountDTO>()
            //    .ForMember(dest => dest.CreateTime, opt => opt.MapFrom(src => ConvertFromUnixTimestamp(src.CreateTime)));
            //// create map from AccountDTO to Account, convert CreateTime from datetime to unix timestamp
            //CreateMap<DTOs.AccountDTO, Data.Auth.Account>()
            //    .ForMember(dest => dest.CreateTime, opt => opt.MapFrom(src => ConvertToUnixTimestamp(src.CreateTime)));
        }

        // convert datetime to unix timestamp
        public static long ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan difference = date.ToUniversalTime() - origin;
            return (long)Math.Floor(difference.TotalSeconds);
        }

        // convert unix timestamp to datetime
        public static DateTime ConvertFromUnixTimestamp(long timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp);
        }
    }
}
