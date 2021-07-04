using RollBackExample;
using System.Collections.Generic;
using System.Linq;

public class RollbackWorld
{
    public List<Entity> entities = new List<Entity>();
    public List<Component> components= new List<Component>();
    public RollbackWorld()
    {
        World.StartLevel();
    }
    public void Update(ulong[] inputs)
    {
        UpdateEntities(inputs);
        UpdateComponents();
        World.Instance.Step();
    }

    private void UpdateComponents()
    {
        foreach (var component in components)
        {
            component.Update();
        }
    }

    private void UpdateEntities(ulong[] inputs)
    {
        foreach (var entity in entities)
        {
            if (entity.receivesInput)
            {
                entity.input = inputs[entity.inputIndex];
            }
        }
    }

    public void AddEntity(Entity entity)
    {
        entity.Id = entities.Count;
        entities.Add(entity);
        if (entity.body != null)
        {
            components.Add(entity.body);
            entity.components.Add(entity.body); 
            World.Instance.AddCapsule(entity.body);
        }
        foreach (var comp in entity.components)
        {
            comp.OwnerId = entity.Id;
            components.Add(comp);
            comp.Start(entity);
        }
     
        components = components.OrderBy(x => x.OwnerId).ThenBy(x=>x.ExecuteIndex).ToList();
    }
}
