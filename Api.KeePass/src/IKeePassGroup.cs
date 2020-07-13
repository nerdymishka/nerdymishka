using System;
using System.Collections.Generic;

namespace NerdyMishka.Api.KeePass
{
    public interface IKeePassGroup : IKeePassNode
    {
        bool IsExpanded { get; set; }

        string DefaultAutoTypeSequence { get; set; }

        bool? EnableAutoType { get; set; }

        bool? EnableSearching { get; set; }

        string Notes { get; set; }

        KeePassIdentifier LastTopVisibleEntryId { get; set; }

        MoveableList<IKeePassEntry> Entries { get; }

        MoveableList<IKeePassGroup> Groups { get; }

        CustomDataDictionary CustomData { get; }

        /// <summary>
        /// Gets a group by index. If the index is higher than
        /// the number of groups or lower than zero, null is returned.
        /// </summary>
        /// <param name="index">The index of the group.</param>
        /// <returns>The group; otherwise, null.</returns>
        IKeePassGroup Group(int index);

        /// <summary>
        /// Gets a group by name. The name equality test is case
        /// insensitive. Returns null if the entry is not found.
        /// </summary>
        /// <param name="name">The name (title) of the group.</param>
        /// <param name="comparison">The type of compare to perform.</param>
        /// <returns>The group; otherwise, null.</returns>
        IKeePassGroup Group(
            string name,
            StringComparison comparison = StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Gets an entry by index. If the index is higher than the
        /// number of entries or lower than zero, null is returned.
        /// </summary>
        /// <param name="index">The index of the entry.</param>
        /// <returns>The entry; otherwise, null.</returns>
        IKeePassEntry Entry(int index);

        /// <summary>
        /// Gets an entry by name. The name equality test is case
        /// insensitive. Returns null if the entry is not found.
        /// </summary>
        /// <param name="name">The name (title) of the entry.</param>
        /// <param name="comparison">The type of compare to perform.</param>
        /// <returns>The entry; otherwise, null.</returns>
        IKeePassEntry Entry(
            string name,
            StringComparison comparison = StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Copies all the values of this instance to the destination group.
        /// </summary>
        /// <param name="destinationGroup">The copy operation destination group.</param>
        /// <returns>The destination group.</returns>
        IKeePassGroup CopyTo(IKeePassGroup destinationGroup);

        void ExportTo(IKeePassGroup destination);

        /// <summary>
        /// Merges all the sub groups and entries to the destination group. Merge only
        /// modifies the <see cref="IKeePassGroup.Groups" /> and <see cref="IKeePassGroup.Entries" />
        /// properties. Entries for each sub group will be merged.
        /// </summary>
        /// <param name="destination">The group that should receive the entries and groups.</param>
        /// <param name="overwrite">
        /// If the entry or group isn't new, the values will be overwritten
        /// if the Uuid property matches.
        /// </param>
        /// <param name="ignoreGroups">
        /// If true, the subgroups and the subgroup entities will not be merged.
        /// </param>
        void MergeTo(IKeePassGroup destination, bool overwrite = false, bool ignoreGroups = false);
    }
}