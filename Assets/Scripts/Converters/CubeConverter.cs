using FixedMath;
using RollBackExample;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CubeConverter : MonoBehaviour
{
    public FixVector Position;
    public FixVector Scale;
    public FixVector Rotation;
    public List<EComponents> Components;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (Application.isEditor)
        {
            var pos = this.transform.position;
            pos.z = 0;
            this.transform.position = pos;
            Position = new FixVector(new FixedMath.Fix(this.transform.position.x), new FixedMath.Fix(this.transform.position.y), new FixedMath.Fix(this.transform.position.z));
            Scale = new FixVector(new FixedMath.Fix(this.transform.localScale.x / 2), new FixedMath.Fix(this.transform.localScale.y / 2), new FixedMath.Fix(this.transform.localScale.z / 2));
            Rotation = new FixVector(new FixedMath.Fix(this.transform.rotation.x), new FixedMath.Fix(this.transform.rotation.y), new FixedMath.Fix(this.transform.rotation.z));
            //if(this.transform.localScale.x!=1)
            Position.x = Position.x + (Scale.x / Fix._2);
            //if (this.transform.localScale.y != 1)
            Position.y = Position.y + (Scale.y / Fix._2);

        }
#endif
    }

    internal void Convert(RollbackWorld world)
    {
        var ent = new Entity();

        ent.body = new Rectangle { Owner = ent, enabled = true, dimensions = Scale };
        ent.body.position = Position;
        ent.inputIndex = -1;
        ent.receivesInput = false;
        foreach (var comp in Components)
        {
            ent.components.Add(ComponentFabric.CreateComponent(comp));
        }
        world.AddEntity(ent);

    }
}
