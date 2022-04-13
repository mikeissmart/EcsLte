namespace EcsLte.Native
{
    public struct EntityData_Native
    {
        /// <summary>
        /// [byte,byte],[TComponent,TComponent]
        /// </summary>
        public unsafe byte* AllComponents;
        public int ComponentCount;

        public unsafe bool GetHasComponent(int componentIndex) => *(AllComponents + componentIndex) == 1;

        public unsafe void SetHasComponent(int componentIndex, bool has) => *(AllComponents + componentIndex) = (byte)(has ? 1 : 0);

        public unsafe byte* Component(int offsetInBytes) => (AllComponents + ComponentConfigs.Instance.AllComponentCount) + offsetInBytes;
    }
}
