using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.IO;
using System.Reflection;
using BobbyFischer.Properties;

//the back-end where all the business logic is determined

namespace BobbyFischer
{
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
        public string offensiveTeam;                                //which side is on the offense
        public bool medMode;                                        //difficulty level
        public bool hardMode;                                       //difficulty level
        public bool firstGame;                                      //has newGame() been called yet?
        private coordinate prevSelected;                            //where the cursor clicked previously
        public List<Assembly> themeList;
        public int themeIndex = 0;                                  //which theme is currently in use
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
        public bool movablePieceSelected = false;                       //if true, the next click will move the selected piece if possible
        private List<move> possible = new List<move>();                 //list of all possible moves
        private static Random rnd = new Random();

        public Chess(Board mainForm)
        {
            this.mForm = mainForm;
        }

        public void createGrid()
        {
            //creates new array with pieces in starting position

            board = new piece[8, 8];
                
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (y == 0)
                    {
                        board[x, 0].color = "light";
                    }

                    else if (y == 1)
                    {
                        board[x, 1].color = "light";
                        board[x, 1].job = "Pawn";
                        board[x, 1].firstMove = true;
                    }

                    else if (y == 6)
                    {
                        board[x, 6].color = "dark";
                        board[x, 6].job = "Pawn";
                        board[x, 6].firstMove = true;
                    }

                    else if(y == 7)
                    {
                        board[x, 7].color = "dark";
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

        private List<move> hardLogic(List<move> pos)
        {
            //gets executed if player selects medium or hard mode

            List<move> capturableMoves = new List<move>();

            foreach(move p in pos)//only look at moves that can capture a piece
            {
                if(board[p.moveSpot.x, p.moveSpot.y].color == "light")
                {
                    capturableMoves.Add(p);
                }
            }
            if(capturableMoves.Count > 0)//if there are any capturable moves
            {
                pos.Clear();
                pos.AddRange(capturableMoves);//replace possible list

                if(hardMode == true)//priority system of most valuable piece to capture
                {
                    //list of moves that can capture piece of said job

                    List<move> queen = new List<move>();
                    List<move> rook = new List<move>();
                    List<move> bishop = new List<move>();
                    List<move> knight = new List<move>();
                    List<move> pawn = new List<move>();

                    foreach(move h in pos)//put moves in apropriate list
                    {
                        switch(board[h.moveSpot.x, h.moveSpot.y].job)
                        {
                            case "Queen":
                            queen.Add(h);
                            break;
                            case "Rook":
                            rook.Add(h);
                            break;
                            case "Bishop":
                            bishop.Add(h);
                            break;
                            case "Knight":
                            knight.Add(h);
                            break;
                            default:
                            pawn.Add(h);
                            break;
                        }
                    }

                    //queen capture gets first priority, then rook, and so on

                    if(queen.Count > 0)
                    {
                        return queen;
                    }
                    else if(rook.Count > 0)
                    {
                        return rook;
                    }
                    else if(bishop.Count > 0)
                    {
                        return bishop;
                    }
                    else if(knight.Count > 0)
                    {
                        return knight;
                    }
                    else
                    {
                        return pawn;
                    }
                }
            }
            return pos;//if no capturable moves, return list given, same as easy mode
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

                //do moves
                board[to.x, to.y].color = offensiveTeam;
                board[to.x, to.y].job = board[from.x, from.y].job.ToString();
                board[from.x, from.y].color = null;
                board[from.x, from.y].job = null;

                //see if in check
                inCheck = isInCheck(offensiveTeam);

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

            if(offensiveTeam == "light")
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
                    node = new historyNode(castleMove, board[3, yCoor], false, true, true);
                    history.Push(node);
                    movePiece(newCastleCoor, board[0, yCoor], oldCastleCoor);
                }

                else if(toSpot.x == 6 && toSpot.y == yCoor) //if moving two spaces to the right
                {
                    coordinate newCastleCoor = new coordinate(5, yCoor);
                    coordinate oldCastleCoor = new coordinate(7, yCoor);
                    castleMove.moveSpot = newCastleCoor;
                    castleMove.pieceSpot = oldCastleCoor;
                    node = new historyNode(castleMove, board[5, yCoor], false, true, true);
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
                            if (board[currentCell.x, currentCell.y].color == "light" && currentCell.y == 7)
                            {
                                PawnTransformation transform = new PawnTransformation(currentCell, this);
                                transform.ShowDialog();
                                node = new historyNode(curTurn, captured, true, false, false);
                            }

                            else if (board[currentCell.x, currentCell.y].color == "dark" && currentCell.y == 0)
                            {
                                PawnTransformation transform = new PawnTransformation(currentCell, this);
                                transform.ShowDialog();
                                node = new historyNode(curTurn, captured, true, false, false);
                            }

                            else    //if pawn, but no transform
                            {
                                node = new historyNode(curTurn, captured, false, false, virginMove);
                            }
                        }

                        else    //not pawn
                        {
                            node = new historyNode(curTurn, captured, false, false, virginMove);
                        }

                        history.Push(node);
                        mForm.undoToolStripMenuItem.Enabled = true;

                        if (mForm.showLastMoveToolStripMenuItem.Checked == true)
                        {
                            clearToAndFrom();
                            coordinateToPictureBox(curTurn.pieceSpot).BackgroundImage = Resources.from;
                            coordinateToPictureBox(curTurn.moveSpot).BackgroundImage = Resources.to;
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
            }
            else
            {
                offensiveTeam = "light";
                endOfGame = isInCheckmate(offensiveTeam, getLightPieces());
            }

            if (endOfGame == false && onePlayer == true)
            {
                foreach (coordinate cell in getDarkPieces()) //for all dark pieces
                {
                    possibleWithoutCheck.AddRange(getCheckRestrictedMoves(cell));  //get all moves possible without going into check
                }

                compTurn(possibleWithoutCheck);
                isInCheckmate("light", getLightPieces()); //did computer turn put player in checkmate?
                offensiveTeam = "light";
            }
        }

        private void compTurn(List<move> poss)
        {
            //computer's turn
            historyNode node;

            if (medMode == true || hardMode == true)
            {
                hardLogic(poss);
            }

            int r = rnd.Next(0, poss.Count);//choose random move
            move curTurn = poss[r];
            coordinate newSpot = new coordinate(curTurn.moveSpot.x, curTurn.moveSpot.y);
            coordinate oldSpot = new coordinate(curTurn.pieceSpot.x, curTurn.pieceSpot.y);

            piece captured = board[newSpot.x, newSpot.y];
            bool virginMove = board[oldSpot.x, oldSpot.y].firstMove;
            movePiece(newSpot, board[oldSpot.x, oldSpot.y], oldSpot);

            if (board[newSpot.x, newSpot.y].job == "Pawn" && newSpot.y == 0)//if pawn makes it to last row
            {
                r = rnd.Next(0, 4); //choose random piece to transform into
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
                node = new historyNode(curTurn, captured, true, true, false);
            }

            else
            {
                node = new historyNode(curTurn, captured, false, true, virginMove);
            }

            history.Push(node);

            if (mForm.showLastMoveToolStripMenuItem.Checked == true)
            {
                clearToAndFrom();
                coordinateToPictureBox(curTurn.pieceSpot).BackgroundImage = Resources.from;
                coordinateToPictureBox(curTurn.moveSpot).BackgroundImage = Resources.to;
            }

            if (board[newSpot.x, newSpot.y].job == "King")
            {
                castling(curTurn);//check if move is a castling
            }
        }

        private void movePiece(coordinate newCell, piece pPiece, coordinate oldCell)
        {
            //overwrite current cell
            board[newCell.x, newCell.y].color = offensiveTeam;
            board[newCell.x, newCell.y].job = pPiece.job;

            //delete prev cell
            board[oldCell.x, oldCell.y].color = null;
            board[oldCell.x, oldCell.y].job = null;

            //overwrite current image
            coordinateToPictureBox(newCell).Image = matchPicture(pPiece);  //take previous piece picture and put it in current cell picture box

            //delete prev image
            coordinateToPictureBox(oldCell).Image = null;

            movablePieceSelected = false;
            board[newCell.x, newCell.y].firstMove = false;
        }

        public void undo()
        {
            //moves pieces backwards
            Image pawnPic;

            historyNode node = history.Pop();
            piece to = board[node.step.moveSpot.x, node.step.moveSpot.y];
            piece from = board[node.step.pieceSpot.x, node.step.pieceSpot.y];
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

                board[node.step.pieceSpot.x, node.step.pieceSpot.y].job = "Pawn";
                coordinateToPictureBox(node.step.pieceSpot).Image = pawnPic;
            }

            else
            {
                board[node.step.pieceSpot.x, node.step.pieceSpot.y].job = to.job;
                coordinateToPictureBox(node.step.pieceSpot).Image = matchPicture(to);
            }

            board[node.step.pieceSpot.x, node.step.pieceSpot.y].color = to.color;
            board[node.step.pieceSpot.x, node.step.pieceSpot.y].firstMove = node.virgin;

            //put captured piece back
            board[node.step.moveSpot.x, node.step.moveSpot.y].job = node.captured.job;
            board[node.step.moveSpot.x, node.step.moveSpot.y].color = node.captured.color;
            board[node.step.moveSpot.x, node.step.moveSpot.y].firstMove = node.captured.firstMove;
            coordinateToPictureBox(node.step.moveSpot).Image = matchPicture(node.captured);

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
            mForm.pictureBox1.BackgroundImage = null;
            mForm.pictureBox2.BackgroundImage = null;
            mForm.pictureBox3.BackgroundImage = null;
            mForm.pictureBox4.BackgroundImage = null;
            mForm.pictureBox5.BackgroundImage = null;
            mForm.pictureBox6.BackgroundImage = null;
            mForm.pictureBox7.BackgroundImage = null;
            mForm.pictureBox8.BackgroundImage = null;
            mForm.pictureBox9.BackgroundImage = null;
            mForm.pictureBox10.BackgroundImage = null;
            mForm.pictureBox11.BackgroundImage = null;
            mForm.pictureBox12.BackgroundImage = null;
            mForm.pictureBox13.BackgroundImage = null;
            mForm.pictureBox14.BackgroundImage = null;
            mForm.pictureBox15.BackgroundImage = null;
            mForm.pictureBox16.BackgroundImage = null;
            mForm.pictureBox17.BackgroundImage = null;
            mForm.pictureBox18.BackgroundImage = null;
            mForm.pictureBox19.BackgroundImage = null;
            mForm.pictureBox20.BackgroundImage = null;
            mForm.pictureBox21.BackgroundImage = null;
            mForm.pictureBox22.BackgroundImage = null;
            mForm.pictureBox23.BackgroundImage = null;
            mForm.pictureBox24.BackgroundImage = null;
            mForm.pictureBox25.BackgroundImage = null;
            mForm.pictureBox26.BackgroundImage = null;
            mForm.pictureBox27.BackgroundImage = null;
            mForm.pictureBox28.BackgroundImage = null;
            mForm.pictureBox29.BackgroundImage = null;
            mForm.pictureBox30.BackgroundImage = null;
            mForm.pictureBox31.BackgroundImage = null;
            mForm.pictureBox32.BackgroundImage = null;
            mForm.pictureBox33.BackgroundImage = null;
            mForm.pictureBox34.BackgroundImage = null;
            mForm.pictureBox35.BackgroundImage = null;
            mForm.pictureBox36.BackgroundImage = null;
            mForm.pictureBox37.BackgroundImage = null;
            mForm.pictureBox38.BackgroundImage = null;
            mForm.pictureBox39.BackgroundImage = null;
            mForm.pictureBox40.BackgroundImage = null;
            mForm.pictureBox41.BackgroundImage = null;
            mForm.pictureBox42.BackgroundImage = null;
            mForm.pictureBox43.BackgroundImage = null;
            mForm.pictureBox44.BackgroundImage = null;
            mForm.pictureBox45.BackgroundImage = null;
            mForm.pictureBox46.BackgroundImage = null;
            mForm.pictureBox47.BackgroundImage = null;
            mForm.pictureBox48.BackgroundImage = null;
            mForm.pictureBox49.BackgroundImage = null;
            mForm.pictureBox50.BackgroundImage = null;
            mForm.pictureBox51.BackgroundImage = null;
            mForm.pictureBox52.BackgroundImage = null;
            mForm.pictureBox53.BackgroundImage = null;
            mForm.pictureBox54.BackgroundImage = null;
            mForm.pictureBox55.BackgroundImage = null;
            mForm.pictureBox56.BackgroundImage = null;
            mForm.pictureBox57.BackgroundImage = null;
            mForm.pictureBox58.BackgroundImage = null;
            mForm.pictureBox59.BackgroundImage = null;
            mForm.pictureBox60.BackgroundImage = null;
            mForm.pictureBox61.BackgroundImage = null;
            mForm.pictureBox62.BackgroundImage = null;
            mForm.pictureBox63.BackgroundImage = null;
            mForm.pictureBox64.BackgroundImage = null;
        }

