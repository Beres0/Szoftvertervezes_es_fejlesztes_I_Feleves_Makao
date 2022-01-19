using System;

namespace Makao.Model
{
    public class GameResponse
    {
        public Game Game { get; }
        public PlayerRequest Request { get; }
        public Result Result { get; }

        public GameResponse(Game game, PlayerRequest request, Result result)
        {
            Game = game;
            Request = request ?? throw new ArgumentNullException(nameof(request));
            Result = result;
        }

        public bool SentByGame()
        {
            return Game.Auth == this;
        }
    }
}