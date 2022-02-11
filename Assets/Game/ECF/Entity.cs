using System.Collections.Generic;

namespace RollBackExample
{
    public class Entity
    {
        public int Id;

        public List<Component> components = new List<Component>();
        
        public Body body;
        
        public bool receivesInput;
        public ulong input;
        public int inputIndex;
        public bool InputPressed(EInputTypes inputType) {

            return (input & (ulong)inputType) == (ulong)inputType;
        }
        public void GetComponent<T>(ref T retVal)
        {
            foreach (var item in components)
            {

                if (typeof(T).Equals(item.GetType()))
                {
                    retVal = (T)item;
                    return;
                }
            }

        }
    }
}