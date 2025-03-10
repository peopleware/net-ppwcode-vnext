﻿// Copyright 2025 by PeopleWare n.v..
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using PPWCode.Vernacular.Exceptions.V;

namespace PPWCode.AspNetCore.Server.I.Exceptions;

/// <summary>
///     This subclass of <see cref="ProgrammingError" />
///     indicates a programming error in the code that is calling the exposed REST api.
///     This error typically means that the calling code is not following the contracts of
///     the REST api and must be fixed.
/// </summary>
public class ApiUsageError : SemanticException
{
    public ApiUsageError(string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
