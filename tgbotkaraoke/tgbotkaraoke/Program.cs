using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

class Program
{
    static Dictionary<long, string> userNames = new Dictionary<long, string>();
    static string token = "8018085431:AAEXIIGNfiX-gU8UItefB_AeWGUOrryCv-I";
    static TelegramBotClient bot;

    static void Main()
    {
        
        bot = new TelegramBotClient(token);
        bot.StartReceiving(OnMessage, OnError);

        Console.WriteLine("Бот запущен. Нажми Enter чтобы выйти.");
        Console.ReadLine();
    }

    static async Task OnMessage(ITelegramBotClient bot, Update update, CancellationToken token)
    {
        var msg = update.Message;
        if (msg == null) return;

        long chatId = msg.Chat.Id;
        string text = msg.Text ?? "";

        bool userExists = userNames.ContainsKey(chatId);

        if (!userExists)
        {
            if (msg.Text == "/start")
            {
                await bot.SendMessage(chatId, "Привет, ты попал в наш караоке бар, как я могу к тебе обращаться?");
                return;
            }
            else
            {
                if (text.Length < 2)
                {
                    await bot.SendMessage(chatId, "Имя слишком короткое!");
                    return;
                }
                if (text.Length > 20)
                {
                    await bot.SendMessage(chatId, "Так не пойдет имя слишком длинное. Максимальная длина имени 20 символов");
                    return;
                }

                userNames[chatId] = text;
                await ShowMainMenu(chatId);
                return;
            }
        }

        string userName = userNames[chatId];

        switch (text)
        {
            case "/start":
            case "Главное меню":
            case "Назад":
                await ShowMainMenu(chatId);
                break;

            case "Песни на выбор":
                await ShowFAQMenu(chatId);
                break;

            case "Команды для атмосферы":
                await ShowExamplesMenu(chatId);
                break;

            case "Хочу лабубу":
                await bot.SendVoice(chatId,
                    "https://muzvoy.com/uploads/music/2025/08/Alya_Bridzh_LABUBA.mp3",
                    caption: "Я сплю, я сплю, я сплю, я сплю, я сплю, я сплю, я сплю\r\nКрепко\r\nЯ сплю, я сплю, я сплю, я сплю, я сплю, я сплю\r\n\r\nПодушка пэчворк — я хочу спать\r\nНа боку крючком я хочу спать (Крючком!)\r\nХочу лабубу, но не хочу просыпать (У-у)\r\nНафиг вязать, если можно просто спать? (Bitch!)\r\n\r\nВ ладони — подушка\r\nВо рту слюня пухлая\r\nЦелую тебя в макушку\r\nХрап мой «Хр-хр-хр-хр»\r\nВ ладони — подушка\r\nВо рту слюня пухлая\r\nЦелую тебя в макушку\r\nХрап мой послушай (Чё?)",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                break;

            case "Матушка земля":
                await bot.SendVoice(chatId,
                    "https://muzvoy.com/uploads/music/2025/08/Tatyana_Kurtukova_Matushka_zemlya_belaya_berezonka.mp3",
                    caption: "Полевых зачётов стопочка, в утренней росе похмелье\r" +
                    "\nПреподавателя свисточек, энергетика глоток\r" +
                    "\nТишины послушать неволю, по коридору втихую\r" +
                    "\nПоболтать с автоматом вдоволь про стипендию и про любовь\r" +
                    "\n\r\n[Припев]\r\nМатушка-сессия, белая зачётко-ночка\r" +
                    "\nДля меня — альма-матер Русь, для мозга — занозонька\r" +
                    "\nМатушка-сессия, ой, белая зачётко-ночка\r" +
                    "\nДля меня — студенческая Русь, для печени — занозонька\r\n",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                break;

            case "Привет":
                await bot.SendMessage(chatId, $"Привет, {userName}!");
                break;

            case "Картинка":
                try
                {
                    await bot.SendPhoto(chatId, "https://cdn.culture.ru/images/c9508923-4509-5b4d-a5ac-fae285526140",
                        caption: $"Держи тебе микрофон, чтобы все тебя услышали, {userName}");
                }
                catch
                {
                    await bot.SendMessage(chatId, "Не удалось загрузить картинку");
                }
                break;

            case "Гифка":
                try
                {
                    await bot.SendAnimation(chatId, "https://media1.tenor.com/m/VwbsizCofagAAAAC/karaoke-metal.gif",
                        caption: "Извините, не нашёл подходящего видео поэтому только гифка");
                }
                catch
                {
                    await bot.SendMessage(chatId, "Не удалось загрузить видео");
                }
                break;

            case "Стикер":
                try
                {
                    await bot.SendSticker(chatId, "CAACAgIAAxkBAAESm4Vo1Yk-cRo2FO-UdOZXqRpAoEQn6gACwIgAAje0sEoGq2z9zR4ilTYE");
                    await bot.SendMessage(chatId, $"Танцуй как никогда, {userName}");
                }
                catch
                {
                    await bot.SendMessage(chatId, "Не удалось отправить стикер");
                }
                break;

            
                

            default:
                await bot.SendMessage(chatId,
                    $"Не понимаю тебя, {userName}. Выбери команду из меню ниже:");
                await ShowMainMenu(chatId);
                break;
        }
    }

    static async Task ShowMainMenu(long chatId)
    {
        string userName = userNames[chatId];

        var menu = new ReplyKeyboardMarkup(new[]
        {
            new[] { new KeyboardButton("Песни на выбор") },
            new[] { new KeyboardButton("Команды для атмосферы") }
        })
        { ResizeKeyboard = true };

        await bot.SendMessage(chatId,
            $" <b>Главное меню, {userName}!</b>\n\nВыбери раздел:",
            replyMarkup: menu,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
    }

    static async Task ShowFAQMenu(long chatId)
    {
        string userName = userNames[chatId];

        var faqMenu = new ReplyKeyboardMarkup(new[]
        {
            new[] { new KeyboardButton("Хочу лабубу"), new KeyboardButton("Матушка земля") },
            new[] { new KeyboardButton("Главное меню") }
        })
        { ResizeKeyboard = true };

        await bot.SendMessage(chatId,
            $" <b>Песни на выбор, {userName}:</b>\n\nВыбери песню:",
            replyMarkup: faqMenu,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
    }

    static async Task ShowExamplesMenu(long chatId)
    {
        string userName = userNames[chatId];

        var examplesMenu = new ReplyKeyboardMarkup(new[]
        {
            new[] { new KeyboardButton("Картинка"), new KeyboardButton("Стикер") },
            new[] { new KeyboardButton("Привет"), new KeyboardButton("Гифка")},
            new[] { new KeyboardButton("Главное меню") }
        })
        { ResizeKeyboard = true };

        await bot.SendMessage(chatId,
            $" <b>Погрузись в атмосферу нашего клуба, {userName}:</b>\n\nВыбери действие:",
            replyMarkup: examplesMenu,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
    }

    static Task OnError(ITelegramBotClient bot, Exception error, CancellationToken token)
    {
        Console.WriteLine($"Ошибка: {error.Message}");
        return Task.CompletedTask;
    }
}