using ClothingStore.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothingStore.Application.IServices
{
    public interface IAuthenticationService
    {
        AuthenticationUserModel? ValidateCredentials(AuthenticationRequestBody authenticationRequestBody);
    }
}
