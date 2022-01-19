using Makao.Collections;
using Makao.Model;
using System;

namespace Makao.View
{
    public class ResponseWriter
    {
        protected static Func<GameResponse, string>[] SimpleFailResponses = BuildFailResponses();
        public GameWriter GameWriter { get; set; }

        public ResponseWriter(GameWriter gameWriter)
        {
            GameWriter = gameWriter;
        }

        private static Func<GameResponse, string>[] BuildFailResponses()
        {
            Func<GameResponse, string>[] failResponse = new Func<GameResponse, string>[Enum.GetValues<Result>().Length];
            SetSimpleFailResponse(Result.CantAskRank, "Nem kérhetsz most rangot");
            SetSimpleFailResponse(Result.CantAskSuit, "Nem kérhetsz most színt");
            SetSimpleFailResponse(Result.CantPass, "Nem lehet most passzolni");
            SetSimpleFailResponse(Result.CantThrowAllCardsAtOnce, "Nem lehet egyszerre letenni az összes kártyát");
            SetSimpleFailResponse(Result.CantThrowMoreCardsAtOnce, "Nem lehet egyszerre több kártyát dobni");
            SetSimpleFailResponse(Result.CardDoesNotExists, "Valamelyik kártya nem létezik a kiválasztott kártyák közül");
            SetSimpleFailResponse(Result.DuplicationInSelection, "Duplikáció van a kiválaszott kártyákban");
            SetSimpleFailResponse(Result.GameHasAlreadyEnded, "A játék már véget ért");
            SetSimpleFailResponse(Result.GameHasAlreadyStarted, "A játék már elindult");
            SetSimpleFailResponse(Result.GameHasNotStartedYet, "A játék még nem indult el");
            SetSimpleFailResponse(Result.GameIsFull, "A játék megtelt");
            SetSimpleFailResponse(Result.GameOutOfDrawableCards, "A játék kifogyott a húzható kártyákból");
            SetSimpleFailResponse(Result.MissATurn, "Kimaradsz a körből");
            SetSimpleFailResponse(Result.NotActivePlayer, "Nem te következel");
            SetSimpleFailResponse(Result.NotAllCardsAreTheSameRank, "Nem mindegyik kártya ugyanolyan rangú");
            SetSimpleFailResponse(Result.NotCounter, "Ezzel/ezekkel nem kontrázhatsz");
            SetSimpleFailResponse(Result.NotMemberOfTheGame, "Nem vagy tagja a játéknak");
            SetSimpleFailResponse(Result.NotRankRequest, "Nem rangkérés");
            SetSimpleFailResponse(Result.NotSameRankOrSuitOrSpecial, "Az első kártya nem ugyanolyan színű vagy rangú");
            SetSimpleFailResponse(Result.NotSuitRequest, "Nem színkérés");
            SetSimpleFailResponse(Result.PlayerIsAlreadyInGame, "Már játékban vagy");
            SetSimpleFailResponse
                (Result.RankAndSuitAreNotFitForModifiers, "Az első kártya színe és rangja nem stimmel érvényben lévő módosítók miatt");
            SetSimpleFailResponse(Result.RankIsNotFitForModifier, "Az első kártya rangja nem stimmel a rangmódosító miatt");
            SetSimpleFailResponse(Result.SuitIsNotFitForModifier, "Az első kártya színe nem stimmel a színmódosító miatt");
            SetSimpleFailResponse(Result.ThrowSelectionIsEmpty, "Nincsenek kiválasztott kártyák");
            return failResponse;

            void SetSimpleFailResponse(Result result, string message)
            {
                failResponse[(int)result] = (r) => $"{r.Request.Source.Name}! Sikertelen {GetRequestString(r)}! {message}!";
            }
        }

        private static string GetRequestString(GameResponse response)
        {
            if (response.Request is ThrowRequest)
            {
                return "dobás";
            }
            else if (response.Request is PassRequest)
            {
                return "passzolás";
            }
            else if (response.Request is DrawRequest)
            {
                return "húzás";
            }
            else if (response.Request is RankRequest)
            {
                return "rangkérés";
            }
            else if (response.Request is SuitRequest)
            {
                return "színkérés";
            }
            else if (response.Request is JoinRequest)
            {
                return "csatlakozás";
            }
            else if (response.Request is LeaveRequest)
            {
                return "kilépés";
            }
            else
            {
                return "#ERROR_REQUEST#";
            }
        }

        private bool RequestSourceIsHuman(GameResponse response)
        {
            return response.Request.Source is HumanPlayer;
        }

        private bool ThrowedCardsContainsReverseCard(SuccessfulThrowResponse response)
        {
            ReadOnlyDynamicArray<Card> cards = response.ThrowedCards;
            Rules rules = response.Game.Rules;

            return cards.Contains(
                (c) => rules.IsItActiveCard(c, out ActiveCardProperties props)
                && props.GameModifiers.HasFlag(GameModifier.Reverse),
                out int index);
        }

        private bool ThrowedCardsContainsSwapCard(SuccessfulThrowResponse response)
        {
            ReadOnlyDynamicArray<Card> cards = response.ThrowedCards;
            Rules rules = response.Game.Rules;

            return cards.Contains(
                (c) => rules.IsItActiveCard(c, out ActiveCardProperties props)
                && props.GameModifiers.HasFlag(GameModifier.SwapHands),
                out int index);
        }

