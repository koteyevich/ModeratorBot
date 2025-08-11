# ModeratorBot

***A bot for moderating Telegram chats.***

<details>
  <summary>Quick history</summary>
This bot is something that I wanted to do for a long time. This is not the first attempt at developing this bot, 
the first bot was also made in C#, but used SQLite with the tables written by ChatGPT and was a disaster to maintain 
because of the horrible code and structure. So, this is attempt number 2, and this is looking much better.
</details>

## What does this bot has to offer?

- Essential Tools: This bot provides administrators with essential moderation tools, including muting, unmuting,
  kicking, warning, unwarning, banning, and unbanning.
- User Insights: It also allows admins to view a user's track record, showing their last activity, message count, and
  past punishments.
- Customization: Additionally, the bot supports customizable settings and filters, enabling automated text or media
  responses to users.

## Documentation for the bot

Check out [this](BOT_USAGE_DOCUMENTATION.md)!

## Running own instance of the bot

Before you start, you need a [bot token](https://t.me/BotFather) and a MongoDB ready.

1. Set up `Secrets.cs`, this file should be located in the `src` directory.
   <br>
   Then, paste this:
   ```csharp
    namespace ModeratorBot
    {
        public enum Server
        {
            Test,
            Production
        }

        public static class Secrets
        {
            // bot tokens, test token should be obtained in telegram test servers.
            // if you do not know where that is, then leave it empty, and just fill in PRODUCTION_TOKEN
            public const string TEST_TOKEN = "";
            public const string PRODUCTION_TOKEN = "";
    
            public static readonly Server SERVER = Server.Production;
    
            // database
            public const string DB_USERNAME = "";
            public const string DB_PASSWORD = "";
        }

    }

   ```
2. Create a database named "moderatorBot" in the MongoDB
3. The collections *should* be created by the bot automatically, if not, create them yourselves.