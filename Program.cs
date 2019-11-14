﻿using System;
using System.Net;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using System.Collections.Generic;
using System.IO;

namespace Air_BOT
{
    class Program
    {
        static ITelegramBotClient botClient;
        static ICollection<Weather> weathers { get; set; }

        public static void Main(string[] args)
        {
            botClient = new TelegramBotClient("668648971:AAF-71Stk_POcLfu4E7nfRArtc3OQsbjei4");

            var me = botClient.GetMeAsync().Result;

            botClient.OnMessage += Bot_OnMessage;
            botClient.StartReceiving();
            Thread.Sleep(int.MaxValue);
        }

        public static void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Text == "/start")
            {
                e.Message.Text = null;

                botClient.SendTextMessageAsync(
                    chatId: e.Message.Chat,
                    text: "Olá, seja bem-vindo.\n"
                        + "Digite algum ICAO para consulta. Exemplo: 'SBGR'"
                );
            }
            else if (e.Message.Text != null || e.Message.Text != string.Empty)
            {
                botClient.SendTextMessageAsync(
                    chatId: e.Message.Chat,
                    text: $"{GetIcaoCode(e.Message.Text)}"
                );
            }
        }

        public static string GetIcaoCode(string Code)
        {
            var request = (HttpWebRequest)WebRequest.Create($"http://www.redemet.aer.mil.br/api/consulta_automatica/index.php?local={Code}&msg=metar");
 
            request.Method = "GET";
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
    
            var content = string.Empty;
    
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var sr = new StreamReader(stream))
                    {
                        content = sr.ReadToEnd();
                    }
                }
            }
    
            return content;
        }
    }
}