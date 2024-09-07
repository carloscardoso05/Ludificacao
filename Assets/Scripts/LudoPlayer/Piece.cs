using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Managers;
using Newtonsoft.Json;
using Photon.Pun;
using UnityEngine;

namespace LudoPlayer
{
    /// <summary>
    ///     Peça do jogo.
    /// </summary>
    [RequireComponent(typeof(PieceVisual))]
    [RequireComponent(typeof(PhotonView))]
    public class Piece : MonoBehaviour
    {
        /// <summary>
        ///     Representação visual desta peça.
        /// </summary>
        public PieceVisual visual;

        /// <summary>
        ///     Cor da peça.
        /// </summary>
        /// <seealso cref="GameColor" />
        public GameColor color;

        /// <summary>
        ///     Id da peça (0, 1, 2 ou 3).
        /// </summary>
        public byte id;

        /// <summary>
        ///     Player dono desta peça.
        /// </summary>
        public Player player;

        /// <summary>
        ///     Se a peça está na sua casa de origem.
        /// </summary>
        public bool inHome = true;

        /// <summary>
        ///     Caminho desta peça.
        /// </summary>
        public NavigationList<Tile> Path;

        private void Start()
        {
            GetComponent<PhotonView>();
            PieceMoved += SendOthersToHome;
            QuizManager.Instance.OnAnswered += MovePieceOnAnswer;
            MoveToHome();
        }

        /// <summary>
        ///     Quando esta peça é clicada e alguma peça na mesma casa que esta possa se mover (podendo ser esta mesma),
        ///     exibe uma questão.
        /// </summary>
        private void OnMouseUp()
        {
            if (GameManager.Instance.state != GameState.SelectingPiece) return;
            if (!PhotonNetwork.OfflineMode)
            {
                var localColor = (GameColor)PhotonNetwork.LocalPlayer.CustomProperties["Color"];
                var isNotThisPlayerTurn = ColorsManager.I.currentColor != localColor;
                if (isNotThisPlayerTurn && !PhotonNetwork.OfflineMode) return;
            }

            if (CanMove(this))
            {
                var data = new GameManager.ExtraData
                    { selectedPiece = this, player = player, diceValue = GameManager.Instance.dice.Value };
                QuizManager.Instance.ShowRandomQuestion(data, color);
                return;
            }

            foreach (var p in Path.Current.pieces)
                if (CanMove(p))
                {
                    var data = new GameManager.ExtraData
                        { selectedPiece = p, player = p.player, diceValue = GameManager.Instance.dice.Value };
                    QuizManager.Instance.ShowRandomQuestion(data, p.color);
                    break;
                }
        }

        /// <summary>
        ///     Evento disparado quando uma peça se move.
        /// </summary>
        public event EventHandler<PieceMovedArgs> PieceMoved;

        /// <summary>
        ///     Executa a lógica de mover a peça quando uma pergunta é respondida.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="answerData">Informações da resposta.</param>
        private void MovePieceOnAnswer(object sender, AnswerData answerData)
        {
            var extraData = (GameManager.ExtraData)answerData.extraData;
            var isNotThisPiece = extraData.selectedPiece.name != name;
            var isNotCorrectAnswer = !answerData.selectedAnswer.correct && answerData.selectedAnswer.id != "0"; // TODO
            if (isNotThisPiece) return;
            if (isNotCorrectAnswer && !GameManager.Instance.alwaysAnswerRight) return;
            var times = extraData.diceValue + GameSettings.Instance.GetDifficultyBonus(answerData.question.difficulty);
            Move(times);
            var piecesPositions = new Dictionary<GameColor, int[]>
            {
                { GameColor.Blue, new[] { 0, 0, 0, 0 } },
                { GameColor.Red, new[] { 0, 0, 0, 0 } },
                { GameColor.Green, new[] { 0, 0, 0, 0 } },
                { GameColor.Yellow, new[] { 0, 0, 0, 0 } }
            };
            var piecesInHome = new Dictionary<GameColor, bool[]>
            {
                { GameColor.Blue, new[] { true, true, true, true } },
                { GameColor.Red, new[] { true, true, true, true } },
                { GameColor.Green, new[] { true, true, true, true } },
                { GameColor.Yellow, new[] { true, true, true, true } }
            };
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("PiecesInHome", out var piecesInHomeObj))
                piecesInHome = JsonConvert.DeserializeObject<Dictionary<GameColor, bool[]>>((string)piecesInHomeObj);

            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("PiecesPositions", out var piecesPositionsObj))
                piecesPositions =
                    JsonConvert.DeserializeObject<Dictionary<GameColor, int[]>>((string)piecesPositionsObj);

