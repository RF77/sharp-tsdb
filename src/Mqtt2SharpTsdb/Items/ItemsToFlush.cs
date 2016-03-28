using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Mqtt2SharpTsdb.Items
{
    public static class ItemsToFlush
    {
        private static readonly HashSet<IMeasurementItem> Items = new HashSet<IMeasurementItem>(); 

        private static Timer _timer = new Timer(TimerElapsed, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));

        private static void TimerElapsed(object state)
        {
            IList<IMeasurementItem> items = null;
            lock (typeof(ItemsToFlush))
            {
                if (Items.Any())
                {
                    items = DequeueAllItems().ToList();
                }
            }

            if (items != null)
            {
                foreach (var item in items)
                {
                    item.Flush();
                }
            }
        }

        public static void AddItem(IMeasurementItem item)
        {
            lock (typeof (ItemsToFlush))
            {
                Items.Add(item);
            }
        }

        public static void AddItems(IEnumerable<IMeasurementItem> items)
        {
            lock (typeof(ItemsToFlush))
            {
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
        }

        public static IEnumerable<IMeasurementItem> DequeueAllItems()
        {
            lock (typeof(ItemsToFlush))
            {
                var items = Items.ToArray();
                Items.Clear();
                return items;
            }
        }
    }
}