using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalLevelEnabler : MonoBehaviour
{
    [SerializeField] private List<PortalBehaviour> portals;
    [SerializeField] private bool developerMode;

    void Awake()
    {
        if (!developerMode) handlePortalGates();
        else enableAllGates();
    }

    void handlePortalGates()
    {
        for (int i = 0; i < LevelHandler.getLevelToPlay() && i < portals.Count; i++)
        {
            if (i < LevelHandler.getLevelToPlay() - 1) portals[i].enableWithGate();
            else portals[i].enable();
        }
    }

    void enableAllGates() {
        for (int i = 0; i < portals.Count; i++) {
            portals[i].enable();
        }
    }
}
