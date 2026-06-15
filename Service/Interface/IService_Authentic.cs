using HRemployee.Entities;
using HRemployee.PayLoad.DTO;
using HRemployee.PayLoad.Request;
using HRemployee.PayLoad.Response;

namespace HRemployee.Service.Interface
{
    public interface IService_Authentic
    {
        Task<ResponseObject<DTO_Token>> Login(Request_Login request);

        Task<ResponseObject<DTO_Token>> RenewAccessToken(DTO_Token request);

        Task<ResponseBase> ChangePassword(Request_ChangePassword request, int userId);

        Task<ResponseObject<object>> DecodeJwtTokenAsync(string token);

        Task<ResponseBase> CreateAccount(Request_CreateUser request);

        Task<ResponseBase> LockAccount(int userId, bool isLocked);
        Task<ResponseBase> LockAccountByEmployeeCode(string employeeCode, bool isLocked);

        Task<ResponseBase> ResetPassword(int userId);
        Task<ResponseBase> ResetPasswordByEmployeeCode(string employeeCode);
    }
}