            piecesPositions[color][id] = Path.CurrentIndex + (inHome ? times - 1 : times);
            piecesInHome[color][id] = false;
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable
            {
                { "PiecesPositions", JsonConvert.SerializeObject(piecesPositions) },
                { "PiecesInHome", JsonConvert.SerializeObject(piecesInHome) }
            });
        }

        /// <summary>
        ///     Muda a localização da peça em seu caminho e inicia a animação do movimento.
        /// </summary>
        /// <param name="times">Quantidade de casas que a peça deve andar.</param>
        public void Move(int times)
        {
            var tilesToMove = inHome ? times - 1 : times;
            var prevIndex = Path.CurrentIndex;
            Path.Current.pieces.Remove(this);
            Path.CurrentIndex += tilesToMove;
            Path.Current.pieces.Add(this);
            var tilesMoved = Path.CurrentIndex - prevIndex;
            PieceMovedArgs args = new()
            {
                Piece = this,
                CurrTile = Path.Current,
                TilesMoved = tilesMoved,
            };
            inHome = false;
            visual.StartAnimation(args);
        }

        /// <summary>
        ///     Envia outras peças para suas casas de origem quando esta peça termina de se mover.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void SendOthersToHome(object sender, EventArgs args)
        {
            if (Path.Current.isSafe) return;
            var copyPieces = new Piece[Path.Current.pieces.Count];
            Path.Current.pieces.CopyTo(copyPieces);
            foreach (var p in copyPieces)
                if (p.color != color)
                    p.MoveToHome();
        }

        /// <summary>
        ///     Envia esta peça para sua casa de origem.
        /// </summary>
        public void MoveToHome()
        {
            Path.Current.pieces.Remove(this);
            inHome = true;
            Path.CurrentIndex = 0;
            visual.SendHome();
            var piecesPositions = new Dictionary<GameColor, int[]>
            {
                { GameColor.Blue, new[] { 0, 0, 0, 0 } },
                { GameColor.Red, new[] { 0, 0, 0, 0 } },
                { GameColor.Green, new[] { 0, 0, 0, 0 } },
                { GameColor.Yellow, new[] { 0, 0, 0, 0 } }
            };
            var piecesInHome = new Dictionary<GameColor, bool[]>
            {
                { GameColor.Blue, new[] { true, true, true, true } },
                { GameColor.Red, new[] { true, true, true, true } },
                { GameColor.Green, new[] { true, true, true, true } },
                { GameColor.Yellow, new[] { true, true, true, true } }
            };
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("PiecesInHome", out var piecesInHomeObj))
                piecesInHome = JsonConvert.DeserializeObject<Dictionary<GameColor, bool[]>>((string)piecesInHomeObj);

            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("PiecesPositions", out var piecesPositionsObj))
                piecesPositions =
                    JsonConvert.DeserializeObject<Dictionary<GameColor, int[]>>((string)piecesPositionsObj);

            piecesPositions[color][id] = 0;
            piecesInHome[color][id] = true;

            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable
            {
                { "PiecesPositions", JsonConvert.SerializeObject(piecesPositions) },
                { "PiecesInHome", JsonConvert.SerializeObject(piecesInHome) }
            });
        }

        /// <summary>
        ///     Verifica se a peça pode se mover.
        /// </summary>
        /// <param name="piece">Peça alvo.</param>
        /// <returns>Se pode se mover.</returns>
        private static bool CanMove(Piece piece)
        {
            var isThisPieceTurn = ColorsManager.I.currentColor == piece.color;
            var isSelectingPiece = GameManager.Instance.state == GameState.SelectingPiece;
            var isNotFinalTile = !piece.Path.Current.isFinal;
            return isThisPieceTurn && isSelectingPiece && isNotFinalTile;
        }

        /// <summary>
        ///     Envia o evento <see cref="PieceMoved" />.
        /// </summary>
        /// <param name="args"></param>
        public void OnPieceMoved(PieceMovedArgs args)
        {
            PieceMoved?.Invoke(this, args);
        }

        /// <summary>
        ///     Informações para disparar o evento quando a peça se move.
        /// </summary>
        public class PieceMovedArgs : EventArgs
        {
            public Tile CurrTile;
            public Piece Piece;
            public int TilesMoved;
        }
    }
}