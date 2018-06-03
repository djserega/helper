﻿using System;
using System.Windows.Input;

namespace Helper.UnmanagedCode
{
    internal class KeyUpArgs : EventArgs
    {
        public Key KeyUp { get; private set; }

        internal KeyUpArgs(Key key)
        {
            KeyUp = key;
        }
    }
}