        private void WriteKeywordsPunishment(SuccessfulThrowResponse response)
        {
            if (!response.Keywords.IsNullOrEmpty())
            {
                for (int i = 0; i < response.Keywords.Count; i++)
                {
                    Console.WriteLine
                        ($"Mivel elfelejtetted mondani, hogy '{response.Keywords[i].Word}', ezért büntetésből fel kellett húznod {response.Keywords[i].Penalty} kártyát!");
                }
                if (GameWriter.PublicWrite || RequestSourceIsHuman(response))
                {
                    ConsoleHelper.WriteLineCardsWithText("Büntetés kártyák: ", response.KeywordPunishment, GameWriter.Graphics);
                }
            }
        }

        private void WriteOtherSuccesfulResponses(GameResponse response)
        {
            string name = response.Request.Source.Name;
            string text = "";
            if (response.Request is SuitRequest suitRequest)
            {
                text = $"{name}! Színt kértél! Szín: {GameWriter.Graphics.Suits[suitRequest.Suit].Name}";
            }
            else if (response.Request is RankRequest rankRequest)
            {
                text = $"{name}! Rangot kértél! Rang: {GameWriter.Graphics.Ranks[rankRequest.Rank].Name}";
            }
            else if (response.Request is PassRequest)
            {
                text = $"{name}! Passzoltál!";
            }
            else if (response.Request is JoinRequest)
            {
                text = $"{name} csatlakozott!";
            }
            else if (response.Request is LeaveRequest)
            {
                text = $"{name} feladta!";
            }
            ConsoleHelper.WriteColorizedText(text + "\n", GameWriter.Settings.HighlightedColor);
        }

        private void WriteRankConditionFail(RankRequestConditionFailResponse response)
        {
            RankRequest request = (RankRequest)response.Request;
            string name = request.Source.Name;
            if (response.Condition.Relation == RankRequestCondition.RelationType.Equal)
            {
                Console.Write
                    ($"{name}! Csak akkor kérhetsz {GameWriter.Graphics.Ranks[request.Rank]}-t, ha pontosan {response.Condition.HandCount} kártyád van");
            }
            else if (response.Condition.Relation == RankRequestCondition.RelationType.Lower)
            {
                Console.Write
                    ($"{name}! Csak akkor kérhetsz {GameWriter.Graphics.Ranks[request.Rank]}-t, ha kevesebb, mint {response.Condition.HandCount} kártyád van");
            }
            else if (response.Condition.Relation == RankRequestCondition.RelationType.Greater)
            {
                Console.Write
                   ($"{name}! Csak akkor kérhetsz {GameWriter.Graphics.Ranks[request.Rank]}-t, ha több, mint {response.Condition.HandCount} kártyád van");
            }
        }

        private void WriteSuccessfulThrowResponse(SuccessfulThrowResponse response)
        {
            string name = response.Request.Source.Name;
            ThrowRequest request = response.Request as ThrowRequest;
            string text = $"{name}! Dobtál" + (request.Keywords.Count > 0 ? $" és azt mondtad, hogy \"{request.Keywords.Join(", ")}\"!" : "!");
            ConsoleHelper.WriteColorizedText(text, GameWriter.Settings.HighlightedColor);
            ConsoleHelper.WriteLineCardsWithText(" ", response.ThrowedCards, GameWriter.Graphics);

            WriteKeywordsPunishment(response);
            WriteThrowExchange(response);
            WriteThrowedCardsEffects(response);
        }

        private void WriteSucessfulDrawResponse(SuccessfulDrawResponse response)
        {
            string name = response.Request.Source.Name;

            ConsoleHelper.WriteColorizedText($"{name}! Felhúztál {response.DrawedCards.Count} kártyát! ", GameWriter.Settings.HighlightedColor);

            if (GameWriter.PublicWrite || RequestSourceIsHuman(response))
            {
                ConsoleHelper.WriteLineCardsWithText("", response.DrawedCards, GameWriter.Graphics);
            }
            else
            {
                Console.WriteLine();
            }
        }

        private void WriteThrowedCardsEffects(SuccessfulThrowResponse response)
        {
            DynamicArray<string> effects = new DynamicArray<string>();
            if (response.Game.TypeOfState == typeof(WaitForSuitState))
            {
                effects.Add("Színt kérhetsz!");
            }
            else if (response.Game.TypeOfState == typeof(WaitForRankState))
            {
                effects.Add("Rangot kérhetsz!");
            }

            if (ThrowedCardsContainsReverseCard(response))
            {
                effects.Add("A kör megfordult!");
            }

            if (ThrowedCardsContainsSwapCard(response))
            {
                effects.Add("Játékosok kártyái felcserélődtek!");
            }
            if (effects.Count > 0)
            {
                Console.WriteLine(effects.Join("\n"));
            }
        }

        private void WriteThrowExchange(SuccessfulThrowResponse response)
        {
            if (!response.ThrowExchange.IsNullOrEmpty())
            {
                Console.WriteLine
                    ($"Pontosan {response.ThrowedCards.Count} kártyát dobtál, ezért cserébe fel kellett húznod {response.ThrowExchange.Count} kártyát!");
                if (GameWriter.PublicWrite || RequestSourceIsHuman(response))
                {
                    ConsoleHelper.WriteLineCardsWithText("Csere kártyák: ", response.ThrowExchange, GameWriter.Graphics);
                }
            }
        }

        public void Write(GameResponse response)
        {
            if (response is SuccessfulThrowResponse thrw)
            {
                WriteSuccessfulThrowResponse(thrw);
            }
            else if (response is SuccessfulDrawResponse draw)
            {
                WriteSucessfulDrawResponse(draw);
            }
            else if (response is RankRequestConditionFailResponse conditionFail)
            {
                WriteRankConditionFail(conditionFail);
            }
            else if (response.Result == Result.Success)
            {
                WriteOtherSuccesfulResponses(response);
            }
            else
            {
                Console.WriteLine(SimpleFailResponses[(int)response.Result](response));
            }
        }
    }
}