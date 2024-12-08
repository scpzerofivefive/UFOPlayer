using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Scripts
{
    public class UFOScript
    {
        public string Name { get; set; }

        private LinkedListNode<ScriptAction> CurrentNodeLeft { get; set; }
        private LinkedListNode<ScriptAction> CurrentNodeRight { get; set; }

        private LinkedList<ScriptAction> _leftActions;
        private LinkedList<ScriptAction> _rightActions;

        public LinkedList<ScriptAction> LeftActions
        {
            get { return _leftActions; }
            set
            {
                _leftActions = new LinkedList<ScriptAction>(value.OrderBy(a => a.Timestamp).ToList());
            }

        }

        public LinkedList<ScriptAction> RightActions
        {
            get { return _rightActions; }
            set
            {
                _rightActions = new LinkedList<ScriptAction>(value.OrderBy(a => a.Timestamp).ToList());
            }
        }

        public UFOScript(List<ScriptAction> leftActions, List<ScriptAction> rightActions)
        {

            LeftActions = new LinkedList<ScriptAction>(leftActions);
            RightActions = new LinkedList<ScriptAction>(rightActions);

            LeftActions.AddFirst(new ScriptAction(new TimeSpan(0), 0));
            RightActions.AddFirst(new ScriptAction(new TimeSpan(0), 0));

            CurrentNodeLeft = LeftActions.First;
            CurrentNodeRight = RightActions.First;

            LinkedListNode<ScriptAction> _node = LeftActions.Last;
        }

        public UFOScript(string name, List<ScriptAction> leftActions, List<ScriptAction> rightActions) : this(leftActions, rightActions)
        {
            Name = name;
        }

        public UFOScript() : this(new List<ScriptAction> { }, new List<ScriptAction> { })
        {

        }
        public UFOScript(string name) : this()
        {
            Name = name;

        }

        public ScriptCommand goTo(TimeSpan timestamp)
        {
            //Debug.WriteLine(timestamp.TotalMilliseconds + " > " + CurrentNodeLeft.Value.Timestamp.TotalMilliseconds);
            if (timestamp < CurrentNodeLeft.Value.Timestamp)
            {
                CurrentNodeLeft = LeftActions.First;
            }
            else
            {

                while (CurrentNodeLeft.Next != null &&
                    timestamp.TotalMilliseconds > CurrentNodeLeft.Next.Value.Timestamp.TotalMilliseconds)
                {
                    CurrentNodeLeft = CurrentNodeLeft.Next;
                }
            }

            if (timestamp < CurrentNodeRight.Value.Timestamp)
            {
                CurrentNodeRight = RightActions.First;
            }
            else
            {
                while (CurrentNodeRight.Next != null &&
                    timestamp.TotalMilliseconds > CurrentNodeRight.Next.Value.Timestamp.TotalMilliseconds)
                {
                    CurrentNodeRight = CurrentNodeRight.Next;
                }

            }

            return new ScriptCommand(CurrentNodeLeft.Value.Intensity, CurrentNodeRight.Value.Intensity);



        }
    }
}
