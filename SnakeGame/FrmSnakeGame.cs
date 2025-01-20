using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace SnakeGame
{
    public partial class FrmSnakeGame : Form
    {
        private const int CELL_SIZE_PIXELS = 20;
        private const int ROWS_NUMBER = 15;
        private const int COLS_NUMBER = 15;
        private const int FIELD_LEFT_OFFSET_PIXELS = 40;
        private const int FIELD_TOP_OFFSET_PIXELS = 40;
        private const int INITIAL_SNAKE_SPEED_INTERVAL = 300;
        private const int SPEED_INCREMENT_BY = 1;
        private const int FIELD_BOTTOM_OFFSET_PIXELS = 150;

        private enum SnakeDirection
        {
            Left,
            Right,
            Up,
            Down
        }

        private SnakeDirection snakeDirection = SnakeDirection.Up;
        private LinkedList<Point> snake = new LinkedList<Point>();
        private Point food;
        private Random rand = new Random();
        private bool isGameEnded;
        private int foodAlpha = 255;
        private int points = 0;
        private int foodEaten = 0;

        private void DrawStatsAndKeyboardHints(Graphics g)
        {
            Font fontStats = new Font("Consolas", 9);
            int statsLeftOffset = FIELD_LEFT_OFFSET_PIXELS + CELL_SIZE_PIXELS * COLS_NUMBER + 10;
            g.DrawString(string.Format("Длина змейки: {0}", snake.Count), fontStats, Brushes.Lime, new Point(statsLeftOffset, 10));
            g.DrawString(string.Format("Скорость: {0}", INITIAL_SNAKE_SPEED_INTERVAL - TimerGameLoop.Interval + 5), fontStats, Brushes.Lime, new Point(statsLeftOffset, 30));
            g.DrawString(string.Format("Очки: {0}", points), fontStats, Brushes.Goldenrod, new Point(statsLeftOffset, 50));
            g.DrawString(string.Format("Еды съедено: {0}", foodEaten), fontStats, Brushes.Crimson, new Point(statsLeftOffset, 70));

            g.DrawString("Управление:", fontStats, Brushes.White, new Point(statsLeftOffset, 160));
            g.DrawString("Вверх: ↑", fontStats, Brushes.White, new Point(statsLeftOffset, 190));
            g.DrawString("Вниз:  ↓", fontStats, Brushes.White, new Point(statsLeftOffset, 210));
            g.DrawString("Влево: ←", fontStats, Brushes.White, new Point(statsLeftOffset, 230));
            g.DrawString("Влево: →", fontStats, Brushes.White, new Point(statsLeftOffset, 250));
            fontStats.Dispose();
        }
        public FrmSnakeGame()
        {
            InitializeComponent();
            TimerGameLoop = new Timer();
            TimerGameLoop.Interval = INITIAL_SNAKE_SPEED_INTERVAL;
            TimerGameLoop.Tick += TimerGameLoop_Tick;
            this.Width = FIELD_LEFT_OFFSET_PIXELS + CELL_SIZE_PIXELS * COLS_NUMBER + 120;
            this.Height = FIELD_TOP_OFFSET_PIXELS + CELL_SIZE_PIXELS * ROWS_NUMBER + FIELD_BOTTOM_OFFSET_PIXELS + 39;

        }
        private void InitializeSnake()
        {
            snakeDirection = SnakeDirection.Up;
            snake.Clear();
            snake.AddFirst(new Point(ROWS_NUMBER - 1, COLS_NUMBER / 2 - 1));
        }

        private void GenerateFood()
        {
            
            if (snake.Count == ROWS_NUMBER * COLS_NUMBER)
            {
                GameWon(); 
                return;
            }

            bool isFoodClashWithSnake;
            do
            {
                food = new Point(rand.Next(0, ROWS_NUMBER), rand.Next(0, COLS_NUMBER));
                isFoodClashWithSnake = false;
                foreach (Point p in snake)
                {
                    if (p.X == food.X && p.Y == food.Y)
                    {
                        isFoodClashWithSnake = true;
                        break;
                    }
                }
            } while (isFoodClashWithSnake);

            TimerGameLoop.Interval -= SPEED_INCREMENT_BY;
        }



        private void StartGame()
        {
            AdjustFormSize();
            GenerateFood();
            InitializeSnake();
            isGameEnded = false;
            points = 0;
            foodEaten = 0;
            TimerGameLoop.Start();
            TimerGameLoop.Interval = INITIAL_SNAKE_SPEED_INTERVAL;
        }
        private void AdjustFormSize()
        {
            this.Width = FIELD_LEFT_OFFSET_PIXELS + CELL_SIZE_PIXELS * COLS_NUMBER + 200;
            this.Height = FIELD_TOP_OFFSET_PIXELS + CELL_SIZE_PIXELS * ROWS_NUMBER + 60;
        }
        private void FrmSnakeGame_Load_1(object sender, EventArgs e)
        {
            DoubleBuffered = true;
            BackColor = Color.Black;
            StartGame();
        }

        private void DrawGrid(Graphics g)
        {
            for (int row = 0; row <= ROWS_NUMBER; row++)
            {
                g.DrawLine(Pens.Cyan,
                    new Point(FIELD_LEFT_OFFSET_PIXELS, FIELD_TOP_OFFSET_PIXELS + row * CELL_SIZE_PIXELS),
                    new Point(FIELD_LEFT_OFFSET_PIXELS + CELL_SIZE_PIXELS * ROWS_NUMBER, FIELD_TOP_OFFSET_PIXELS + row * CELL_SIZE_PIXELS)
                );

                for (int col = 0; col <= COLS_NUMBER; col++)
                {
                    g.DrawLine(Pens.Cyan,
                        new Point(FIELD_LEFT_OFFSET_PIXELS + col * CELL_SIZE_PIXELS, FIELD_TOP_OFFSET_PIXELS),
                        new Point(FIELD_LEFT_OFFSET_PIXELS + col * CELL_SIZE_PIXELS, FIELD_TOP_OFFSET_PIXELS + CELL_SIZE_PIXELS * COLS_NUMBER)
                    );
                }
            }
        }
        private void DrawSnake(Graphics g)
        {
            for (int i = 0; i < snake.Count; i++)
            {
                Brush brush;

                if (i == 0)
                {
                   
                    brush = Brushes.Bisque;

                   
                    g.FillRectangle(brush, new Rectangle(
                        FIELD_LEFT_OFFSET_PIXELS + snake.ElementAt(i).Y * CELL_SIZE_PIXELS + 1,
                        FIELD_TOP_OFFSET_PIXELS + snake.ElementAt(i).X * CELL_SIZE_PIXELS + 1,
                        CELL_SIZE_PIXELS - 1,
                        CELL_SIZE_PIXELS - 1));

                    
                    int eyeSize = 4; 
                    int eyeOffset = 5; 

                    if (snakeDirection == SnakeDirection.Up || snakeDirection == SnakeDirection.Down)
                    {
                       
                        g.FillEllipse(Brushes.Black, FIELD_LEFT_OFFSET_PIXELS + snake.ElementAt(i).Y * CELL_SIZE_PIXELS + eyeOffset,
                                                      FIELD_TOP_OFFSET_PIXELS + snake.ElementAt(i).X * CELL_SIZE_PIXELS + 5,
                                                      eyeSize, eyeSize);
                        g.FillEllipse(Brushes.Black, FIELD_LEFT_OFFSET_PIXELS + snake.ElementAt(i).Y * CELL_SIZE_PIXELS + CELL_SIZE_PIXELS - eyeOffset - eyeSize,
                                                      FIELD_TOP_OFFSET_PIXELS + snake.ElementAt(i).X * CELL_SIZE_PIXELS + 5,
                                                      eyeSize, eyeSize);
                    }
                    else
                    {
                      
                        g.FillEllipse(Brushes.Black, FIELD_LEFT_OFFSET_PIXELS + snake.ElementAt(i).Y * CELL_SIZE_PIXELS + 5,
                                                      FIELD_TOP_OFFSET_PIXELS + snake.ElementAt(i).X * CELL_SIZE_PIXELS + eyeOffset,
                                                      eyeSize, eyeSize);
                        g.FillEllipse(Brushes.Black, FIELD_LEFT_OFFSET_PIXELS + snake.ElementAt(i).Y * CELL_SIZE_PIXELS + 5,
                                                      FIELD_TOP_OFFSET_PIXELS + snake.ElementAt(i).X * CELL_SIZE_PIXELS + CELL_SIZE_PIXELS - eyeOffset - eyeSize,
                                                      eyeSize, eyeSize);
                    }
                }
                else if (i == snake.Count - 1)
                {
                   
                    brush = Brushes.Purple;
                    g.FillRectangle(brush, new Rectangle(
                        FIELD_LEFT_OFFSET_PIXELS + snake.ElementAt(i).Y * CELL_SIZE_PIXELS + 1,
                        FIELD_TOP_OFFSET_PIXELS + snake.ElementAt(i).X * CELL_SIZE_PIXELS + 1,
                        CELL_SIZE_PIXELS - 1,
                        CELL_SIZE_PIXELS - 1));
                }
                else
                {
                   
                    brush = Brushes.Lime;
                    g.FillRectangle(brush, new Rectangle(
                        FIELD_LEFT_OFFSET_PIXELS + snake.ElementAt(i).Y * CELL_SIZE_PIXELS + 1,
                        FIELD_TOP_OFFSET_PIXELS + snake.ElementAt(i).X * CELL_SIZE_PIXELS + 1,
                        CELL_SIZE_PIXELS - 1,
                        CELL_SIZE_PIXELS - 1));
                }
            }
        }
        private void AddPlayerPoints()
        {
            if (food.X == 0 && food.Y == 0 || food.X == ROWS_NUMBER - 1 && food.Y == 0 || food.X == ROWS_NUMBER - 1 && food.Y == COLS_NUMBER - 1 || food.X == 0 && food.Y == COLS_NUMBER - 1)
            {
                points += 1000;
            }
            else if (food.X == 0 || food.X == ROWS_NUMBER - 1 || food.Y == 0 || food.Y == COLS_NUMBER - 1)
            {
                points += 500;
            }
            else
            {
                points += 250;
            }
        }

        private void DrawFood(Graphics g)
        {
            Rectangle foodRectangle = new Rectangle(
                FIELD_LEFT_OFFSET_PIXELS + food.Y * CELL_SIZE_PIXELS + 1,
                FIELD_TOP_OFFSET_PIXELS + food.X * CELL_SIZE_PIXELS + 1,
                CELL_SIZE_PIXELS - 1,
                CELL_SIZE_PIXELS - 1);
            Brush brushFood = new LinearGradientBrush(foodRectangle,
                Color.FromArgb(foodAlpha, Color.Crimson.R, Color.DarkSeaGreen.G, Color.Crimson.B),
                Color.FromArgb(foodAlpha, Color.RosyBrown.R, Color.RosyBrown.G, Color.RosyBrown.B),
                100, true);
            g.FillEllipse(brushFood, foodRectangle);
            Brush brushFoodBorder = new SolidBrush(Color.FromArgb(foodAlpha, Color.Red.R, Color.Red.G, Color.Red.B));
            Pen penFoodBorder = new Pen(brushFoodBorder);
            g.DrawEllipse(penFoodBorder, foodRectangle);
            brushFood.Dispose();
            penFoodBorder.Dispose();
            brushFoodBorder.Dispose();
        }
        private void FrmSnakeGame_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
           
            DrawGrid(g);
            DrawFood(g);
            DrawSnake(g);
            DrawStatsAndKeyboardHints(g);

        }
        private void GameOver()
        {
            isGameEnded = true;
            TimerGameLoop.Stop();
            if (MessageBox.Show("Ты проиграл/а! Начать заново?", "Конец игры", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                StartGame();
            }
        }
        private void MoveSnake()
        {
            LinkedListNode<Point> head = snake.First;
            Point newHead = new Point(0, 0);
            switch (snakeDirection)
            {
                case SnakeDirection.Left:
                    newHead = new Point(head.Value.X, head.Value.Y - 1);
                    break;
                case SnakeDirection.Right:
                    newHead = new Point(head.Value.X, head.Value.Y + 1);
                    break;
                case SnakeDirection.Down:
                    newHead = new Point(head.Value.X + 1, head.Value.Y);
                    break;
                case SnakeDirection.Up:
                    newHead = new Point(head.Value.X - 1, head.Value.Y);
                    break;
            }

            foreach (Point p in snake)
            {
                if (p.X == newHead.X && p.Y == newHead.Y)
                {
                    // змейка съела сама себя! конец игры!
                    Invalidate();
                    GameOver();
                    return;
                }
            }

            snake.AddFirst(newHead);

            if (newHead.X == food.X && newHead.Y == food.Y)
            {
               
                AddPlayerPoints();

                
                foodEaten++;

               
                GenerateFood();
            }
            else
            {
                snake.RemoveLast();
            }
        }
        private bool IsGameOver()
        {
            if (snake.First == null)
            {
                
                return true;
            }

            LinkedListNode<Point> head = snake.First;
            switch (snakeDirection)
            {
                case SnakeDirection.Left:
                    return head.Value.Y - 1 < 0;
                case SnakeDirection.Right:
                    return head.Value.Y + 1 >= COLS_NUMBER;
                case SnakeDirection.Down:
                    return head.Value.X + 1 >= ROWS_NUMBER;
                case SnakeDirection.Up:
                    return head.Value.X - 1 < 0;
            }
            return false;

        }

        private void GameWon()
        {
            if (isGameEnded) return; 
            isGameEnded = true;

            TimerGameLoop.Stop(); 

            DialogResult result = MessageBox.Show(
                "Ты победил! Начать заново?",
                "Победа",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information
            );

            if (result == DialogResult.Yes)
            {
                StartGame(); 
            }
            else
            {
                Close(); 
            }
        }
        private bool IsGameWon()
        {
            
            return snake.Count >= ROWS_NUMBER * COLS_NUMBER;
        }

        private void TimerGameLoop_Tick(object sender, EventArgs e)
        {
            if (isGameEnded) return; 

            if (IsGameOver())
            {
                GameOver();
            }
            else if (IsGameWon())
            {
                GameWon();
            }
            else
            {
                MoveSnake(); 
                Invalidate(); 
            }
        }
        private void ChangeSnakeDirection(SnakeDirection restrictedDirection, SnakeDirection newDirection)
        {
            if (snakeDirection != restrictedDirection)
            {
                snakeDirection = newDirection;
            }
        }

        private void FrmSnakeGame_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                case Keys.A:
                    ChangeSnakeDirection(SnakeDirection.Right, SnakeDirection.Left);
                    break;
                case Keys.Right:
                case Keys.D:
                    ChangeSnakeDirection(SnakeDirection.Left, SnakeDirection.Right);
                    break;
                case Keys.Down:
                case Keys.S:
                    ChangeSnakeDirection(SnakeDirection.Up, SnakeDirection.Down);
                    break;
                case Keys.Up:
                case Keys.W:
                    ChangeSnakeDirection(SnakeDirection.Down, SnakeDirection.Up);
                    break;
                case Keys.Escape:
                    TimerGameLoop.Stop();
                    Close();
                    break;
                case Keys.Space:
                    if (isGameEnded && !TimerGameLoop.Enabled)
                    {
                        StartGame();
                    }
                    break;
            }
        }
    }
}