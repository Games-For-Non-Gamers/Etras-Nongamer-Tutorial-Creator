using UnityEngine;
using System;
using Codice.Client.BaseCommands;
using System.Linq;
using UnityEditor;
using System.Collections.Generic;
using Codice.Client.BaseCommands.BranchExplorer;
using static Codice.Client.BaseCommands.Import.Commit;
using static PlasticGui.WorkspaceWindow.CodeReview.Summary.CommentSummaryData;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering.VirtualTexturing;

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