﻿using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MathAttack.Class;
using Windows.UI;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MathAttack

{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Game level names
        private CanvasBitmap BG, StartScreen, ScoreScreen, Level1, Blast;

        // Boundaries of the application view
        public static Rect boundaries = ApplicationView.GetForCurrentView().VisibleBounds;

        // Width and Height of canvas and scale width and height
        public static float DesignWidth = 1920;
        public static float DesignHeight = 1080;
        public static float scaleWidth, scaleHeight;

        // Round Timer
        private DispatcherTimer RoundTimer = new DispatcherTimer();

        // List of projectiles positions
        public List<float> blastX = new List<float>();
        public List<float> blastY = new List<float>();

        // Level of the game
        private int GameState = 0;

        // Timer starting value
        private int countdown = 3;

        // Controls when a round is over
        private bool RoundEnded = false;

        public MainPage()
        {
            this.InitializeComponent();
            // Fires when the window has changed its rendering size
            Window.Current.SizeChanged += Current_SizeChanged;
            // Set the scale on page load
            Scaling.SetScale();

            RoundTimer.Tick += RoundTimer_Tick;
            RoundTimer.Interval = new TimeSpan(0, 0, 1);

        }

        private void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            // Everytime the window size changes reset the scale
            Scaling.SetScale();
        }

        // Adapted from https://microsoft.github.io/Win2D/html/T_Microsoft_Graphics_Canvas_UI_Xaml_CanvasControl.htm
        private void GameCanvas_CreateResources(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            // Calls CreateResourcesAsync Task and ensures could will not execute until the task completes
            args.TrackAsyncAction(CreateResourcesAsync(sender).AsAsyncAction());
        }

        async Task CreateResourcesAsync(CanvasControl sender)
        {
            // Loads the demo start screen
            StartScreen = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Images/how-to-play.png"));

            // Loads Level 1 screen
            Level1 = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Images/AVP.jpg"));

            // Loads the score screen
            ScoreScreen = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Images/ALwallpaper.png"));

            // Loads a blast projectile
            Blast = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Images/blast.png"));

        }

        private void GameCanvas_Draw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
        {
            // Load initial Game State
            GSM();
            // Draw the start screen
            args.DrawingSession.DrawImage(Scaling.ScaleImage(BG));
            args.DrawingSession.DrawText(countdown.ToString(), 100, 100, Colors.Yellow);

            //Display projectiles
            for (int i = 0; i < blastX.Count; i++)
            {
                args.DrawingSession.DrawImage(Scaling.ScaleImage(Blast), blastX[i] - (30 * scaleWidth), blastY[i] - (30 * scaleHeight)); // Decrease by 30 to compensate for the offset of the mouse
            }

            // Redraw everything in the draw method (roughly 60fps)
            GameCanvas.Invalidate();
        }

        // Handles touch screen taps
        private void GameCanvas_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // Display the score screen if the round ends
            if (RoundEnded == true)
            {

                GameState = 0;
                // Reset the round
                RoundEnded = false;
                countdown = 6;
            }
            else
            {
                // If the screen is tapped/clicked go up one level
                if (GameState == 0)
                {
                    GameState += 1;
                    RoundTimer.Start();

                   

                } else if (GameState > 0)
                {
                    // Add the xy coordinates of a blast projectile from user mouse position
                    blastX.Add((float)e.GetPosition(GameCanvas).X);
                    blastY.Add((float)e.GetPosition(GameCanvas).Y);
                }
            }
           
        }

        // The Game State Manager
        public void GSM()
        {
            // Shows the score screen if the round ends
            if (RoundEnded == true)
            {
                BG = ScoreScreen;
            }
            else
            {
                // Loads the Start Screen
                if (GameState == 0)
                {
                    BG = StartScreen;
                }

                // Loads Level 1
                else if (GameState == 1)
                {
                    BG = Level1;
                }
            }
            
        }

        // RoundTimer_Tick controls the decrementing round time
        private void RoundTimer_Tick(object sender, object e)
        {
            // Decrement the timer
            countdown -= 1;

            // Stops the timer once it reaches 0 and ends the round
            if (countdown < 1)
            {
                RoundTimer.Stop();
                RoundEnded = true;
            }
        }
    }
}
