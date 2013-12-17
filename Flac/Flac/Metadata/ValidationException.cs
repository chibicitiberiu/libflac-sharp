using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Flac.Metadata
{
    /// <summary>
    /// Metadata format validation exception
    /// </summary>
    public class ValidationException : Exception
    {
        /// <summary>
        /// Creates a new instance of bad header exception.
        /// </summary>
        public ValidationException()
            : base()
        {
        }

        /// <summary>
        /// Creates a new instance of bad header exception with specified message.
        /// </summary>
        /// <param name="message">The message</param>
        public ValidationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates a new instance of bad header exception with specified message 
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="innerException">The inner exception</param>
        public ValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
