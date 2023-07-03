using UnityEngine;
using System;

namespace Etra.NonGamerTutorialCreator
{
    [AttributeUsage(AttributeTargets.Field)]
    public class AbilitySelectorAttribute : PropertyAttribute
    {
        public AbilitySelectorAttribute() { }

        public AbilitySelectorAttribute(bool showItems) : this()
        {
            ShowItems = showItems;
        }

        public bool ShowItems { get; private set; } = true;
    }

}