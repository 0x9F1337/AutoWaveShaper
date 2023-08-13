using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AutoWaveShaper
{
    internal class Native
    {
        [DllImport( "gdi32.dll" )]
        public static extern uint GetPixel( IntPtr hdc, int nXPos, int nYPos );

        [DllImport( "user32.dll" )]
        public static extern bool GetWindowRect( IntPtr hWnd, out RECT lpRect );

        [StructLayout( LayoutKind.Sequential )]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
    }
}
