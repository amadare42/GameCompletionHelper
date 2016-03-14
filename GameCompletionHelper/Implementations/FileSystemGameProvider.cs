using GameCompletionHelper.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using GameCompletionHelper.Interfaces;

namespace GameCompletionHelper.Implementations
{
    internal class FileSystemGameProvider : IGamesProvider
    {
        private XmlSerializer serializer = new XmlSerializer(typeof(List<Game>));

        public string FilePath { get; set; }

        public FileSystemGameProvider(string filePath = "data.xml")
        {
            FilePath = filePath;
        }

        public IEnumerable<IGame> GetGames()
        {
            try
            {
                using (Stream stream = File.Open(FilePath, FileMode.Open))
                {
                    return (IEnumerable<IGame>)serializer.Deserialize(stream);
                }
            }
            catch
            {
                return new List<IGame>();
            }
        }

        public void SaveGames(IEnumerable<IGame> games)
        {
            List<Game> gamesList;
            var gameEnumerable = games as IEnumerable<Game>;
            if (gameEnumerable != null)
            {
                gamesList = gameEnumerable.ToList();
            }
            else
            {
                gamesList = games.Select(igame =>
                {
                    //todo: implement automapping
                    return new Game()
                    {
                        Name = igame.Name,
                        PathToExe = igame.PathToExe,
                        Sessions = igame.Sessions.ToList(),
                        Options = igame.Options
                    };
                }).ToList();
            }
            using (Stream stream = File.Open(FilePath, FileMode.Create))
            {
                serializer.Serialize(stream, gamesList);
            }
        }
    }
}