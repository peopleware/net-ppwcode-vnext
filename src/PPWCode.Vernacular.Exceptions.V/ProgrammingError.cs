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
    ///     This error is thrown when a programming condition occurs, which we know can happen
    ///     (however unlikely), which we do not want to deal with in our application.
    ///     <inheritdoc cref="Error" />
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <c>ProgrammingError</c> instances are used to signal programming errors,
    ///         when we become aware of them in the code. Examples are branches of if-statements
    ///         or switch-statement, or catch-branches, where from reasoning we assume execution
    ///         would never reach. Instead of merely writing a comment, throwing a
    ///         <c>ProgrammingError</c> is better.
    ///     </para>
    ///     <para>
    ///         The audience of <c>ProgrammingErrors</c> are developers. To help in debugging,
    ///         it makes sense to include a message that is as descriptive as possible.
    ///         If you become aware of the external condition you do not want to deal
    ///         with through an <see cref="Exception" />, it should be carried
    ///         by an instance of this class as its <see cref="Exception.InnerException" />.
    ///     </para>
    ///     <para>
    ///         Administrators should be aware of the errors too. They need to be aware
    ///         of the state of the application, and are probably on the path of communication
    ///         to the developers.
    ///     </para>
    /// </remarks>
    public class ProgrammingError : Error
    {
        protected const string ExceptionWithProgrammingCauseMessage = "An exception occured, which appears to be of a programming nature.";
        protected const string UnspecifiedProgrammingErrorMessage = "Could not continue due to an unspecified programming error.";

        public ProgrammingError(string? message = null, Exception? innerException = null)
            : base(message ?? (innerException == null ? UnspecifiedProgrammingErrorMessage : ExceptionWithProgrammingCauseMessage), innerException)
        {
        }
    }
}
