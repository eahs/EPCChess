using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ADSBackend.Util
{
    public class Chess
    {
        private static readonly string BLACK = "b";
        private static readonly string WHITE = "w";

        private static readonly int EMPTY = -1;

        private static readonly string PAWN = "p";
        private static readonly string KNIGHT = "n";
        private static readonly string BISHOP = "b";
        private static readonly string ROOK = "r";
        private static readonly string QUEEN = "q";
        private static readonly string KING = "k";

        private static readonly string SYMBOLS = "pnbrqkPNBRQK";

        private static readonly string DEFAULT_POSITION = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";


        private static readonly Dictionary<string, int[]> PAWN_OFFSETS = new Dictionary<string, int[]>
        {
            {"b", new[] {16, 32, 17, 15}},
            {"w", new[] {-16, -32, -17, -15}}
        };

        private static readonly Dictionary<string, int[]> PIECE_OFFSETS = new Dictionary<string, int[]>
        {
            {"n", new[] {-18, -33, -31, -14, 18, 33, 31, 14}},
            {"b", new[] {-17, -15, 17, 15}},
            {"r", new[] {-16, 1, 16, -1}},
            {"q", new[] {-17, -16, -15, 1, 17, 16, 15, -1}},
            {"k", new[] {-17, -16, -15, 1, 17, 16, 15, -1}}
        };

        private static readonly int[] ATTACKS =
        {
            20, 0, 0, 0, 0, 0, 0, 24, 0, 0, 0, 0, 0, 0, 20, 0,
            0, 20, 0, 0, 0, 0, 0, 24, 0, 0, 0, 0, 0, 20, 0, 0,
            0, 0, 20, 0, 0, 0, 0, 24, 0, 0, 0, 0, 20, 0, 0, 0,
            0, 0, 0, 20, 0, 0, 0, 24, 0, 0, 0, 20, 0, 0, 0, 0,
            0, 0, 0, 0, 20, 0, 0, 24, 0, 0, 20, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 20, 2, 24, 2, 20, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 2, 53, 56, 53, 2, 0, 0, 0, 0, 0, 0,
            24, 24, 24, 24, 24, 24, 56, 0, 56, 24, 24, 24, 24, 24, 24, 0,
            0, 0, 0, 0, 0, 2, 53, 56, 53, 2, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 20, 2, 24, 2, 20, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 20, 0, 0, 24, 0, 0, 20, 0, 0, 0, 0, 0,
            0, 0, 0, 20, 0, 0, 0, 24, 0, 0, 0, 20, 0, 0, 0, 0,
            0, 0, 20, 0, 0, 0, 0, 24, 0, 0, 0, 0, 20, 0, 0, 0,
            0, 20, 0, 0, 0, 0, 0, 24, 0, 0, 0, 0, 0, 20, 0, 0,
            20, 0, 0, 0, 0, 0, 0, 24, 0, 0, 0, 0, 0, 0, 20
        };

        private static readonly int[] RAYS =
        {
            17, 0, 0, 0, 0, 0, 0, 16, 0, 0, 0, 0, 0, 0, 15, 0,
            0, 17, 0, 0, 0, 0, 0, 16, 0, 0, 0, 0, 0, 15, 0, 0,
            0, 0, 17, 0, 0, 0, 0, 16, 0, 0, 0, 0, 15, 0, 0, 0,
            0, 0, 0, 17, 0, 0, 0, 16, 0, 0, 0, 15, 0, 0, 0, 0,
            0, 0, 0, 0, 17, 0, 0, 16, 0, 0, 15, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 17, 0, 16, 0, 15, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 17, 16, 15, 0, 0, 0, 0, 0, 0, 0,
            1, 1, 1, 1, 1, 1, 1, 0, -1, -1, -1, -1, -1, -1, -1, 0,
            0, 0, 0, 0, 0, 0, -15, -16, -17, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, -15, 0, -16, 0, -17, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, -15, 0, 0, -16, 0, 0, -17, 0, 0, 0, 0, 0,
            0, 0, 0, -15, 0, 0, 0, -16, 0, 0, 0, -17, 0, 0, 0, 0,
            0, 0, -15, 0, 0, 0, 0, -16, 0, 0, 0, 0, -17, 0, 0, 0,
            0, -15, 0, 0, 0, 0, 0, -16, 0, 0, 0, 0, 0, -17, 0, 0,
            -15, 0, 0, 0, 0, 0, 0, -16, 0, 0, 0, 0, 0, 0, -17
        };

        private static readonly Dictionary<string, int> SHIFTS = new Dictionary<string, int>
        {
            {"p", 0},
            {"n", 1},
            {"b", 2},
            {"r", 3},
            {"q", 4},
            {"k", 5}
        };

        private static Dictionary<string, string> FLAGS = new Dictionary<string, string>
        {
            {"NORMAL", "n"},
            {"CAPTURE", "c"},
            {"BIG_PAWN", "b"},
            {"EP_CAPTURE", "e"},
            {"PROMOTION", "p"},
            {"KSIDE_CASTLE", "k"},
            {"QSIDE_CASTLE", "q"}
        };


        private static readonly int RANK_1 = 7;
        private static readonly int RANK_2 = 6;
        private static int RANK_3 = 5;
        private static int RANK_4 = 4;
        private static int RANK_5 = 3;
        private static int RANK_6 = 2;
        private static readonly int RANK_7 = 1;
        private static readonly int RANK_8 = 0;


        private static readonly Dictionary<string, int> SQAURES = new Dictionary<string, int>
        {
            {"a8", 0}, {"b8", 1},
            {"c8", 2}, {"d8", 3}, {"e8", 4}, {"f8", 5}, {"g8", 6}, {"h8", 7},
            {"a7", 16}, {"b7", 17}, {"c7", 18}, {"d7", 19}, {"e7", 20}, {"f7", 21}, {"g7", 22}, {"h7", 23},
            {"a6", 32}, {"b6", 33}, {"c6", 34}, {"d6", 35}, {"e6", 36}, {"f6", 37}, {"g6", 38}, {"h6", 39},
            {"a5", 48}, {"b5", 49}, {"c5", 50}, {"d5", 51}, {"e5", 52}, {"f5", 53}, {"g5", 54}, {"h5", 55},
            {"a4", 64}, {"b4", 65}, {"c4", 66}, {"d4", 67}, {"e4", 68}, {"f4", 69}, {"g4", 70}, {"h4", 71},
            {"a3", 80}, {"b3", 81}, {"c3", 82}, {"d3", 83}, {"e3", 84}, {"f3", 85}, {"g3", 86}, {"h3", 87},
            {"a2", 96}, {"b2", 97}, {"c2", 98}, {"d2", 99}, {"e2", 100}, {"f2", 101}, {"g2", 102}, {"h2", 103},
            {"a1", 112}, {"b1", 113}, {"c1", 114}, {"d1", 115}, {"e1", 116}, {"f1", 117}, {"g1", 118}, {"h1", 119}
        };


        private Piece[] board = new Piece[128];

        private Dictionary<string, int> castling;

        private int ep_square = EMPTY;
        private int half_moves;

        private Dictionary<string, string> header = new Dictionary<string, string>();

        private Stack<BoardHistoryMove> history;

        private Dictionary<string, int> kings;
        private int move_number = 1;

        private string[] POSSIBLE_RESULTS = { "1-0", "0-1", "1/2-1/2", "*" };

        private readonly Dictionary<string, Dictionary<string, int>[]> ROOKS =
            new Dictionary<string, Dictionary<string, int>[]>
            {
                {
                    "w", new[]
                    {
                        new Dictionary<string, int> {{"square", SQAURES["a1"]}, {"flag", BITS.QSIDE_CASTLE}},
                        new Dictionary<string, int> {{"square", SQAURES["h1"]}, {"flag", BITS.KSIDE_CASTLE}}
                    }
                },
                {
                    "b", new[]
                    {
                        new Dictionary<string, int> {{"square", SQAURES["a8"]}, {"flag", BITS.QSIDE_CASTLE}},
                        new Dictionary<string, int> {{"square", SQAURES["h8"]}, {"flag", BITS.KSIDE_CASTLE}}
                    }
                }
            };


        private string turn = WHITE;

        // ================ Constructor ====================
        public Chess()
        {
            load(DEFAULT_POSITION);
        }

        public Chess(string fen)
        {
            if (!load(fen)) throw new ArgumentException("Invalid fen format", "fen");
        }

        /// <summary>
        /// Loads a game from SAN notation
        /// ex. d4 d5 c4 c6 Nc3 e6 e4 Nd7 exd5 cxd5 cxd5 exd5 Nxd5 Nb6 Bb5+ Bd7 Qe2+
        /// </summary>
        /// <param name="san"></param>
        public void loadSAN(string notation)
        {
            List<string> moves = notation.Split(' ').ToList();

            reset();
            for (int i = 0; i < moves.Count; i++)
            {
                this.Move(moves[i]);
            }
        }

        private void clear(bool keep_headers = false)
        {
            board = new Piece[128];

            kings = new Dictionary<string, int> { { "w", EMPTY }, { "b", EMPTY } };
            turn = WHITE;
            castling = new Dictionary<string, int> { { "w", 0 }, { "b", 0 } };
            castling["w"] = 0;
            castling["b"] = 0;
            ep_square = EMPTY;
            half_moves = 0;
            move_number = 1;
            history = new Stack<BoardHistoryMove>();
            if (!keep_headers) header = new Dictionary<string, string>();
            update_setup(generate_fen());
        }


        private void reset()
        {
            load(DEFAULT_POSITION);
        }

        private bool load(string fen, bool keep_headers = false)
        {
            var tokens = Regex.Split(fen, @"\s+");
            var position = tokens[0];
            var square = 0;

            if (!validate_fen(fen).valid) return false;

            clear(keep_headers);

            for (var i = 0; i < position.Length; i++)
            {
                var piece = position[i];
                if (piece == '/')
                {
                    square += 8;
                }
                else if (is_digit(piece))
                {
                    square += int.Parse(piece.ToString());
                }
                else
                {
                    var color = char.IsUpper(piece) ? WHITE : BLACK;
                    put(new Piece { type = piece.ToString().ToLower(), color = color }, algebraic(square));
                    square++;
                }
            }

            turn = tokens[1];

            if (tokens[2].IndexOf("K") > -1) castling["w"] |= BITS.KSIDE_CASTLE;

            if (tokens[2].IndexOf("Q") > -1) castling["w"] |= BITS.QSIDE_CASTLE;

            if (tokens[2].IndexOf("k") > -1) castling["b"] |= BITS.KSIDE_CASTLE;

            if (tokens[2].IndexOf("q") > -1) castling["b"] |= BITS.QSIDE_CASTLE;

            ep_square = tokens[3] == "-" ? EMPTY : SQAURES[tokens[3]];
            half_moves = int.Parse(tokens[4]);
            move_number = int.Parse(tokens[5]);

            update_setup(generate_fen());

            return true;
        }


        /* This function needs improvement while it validates structure it needs work o content
        for example does not verify each side has a king */
        private FenValidator validate_fen(string fen)
        {
            var errors = new string[12];
            errors[0] = "No errors";
            errors[1] = "FEN string must contain six space-delimited fields.";
            errors[2] = "6th field (move number) must be a positive integer.";
            errors[3] = "5th field (half move counter) must be a non-negative integer.";
            errors[4] = "4th field (en-passant square) is invalid.";
            errors[5] = "3rd field (castling availability) is invalid.";
            errors[6] = "2nd field (side to move) is invalid.";
            errors[7] = "1st field (piece positions) does not contain 8 \'/\'-delimited rows.:";
            errors[8] = "1st field (piece positions) is invalid [consecutive numbers].";
            errors[9] = "1st field (piece positions) is invalid [invalid piece].'";
            errors[10] = "1st field (piece positions) is invalid [row too large].'";
            errors[11] = "Illegal en-passant square";

            var tokens = Regex.Split(fen, @"\s+");
            /* 1st criterion: 6 space-seperated fields? */
            if (tokens.Length != 6) return new FenValidator { valid = false, error = errors[1] };

            /* 2nd criterion: move number field is a integer value > 0? */
            int mn;
            if (int.TryParse(tokens[5], out mn))
            {
                if (mn <= 0) return new FenValidator { valid = false, error = errors[2] };
            }
            else
            {
                return new FenValidator { valid = false, error = errors[2] };
            }

            /* 3rd criterion: half move counter is an integer >= 0? */
            int hm;
            if (int.TryParse(tokens[4], out hm))
            {
                if (hm < 0) return new FenValidator { valid = false, error = errors[3] };
            }
            else
            {
                return new FenValidator { valid = false, error = errors[3] };
            }

            /* 4th criterion: 4th field is a valid e.p.-string? */
            var rx = new Regex(@"^(-|[abcdefgh][36])$");
            if (!rx.IsMatch(tokens[3])) return new FenValidator { valid = false, error = errors[4] };

            /* 5th criterion: 3th field is a valid castle-string? */
            rx = new Regex(@"^(KQ?k?q?|Qk?q?|kq?|q|-)$");
            if (!rx.IsMatch(tokens[2])) return new FenValidator { valid = false, error = errors[5] };

            /* 6th criterion: 2nd field is "w" (white) or "b" (black)? */
            rx = new Regex(@"^(w|b)$");
            if (!rx.IsMatch(tokens[1])) return new FenValidator { valid = false, error = errors[6] };

            /* 7th criterion: 1st field contains 8 rows? */
            var rows = Regex.Split(tokens[0], @"/");
            if (rows.Length != 8) return new FenValidator { valid = false, error = errors[7] };

            /* 8th criterion: every row is valid? */
            for (var i = 0; i < rows.Length; i++)
            {
                var sum_fields = 0;
                var previous_was_number = false;
                for (var k = 0; k < rows[i].Length; k++)
                    if (!isNAN(rows[i][k].ToString()))
                    {
                        if (previous_was_number) return new FenValidator { valid = false, error = errors[8] };

                        sum_fields += int.Parse(rows[i][k].ToString());
                        previous_was_number = true;
                    }
                    else
                    {
                        rx = new Regex(@"^[prnbqkPRNBQK]$");
                        if (!rx.IsMatch(rows[i][k].ToString()))
                            return new FenValidator { valid = false, error = errors[9] };

                        sum_fields += 1;
                        previous_was_number = false;
                    }

                if (sum_fields != 8) return new FenValidator { valid = false, error = errors[10] };
            }

            if (tokens[3].Length > 1)
                if (tokens[3][1].ToString() == "3" && tokens[1] == "w" ||
                    tokens[3][1].ToString() == "6" && tokens[1] == "b")
                    return new FenValidator { valid = false, error = errors[11] };

            return new FenValidator { valid = true, error = errors[0] };
        }

        private string generate_fen()
        {
            var empty = 0;
            var fen = "";
            for (var i = SQAURES["a8"]; i <= SQAURES["h1"]; i++)
            {
                if (board[i] == null)
                {
                    empty++;
                }
                else
                {
                    if (empty > 0)
                    {
                        fen += empty.ToString();
                        empty = 0;
                    }

                    var color = board[i].color;
                    var piece = board[i].type;

                    fen += color == WHITE ? piece.ToUpper() : piece.ToLower();
                }

                if (((i + 1) & 0x88) != 0)
                {
                    if (empty > 0) fen += empty.ToString();

                    if (i != SQAURES["h1"]) fen += "/";

                    empty = 0;
                    i += 8;
                }
            }

            var cflags = "";
            if ((castling[WHITE] & BITS.KSIDE_CASTLE) != 0) cflags += "K";

            ;
            if ((castling[WHITE] & BITS.QSIDE_CASTLE) != 0) cflags += "Q";

            ;
            if ((castling[BLACK] & BITS.KSIDE_CASTLE) != 0) cflags += "k";

            ;
            if ((castling[BLACK] & BITS.QSIDE_CASTLE) != 0) cflags += "q";

            ;

            /* do we have an empty castling flag? */
            if (cflags == "") cflags = "-";
            var epflags = ep_square == EMPTY ? "-" : algebraic(ep_square);

            return fen + " " + turn + " " + cflags + " " + epflags + " " + half_moves + " " +
                   move_number;
        }


        /* called when the initial board setup is changed with put() or remove().
         * modifies the SetUp and FEN properties of the header object.  if the FEN is
         * equal to the default position, the SetUp and FEN are deleted
         * the setup is only updated if history.length is zero, ie moves haven't been
         * made.
         */
        private void update_setup(string fen)
        {
            if (history.Count > 0) return;

            if (fen != DEFAULT_POSITION)
            {
                header["SetUp"] = "1";
                header["FEN"] = fen;
            }
            else
            {
                header.Remove("SetUp");
                header.Remove("FEN");
            }
        }

        private Piece get(string square)
        {
            if (!SQAURES.ContainsKey(square)) return null;

            var piece = board[SQAURES[square]];
            return piece;
        }


        private bool put(Piece piece, string square)
        {
            /* check for valid piece object */
            if (string.IsNullOrEmpty(piece.type) || string.IsNullOrEmpty(piece.color)) return false;

            /* check for piece */
            if (SYMBOLS.IndexOf(piece.type.ToLower()) == -1) return false;

            /* check for valid square */
            if (!SQAURES.ContainsKey(square)) return false;

            var sq = SQAURES[square];

            /* don't let the user place more than one king */
            if (piece.type == KING
                && !(kings[piece.color] == EMPTY || kings[piece.color] == sq))
                return false;

            board[sq] = piece;
            if (piece.type == KING) kings[piece.color] = sq;


            update_setup(generate_fen());

            return true;
        }

        private Piece remove(string square)
        {
            if (!SQAURES.ContainsKey(square)) return null;

            var piece = get(square);
            board[SQAURES[square]] = null;
            if (piece != null && piece.type == KING) kings[piece.color] = EMPTY;

            update_setup(generate_fen());
            return piece;
        }


        private Move build_move(Piece[] board, int from, int to, int flags, string promotion = null)
        {
            var move = new Move
            {
                color = turn,
                from = from,
                to = to,
                flags = flags,
                piece = board[from].type
            };

            if (promotion != null)
            {
                move.flags |= BITS.PROMOTION;
                move.promotion = promotion;
            }

            if (board[to] != null)
                move.captured = board[to].type;
            else if ((flags & BITS.EP_CAPTURE) != 0) move.captured = PAWN;

            return move;
        }


        private void add_move(Piece[] board, Stack<Move> moves, int from, int to, int flags)
        {
            if (board[from].type == PAWN && (rank(to) == RANK_8 || rank(to) == RANK_1))
            {
                string[] pieces = { QUEEN, ROOK, BISHOP, KNIGHT };
                for (var i = 0; i < pieces.Length; i++) moves.Push(build_move(board, from, to, flags, pieces[i]));
            }
            else
            {
                moves.Push(build_move(board, from, to, flags));
            }
        }

        private Stack<Move> generate_moves(Dictionary<string, object> options = null)
        {
            var moves = new Stack<Move>();
            var us = turn;
            var them = swap_color(us);
            var second_rank = new Dictionary<string, int> { { "b", RANK_7 }, { "w", RANK_2 } };
            var first_sq = SQAURES["a8"];
            var last_sq = SQAURES["h1"];
            var single_sqaure = false;

            var legal = options != null && options.ContainsKey("legal") ? (bool)options["legal"] : true;

            if (options != null && options.ContainsKey("square"))
            {
                var sq = (string)options["square"];
                if (SQAURES.ContainsKey(sq))
                {
                    first_sq = last_sq = SQAURES[sq];
                    single_sqaure = true;
                }
                else
                {
                    /* invalid square */
                    return null;
                }
            }

            for (var i = first_sq; i <= last_sq; i++)
            {
                if ((i & 0x88) != 0)
                {
                    i += 7;
                    continue;
                }

                var piece = board[i];
                if (piece == null) continue;
                if (piece.color != us) continue;

                if (piece.type == PAWN)
                {
                    /* single square, non-capturing */
                    var square = i + PAWN_OFFSETS[us][0];
                    if (board[square] == null)
                    {
                        add_move(board, moves, i, square, BITS.NORMAL);
                        /* double square */
                        square = i + PAWN_OFFSETS[us][1];
                        if (second_rank[us] == rank(i) && board[square] == null)
                            add_move(board, moves, i, square, BITS.BIG_PAWN);
                    }

                    /* pawn captures */
                    for (var j = 2; j < 4; j++)
                    {
                        square = i + PAWN_OFFSETS[us][j];
                        if ((square & 0x88) != 0) continue;
                        if (board[square] != null && board[square].color == them)
                            add_move(board, moves, i, square, BITS.CAPTURE);
                        else if (square == ep_square) add_move(board, moves, i, ep_square, BITS.EP_CAPTURE);
                    }
                }
                else
                {
                    for (var j = 0; j < PIECE_OFFSETS[piece.type].Length; j++)
                    {
                        var offset = PIECE_OFFSETS[piece.type][j];
                        var square = i;
                        while (true)
                        {
                            square += offset;
                            if ((square & 0x88) != 0) break;
                            if (board[square] == null)
                            {
                                add_move(board, moves, i, square, BITS.NORMAL);
                            }
                            else
                            {
                                if (board[square].color == us) break;
                                add_move(board, moves, i, square, BITS.CAPTURE);
                                break;
                            }

                            /* break, if knight or king */
                            if (piece.type == "n" || piece.type == "k") break;
                        }
                    }
                }
            }

            /* check for castling if: a) we're generating all moves, or b) we're doing
        * single square move generation on the king's square
        */

            if (!single_sqaure || last_sq == kings[us])
            {
                /* king-side castling */
                if ((castling[us] & BITS.KSIDE_CASTLE) != 0)
                {
                    var castling_from = kings[us];
                    var castling_to = castling_from + 2;
                    if (board[castling_from + 1] == null && board[castling_to] == null &&
                        !attacked(them, kings[us]) &&
                        !attacked(them, castling_from + 1) &&
                        !attacked(them, castling_to))
                        add_move(board, moves, kings[us], castling_to, BITS.KSIDE_CASTLE);
                }

                /* queen-side castling */
                if ((castling[us] & BITS.QSIDE_CASTLE) != 0)
                {
                    var castling_from = kings[us];
                    var castling_to = castling_from - 2;
                    if (board[castling_from - 1] == null &&
                        board[castling_from - 2] == null &&
                        board[castling_from - 3] == null &&
                        !attacked(them, kings[us]) &&
                        !attacked(them, castling_from - 1) &&
                        !attacked(them, castling_to))
                        add_move(board, moves, kings[us], castling_to, BITS.QSIDE_CASTLE);
                }
            }

            /* return all pseudo-legal moves (this includes moves that allow the king
               * to be captured)
               */
            if (!legal) return moves;

            /* filter out illegal moves */
            var legal_moves = new Stack<Move>();
            for (var i = 0; i < moves.Count; i++)
            {
                make_move(moves.ElementAt(i));
                if (!king_attacked(us)) legal_moves.Push(moves.ElementAt(i));

                undo_move();
            }

            return legal_moves;
        }

        private string move_to_san(Move move, bool sloppy = false)
        {
            var output = "";
            if ((move.flags & BITS.KSIDE_CASTLE) != 0)
            {
                output = "O-O";
            }
            else if ((move.flags & BITS.QSIDE_CASTLE) != 0)
            {
                output = "O-O-O";
            }
            else
            {
                var disambiguator = get_disambiguator(move, sloppy);
                if (move.piece != PAWN) output += move.piece.ToUpper() + disambiguator;

                if ((move.flags & (BITS.CAPTURE | BITS.EP_CAPTURE)) != 0)
                {
                    if (move.piece == PAWN) output += algebraic(move.from).Substring(0, 1);

                    output += "x";
                }

                output += algebraic(move.to);

                if ((move.flags & BITS.PROMOTION) != 0) output += "=" + move.promotion.ToUpper();
            }

            make_move(move);
            if (in_check())
            {
                if (in_checkmate())
                    output += "#";
                else
                    output += "+";
            }

            undo_move();

            return output;
        }


        // parses all of the decorators out of a SAN string
        private string stripped_san(string move)
        {
            var rt = Regex.Replace(move, @"=", "");
            rt = Regex.Replace(rt, @"[+#]?[?!]*$", "");
            return rt;
        }

        private bool attacked(string color, int square)
        {
            for (var i = SQAURES["a8"]; i <= SQAURES["h1"]; i++)
            {
                /* did we run off the end of the board */
                if ((i & 0x88) != 0)
                {
                    i += 7;
                    continue;
                }

                /* if empty square or wrong color */
                if (board[i] == null || board[i].color != color) continue;

                var piece = board[i];
                var difference = i - square;
                var index = difference + 119;

                if ((ATTACKS[index] & (1 << SHIFTS[piece.type])) != 0)
                {
                    if (piece.type == PAWN)
                    {
                        if (difference > 0)
                        {
                            if (piece.color == WHITE) return true;
                        }
                        else
                        {
                            if (piece.color == BLACK) return true;
                        }

                        continue;
                    }

                    /* if the piece is a knight or a king */
                    if (piece.type == "n" || piece.type == "k") return true;

                    var offset = RAYS[index];
                    var j = i + offset;

                    var blocked = false;
                    while (j != square)
                    {
                        if (board[j] != null)
                        {
                            blocked = true;
                            break;
                        }

                        j += offset;
                    }

                    if (!blocked) return true;
                }
            }

            return false;
        }


        private bool king_attacked(string color)
        {
            return attacked(swap_color(color), kings[color]);
        }

        private bool in_check()
        {
            return king_attacked(turn);
        }

        private bool in_checkmate()
        {
            return in_check() && generate_moves().Count == 0;
        }

        private bool in_stalemate()
        {
            return !in_check() && generate_moves().Count == 0;
        }

        private bool insufficient_material()
        {
            var pieces = new Dictionary<string, int>();
            var bishops = new Stack<int>();
            var num_pieces = 0;
            var sq_color = 0;

            for (var i = SQAURES["a8"]; i <= SQAURES["h1"]; i++)
            {
                sq_color = (sq_color + 1) % 2;
                if ((i & 0x88) != 0)
                {
                    i += 7;
                    continue;
                }

                var piece = board[i];
                if (piece != null)
                {
                    pieces[piece.type] = pieces.ContainsKey(piece.type) ? pieces[piece.type] += 1 : 1;
                    if (piece.type == BISHOP) bishops.Push(sq_color);

                    num_pieces++;
                }
            }

            /* k vs. k */
            if (num_pieces == 2)
                return true;
            /* k vs. kn .... or .... k vs. kb */

            if (num_pieces == 3)
            {
                if (pieces.ContainsKey(BISHOP))
                    if (pieces[BISHOP] == 1)
                        return true;

                if (pieces.ContainsKey(KNIGHT))
                    if (pieces[KNIGHT] == 1)
                        return true;
            }
            /* kb vs. kb where any number of bishops are all on the same color */
            else if (pieces.ContainsKey(BISHOP))
            {
                if (num_pieces == pieces[BISHOP] + 2)
                {
                    var sum = 0;
                    var len = bishops.Count;
                    for (var i = 0; i < len; i++) sum += bishops.ElementAt(i);

                    if (sum == 0 || sum == len) return true;
                }
            }

            return false;
        }

        private bool in_threefold_repetition()
        {
            var repetition = false;
            var moves = new Stack<Move>();
            var positions = new Dictionary<string, int>();

            while (true)
            {
                var move = undo_move();
                if (move == null) break;
                moves.Push(move);
            }

            while (true)
            {
                /* remove the last two fields in the FEN string, they're not needed
                 * when checking for draw by rep */
                var fen = generate_fen();
                var fenArray = fen.Split(' ');
                fen = fenArray[0] + " " + fenArray[1] + " " + fenArray[2] + " " + fenArray[3];

                /* has the position occurred three or move times */
                positions[fen] = positions.ContainsKey(fen) ? positions[fen] + 1 : 1;
                if (positions[fen] >= 3) repetition = true;

                if (moves.Count < 1) break;

                make_move(moves.Pop());
            }

            return repetition;
        }

        private void push(Move move)
        {
            var historyMove = new BoardHistoryMove
            {
                move = move,
                kings = new Dictionary<string, int> { { "b", kings["b"] }, { "w", kings["w"] } },
                turn = turn,
                castling = new Dictionary<string, int> { { "b", castling["b"] }, { "w", castling["w"] } },
                ep_square = ep_square,
                half_moves = half_moves,
                move_number = move_number
            };
            history.Push(historyMove);
        }


        private void make_move(Move move)
        {
            var us = turn;
            var them = swap_color(us);
            push(move);

            board[move.to] = board[move.from];
            board[move.from] = null;

            /* if ep capture, remove the captured pawn */
            if ((move.flags & BITS.EP_CAPTURE) != 0)
            {
                if (turn == BLACK)
                    board[move.to - 16] = null;
                else
                    board[move.to + 16] = null;
            }

            /* if pawn promotion, replace with new piece */
            if ((move.flags & BITS.PROMOTION) != 0) board[move.to] = new Piece { type = move.promotion, color = us };

            /* if we moved the king */
            if (board[move.to].type == KING)
            {
                kings[board[move.to].color] = move.to;
                /* if we castled, move the rook next to the king */
                if ((move.flags & BITS.KSIDE_CASTLE) != 0)
                {
                    var castling_to = move.to - 1;
                    var castling_from = move.to + 1;
                    board[castling_to] = board[castling_from];
                    board[castling_from] = null;
                }
                else if ((move.flags & BITS.QSIDE_CASTLE) != 0)
                {
                    var castling_to = move.to + 1;
                    var castling_from = move.to - 2;
                    board[castling_to] = board[castling_from];
                    board[castling_from] = null;
                }

                /* turn off castling */
                castling[us] = 0;
            }

            /* turn off castling if we move a rook */
            if (castling[us] != 0)
                for (var i = 0; i < ROOKS[us].Length; i++)
                    if (move.from == ROOKS[us][i]["square"] && (castling[us] & ROOKS[us][i]["flag"]) != 0)
                    {
                        castling[us] ^= ROOKS[us][i]["flag"];
                        break;
                    }

            /* turn off castling if we capture a rook */
            if (castling[them] != 0)
                for (var i = 0; i < ROOKS[them].Length; i++)
                    if (move.to == ROOKS[them][i]["square"] && (castling[them] & ROOKS[them][i]["flag"]) != 0)
                    {
                        castling[them] ^= ROOKS[them][i]["flag"];
                        break;
                    }

            /* if big pawn move, update the en passant square */
            if ((move.flags & BITS.BIG_PAWN) != 0)
            {
                if (turn == "b")
                    ep_square = move.to - 16;
                else
                    ep_square = move.to + 16;
            }
            else
            {
                ep_square = EMPTY;
            }

            /* reset the 50 move counter if a pawn is moved or a piece is captured */
            if (move.piece == PAWN)
                half_moves = 0;
            else if ((move.flags & (BITS.CAPTURE | BITS.EP_CAPTURE)) != 0)
                half_moves = 0;
            else
                half_moves++;

            if (turn == BLACK) move_number++;

            turn = swap_color(turn);
        }


        private Move undo_move()
        {
            if (history.Count == 0) return null;

            var old = history.Pop();
            if (old == null) return null;

            var move = old.move;
            kings = old.kings;
            turn = old.turn;
            castling = old.castling;
            ep_square = old.ep_square;
            half_moves = old.half_moves;
            move_number = old.move_number;

            var us = turn;
            var them = swap_color(turn);

            board[move.from] = board[move.to];
            board[move.from].type = move.piece; // to undo any promotions
            board[move.to] = null;

            if ((move.flags & BITS.CAPTURE) != 0)
            {
                board[move.to] = new Piece { type = move.captured, color = them };
            }
            else if ((move.flags & BITS.EP_CAPTURE) != 0)
            {
                int index;
                if (us == BLACK)
                    index = move.to - 16;
                else
                    index = move.to + 16;

                board[index] = new Piece { type = PAWN, color = them };
            }

            if ((move.flags & (BITS.KSIDE_CASTLE | BITS.QSIDE_CASTLE)) != 0)
            {
                var castling_to = 0;
                var castling_from = 0;
                if ((move.flags & BITS.KSIDE_CASTLE) != 0)
                {
                    castling_to = move.to + 1;
                    castling_from = move.to - 1;
                }
                else if ((move.flags & BITS.QSIDE_CASTLE) != 0)
                {
                    castling_to = move.to - 2;
                    castling_from = move.to + 1;
                }

                board[castling_to] = board[castling_from];
                board[castling_from] = null;
            }

            return move;
        }

        /* this function is used to uniquely identify ambiguous moves */
        private string get_disambiguator(Move move, bool sloppy)
        {
            var moves = generate_moves(new Dictionary<string, object> { { "legal", !sloppy } });
            var from = move.from;
            var to = move.to;
            var piece = move.piece;

            var ambiguities = 0;
            var same_rank = 0;
            var same_file = 0;

            for (var i = 0; i < moves.Count; i++)
            {
                var ambig_from = moves.ElementAt(i).from;
                var ambig_to = moves.ElementAt(i).to;
                var ambig_piece = moves.ElementAt(i).piece;

                /* if a move of the same piece type ends on the same to square, we'll
          * need to add a disambiguator to the algebraic notation
          */
                if (piece == ambig_piece && from != ambig_from && to == ambig_to)
                {
                    ambiguities++;
                    if (rank(from) == rank(ambig_from)) same_rank++;

                    if (file(from) == file(ambig_from)) same_file++;
                }
            }

            if (ambiguities > 0)
            {
                /* if there exists a similar moving piece on the same rank and file as
            * the move in question, use the square as the disambiguator
            */
                if (same_rank > 0 && same_file > 0)
                    return algebraic(from);
                /* if the moving piece rests on the same file, use the rank symbol as the
         * disambiguator
         */
                if (same_file > 0)
                    return algebraic(from).Substring(1, 1);
                /* else use the file symbol */
                return algebraic(@from).Substring(0, 1);
            }

            return "";
        }

        private string ascii()
        {
            var s = "   +------------------------+\n";
            for (var i = SQAURES["a8"]; i <= SQAURES["h1"]; i++)
            {
                /* display the rank */
                if (file(i) == 0) s += " " + "87654321".Substring(rank(i), 1) + " |";

                /* empty piece */
                if (board[i] == null)
                {
                    s += " . ";
                }
                else
                {
                    var piece = board[i].type;
                    var color = board[i].color;
                    var symbol = color == WHITE ? piece.ToUpper() : piece.ToLower();
                    s += " " + symbol + " ";
                }

                if (((i + 1) & 0x88) != 0)
                {
                    s += "|\n";
                    i += 8;
                }
            }

            s += "   +------------------------+\n";
            s += "     a  b  c  d  e  f  g  h\n";
            return s;
        }

        // convert a move from Standard Algebraic Notation (SAN) to 0x88 coordinates
        private Move move_from_san(string move, bool sloppy)
        {
            // strip off any move decorations: e.g Nf3+?!
            var clean_move = stripped_san(move);
            // if we're using the sloppy parser run a regex to grab piece, to, and from
            // this should parse invalid SAN like: Pe2-e4, Rc1c4, Qf3xf7
            string[] matches = { };
            string piece = null;
            string from = null;
            string to = null;
            string promotion = null;
            if (sloppy)
            {
                var rx = new Regex(@"([pnbrqkPNBRQK])?([a-h][1-8])x?-?([a-h][1-8])([qrbnQRBN])?");
                matches = rx.Split(clean_move);
                if (matches.Length > 1)
                {
                    if (matches.Length == 4)
                    {
                        from = matches[1];
                        to = matches[2];
                    }

                    if (matches.Length == 5)
                    {
                        if (matches[1].Length == 1)
                        {
                            piece = matches[1];
                            from = matches[2];
                            to = matches[3];
                        }
                        else
                        {
                            from = matches[1];
                            to = matches[2];
                            promotion = matches[3];
                        }
                    }

                    if (matches.Length == 6)
                    {
                        piece = matches[1];
                        from = matches[2];
                        to = matches[3];
                        promotion = matches[4];
                    }
                }
            }

            var moves = generate_moves(); // edgeway- get all legal moves for current player

            for (var i = 0; i < moves.Count; i++)
                // try the strict parser first, then the sloppy parser if requested
                // by the user
                if (clean_move == stripped_san(move_to_san(moves.ElementAt(i))) ||
                    sloppy && clean_move == stripped_san(move_to_san(moves.ElementAt(i), true)))
                {
                    return moves.ElementAt(i);
                }
                else
                {
                    if (matches.Length > 1 &&
                        (string.IsNullOrEmpty(piece) || piece.ToLower() == moves.ElementAt(i).piece) &&
                        SQAURES[from] == moves.ElementAt(i).from &&
                        SQAURES[to] == moves.ElementAt(i).to &&
                        (string.IsNullOrEmpty(promotion) || promotion.ToLower() == moves.ElementAt(i).promotion))
                        return moves.ElementAt(i);
                }

            return null;
        }


        // UTILITY FUNCTIONS
        // ****************************************************************************
        private int rank(int i)
        {
            return i >> 4;
        }

        private int file(int i)
        {
            return i & 15;
        }

        private string algebraic(int i)
        {
            var f = file(i);
            var r = rank(i);
            return "abcdefgh".Substring(f, 1) + "87654321".Substring(r, 1);
        }

        private string swap_color(string c)
        {
            return c == WHITE ? BLACK : WHITE;
        }

        private bool is_digit(char c)
        {
            return "0123456789".IndexOf(c) != -1;
        }


        private string trim(string str)
        {
            // todo-e
            return "";
        }

        private static bool isNAN(string i)
        {
            int n;
            return !int.TryParse(i, out n);
        }


        // ===========================================================================
        // ================================ public API ===============================
        // ===========================================================================

        public string Ascii()
        {
            return ascii();
        }

        public void Clear()
        {
            clear();
        }

        public void Reset()
        {
            reset();
        }

        public string Fen()
        {
            return generate_fen();
        }


        public bool InCheck()
        {
            return king_attacked(turn);
        }

        public bool InCheckmate()
        {
            return in_checkmate();
        }

        public bool InStalemate()
        {
            return in_stalemate();
        }

        public bool InsufficientMaterial()
        {
            return insufficient_material();
        }

        public bool InThreefoldRepetition()
        {
            return in_threefold_repetition();
        }

        public bool FiftyMoveRule()
        {
            return half_moves >= 100;
        }

        public bool InDraw()
        {
            return FiftyMoveRule() || in_stalemate() || insufficient_material() || in_threefold_repetition();
        }

        public bool GameOver()
        {
            return FiftyMoveRule() || in_checkmate() || in_stalemate() || insufficient_material() ||
                   in_threefold_repetition();
        }

        public bool LoadFen(string fen)
        {
            return load(fen);
        }

        public bool Move(string move, bool sloppy = false)
        {
            var move_obj = move_from_san(move, sloppy);
            if (move_obj == null) return false;

            make_move(move_obj);

            return true;
        }

        public string[] MoveHistory()
        {
            var reversed_history = new Stack<Move>();
            var move_history = new Stack<string>();
            while (history.Count > 0) reversed_history.Push(undo_move());

            while (reversed_history.Count > 0)
            {
                var move = reversed_history.Pop();
                move_history.Push(move_to_san(move));
                make_move(move);
            }

            var h = new string[move_history.Count];
            for (var i = h.Length - 1; i > -1; i--) h[i] = move_history.Pop();

            return h;
        }

        public UndoMoveArgs Undo()
        {
            var move = undo_move();
            if (move == null) return null;

            var undoMoveArgs = new UndoMoveArgs();
            undoMoveArgs.color = move.color;
            undoMoveArgs.from = algebraic(move.from);
            undoMoveArgs.to = algebraic(move.to);
            undoMoveArgs.piece = move.piece;
            return undoMoveArgs;
        }

        public bool Put(Piece piece, string square)
        {
            return put(piece, square);
        }

        public Piece GetPiece(string square)
        {
            return get(square);
        }

        public Piece Remove(string square)
        {
            return remove(square);
        }

        public string Turn()
        {
            return turn;
        }

        public string SquareColor(string square)
        {
            if (SQAURES.ContainsKey(square))
            {
                var sq = SQAURES[square];
                return (rank(sq) + file(sq)) % 2 == 0 ? "light" : "dark";
            }

            return null;
        }


        // todo-e maybe return some sort of verbose object for legal moves i.e from e2 to e4 like chess.js
        public string[] LegalMovesAll()
        {
            Stack<Move> moves = null;
            moves = generate_moves();
            var legalMoves = new Stack<string>();
            for (var i = 0; i < moves.Count; i++) legalMoves.Push(move_to_san(moves.ElementAt(i)));

            return legalMoves.ToArray();
        }

        public string[] LegalMovesSquare(string square)
        {
            Stack<Move> moves = null;
            moves = generate_moves(new Dictionary<string, object> { { "square", square } });
            var legalMoves = new Stack<string>();
            if (moves != null)
                for (var i = 0; i < moves.Count; i++)
                    legalMoves.Push(move_to_san(moves.ElementAt(i)));

            return legalMoves.ToArray();
        }


        public ValidateFenArgs ValidateFen(string fen)
        {
            var validateFenArgs = new ValidateFenArgs();
            var fenValidator = validate_fen(fen);
            if (fenValidator.valid)
            {
                validateFenArgs.success = true;
                validateFenArgs.validatedFen = fen;
            }
            else
            {
                validateFenArgs.success = false;
                validateFenArgs.errorMessage = fenValidator.error;
            }

            return validateFenArgs;
        }
    }


    // =============================== CLASSES =================================

    public class Piece
    {
        public string type { get; set; }
        public string color { get; set; }
    }


    public class Move
    {
        public string color { get; set; }
        public int from { get; set; }
        public int to { get; set; }
        public int flags { get; set; }

        public string piece { get; set; }
        public string promotion { get; set; }
        public string captured { get; set; }
    }

    public class BoardHistoryMove
    {
        public Move move { get; set; }
        public Dictionary<string, int> kings { get; set; }
        public string turn { get; set; }
        public Dictionary<string, int> castling { get; set; }
        public int ep_square { get; set; }
        public int half_moves { get; set; }
        public int move_number { get; set; }
    }


    public class FenValidator
    {
        public bool valid { get; set; }
        public string error { get; set; }
    }

    internal static class BITS
    {
        public static int NORMAL => 1;

        public static int CAPTURE => 2;

        public static int BIG_PAWN => 4;

        public static int EP_CAPTURE => 8;

        public static int PROMOTION => 16;

        public static int KSIDE_CASTLE => 32;

        public static int QSIDE_CASTLE => 64;
    }


    public class ValidateFenArgs
    {
        public string errorMessage = "";
        public bool success;
        public string validatedFen = "";
    }

    public class UndoMoveArgs
    {
        public string color = "";
        public string from = "";
        public string piece = "";
        public string to = "";
    }

}
