


using iTextSharp.text;
using iTextSharp.text.pdf;


namespace Test
{
    class Test
    {
        public class YourObjectType
        {
            public int Id { get; set; }
            public int Level { get; set; }
        }

        internal class Program
        {
            
            static void Main(string[] args)
            {
                var items = new List<YourObjectType>
                {
                    new YourObjectType { Id = 1, Level = 1 },
                    new YourObjectType { Id = 2, Level = 2 },
                    new YourObjectType { Id = 3, Level = 2 },
                    new YourObjectType { Id = 4, Level = 1 },
                    new YourObjectType { Id = 5, Level = 2 }
                };

                var groupedItems = GroupByLevel(items);

                // Access level 1 items
                var level1Items = groupedItems[1];

                foreach (var item in level1Items)
                {
                    Console.WriteLine(item.Id + "__" + item.Level);
                }

                // Access level 2 items
                var level2Items = groupedItems[2];
                foreach (var item in level2Items)
                {
                    Console.WriteLine(item.Id + "__" + item.Level);
                }

            }
            public static Dictionary<int, List<YourObjectType>> GroupByLevel(List<YourObjectType> items)
            {
                return items.GroupBy(item => item.Level)
                            .ToDictionary(group => group.Key, group => group.ToList());
            }

        }


    }
    
}
