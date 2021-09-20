using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murio
{
    public class DelayedCallbackWatcher : Watchers.Watcher
    {
        public int FrameDelay { get; set; } = 1;

        private Action _action;

        public DelayedCallbackWatcher(Action action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public DelayedCallbackWatcher(int frameDelay, Action action) : this(action)
        {
            FrameDelay = frameDelay;
        }

        public override void Update()
        {
            if (FrameDelay > 0)
            {
                FrameDelay--;
            }

            if (!bToBeDestroyed && FrameDelay == 0)
            {
                bToBeDestroyed = true;

                _action.Invoke();
            }
        }
    }
}
