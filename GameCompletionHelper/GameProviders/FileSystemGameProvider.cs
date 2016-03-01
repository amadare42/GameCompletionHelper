using System.Collections.Generic;
using System.Linq;
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
        public IEnumerable<IGame> GetGames()
        {
            try {
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
            if (gameEnumerable!=null)
            {
                gamesList = gameEnumerable.ToList();
            }
            else
            {
                gamesList = games.Select(igame =>
                {

                    return new Game()
                    {
                        Name = igame.Name,
                        PathToExe = igame.PathToExe,
                        Sessions = igame.Sessions.ToList(),
                        RunAsAdmin = igame.RunAsAdmin,
                        RunPath = igame.RunPath
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
