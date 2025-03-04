using StreamingServerApi.Controllers.model;
using System.Collections.Generic;

namespace StreamingServerApi.Controllers
{
    public class Service
    {
        public int Add(int number)
        {
            return number + number;
        }

        public int Multiply(int number)
        {
            return number * number;
        }

        public int Subtract(int number)
        {
            return number - number;
        }

        public int Divide(int number)
        {
            return number / number;
        }

        public int Modulus(int number)
        {
            return number % number;
        }
    }
}
