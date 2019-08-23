using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace produce.Models
{
    class Item
    {
        public string id;
        public string Type;
        public string Title;
        public string iTunesSubtitle;
        public string iTunesSummary;
        public string EnclosureLength;
        public string EnclosureType;
        public string EnclosureURL;
        public string GUID;
        public string PubDate;
        public string iTunesDuration;
        public string iTunesExplicit;
        public string iTunesEpisodeNumber;

        public async Task Persist()
        {
            await DocumentDBRepository<Item>.UpdateItemAsync(id, this);
        }

        public static async Task<IEnumerable<Item>> GetAllItemsAsync()
        {
            var items = await DocumentDBRepository<Item>.GetItemsAsync(d => d.Type == "item");
            return items;
        }
    }
}
