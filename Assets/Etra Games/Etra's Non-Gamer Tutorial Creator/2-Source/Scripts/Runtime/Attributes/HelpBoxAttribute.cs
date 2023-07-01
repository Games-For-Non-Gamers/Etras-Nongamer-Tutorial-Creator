using UnityEngine;
using System;

namespace Etra.NonGamerTutorialCreator
{
    [AttributeUsage(AttributeTargets.Field)]
    public class HelpBoxAttribute : PropertyAttribute
    {
        public HelpBoxAttribute(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }
    }
}