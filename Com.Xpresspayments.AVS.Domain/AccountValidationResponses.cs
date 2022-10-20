using Com.Xpresspayments.AVS.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Xpresspayments.AVS.Common
{
    public  class AccountValidationResponses
    {
        public  Dictionary<string, GenericResponse> _respDictionary;

        public AccountValidationResponses()
        {
            _respDictionary = new Dictionary<string, GenericResponse> {
                { "00", new GenericResponse{
                  ResponseCode = "00",
                  ResponseMessage = "Account Validation Successful."
                }},

                { "01",new GenericResponse{
                  ResponseCode = "01",
                  ResponseMessage = "Invalid Account Number."
                }},

                { "03",new GenericResponse{
                  ResponseCode = "03",
                  ResponseMessage = "Invaid Bank Code."
                }},

                { "04",new GenericResponse{
                  ResponseCode = "04",
                  ResponseMessage = "Invalid Application ID."
                }},

                 { "05",new GenericResponse{
                  ResponseCode = "05",
                  ResponseMessage = "Account Not Found."
                }},

                { "10",new GenericResponse{
                  ResponseCode = "10",
                  ResponseMessage = "Processing Error."
                } },

                { "1001",new GenericResponse{
                  ResponseCode = "1001",
                  ResponseMessage = "Unauthorized Access."
                }},
            };
        }
       
    }
}
