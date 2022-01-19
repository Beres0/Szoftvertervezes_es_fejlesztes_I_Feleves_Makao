using Makao.Collections;
using System;

namespace Makao.Model
{
    public partial class Game
    {
        private Random rnd;
        protected readonly DynamicArray<Player> leavers;
        protected readonly DynamicArray<PlayerStatus> players;
        protected readonly PostGameState postGame;
        protected readonly PreGameState preGame;
        protected readonly WaitForDecisionState waitForDecision;
        protected readonly WaitForRankState waitForRank;
        protected readonly WaitForSuitState waitForSuit;
        protected readonly DynamicArray<Player> winners;
        protected DynamicArray<Card> discardPile;
        protected DynamicArray<Card> drawPile;
        protected GameState state;
        private Hand ActivePlayerHand => players[ActivePlayerIndex].Hand;
        public Player ActivePlayer => state is ActiveState ? players[ActivePlayerIndex].Player : null;
        public int ActivePlayerIndex { get; private set; }
        public int ActivePlayersCount => players.Count - leavers.Count - winners.Count;

        public int ActualDrawCount
        {
            get
            {
                if (Punishment is DrawExtraCards drawExtra)
                {
                    return drawExtra.Stack <= MaxDrawableCardsCount ? drawExtra.Stack : MaxDrawableCardsCount;
                }
                else
                {
                    return 1;
                }
            }
        }

        public Ask Ask { get; protected set; }
        public GameResponse Auth { get; private set; }
        public RequestType AvaliableRequests => state.AvaliableRequests;

        public int DefaultModifierDuration
        {
            get
            {
                switch (Rules.ThrowModifierDuration)
                {
                    case ThrowModifierDuration.Turn: return ActivePlayersCount;
                    case ThrowModifierDuration.NextPlayer: return 1;
                    default: return 0;
                }
            }
        }

        public GameDirection Direction { get; protected set; }
        public ReadOnlyDynamicArray<Card> DiscardPile { get; private set; }
        public ReadOnlyDynamicArray<Card> DrawPile { get; private set; }
        public bool IsEndOfGame => state is PostGameState;
        public GameResponse LastResponse { get; private set; }
        public ReadOnlyDynamicArray<Player> Leavers { get; }
        public int MaxDrawableCardsCount => discardPile.Count - 1 + drawPile.Count;

        public int NextPlayerIndex
        {
            get
            {
                if (state is ActiveState)
                {
                    int nextIndex = ActivePlayerIndex;
                    do

                    {
                        nextIndex = nextIndex + (int)Direction;
                        if (nextIndex < 0)
                        {
                            nextIndex = players.Count - 1;
                        }
                        else if (nextIndex > players.Count - 1)
                        {
                            nextIndex = 0;
                        }
                    } while (players[nextIndex].Status != PlayerStatus.Type.Playing);

                    return nextIndex;
                }
                else return 0;
            }
        }

        public int PlayersCount => players.Count;
        public Punishment Punishment { get; protected set; }
        public RankModifier RankModifier { get; protected set; }
        public Rules Rules { get; }
        public int Seed { get; private set; }
        public SuitModifier SuitModifier { get; protected set; }
        public ThrowValidator ThrowValidator { get; }

        public Card? TopOfDiscardPile
        {
            get
            {
                if (discardPile != null && discardPile.TryGetLastItem(out Card lastItem))
                {
                    return lastItem;
                }
                else return null;
            }
        }

        public Type TypeOfState => state.GetType();
        public ReadOnlyDynamicArray<Player> Winners { get; }

        public Game(Rules rules, int seed)
        {
            Rules = rules;
            Seed = seed;
            rnd = new Random(Seed);

            players = new DynamicArray<PlayerStatus>();
            winners = new DynamicArray<Player>();
            Winners = winners.AsReadOnly();
            leavers = new DynamicArray<Player>();
            Leavers = leavers.AsReadOnly();

            preGame = new PreGameState(this);
            postGame = new PostGameState(this);
            waitForDecision = new WaitForDecisionState(this);
            waitForRank = new WaitForRankState(this);
            waitForSuit = new WaitForSuitState(this);

            state = preGame;
            Direction = GameDirection.Normal;
            ThrowValidator = new ThrowValidator(this);
        }

        public Game(Rules rules) : this(rules, new Random().Next(int.MaxValue))
        {
        }

        private void ActivateGameModifiers(ActiveCardProperties props)
        {
            if (props.GameModifiers.HasFlag(GameModifier.Reverse))
            {
                ReverseTurn();
            }
            if (props.GameModifiers.HasFlag(GameModifier.SwapHands))
            {
                SwapHands();
            }
        }

