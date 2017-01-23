// -----------------------------------------------------------------------
// <copyright file="PartnerDomainException.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    /// <summary>
    /// The exception that is thrown when an error is encountered when protecting, or unprotecting, data.
    /// </summary>
    [Serializable]
    public class PartnerDomainException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerDomainException"/> class.
        /// </summary>
        public PartnerDomainException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerDomainException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public PartnerDomainException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerDomainException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public PartnerDomainException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerDomainException" /> class.
        /// </summary>
        /// <param name="errorCode">Code for the error that occurred.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public PartnerDomainException(ErrorCode errorCode, string message = default(string), Exception innerException = null) : base(message, innerException)
        {
            this.ErrorCode = errorCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerDomainException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected PartnerDomainException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// Gets the error code which specifies what happened in the business area that caused the error.
        /// </summary>
        public ErrorCode ErrorCode { get; private set; } = ErrorCode.ServerError;

        /// <summary>
        /// Gets a dictionary of the error details.
        /// </summary>
        public IDictionary<string, string> Details { get; private set; } = new Dictionary<string, string>();

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter" />
        /// </PermissionSet>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ErrorCode", this.ErrorCode);
            info.AddValue("Details", this.Details);

            base.GetObjectData(info, context);
        }
    }
}