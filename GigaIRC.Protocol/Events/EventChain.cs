using System;
using System.Collections.Generic;
using System.Linq;

namespace GigaIRC.Events
{
    public class EventChain<TContext, TArgs>
        where TArgs : EventArgs
    {
        public delegate bool EventHandler(TContext c, TArgs e);

        readonly List<EventHandler> _delegateStack = new List<EventHandler>();

        public virtual void Add(EventHandler toAdd)
        {
            _delegateStack.Insert(0,toAdd);
        }

        public virtual void Remove(EventHandler toRemove)
        {
            _delegateStack.Remove(toRemove);
        }

        public virtual bool Invoke(TContext c, TArgs e)
        {
            return _delegateStack.Any(t => t(c, e));
        }

        public static EventChain<TContext, TArgs> operator +(EventChain<TContext, TArgs> e, EventHandler toAdd)
        {
            e.Add(toAdd);
            return e;
        }

        public static EventChain<TContext, TArgs> operator -(EventChain<TContext, TArgs> e, EventHandler toRemove)
        {
            e.Remove(toRemove);
            return e;
        }
    }
}
