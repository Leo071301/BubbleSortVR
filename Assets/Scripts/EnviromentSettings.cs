using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpatialSys.UnitySDK;

public class EnviromentSettings : MonoBehaviour
{
    void Start()
    {
        //https://toolkit.spatial.io/docs/components/environment-settings-overrides

        SpatialEnvironmentSettingsOverrides settings = new SpatialEnvironmentSettingsOverrides();
        settings.environmentSettings.disableTeleport = false;
        settings.environmentSettings.virtualizeMouseClickInXR = true;
        
    }

}
