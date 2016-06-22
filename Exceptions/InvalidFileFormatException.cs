﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brockhaus.PraktikumZeugnisGenerator.Exceptions
{
    class InvalidFileFormatException : System.Exception
    {
        public InvalidFileFormatException() : base() { }
        public InvalidFileFormatException(string message) : base(message) { }
        public InvalidFileFormatException(string message, System.Exception inner) : base(message, inner) { }

        protected InvalidFileFormatException(System.Runtime.Serialization.SerializationInfo info,
       System.Runtime.Serialization.StreamingContext context)
        { }

    }
}
