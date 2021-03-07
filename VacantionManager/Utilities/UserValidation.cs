using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace VacantionManager.Utilities
{
    public static class UserValidation
    {
        public static bool passwordCheck(string password)
        {
            if (password.Length >= 8 && Regex.IsMatch(password, @"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
