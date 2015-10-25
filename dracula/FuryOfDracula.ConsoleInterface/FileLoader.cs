using FuryOfDracula.GameLogic;
using System;
using System.IO;
using System.Runtime.Serialization;

namespace FuryOfDracula.ConsoleInterface
{
    public class FileLoader
    {
        /// <summary>
        /// Loads a previously saved gamestate from a file with the given fileName
        /// </summary>
        /// <param name="fileName">The name of the file, without extension</param>
        /// <returns>A deserialised GameState object from the file</returns>
        public GameState LoadGameState(string fileName)
        {
            GameState tempGame = null;
            try
            {
                string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
                foreach (char c in invalid)
                {
                    fileName = fileName.Replace(c.ToString(), "");
                }
                fileName = fileName + ".sav";
                DataContractSerializer fileReader = new DataContractSerializer(typeof(GameState));
                FileStream readStream = File.OpenRead(fileName);

                tempGame = (GameState)fileReader.ReadObject(readStream);
                readStream.Close();
                Console.WriteLine(fileName + " loaded");
            }
            catch (Exception e)
            {
                Console.WriteLine("File {0} could not be loaded because {1}", fileName, e.Message);
            }
            return tempGame;
        }
    }
}
