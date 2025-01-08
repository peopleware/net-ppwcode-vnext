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

namespace PPWCode.Util.Collection.I;

public static class CollectionExtensions
{
    /// <summary>Checks whether 2 IEnumerable of T are equal.</summary>
    /// <typeparam name="T">The type used.</typeparam>
    /// <param name="outerSequence">The first IEnumerable of T.</param>
    /// <param name="innerSequence">The second IEnumerable of T.</param>
    /// <returns>True or false.</returns>
    /// <pure />
    public static bool BagEqual<T>(this IEnumerable<T> outerSequence, IEnumerable<T> innerSequence)
        => outerSequence.BagEqual(innerSequence, Comparer<T>.Default);

    /// <summary>
    ///     Checks whether 2 IEnumerable of T are equal given a comparer.
    /// </summary>
    /// <typeparam name="T">The type used.</typeparam>
    /// <param name="outerSequence">The first IEnumerable of T.</param>
    /// <param name="innerSequence">The second IEnumerable of T.</param>
    /// <param name="comparer">The equality comparer.</param>
    /// <returns>True or false.</returns>
    /// <pure />
    public static bool BagEqual<T>(
        this IEnumerable<T> outerSequence,
        IEnumerable<T> innerSequence,
        IComparer<T> comparer)
        => outerSequence
            .OrderBy(x => x, comparer)
            .SequenceEqual(innerSequence.OrderBy(x => x, comparer));

    /// <summary>Checks whether 2 IEnumerable of T are equal.</summary>
    /// <remarks>Doubles are ignored.</remarks>
    /// <typeparam name="T">The type used.</typeparam>
    /// <param name="outerSequence">The first IEnumerable of T.</param>
    /// <param name="innerSequence">The second IEnumerable of T.</param>
    /// <returns>True or false.</returns>
    /// <pure />
    public static bool SetEqual<T>(this IEnumerable<T> outerSequence, IEnumerable<T> innerSequence)
        => outerSequence.SetEqual(innerSequence, EqualityComparer<T>.Default);

    /// <summary>
    ///     Checks whether 2 IEnumerable of T are equal given a comparer.
    /// </summary>
    /// <remarks>Doubles are ignored.</remarks>
    /// <typeparam name="T">The type used.</typeparam>
    /// <param name="outerSequence">The first IEnumerable of T.</param>
    /// <param name="innerSequence">The second IEnumerable of T.</param>
    /// <param name="comparer">The equality comparer.</param>
    /// <returns>True or false.</returns>
    /// <pure />
    public static bool SetEqual<T>(
        this IEnumerable<T> outerSequence,
        IEnumerable<T> innerSequence,
        IEqualityComparer<T> comparer)
        => new HashSet<T>(outerSequence, comparer).SetEquals(innerSequence);

    public static DiffResult<T> CalcDiff<T>(
        this IEnumerable<T> from,
        IEnumerable<T> to)
        where T : class
        => from.CalcDiff(to, EqualityComparer<T>.Default);

    public static DiffResult<T> CalcDiff<T>(
        this IEnumerable<T> from,
        IEnumerable<T> to,
        IEqualityComparer<T> comparer)
    {
        ISet<T> currentItems = from.ToHashSet(comparer);
        ISet<T> targetItems = to.ToHashSet(comparer);

        ISet<T> newItems = new HashSet<T>(targetItems, comparer);
        newItems.ExceptWith(currentItems);
        ISet<T> obsoleteItems = new HashSet<T>(currentItems, comparer);
        obsoleteItems.ExceptWith(targetItems);
        ISet<T> intersectedItems = new HashSet<T>(currentItems, comparer);
        intersectedItems.IntersectWith(targetItems);
        return new DiffResult<T>(newItems, obsoleteItems, intersectedItems);
    }

    public static void ExecDiff<T>(
        this IEnumerable<T> from,
        IEnumerable<T> to,
        Action<T>? newAction,
        Action<T>? obsoleteAction,
        Action<T>? intersectedAction)
        => from.ExecDiff(to, EqualityComparer<T>.Default, newAction, obsoleteAction, intersectedAction);

    public static void ExecDiff<T>(
        this IEnumerable<T> from,
        IEnumerable<T> to,
        IEqualityComparer<T> comparer,
        Action<T>? newAction,
        Action<T>? obsoleteAction,
        Action<T>? intersectedAction)
    {
        DiffResult<T> diff = from.CalcDiff(to, comparer);

        if (newAction != null)
        {
            foreach (T item in diff.NewItems)
            {
                newAction(item);
            }
        }

        if (obsoleteAction != null)
        {
            foreach (T item in diff.ObsoleteItems)
            {
                obsoleteAction(item);
            }
        }

        if (intersectedAction != null)
        {
            foreach (T item in diff.IntersectedItems)
            {
                intersectedAction(item);
            }
        }
    }
}

public class DiffResult<T>
{
    public DiffResult(
        ISet<T> newItems,
        ISet<T> obsoleteItems,
        ISet<T> intersectedItems)
    {
        NewItems = newItems;
        ObsoleteItems = obsoleteItems;
        IntersectedItems = intersectedItems;
    }

    public ISet<T> ObsoleteItems { get; }
    public ISet<T> IntersectedItems { get; }
    public ISet<T> NewItems { get; }
}
