﻿using UnityEngine;

public class ShootDelayArea : Area
{
    [SerializeField] float value = 0.1f;

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    protected override void ApplyEffect(GameObject player)
    {
        if (isUpgrade)
        {
            StatsManager.instance.UpgradeShootDelay(value);
        }
        else
        {
            StatsManager.instance.DowngradeShootDelay(value);
        }
    }
}
