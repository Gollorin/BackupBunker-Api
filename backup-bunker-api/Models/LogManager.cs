namespace backup_bunker_api.Models
{
    public class LogManager
    {
        private string Path = Directory.GetCurrentDirectory() + @"\Log\Log.txt";

        public void WriteMessage(string message)
        {
            using (StreamWriter writer = new StreamWriter(this.Path, true))
            {
                string time = DateTime.Now.ToString();
                writer.WriteLine(time + "  :  " + message);
            }
        }

        public List<string> ReadLastHundredMessage()
        {
            List<string> lastLines = new List<string>();

            using (StreamReader reader = new StreamReader(this.Path))
            {
                string line;
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    lastLines.Add(line);
                }
            }

            lastLines.Reverse();

            if(lastLines.Count > 100)
                lastLines = lastLines.Skip(lastLines.Count - 100).Take(100).ToList();

            return lastLines;
        }
    }
}
