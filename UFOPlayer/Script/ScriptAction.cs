using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFOPlayer.Script
{
    public class ScriptAction
    {
        public int Timestamp { get; set; }
        public sbyte Left { get; set; }
        public sbyte Right { get; set; }

        public ScriptAction(int timestamp, sbyte left, sbyte right)
        {
            Timestamp = timestamp; Left = left; Right = right;
        }

        public ScriptAction(sbyte left, sbyte right)
        {
            Left = left; Right = right;
        }

        public byte[] getBuffer()
        {
            byte[] arr = { 0x05, getSignedMagnitude(Left), getSignedMagnitude(Right)};
            return arr;
        }

        private byte getSignedMagnitude(sbyte input)
        {
            return input < 0 ? (byte)(~input + 1) : (byte)input;
        }
    }
}
