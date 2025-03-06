namespace Roo.Azure.Configuration.Common.ServiceExceptions
{
    /// <summary>
    /// Custom error codes to simplify error handling related to errors specific to this package.<br/>
    /// 43x: Request Header Error
    /// </summary>
    public enum ErrorCode
    {
        /// <summary>
        /// No pre-defined error.
        /// </summary>
        None = 0,
        /// <summary>
        /// Request was different than what the receiver expected. Example: missing model, incorrect model, missing header, type incorrect, etc.
        /// </summary>
        BadRequest = 400,
        /// <summary>
        /// session-id not found in request headers.
        /// </summary>
        SessionIdHeaderNotFound = 432,
        /// <summary>
        /// transaction-id not found in request headers.
        /// </summary>
        TransactionIdHeaderNotFound = 433,
        /// <summary>
        /// channel-id not found in request headers.
        /// </summary>
        ChannelIdHeaderNotFound = 434,
        /// <summary>
        /// user-info not found in request headers.
        /// </summary>
        UserInfoHeaderNotFound = 435,
        /// <summary>
        /// Attempted to read or write to protected memory.
        /// </summary>
        AccessViolation = 520,
        /// <summary>
        /// Multiple exceptions occured.
        /// </summary>
        AggregateFailure,
        /// <summary>
        /// Attempted to access an unloaded application domain.
        /// </summary>
        AppDomainUnloaded,
        /// <summary>
        /// A null reference was passed to a method that does not accept a null argument.
        /// </summary>
        ArgumentNull,
        /// <summary>
        /// Value of an arugment was outside the allowable range of values defined by the invoked method.
        /// </summary>
        ArgumentOutOfRange,
        /// <summary>
        /// One of the arguments provided to a method was not valid.
        /// </summary>
        ArgumentInvalid,
        /// <summary>
        /// Error during an arithmetic, casting, or conversion operation.
        /// </summary>
        ArithmeticInvalid,
        /// <summary>
        /// Attempted to store an element of the wrong type within an array.
        /// </summary>
        ArrayTypeMismatch,
        /// <summary>
        /// Invalid file image of a dynamic link library (DLL) or executable program.
        /// </summary>
        BadImageFormat,
        /// <summary>
        /// Failure while trying to unload application domain.
        /// </summary>
        CannotUnloadAppDomain,
        /// <summary>
        /// Failure while attempting to marshal an object across a context boundary.
        /// </summary>
        ContextMashalFailure,
        /// <summary>
        /// Attempted to read or write to an address that is not a multiple of the data size.
        /// </summary>
        DataMisaligned,
        /// <summary>
        /// Unexpected number of rows are affected during a database save operation. This usually occurs if the database has been modified since it was loaded into memory.
        /// </summary>
        DbUpdateConcurrency,
        /// <summary>
        /// Failure while saving to the database.
        /// </summary>
        DbUpdateFailure,
        /// <summary>
        /// Unable to find a file or directory.
        /// </summary>
        DirectoryNotFound,
        /// <summary>
        /// Attempted to divide an integral or decimal value by zero.
        /// </summary>
        DivideByZero,
        /// <summary>
        /// Unable to find a specified DLL in a DLL import.
        /// </summary>
        DllNotFound,
        /// <summary>
        /// An object appeared more than once in an array of synchronization objects.
        /// </summary>
        DuplicateWaitObject,
        /// <summary>
        /// Attempted to read past the end of a stream.
        /// </summary>
        EndOfStream,
        /// <summary>
        /// Failed to load a class due to the absense of an entry method.
        /// </summary>
        EntryPointNotFound,
        /// <summary>
        /// Attempted to access an invalid private or protected field inside a class.
        /// </summary>
        FieldAccessInvalid,
        /// <summary>
        /// Attempted to access a file that does not exist.
        /// </summary>
        FileNotFound,
        /// <summary>
        /// Either invalid format of an argument or composite format string is not well formed.
        /// </summary>
        FormatInvalid,
        /// <summary>
        /// Failure when trying to read HTTP response.
        /// </summary>
        HttpIOFailure,
        /// <summary>
        /// Attempted to access an element of an array or collection with an index outside of its bounds.
        /// </summary>
        IndexOutOfRange,
        /// <summary>
        /// Insufficient execution stack available to allow most methods to execute.
        /// </summary>
        InsufficientExecutionStack,
        /// <summary>
        /// Insufficient memory available to execute.
        /// </summary>
        InsufficientMemory,
        /// <summary>
        /// Invalid casting or explicit conversion.
        /// </summary>
        InvalidCast,
        /// <summary>
        /// Invalid data stream format.
        /// </summary>
        InvalidData,
        /// <summary>
        /// Invalid method call for the objects current state.
        /// </summary>
        InvalidOperation,
        /// <summary>
        /// Invalid time zone information.
        /// </summary>
        InvalidTimeZone,
        /// <summary>
        /// Attempted to access an element in a collection with a key that does not match keys in it.
        /// </summary>
        KeyNotFound,
        /// <summary>
        /// Attempted a recursive entry into a lock that was not compatible with the recursion policy for the lock.
        /// </summary>
        LockRecursionFailure,
        /// <summary>
        /// Failed during an attempt to access a class member.
        /// </summary>
        MemberAcessInvalid,
        /// <summary>
        /// Invalid attempt to access a method, such as accessing a private method from partially trusted code.
        /// </summary>
        MethodAccessInvalid,
        /// <summary>
        /// Invalid attempt to dynamically access a field that does not exist.
        /// </summary>
        MissingField,
        /// <summary>
        /// Invalid attempt to dynamically access a class member that does not exist or that is not declared as public.
        /// </summary>
        MissingMember,
        /// <summary>
        /// Invalid attempt to dynamically access a method that does not exist.
        /// </summary>
        MissingMethod,
        /// <summary>
        /// A floating-point value is positive infinity, negative infinity, or Not-a-Number (NaN).
        /// </summary>
        NotFiniteNumber,
        /// <summary>
        /// Attempted to use a method or operation that is not implemented.
        /// </summary>
        NotImplemented,
        /// <summary>
        /// Either attempted to invoke an unsupported method or attempted to read, seek, or write to a stream that does not support the invoked functionality.
        /// </summary>
        NotSupported,
        /// <summary>
        /// Attempted to dereference a null object reference.
        /// </summary>
        NullReference,
        /// <summary>
        /// Operation was peformed on a disposed object.
        /// </summary>
        ObjectDisposed,
        /// <summary>
        /// An operation was cancelled while the thread was executing it.
        /// </summary>
        OperationCanceled,
        /// <summary>
        /// Not enough memory to continue the execution of the program.
        /// </summary>
        OutOfMemory,
        /// <summary>
        /// An overflow occured during an arithmetic, casting, or conversion operation. 
        /// </summary>
        OverflowFailure,
        /// <summary>
        /// Invalid path or fully qualified file  name is longer than the system-defined maximum length.
        /// </summary>
        PathTooLongFileName,
        /// <summary>
        /// Attempted to run a feature that is not supported by the servers platform.
        /// </summary>
        PlatformNotSupported,
        /// <summary>
        /// An array with the wrong number of dimensions was passed to a method.
        /// </summary>
        RankArray,
        /// <summary>
        /// The execution stack has exceeded the stack size.
        /// </summary>
        StackOverflow,
        /// <summary>
        /// The time allotted for a process or operation has expired.
        /// </summary>
        Timeout,
        /// <summary>
        /// A time zone was not found.
        /// </summary>
        TimeZoneNotFound,
        /// <summary>
        /// A method attempted to use a type that it does not have access to.
        /// </summary>
        TypeAccessInvalid,
        /// <summary>
        /// Error occurred during a class initialization.
        /// </summary>
        TypeInitializationFailure,
        /// <summary>
        /// Failure during type-loading.
        /// </summary>
        TypeLoadFailure,
        /// <summary>
        /// Attempted to access an unloaded class.
        /// </summary>
        TypeUnloaded,
        /// <summary>
        /// Operating system denied access because of an I/O error or a specific type of security error.<br/>
        /// Different from a 4XX unauthorized error, server level access has been denied.
        /// </summary>
        UnauthorizedAccess,
        /// <summary>
        /// Invalid Uniform Resource Identifier (URI) detected.
        /// </summary>
        UriFormatException,
        /// <summary>
        /// Failure occurred during the validation of a data fiend when the <see cref="ValidationAttribute"/> class is used.
        /// </summary>
        ValidationFailure
    }
}
