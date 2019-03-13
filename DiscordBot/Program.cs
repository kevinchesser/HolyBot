using Discord;
using Discord.WebSocket;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using QuickType;
using System.Linq;
using System.Collections.Generic;

namespace DiscordBot
{
    public class Program
    {
        private DiscordSocketClient _client;
        private List<Welcome> _welcomes;
        private string[] jsonFiles = Directory.GetFiles("./Resources/", "*.json")
                                     .Select(Path.GetFileName)
                                     .ToArray();

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();

            _client.MessageReceived += MessageReceived;
            _welcomes = new List<Welcome>();

            for(int i = 0; i < jsonFiles.Length; i++)
            {
                string file = jsonFiles[i];
                StreamReader sr = new StreamReader("./Resources/" + file);
                string jsonString = sr.ReadToEnd();
                Welcome welcome = Welcome.FromJson(jsonString);
                _welcomes.Add(welcome);
            }

            string token = ""; // Remember to keep this private!
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private async Task MessageReceived(SocketMessage message)
        {
            if (message.Content.Contains("!spongebob"))
            {
                string s = message.Content.Substring(11);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < s.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        sb.Append(s[i].ToString().ToUpper());
                    }
                    else
                    {
                        sb.Append(s[i].ToString());
                    }
                }
                await message.Channel.SendMessageAsync(sb.ToString());
            }

            if (message.Content == "Worst team in the nfl?")
            {
                await message.Channel.SendMessageAsync("Raiders");
            }

            if (message.Content.Contains("!verse"))
            {
                Random rnd = new Random();
                int randomBook = rnd.Next(0, _welcomes.Count);
                Welcome welcome = _welcomes.ElementAt(randomBook);
                int randomChapter = rnd.Next(0, welcome.Chapters.Length - 1);
                int randomVerse = rnd.Next(0, welcome.Chapters[randomChapter].Verses.Length);
                welcome.Chapters[randomChapter].Verses[randomVerse].TryGetValue((randomVerse + 1).ToString(), out string verse);
                int realChapter = randomChapter + 1;
                int realVerse = randomVerse + 1;
                verse = verse + " - " + welcome.Book + " " + realChapter + ":" + realVerse;
                await message.Channel.SendMessageAsync(verse);

            }
        }



    }
}