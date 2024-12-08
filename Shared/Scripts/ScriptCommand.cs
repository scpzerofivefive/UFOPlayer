

namespace Shared.Scripts
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

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }
            if (obj is ScriptCommand cmd)
            {
                return Left == cmd.Left && Right == cmd.Right;
            }
            return false;
        }
    }
}