        private void ActivateThrowedCards(ReadOnlyDynamicArray<Card> throwedCards)
        {
            Rules.IsItActiveCard(throwedCards[throwedCards.Count - 1], out ActiveCardProperties lastProps);
            int missATurnStack = 0;
            int drawExtraStack = 0;
            for (int i = 0; i < throwedCards.Count; i++)
            {
                if (Rules.IsItActiveCard(throwedCards[i], out ActiveCardProperties props))
                {
                    ActivateGameModifiers(props);
                    if (props.Punishment is MissATurn)
                    {
                        missATurnStack += props.Punishment.Stack;
                    }
                    else if (props.Punishment is DrawExtraCards)
                    {
                        drawExtraStack += props.Punishment.Stack;
                    }
                }
            }

            if (Rules.Settings.HasFlag(RulesSettings.AllowPunishmentStacking))
            {
                if (Punishment != null)
                {
                    if (Punishment is MissATurn)
                    {
                        Punishment = Punishment.Increase(missATurnStack);
                    }
                    else if (Punishment is DrawExtraCards)
                    {
                        Punishment = Punishment.Increase(drawExtraStack);
                    }
                }
                else if (lastProps != null)
                {
                    if (lastProps.Punishment is MissATurn)
                    {
                        Punishment = new MissATurn(missATurnStack);
                    }
                    else if (lastProps.Punishment is DrawExtraCards)
                    {
                        Punishment = new DrawExtraCards(drawExtraStack);
                    }
                }
            }
            else if (lastProps != null)
            {
                Punishment = lastProps.Punishment;
            }

            if (lastProps != null)
            {
                Ask = lastProps.Ask;
            }
        }

