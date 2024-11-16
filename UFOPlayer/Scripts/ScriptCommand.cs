using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFOPlayer.Scripts
{
    public class ScriptCommand
    {
        public sbyte Left { get; }
        public sbyte Right { get; }

        public ScriptCommand(sbyte left, sbyte right)
        {
            Left = left; Right = right;
        }

        public byte[] getBuffer()
        {
            byte[] arr = { 0x05, getSignedMagnitude(Left), getSignedMagnitude(Right) };
            return arr;
        }

        private byte getSignedMagnitude(sbyte input)
        {
            return input < 0 ? (byte)(~input + 129) : (byte)input;
        }

        public override bool Equals(Object obj)
        {
            if (obj is null)
            {
                return false;
            }
            if (obj is ScriptCommand cmd)
            {
                return this.Left == cmd.Left && this.Right == cmd.Right;
            }
            return false;
        }
    }
}
