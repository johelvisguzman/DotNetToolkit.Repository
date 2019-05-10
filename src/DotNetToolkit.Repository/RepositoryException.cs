namespace DotNetToolkit.Repository
{
    using System;
    using System.Runtime.InteropServices;
    using Properties;
#if !NETSTANDARD1_3
    using System.Runtime.Serialization;
#endif

    /// <summary>
    /// Represents errors that occur during application execution.
    /// </summary>
    [ComVisible(true)]
#if !NETSTANDARD1_3
    [Serializable]
#endif
    public class RepositoryException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryException" /> class.
        /// </summary>
        public RepositoryException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryException" /> class with a specified error message.
        /// </summary>
        public RepositoryException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        public RepositoryException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryException" /> class with a reference to the inner exception that is the cause of this exception.
        /// </summary>
        public RepositoryException(Exception inner) : this(Resources.RepositoryExceptionMessage, inner) { }

#if !NETSTANDARD1_3
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryException" /> class with serialized data.
        /// </summary>
        protected RepositoryException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}
