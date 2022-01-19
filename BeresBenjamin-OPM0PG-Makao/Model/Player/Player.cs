using Makao.Collections;
using System;

namespace Makao.Model
{
    public abstract class Player
    {
        private Func<ReadOnlyDynamicArray<Card>> hand;
        public PlayerRequest Auth { get; private set; }
        public Game Game { get; private set; }

        public ReadOnlyDynamicArray<Card> Hand
        {
            get
            {
                return IsInGame ? hand() : ReadOnlyDynamicArray<Card>.Empty;
            }
        }

        public bool IsInGame => Game != null;
        public GameResponse LastResponse { get; private set; }
        public string Name { get; }

        public Player(string name)
        {
            Name = name;
        }

        protected void CheckIsIngame()
        {
            if (!IsInGame)
            {
                throw new InvalidOperationException("You are not in game!");
            }
        }

        protected void Send(Game game, PlayerRequest request)
        {
            Auth = request;
            game.Recieve(request);
            Auth = null;
        }

        public void Join(Game game)
        {
            Send(game, new JoinRequest(this));
        }

        public void Leave()
        {
            CheckIsIngame();
            Send(Game, new LeaveRequest(this));
        }

        public void Recieve(GameResponse response)
        {
            if (response.Request.Source == this)
            {
                if (response is SuccesfulJoinResponse joinResp)
                {
                    Game = response.Game;
                    hand = joinResp.Hand;
                }
                else if (Game.Auth == response && response.Request is LeaveRequest)
                {
                    Game = null;
                    hand = null;
                }
                LastResponse = response;
            }
        }
    }
}