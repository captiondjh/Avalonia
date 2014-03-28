﻿// -----------------------------------------------------------------------
// <copyright file="Match.cs" company="Steven Kirk">
// Copyright 2014 MIT Licence. See licence.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Perspex.Styling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Text;
    using Perspex.Controls;

    public class Selector
    {
        private bool stopTraversal;

        public Selector()
        {
            this.GetObservable = _ => Observable.Return(true);
        }

        public Selector(Selector previous, bool stopTraversal = false)
            : this()
        {
            this.Previous = previous;
            this.InTemplate = previous != null ? previous.InTemplate : false;
            this.stopTraversal = stopTraversal;
        }

        public bool InTemplate
        {
            get;
            set;
        }

        public Func<IStyleable, IObservable<bool>> GetObservable
        {
            get;
            set;
        }

        public Selector Previous
        {
            get;
            private set;
        }

        public string SelectorString
        {
            get;
            set;
        }

        public Selector MovePrevious()
        {
            return this.stopTraversal ? null : this.Previous;
        }

        public StyleActivator GetActivator(IStyleable control)
        {
            List<IObservable<bool>> inputs = new List<IObservable<bool>>();
            Selector selector = this;
            
            while (selector != null)
            {
                if ((selector.InTemplate && control.TemplatedParent == null) ||
                    (!selector.InTemplate && control.TemplatedParent != null))
                {
                    inputs.Add(Observable.Return(false));
                }
                else
                {
                    inputs.Add(selector.GetObservable(control));
                }

                selector = selector.MovePrevious();
            }

            return new StyleActivator(inputs);
        }

        public override string ToString()
        {
            string result = string.Empty;

            if (this.Previous != null)
            {
                result = this.Previous.ToString();
            }

            return result + this.SelectorString;
        }
    }
}
