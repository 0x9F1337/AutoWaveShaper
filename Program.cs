using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace AutoWaveShaper
{
    internal class Program
    {
        static async Task Main( string[] args )
        {
            Console.WriteLine( "Automated pixel bot for WAVESHAPER" );

            var game = GetProcess( "WAVESHAPER" );

            if (game == null)
            {
                await Console.Out.WriteLineAsync( "[-] Make sure you start the game first." );
                await Task.Delay( 1_500 );
                return;
            }

            Console.WriteLine( "[+] Game detected (PID: {0} | HWND: {1:x2})", game.Id, game.MainWindowHandle );

            IntPtr hwnd = game.MainWindowHandle;

            Task logic = Logic( hwnd );

            await logic;

            // Task renderAffectedArea = RenderAffectedArea( hwnd );

            await Console.Out.WriteLineAsync( "We chilling" );

            await Task.Delay( Timeout.Infinite );
        }

        static async Task Logic( IntPtr hwnd )
        {
            var bounds = GetWindowBounds( hwnd );
            var area = GetAffectedAreaRelative( bounds );
            var font = new Font( FontFamily.GenericSansSerif, 40 );
            Pen pen = new Pen( Color.Red, 1 );

            int windowWidth = bounds.Right - bounds.Left;
            int windowHeight = bounds.Bottom - bounds.Top;
            int scanX = windowWidth / 2;
            int pixelPosY1 = ( windowHeight / 2 ) - 18; // Upper site: Detection for Q and W
            int pixelPosY2 = ( windowHeight / 2 ) - 15; // Lower site: Detection for A and S

            using (var g = Graphics.FromHwnd( hwnd ))
            {
                //while (true)
                //{
                //    g.DrawLine( pen, scanX, pixelPosY1, scanX + 300, pixelPosY1 );
                //    g.DrawLine( pen, scanX, pixelPosY2, scanX + 300, pixelPosY2 );
                //}

                IntPtr hdc = g.GetHdc();

                //var colup = GetColorFromPixel( Native.GetPixel( hdc, scanX, pixelPosY1 ) );
                //var coldown = GetColorFromPixel( Native.GetPixel( hdc, scanX, pixelPosY2 ) );

                //Console.WriteLine( "Upper: (R,G,B) ({0} {1} {2})", colup.R, colup.G, colup.B );
                //Console.WriteLine( "Lower: (R,G,B) ({0} {1} {2})", coldown.R, coldown.G, coldown.B );

                await Console.Out.WriteLineAsync( "[+] Autoplay bot running..." );

                while (true)
                {
                    for (int rangeScan = 500; rangeScan > 180; rangeScan--)
                    {
                        if (Native.GetForegroundWindow() != hwnd)
                        {
                            Console.WriteLine( "[!] Paused until WAVESHAPER window is focused again." );
                            continue;
                        }

                        var upperPixel = Native.GetPixel( hdc, rangeScan, pixelPosY1 );
                        var lowerPixel = Native.GetPixel( hdc, rangeScan, pixelPosY2 );

                        // Anything else than the 0,0,0 ( we have a hit )

                        if (upperPixel != 0) // Detect Q or W
                        {
                            var nextPixel = Native.GetPixel( hdc, rangeScan, pixelPosY1 - 5 );

                            if (nextPixel == upperPixel) // Pixel above is also not 0,0,0 - so we probably have W shape.
                            {
                                SendKeys.SendWait( "W" );

                                // Console.Write( "W" );
                                // Console.Write( " " );

                            }
                            else // Pixel above is black, so we have Q
                            {
                                SendKeys.SendWait( "Q" );
                                // Console.Write( "Q" );
                                // Console.Write( " " );
                            }
                        }
                        else if (lowerPixel != 0) // Detect A or S
                        {
                            var nextPixel = Native.GetPixel( hdc, rangeScan, pixelPosY2 + 5 );

                            if (nextPixel == lowerPixel) // Pixel above is also not 0,0,0 - so we probably have S shape.
                            {
                                SendKeys.SendWait( "S" );
                                // Console.Write( "S" );
                                // Console.Write( " " );
                            }
                            else // Pixel above is black, so we have A
                            {
                                SendKeys.SendWait( "A" );

                                // Console.Write( "A" );
                                // Console.Write( " " );
                            }
                        }
                    }

                    await Task.Delay( 1 );
                }

            }
        }

        private static Color GetColorFromPixel( uint pixel )
        {
            return Color.FromArgb( ( int )pixel );
        }


        static Bitmap TakeSS2( Native.RECT windowBounds )
        {
            int w = windowBounds.Right - windowBounds.Left;
            int h = windowBounds.Bottom - windowBounds.Top;

            Bitmap bmp = new Bitmap( w, h, PixelFormat.Format24bppRgb );

            using (var g = Graphics.FromImage( bmp ))
            {
                g.CopyFromScreen( windowBounds.Left, windowBounds.Top, 0, 0, new Size( w, h ), CopyPixelOperation.SourceCopy );

                // bmp.Save( "test.png", ImageFormat.Png );

                return bmp;
            }
        }

        static Bitmap TakeSS( Native.RECT windowBounds, Native.RECT affectedArea )
        {
            int w = affectedArea.Right - affectedArea.Left;
            int h = affectedArea.Bottom - affectedArea.Top;

            Native.RECT converted = new Native.RECT()
            {
                Left = windowBounds.Left + affectedArea.Left,
                Right = windowBounds.Right - affectedArea.Right,
                Top = windowBounds.Top + affectedArea.Top,
                Bottom = windowBounds.Bottom - affectedArea.Bottom
            };

            Bitmap bmp = new Bitmap( w, h, PixelFormat.Format24bppRgb );

            using (var g = Graphics.FromImage( bmp ))
            {
                g.CopyFromScreen( converted.Left, converted.Top, 0, 0, new Size( w, h ), CopyPixelOperation.SourceCopy );

                bmp.Save( "test.png", ImageFormat.Png );

                return bmp;
            }
        }

        private static Native.RECT GetAffectedAreaRelative( Native.RECT rect )
        {
            //int boxWidth = 100;
            //int boxHeight = 100;

            //// var rect = new Rectangle( startX, startY, boxWidth, boxHeight );

            //// rect.Size = new Size( boxWidth, boxHeight );

            //int w = rect.Right - rect.Left;
            //int h = rect.Bottom - rect.Top;

            //int startX = ( w / 2 ) - boxWidth;
            //int endX = ( w / 2 ) + boxWidth;
            //int startY = ( h / 2 ) - boxHeight;
            //int endY = ( h / 2 ) + boxHeight;

            //return new Native.RECT()
            //{
            //    Left = startX,
            //    Right = endX,
            //    Top = startY,
            //    Bottom = endY
            //};

            return rect;
        }

        private static Native.RECT GetWindowBounds( IntPtr hWnd )
        {
            Native.RECT rect;

            Native.GetWindowRect( hWnd, out rect );

            //int w = rect.Right - rect.Left;
            //int h = rect.Bottom - rect.Top;

            return rect;
        }

        static Process? GetProcess( string processName )
                => Process.GetProcessesByName( processName ).FirstOrDefault();
    }
}