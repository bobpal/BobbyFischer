using BobbyFischer.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using Trinet.Core.IO.Ntfs;

//the back-end where all the business logic is determined

namespace BobbyFischer
{
    [Serializable]
    public class Chess
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Board());
        }

        public piece[,] board;                                      //8x8 array of pieces
        public Board mForm;                                         //main form
        public bool onePlayer;                                      //versus computer or human
        public string compTeam;                                     //color of computer team
        public string offensiveTeam;                                //which side is on the offense
        public string baseOnBottom;                                 //which side is currently on bottom, going up
        public bool medMode;                                        //difficulty level
        public bool hardMode;                                       //difficulty level
        public bool firstGame;                                      //has a game been setup yet?
        private coordinate prevSelected;                            //where the cursor clicked previously
        private coordinate toCoor;
        private coordinate fromCoor;
        public List<Assembly> themeList;
        public int themeIndex;                                      //which theme is currently in use
        public int tick;
        public Image lKing;
        public Image lQueen;
        public Image lBishop;
        public Image lKnight;
        public Image lRook;
        private Image lPawn;
        private Image dKing;
        public Image dQueen;
        public Image dBishop;
        public Image dKnight;
        public Image dRook;
        private Image dPawn;

        public Stack<historyNode> history = new Stack<historyNode>();   //stores all moves on a stack
        public bool gameOverExit = false;                               //Did player exit from game over screen?
        public bool lastMove = true;                                    //is lastMove menu option checked?
        public bool saveGame = true;                                    //Save game on exit?
        public bool rotate = true;                                      //Rotate board between turns on 2Player mode?
        public bool movablePieceSelected = false;                       //if true, the next click will move the selected piece if possible
        private List<move> possible = new List<move>();                 //list of all possible moves
        public string dirPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\BobbyFischer";
        public string filePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\BobbyFischer\\save.chess";
        private static Random rnd = new Random();

        public Chess(Board mainForm)
        {
            this.mForm = mainForm;
        }

        public void createGrid()
        {
            //creates new array with pieces in starting position

            string defensiveTeam;
            board = new piece[8, 8];

            if(offensiveTeam == "light")
            {
                defensiveTeam = "dark";
            }
            else
            {
                defensiveTeam = "light";
            }
                
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (y == 0)
                    {
                        board[x, 0].color = offensiveTeam;
                    }

                    else if (y == 1)
                    {
                        board[x, 1].color = offensiveTeam;
                        board[x, 1].job = "Pawn";
                        board[x, 1].firstMove = true;
                    }

                    else if (y == 6)
                    {
                        board[x, 6].color = defensiveTeam;
                        board[x, 6].job = "Pawn";
                        board[x, 6].firstMove = true;
                    }

                    else if(y == 7)
                    {
                        board[x, 7].color = defensiveTeam;
                    }

                    else
                    {
                        board[x, y].color = null;
                        board[x, y].job = null;
                    }
                }
            }

            board[0, 0].job = "Rook";
            board[1, 0].job = "Knight";
            board[2, 0].job = "Bishop";
            board[3, 0].job = "Queen";
            board[4, 0].job = "King";
            board[5, 0].job = "Bishop";
            board[6, 0].job = "Knight";
            board[7, 0].job = "Rook";
            board[0, 7].job = "Rook";
            board[1, 7].job = "Knight";
            board[2, 7].job = "Bishop";
            board[3, 7].job = "Queen";
            board[4, 7].job = "King";
            board[5, 7].job = "Bishop";
            board[6, 7].job = "Knight";
            board[7, 7].job = "Rook";

            board[0,0].firstMove = true;
            board[4,0].firstMove = true;
            board[7,0].firstMove = true;
            board[0,7].firstMove = true;
            board[4,7].firstMove = true;
            board[7,7].firstMove = true;
        }

        private List<coordinate> getDarkPieces()
        {
            //searches through board and returns list of coordinates where all dark pieces are located

            coordinate temp = new coordinate();
            List<coordinate> possiblePieces = new List<coordinate>();

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (board[x, y].color == "dark")
                    {
                        temp.x = x;
                        temp.y = y;
                        possiblePieces.Add(temp);
                    }
                }
            }
            return possiblePieces;
        }

        private List<coordinate> getLightPieces()
        {
            //searches through board and returns list of coordinates where all light pieces are located

            coordinate temp = new coordinate();
            List<coordinate> possiblePieces = new List<coordinate>();

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (board[x, y].color == "light")
                    {
                        temp.x = x;
                        temp.y = y;
                        possiblePieces.Add(temp);
                    }
                }
            }
            return possiblePieces;
        }

        private List<coordinate> getAllPieces()
        {
            //searches through board and returns list of coordinates where all pieces are located

            coordinate temp = new coordinate();
            List<coordinate> possiblePieces = new List<coordinate>();

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (board[x, y].color != null)
                    {
                        temp.x = x;
                        temp.y = y;
                        possiblePieces.Add(temp);
                    }
                }
            }
            return possiblePieces;
        }

        private List<move> getMoves(coordinate spot)
        {
            //returns all possible moves of spot given in argument disregarding check restrictions
            //determines job of piece and calls apropriate function to get correct move list

            List<move> temp = new List<move>();

            switch (board[spot.x, spot.y].job)
            {
                case "Rook":
                    return rookMoves(spot);
                case "Knight":
                    return knightMoves(spot);
                case "Bishop":
                    return bishopMoves(spot);
                case "Queen":
                    temp.AddRange(bishopMoves(spot));
                    temp.AddRange(rookMoves(spot));
                    return temp;
                case "King":
                    return kingMoves(spot);
                case "Pawn":
                    return pawnMoves(spot);
                default:
                    return temp;    //temp should be empty
            }
        }

        private move medLogic(List<move> pos)
        {
            //gets executed if player selects medium mode
            List<move> bestMovesList = new List<move>();

            for (int i = 0; i < pos.Count; i++)
            {
                switch (board[pos[i].moveSpot.x, pos[i].moveSpot.y].job)    //what piece can you capture
                {
                    case "Queen":
                        pos[i].value = 30;
                        break;
                    case "Rook":
                        pos[i].value = 24;
                        break;
                    case "Bishop":
                        pos[i].value = 18;
                        break;
                    case "Knight":
                        pos[i].value = 12;
                        break;
                    case "Pawn":
                        pos[i].value = 6;
                        break;
                    default:    //empty cell
                        pos[i].value = 0;
                        break;
                }

                switch (board[pos[i].pieceSpot.x, pos[i].pieceSpot.y].job)    //what piece does that capturing
                {
                    case "King":
                        pos[i].value -= 5;
                        break;
                    case "Queen":
                        pos[i].value -= 4;
                        break;
                    case "Rook":
                        pos[i].value -= 3;
                        break;
                    case "Bishop":
                        pos[i].value -= 2;
                        break;
                    case "Knight":
                        pos[i].value -= 1;
                        break;
                    default:    //pawn
                        break;
                }
            }
            pos.Sort((x, y) => y.value.CompareTo(x.value)); //descending order sort

            for (int j = 0; j < pos.Count; j++)
            {
                if(pos[j].value != pos[0].value)    //find all moves with highest value
                {
                    break;
                }
                bestMovesList.Add(pos[j]);  //add them to list
            }
            return bestMovesList[rnd.Next(0, bestMovesList.Count)]; //choose random move from bestMovesList
        }

        private move hardLogic(List<move> pos)
        {
            //gets executed if player selects hard mode
            List<move> humanMoves = new List<move>();
            List<move> bestMovesList = new List<move>();
            List<coordinate> humanPiecesAfterMove = new List<coordinate>();
            coordinate to;
            coordinate from;
            string fromColor;
            string toColor;
            string toJob;

            for (int i = 0; i < pos.Count; i++) //go through all moves
            {
                switch (board[pos[i].moveSpot.x, pos[i].moveSpot.y].job)    //find value of computer move
                {
                    case "Queen":
                        pos[i].value = 900;
                        break;
                    case "Rook":
                        pos[i].value = 720;
                        break;
                    case "Bishop":
                        pos[i].value = 540;
                        break;
                    case "Knight":
                        pos[i].value = 360;
                        break;
                    case "Pawn":
                        pos[i].value = 180;
                        break;
                    default:
                        pos[i].value = 0;
                        break;
                }

                switch (board[pos[i].pieceSpot.x, pos[i].pieceSpot.y].job)    //what piece does the capturing
                {
                    case "King":
                        pos[i].value -= 150;
                        break;
                    case "Queen":
                        pos[i].value -= 120;
                        break;
                    case "Rook":
                        pos[i].value -= 90;
                        break;
                    case "Bishop":
                        pos[i].value -= 60;
                        break;
                    case "Knight":
                        pos[i].value -= 30;
                        break;
                    default:    //pawn
                        break;
                }
                to = pos[i].moveSpot;
                toColor = board[to.x, to.y].color;
                toJob = board[to.x, to.y].job;
                from = pos[i].pieceSpot;
                fromColor = board[from.x, from.y].color;

                //do move
                board[to.x, to.y].color = fromColor;
                board[to.x, to.y].job = board[from.x, from.y].job.ToString();
                board[from.x, from.y].color = null;
                board[from.x, from.y].job = null;

                //human turn
                if(compTeam == "dark")
                {
                    humanPiecesAfterMove = getLightPieces();
                }
                else
                {
                    humanPiecesAfterMove = getDarkPieces();
                }

                foreach(coordinate c in humanPiecesAfterMove)   //go through each human piece
                {
                    humanMoves.AddRange(getCheckRestrictedMoves(c));
                }
                
                for (int j = 0; j < humanMoves.Count; j++)
                {
                    switch (board[humanMoves[j].moveSpot.x, humanMoves[j].moveSpot.y].job)
                    {
                        case "Queen":
                            humanMoves[j].value = 30;
                            break;
                        case "Rook":
                            humanMoves[j].value = 24;
                            break;
                        case "Bishop":
                            humanMoves[j].value = 18;
                            break;
                        case "Knight":
                            humanMoves[j].value = 12;
                            break;
                        case "Pawn":
                            humanMoves[j].value = 6;
                            break;
                        default:    //empty cell
                            humanMoves[j].value = 0;
                            break;
                    }

                    switch (board[humanMoves[j].pieceSpot.x, humanMoves[j].pieceSpot.y].job)    //what piece does the capturing
                    {
                        case "King":
                            humanMoves[j].value -= 5;
                            break;
                        case "Queen":
                            humanMoves[j].value -= 4;
                            break;
                        case "Rook":
                            humanMoves[j].value -= 3;
                            break;
                        case "Bishop":
                            humanMoves[j].value -= 2;
                            break;
                        case "Knight":
                            humanMoves[j].value -= 1;
                            break;
                        default:    //pawn
                            break;
                    }
                }
                humanMoves.Sort((x, y) => x.value.CompareTo(y.value));  //sort ascending

                for (int j = 0; j < humanMoves.Count; j++)
                {
                    if (humanMoves[j].value != humanMoves[0].value)    //find all moves with highest value
                    {
                        break;
                    }
                    bestMovesList.Add(humanMoves[j]);  //add them to list
                }
                pos[i].value -= bestMovesList[rnd.Next(0, bestMovesList.Count)].value; //score of computer move - human reaction move

                //reset pieces
                board[from.x, from.y].color = board[to.x, to.y].color;
                board[from.x, from.y].job = board[to.x, to.y].job;
                board[to.x, to.y].color = toColor;
                board[to.x, to.y].job = toJob;
            }
            pos.Sort((x, y) => y.value.CompareTo(x.value)); //descending order sort
            bestMovesList.Clear();

            for (int i = 0; i < pos.Count; i++)
            {
                if (pos[i].value != pos[0].value)    //find all moves with highest value
                {
                    break;
                }
                bestMovesList.Add(pos[i]);  //add them to list
            }
            return bestMovesList[rnd.Next(0, bestMovesList.Count)]; //choose random move from bestMovesList
        }

        private bool isInCheck(string teamInQuestion)
        {
            //returns whether team in question is in check

            List<coordinate> spots;
            List<move> poss = new List<move>();

            if(teamInQuestion == "dark")
            {
                spots = getLightPieces();//get all opposing team's pieces
            }

            else
            {
                spots = getDarkPieces();//get all opposing team's pieces
            }

            foreach(coordinate c in spots)
            {
                //get possible moves of opposing team,
                //doesn't matter if opposing team move gets them in check,
                //still a valid move for current team
                poss.AddRange(getMoves(c));
            }

            foreach(move m in poss)
            {
                //if opposing team's move can capture your king, you're in check
                if(board[m.moveSpot.x, m.moveSpot.y].job == "King" && board[m.moveSpot.x, m.moveSpot.y].color == teamInQuestion)
                {
                    return true;
                }
            }
            return false;
        }

        private List<move> getCheckRestrictedMoves(coordinate aPiece)
        {
            //takes single piece and returns list of moves that don't put player in check

            List<move> allPossible = new List<move>();
            List<move> possibleWithoutCheck = new List<move>();
            coordinate to;
            coordinate from;
            string fromColor;
            string toColor;
            string toJob;
            bool inCheck;

            allPossible = getMoves(aPiece);

            foreach(move m in allPossible)
            {
                to = m.moveSpot;
                toColor = board[to.x, to.y].color;
                toJob = board[to.x, to.y].job;
                from = m.pieceSpot;
                fromColor = board[from.x, from.y].color;

                //do moves
                board[to.x, to.y].color = fromColor;
                board[to.x, to.y].job = board[from.x, from.y].job.ToString();
                board[from.x, from.y].color = null;
                board[from.x, from.y].job = null;

                //see if in check
                inCheck = isInCheck(fromColor);

                if(inCheck == false)//if not in check
                {
                    possibleWithoutCheck.Add(m);
                }

                //reset pieces
                board[from.x, from.y].color = board[to.x, to.y].color;
                board[from.x, from.y].job = board[to.x, to.y].job;
                board[to.x, to.y].color = toColor;
                board[to.x, to.y].job = toJob;
            }
            return possibleWithoutCheck;
        }

        private bool isInCheckmate(string teamInQuestion, List<coordinate> availablePieces)
        {
            //takes list of pieces and returns whether or not player is in checkmate

            List<move> allPossible = new List<move>();
            List<move> possibleWithoutCheck = new List<move>();
            coordinate to;
            coordinate from;
            string toColor;
            string toJob;
            bool inCheck;

            //find all moves that can be done without going into check
            foreach(coordinate aPiece in availablePieces)
            {
                allPossible = getMoves(aPiece);

                foreach(move m in allPossible)
                {
                    to = m.moveSpot;
                    toColor = board[to.x, to.y].color;
                    toJob = board[to.x, to.y].job;
                    from = m.pieceSpot;

                    //do moves
                    board[to.x, to.y].color = teamInQuestion;
                    board[to.x, to.y].job = board[from.x, from.y].job.ToString();
                    board[from.x, from.y].color = null;
                    board[from.x, from.y].job = null;

                    //see if in check
                    inCheck = isInCheck(teamInQuestion);

                    if(inCheck == false)//if not in check
                    {
                        possibleWithoutCheck.Add(m);
                    }

                    //reset pieces
                    board[from.x, from.y].color = board[to.x, to.y].color;
                    board[from.x, from.y].job = board[to.x, to.y].job;
                    board[to.x, to.y].color = toColor;
                    board[to.x, to.y].job = toJob;
                }
            }

            if(possibleWithoutCheck.Count == 0)//if no moves available that don't go into check
            {
                GameOver gameEnd = new GameOver(this, teamInQuestion);
                gameEnd.ShowDialog();
                return true;
            }
            return false;
        }

        private void castling(move shift)
        {
            //if selected move is a castling move, move Rook in this function

            historyNode node;
            move castleMove = new move();
            int yCoor;                              //which row the move is being conducted in
            coordinate toSpot = shift.moveSpot;
            coordinate fromSpot = shift.pieceSpot;

            if(offensiveTeam == baseOnBottom)
            {
                yCoor = 0;
            }
            else
            {
                yCoor = 7;
            }

            if(fromSpot.x == 4 && fromSpot.y == yCoor)  //if moving from King default position
            {
                if(toSpot.x == 2 && toSpot.y == yCoor)  //if moving two spaces to the left
                {
                    coordinate newCastleCoor = new coordinate(3, yCoor);
                    coordinate oldCastleCoor = new coordinate(0, yCoor);
                    castleMove.moveSpot = newCastleCoor;
                    castleMove.pieceSpot = oldCastleCoor;
                    node = new historyNode(castleMove, board[3, yCoor], false, true, true, baseOnBottom);
                    history.Push(node);
                    movePiece(newCastleCoor, board[0, yCoor], oldCastleCoor);
                }

                else if(toSpot.x == 6 && toSpot.y == yCoor) //if moving two spaces to the right
                {
                    coordinate newCastleCoor = new coordinate(5, yCoor);
                    coordinate oldCastleCoor = new coordinate(7, yCoor);
                    castleMove.moveSpot = newCastleCoor;
                    castleMove.pieceSpot = oldCastleCoor;
                    node = new historyNode(castleMove, board[5, yCoor], false, true, true, baseOnBottom);
                    history.Push(node);
                    movePiece(newCastleCoor, board[7, yCoor], oldCastleCoor);
                }
            }
        }

        public void clicker(coordinate currentCell)
        {
            //human player's turn, gets called when player clicks on spot
            bool movableSpot;
            historyNode node;
            string baseOnTop;
            move curTurn = new move();

            if (firstGame == true)  //blocks functionality if game hasn't started yet
            {
                piece currentPiece = board[currentCell.x, currentCell.y];

                if (currentPiece.color == offensiveTeam)//if selected own piece
                {
                    movablePieceSelected = true;
                    clearSelectedOrPossible();
                    coordinateToPictureBox(currentCell).BackColor = System.Drawing.Color.DeepSkyBlue;
                    prevSelected = currentCell;
                    possible.Clear();
                    possible.AddRange(getCheckRestrictedMoves(currentCell));

                    foreach (move m in possible)
                    {
                        coordinateToPictureBox(m.moveSpot).BackColor = System.Drawing.Color.LawnGreen;
                    }
                }

                else if (movablePieceSelected == true)//if previously selected own piece
                {
                    movableSpot = false;

                    foreach (move m in possible)
                    {
                        if (currentCell.Equals(m.moveSpot))//if selected spot is in possible move list
                        {
                            movableSpot = true;
                            curTurn = m;
                        }
                    }

                    if (movableSpot == true)
                    {
                        piece captured = board[currentCell.x, currentCell.y];
                        bool virginMove = board[prevSelected.x, prevSelected.y].firstMove;
                        movePiece(currentCell, board[prevSelected.x, prevSelected.y], prevSelected);
                        clearSelectedOrPossible();

                        if (board[currentCell.x, currentCell.y].job == "Pawn")
                        {
                            if(baseOnBottom == "light")
                            {
                                baseOnTop = "dark";
                            }
                            else
                            {
                                baseOnTop = "light";
                            }

                            if (board[currentCell.x, currentCell.y].color == baseOnBottom && currentCell.y == 7)
                            {
                                PawnTransformation transform = new PawnTransformation(currentCell, this);
                                transform.ShowDialog();
                                node = new historyNode(curTurn, captured, true, false, false, baseOnBottom);
                            }

                            else if (board[currentCell.x, currentCell.y].color == baseOnTop && currentCell.y == 0)
                            {
                                PawnTransformation transform = new PawnTransformation(currentCell, this);
                                transform.ShowDialog();
                                node = new historyNode(curTurn, captured, true, false, false, baseOnBottom);
                            }

                            else    //if pawn, but no transform
                            {
                                node = new historyNode(curTurn, captured, false, false, virginMove, baseOnBottom);
                            }
                        }

                        else    //not pawn
                        {
                            node = new historyNode(curTurn, captured, false, false, virginMove, baseOnBottom);
                        }

                        history.Push(node);
                        mForm.undoToolStripMenuItem.Enabled = true;

                        if (lastMove == true)
                        {
                            clearToAndFrom();
                            coordinateToPictureBox(curTurn.pieceSpot).BackgroundImage = Resources.from;
                            coordinateToPictureBox(curTurn.moveSpot).BackgroundImage = Resources.to;
                            toCoor = curTurn.moveSpot;
                            fromCoor = curTurn.pieceSpot;
                        }

                        if (board[currentCell.x, currentCell.y].job == "King")
                        {
                            castling(curTurn);//check if move is a castling
                        }
                        betweenTurns();
                    }
                }
            }
        }

        private void betweenTurns()
        {
            //In between light and dark's turns
            List<move> possibleWithoutCheck = new List<move>();
            bool endOfGame;

            //change teams
            if (offensiveTeam == "light")
            {
                offensiveTeam = "dark";
                endOfGame = isInCheckmate(offensiveTeam, getDarkPieces());  //did previous turn put other team in checkmate?

                if (endOfGame == false && onePlayer == true)
                {
                    foreach (coordinate cell in getDarkPieces())
                    {
                        possibleWithoutCheck.AddRange(getCheckRestrictedMoves(cell));
                    }

                    compTurn(possibleWithoutCheck);
                    isInCheckmate("light", getLightPieces()); //did computer turn put player in checkmate?
                    offensiveTeam = "light";
                }
            }
            else
            {
                offensiveTeam = "light";
                endOfGame = isInCheckmate(offensiveTeam, getLightPieces()); //did previous turn put other team in checkmate?

                if (endOfGame == false && onePlayer == true)
                {
                    foreach (coordinate cell in getLightPieces())
                    {
                        possibleWithoutCheck.AddRange(getCheckRestrictedMoves(cell));
                    }

                    compTurn(possibleWithoutCheck);
                    isInCheckmate("dark", getDarkPieces()); //did computer turn put player in checkmate?
                    offensiveTeam = "dark";
                }
            }

            if(onePlayer == false && rotate == true)    //rotate
            {
                rotateBoard();
            }
        }

        private void compTurn(List<move> poss)
        {
            //computer's turn
            historyNode node;
            move bestMove;
            int r;

            if(medMode == true)
            {
                bestMove = medLogic(poss);
            }

            else if(hardMode == true)
            {
                bestMove = hardLogic(poss);
            }

            else
            {
                r = rnd.Next(0, poss.Count);//choose random move
                bestMove = poss[r];
            }

            coordinate newSpot = new coordinate(bestMove.moveSpot.x, bestMove.moveSpot.y);
            coordinate oldSpot = new coordinate(bestMove.pieceSpot.x, bestMove.pieceSpot.y);

            piece captured = board[newSpot.x, newSpot.y];
            bool virginMove = board[oldSpot.x, oldSpot.y].firstMove;
            movePiece(newSpot, board[oldSpot.x, oldSpot.y], oldSpot);

            if (board[newSpot.x, newSpot.y].job == "Pawn" && newSpot.y == 0)//if pawn makes it to last row
            {
                r = rnd.Next(0, 4); //choose random piece to transform into

                if(compTeam == "dark")
                {
                    switch (r)
                    {
                        case 0:
                            board[newSpot.x, newSpot.y].job = "Queen";
                            coordinateToPictureBox(newSpot).Image = dQueen;
                            break;
                        case 1:
                            board[newSpot.x, newSpot.y].job = "Rook";
                            coordinateToPictureBox(newSpot).Image = dRook;
                            break;
                        case 2:
                            board[newSpot.x, newSpot.y].job = "Bishop";
                            coordinateToPictureBox(newSpot).Image = dBishop;
                            break;
                        case 3:
                            board[newSpot.x, newSpot.y].job = "Knight";
                            coordinateToPictureBox(newSpot).Image = dKnight;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (r)
                    {
                        case 0:
                            board[newSpot.x, newSpot.y].job = "Queen";
                            coordinateToPictureBox(newSpot).Image = lQueen;
                            break;
                        case 1:
                            board[newSpot.x, newSpot.y].job = "Rook";
                            coordinateToPictureBox(newSpot).Image = lRook;
                            break;
                        case 2:
                            board[newSpot.x, newSpot.y].job = "Bishop";
                            coordinateToPictureBox(newSpot).Image = lBishop;
                            break;
                        case 3:
                            board[newSpot.x, newSpot.y].job = "Knight";
                            coordinateToPictureBox(newSpot).Image = lKnight;
                            break;
                        default:
                            break;
                    }
                }
                node = new historyNode(bestMove, captured, true, true, false, baseOnBottom);
            }

            else
            {
                node = new historyNode(bestMove, captured, false, true, virginMove, baseOnBottom);
            }

            history.Push(node);

            if (lastMove == true)
            {
                clearToAndFrom();
                coordinateToPictureBox(bestMove.pieceSpot).BackgroundImage = Resources.from;
                coordinateToPictureBox(bestMove.moveSpot).BackgroundImage = Resources.to;
                toCoor = bestMove.moveSpot;
                fromCoor = bestMove.pieceSpot;
            }

            if (board[newSpot.x, newSpot.y].job == "King")
            {
                castling(bestMove);//check if move is a castling
            }
        }

        private void rotateBoard()
        {
            firstGame = false;  //So can't click anything during animation
            tick = 0;
            clearToAndFrom();
            mForm.timer.Start();
            while(tick < 42)
            {
                Application.DoEvents();
            }
            
            mForm.timer.Stop();
            rotatePieces();
            rotateToAndFrom();

            if (baseOnBottom == "light")
            {
                baseOnBottom = "dark";
            }
            else
            {
                baseOnBottom = "light";
            }
            firstGame = true;
        }

        private void rotatePieces()
        {
            piece[,] bufferBoard = new piece[8,8];
            int newX;
            int newY;

            foreach(coordinate piece in getAllPieces())
            {
                newX = 7 - piece.x;
                newY = 7 - piece.y;
                bufferBoard[newX, newY] = board[piece.x, piece.y];
            }
            board = bufferBoard;
        }

        private void rotateToAndFrom()
        {
            coordinate temp;
            temp = new coordinate(7 - toCoor.x, 7 - toCoor.y);
            coordinateToPictureBox(temp).BackgroundImage = Resources.to;
            toCoor = temp;
            temp = new coordinate(7 - fromCoor.x, 7 - fromCoor.y);
            coordinateToPictureBox(temp).BackgroundImage = Resources.from;
            fromCoor = temp;
        }

        public void moveRing(int small, int big)
        {
            string direction;
            Image saved = coordinateToPictureBox(new coordinate(small, big)).Image; //first image moved

            direction = "down";
            for (int y = big; y > small; y--)
            {
                saved = moveImage(small, y, direction, saved);
            }

            direction = "right";
            for (int x = small; x < big; x++)
            {
                saved = moveImage(x, small, direction, saved);
            }

            direction = "up";
            for (int y = small; y < big; y++)
            {
                saved = moveImage(big, y, direction, saved);
            }

            direction = "left";
            for (int x = big; x > small; x--)
            {
                saved = moveImage(x, big, direction, saved);
            }
        }

        private Image moveImage(int fromX, int fromY, string dir, Image overwrite)
        {
            Image replace;
            coordinate toCoor;
            coordinate fromCoor = new coordinate(fromX, fromY);

            if(dir == "down")
            {
                toCoor = new coordinate(fromX, fromY - 1);
            }
            else if (dir == "right")
            {
                toCoor = new coordinate(fromX + 1, fromY);
            }
            else if (dir == "up")
            {
                toCoor = new coordinate(fromX, fromY + 1);
            }
            else//left
            {
                toCoor = new coordinate(fromX - 1, fromY);
            }

            replace = coordinateToPictureBox(toCoor).Image;
            coordinateToPictureBox(toCoor).Image = overwrite;
            return replace;
        }

        private void movePiece(coordinate newCell, piece pPiece, coordinate oldCell)
        {
            //overwrite current cell
            board[newCell.x, newCell.y].color = pPiece.color;
            board[newCell.x, newCell.y].job = pPiece.job;

            //delete prev cell
            board[oldCell.x, oldCell.y].color = null;
            board[oldCell.x, oldCell.y].job = null;

            //overwrite current image
            //take previous piece picture and put it in current cell picture box
            coordinateToPictureBox(newCell).Image = matchPicture(pPiece);

            //delete prev image
            coordinateToPictureBox(oldCell).Image = null;

            movablePieceSelected = false;
            board[newCell.x, newCell.y].firstMove = false;
        }

        public void undo()
        {
            //moves pieces backwards
            Image pawnPic;
            piece to;
            piece from;
            int xMove;
            int yMove;
            int xPiece;
            int yPiece;
            historyNode node = history.Pop();

            xMove = node.step.moveSpot.x;
            yMove = node.step.moveSpot.y;
            xPiece = node.step.pieceSpot.x;
            yPiece = node.step.pieceSpot.y;

            if (!node.whoIsOnBottom.Equals(baseOnBottom))
            {
                rotateBoard();
            }

            to = board[xMove, yMove];
            from = board[xPiece, yPiece];
            offensiveTeam = to.color;

            if (node.pawnTransform == true)
            {
                if (to.color == "light")
                {
                    pawnPic = lPawn;
                }

                else
                {
                    pawnPic = dPawn;
                }

                board[xPiece, yPiece].job = "Pawn";
                coordinateToPictureBox(new coordinate(xPiece, yPiece)).Image = pawnPic;
            }

            else
            {
                board[xPiece, yPiece].job = to.job;
                coordinateToPictureBox(new coordinate(xPiece, yPiece)).Image = matchPicture(to);
            }

            board[xPiece, yPiece].color = to.color;
            board[xPiece, yPiece].firstMove = node.virgin;

            //put captured piece back
            board[xMove, yMove].job = node.captured.job;
            coordinateToPictureBox(new coordinate(xMove, yMove)).Image = matchPicture(node.captured);
            board[xMove, yMove].color = node.captured.color;
            board[xMove, yMove].firstMove = node.captured.firstMove;


            if (node.skip == true)
            {
                undo(); //call function again to undo another move
            }

            else if (history.Count == 0)    //if stack is empty, disable button; skip and empty stack can't both happen
            {
                mForm.undoToolStripMenuItem.Enabled = false;
            }
            clearToAndFrom();
            clearSelectedOrPossible();
        }

        private Image matchPicture(piece figure)
        {
            //returns image based on what piece it is

            if(figure.color == "dark")
            {
                switch(figure.job)
                {
                    case "King":
                        return dKing;
                    case "Queen":
                        return dQueen;
                    case "Bishop":
                        return dBishop;
                    case "Knight":
                        return dKnight;
                    case "Rook":
                        return dRook;
                    case "Pawn":
                        return dPawn;
                    default:
                        return null;
                }
            }
            else
            {
                switch (figure.job)
                {
                    case "King":
                        return lKing;
                    case "Queen":
                        return lQueen;
                    case "Bishop":
                        return lBishop;
                    case "Knight":
                        return lKnight;
                    case "Rook":
                        return lRook;
                    case "Pawn":
                        return lPawn;
                    default:
                        return null;
                }
            }
        }

        public PictureBox coordinateToPictureBox(coordinate spot)
        {
            //takes (x, y) and returns associated pictureBox

            switch (spot.y)
            {
                case 0:
                    switch (spot.x)
                    {
                        case 0:
                            return mForm.pictureBox57;
                        case 1:
                            return mForm.pictureBox58;
                        case 2:
                            return mForm.pictureBox59;
                        case 3:
                            return mForm.pictureBox60;
                        case 4:
                            return mForm.pictureBox61;
                        case 5:
                            return mForm.pictureBox62;
                        case 6:
                            return mForm.pictureBox63;
                        case 7:
                            return mForm.pictureBox64;
                        default:
                            return null;
                    }
                case 1:
                    switch (spot.x)
                    {
                        case 0:
                            return mForm.pictureBox49;
                        case 1:
                            return mForm.pictureBox50;
                        case 2:
                            return mForm.pictureBox51;
                        case 3:
                            return mForm.pictureBox52;
                        case 4:
                            return mForm.pictureBox53;
                        case 5:
                            return mForm.pictureBox54;
                        case 6:
                            return mForm.pictureBox55;
                        case 7:
                            return mForm.pictureBox56;
                        default:
                            return null;
                    }
                case 2:
                    switch (spot.x)
                    {
                        case 0:
                            return mForm.pictureBox41;
                        case 1:
                            return mForm.pictureBox42;
                        case 2:
                            return mForm.pictureBox43;
                        case 3:
                            return mForm.pictureBox44;
                        case 4:
                            return mForm.pictureBox45;
                        case 5:
                            return mForm.pictureBox46;
                        case 6:
                            return mForm.pictureBox47;
                        case 7:
                            return mForm.pictureBox48;
                        default:
                            return null;
                    }
                case 3:
                    switch (spot.x)
                    {
                        case 0:
                            return mForm.pictureBox33;
                        case 1:
                            return mForm.pictureBox34;
                        case 2:
                            return mForm.pictureBox35;
                        case 3:
                            return mForm.pictureBox36;
                        case 4:
                            return mForm.pictureBox37;
                        case 5:
                            return mForm.pictureBox38;
                        case 6:
                            return mForm.pictureBox39;
                        case 7:
                            return mForm.pictureBox40;
                        default:
                            return null;
                    }
                case 4:
                    switch (spot.x)
                    {
                        case 0:
                            return mForm.pictureBox25;
                        case 1:
                            return mForm.pictureBox26;
                        case 2:
                            return mForm.pictureBox27;
                        case 3:
                            return mForm.pictureBox28;
                        case 4:
                            return mForm.pictureBox29;
                        case 5:
                            return mForm.pictureBox30;
                        case 6:
                            return mForm.pictureBox31;
                        case 7:
                            return mForm.pictureBox32;
                        default:
                            return null;
                    }
                case 5:
                    switch (spot.x)
                    {
                        case 0:
                            return mForm.pictureBox17;
                        case 1:
                            return mForm.pictureBox18;
                        case 2:
                            return mForm.pictureBox19;
                        case 3:
                            return mForm.pictureBox20;
                        case 4:
                            return mForm.pictureBox21;
                        case 5:
                            return mForm.pictureBox22;
                        case 6:
                            return mForm.pictureBox23;
                        case 7:
                            return mForm.pictureBox24;
                        default:
                            return null;
                    }
                case 6:
                    switch (spot.x)
                    {
                        case 0:
                            return mForm.pictureBox9;
                        case 1:
                            return mForm.pictureBox10;
                        case 2:
                            return mForm.pictureBox11;
                        case 3:
                            return mForm.pictureBox12;
                        case 4:
                            return mForm.pictureBox13;
                        case 5:
                            return mForm.pictureBox14;
                        case 6:
                            return mForm.pictureBox15;
                        case 7:
                            return mForm.pictureBox16;
                        default:
                            return null;
                    }
                case 7:
                    switch (spot.x)
                    {
                        case 0:
                            return mForm.pictureBox1;
                        case 1:
                            return mForm.pictureBox2;
                        case 2:
                            return mForm.pictureBox3;
                        case 3:
                            return mForm.pictureBox4;
                        case 4:
                            return mForm.pictureBox5;
                        case 5:
                            return mForm.pictureBox6;
                        case 6:
                            return mForm.pictureBox7;
                        case 7:
                            return mForm.pictureBox8;
                        default:
                            return null;
                    }
                default:
                    return null;
            }
        }

        public void clearSelectedOrPossible()
        {
            mForm.pictureBox1.BackColor = System.Drawing.Color.White;
            mForm.pictureBox2.BackColor = System.Drawing.Color.DarkGray;
            mForm.pictureBox3.BackColor = System.Drawing.Color.White;
            mForm.pictureBox4.BackColor = System.Drawing.Color.DarkGray;
            mForm.pictureBox5.BackColor = System.Drawing.Color.White;
            mForm.pictureBox6.BackColor = System.Drawing.Color.DarkGray;
            mForm.pictureBox7.BackColor = System.Drawing.Color.White;
            mForm.pictureBox8.BackColor = System.Drawing.Color.DarkGray;
            mForm.pictureBox9.BackColor = System.Drawing.Color.DarkGray;
            mForm.pictureBox10.BackColor = System.Drawing.Color.White;
            mForm.pictureBox11.BackColor = System.Drawing.Color.DarkGray;
            mForm.pictureBox12.BackColor = System.Drawing.Color.White;
            mForm.pictureBox13.BackColor = System.Drawing.Color.DarkGray;
            mForm.pictureBox14.BackColor = System.Drawing.Color.White;
            mForm.pictureBox15.BackColor = System.Drawing.Color.DarkGray;
            mForm.pictureBox16.BackColor = System.Drawing.Color.White;
            mForm.pictureBox17.BackColor = System.Drawing.Color.White;
            mForm.pictureBox18.BackColor = System.Drawing.Color.DarkGray;
            mForm.pictureBox19.BackColor = System.Drawing.Color.White;
            mForm.pictureBox20.BackColor = System.Drawing.Color.DarkGray;
            mForm.pictureBox21.BackColor = System.Drawing.Color.White;
            mForm.pictureBox22.BackColor = System.Drawing.Color.DarkGray;
            mForm.pictureBox23.BackColor = System.Drawing.Color.White;
            mForm.pictureBox24.BackColor = System.Drawing.Color.DarkGray;
            mForm.pictureBox25.BackColor = System.Drawing.Color.DarkGray;
            mForm.pictureBox26.BackColor = System.Drawing.Color.White;
            mForm.pictureBox27.BackColor = System.Drawing.Color.DarkGray;
            mForm.pictureBox28.BackColor = System.Drawing.Color.White;
            mForm.pictureBox29.BackColor = System.Drawing.Color.DarkGray;
            mForm.pictureBox30.BackColor = System.Drawing.Color.White;
            mForm.pictureBox31.BackColor = System.Drawing.Color.DarkGray;
            mForm.pictureBox32.BackColor = System.Drawing.Color.White;
            mForm.pictureBox33.BackColor = System.Drawing.Color.White;
            mForm.pictureBox34.BackColor = System.Drawing.Color.DarkGray;
            mForm.pictureBox35.BackColor = System.Drawing.Color.White;
            mForm.pictureBox36.BackColor = System.Drawing.Color.DarkGray;
            mForm.pictureBox37.BackColor = System.Drawing.Color.White;
            mForm.pictureBox38.BackColor = System.Drawing.Color.DarkGray;
            mForm.pictureBox39.BackColor = System.Drawing.Color.White;
            mForm.pictureBox40.BackColor = System.Drawing.Color.DarkGray;
            mForm.pictureBox41.BackColor = System.Drawing.Color.DarkGray;
            mForm.pictureBox42.BackColor = System.Drawing.Color.White;
            mForm.pictureBox43.BackColor = System.Drawing.Color.DarkGray;
            mForm.pictureBox44.BackColor = System.Drawing.Color.White;
            mForm.pictureBox45.BackColor = System.Drawing.Color.DarkGray;
            mForm.pictureBox46.BackColor = System.Drawing.Color.White;
            mForm.pictureBox47.BackColor = System.Drawing.Color.DarkGray;
            mForm.pictureBox48.BackColor = System.Drawing.Color.White;
            mForm.pictureBox49.BackColor = System.Drawing.Color.White;
            mForm.pictureBox50.BackColor = System.Drawing.Color.DarkGray;
            mForm.pictureBox51.BackColor = System.Drawing.Color.White;
            mForm.pictureBox52.BackColor = System.Drawing.Color.DarkGray;
            mForm.pictureBox53.BackColor = System.Drawing.Color.White;
            mForm.pictureBox54.BackColor = System.Drawing.Color.DarkGray;
            mForm.pictureBox55.BackColor = System.Drawing.Color.White;
            mForm.pictureBox56.BackColor = System.Drawing.Color.DarkGray;
            mForm.pictureBox57.BackColor = System.Drawing.Color.DarkGray;
            mForm.pictureBox58.BackColor = System.Drawing.Color.White;
            mForm.pictureBox59.BackColor = System.Drawing.Color.DarkGray;
            mForm.pictureBox60.BackColor = System.Drawing.Color.White;
            mForm.pictureBox61.BackColor = System.Drawing.Color.DarkGray;
            mForm.pictureBox62.BackColor = System.Drawing.Color.White;
            mForm.pictureBox63.BackColor = System.Drawing.Color.DarkGray;
            mForm.pictureBox64.BackColor = System.Drawing.Color.White;
        }

        public void clearToAndFrom()
        {
            coordinateToPictureBox(toCoor).BackgroundImage = null;
            coordinateToPictureBox(fromCoor).BackgroundImage = null;
        }

        public void playAsLight()
        {
            //sets images on board for new game

            mForm.pictureBox1.Image = dRook;
            mForm.pictureBox2.Image = dKnight;
            mForm.pictureBox3.Image = dBishop;
            mForm.pictureBox4.Image = dQueen;
            mForm.pictureBox5.Image = dKing;
            mForm.pictureBox6.Image = dBishop;
            mForm.pictureBox7.Image = dKnight;
            mForm.pictureBox8.Image = dRook;
            mForm.pictureBox9.Image = dPawn;
            mForm.pictureBox10.Image = dPawn;
            mForm.pictureBox11.Image = dPawn;
            mForm.pictureBox12.Image = dPawn;
            mForm.pictureBox13.Image = dPawn;
            mForm.pictureBox14.Image = dPawn;
            mForm.pictureBox15.Image = dPawn;
            mForm.pictureBox16.Image = dPawn;
            mForm.pictureBox17.Image = null;
            mForm.pictureBox18.Image = null;
            mForm.pictureBox19.Image = null;
            mForm.pictureBox20.Image = null;
            mForm.pictureBox21.Image = null;
            mForm.pictureBox22.Image = null;
            mForm.pictureBox23.Image = null;
            mForm.pictureBox24.Image = null;
            mForm.pictureBox25.Image = null;
            mForm.pictureBox26.Image = null;
            mForm.pictureBox27.Image = null;
            mForm.pictureBox28.Image = null;
            mForm.pictureBox29.Image = null;
            mForm.pictureBox30.Image = null;
            mForm.pictureBox31.Image = null;
            mForm.pictureBox32.Image = null;
            mForm.pictureBox33.Image = null;
            mForm.pictureBox34.Image = null;
            mForm.pictureBox35.Image = null;
            mForm.pictureBox36.Image = null;
            mForm.pictureBox37.Image = null;
            mForm.pictureBox38.Image = null;
            mForm.pictureBox39.Image = null;
            mForm.pictureBox40.Image = null;
            mForm.pictureBox41.Image = null;
            mForm.pictureBox42.Image = null;
            mForm.pictureBox43.Image = null;
            mForm.pictureBox44.Image = null;
            mForm.pictureBox45.Image = null;
            mForm.pictureBox46.Image = null;
            mForm.pictureBox47.Image = null;
            mForm.pictureBox48.Image = null;
            mForm.pictureBox49.Image = lPawn;
            mForm.pictureBox50.Image = lPawn;
            mForm.pictureBox51.Image = lPawn;
            mForm.pictureBox52.Image = lPawn;
            mForm.pictureBox53.Image = lPawn;
            mForm.pictureBox54.Image = lPawn;
            mForm.pictureBox55.Image = lPawn;
            mForm.pictureBox56.Image = lPawn;
            mForm.pictureBox57.Image = lRook;
            mForm.pictureBox58.Image = lKnight;
            mForm.pictureBox59.Image = lBishop;
            mForm.pictureBox60.Image = lQueen;
            mForm.pictureBox61.Image = lKing;
            mForm.pictureBox62.Image = lBishop;
            mForm.pictureBox63.Image = lKnight;
            mForm.pictureBox64.Image = lRook;
        }

        public void playAsDark()
        {
            //sets images on board for new game

            mForm.pictureBox1.Image = lRook;
            mForm.pictureBox2.Image = lKnight;
            mForm.pictureBox3.Image = lBishop;
            mForm.pictureBox4.Image = lQueen;
            mForm.pictureBox5.Image = lKing;
            mForm.pictureBox6.Image = lBishop;
            mForm.pictureBox7.Image = lKnight;
            mForm.pictureBox8.Image = lRook;
            mForm.pictureBox9.Image = lPawn;
            mForm.pictureBox10.Image = lPawn;
            mForm.pictureBox11.Image = lPawn;
            mForm.pictureBox12.Image = lPawn;
            mForm.pictureBox13.Image = lPawn;
            mForm.pictureBox14.Image = lPawn;
            mForm.pictureBox15.Image = lPawn;
            mForm.pictureBox16.Image = lPawn;
            mForm.pictureBox17.Image = null;
            mForm.pictureBox18.Image = null;
            mForm.pictureBox19.Image = null;
            mForm.pictureBox20.Image = null;
            mForm.pictureBox21.Image = null;
            mForm.pictureBox22.Image = null;
            mForm.pictureBox23.Image = null;
            mForm.pictureBox24.Image = null;
            mForm.pictureBox25.Image = null;
            mForm.pictureBox26.Image = null;
            mForm.pictureBox27.Image = null;
            mForm.pictureBox28.Image = null;
            mForm.pictureBox29.Image = null;
            mForm.pictureBox30.Image = null;
            mForm.pictureBox31.Image = null;
            mForm.pictureBox32.Image = null;
            mForm.pictureBox33.Image = null;
            mForm.pictureBox34.Image = null;
            mForm.pictureBox35.Image = null;
            mForm.pictureBox36.Image = null;
            mForm.pictureBox37.Image = null;
            mForm.pictureBox38.Image = null;
            mForm.pictureBox39.Image = null;
            mForm.pictureBox40.Image = null;
            mForm.pictureBox41.Image = null;
            mForm.pictureBox42.Image = null;
            mForm.pictureBox43.Image = null;
            mForm.pictureBox44.Image = null;
            mForm.pictureBox45.Image = null;
            mForm.pictureBox46.Image = null;
            mForm.pictureBox47.Image = null;
            mForm.pictureBox48.Image = null;
            mForm.pictureBox49.Image = dPawn;
            mForm.pictureBox50.Image = dPawn;
            mForm.pictureBox51.Image = dPawn;
            mForm.pictureBox52.Image = dPawn;
            mForm.pictureBox53.Image = dPawn;
            mForm.pictureBox54.Image = dPawn;
            mForm.pictureBox55.Image = dPawn;
            mForm.pictureBox56.Image = dPawn;
            mForm.pictureBox57.Image = dRook;
            mForm.pictureBox58.Image = dKnight;
            mForm.pictureBox59.Image = dBishop;
            mForm.pictureBox60.Image = dQueen;
            mForm.pictureBox61.Image = dKing;
            mForm.pictureBox62.Image = dBishop;
            mForm.pictureBox63.Image = dKnight;
            mForm.pictureBox64.Image = dRook;
        }

        public void changeTheme()
        {
            //calls matchPicture() on each piece and puts image in PictureBox
            foreach (coordinate spot in getAllPieces())
            {
                coordinateToPictureBox(spot).Image = matchPicture(board[spot.x, spot.y]);
            }
        }

        public void setTheme()
        {
            //sets image variables based on themeIndex
            System.IO.Stream lKingFile = 
                themeList[themeIndex].GetManifestResourceStream(themeList[themeIndex].GetName().Name + ".lKing.png");
            System.IO.Stream lQueenFile = 
                themeList[themeIndex].GetManifestResourceStream(themeList[themeIndex].GetName().Name + ".lQueen.png");
            System.IO.Stream lBishopFile = 
                themeList[themeIndex].GetManifestResourceStream(themeList[themeIndex].GetName().Name + ".lBishop.png");
            System.IO.Stream lKnightFile = 
                themeList[themeIndex].GetManifestResourceStream(themeList[themeIndex].GetName().Name + ".lKnight.png");
            System.IO.Stream lRookFile = 
                themeList[themeIndex].GetManifestResourceStream(themeList[themeIndex].GetName().Name + ".lRook.png");
            System.IO.Stream lPawnFile = 
                themeList[themeIndex].GetManifestResourceStream(themeList[themeIndex].GetName().Name + ".lPawn.png");
            System.IO.Stream dKingFile = 
                themeList[themeIndex].GetManifestResourceStream(themeList[themeIndex].GetName().Name + ".dKing.png");
            System.IO.Stream dQueenFile = 
                themeList[themeIndex].GetManifestResourceStream(themeList[themeIndex].GetName().Name + ".dQueen.png");
            System.IO.Stream dBishopFile = 
                themeList[themeIndex].GetManifestResourceStream(themeList[themeIndex].GetName().Name + ".dBishop.png");
            System.IO.Stream dKnightFile = 
                themeList[themeIndex].GetManifestResourceStream(themeList[themeIndex].GetName().Name + ".dKnight.png");
            System.IO.Stream dRookFile = 
                themeList[themeIndex].GetManifestResourceStream(themeList[themeIndex].GetName().Name + ".dRook.png");
            System.IO.Stream dPawnFile = 
                themeList[themeIndex].GetManifestResourceStream(themeList[themeIndex].GetName().Name + ".dPawn.png");

            try
            {
                lKing = Image.FromStream(lKingFile);
                lQueen = Image.FromStream(lQueenFile);
                lBishop = Image.FromStream(lBishopFile);
                lKnight = Image.FromStream(lKnightFile);
                lRook = Image.FromStream(lRookFile);
                lPawn = Image.FromStream(lPawnFile);
                dKing = Image.FromStream(dKingFile);
                dQueen = Image.FromStream(dQueenFile);
                dBishop = Image.FromStream(dBishopFile);
                dKnight = Image.FromStream(dKnightFile);
                dRook = Image.FromStream(dRookFile);
                dPawn = Image.FromStream(dPawnFile);
            }
            catch (ArgumentException)
            {
                themeList.RemoveAt(themeIndex);
            }
        }

        public void tryDlls()
        {
            //calls loadDlls() and setTheme() till found all themes
            bool dllsFound = false;
            int originalSize;

            while (dllsFound == false)
            {
                loadDlls();

                originalSize = themeList.Count;

                for (int i = 0; i < originalSize; i++)
                {
                    themeIndex = originalSize - i - 1;
                    setTheme();
                }

                if (themeList.Count < 1)
                {
                    NoThemes none = new NoThemes();
                    none.ShowDialog();
                }
                else
                {
                    dllsFound = true;
                }
            }
            //find default theme
            themeIndex = themeList.FindIndex(x => x.GetName().Name == "Figure");

            if(themeIndex == -1)    //if can't find default
            {
                themeIndex = 0;
            }
            setTheme();
        }

        private void loadDlls()
        {
            //searches dlls in working directory and loads themes
            AssemblyName an;
            Assembly assembly;
            themeList = new List<Assembly>();
            string[] dllFilePathArray = null;
            List<string> ignore = new List<string>();
            string path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            bool themesFound = false;

            while (themesFound == false)
            {
                try
                {
                    dllFilePathArray = Directory.GetFiles(path, "*.dll");

                    foreach (string dllFilePath in dllFilePathArray.Except(ignore))
                    {
                        FileInfo file = new FileInfo(dllFilePath);
                        file.DeleteAlternateDataStream("Zone.Identifier");
                        an = AssemblyName.GetAssemblyName(dllFilePath);
                        assembly = Assembly.Load(an);
                        
                        if(!themeList.Contains(assembly))
                        {
                            themeList.Add(assembly);
                        }
                    }
                }
                catch (BadImageFormatException ex)
                {
                    ignore.Add(path + "\\" + ex.FileName);
                    themeList.Clear();
                }

                if (themeList.Count < 1)    //if 0 themes or 1 bad file before added to ignore
                {
                    //if 0 bad files <OR> bad files = total dll files
                    if (ignore.Count < 1 || ignore.Count == dllFilePathArray.Count())
                    {
                        NoThemes none = new NoThemes();
                        none.ShowDialog();
                        ignore.Clear();
                    }
                }
                else
                {
                    themesFound = true;
                }
            }
        }

        public void saveState()
        {
            if(firstGame == true)
            {
                string theme = themeList[themeIndex].GetName().Name;
                saveData sData = new saveData(board, offensiveTeam, theme, baseOnBottom, onePlayer, medMode, hardMode, 
                    lastMove, saveGame, gameOverExit, rotate);

                System.IO.Directory.CreateDirectory(dirPath);
                BinaryFormatter writer = new BinaryFormatter();
                FileStream saveStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                writer.Serialize(saveStream, sData);
                saveStream.Close();
            }
        }

        public void loadState()
        {
            BinaryFormatter reader = new BinaryFormatter();
            FileStream loadStream = new FileStream(filePath, FileMode.Open);

            try
            {
                saveData lData = (saveData)reader.Deserialize(loadStream);  //load file
                loadStream.Close();

                if(lData.sSaveGame == true)
                {
                    if(lData.sGameOverExit == false)
                    {
                        board = new piece[8, 8];
                        firstGame = true;
                        movablePieceSelected = false;
                        board = lData.sBoard;
                        offensiveTeam = lData.sOffensiveTeam;
                        baseOnBottom = lData.sBaseOnBottom;
                        onePlayer = lData.sOnePlayer;
                        medMode = lData.sMedMode;
                        hardMode = lData.sHardMode;

                        if (offensiveTeam == "light")
                        {
                            compTeam = "dark";
                        }
                        else
                        {
                            compTeam = "light";
                        }

                        if (onePlayer == true)
                        {
                            mForm.rotateBoardToolStripMenuItem.Enabled = false;
                        }
                        else
                        {
                            mForm.rotateBoardToolStripMenuItem.Enabled = true;
                        }
                    }
                    else    //Exit on Game Over
                    {
                        newGame();
                    }
                }
                else    //Exit with saveGame set to false
                {
                    saveGame = false;
                    mForm.saveGameOnExitToolStripMenuItem.Checked = false;
                    newGame();
                }
                //load preferences regardless of whether saveGame was enabled
                lastMove = lData.sLastMove;
                mForm.showLastMoveToolStripMenuItem.Checked = lastMove;
                rotate = lData.sRotate;
                mForm.rotateBoardToolStripMenuItem.Checked = rotate;
                string theme = lData.sTheme;

                for (int i = 0; i < themeList.Count(); i++)
                {
                    if (themeList[i].GetName().Name == theme)
                    {
                        themeIndex = i;
                    }
                }
                setTheme();

                if(firstGame == true)
                {
                    changeTheme();
                }
            }
            catch(InvalidCastException) //Error loading data
            {
                newGame();
            }
        }

        public void newGame()
        {
            NewGame play = new NewGame(this, mForm);
            play.ShowDialog();
        }

        public void themeForm()
        {
            ChangeTheme change = new ChangeTheme(this);
            change.ShowDialog();
        }

        [Serializable]
        private class saveData
        {
            public piece[,] sBoard { get; private set; }
            public string sOffensiveTeam { get; private set; }
            public string sTheme { get; private set; }
            public string sBaseOnBottom { get; private set; }
            public bool sOnePlayer { get; private set; }
            public bool sMedMode { get; private set; }
            public bool sHardMode { get; private set; }
            public bool sLastMove { get; private set; }
            public bool sSaveGame { get; private set; }
            public bool sGameOverExit { get; private set; }
            public bool sRotate { get; private set; }

            public saveData(piece[,] p1, string p2, string p3, string p4, bool p5, bool p6, bool p7, bool p8, bool p9, bool p10, bool p11)
            {
                this.sBoard = p1;
                this.sOffensiveTeam = p2;
                this.sTheme = p3;
                this.sBaseOnBottom = p4;
                this.sOnePlayer = p5;
                this.sMedMode = p6;
                this.sHardMode = p7;
                this.sLastMove = p8;
                this.sSaveGame = p9;
                this.sGameOverExit = p10;
                this.sRotate = p11;
            }
        }

        [Serializable]
        public struct piece
        {
            public string color { get; set; }
            public string job { get; set; }
            public bool firstMove { get; set; }
        }

        [Serializable]
        public struct coordinate
        {
            public int x { get; set; }
            public int y { get; set; }

            public coordinate(int p1, int p2) : this()
            {
                this.x = p1;
                this.y = p2;
            }
        }

        [Serializable]
        public class move
        {
            //represents a move that a piece can do
            public coordinate pieceSpot { get; set; }   //starting position
            public coordinate moveSpot { get; set; }    //ending position
            public int value { get; set; }              //how good the move is

            public move(coordinate p1, coordinate p2, int p3)
            {
                this.pieceSpot = p1;
                this.moveSpot = p2;
                this.value = p3;
            }

            public move() { }
        }

        [Serializable]
        public struct historyNode
        {
            public move step;               //move that happened previously
            public piece captured;          //piece that move captured, if no capture, use null
            public bool pawnTransform;      //did a pawn transformation happen?
            public bool skip;               //undo next move also
            public bool virgin;             //was this the piece's first move?
            public string whoIsOnBottom;    //who is going from bottom to top?

            public historyNode(move p1, piece p2, bool p3, bool p4, bool p5, string p6)
            {
                this.step = p1;
                this.captured = p2;
                this.pawnTransform = p3;
                this.skip = p4;
                this.virgin = p5;
                this.whoIsOnBottom = p6;
            }
        }

        //the next few functions define the rules for what piece can move where in any situation
        //does not account for check restrictions
        //takes coordinate and returns list of possible moves for that piece

        private List<move> rookMoves(coordinate current)
        {
            string oppositeColor;
            move availableMove = new move();
            int availableX = current.x;             //put coordinate in temp variable to manipulate while preserving original
            int availableY = current.y;
            List<move> availableList = new List<move>();
            coordinate moveCoor = new coordinate(); //when found possible move, put in this variable to add to list
            string pieceColor = board[current.x, current.y].color;

            if (pieceColor == "light")
            {
                oppositeColor = "dark";
            }

            else
            {
                oppositeColor = "light";
            }

            //search up
            availableY++;
            while(availableY < 8)
            {
                if (board[availableX, availableY].color == pieceColor)  //if same team
                {
                    break;                                              //can't go past
                }

                else if (board[availableX, availableY].color == oppositeColor)   //if enemy
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove = new move(current, moveCoor, 0);
                    availableList.Add(availableMove);       //add to list
                    break;                                  //can't go past
                }

                else                                        //if unoccupied
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove = new move(current, moveCoor, 0);
                    availableList.Add(availableMove);       //add to list
                    availableY++;                           //try next spot
                }
            }

            //search left
            availableX = current.x;
            availableY = current.y;
            availableX--;
            while (availableX >= 0)
            {
                if (board[availableX, availableY].color == pieceColor)
                {
                    break;
                }

                else if (board[availableX, availableY].color == oppositeColor)
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove = new move(current, moveCoor, 0);
                    availableList.Add(availableMove);
                    break;
                }

                else
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove = new move(current, moveCoor, 0);
                    availableList.Add(availableMove);
                    availableX--;
                }
            }

            //search down
            availableX = current.x;
            availableY = current.y;
            availableY--;
            while (availableY >= 0)
            {
                if (board[availableX, availableY].color == pieceColor)
                {
                    break;
                }

                else if (board[availableX, availableY].color == oppositeColor)
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove = new move(current, moveCoor, 0);
                    availableList.Add(availableMove);
                    break;
                }

                else
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove = new move(current, moveCoor, 0);
                    availableList.Add(availableMove);
                    availableY--;
                }
            }

            //search right
            availableX = current.x;
            availableY = current.y;
            availableX++;
            while (availableX < 8)
            {
                if (board[availableX, availableY].color == pieceColor)
                {
                    break;
                }

                else if (board[availableX, availableY].color == oppositeColor)
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove = new move(current, moveCoor, 0);
                    availableList.Add(availableMove);
                    break;
                }

                else
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove = new move(current, moveCoor, 0);
                    availableList.Add(availableMove);
                    availableX++;
                }
            }
            return availableList;
        }

        private List<move> knightMoves(coordinate current)
        {
            move availableMove = new move();
            int availableX = current.x;
            int availableY = current.y;
            List<move> availableList = new List<move>();
            coordinate moveCoor = new coordinate();
            string pieceColor = board[current.x, current.y].color;
                    
            //search up.right
            availableY += 2;
            availableX++;
            if (availableY < 8 && availableX < 8)
            {
                if (board[availableX, availableY].color != pieceColor)
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove = new move(current, moveCoor, 0);
                    availableList.Add(availableMove);
                }
            }

            //search up.left
            availableX = current.x;
            availableY = current.y;
            availableY += 2;
            availableX--;
            if (availableY < 8 && availableX >= 0)
            {
                if (board[availableX, availableY].color != pieceColor)
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove = new move(current, moveCoor, 0);
                    availableList.Add(availableMove);
                }
            }

            //search left.up
            availableX = current.x;
            availableY = current.y;
            availableY++;
            availableX -= 2;
            if (availableY < 8 && availableX >= 0)
            {
                if (board[availableX, availableY].color != pieceColor)
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove = new move(current, moveCoor, 0);
                    availableList.Add(availableMove);
                }
            }

            //search left.down
            availableX = current.x;
            availableY = current.y;
            availableY--;
            availableX -= 2;
            if (availableY >= 0 && availableX >= 0)
            {
                if (board[availableX, availableY].color != pieceColor)
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove = new move(current, moveCoor, 0);
                    availableList.Add(availableMove);
                }
            }

            //search down.left
            availableX = current.x;
            availableY = current.y;
            availableY -= 2;
            availableX--;
            if (availableY >= 0 && availableX >= 0)
            {
                if (board[availableX, availableY].color != pieceColor)
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove = new move(current, moveCoor, 0);
                    availableList.Add(availableMove);
                }
            }

            //search down.right
            availableX = current.x;
            availableY = current.y;
            availableY -= 2;
            availableX++;
            if (availableY >= 0 && availableX < 8)
            {
                if (board[availableX, availableY].color != pieceColor)
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove = new move(current, moveCoor, 0);
                    availableList.Add(availableMove);
                }
            }

            //search right.down
            availableX = current.x;
            availableY = current.y;
            availableY--;
            availableX += 2;
            if (availableY >= 0 && availableX < 8)
            {
                if (board[availableX, availableY].color != pieceColor)
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove = new move(current, moveCoor, 0);
                    availableList.Add(availableMove);
                }
            }

            //search right.up
            availableX = current.x;
            availableY = current.y;
            availableY++;
            availableX += 2;
            if (availableY < 8 && availableX < 8)
            {
                if (board[availableX, availableY].color != pieceColor)
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove = new move(current, moveCoor, 0);
                    availableList.Add(availableMove);
                }
            }
            return availableList;
        }

        private List<move> bishopMoves(coordinate current)
        {
            string oppositeColor;
            move availableMove = new move();
            int availableX = current.x;
            int availableY = current.y;
            List<move> availableList = new List<move>();
            coordinate moveCoor = new coordinate();
            string pieceColor = board[current.x, current.y].color;

            if (pieceColor == "light")
            {
                oppositeColor = "dark";
            }

            else
            {
                oppositeColor = "light";
            }

            //search upper right
            availableX++;
            availableY++;
            while(availableX < 8 && availableY < 8)
            {
                if (board[availableX, availableY].color == pieceColor)
                {
                    break;
                }

                else if (board[availableX, availableY].color == oppositeColor)
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove = new move(current, moveCoor, 0);
                    availableList.Add(availableMove);
                    break;
                }

                else
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove = new move(current, moveCoor, 0);
                    availableList.Add(availableMove);
                    availableX++;
                    availableY++;
                }
            }

            //search upper left
            availableX = current.x;
            availableY = current.y;
            availableX--;
            availableY++;
            while (availableX >= 0 && availableY < 8)
            {
                if (board[availableX, availableY].color == pieceColor)
                {
                    break;
                }

                else if (board[availableX, availableY].color == oppositeColor)
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove = new move(current, moveCoor, 0);
                    availableList.Add(availableMove);
                    break;
                }

                else
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove = new move(current, moveCoor, 0);
                    availableList.Add(availableMove);
                    availableX--;
                    availableY++;
                }
            }

            //search lower left
            availableX = current.x;
            availableY = current.y;
            availableX--;
            availableY--;
            while (availableX >= 0 && availableY >= 0)
            {
                if (board[availableX, availableY].color == pieceColor)
                {
                    break;
                }

                else if (board[availableX, availableY].color == oppositeColor)
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove = new move(current, moveCoor, 0);
                    availableList.Add(availableMove);
                    break;
                }

                else
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove = new move(current, moveCoor, 0);
                    availableList.Add(availableMove);
                    availableX--;
                    availableY--;
                }
            }

            //search lower right
            availableX = current.x;
            availableY = current.y;
            availableX++;
            availableY--;
            while (availableX < 8 && availableY >= 0)
            {
                if (board[availableX, availableY].color == pieceColor)
                {
                    break;
                }

                else if (board[availableX, availableY].color == oppositeColor)
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove = new move(current, moveCoor, 0);
                    availableList.Add(availableMove);
                    break;
                }

                else
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove = new move(current, moveCoor, 0);
                    availableList.Add(availableMove);
                    availableX++;
                    availableY--;
                }
            }
            return availableList;
        }

        private List<move> kingMoves(coordinate current)
        {
            move availableMove = new move();
            int availableX = current.x;
            int availableY = current.y;
            List<move> availableList = new List<move>();
            coordinate moveCoor = new coordinate();
            string pieceColor = board[current.x, current.y].color;

            //search up
            availableY++;
            if(availableY < 8)
            {
                if (board[availableX, availableY].color != pieceColor)
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove = new move(current, moveCoor, 0);
                    availableList.Add(availableMove);
                }
            }

            //search upper left
            availableX = current.x;
            availableY = current.y;
            availableY++;
            availableX--;
            if (availableY < 8 && availableX >= 0)
            {
                if (board[availableX, availableY].color != pieceColor)
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove = new move(current, moveCoor, 0);
                    availableList.Add(availableMove);
                }
            }

            //search left
            availableX = current.x;
            availableY = current.y;
            availableX--;
            if (availableX >= 0)
            {
                if (board[availableX, availableY].color != pieceColor)
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove = new move(current, moveCoor, 0);
                    availableList.Add(availableMove);
                }
            }

            //search lower left
            availableX = current.x;
            availableY = current.y;
            availableY--;
            availableX--;
            if (availableY >= 0 && availableX >= 0)
            {
                if (board[availableX, availableY].color != pieceColor)
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove = new move(current, moveCoor, 0);
                    availableList.Add(availableMove);
                }
            }

            //search down
            availableX = current.x;
            availableY = current.y;
            availableY--;
            if (availableY >= 0)
            {
                if (board[availableX, availableY].color != pieceColor)
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove = new move(current, moveCoor, 0);
                    availableList.Add(availableMove);
                }
            }

            //search lower right
            availableX = current.x;
            availableY = current.y;
            availableY--;
            availableX++;
            if (availableY >= 0 && availableX < 8)
            {
                if (board[availableX, availableY].color != pieceColor)
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove = new move(current, moveCoor, 0);
                    availableList.Add(availableMove);
                }
            }

            //search right
            availableX = current.x;
            availableY = current.y;
            availableX++;
            if (availableX < 8)
            {
                if (board[availableX, availableY].color != pieceColor)
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove = new move(current, moveCoor, 0);
                    availableList.Add(availableMove);
                }
            }

            //search upper right
            availableX = current.x;
            availableY = current.y;
            availableY++;
            availableX++;
            if (availableY < 8 && availableX < 8)
            {
                if (board[availableX, availableY].color != pieceColor)
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove = new move(current, moveCoor, 0);
                    availableList.Add(availableMove);
                }
            }

            //search for castleing opportunity
            if (board[current.x, current.y].firstMove == true)//if king's first move
            {
                if (pieceColor == baseOnBottom)
                {
                    if (board[0, 0].firstMove == true)//if left rook's first move
                    {
                        if (board[1, 0].job == null && board[2, 0].job == null && board[3, 0].job == null)
                        {
                            moveCoor.x = 2;
                            moveCoor.y = 0;
                            availableMove = new move(current, moveCoor, 0);
                            availableList.Add(availableMove);
                        }
                    }

                    if (board[7, 0].firstMove == true)//if right rook's first move
                    {
                        if (board[6, 0].job == null && board[5, 0].job == null)
                        {
                            moveCoor.x = 6;
                            moveCoor.y = 0;
                            availableMove = new move(current, moveCoor, 0);
                            availableList.Add(availableMove);
                        }
                    }
                }

                else
                {
                    if (board[0, 7].firstMove == true)//if left rook's first move
                    {
                        //if clear path from rook to king
                        if (board[1, 7].job == null && board[2, 7].job == null && board[3, 7].job == null)
                        {
                            moveCoor.x = 2;
                            moveCoor.y = 7;
                            availableMove = new move(current, moveCoor, 0);
                            availableList.Add(availableMove);
                        }
                    }

                    if (board[7, 7].firstMove == true)//if right rook's first move
                    {
                        if (board[6, 7].job == null && board[5, 7].job == null)
                        {
                            moveCoor.x = 6;
                            moveCoor.y = 7;
                            availableMove = new move(current, moveCoor, 0);
                            availableList.Add(availableMove);
                        }
                    }
                }
            }
            return availableList;
        }

        private List<move> pawnMoves(coordinate current)
        {
            string oppositeColor;
            move availableMove = new move();
            int availableX = current.x;
            int availableY = current.y;
            List<move> availableList = new List<move>();
            coordinate moveCoor = new coordinate();
            string pieceColor = board[current.x, current.y].color;

            if(pieceColor == "light")
            {
                oppositeColor = "dark";
            }
            else
            {
                oppositeColor = "light";
            }

            if (pieceColor == baseOnBottom)
            {
                //search up
                availableY++;
                if (availableY < 8)
                {
                    if (board[availableX, availableY].color == null)
                    {
                        moveCoor.x = availableX;
                        moveCoor.y = availableY;
                        availableMove = new move(current, moveCoor, 0);
                        availableList.Add(availableMove);

                        //search first move
                        availableY++;
                        if (availableY < 8 && board[current.x, current.y].firstMove == true)
                        {
                            if (board[availableX, availableY].color == null)
                            {
                                moveCoor.x = availableX;
                                moveCoor.y = availableY;
                                availableMove = new move(current, moveCoor, 0);
                                availableList.Add(availableMove);
                            }
                        }
                        availableY--;
                    }
                }

                //search upper right
                availableX++;
                if (availableY < 8 && availableX < 8)
                {
                    if (board[availableX, availableY].color == oppositeColor)
                    {
                        moveCoor.x = availableX;
                        moveCoor.y = availableY;
                        availableMove = new move(current, moveCoor, 0);
                        availableList.Add(availableMove);
                    }
                }

                //search upper left
                availableX -= 2;
                if (availableY < 8 && availableX >= 0)
                {
                    if (board[availableX, availableY].color == oppositeColor)
                    {
                        moveCoor.x = availableX;
                        moveCoor.y = availableY;
                        availableMove = new move(current, moveCoor, 0);
                        availableList.Add(availableMove);
                    }
                }
            }

            else
            {
                //search down
                availableY--;
                if (availableY >= 0)
                {
                    if (board[availableX, availableY].color == null)
                    {
                        moveCoor.x = availableX;
                        moveCoor.y = availableY;
                        availableMove = new move(current, moveCoor, 0);
                        availableList.Add(availableMove);

                        //search first move
                        availableY--;
                        if (availableY >= 0 && board[current.x, current.y].firstMove == true)
                        {
                            if (board[availableX, availableY].color == null)
                            {
                                moveCoor.x = availableX;
                                moveCoor.y = availableY;
                                availableMove = new move(current, moveCoor, 0);
                                availableList.Add(availableMove);
                            }
                        }
                        availableY++;
                    }
                }

                //search lower right
                availableX++;
                if (availableY >= 0 && availableX < 8)
                {
                    if (board[availableX, availableY].color == oppositeColor)
                    {
                        moveCoor.x = availableX;
                        moveCoor.y = availableY;
                        availableMove = new move(current, moveCoor, 0);
                        availableList.Add(availableMove);
                    }
                }

                //search lower left
                availableX -= 2;
                if (availableY >= 0 && availableX >= 0)
                {
                    if (board[availableX, availableY].color == oppositeColor)
                    {
                        moveCoor.x = availableX;
                        moveCoor.y = availableY;
                        availableMove = new move(current, moveCoor, 0);
                        availableList.Add(availableMove);
                    }
                }
            }
            return availableList;
        }
    }
}