        public void setImages()
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

        public void changeTheme()
        {
            //calls matchPicture() on each piece and puts image in PictureBox
            List<coordinate> pieceList = new List<coordinate>();

            pieceList = getAllPieces();

            foreach (coordinate spot in pieceList)
            {
                coordinateToPictureBox(spot).Image = matchPicture(board[spot.x, spot.y]);
            }
        }

        public void setTheme()
        {
            //sets image variables based on themeIndex
            System.IO.Stream lKingFile = themeList[themeIndex].GetManifestResourceStream(themeList[themeIndex].GetName().Name + ".lKing.png");
            System.IO.Stream lQueenFile = themeList[themeIndex].GetManifestResourceStream(themeList[themeIndex].GetName().Name + ".lQueen.png");
            System.IO.Stream lBishopFile = themeList[themeIndex].GetManifestResourceStream(themeList[themeIndex].GetName().Name + ".lBishop.png");
            System.IO.Stream lKnightFile = themeList[themeIndex].GetManifestResourceStream(themeList[themeIndex].GetName().Name + ".lKnight.png");
            System.IO.Stream lRookFile = themeList[themeIndex].GetManifestResourceStream(themeList[themeIndex].GetName().Name + ".lRook.png");
            System.IO.Stream lPawnFile = themeList[themeIndex].GetManifestResourceStream(themeList[themeIndex].GetName().Name + ".lPawn.png");
            System.IO.Stream dKingFile = themeList[themeIndex].GetManifestResourceStream(themeList[themeIndex].GetName().Name + ".dKing.png");
            System.IO.Stream dQueenFile = themeList[themeIndex].GetManifestResourceStream(themeList[themeIndex].GetName().Name + ".dQueen.png");
            System.IO.Stream dBishopFile = themeList[themeIndex].GetManifestResourceStream(themeList[themeIndex].GetName().Name + ".dBishop.png");
            System.IO.Stream dKnightFile = themeList[themeIndex].GetManifestResourceStream(themeList[themeIndex].GetName().Name + ".dKnight.png");
            System.IO.Stream dRookFile = themeList[themeIndex].GetManifestResourceStream(themeList[themeIndex].GetName().Name + ".dRook.png");
            System.IO.Stream dPawnFile = themeList[themeIndex].GetManifestResourceStream(themeList[themeIndex].GetName().Name + ".dPawn.png");

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
            themeIndex = 0;
        }

