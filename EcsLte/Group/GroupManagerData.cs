using System.Collections.Generic;

namespace EcsLte
{
    internal class GroupManagerData
    {
        public List<Group>[] GroupComponentIndexes = new List<Group>[ComponentIndexes.Instance.Count];
        public Dictionary<Filter, Group> Groups = new Dictionary<Filter, Group>();

        public GroupManagerData()
        {
            for (var i = 0; i < GroupComponentIndexes.Length; i++)
                GroupComponentIndexes[i] = new List<Group>();
        }

        public void Reset()
        {
            for (var i = 0; i < GroupComponentIndexes.Length; i++)
                GroupComponentIndexes[i].Clear();
            Groups.Clear();
        }
    }
}