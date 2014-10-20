using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Data;
using System.Text;
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
        public Chess.coordinate currSelected;                       //where the cursor clicked
        public Chess.coordinate prevSelected;                       //where the cursor clicked previously
        public Image lKing;
        public Image lQueen;
        public Image lBishop;
        public Image lKnight;
        public Image lRook;
        public Image lPawn;
        public Image dKing;
        public Image dQueen;
        public Image dBishop;
        public Image dKnight;
        public Image dRook;
        public Image dPawn;
        public bool movablePieceSelected = false;                   //if true, the next click will move the selected piece if possible
        public List<Chess.move> possible = new List<Chess.move>();  //list of all possible moves
        public static Random rnd = new Random();

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

        public List<Chess.move> getMoves(Chess.coordinate spot)
        {
            //returns all possible moves of spot given in argument disregarding check restrictions
            //determines job of piece and calls apropriate function to get correct move list

            List<Chess.move> temp = new List<Chess.move>();

            switch(board[spot.x, spot.y].job)
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

        public List<Chess.move> hardLogic(List<Chess.move> pos)
        {
            //gets executed if player selects medium or hard mode

            List<Chess.move> capturableMoves = new List<Chess.move>();

            foreach(Chess.move p in pos)//only look at moves that can capture a piece
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

                    List<Chess.move> queen = new List<Chess.move>();
                    List<Chess.move> rook = new List<Chess.move>();
                    List<Chess.move> bishop = new List<Chess.move>();
                    List<Chess.move> knight = new List<Chess.move>();
                    List<Chess.move> pawn = new List<Chess.move>();

                    foreach(Chess.move h in pos)//put moves in apropriate list
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

        public List<Chess.coordinate> getDarkPieces()
        {
            //searches through board and returns list of coordinates where all dark pieces are located

            Chess.coordinate temp;
            List<Chess.coordinate> possiblePieces = new List<Chess.coordinate>();

            for(int y = 0; y < 8; y++)
            {
                for(int x = 0; x < 8; x++)
                {
                    if(board[x, y].color == "dark")
                    {
                        temp.x = x;
                        temp.y = y;
                        possiblePieces.Add(temp);
                    }
                }
            }
            return possiblePieces;
        }

        public List<Chess.coordinate> getLightPieces()
        {
            //searches through board and returns list of coordinates where all light pieces are located

            Chess.coordinate temp;
            List<Chess.coordinate> possiblePieces = new List<Chess.coordinate>();

            for(int y = 0; y < 8; y++)
            {
                for(int x = 0; x < 8; x++)
                {
                    if(board[x, y].color == "light")
                    {
                        temp.x = x;
                        temp.y = y;
                        possiblePieces.Add(temp);
                    }
                }
            }
            return possiblePieces;
        }

        public bool isInCheck(string teamInQuestion)
        {
            //returns whether team in question is in check

            List<Chess.coordinate> spots;
            List<Chess.move> poss = new List<Chess.move>();

            if(teamInQuestion == "dark")
            {
                spots = getLightPieces();//get all opposing team's pieces
            }

            else
            {
                spots = getDarkPieces();//get all opposing team's pieces
            }

            foreach(Chess.coordinate c in spots)
            {
                //get possible moves of opposing team,
                //doesn't matter if opposing team move gets them in check,
                //still a valid move for current team
                poss.AddRange(getMoves(c));
            }

            foreach(Chess.move m in poss)
            {
                //if opposing team's move can capture your king, you're in check
                if(board[m.moveSpot.x, m.moveSpot.y].job == "King" && board[m.moveSpot.x, m.moveSpot.y].color == teamInQuestion)
                {
                    return true;
                }
            }
            return false;
        }

        public List<Chess.move> getCheckRestrictedMoves(Chess.coordinate aPiece)
        {
            //takes single piece and returns list of moves that don't put player in check

            List<Chess.move> allPossible = new List<Chess.move>();
            List<Chess.move> possibleWithoutCheck = new List<Chess.move>();
            Chess.coordinate to;
            Chess.coordinate from;
            string toColor;
            string toJob;
            bool inCheck;

            allPossible = getMoves(aPiece);

            foreach(Chess.move m in allPossible)
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

        public bool isInCheckmate(string teamInQuestion, List<Chess.coordinate> availablePieces)
        {
            //takes list of pieces and returns whether or not player is in checkmate

            List<Chess.move> allPossible = new List<Chess.move>();
            List<Chess.move> possibleWithoutCheck = new List<Chess.move>();
            Chess.coordinate to;
            Chess.coordinate from;
            string toColor;
            string toJob;
            bool inCheck;

            //find all moves that can be done without going into check
            foreach(Chess.coordinate aPiece in availablePieces)
            {
                allPossible = getMoves(aPiece);

                foreach(Chess.move m in allPossible)
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

        public void castling(Chess.coordinate toSpot, Chess.coordinate fromSpot)
        {
            //if selected move is a castling move, move Rook in this function

            int yCoor;  //which row the move is being conducted in

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
                    Chess.coordinate newCastleCoor = new Chess.coordinate(3, yCoor);
                    Chess.coordinate oldCastleCoor = new Chess.coordinate(0, yCoor);
                    movePiece(newCastleCoor, board[oldCastleCoor.x, oldCastleCoor.y], oldCastleCoor);
                }

                else if(toSpot.x == 6 && toSpot.y == yCoor) //if moving two spaces to the right
                {
                    Chess.coordinate newCastleCoor = new Chess.coordinate(5, yCoor);
                    Chess.coordinate oldCastleCoor = new Chess.coordinate(7, yCoor);
                    movePiece(newCastleCoor, board[oldCastleCoor.x, oldCastleCoor.y], oldCastleCoor);
                }
            }
        }

        public void clicker(Chess.coordinate currentCell)
        {
            //human player's turn, gets called when player clicks on spot
            bool movableSpot;

            if (firstGame == true)  //blocks functionality if game hasn't started yet
            {
                Chess.piece currentPiece = board[currentCell.x, currentCell.y];

                if (currentPiece.color == offensiveTeam)//if selected own piece
                {
                    movablePieceSelected = true;
                    clearBackgroundImages();
                    coordinateToPictureBox(currentCell).BackgroundImage = Resources.selected;
                    prevSelected = currentCell;
                    possible.Clear();
                    possible.AddRange(getCheckRestrictedMoves(currentCell));

                    foreach (Chess.move m in possible)
                    {
                        coordinateToPictureBox(m.moveSpot).BackgroundImage = Resources.possible;
                    }
                }

                else if (movablePieceSelected == true)//if previously selected own piece
                {
                    movableSpot = false;

                    foreach (Chess.move m in possible)
                    {
                        if (currentCell.Equals(m.moveSpot))//if selected spot is in possible move list
                        {
                            movableSpot = true;
                        }
                    }

                    if (movableSpot == true)
                    {
                        if (board[prevSelected.x, prevSelected.y].job == "King")
                        {
                            castling(currentCell, prevSelected);//check if move is a castling
                        }

                        movePiece(currentCell, board[prevSelected.x, prevSelected.y], prevSelected);
                        clearBackgroundImages();

                        if (board[currentCell.x, currentCell.y].job == "Pawn")//if pawn makes it to last row
                        {
                            if (board[currentCell.x, currentCell.y].color == "light" && currentCell.y == 7)
                            {
                                PawnTransformation transform = new PawnTransformation(currentCell, this);
                                transform.ShowDialog();
                            }

                            if (board[currentCell.x, currentCell.y].color == "dark" && currentCell.y == 0)
                            {
                                PawnTransformation transform = new PawnTransformation(currentCell, this);
                                transform.ShowDialog();
                            }
                        }
                        betweenTurns();
                    }
                }
            }
        }

        public void betweenTurns()
        {
            //In between light and dark's turns
            List<Chess.move> possibleWithoutCheck = new List<Chess.move>();
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
                foreach (Chess.coordinate cell in getDarkPieces()) //for all dark pieces
                {
                    possibleWithoutCheck.AddRange(getCheckRestrictedMoves(cell));  //get all moves possible without going into check
                }

                compTurn(possibleWithoutCheck);
                isInCheckmate("light", getLightPieces()); //did computer turn put player in checkmate?
                offensiveTeam = "light";
            }
        }

        public void compTurn(List<Chess.move> poss)
        {
            //computer's turn

            if (medMode == true || hardMode == true)
            {
                hardLogic(poss);
            }

            int r = rnd.Next(0, poss.Count);//choose random move
            Chess.coordinate newSpot = new Chess.coordinate(poss[r].moveSpot.x, poss[r].moveSpot.y);
            Chess.coordinate oldSpot = new Chess.coordinate(poss[r].pieceSpot.x, poss[r].pieceSpot.y);

            if (board[oldSpot.x, oldSpot.y].job == "King")
            {
                castling(newSpot, oldSpot);//check if move is a castling
            }

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
            }
        }

        public void movePiece(Chess.coordinate newCell, Chess.piece pPiece, Chess.coordinate oldCell)
        {
            //overwrite current cell
            board[newCell.x, newCell.y].color = offensiveTeam;
            board[newCell.x, newCell.y].job = pPiece.job.ToString();

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

        public Image matchPicture(piece figure)
        {
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

        public PictureBox coordinateToPictureBox(Chess.coordinate spot)
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

        public void clearBackgroundImages()
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

        public void setFigureTheme()
        {
            lKing = Resources.figLking;
            lQueen = Resources.figLqueen;
            lBishop = Resources.figLbishop;
            lKnight = Resources.figLknight;
            lRook = Resources.figLrook;
            lPawn = Resources.figLpawn;
            dKing = Resources.figDking;
            dQueen = Resources.figDqueen;
            dBishop = Resources.figDbishop;
            dKnight = Resources.figDknight;
            dRook = Resources.figDrook;
            dPawn = Resources.figDpawn;
        }

        public void setLetterTheme()
        {
            lKing = Resources.letLking;
            lQueen = Resources.letLqueen;
            lBishop = Resources.letLbishop;
            lKnight = Resources.letLknight;
            lRook = Resources.letLrook;
            lPawn = Resources.letLpawn;
            dKing = Resources.letDking;
            dQueen = Resources.letDqueen;
            dBishop = Resources.letDbishop;
            dKnight = Resources.letDknight;
            dRook = Resources.letDrook;
            dPawn = Resources.letDpawn;
        }

        public void setFFTheme()
        {
            lKing = Resources.ffLking;
            lQueen = Resources.ffLqueen;
            lBishop = Resources.ffLbishop;
            lKnight = Resources.ffLknight;
            lRook = Resources.ffLrook;
            lPawn = Resources.ffLpawn;
            dKing = Resources.ffDking;
            dQueen = Resources.ffDqueen;
            dBishop = Resources.ffDbishop;
            dKnight = Resources.ffDknight;
            dRook = Resources.ffDrook;
            dPawn = Resources.ffDpawn;
        }

        public void setSOMTheme()
        {
            lKing = Resources.somLking;
            lQueen = Resources.somLqueen;
            lBishop = Resources.somLbishop;
            lKnight = Resources.somLknight;
            lRook = Resources.somLrook;
            lPawn = Resources.somLpawn;
            dKing = Resources.somDking;
            dQueen = Resources.somDqueen;
            dBishop = Resources.somDbishop;
            dKnight = Resources.somDknight;
            dRook = Resources.somDrook;
            dPawn = Resources.somDpawn;
        }

        public void setMarioTheme()
        {
            lKing = Resources.marioLking;
            lQueen = Resources.marioLqueen;
            lBishop = Resources.marioLbishop;
            lKnight = Resources.marioLknight;
            lRook = Resources.marioLrook;
            lPawn = Resources.marioLpawn;
            dKing = Resources.marioDking;
            dQueen = Resources.marioDqueen;
            dBishop = Resources.marioDbishop;
            dKnight = Resources.marioDknight;
            dRook = Resources.marioDrook;
            dPawn = Resources.marioDpawn;
        }

        public void setMMTheme()
        {
            lKing = Resources.mmLking;
            lQueen = Resources.mmLqueen;
            lBishop = Resources.mmLbishop;
            lKnight = Resources.mmLknight;
            lRook = Resources.mmLrook;
            lPawn = Resources.mmLpawn;
            dKing = Resources.mmDking;
            dQueen = Resources.mmDqueen;
            dBishop = Resources.mmDbishop;
            dKnight = Resources.mmDknight;
            dRook = Resources.mmDrook;
            dPawn = Resources.mmDpawn;
        }

        public void setFF4Theme()
        {
            lKing = Resources.ff4Lking;
            lQueen = Resources.ff4Lqueen;
            lBishop = Resources.ff4Lbishop;
            lKnight = Resources.ff4Lknight;
            lRook = Resources.ff4Lrook;
            lPawn = Resources.ff4Lpawn;
            dKing = Resources.ff4Dking;
            dQueen = Resources.ff4Dqueen;
            dBishop = Resources.ff4Dbishop;
            dKnight = Resources.ff4Dknight;
            dRook = Resources.ff4Drook;
            dPawn = Resources.ff4Dpawn;
        }

        public void setLttPTheme()
        {
            lKing = Resources.lttpLking;
            lQueen = Resources.lttpLqueen;
            lBishop = Resources.lttpLbishop;
            lKnight = Resources.lttpLknight;
            lRook = Resources.lttpLrook;
            lPawn = Resources.lttpLpawn;
            dKing = Resources.lttpDking;
            dQueen = Resources.lttpDqueen;
            dBishop = Resources.lttpDbishop;
            dKnight = Resources.lttpDknight;
            dRook = Resources.lttpDrook;
            dPawn = Resources.lttpDpawn;
        }

        public void changeTheme()
        {
            List<Chess.coordinate> temp = new List<coordinate>();

            temp = getDarkPieces();
            temp.AddRange(getLightPieces());

            foreach(Chess.coordinate spot in temp)
            {
                coordinateToPictureBox(spot).Image = matchPicture(board[spot.x, spot.y]);
            }
        }

        public void newGame()
        {
            NewGame play = new NewGame(this);
            play.ShowDialog();
        }

        public struct piece
        {
            public string color { get; set; }
            public string job { get; set; }
            public bool firstMove { get; set; }
        }

        public struct coordinate
        {
            public int x;
            public int y;

            public coordinate(int p1, int p2)
            {
                this.x = p1;
                this.y = p2;
            }
        }

        public struct move
        {
            //represents a move that a piece can do, includes starting position and ending position

            public coordinate pieceSpot;    //starting position
            public coordinate moveSpot;     //ending position

            public move(coordinate p1, coordinate p2)
            {
                this.pieceSpot = p1;
                this.moveSpot = p2;
            }
        }

        //the next few functions define the rules for what piece can move where in any situation except check restrictions
        //takes coordinate and returns list of possible moves for that piece

        public List<move> rookMoves(coordinate current)
        {
            string oppositeColor;
            move availableMove;
            int availableX = current.x;              //put coordinate in temp variable to manipulate while preserving original
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

        public List<move> knightMoves(coordinate current)
        {
            move availableMove;
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

        public List<move> bishopMoves(coordinate current)
        {
            string oppositeColor;
            move availableMove;
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

        public List<move> kingMoves(coordinate current)
        {
            move availableMove;
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

        public List<move> pawnMoves(coordinate current)
        {
            string oppositeColor;
            move availableMove;
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
