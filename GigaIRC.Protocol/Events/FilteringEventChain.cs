using System.Collections.Generic;
using System.Linq;

namespace GigaIRC.Events
{
    public class FilteringEventChain<TContext, TArgs> : EventChain<TContext, TArgs>
        where TArgs : NamedEventArgs
    {
        private class EventFilter 
        {
            public readonly string[] Names;
            public readonly EventHandler Handler;

            public EventFilter(EventHandler handler, string[] names)
            {
                Names = names;
                Handler = handler;
            }
        }

        readonly List<EventFilter> delegateStack;

        public FilteringEventChain()
        {
            delegateStack = new List<EventFilter>();
        }

        public void Add(EventHandler toAdd, params string[] filters)
        {
            delegateStack.Insert(0, new EventFilter(toAdd, filters));
        }

        public override void Add(EventHandler toAdd)
        {
            delegateStack.Insert(0, new EventFilter(toAdd, null));
        }

        public override void Remove(EventHandler toRemove)
        {
            bool found;
            do
            {
                found = false;
                foreach (EventFilter f in delegateStack)
                {
                    if(f.Handler == toRemove)
                    {
                        found = true;
                        delegateStack.Remove(f);
                        break;
                    }
                }
            } while (found);
        }

        public override bool Invoke(TContext c, TArgs e)
        {
            foreach (var f in delegateStack)
            {
                var t = f.Handler;

                if (f.Names != null)
                {
                    if (f.Names.Where(e.Is).Any(name => t(c, e)))
                    {
                        return true;
                    }
                }
                else
                {
                    if (t(c, e))
                        return true;
                }
            }
            return false;
        }
    }
}