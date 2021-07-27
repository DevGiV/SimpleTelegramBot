using System;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Telegram.Bot.Types.InputFiles;


namespace TelegramBotVeniamin
{
    class Program
    {
        private static getCompliment getComplimentObject;
        private static TelegramBotClient Bot;

        static string imagesPath = @"C:\image\";
        static string videosPath = @"C:\video\";
        static List<string> images = new List<string>
        {
            "1.jpg",
            "2.jpg",
            "3.jpg",
            "4.jpg",
            "5.jpg",
        };
        static List<string> videos = new List<string>
        {
            "1.mp4",
            "2.mp4",
            "3.mp4",
            "4.mp4",
            "5.mp4",
        };
        static void Main(string[] args)
        {
            getComplimentObject = new getCompliment();
            Bot = new TelegramBotClient("1879720952:AAE97XzDafheTg55WqxkusERdAuk1zkg9QU");

            Bot.OnMessage += Bot_OnMessageReceived;
            Bot.OnCallbackQuery += BotOnCallbackQueryRecieved;

            Bot.StartReceiving();
            Console.ReadLine();
            Bot.StopReceiving();
        }

        private static async void BotOnCallbackQueryRecieved(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            string buttonText = e.CallbackQuery.Data;
            string name = $"{e.CallbackQuery.From.FirstName} {e.CallbackQuery.From.LastName}";
            Console.WriteLine($"{name} нажал кнопку {buttonText}");

            await Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, $"Вы нажали кнопку {buttonText}");


        }

        private static async void Bot_OnMessageReceived(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {


            Random random = new Random();
            var message = e.Message;

            if (message == null || message.Type != MessageType.Text)
                return;

            string name = $"{message.From.FirstName} {message.From.LastName}";
            Console.WriteLine($"{name} отправил сообщение: '{message.Text}'");

            switch (message.Text)
            {
                case "/start":
                    Bot.StartReceiving();
                    string text =
@"Привет
Чтобы посмотреть команды напиши мне: /commands";

                    await Bot.SendTextMessageAsync(message.From.Id, text);
                    break;

                case "/commands":
                    string commands =
@"Список команд:
/start - запуск бота
/inline - вывод меню
/keyboard - вывод клавиатуры
/getImage - получить картинку
/getVideo - получить видео
/getCompliment - получить комплимент
/stop - остановить бота";

                    await Bot.SendTextMessageAsync(message.From.Id, commands);
                    break;
                case "/inline":
                    var inlineKeyboard = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithUrl("Написать в ВК","https://vk.com/andreas_vold"),
                            InlineKeyboardButton.WithUrl("Отправить фотку","https://t.me/Andreas_Vold"),
                        }

                    }
                        );

                    await Bot.SendTextMessageAsync(message.From.Id, "Выберите пункт меню", replyMarkup: inlineKeyboard);
                    break;

                case "/keyboard":
                    var replyKeyboard = new ReplyKeyboardMarkup(new[]
                    {
                        new[]
                        {
                             new KeyboardButton("Отправить свой номер телефона") {RequestContact = true },
                            new KeyboardButton("Где я нахожусь?") {RequestLocation = true }
                        }
                    });

                    await Bot.SendTextMessageAsync(message.Chat.Id, "Сообщение", replyMarkup: replyKeyboard);
                    break;

                case "/stop":
                    await Bot.SendTextMessageAsync(message.From.Id, "До встречи!");
                    Bot.StopReceiving();

                    break;

                case "/getCompliment":
                    var compliment = getComplimentObject.GetComplimentItem();
                    await Bot.SendTextMessageAsync(message.From.Id, compliment.item);
                    break;

                case "/getImage":

                    int count = random.Next(images.Count);

                    using (var stream = File.OpenRead(imagesPath + images[count]))
                    {
                        InputOnlineFile inputOnlineFile = new InputOnlineFile(stream);
                        await Bot.SendPhotoAsync(e.Message.Chat.Id, inputOnlineFile);
                    }
                    break;


                case "/getVideo":
                    int item = random.Next(images.Count);

                    using (var stream = File.OpenRead(videosPath + videos[item]))
                    {
                        InputOnlineFile inputOnlineFile = new InputOnlineFile(stream);
                        await Bot.SendVideoAsync(e.Message.Chat.Id, inputOnlineFile);
                    }
                    break;
                default:
                    await Bot.SendTextMessageAsync(message.From.Id, "Введите правильную команду!");
                    break;
            }
        }
    }

    class getCompliment
    {

        List<complimentItem> compliments;

        Random random;
        int count;

        public getCompliment(string path = "compliments.txt")
        {
            random = new Random();

            var lines = File.ReadAllLines("compliments.txt");
            compliments = lines.Select(s => s.Split("|")).Select(s =>
            new complimentItem
            {
                item = s[0]
            }).ToList();

        }

        public complimentItem GetComplimentItem()
        {
            if (count < 1)
            {
                count = compliments.Count;
            }

            var index = random.Next(count - 1);
            var item = compliments[index];

            compliments.RemoveAt(index);
            compliments.Add(item);
            count--;

            return item;
        }
    }

    class complimentItem
    {
        public string item { get; set; }
    }



  
}
 