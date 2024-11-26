// -----------------------------------------------------------------------
// <copyright file="EventPatchAttribute.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Interfaces
{
    using System;
    using System.Reflection;

    using Events.EventArgs.Interfaces;


    /// <summary>
    /// An attribute to contain data about an event patch.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class EventListenerAttribute : Attribute
    {
        public Type eventType { get; set; }
        public string functionToSubscribeTo { get; set; }
       
        public EventListenerAttribute(Type currentEventToSubscribe, string nameToSubscribeTo)
        {
            eventType = currentEventToSubscribe;
            functionToSubscribeTo = nameToSubscribeTo;
            
        }
    }
}