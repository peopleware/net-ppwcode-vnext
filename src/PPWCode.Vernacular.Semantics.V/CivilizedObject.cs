// Copyright 2024 by PeopleWare n.v..
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using PPWCode.Vernacular.Exceptions.V;
using PPWCode.Vernacular.Semantics.V.Exceptions;

namespace PPWCode.Vernacular.Semantics.V
{
    public abstract class CivilizedObject : ICivilizedObject
    {
        /// <summary>
        ///     A call to <see cref="ICivilizedObject.WildExceptions" />
        ///     returns an <see cref="CompoundSemanticException.IsEmpty" />
        ///     exception.
        /// </summary>
        [JsonIgnore]
        public virtual bool IsCivilized
            => WildExceptions().IsEmpty;

        /// <summary>
        ///     Build a set of <see cref="CompoundSemanticException" /> instances
        ///     that tell what is wrong with this instance, with respect to
        ///     <em>being civilized</em>.
        /// </summary>
        /// <returns>
        ///     <para>
        ///         The result comes in the form of an <strong>unclosed</strong>
        ///         <see cref="CompoundSemanticException" />, of
        ///         which the set of element exceptions might be empty.
        ///     </para>
        ///     <para>This method should work in any state of the object.</para>
        ///     <para>
        ///         This method is public instead of
        ///         protected to make it more easy to describe to users what the business
        ///         rules for this type are.
        ///     </para>
        /// </returns>
        public virtual CompoundSemanticException WildExceptions()
        {
            CompoundSemanticException result = new ();
            ICollection<ValidationResult> validationResults =
                new List<ValidationResult>();
            if (Validator.TryValidateObject(this, new ValidationContext(this), validationResults, true))
            {
                return result;
            }

            foreach (ValidationResult validationResult in validationResults)
            {
                result.AddElement(new ValidationViolationException(validationResult));
            }

            return result;
        }

        /// <summary>
        ///     Call <see cref="ICivilizedObject.WildExceptions" />, and if the result
        ///     is not <see cref="CompoundSemanticException.IsEmpty" />,
        ///     close the exception and throw it.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This method has no effects. If it ends nominally,
        ///         and if it throws an exception, no state is changed.
        ///     </para>
        ///     <para>
        ///         It is not <c>[Pure]</c> however, since it changes
        ///         the state of the exception to
        ///         <see cref="CompoundSemanticException.Closed" />.
        ///     </para>
        /// </remarks>
        public virtual void ThrowIfNotCivilized()
        {
            CompoundSemanticException cse = WildExceptions();
            if (!cse.IsEmpty)
            {
                cse.Close();
                throw cse;
            }
        }
    }
}
