using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Roo.Azure.Configuration.Common.ServiceExceptions
{
    /// <summary>
    /// Converts between <see cref="Exception"/> and <see cref="ServiceException"/>
    /// </summary>
    public static class ServiceExceptionConverter
    {
        private static Dictionary<string, string> GetDictionary(Exception ex, Dictionary<string, string>? additional = null)
        {
            Dictionary<string, string> dictionary = new()
            {
                { "Type", ex.GetType().FullName ?? "" },
                { "BaseMessage", ex.GetBaseException().Message ?? "" },
                { "Source", ex.Source ?? "" },
                { "Method", ex.TargetSite?.Name ?? "" },
                { "StackTrace", ex.StackTrace ?? "" },
                { "HelpLink", ex.HelpLink ?? "" }
            };

            if (additional != null)
            {
                foreach (var item in additional)
                {
                    dictionary.Add(item.Key, item.Value);
                }
            }

            return dictionary;
        }

        /// <summary>
        /// Convert an <see cref="Exception"/> to a <see cref="ServiceException"/>.
        /// </summary>
        /// <param name="exception">Exception to convert.</param>
        /// <param name="transactionId">Transaction Id associated with the exception.</param>
        /// <returns>A ServiceException configured for the input exception.</returns>
        public static ServiceException ConvertTo(Exception exception, string? transactionId = null)
        {
            return exception switch
            {
                ServiceException ex => ex,
                AccessViolationException ex => new ServiceException(ErrorCode.AccessViolation, ex.Message, GetDictionary(ex), ex, transactionId),
                AggregateException ex => new ServiceException(ErrorCode.AggregateFailure, ex.Message, GetDictionary(ex, new()
                {
                    { "Exceptions", string.Join(", ", ex.InnerExceptions.Select(x => x.GetType().Name)) }
                }), ex, transactionId),
                AppDomainUnloadedException ex => new ServiceException(ErrorCode.AppDomainUnloaded, ex.Message, GetDictionary(ex), ex, transactionId),
                ArgumentNullException ex => new ServiceException(ErrorCode.ArgumentNull, ex.Message, GetDictionary(ex, new()
                {
                    { "ParamName", ex.ParamName ?? "" }
                }), ex, transactionId),
                ArgumentOutOfRangeException ex => new ServiceException(ErrorCode.ArgumentOutOfRange, ex.Message, GetDictionary(ex, new()
                {
                    { "ParamName", ex.ParamName ?? "" },
                    { "Value", ex.ActualValue?.ToString() ?? "" }
                }), ex, transactionId),
                DuplicateWaitObjectException ex => new ServiceException(ErrorCode.DuplicateWaitObject, ex.Message, GetDictionary(ex, new()
                {
                    { "ParamName", ex.ParamName ?? "" }
                }), ex, transactionId),
                ArgumentException ex => new ServiceException(ErrorCode.ArgumentInvalid, ex.Message, GetDictionary(ex, new()
                {
                    { "ParamName", ex.ParamName ?? "" }
                }), ex, transactionId),
                DivideByZeroException ex => new ServiceException(ErrorCode.DivideByZero, ex.Message, GetDictionary(ex), ex, transactionId),
                NotFiniteNumberException ex => new ServiceException(ErrorCode.NotFiniteNumber, ex.Message, GetDictionary(ex, new()
                {
                    { "OffendingNumber", ex.OffendingNumber.ToString() }
                }), ex, transactionId),
                OverflowException ex => new ServiceException(ErrorCode.OverflowFailure, ex.Message, GetDictionary(ex), ex, transactionId),
                ArithmeticException ex => new ServiceException(ErrorCode.ArithmeticInvalid, ex.Message, GetDictionary(ex), ex, transactionId),
                ArrayTypeMismatchException ex => new ServiceException(ErrorCode.ArrayTypeMismatch, ex.Message, GetDictionary(ex), ex, transactionId),
                BadImageFormatException ex => new ServiceException(ErrorCode.BadImageFormat, ex.Message, GetDictionary(ex, new()
                {
                    { "FileName", ex.FileName ?? "" },
                    { "FusionLog", ex.FusionLog ?? "" }
                }), ex, transactionId),
                CannotUnloadAppDomainException ex => new ServiceException(ErrorCode.CannotUnloadAppDomain, ex.Message, GetDictionary(ex), ex, transactionId),
                ContextMarshalException ex => new ServiceException(ErrorCode.ContextMashalFailure, ex.Message, GetDictionary(ex), ex, transactionId),
                DataMisalignedException ex => new ServiceException(ErrorCode.DataMisaligned, ex.Message, GetDictionary(ex), ex, transactionId),
                DbUpdateConcurrencyException ex => new ServiceException(ErrorCode.DbUpdateConcurrency, ex.Message, GetDictionary(ex), ex, transactionId),
                DbUpdateException ex => new ServiceException(ErrorCode.DbUpdateFailure, ex.Message, GetDictionary(ex), ex, transactionId),
                DirectoryNotFoundException ex => new ServiceException(ErrorCode.DirectoryNotFound, ex.Message, GetDictionary(ex), ex, transactionId),
                DllNotFoundException ex => new ServiceException(ErrorCode.DllNotFound, ex.Message, GetDictionary(ex, new()
                {
                    { "TypeName", ex.TypeName }
                }), ex, transactionId),
                EndOfStreamException ex => new ServiceException(ErrorCode.EndOfStream, ex.Message, GetDictionary(ex), ex, transactionId),
                EntryPointNotFoundException ex => new ServiceException(ErrorCode.EntryPointNotFound, ex.Message, GetDictionary(ex, new()
                {
                    { "TypeName", ex.TypeName }
                }), ex, transactionId),
                FieldAccessException ex => new ServiceException(ErrorCode.FieldAccessInvalid, ex.Message, GetDictionary(ex), ex, transactionId),
                FileNotFoundException ex => new ServiceException(ErrorCode.FileNotFound, ex.Message, GetDictionary(ex, new()
                {
                    { "FileName", ex.FileName ?? "" },
                    { "FusionLog", ex.FusionLog ?? "" }
                }), ex, transactionId),
                UriFormatException ex => new ServiceException(ErrorCode.UriFormatException, ex.Message, GetDictionary(ex), ex, transactionId),
                FormatException ex => new ServiceException(ErrorCode.FormatInvalid, ex.Message, GetDictionary(ex), ex, transactionId),
                HttpIOException ex => new ServiceException(ErrorCode.HttpIOFailure, ex.Message, GetDictionary(ex, new()
                {
                    { "HttpRequestError", ex.HttpRequestError.ToString() }
                }), ex, transactionId),
                IndexOutOfRangeException ex => new ServiceException(ErrorCode.IndexOutOfRange, ex.Message, GetDictionary(ex), ex, transactionId),
                InsufficientExecutionStackException ex => new ServiceException(ErrorCode.InsufficientExecutionStack, ex.Message, GetDictionary(ex), ex, transactionId),
                InsufficientMemoryException ex => new ServiceException(ErrorCode.InsufficientMemory, ex.Message, GetDictionary(ex), ex, transactionId),
                InvalidCastException ex => new ServiceException(ErrorCode.InvalidCast, ex.Message, GetDictionary(ex), ex, transactionId),
                InvalidDataException ex => new ServiceException(ErrorCode.InvalidData, ex.Message, GetDictionary(ex), ex, transactionId),
                ObjectDisposedException ex => new ServiceException(ErrorCode.ObjectDisposed, ex.Message, GetDictionary(ex, new()
                {
                    { "ObjectName", ex.ObjectName }
                }), ex, transactionId),
                InvalidOperationException ex => new ServiceException(ErrorCode.InvalidOperation, ex.Message, GetDictionary(ex), ex, transactionId),
                InvalidTimeZoneException ex => new ServiceException(ErrorCode.InvalidTimeZone, ex.Message, GetDictionary(ex), ex, transactionId),
                KeyNotFoundException ex => new ServiceException(ErrorCode.KeyNotFound, ex.Message, GetDictionary(ex), ex, transactionId),
                LockRecursionException ex => new ServiceException(ErrorCode.LockRecursionFailure, ex.Message, GetDictionary(ex), ex, transactionId),
                MissingMethodException ex => new ServiceException(ErrorCode.MissingMethod, ex.Message, GetDictionary(ex), ex, transactionId),
                MethodAccessException ex => new ServiceException(ErrorCode.MethodAccessInvalid, ex.Message, GetDictionary(ex), ex, transactionId),
                MissingFieldException ex => new ServiceException(ErrorCode.MissingField, ex.Message, GetDictionary(ex), ex, transactionId),
                MissingMemberException ex => new ServiceException(ErrorCode.MissingMember, ex.Message, GetDictionary(ex), ex, transactionId),
                MemberAccessException ex => new ServiceException(ErrorCode.MemberAcessInvalid, ex.Message, GetDictionary(ex), ex, transactionId),
                NotImplementedException ex => new ServiceException(ErrorCode.NotImplemented, ex.Message, GetDictionary(ex), ex, transactionId),
                PlatformNotSupportedException ex => new ServiceException(ErrorCode.PlatformNotSupported, ex.Message, GetDictionary(ex), ex, transactionId),
                NotSupportedException ex => new ServiceException(ErrorCode.NotSupported, ex.Message, GetDictionary(ex), ex, transactionId),
                NullReferenceException ex => new ServiceException(ErrorCode.NullReference, ex.Message, GetDictionary(ex), ex, transactionId),
                OperationCanceledException ex => new ServiceException(ErrorCode.OperationCanceled, ex.Message, GetDictionary(ex), ex, transactionId),
                OutOfMemoryException ex => new ServiceException(ErrorCode.OutOfMemory, ex.Message, GetDictionary(ex), ex, transactionId),
                PathTooLongException ex => new ServiceException(ErrorCode.PathTooLongFileName, ex.Message, GetDictionary(ex), ex, transactionId),
                RankException ex => new ServiceException(ErrorCode.RankArray, ex.Message, GetDictionary(ex), ex, transactionId),
                StackOverflowException ex => new ServiceException(ErrorCode.StackOverflow, ex.Message, GetDictionary(ex), ex, transactionId),
                TimeoutException ex => new ServiceException(ErrorCode.Timeout, ex.Message, GetDictionary(ex), ex, transactionId),
                TimeZoneNotFoundException ex => new ServiceException(ErrorCode.TimeZoneNotFound, ex.Message, GetDictionary(ex), ex, transactionId),
                TypeAccessException ex => new ServiceException(ErrorCode.TypeAccessInvalid, ex.Message, GetDictionary(ex, new()
                {
                    { "TypeName", ex.TypeName }
                }), ex, transactionId),
                TypeInitializationException ex => new ServiceException(ErrorCode.TypeInitializationFailure, ex.Message, GetDictionary(ex, new()
                {
                    { "TypeName", ex.TypeName }
                }), ex, transactionId),
                TypeLoadException ex => new ServiceException(ErrorCode.TypeLoadFailure, ex.Message, GetDictionary(ex, new()
                {
                    { "TypeName", ex.TypeName }
                }), ex, transactionId),
                TypeUnloadedException ex => new ServiceException(ErrorCode.TypeUnloaded, ex.Message, GetDictionary(ex), ex, transactionId),
                UnauthorizedAccessException ex => new ServiceException(ErrorCode.UnauthorizedAccess, ex.Message, GetDictionary(ex), ex, transactionId),
                ValidationException ex => new ServiceException(ErrorCode.ValidationFailure, ex.Message, GetDictionary(ex, new()
                {
                    { "Value", ex.Value?.ToString() ?? "" },
                    { "AttributeErrorMessage", ex.ValidationAttribute?.ErrorMessage ?? "" },
                    { "AttributeErrorMessage", ex.ValidationAttribute?.ErrorMessageResourceName ?? "" },
                    { "ResultMemberNames", string.Join(", ", ex.ValidationResult.MemberNames) },
                }), ex, transactionId),
                var ex => new ServiceException(ErrorCode.None, ex.Message, GetDictionary(ex), ex, transactionId)
            };
        }

        /// <summary>
        /// Convert a <see cref="ServiceException"/> to an <see cref="Exception"/>.
        /// </summary>
        /// <param name="ex">ServiceException to convert.</param>
        /// <returns>An exception created from the input ServiceException.</returns>
        public static Exception ConvertFrom(ServiceException ex)
        {
            return ex.Error.Code switch
            {
                ErrorCode.AccessViolation => new AccessViolationException(ex.Error.Message, ex.InnerException),
                ErrorCode.AggregateFailure => new AggregateException(ex.Error.Message, ex.InnerException ?? new Exception()),
                ErrorCode.AppDomainUnloaded => new AppDomainUnloadedException(ex.Error.Message, ex.InnerException),
                ErrorCode.ArgumentNull => ex.Error.Details != null && ex.Error.Details.TryGetValue("ParamName", out string? paramName) ? new ArgumentNullException(paramName, ex.Error.Message) : new ArgumentNullException(ex.Error.Message, ex.InnerException),
                ErrorCode.ArgumentOutOfRange => ex.Error.Details != null && ex.Error.Details.TryGetValue("ParamName", out string? paramName) ? ex.Error.Details.TryGetValue("Value", out string? value) ? new ArgumentOutOfRangeException(paramName, value, ex.Error.Message) : new ArgumentOutOfRangeException(paramName, ex.Error.Message) : new ArgumentOutOfRangeException(ex.Message, ex.InnerException),
                ErrorCode.DuplicateWaitObject => ex.Error.Details != null && ex.Error.Details.TryGetValue("ParamName", out string? paramName) ? new DuplicateWaitObjectException(paramName, ex.Message) : new DuplicateWaitObjectException(ex.Message, ex.InnerException),
                ErrorCode.ArgumentInvalid => ex.Error.Details != null && ex.Error.Details.TryGetValue("ParamName", out string? paramName) ? new ArgumentException(ex.Message, paramName, ex.InnerException) : new ArgumentException(ex.Message, ex.InnerException),
                ErrorCode.DivideByZero => new DivideByZeroException(ex.Message, ex.InnerException),
                ErrorCode.NotFiniteNumber => ex.Error.Details != null && ex.Error.Details.TryGetValue("OffendingNumber", out string? offendingNumber) ? new NotFiniteNumberException(ex.Message, Convert.ToDouble(offendingNumber), ex.InnerException) : new NotFiniteNumberException(ex.Message, ex.InnerException),
                ErrorCode.OverflowFailure => new OverflowException(ex.Message, ex.InnerException),
                ErrorCode.ArithmeticInvalid => new ArithmeticException(ex.Message, ex.InnerException),
                ErrorCode.ArrayTypeMismatch => new ArrayTypeMismatchException(ex.Message, ex.InnerException),
                ErrorCode.BadImageFormat => ex.Error.Details != null && ex.Error.Details.TryGetValue("FileName", out string? fileName) ? new BadImageFormatException(ex.Message, fileName, ex.InnerException) : new BadImageFormatException(ex.Message, ex.InnerException),
                ErrorCode.CannotUnloadAppDomain => new CannotUnloadAppDomainException(ex.Message, ex.InnerException),
                ErrorCode.ContextMashalFailure => new ContextMarshalException(ex.Message, ex.InnerException),
                ErrorCode.DataMisaligned => new DataMisalignedException(ex.Message, ex.InnerException),
                ErrorCode.DbUpdateConcurrency => new DbUpdateConcurrencyException(ex.Message, ex.InnerException),
                ErrorCode.DbUpdateFailure => new DbUpdateException(ex.Message, ex.InnerException),
                ErrorCode.DirectoryNotFound => new DirectoryNotFoundException(ex.Message, ex.InnerException),
                ErrorCode.DllNotFound => new DllNotFoundException(ex.Message, ex.InnerException),
                ErrorCode.EndOfStream => new EndOfStreamException(ex.Message, ex.InnerException),
                ErrorCode.EntryPointNotFound => new EntryPointNotFoundException(ex.Message, ex.InnerException),
                ErrorCode.FieldAccessInvalid => new FieldAccessException(ex.Message, ex.InnerException),
                ErrorCode.FileNotFound => ex.Error.Details != null && ex.Error.Details.TryGetValue("FileName", out string? fileName) ? new FileNotFoundException(ex.Message, fileName, ex.InnerException) : new FileNotFoundException(ex.Message, ex.InnerException),
                ErrorCode.UriFormatException => new UriFormatException(ex.Message, ex.InnerException),
                ErrorCode.FormatInvalid => new FormatException(ex.Message, ex.InnerException),
                ErrorCode.HttpIOFailure => ex.Error.Details != null && ex.Error.Details.TryGetValue("HttpRequestError", out string? httpRequestError) ? new HttpIOException((HttpRequestError)Convert.ToInt32(httpRequestError), ex.Message, ex.InnerException) : new IOException(ex.Message, ex.InnerException),
                ErrorCode.IndexOutOfRange => new IndexOutOfRangeException(ex.Message, ex.InnerException),
                ErrorCode.InsufficientExecutionStack => new InsufficientExecutionStackException(ex.Message, ex.InnerException),
                ErrorCode.InsufficientMemory => new InsufficientMemoryException(ex.Message, ex.InnerException),
                ErrorCode.InvalidCast => new InvalidCastException(ex.Message, ex.InnerException),
                ErrorCode.InvalidData => new InvalidDataException(ex.Message, ex.InnerException),
                ErrorCode.ObjectDisposed => ex.Error.Details != null && ex.Error.Details.TryGetValue("ObjectName", out string? objectName) ? new ObjectDisposedException(objectName, ex.Message) : new ObjectDisposedException(ex.Message, ex.InnerException),
                ErrorCode.InvalidOperation => new InvalidOperationException(ex.Message, ex.InnerException),
                ErrorCode.InvalidTimeZone => new InvalidTimeZoneException(ex.Message, ex.InnerException),
                ErrorCode.KeyNotFound => new KeyNotFoundException(ex.Message, ex.InnerException),
                ErrorCode.LockRecursionFailure => new LockRecursionException(ex.Message, ex.InnerException),
                ErrorCode.MissingMethod => new MissingMethodException(ex.Message, ex.InnerException),
                ErrorCode.MethodAccessInvalid => new MethodAccessException(ex.Message, ex.InnerException),
                ErrorCode.MissingField => new MissingFieldException(ex.Message, ex.InnerException),
                ErrorCode.MissingMember => new MissingMemberException(ex.Message, ex.InnerException),
                ErrorCode.MemberAcessInvalid => new MemberAccessException(ex.Message, ex.InnerException),
                ErrorCode.NotImplemented => new NotImplementedException(ex.Message, ex.InnerException),
                ErrorCode.PlatformNotSupported => new PlatformNotSupportedException(ex.Message, ex.InnerException),
                ErrorCode.NotSupported => new NotSupportedException(ex.Message, ex.InnerException),
                ErrorCode.NullReference => new NullReferenceException(ex.Message, ex.InnerException),
                ErrorCode.OperationCanceled => new OperationCanceledException(ex.Message, ex.InnerException),
                ErrorCode.OutOfMemory => new OutOfMemoryException(ex.Message, ex.InnerException),
                ErrorCode.PathTooLongFileName => new PathTooLongException(ex.Message, ex.InnerException),
                ErrorCode.RankArray => new RankException(ex.Message, ex.InnerException),
                ErrorCode.StackOverflow => new StackOverflowException(ex.Message, ex.InnerException),
                ErrorCode.Timeout => new TimeoutException(ex.Message, ex.InnerException),
                ErrorCode.TimeZoneNotFound => new TimeZoneNotFoundException(ex.Message, ex.InnerException),
                ErrorCode.TypeAccessInvalid => new TypeAccessException(ex.Message, ex.InnerException),
                ErrorCode.TypeInitializationFailure => ex.Error.Details != null && ex.Error.Details.TryGetValue("TypeName", out string? typeName) ? new TypeInitializationException(typeName, ex.InnerException) : new TypeInitializationException(null, ex.InnerException),
                ErrorCode.TypeLoadFailure => new TypeLoadException(ex.Message, ex.InnerException),
                ErrorCode.TypeUnloaded => new TypeUnloadedException(ex.Message, ex.InnerException),
                ErrorCode.UnauthorizedAccess => new UnauthorizedAccessException(ex.Message, ex.InnerException),
                ErrorCode.ValidationFailure => ex.Error.Details != null && ex.Error.Details.TryGetValue("Value", out string? value) && ex.Error.Details.TryGetValue("ResultMemberNames", out string? resultMemberNames) ?
                    new ValidationException(new ValidationResult(ex.Message, resultMemberNames.Split(", ").ToList()), null, value) : ex.Error.Details != null && ex.Error.Details.TryGetValue("Value", out string? valueNoMember) ? new ValidationException(ex.Message, null, valueNoMember) : new ValidationException(ex.Message, ex.InnerException),
                ErrorCode.None => new Exception(ex.Message, ex.InnerException)
            };
        }
    }
}
