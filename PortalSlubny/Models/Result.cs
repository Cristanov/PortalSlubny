using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortalSlubny.Models
{
    public class Result
    {
        public Result()
        {
            IsSuccess = true;
        }

        public Result(string errorMessage)
        {
            IsSuccess = false;
            ErrorMessage = errorMessage;
        }
        public bool IsSuccess { get; set; }

        public string ErrorMessage { get; set; }
    }
}