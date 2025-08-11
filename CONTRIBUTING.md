# Contributing

First of all, if you are interested in contributing to the project - **THANK YOU!**

## Ways to Contribute

- Report bugs via [issues](https://github.com/koteyevich/ModeratorBot/issues)
- Suggest features or improvements via [issues](https://github.com/koteyevich/ModeratorBot/issues)
- Submit pull requests for fixes or enhancements
- Improve documentation (XML comments, README, bot usage)

## Development Setup

You will need a [bot token](https://t.me/BotFather) and a MongoDB ready.

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

## Coding Standards

- The [.editorconfig](https://github.com/koteyevich/ModeratorBot/blob/master/.editorconfig) is in the project, please
  follow it.
- Write clear, descriptive commit messages. (don't be like me, please!)
- Document new functions and classes, especially public ones, using XML comments.
- Avoid magic numbers and strings—use constants or enums instead.
- Prefer explicit access modifiers (public, private, etc.).
- Use exception handling where appropriate, but avoid empty catch blocks.

## Pull Request Process

1. Fork and clone the repo.
2. Create a new branch: `git checkout -b my-feature`
3. Make your changes.
4. Submit a pull request with a clear description. (don't be like me, please!)

## Reporting Bugs / Requesting Features

- Use the [issue tracker](https://github.com/koteyevich/ModeratorBot/issues)
- Provide as much detail as possible (steps to reproduce, screenshots, etc.)

---
Thank you, and thank you again for helping the project!