        private void loadDlls()
        {
            //searches dlls in working directory and loads themes
            string path;
            AssemblyName an;
            Assembly assembly;
            themeList = new List<Assembly>();
            string[] dllFilePathArray = null;
            List<string> ignore = new List<string>();
            path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            
            bool themesFound = false;

            while (themesFound == false)
            {
                try
                {
                    dllFilePathArray = Directory.GetFiles(path, "*.dll");

                    foreach (string dllFilePath in dllFilePathArray.Except(ignore))
                    {
                        an = AssemblyName.GetAssemblyName(dllFilePath);
                        assembly = Assembly.Load(an);
                        themeList.Add(assembly);
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
                    }
                }
                else
                {
                    themesFound = true;
                }
            }
        }

        public void newGame()
        {
            NewGame play = new NewGame(this);
            play.ShowDialog();
        }

        public void themeForm()
        {
            ChangeTheme change = new ChangeTheme(this);
            change.ShowDialog();
        }

        public struct piece
        {
            public string color { get; set; }
            public string job { get; set; }
            public bool firstMove { get; set; }
        }

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

        public struct move
        {
            //represents a move that a piece can do, includes starting position and ending position
            public coordinate pieceSpot { get; set; }    //starting position
            public coordinate moveSpot { get; set; }     //ending position
        }

