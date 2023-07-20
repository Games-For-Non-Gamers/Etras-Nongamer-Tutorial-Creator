using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Etra.StarterAssets.EtraCharacterMainController;

public class ExecutableNewLevelDataHolder : MonoBehaviour
{
     public List<string> tempSelectedAbilities;

    [Header("Current Values")]
    public GameplayType gameplayType = GameplayType.FirstPerson; //Set by toggle
    public Model characterModel; //Set by dropdown
    public List<string> abilitiesToActivate;
}
