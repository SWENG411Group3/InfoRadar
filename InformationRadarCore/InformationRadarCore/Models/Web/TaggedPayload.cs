using InformationRadarCore.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace InformationRadarCore.Models.Web
{
    public class TaggedPayload
    {
        public IList<string>? Tags { get; set; }

        // Returns null if all tags are valid
        // Otherwise, the firstr invalid tag is returned
        public string? HasInvalidTag()
        {
            if (Tags == null)
            {
                return null;
            }

            for (int i = 0; i < Tags.Count; i++)
            {
                Tags[i] = Tags[i].ToLower();
                var tag = Tags[i];
                if (string.IsNullOrEmpty(tag) || !Regex.IsMatch(tag, @"^\w{1,100}$"))
                {
                    return tag;
                }
            }

            return null;
        }

        public async Task EnsureTags(ApplicationDbContext db, Lighthouse lighthouse)
        {
            if (Tags == null) { return; }

            foreach (var tag in Tags)
            {
                var t = await db.Tags
                    .Include(t => t.Lighthouses)
                    .SingleOrDefaultAsync(t => t.TagName == tag);
                if (t != null)
                {
                    if (!t.Lighthouses.Contains(lighthouse))
                    {
                        t.Lighthouses.Add(lighthouse);
                    }
                }
                else
                {
                    await db.Tags.AddAsync(new Tag()
                    {
                        TagName = tag,
                        Lighthouses = new List<Lighthouse>() { lighthouse },
                    });
                }
            }
        }
    }
}
