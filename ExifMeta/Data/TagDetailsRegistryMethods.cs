using System.Linq;

namespace ExifMeta
{
    internal static partial class TagDetailsRegistry
    {
        public static TagDetails GetTagDetails(TagId tagId)
        {
            return tagDetails[tagId];
        }

        internal static bool IsUserTag(IfdId ifdId, Tag tag)
        {
            var found = tagDetails.Values.FirstOrDefault(x => x.IfdId == ifdId && x.Tag == tag);

            return found != null && found.IsUser;
        }

        internal static TagId FindTagId(IfdId ifdId, Tag tag, out TagDetails tagDetail)
        {
            foreach (var kvp in tagDetails)
            {
                if (kvp.Value.IfdId == ifdId && kvp.Value.Tag == tag)
                {
                    tagDetail = kvp.Value;
                    return kvp.Key;
                }
            }

            tagDetail = null;
            return 0;
        }
    }
}
