using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Linq;

namespace Paidy
{
    class Program
    {
        static bool CheckPhoneNumber(string phoneNumber)
        {
            int phoneNumberLength = phoneNumber.Where(char.IsDigit).ToArray().Length;
            string phonePattern = @"^\+?(\d[\d- ]+)?(\([\d- ]+\))?[\d- ]+\d$";

            if (phoneNumberLength >= 9 && phoneNumberLength <= 13 && Regex.Match(phoneNumber, phonePattern).Success)
            {
                return true;
            }
            return false;
        }

        static string GetNumberSuffix(int number)
        {
            // modulo 10 is used to get the last number to determine it's suffix
            var m10 = number % 10;
            // modulo 100 is used for the special rule of (11th, 12th and 13th)
            var m100 = number % 100;

            if (m10 == 1 && m100 != 11)
            {
                return number + "st";
            }
            if (m10 == 2 && m100 != 12)
            {
                return number + "nd";
            }
            if (m10 == 3 && m100 != 13)
            {
                return number + "rd";
            }

            return number + "th";
        }

        static int GetNumberOfSundays(DateTime startDate, DateTime endDate)
        {
            const int coutDayOffset = 1;
            const int weekDaysOffset = 6;
            const int weekDaysCount = 7; 

            // Rounding is automatic by using an int
            int sundays = (endDate.Subtract(startDate).Days + coutDayOffset +  // Calculates days betweend the 2 days and adds 1 as an offest for the count
                (weekDaysOffset + (int)startDate.DayOfWeek) % weekDaysCount) / weekDaysCount; // Gets the offset from the modulo base on the day of the week to calculate extra Sunday

            return sundays;
        }

        static string GetObfuscateTelOrEmail(string info)
        {
            // Checking to see if it's a telephone number and obfuscates
            if (CheckPhoneNumber(info))
            {
                char[] telephoneCharArr = info.ToCharArray();
                int lastDigits = 4;

                for (int i = telephoneCharArr.Length -1; i > 0; i--)
                {
                    if (char.IsDigit(telephoneCharArr[i]))
                    {
                        lastDigits--;

                        if (lastDigits < 0)
                        {
                            telephoneCharArr[i] = '*';
                        }
                    }
                    else if(telephoneCharArr[i] == ' ')
                    {
                        telephoneCharArr[i] = '-';
                    }
                }
                var result = string.Join("", telephoneCharArr);
                return result;
            }
            // Checks to see if it's an email and obfuscates
            else if(new MailAddress(info).Address == info)
            {
                info = info.ToLower();

                char firstChar = info[0];
                string obfuscationText = "*****";
                int indexOfAt = info.IndexOf('@');
                string domain = info.Substring(indexOfAt - 1);

                string obfuscatedEmail = firstChar + obfuscationText + domain;

                return obfuscatedEmail;
            }
            // throws an error for invalid input
            else
            {
                return "Invalid input, please try again using an valid email or telephone number.";
            }
        }

        static void Main(string[] args)
        {
            try
            {
                int functionCode = 0; // the functionality option
                string status = "y"; // Using status to avoing the need to run the program again for each function
              
                while (status == "y")
                {
                    switch (functionCode)
                    {
                        case -1:
                            Console.WriteLine("Do you want to run the program again? 'y' for yes and any other key for no.");
                            status = Console.ReadLine();
                            functionCode = 0;
                            break;

                        case 0:
                            Console.WriteLine("Please select your functionality: " +
                                "\n 1. Number Suffix" +
                                "\n 2. Number of Sundays in a date range" +
                                "\n 3. Obfuscation of either email or telephone" +
                                "\n(type: '1, 2 or 3' in console)");
                            int.TryParse(Console.ReadLine(), out functionCode);
                            break;

                        case 1:
                            int number;
                            Console.WriteLine("Please enter your number (intiger)");
                            int.TryParse(Console.ReadLine(), out number);
                            Console.WriteLine(GetNumberSuffix(number));
                            functionCode = -1;
                            break;

                        case 2:
                            Console.WriteLine("Please enter the start date:");
                            DateTime startDate = DateTime.ParseExact(Console.ReadLine(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                            Console.WriteLine("And the end date:");
                            DateTime endDate = DateTime.ParseExact(Console.ReadLine(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                            Console.WriteLine(GetNumberOfSundays(startDate, endDate));
                            functionCode = -1;
                            break;

                        case 3:
                            Console.WriteLine("Please enter telephone or email to obfuscate.");
                            Console.WriteLine(GetObfuscateTelOrEmail(Console.ReadLine()));
                            functionCode = -1;
                            break;

                        default:
                            Console.WriteLine("Invalid input, try again using a number ranged 1-3");
                            functionCode = 0;
                            break;
                    }
                }
                
            }
            catch (Exception)
            {
                throw;
            }
          
        }
    }
}
