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

namespace PPWCode.Util.Time.I;

/// <summary>
///     This is a generic helper class to work with <see cref="IPeriod{T}" /> instances.
///     It represents a set of multiple, potentially overlapping, periods.
/// </summary>
/// <typeparam name="TPeriod">a specific type that implements <see cref="IPeriod{T}" /> </typeparam>
/// <typeparam name="T">a specific type of period</typeparam>
public abstract class PeriodMultiHistory<TPeriod, T>
    where TPeriod : class, IPeriod<T>
    where T : struct, IComparable<T>, IEquatable<T>
{
    private readonly RangeTreeNode _root;

    /// <summary>
    ///     Create a multi-history.
    /// </summary>
    /// <param name="periods">given set of periods</param>
    /// <exception cref="InternalProgrammingError">
    ///     This exception is thrown when the period multi-history is not correctly initialized.
    /// </exception>
    protected PeriodMultiHistory(params TPeriod[] periods)
        : this(periods, true)
    {
    }

    /// <summary>
    ///     Create a multi-history.
    /// </summary>
    /// <param name="periods">given set of periods</param>
    /// <exception cref="InternalProgrammingError">
    ///     This exception is thrown when the period multi-history is not correctly initialized.
    /// </exception>
    protected PeriodMultiHistory(IEnumerable<TPeriod> periods)
        : this(periods, true)
    {
    }

    /// <summary>
    ///     Create a multi-history.
    /// </summary>
    /// <param name="periods">given set of periods</param>
    /// <param name="checkCivilized">check each period if it is civilized</param>
    /// <exception cref="InternalProgrammingError">
    ///     This exception is thrown when the period multi-history is not correctly initialized.
    /// </exception>
    protected PeriodMultiHistory(IEnumerable<TPeriod> periods, bool checkCivilized)
    {
        // input periods must be civilized
        if (checkCivilized && periods.Any(p => !p.IsCivilized))
        {
            CompoundSemanticException cse =
                periods
                    .Where(p => !p.IsCivilized)
                    .Select(p => p.WildExceptions())
                    .Aggregate(
                        new CompoundSemanticException(),
                        (source, seed) =>
                        {
                            seed.AddElement(source);
                            return seed;
                        });
            throw cse;
        }

        // setup binary tree
        _root = new RangeTreeNode(periods);
    }

    protected abstract TPeriod Create(T? from, T? to);

    /// <summary>
    ///     Returns the periods in the multi-history that contain the given <paramref name="date" />.
    /// </summary>
    /// <param name="date">a given point in time</param>
    /// <returns>
    ///     An enumerable collection of instances of type <typeparamref name="TPeriod" /> that overlap with the
    ///     given <paramref name="date" />; the collection is empty if there are no overlapping periods
    /// </returns>
    public IEnumerable<TPeriod> GetPeriodsAt(T date)
        => _root.GetPeriodsAt(date);

    /// <summary>
    ///     Returns the periods in the multi-history that overlap with the period
    ///     defined by the given <paramref name="startDate" /> and <paramref name="endDate" />.
    /// </summary>
    /// <param name="startDate">the given start date</param>
    /// <param name="endDate">the given end date</param>
    /// <returns>
    ///     An enumerable collection of periods of type <typeparamref name="TPeriod" /> that overlap
    ///     with the period defined by <paramref name="startDate" /> and <paramref name="endDate" />.
    ///     If there are no overlapping periods found, the returned collection is empty.
    /// </returns>
    public IEnumerable<TPeriod> GetPeriodsOverlappingAt(T startDate, T endDate)
        => _root.GetPeriodsOverlappingAt(startDate, endDate);

    /// <summary>
    ///     Returns the periods in the multi-history that overlap with the period
    ///     defined by the given <paramref name="startDate" /> and <paramref name="endDate" />.
    /// </summary>
    /// <param name="startDate">the given start date</param>
    /// <param name="endDate">the given end date</param>
    /// <returns>
    ///     An enumerable collection of periods of type <typeparamref name="TPeriod" /> that overlap
    ///     with the period defined by <paramref name="startDate" /> and <paramref name="endDate" />.
    ///     If there are no overlapping periods found, the returned collection is empty.
    /// </returns>
    public IEnumerable<TPeriod> GetPeriodsOverlappingAt(T? startDate, T? endDate)
    {
        TPeriod period = Create(startDate, endDate);
        return GetPeriodsOverlappingAt(period.CoalesceFrom, period.CoalesceTo);
    }

    /// <summary>
    ///     Calculates and returns the optimal set of <typeparamref name="TPeriod"/> instances that
    ///     completely cover all time intervals that are occupied by at least one instance
    ///     of type <typeparamref name="TPeriod"/>.
    /// </summary>
    /// <returns>
    ///     An enumerable collection of <typeparamref name="TPeriod"/> instances
    /// </returns>
    public IEnumerable<TPeriod> GetOptimalCoveringPeriods()
        => _root.GetOptimalCoveringPeriods(Create);

    /// <summary>
    ///     Helper class to create a binary search tree for the periods.
    /// </summary>
    private class RangeTreeNode
    {
        public RangeTreeNode(IEnumerable<TPeriod> periodInstances)
        {
            if (periodInstances.Any())
            {
                Center =
                    periodInstances
                        .Select(p => p.CoalesceFrom)
                        .Concat(periodInstances.Select(p => p.CoalesceTo))
                        .OrderBy(dt => dt)
                        .ElementAt(periodInstances.Count() - 1);
                Inner =
                    periodInstances
                        .Where(p => p.Contains(Center))
                        .ToList();

                IEnumerable<TPeriod> leftPeriods =
                    periodInstances
                        .Where(p => p.CoalesceTo.CompareTo(Center) <= 0)
                        .ToList();
                Left = leftPeriods.Any() ? new RangeTreeNode(leftPeriods) : null;

                IEnumerable<TPeriod> rightPeriods =
                    periodInstances
                        .Where(p => Center.CompareTo(p.CoalesceFrom) < 0)
                        .ToList();
                Right = rightPeriods.Any() ? new RangeTreeNode(rightPeriods) : null;
            }
            else
            {
                Inner = new List<TPeriod>();
                Left = null;
                Right = null;
            }
        }

        public RangeTreeNode? Left { get; }

        public RangeTreeNode? Right { get; }

        public T Center { get; }

        public IList<TPeriod> Inner { get; }

        public IEnumerable<TPeriod> GetPeriodsAt(T date)
        {
            IEnumerable<TPeriod> result = Inner.Where(p => p.Contains(date));

            if ((Right != null) && (Center.CompareTo(date) < 0))
            {
                result = result.Concat(Right.GetPeriodsAt(date));
            }

            if ((Left != null) && (date.CompareTo(Center) < 0))
            {
                result = result.Concat(Left.GetPeriodsAt(date));
            }

            return result;
        }

        public IEnumerable<TPeriod> GetPeriodsOverlappingAt(T startDate, T endDate)
        {
            if (endDate.CompareTo(startDate) <= 0)
            {
                return [];
            }

            IEnumerable<TPeriod> result =
                Inner.Where(p => (p.CoalesceFrom.CompareTo(endDate) < 0) && (startDate.CompareTo(p.CoalesceTo) < 0));

            if ((Right != null) && (Center.CompareTo(endDate) < 0))
            {
                result = result.Concat(Right.GetPeriodsOverlappingAt(startDate, endDate));
            }

            if ((Left != null) && (startDate.CompareTo(Center) < 0))
            {
                result = result.Concat(Left.GetPeriodsOverlappingAt(startDate, endDate));
            }

            return result;
        }

        public LinkedList<TPeriod> GetOptimalCoveringPeriods(Func<T?, T?, TPeriod> create)
        {
            // get optimal covering periods left and right
            LinkedList<TPeriod>? leftCoveringPeriods = Left?.GetOptimalCoveringPeriods(create);
            LinkedList<TPeriod>? rightCoveringPeriods = Right?.GetOptimalCoveringPeriods(create);
            LinkedListNode<TPeriod>? leftTailNode = leftCoveringPeriods?.Last;
            LinkedListNode<TPeriod>? rightHeadNode = rightCoveringPeriods?.First;
            LinkedList<TPeriod> coveringPeriods = new ();

            if (Inner.Any())
            {
                // inner periods are all overlapping "Center" value
                T? rangeFrom = Inner.OrderBy(p => p.CoalesceFrom).First().From;
                T? rangeTo = Inner.OrderBy(p => p.CoalesceTo).Last().To;
                TPeriod runningPeriod = create(rangeFrom, rangeTo);

                // plug to left, all left periods are left of Center
                //     BUT not left of [rangeFrom, rangeTo[
                while ((leftTailNode != null) && (runningPeriod.CoalesceFrom.CompareTo(leftTailNode.Value.CoalesceTo) <= 0))
                {
                    if (leftTailNode.Value.CoalesceFrom.CompareTo(runningPeriod.CoalesceFrom) < 0)
                    {
                        rangeFrom = leftTailNode.Value.From;
                        runningPeriod = create(rangeFrom, rangeTo);
                    }

                    leftTailNode = leftTailNode.Previous;
                }

                // plug to right, all right periods are right of Center
                //     BUT not right of [rangeFrom, rangeTo[
                while ((rightHeadNode != null) && (rightHeadNode.Value.CoalesceFrom.CompareTo(runningPeriod.CoalesceTo) <= 0))
                {
                    if (runningPeriod.CoalesceTo.CompareTo(rightHeadNode.Value.CoalesceTo) < 0)
                    {
                        rangeTo = rightHeadNode.Value.To;
                        runningPeriod = create(rangeFrom, rangeTo);
                    }

                    rightHeadNode = rightHeadNode.Next;
                }

                // create new linked list
                TPeriod coveringPeriod = runningPeriod;
                LinkedListNode<TPeriod> coveringPeriodNode = new (coveringPeriod);
                coveringPeriods.AddFirst(coveringPeriodNode);
            }

            // link to left
            while (leftTailNode != null)
            {
                coveringPeriods.AddFirst(leftTailNode.Value);
                leftTailNode = leftTailNode.Previous;
            }

            // link to right
            while (rightHeadNode != null)
            {
                coveringPeriods.AddLast(rightHeadNode.Value);
                rightHeadNode = rightHeadNode.Next;
            }

            return coveringPeriods;
        }
    }
}
