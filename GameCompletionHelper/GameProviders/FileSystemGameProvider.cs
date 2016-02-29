using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCompletionHelper.Model;
using System.Xml.Serialization;
using System.IO;

namespace GameCompletionHelper.GameProviders
{
    class FileSystemGameProvider : IGamesProvider
    {
        XmlSerializer serializer = new XmlSerializer(typeof(List<Game>));

        public string FilePath { get; set; }

        public FileSystemGameProvider(string filePath = "data.xml")
        {
            FilePath = filePath;
        }
        public IEnumerable<Game> GetGames()
        {
            try {
                using (Stream stream = File.Open(FilePath, FileMode.Open))
                {
                    return (List<Game>)serializer.Deserialize(stream);
                }
            }
            catch
            {
                return new List<Game>();
            }
        }

        public void SaveGames(IEnumerable<Game> games)
        {
            var gamesList = games.ToList();
            using (Stream stream = File.Open(FilePath, FileMode.Create))
            {
                serializer.Serialize(stream, gamesList);
            }
        }
    }
}