        public struct historyNode
        {
            public move step;           //move that happened previously
            public piece captured;      //piece that move captured, if no capture, use null
            public bool pawnTransform;  //did a pawn transformation happen?
            public bool skip;           //undo next move also
            public bool virgin;         //was this the piece's first move?

            public historyNode(move p1, piece p2, bool p3, bool p4, bool p5)
            {
                this.step = p1;
                this.captured = p2;
                this.pawnTransform = p3;
                this.skip = p4;
                this.virgin = p5;
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
            availableMove.pieceSpot = current;
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
                    availableMove.moveSpot = moveCoor;
                    availableList.Add(availableMove);       //add to list
                    break;                                  //can't go past
                }

                else                                        //if unoccupied
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove.moveSpot = moveCoor;
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
                    availableMove.moveSpot = moveCoor;
                    availableList.Add(availableMove);
                    break;
                }

                else
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove.moveSpot = moveCoor;
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
                    availableMove.moveSpot = moveCoor;
                    availableList.Add(availableMove);
                    break;
                }

                else
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove.moveSpot = moveCoor;
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
                    availableMove.moveSpot = moveCoor;
                    availableList.Add(availableMove);
                    break;
                }

                else
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove.moveSpot = moveCoor;
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
            availableMove.pieceSpot = current;
                    
            //search up.right
            availableY += 2;
            availableX++;
            if (availableY < 8 && availableX < 8)
            {
                if (board[availableX, availableY].color != pieceColor)
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove.moveSpot = moveCoor;
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
                    availableMove.moveSpot = moveCoor;
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
                    availableMove.moveSpot = moveCoor;
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
                    availableMove.moveSpot = moveCoor;
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
                    availableMove.moveSpot = moveCoor;
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
                    availableMove.moveSpot = moveCoor;
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
                    availableMove.moveSpot = moveCoor;
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
                    availableMove.moveSpot = moveCoor;
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
            availableMove.pieceSpot = current;

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
                    availableMove.moveSpot = moveCoor;
                    availableList.Add(availableMove);
                    break;
                }

                else
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove.moveSpot = moveCoor;
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
                    availableMove.moveSpot = moveCoor;
                    availableList.Add(availableMove);
                    break;
                }

                else
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove.moveSpot = moveCoor;
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
                    availableMove.moveSpot = moveCoor;
                    availableList.Add(availableMove);
                    break;
                }

                else
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove.moveSpot = moveCoor;
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
                    availableMove.moveSpot = moveCoor;
                    availableList.Add(availableMove);
                    break;
                }

                else
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove.moveSpot = moveCoor;
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
            availableMove.pieceSpot = current;
            string pieceColor = board[current.x, current.y].color;

            //search up
            availableY++;
            if(availableY < 8)
            {
                if (board[availableX, availableY].color != pieceColor)
                {
                    moveCoor.x = availableX;
                    moveCoor.y = availableY;
                    availableMove.moveSpot = moveCoor;
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
                    availableMove.moveSpot = moveCoor;
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
                    availableMove.moveSpot = moveCoor;
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
                    availableMove.moveSpot = moveCoor;
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
                    availableMove.moveSpot = moveCoor;
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
                    availableMove.moveSpot = moveCoor;
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
                    availableMove.moveSpot = moveCoor;
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
                    availableMove.moveSpot = moveCoor;
                    availableList.Add(availableMove);
                }
            }

            //search for castleing opportunity
            if (board[current.x, current.y].firstMove == true)//if king's first move
            {
                if (pieceColor == "light")
                {
                    if (board[0, 0].firstMove == true)//if left rook's first move
                    {
                        if (board[1, 0].job == null && board[2, 0].job == null && board[3, 0].job == null)
                        {
                            moveCoor.x = 2;
                            moveCoor.y = 0;
                            availableMove.moveSpot = moveCoor;
                            availableList.Add(availableMove);
                        }
                    }

                    if (board[7, 0].firstMove == true)//if right rook's first move
                    {
                        if (board[6, 0].job == null && board[5, 0].job == null)
                        {
                            moveCoor.x = 6;
                            moveCoor.y = 0;
                            availableMove.moveSpot = moveCoor;
                            availableList.Add(availableMove);
                        }
                    }
                }

                else if(pieceColor == "dark")
                {
                    if (board[0, 7].firstMove == true)//if left rook's first move
                    {
                        if (board[1, 7].job == null && board[2, 7].job == null && board[3, 7].job == null)//if clear path from rook to king
                        {
                            moveCoor.x = 2;
                            moveCoor.y = 7;
                            availableMove.moveSpot = moveCoor;
                            availableList.Add(availableMove);
                        }
                    }

                    if (board[7, 7].firstMove == true)//if right rook's first move
                    {
                        if (board[6, 7].job == null && board[5, 7].job == null)
                        {
                            moveCoor.x = 6;
                            moveCoor.y = 7;
                            availableMove.moveSpot = moveCoor;
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
            availableMove.pieceSpot = current;

            if (pieceColor == "light")
            {
                oppositeColor = "dark";

                //search up
                availableY++;
                if (availableY < 8)
                {
                    if (board[availableX, availableY].color == null)
                    {
                        moveCoor.x = availableX;
                        moveCoor.y = availableY;
                        availableMove.moveSpot = moveCoor;
                        availableList.Add(availableMove);

                        //search first move
                        availableY++;
                        if (availableY < 8 && board[current.x, current.y].firstMove == true)
                        {
                            if (board[availableX, availableY].color == null)
                            {
                                moveCoor.x = availableX;
                                moveCoor.y = availableY;
                                availableMove.moveSpot = moveCoor;
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
                        availableMove.moveSpot = moveCoor;
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
                        availableMove.moveSpot = moveCoor;
                        availableList.Add(availableMove);
                    }
                }
            }

            else
            {
                oppositeColor = "light";

                //search down
                availableY--;
                if (availableY >= 0)
                {
                    if (board[availableX, availableY].color == null)
                    {
                        moveCoor.x = availableX;
                        moveCoor.y = availableY;
                        availableMove.moveSpot = moveCoor;
                        availableList.Add(availableMove);

                        //search first move
                        availableY--;
                        if (availableY >= 0 && board[current.x, current.y].firstMove == true)
                        {
                            if (board[availableX, availableY].color == null)
                            {
                                moveCoor.x = availableX;
                                moveCoor.y = availableY;
                                availableMove.moveSpot = moveCoor;
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
                        availableMove.moveSpot = moveCoor;
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
                        availableMove.moveSpot = moveCoor;
                        availableList.Add(availableMove);
                    }
                }
            }
            return availableList;
        }
    }
}
