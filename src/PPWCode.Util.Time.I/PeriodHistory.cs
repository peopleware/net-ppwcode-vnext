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

using PPWCode.Vernacular.Exceptions.V;

namespace PPWCode.Util.Time.I
{
    /// <summary>
    ///     This is a generic helper class to work with <see cref="IPeriod{TPeriod}" /> instances.
    ///     It represents a timeline consisting of multiple non-overlapping periods.
    /// </summary>
    /// <typeparam name="TPeriod">a specific type that implements <see cref="IPeriod{TPeriod}" /> </typeparam>
    /// <typeparam name="T">a specific type of period</typeparam>
    public abstract class PeriodHistory<TPeriod, T>
        where TPeriod : class, IPeriod<T>
        where T : struct, IComparable<T>, IEquatable<T>
    {
        /// <summary>
        ///     Create a history.
        /// </summary>
        /// <param name="periods">given set of periods</param>
        /// <exception cref="InternalProgrammingError">
        ///     This exception is thrown when the period history is not correctly initialized.
        ///     The given <paramref name="periods" /> should not overlap.
        /// </exception>
        protected PeriodHistory(params TPeriod[] periods)
            : this(periods, true)
        {
        }

        /// <summary>
        ///     Create a history.
        /// </summary>
        /// <param name="periods">given set of periods</param>
        /// <exception cref="InternalProgrammingError">
        ///     This exception is thrown when the period history is not correctly initialized.
        ///     The given <paramref name="periods" /> should not overlap.
        /// </exception>
        protected PeriodHistory(IEnumerable<TPeriod> periods)
            : this(periods, true)
        {
        }

        /// <summary>
        ///     Create a history.
        /// </summary>
        /// <param name="periods">given set of periods</param>
        /// <param name="checkCivilized">check if all periods are civilized</param>
        /// <exception cref="InternalProgrammingError">
        ///     This exception is thrown when the period history is not correctly initialized.
        ///     The given <paramref name="periods" /> should not overlap.
        /// </exception>
        protected PeriodHistory(IEnumerable<TPeriod> periods, bool checkCivilized)
        {
            // wrap given intervals
            Periods =
                periods
                    .OrderBy(i => i.CoalesceFrom)
                    .ToList();
            LinkedPeriods = new LinkedList<TPeriod>(Periods);

            // input periods must be civilized
            if (checkCivilized && Periods.Any(e => !e.IsCivilized))
            {
                throw new InternalProgrammingError("Invalid history: all events must be civilized!");
            }

            // no intersections!
            LinkedListNode<TPeriod>? node = LinkedPeriods.First;
            while (node != null)
            {
                // current-from must be greater or equal than previous-to
                if ((node.Previous != null) && (node.Value.CoalesceFrom.CompareTo(node.Previous.Value.CoalesceTo) < 0))
                {
                    throw new InternalProgrammingError("Invalid history: must use non-overlapping intervals.");
                }

                node = node.Next;
            }

            // interval IPeriod{TPeriod}
            // already ordered, and were not overlapping
            PeriodDateTimes =
                Periods
                    .SelectMany(p => new[] { p.From, p.To })
                    .Distinct()
                    .ToList();
        }

        /// <summary>
        ///     Return the periods in the history as an ordered list.
        /// </summary>
        public IList<TPeriod> Periods { get; }

        /// <summary>
        ///     Return the periods in the history as an ordered, linked list.
        /// </summary>
        public LinkedList<TPeriod> LinkedPeriods { get; }

        /// <summary>
        ///     Returns an ordered list of all <see cref="DateTime" />s used as either <see cref="IPeriod{TPeriod}.From" />
        ///     or <see cref="IPeriod{TPeriod}.To" /> values in the history.
        /// </summary>
        /// <remarks>
        ///     If the first <see cref="DateTime" /> is <c>null</c>, then this corresponds
        ///     to minus infinity. If the last <see cref="DateTime" /> is <c>null</c>, then
        ///     this corresponds to plus infinity.
        /// </remarks>
        public IList<T?> PeriodDateTimes { get; }

        public T? OldestFromDate
            => LinkedPeriods.First?.Value.From;

        protected abstract TPeriod Create(T? from, T? to);

        /// <summary>
        ///     Returns the period in the history that contains the
        ///     given <paramref name="date" />.
        /// </summary>
        /// <param name="date">the given date</param>
        /// <returns>
        ///     a period of type <typeparamref name="TPeriod" /> if it contains the
        ///     given <paramref name="date" />, or <c>null</c> if no such period can be found
        /// </returns>
        public TPeriod? GetPeriodAt(T date)
        {
            LinkedListNode<TPeriod>? node = LinkedPeriods.First;

            while ((node != null) && (node.Value.CoalesceFrom.CompareTo(date) <= 0))
            {
                if (date.CompareTo(node.Value.CoalesceTo) < 0)
                {
                    return node.Value;
                }

                node = node.Next;
            }

            // no interval found
            return null;
        }

        /// <summary>
        ///     Returns the periods in the history that overlap with the period
        ///     defined by the given <paramref name="startDate" /> and
        ///     <paramref name="endDate" />.
        /// </summary>
        /// <param name="startDate">the given start date</param>
        /// <param name="endDate">the given end date</param>
        /// <returns>
        ///     An ordered list of periods of type <typeparamref name="TPeriod" /> each one
        ///     overlapping with the period defined by <paramref name="startDate" />
        ///     and <paramref name="endDate" />. If there are no overlapping periods
        ///     found, an empty list is returned.
        /// </returns>
        public IList<TPeriod> GetPeriodsOverlappingAt(T startDate, T endDate)
        {
            List<TPeriod> result = new ();
            if (endDate.CompareTo(startDate) <= 0)
            {
                return result;
            }

            LinkedListNode<TPeriod>? node = LinkedPeriods.First;
            while (node != null)
            {
                if ((startDate.CompareTo(node.Value.CoalesceTo) < 0)
                    && (node.Value.CoalesceFrom.CompareTo(endDate) < 0))
                {
                    result.Add(node.Value);
                }

                node = node.Next;
            }

            return result;
        }

        public IList<TPeriod> GetPeriodsOverlappingAt(IPeriod<T> period)
            => GetPeriodsOverlappingAt(period.CoalesceFrom, period.CoalesceTo);

        /// <summary>
        ///     Returns the period in the history that contains the
        ///     given <paramref name="date" />. If there is no such
        ///     period, then use the first period after <paramref name="date" />.
        ///     If there is also no such period, then null will be returned.
        /// </summary>
        /// <param name="date">the given date</param>
        /// <returns>
        ///     a period of type <typeparamref name="TPeriod" /> if a period is
        ///     found, or <c>null</c> if no such period can be found
        /// </returns>
        public TPeriod? GetPeriodAtOrImmediatelyAfter(T date)
        {
            LinkedListNode<TPeriod>? node = LinkedPeriods.First;

            while ((node != null) && (node.Value.CoalesceFrom.CompareTo(date) <= 0))
            {
                if (date.CompareTo(node.Value.CoalesceTo) < 0)
                {
                    return node.Value;
                }

                node = node.Next;
            }

            // no matching interval found, but if the node is not null, then
            // it is the first interval after an empty interval
            return node?.Value;
        }

        /// <summary>
        ///     Returns the period in the history that contains the
        ///     given <paramref name="date" />. If there is no such
        ///     period, then use the last period after <paramref name="date" />.
        ///     If there is also no such period, then null will be returned.
        /// </summary>
        /// <param name="date">the given date</param>
        /// <returns>
        ///     A period of type <typeparamref name="TPeriod" /> if a period is
        ///     found, or <c>null</c> if no such period can be found
        /// </returns>
        public TPeriod? GetPeriodAtOrImmediatelyBefore(T date)
        {
            LinkedListNode<TPeriod>? node = LinkedPeriods.First;
            LinkedListNode<TPeriod>? previousNode = null;

            while ((node != null) && (node.Value.CoalesceFrom.CompareTo(date) <= 0))
            {
                if (date.CompareTo(node.Value.CoalesceTo) < 0)
                {
                    return node.Value;
                }

                previousNode = node;
                node = node.Next;
            }

            // no matching interval found, but previousNode is the
            // last interval before an empty interval
            return previousNode?.Value;
        }

        /// <summary>
        ///     Returns the period in the history that contains the
        ///     given <paramref name="date" />. If there is no such
        ///     period, then use the first period after <paramref name="date" />.
        ///     If there also is no such period, then use the last period
        ///     before <paramref name="date" />. If there is also no such
        ///     period, this is an empty history, and null will be
        ///     returned.
        /// </summary>
        /// <param name="date">the given date</param>
        /// <returns>
        ///     a period of type <typeparamref name="TPeriod" /> if a period is
        ///     found, or <c>null</c> if no such period can be found
        /// </returns>
        public TPeriod? GetPeriodAtOrImmediatelyAfterOrImmediatelyBefore(T date)
        {
            LinkedListNode<TPeriod>? node = LinkedPeriods.First;
            LinkedListNode<TPeriod>? previousNode = null;

            while ((node != null) && (node.Value.CoalesceFrom.CompareTo(date) <= 0))
            {
                if (date.CompareTo(node.Value.CoalesceTo) < 0)
                {
                    return node.Value;
                }

                previousNode = node;
                node = node.Next;
            }

            // no matching interval found, but if the node is not null, then
            // it is the first interval after an empty interval, and if
            // previousNode is not null, it is the last interval before an
            // empty interval
            return node?.Value ?? previousNode?.Value;
        }

        /// <summary>
        ///     Returns the period in the history that contains the
        ///     given <paramref name="date" />. If there also is no such period, then use the last period
        ///     before <paramref name="date" />. If there is no such period, then use the first period after
        ///     <paramref name="date" />.
        ///     If there is also no such period, this is an empty history. A null will be returned.
        /// </summary>
        /// <param name="date">the given date</param>
        /// <returns>
        ///     A period of type <typeparamref name="TPeriod" /> if a period is
        ///     found, or <c>null</c> if no such period can be found
        /// </returns>
        public TPeriod? GetPeriodAtOrImmediatelyBeforeOrImmediatelyAfter(T date)
        {
            LinkedListNode<TPeriod>? node = LinkedPeriods.First;
            LinkedListNode<TPeriod>? previousNode = null;

            while ((node != null) && (node.Value.CoalesceFrom.CompareTo(date) <= 0))
            {
                if (date.CompareTo(node.Value.CoalesceTo) < 0)
                {
                    return node.Value;
                }

                previousNode = node;
                node = node.Next;
            }

            // no matching interval found, but if the node is not null, then
            // it is the first interval after an empty interval, and if
            // previousNode is not null, it is the last interval before an
            // empty interval
            return previousNode?.Value ?? node?.Value;
        }

        public bool HasPeriodAt(T date)
            => GetPeriodAt(date) != null;

        public IEnumerable<TPeriod> IntersectWith<T2>(PeriodHistory<T2, T>? other)
            where T2 : class, IPeriod<T>
        {
            if ((other == null) || (other.Periods.Count == 0))
            {
                return [];
            }

            List<TPeriod> result = new ();

            LinkedList<TPeriod> ours = LinkedPeriods;
            LinkedListNode<TPeriod>? oursCurrent = ours.First;

            LinkedList<T2> others = other.LinkedPeriods;
            LinkedListNode<T2>? othersCurrent = others.First;

            while ((oursCurrent != null) && (othersCurrent != null))
            {
                // if oursCurrent < othersCurrent
                if (oursCurrent.Value.CoalesceFrom.CompareTo(othersCurrent.Value.CoalesceFrom) < 0)
                {
                    if (!oursCurrent.Value.Overlaps(othersCurrent.Value))
                    {
                        // oursCurrent lies completely before othersCurrent
                        // => nothing to do, skip to next
                        oursCurrent = oursCurrent.Next;
                    }
                    else
                    {
                        // intersection, starting from othersCurrent.From
                        // => determine the To
                        T? from = othersCurrent.Value.From;
                        T? to;
                        if (oursCurrent.Value.CoalesceTo.CompareTo(othersCurrent.Value.CoalesceTo) < 0)
                        {
                            to = oursCurrent.Value.To;
                            oursCurrent = oursCurrent.Next;
                        }
                        else
                        {
                            to = othersCurrent.Value.To;
                            othersCurrent = othersCurrent.Next;
                        }

                        result.Add(Create(from, to));
                    }
                }
                else
                {
                    if (!othersCurrent.Value.Overlaps(oursCurrent.Value))
                    {
                        // othersCurrent lies completely before oursCurrent
                        // => nothing to do, skip to next
                        othersCurrent = othersCurrent.Next;
                    }
                    else
                    {
                        // intersection, starting from oursCurrent.From
                        // => determine the To
                        T? from = oursCurrent.Value.From;
                        T? to;
                        if (othersCurrent.Value.CoalesceTo.CompareTo(oursCurrent.Value.CoalesceTo) < 0)
                        {
                            to = othersCurrent.Value.To;
                            othersCurrent = othersCurrent.Next;
                        }
                        else
                        {
                            to = oursCurrent.Value.To;
                            oursCurrent = oursCurrent.Next;
                        }

                        result.Add(Create(from, to));
                    }
                }
            }

            return result;
        }

        public IEnumerable<TPeriod> ExceptWith<T2>(PeriodHistory<T2, T>? otherPeriodHistory)
            where T2 : class, IPeriod<T>
        {
            if ((otherPeriodHistory == null) || (otherPeriodHistory.Periods.Count == 0))
            {
                return Periods;
            }

            List<TPeriod> result = new ();

            LinkedList<TPeriod> ourPeriods = LinkedPeriods;
            LinkedListNode<TPeriod>? oursCurrent = ourPeriods.First;

            LinkedList<T2> others = otherPeriodHistory.LinkedPeriods;
            LinkedListNode<T2>? othersCurrent = others.First;

            while ((oursCurrent != null) && (othersCurrent != null))
            {
                // skip others until we reach ours
                while ((othersCurrent != null) && (othersCurrent.Value.CoalesceTo.CompareTo(oursCurrent.Value.CoalesceFrom) < 0))
                {
                    othersCurrent = othersCurrent.Next;
                }

                TPeriod? period = oursCurrent.Value;
                while ((othersCurrent != null) && (period != null) && period.Overlaps(othersCurrent.Value))
                {
                    if (period.CoalesceFrom.CompareTo(othersCurrent.Value.CoalesceFrom) < 0)
                    {
                        result.Add(Create(period.From, othersCurrent.Value.From));
                    }

                    // Is the period completely processed?
                    // if not, break it down
                    period =
                        othersCurrent.Value.CoalesceTo.CompareTo(period.CoalesceTo) < 0
                            ? Create(othersCurrent.Value.To, period.To)
                            : null;

                    // if others are processed, move further
                    if ((period != null) && (othersCurrent.Value.CoalesceTo.CompareTo(period.CoalesceTo) < 0))
                    {
                        othersCurrent = othersCurrent.Next;
                    }
                }

                // if there was a piece left, it wasn't covered by the other periods,
                // add to our result
                if (period != null)
                {
                    result.Add(period);
                }

                // ours is processed, move further
                oursCurrent = oursCurrent.Next;
            }

            // where do all nodes process from ours?
            // if not, add them all to our result
            while (oursCurrent != null)
            {
                result.Add(oursCurrent.Value);
                oursCurrent = oursCurrent.Next;
            }

            return result;
        }
    }
}
