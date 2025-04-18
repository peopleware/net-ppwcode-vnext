﻿// Copyright 2024 by PeopleWare n.v..
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace PPWCode.Vernacular.Exceptions.V
{
    /// <summary>
    ///     This error is thrown when an external condition occurs, which we know can happen (however unlikely, however
    ///     vague), which we do not want to deal with in our application.
    ///     <inheritdoc cref="Error" />
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Most often, these are exceptional conditions of a technical nature. These conditions are considered
    ///         (external) preconditions on the system level. Examples are a disk that is full, a network connection that
    ///         cannot be established, a power failure, etcetera. The indented audience of these errors is neither the end
    ///         user, nor the developer, but the <strong>administrator</strong>, who is responsible for system
    ///         configuration, integration and infrastructure.
    ///     </para>
    ///     <para>
    ///         The <see cref="Exception.Message" /> should describe the error as closely as possible. It is the only
    ///         channel through which to communicate to the administrator what went wrong. If you cannot pinpoint the exact
    ///         nature of the error, the message should say so explicitly. If you become aware of the external condition you
    ///         do not want to deal with through an <see cref="Exception" />, it should be carried by an instance of this
    ///         class as its <see cref="Exception.InnerException" />.
    ///     </para>
    ///     <para>
    ///         These errors should not be mentioned in the exception part of a method contract. They could be mentioned in
    ///         the preconditions of a method contract, but in general this has little benefit. <c>ExternalError</c>s are a
    ///         mechanism to signal <em>system precondition</em> violations to administrators, it is not a part of the
    ///         contract between developers, but rather a contract between developers in general and the system
    ///         administrator. These errors could be documented in a document that communicates between developers and
    ///         administrators (e.g., installation documentations), and this should be done in specific cases. But most
    ///         often, these system preconditions are considered implicit (e.g., when we need a database, it is implied that
    ///         the database connection works). Furthermore, any documentation you write, probably is never going to be
    ///         read, and most assuredly not when an <c>ExternalError</c> occurs. The <see cref="Exception.Message" />
    ///         is your primary channel of communication.
    ///     </para>
    ///     <para>
    ///         It probably does not make sense to create subtypes of this error for specific situations. There is no need
    ///         for internationalization for external errors. If there is extra information that we can communicate to the
    ///         administrator, we can add it to the message.
    ///     </para>
    ///     <para>
    ///         Some string class variables are provided, that can be used as <see cref="Exception.Message" /> if you wish.
    ///         Using the same strings might help keep the code in your project more consistent. The audience of these
    ///         strings are system administrators.
    ///     </para>
    /// </remarks>
    public class ExternalError : Error
    {
        /// <summary>
        ///     A string that can be used as the <see cref="Exception.Message" /> to signal that the external error was
        ///     detected through an exception.
        /// </summary>
        public const string ExceptionWithExternalCauseMessage = "An exception occurred, which appears to be of an external nature.";

        /// <summary>
        ///     A string that can be used as the <see cref="Exception.Message" /> to signal that we are not able to provide
        ///     more information as to the cause of the error.
        /// </summary>
        public const string UnspecifiedExternalErrorMessage = "Could not continue due to an unspecified external error.";

        public ExternalError(string? message = null, Exception? innerException = null)
            : base(message ?? (innerException == null ? UnspecifiedExternalErrorMessage : ExceptionWithExternalCauseMessage), innerException)
        {
        }
    }
}
