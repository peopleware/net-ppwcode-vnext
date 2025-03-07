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

using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace PPWCode.AspNetCore.Server.I.Exceptions;

[ExcludeFromCodeCoverage]
public sealed class OperationCancelledExceptionHandler
    : BaseExceptionHandler<OperationCancelledExceptionHandler, OperationCanceledException>
{
    /// <inheritdoc />
    protected override IActionResult? CreateActionResult(ExceptionContext context)
        => new ClientClosedResult();

    [DefaultStatusCode(DefaultStatusCode)]
    private class ClientClosedResult : StatusCodeResult
    {
        private const int DefaultStatusCode = 499;

        /// <summary>
        ///     Creates a new <see cref="ClientClosedResult" /> instance.
        /// </summary>
        public ClientClosedResult()
            : base(DefaultStatusCode)
        {
        }
    }
}