        private void ActivePlayerDraw(ReadOnlyDynamicArray<Card> cards)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                players[ActivePlayerIndex].Hand.ForGame.Add(cards[i]);
            }
        }

        private void ActivePlayerThrow(ThrowRequest request, out ReadOnlyDynamicArray<Card> throwedCards)
        {
            DynamicArray<Card> cards = new DynamicArray<Card>();

            for (int i = 0; i < request.Selection.Count; i++)
            {
                cards.Add(ActivePlayerHand[request.Selection[i]]);
            }

            for (int i = 0; i < cards.Count; i++)
            {
                ActivePlayerHand.ForGame.Remove(cards[i]);
                discardPile.Add(cards[i]);
            }
            throwedCards = cards.AsReadOnly();
        }

        private bool AddPlayer(Player player, out Func<ReadOnlyDynamicArray<Card>> hand)
        {
            hand = null;
            if (players.Count <= Rules.MaxPlayer)
            {
                Hand starterHand = new Hand();

                PlayerStatus status = new PlayerStatus(starterHand, player, PlayerStatus.Type.Waiting);
                hand = () => status.Hand.ForPlayer;
                players.Add(status);
                return true;
            }
            else return false;
        }

        private void AddPlayerToWinners(int index)
        {
            if (players[index].Status == PlayerStatus.Type.Playing)
            {
                players[index].Status = PlayerStatus.Type.Winner;
                winners.Add(players[index].Player);
            }
        }

        private void CreatePiles()
        {
            drawPile = Rules.GetDrawPile(players.Count);
            DrawPile = drawPile.AsReadOnly();
            ShuffleCards(drawPile);

            discardPile = new DynamicArray<Card>();
            DiscardPile = discardPile.AsReadOnly();
            if (drawPile.TryPopLast(out Card item))
            {
                discardPile.Add(item);
            }
        }

        private void DealCards()
        {
            for (int i = 0; i < Rules.StarterHandSize; i++)
            {
                for (int j = 0; j < players.Count; j++)
                {
                    if (drawPile.TryPopLast(out Card card))
                    {
                        players[j].Hand.ForGame.Add(card);
                    }
                }
            }
        }

        private void DecreaseThrowModifiers()
        {
            if (RankModifier != null)
            {
                RankModifier = RankModifier.Decrease();
            }
            if (SuitModifier != null)
            {
                SuitModifier = SuitModifier.Decrease();
            }
        }

        private void EraseThrowModifiers()
        {
            SuitModifier = null;
            RankModifier = null;
        }

        private void GiveCardsIfForgotToSayKeyword
          (int countBeforeThrow, ThrowRequest request,
           out ReadOnlyDynamicArray<Keyword> keywords,
           out ReadOnlyDynamicArray<Card> penalties)
        {
            DynamicArray<Card> finalPens = new DynamicArray<Card>();
            DynamicArray<Keyword> forgottens = new DynamicArray<Keyword>();
            if (Rules.NeedKeywords(countBeforeThrow, request.Source.Hand.Count, out ReadOnlyDynamicArray<Keyword> kws))
            {
                for (int i = 0; i < kws.Count; i++)
                {
                    if (!request.Keywords.Contains(kws[i].Word, out int index))
                    {
                        if (TryDrawCards(kws[i].Penalty, out ReadOnlyDynamicArray<Card> actualPens))
                        {
                            for (int j = 0; j < actualPens.Count; j++)
                            {
                                finalPens.Add(actualPens[j]);
                            }
                        }

                        forgottens.Add(kws[i]);
                    }
                }
            }

            penalties = finalPens.AsReadOnly();
            if (!penalties.IsEmpty)
            {
                ActivePlayerDraw(penalties);
            }

            keywords = forgottens.AsReadOnly();
        }

        private void GiveCardsIfHasThrowingExchange(int throwCount, out ReadOnlyDynamicArray<Card> exchange)
        {
            exchange = ReadOnlyDynamicArray<Card>.Empty;
            if (Rules.HasThrowingExchange(throwCount, out int exch))
            {
                if (TryDrawCards(exch, out exchange))
                {
                    ActivePlayerDraw(exchange);
                }
            }
        }

        private void HandleEffects(GameResponse lastSuccesfulResponse)
        {
            if (lastSuccesfulResponse.Result != Result.Success)
            {
                throw new ArgumentException("Result is not succes!", nameof(lastSuccesfulResponse));
            }

            if (lastSuccesfulResponse is SuccessfulThrowResponse throwResp)
            {
                if (Rules.ThrowModifierDuration == ThrowModifierDuration.NextValidThrow)
                {
                    EraseThrowModifiers();
                }

                ActivateThrowedCards(throwResp.ThrowedCards);
            }
            else
            {
                if (lastSuccesfulResponse is SuccessfulDrawResponse)
                {
                    if (Punishment is DrawExtraCards)
                    {
                        Punishment = null;
                    }
                }
                else if (lastSuccesfulResponse.Request is PassRequest)
                {
                    if (Punishment is MissATurn miss)
                    {
                        Punishment = miss.Decrease();
                    }
                    else if (Punishment is DrawExtraCards)
                    {
                        Punishment = null;
                    }
                }
                else if (lastSuccesfulResponse.Request is RankRequest rank)
                {
                    SetRankModifier(rank.Rank);
                }
                else if (lastSuccesfulResponse.Request is SuitRequest suit)
                {
                    SetSuitModifier(suit.Suit);
                }
                else if (lastSuccesfulResponse.Request is LeaveRequest)
                {
                    if (state is WaitForRankState)
                    {
                        SetRankModifier(rnd.Next(Rules.Deck.NumberOfRanks));
                    }
                    else if (state is WaitForSuitState)
                    {
                        SetSuitModifier(rnd.Next(Rules.Deck.NumberOfSuits));
                    }
                }
            }

            if (Rules.ThrowModifierDuration != ThrowModifierDuration.NextValidThrow)
            {
                DecreaseThrowModifiers();
            }
        }

        private void HandleWinners()
        {
            if (Ask == Ask.None)
            {
                if (!Rules.Settings.HasFlag(RulesSettings.PlayersCanBeCalledBack))
                {
                    if (ActivePlayer.Hand.Count == 0)
                    {
                        AddPlayerToWinners(ActivePlayerIndex);
                    }
                }
                else
                {
                    int next = NextPlayerIndex;
                    if (GetHand(next).Count == 0 && (Punishment is not DrawExtraCards || MaxDrawableCardsCount == 0))
                    {
                        AddPlayerToWinners(NextPlayerIndex);
                        HandleWinners();
                    }
                }

                if (ActivePlayersCount == 1 && winners.Count == 0)
                {
                    AddPlayerToWinners(NextPlayerIndex);
                }
            }
        }

        private void NextState(GameResponse lastSuccesfulResponse)
        {
            if (state is ActiveState)
            {
                HandleEffects(lastSuccesfulResponse);
                HandleWinners();
                Transition();
            }
        }

        private void RefillDrawPile()
        {
            if (discardPile.TryPopLast(out Card last))
            {
                ShuffleCards(discardPile);

                while (DiscardPile.Count > 0)
                {
                    if (discardPile.TryPopLast(out Card item))
                    {
                        drawPile.Append(item);
                    }
                }

                discardPile.Add(last);
            }
        }

        private void RemovePlayer(int index)
        {
            Hand hand = players[index].Hand;
            ShuffleCards(hand.ForGame);

            while (hand.ForGame.Count > 0)
            {
                if (hand.ForGame.TryPopLast(out Card item))
                {
                    drawPile.Append(item);
                }
            }

            if (state is not PreGameState)
            {
                players[index].Status = PlayerStatus.Type.Leaver;
                leavers.Add(players[index].Player);
            }
            else
            {
                players.RemoveAt(index);
            }
        }

        private void ReverseTurn()
        {
            Direction = (GameDirection)(-(int)Direction);
        }

        private void Send(Player player, GameResponse response)
        {
            LastResponse = response;
            Auth = response;
            player.Recieve(response);
            Auth = null;
        }

        private void SendBack(GameResponse response)
        {
            Send(response.Request.Source, response);
        }

        private void SetRankModifier(int rank)
        {
            Ask = Ask.None;
            RankModifier = new RankModifier(DefaultModifierDuration, rank);
        }

        private void SetSuitModifier(int suit)
        {
            Ask = Ask.None;
            SuitModifier = new SuitModifier(DefaultModifierDuration, suit);
        }

        private void ShuffleCards(DynamicArray<Card> cards)
        {
            for (int n = 0; n < 2; n++)
            {
                for (int i = cards.Count - 1; i > 0; i--)
                {
                    int j = rnd.Next(i);
                    if (i != j)
                    {
                        Card temp = cards[i];
                        cards[i] = cards[j];
                        cards[j] = temp;
                    }
                }
            }
        }

        private void SwapHands()
        {
            if (Direction == GameDirection.Normal) SwapHandsNormalOrder();
            else SwapHandsReverseOrder();
        }

        private void SwapHandsReverseOrder()
        {
            Hand first = null;
            int prevIndex = -1;

            for (int i = 0; i < players.Count - 1; i++)
            {
                if (players[i].Status == PlayerStatus.Type.Playing)
                {
                    if (first == null)
                    {
                        first = players[i].Hand;
                        prevIndex = i;
                    }
                    else
                    {
                        players[prevIndex].Hand = players[i].Hand;
                        prevIndex = i;
                    }
                }
            }
            players[prevIndex].Hand = first;
        }

        private void Transition()
        {
            if (ActivePlayersCount < 2
               || (winners.Count > 0
               && Rules.Settings.HasFlag(RulesSettings.GameEndsWithTheFirstWinner)))
            {
                state = postGame;
            }
            else
            {
                if (Ask == Ask.Rank)
                {
                    state = waitForRank;
                }
                else if (Ask == Ask.Suit)
                {
                    state = waitForSuit;
                }
                else
                {
                    ActivePlayerIndex = NextPlayerIndex;
                    state = waitForDecision;
                }
            }
        }

        private bool TryDrawCards(int count, out ReadOnlyDynamicArray<Card> cards)
        {
            if (MaxDrawableCardsCount != 0)
            {
                if (drawPile.Count < count)
                {
                    RefillDrawPile();
                }
                DynamicArray<Card> poppedCards = new DynamicArray<Card>();

                for (int i = 0; i < count; i++)
                {
                    if (drawPile.TryPopLast(out Card item))
                    {
                        poppedCards.Add(item);
                    }
                }
                cards = poppedCards.AsReadOnly();

                return true;
            }
            else
            {
                cards = ReadOnlyDynamicArray<Card>.Empty;
                return false;
            }
        }

        public bool ContainsPlayer(Player player, out int index)
        {
            int i = 0;
            while (i < players.Count && player != players[i].Player)
            {
                i++;
            }
            index = i;
            return index < players.Count;
        }

        public ReadOnlyDynamicArray<Card> GetHand(int index)
        {
            return players[index].Hand.ForPlayer;
        }

        public Player GetPlayer(int index)
        {
            return players[index].Player;
        }

        public PlayerStatus.Type GetPlayerStatus(int index)
        {
            return players[index].Status;
        }

        public void Recieve(PlayerRequest request)
        {
            if (request.SentByTheSource())
            {
                state.Recieve(request);
            }
        }

        public void StartTheGame()
        {
            if (players.Count < 2)
            {
                throw new InvalidOperationException("You can start with at least two players!");
            }
            else if (state is PreGameState)
            {
                CreatePiles();
                DealCards();

                ActivePlayerIndex = rnd.Next(ActivePlayersCount);
                state = waitForDecision;

                for (int i = 0; i < players.Count; i++)
                {
                    players[i].Status = PlayerStatus.Type.Playing;
                }
            }
        }

        public void SwapHandsNormalOrder()
        {
            Hand last = null;
            int nextIndex = -1;

            for (int i = players.Count - 1; i >= 0; i--)
            {
                if (players[i].Status == PlayerStatus.Type.Playing)
                {
                    if (last == null)
                    {
                        last = players[i].Hand;
                        nextIndex = i;
                    }
                    else
                    {
                        players[nextIndex].Hand = players[i].Hand;
                        nextIndex = i;
                    }
                }
            }
            players[nextIndex].Hand = last;
        }
    }
}