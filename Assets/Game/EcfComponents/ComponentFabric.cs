﻿using RollBackExample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum EComponents
{
    GameStateComponent,
    MovementComponent,
    KillerSpike,
    AnimationComponent,
    HitComponent
}
public class ComponentFabric
{
    public static Component CreateComponent(EComponents component)
    {
        switch (component)
        {
            case EComponents.MovementComponent:
                return new MovementComponent();
            case EComponents.KillerSpike:
                return new KillerSpikeComponent();
            case EComponents.GameStateComponent:
                return new GameStateComponent();     
            case EComponents.AnimationComponent:
                return new AnimationComponent();        
            case EComponents.HitComponent:
                return new HitComponent();
        }
        return null;
    }
}
