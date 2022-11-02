using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PingPong
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            BallMovment();
            BortMovment();
        }

        /*---------Скорость бортов---------*/

        int BortSpeed = 15;

        /*------------Левый борт-------------*/

        int LBMovmentY = 150;
        int LBMovment_Y = 150;

        /*----------Правый борт-------------*/

        int RBMovmentY = 150;
        int RBMovment_Y = 150;

        /*------------Шар-------------*/

        int BallSpeed = 10;

        int BallMovmentX = 383;
        int BallMovmentY = 194;
        int BallMovment_X = 384;
        int BallMovment_Y = 200;

        bool HitUp = false;
        bool HitRight = false;

        /*---------------------------*/



        Random r = new Random();

        int randY = 0;
        int NumSpeed = 10;

        bool isPaused = true;



        private void Window_KeyDown(object sender, KeyEventArgs e) //Считывание клавиш
        {


            if (Keyboard.IsKeyDown(Key.Escape))//Пауза 
            {
                if (!isPaused)
                {
                    isPaused = true;
                    PauseLB.Content = "Пауза";
                }
                else
                {
                    isPaused = false;
                    PauseLB.Content = "";
                }

            }

            if (Keyboard.IsKeyDown(Key.NumPad1))//Изменение скорости мяча
            {
                BallSpeed++;
                NumSpeed++;
            }
            else if (Keyboard.IsKeyDown(Key.NumPad2))
            {
                BallSpeed--;
                NumSpeed--;
            }


            if (Keyboard.IsKeyDown(Key.NumPad4))
            {
                BortSpeed++;
            }
            else if (Keyboard.IsKeyDown(Key.NumPad5))
            {
                BortSpeed--;
            }
        }


        async Task BortMovment()
        {
            while (true)
            {
                LeftBortMovment();
                RightBortMovment();
                await Task.Delay(1);
            }
        }

        async Task LeftBortMovment()     //Управление левым бортом
        {

            if (Keyboard.IsKeyDown(Key.W))
            {
                LBMovmentY += BortSpeed;
                LBMovment_Y -= BortSpeed;
            }
            else if (Keyboard.IsKeyDown(Key.S))
            {
                LBMovmentY -= BortSpeed;
                LBMovment_Y += BortSpeed;
            }

            if (LBMovmentY > 290) //Проверка столкновения с потолком
            {
                LBMovmentY = 290;
                LBMovment_Y = 10;
                
            }
            else if (LBMovmentY < 10) //Проверка столкновения с дном
            {
                LBMovmentY = 10;
                LBMovment_Y = 290;
            }
            LeftBort.Margin = new Thickness(10, LBMovment_Y, 755, LBMovmentY); //Изменение координат борта
        }


        async Task RightBortMovment()   //Управление правым бортом
        {

            if (Keyboard.IsKeyDown(Key.Up))
            {
                RBMovmentY += BortSpeed;
                RBMovment_Y -= BortSpeed;
            }
            else if (Keyboard.IsKeyDown(Key.Down))
            {
                RBMovmentY -= BortSpeed;
                RBMovment_Y += BortSpeed;
            }


            if (RBMovmentY > 290) //Проверка столкновения с потолком
            {
                RBMovmentY = 290;
                RBMovment_Y = 10;
            }
            else if (RBMovmentY < 10) //Проверка столкновения с дном
            {
                RBMovmentY = 10;
                RBMovment_Y = 290;
            }
            RightBort.Margin = new Thickness(755, RBMovment_Y, 10, RBMovmentY); //Изменение координат борта
        }


        async Task BallMovment()
        {

            while (true)
            {
                if (isPaused)
                {
                    await GamePause();
                }

                BortChecker();
                BallDirectionChange();

                if (BorderChecker())
                {
                    await ResetBall();
                }

                Ball.Margin = new Thickness(BallMovmentX, BallMovment_Y, BallMovment_X, BallMovmentY);
                await Task.Delay(1);

            }
        }


        async Task ResetBall()
        {
            BallMovmentX = 383;
            BallMovmentY = 194;
            BallMovment_X = 384;
            BallMovment_Y = 200;

            BallMovmentY += 0; BallMovmentX += 0;
            BallMovment_Y -= 0; BallMovment_X -= 0;

            BallSpeed = r.Next(NumSpeed, NumSpeed + 4);
            randY = r.Next(NumSpeed - 18, NumSpeed);


            Ball.Margin = new Thickness(BallMovmentX, BallMovment_Y, BallMovment_X, BallMovmentY);
            await Task.Delay(1000);
        }

        async Task GamePause()    //Пауза игры
        {
            int BallSpeedCopy = BallSpeed;
            int RandYCopy = randY;
            int BortSpeedCopy = BortSpeed;

            while (isPaused)
            {
                BallSpeed = 0;
                randY = 0;
                BortSpeed = 0;

                await Task.Delay(1);
            }

            BallSpeed = BallSpeedCopy;
            BortSpeed = BortSpeedCopy;
            randY = RandYCopy;
        }




        void BortChecker()
        {
            //Проверка на столкновение правого борта
            if (BallMovmentX > 740)
            {
                if (BallMovment_Y + 15 >= RBMovment_Y && BallMovmentY + 15 >= RBMovmentY)
                {

                    HitRight = true;
                    RandBallDirection();
                }
            }

            //Проверка на столкновение левого борта
            else if (BallMovmentX < 30)
            {
                if (BallMovment_Y + 15 >= LBMovment_Y && BallMovmentY + 15 >= LBMovmentY)
                {

                    HitRight = false;
                    RandBallDirection();
                }
            }
        }

        bool BorderChecker()      //Проверка границ мяча
        {
            //Проверка на выход из правой границы

            if (BallMovmentX > 760)
            {
                AddScore(LeftCount);
                RandBallDirection();
                ResetBall();

                HitRight = true;


                return true;
            }
            //Проверка на удар потолока
            else if (BallMovmentY > 390)
            {
                if(HitUp)
                {
                    HitUp = false;
                }
                else
                {
                    HitUp = true;
                }
                Console.WriteLine("X " + BallMovmentX + "\tY " + BallMovmentY);
                Thread.Sleep(1);
            }

            //Проверка на выход из левой границы
            else if (BallMovmentX < 10)
            {

                AddScore(RightCount);
                RandBallDirection();
                ResetBall();

                HitRight = false;


                return true;
            }
            //Проверка на удар дна
            else if (BallMovmentY < 2)
            {

                if (!HitUp)
                {
                    HitUp = true;
                }
                else
                {
                    HitUp = false;
                }
                Console.WriteLine("X " + BallMovmentX + "       Y " + BallMovmentY);
                Thread.Sleep(1);

            }

            return false;
        }

        void BallDirectionChange()//Cмена направления мяча 
        {
            if (HitUp && !HitRight)
            {
                BallMovmentY -= randY; BallMovmentX += BallSpeed;
                BallMovment_Y +=randY; BallMovment_X -= BallSpeed;

            }
            else if (HitUp && HitRight)
            {
                BallMovmentY -= randY; BallMovmentX -= BallSpeed;
                BallMovment_Y += randY; BallMovment_X += BallSpeed;
            }
            else if (!HitUp && HitRight)
            {
                BallMovmentY += randY; BallMovmentX -= BallSpeed;
                BallMovment_Y -= randY; BallMovment_X += BallSpeed;
            }
            else if (!HitUp && !HitRight)
            {
                BallMovmentY += randY; BallMovmentX += BallSpeed;
                BallMovment_Y -= randY; BallMovment_X -= BallSpeed;
            }
        }


        void RandBallDirection()//Генерация направления мяча по оси Y и скорости мяча
        {

            //BallSpeed = r.Next(10, 14);
            //randY = r.Next(-10, 11);

            BallSpeed = r.Next(NumSpeed, NumSpeed + 4);
            randY = r.Next(NumSpeed - 18, NumSpeed + 1);

            if (randY >= -2 && randY <= 2)
            {
                //BallSpeed = r.Next(14, 19);
                BallSpeed = r.Next(NumSpeed+5, NumSpeed + 10);
            }


        }


        void AddScore(Label LabelName)//Подсчёт очков
        {
            int scrore = Convert.ToInt32(LabelName.Content);
            ++scrore;
            LabelName.Content = scrore.ToString();
        }

    }
}
