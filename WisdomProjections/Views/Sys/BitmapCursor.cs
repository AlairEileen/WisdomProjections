using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WisdomProjections.Views.Sys
{
    internal class BitmapCursor : System.Runtime.InteropServices.SafeHandle

    {

        public override bool IsInvalid

        {

            get { return handle == (IntPtr)(-1); }

        }

        public static Cursor CreateBmpCursor(System.Drawing.Bitmap cursorBitmap)

        {

            var c = new BitmapCursor(cursorBitmap);

            return System.Windows.Interop.CursorInteropHelper.Create(c);

        }

        protected BitmapCursor(System.Drawing.Bitmap cursorBitmap)

        : base((IntPtr)(-1), true)

        {

            handle = cursorBitmap.GetHicon();

        }

        protected override bool ReleaseHandle()

        {

            bool result = DestroyIcon(handle);

            handle = (IntPtr)(-1);

            return result;

        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]

        public static extern bool DestroyIcon(IntPtr hIcon);

    }
}
