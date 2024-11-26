namespace Exiled.Events.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using API.Features;
    using API.Interfaces;

    using EventArgs.Interfaces;

    using Features;

    public class ProcessDynamicEvents
    {
        ProcessDynamicEvents()
        {
            
        }
        public static void loadDelegateFunctions(IPlugin<IConfig> plugin)
        {
            Log.Info("Load delegeate func \n\n  ");
            Assembly currentAssembly = plugin.Assembly;
            HashSet<Type> delegateToAdd = currentAssembly.GetTypes().Where(type => type.CustomAttributes.Any(customAtt => customAtt.AttributeType == typeof(EventListenerAttribute))).ToHashSet();
            
            IEnumerable<MethodInfo> methodsWithAttribute = currentAssembly.GetTypes()
                .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
                .Where(m => m.GetCustomAttributes(typeof(EventListenerAttribute), false).Any());
            
            // foreach (Type type in Assembly.GetTypes())
            // {
            //     Log.Info($"What are the available types {type}");
            // }
            
            foreach (Type type in delegateToAdd)
            {
                Log.Info($"What are the available types {type}");
            }    

            // Print the methods
            foreach (MethodInfo methodToAdd in methodsWithAttribute)
            {
                EventListenerAttribute attribute = (EventListenerAttribute)methodToAdd.GetCustomAttribute(typeof(EventListenerAttribute));
                Type currentType = attribute.eventType;
                // MethodInfo currentMethod = currentType.GetMethod("set_" + attribute.functionToSubscribeTo);
                PropertyInfo eventField = currentType.GetProperty(attribute.functionToSubscribeTo,BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
                // eventField.SetMethod.Invoke()
                if (eventField != null && eventField.CanWrite)
                {
                    // Get the static value of the event field
                    var eventValue = eventField.GetValue(null); 
                    // Check if the event is a non-generic Event
                    if (eventValue.GetType() == typeof(Event))
                    {
                        Log.Info("Non generic");

                        // Handle non-generic Event
                        Event currentEvent = (Event)eventValue;
                        currentEvent.InnerEvent += (CustomEventHandler)Delegate.CreateDelegate(
                            typeof(CustomEventHandler), methodToAdd);
                    }
                    // Check if the event is a generic Event<T>
                    else if (eventValue.GetType().IsGenericType)
                    {
                        Log.Info("Generic event");
                        // Handle generic Event<T>
                        var genericEventType = eventValue.GetType();
                        var eventArgType = genericEventType.GetGenericArguments()[0]; // Type of T

                        Log.Info("\n\n 1 \n\n");
                        // Create the appropriate delegate type for the specific generic event
                        Type customEventHandlerType = typeof(CustomEventHandler<>).MakeGenericType(eventArgType);
                        Log.Info($"\n\n 2 {methodToAdd} - and {customEventHandlerType} - and {eventArgType} \n\n");
                        // Create delegate and subscribe to the event
                        Delegate eventDelegate = Delegate.CreateDelegate(customEventHandlerType, methodToAdd);
                        Log.Info("\n\n 3 \n\n");
                        // Get the specific InnerEvent for this generic event
                        var innerEventProperty = genericEventType.GetProperty("InnerEvent");
                        var innerEvent = innerEventProperty?.GetValue(eventValue);
                        Log.Info("\n\n 4 \n\n");
                        if (innerEvent != null)
                        {
                            Log.Info("\n\n 5 \n\n");
                            innerEvent.GetType().GetMethod("AddHandler")?.Invoke(innerEvent, new object[] { eventDelegate });
                        }
                    }
                    else
                    {
                        // Handle the case where the event type is unknown
                        Log.Error("Event type is neither 'Event' nor 'Event<T>'");
                    }
              
                    // Step 4: Add a delegate to the event
                  
                }
                
                // HashSet<MethodInfo> currentEvents = currentType.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic).ToHashSet();
                //
                // foreach (MethodInfo currentEvent in currentEvents)
                // {
                //     // Log.Info($"Current method {currentEvent.Name} \n");
                //     if (currentEvent.Name.Contains(attribute.functionToSubscribeTo))
                //     {
                //         Log.Info($"Found my func {attribute.functionToSubscribeTo} versus {currentEvent.Name}");
                //     }
                // }

           
                List<ParameterInfo> parameterInfos = methodToAdd.GetParameters().ToList();
                Log.Info($"\n\nClass: {methodToAdd.DeclaringType?.Name}, Method: {methodToAdd.Name}, Attribute {attribute}" +
                         $" {parameterInfos}\n\n");
            }
        }

    }
}