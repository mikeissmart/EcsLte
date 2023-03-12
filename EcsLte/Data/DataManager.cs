using System;
using System.Collections.Generic;
using System.Text;

namespace EcsLte.Data
{
    internal class DataManager : IDisposable
    {
        public static readonly int PageCapacity = 10000;
        public static readonly int MinBookPageCount = 10;

        private DataCatalog<UnmanagedDataBook<Entity>> _entityCatalog;
        private IDataCatalog[] _generalCatalogs;
        private IDataCatalog[] _managedCatalogs;

        internal DataManager()
        {
            _entityCatalog = new DataCatalog<UnmanagedDataBook<Entity>>();
            _generalCatalogs = new IDataCatalog[ComponentConfigs.Instance.AllGeneralCount];
            for (var i = 0; i < _generalCatalogs.Length; i++)
                _generalCatalogs[i] = ComponentConfigs.Instance.AllGeneralAdapters[i].CreateCatalog();
            _managedCatalogs = new IDataCatalog[ComponentConfigs.Instance.AllGeneralCount];
            for (var i = 0; i < _managedCatalogs.Length; i++)
                _managedCatalogs[i] = ComponentConfigs.Instance.AllManagedAdapters[i].CreateCatalog();
        }

        internal void DequeuePages(in ComponentConfigOffset[] generalConfigOffsets, in ComponentConfigOffset[] managedConfigOffsets,
            ref DataChunk[] chunks, int startingIndex, int count)
        {
            if (count == 0)
                return;

            _entityCatalog.CheckUnusedPages(count);
            for (var i = 0; i < generalConfigOffsets.Length; i++)
                _generalCatalogs[generalConfigOffsets[i].Config.GeneralIndex].CheckUnusedPages(count);
            for (var i = 0; i < managedConfigOffsets.Length; i++)
                _managedCatalogs[managedConfigOffsets[i].Config.ManagedIndex].CheckUnusedPages(count);

            for (var i = 0; i < count; i++)
            {
                var chunk = chunks[startingIndex + i];

                chunk.EntityPage = (UnmanagedDataPage<Entity>)_entityCatalog.DequeuePage();
                for (var j = 0; j < generalConfigOffsets.Length; j++)
                    chunk.GeneralComponentPages[j] = _generalCatalogs[generalConfigOffsets[j].Config.GeneralIndex].DequeuePage();
                for (var j = 0; j < managedConfigOffsets.Length; j++)
                    chunk.ManagedComponentPages[j] = _managedCatalogs[managedConfigOffsets[j].Config.ManagedIndex].DequeuePage();
            }
        }

        internal void EnqueuePages(in ComponentConfigOffset[] generalConfigOffsets, in ComponentConfigOffset[] managedConfigOffsets,
            ref DataChunk[] chunks, int startingIndex, int count)
        {
            for (var i = 0; i < count; i++)
            {
                var chunk = chunks[startingIndex + i];

                _entityCatalog.EnqueuePage(chunk.EntityPage);
                for (var j = 0; j < generalConfigOffsets.Length; j++)
                    _generalCatalogs[generalConfigOffsets[j].Config.GeneralIndex].EnqueuePage(chunk.GeneralComponentPages[j]);
                for (var j = 0; j < managedConfigOffsets.Length; j++)
                    _managedCatalogs[managedConfigOffsets[j].Config.ManagedIndex].EnqueuePage(chunk.ManagedComponentPages[j]);

                chunk.EntityPage = null;
                Array.Clear(chunk.GeneralComponentPages, 0, chunk.GeneralComponentPages.Length);
                Array.Clear(chunk.ManagedComponentPages, 0, chunk.ManagedComponentPages.Length);
            }
        }

        public void Dispose()
        {
            _entityCatalog.Dispose();
            _entityCatalog = null;
            for (var i = 0; i < _generalCatalogs.Length; i++)
                _generalCatalogs[i].Dispose();
            _generalCatalogs = null;
            for (var i = 0; i < _managedCatalogs.Length; i++)
                _managedCatalogs[i].Dispose();
            _managedCatalogs = null;
        }
    }
}
